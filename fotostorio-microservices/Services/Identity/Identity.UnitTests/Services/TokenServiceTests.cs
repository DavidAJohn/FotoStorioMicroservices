using Identity.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace Identity.UnitTests.Services;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;
    private readonly IConfiguration _config;
    private readonly UserManager<AppUser> _userManager;
    private readonly AppUser _user;
    private readonly ILogger<TokenService> _logger;

    public TokenServiceTests()
    {
        var inMemoryConfig = new Dictionary<string, string> {
            {"JwtKey", "example:6XUgUR$@n#vgYK2amPzLz^mBsc&ZKX@&i9&jJvUJ&*ngA@%27ZByGAzP"},
            {"JwtIssuer", "http://testissuer"},
            {"JwtAudience", "http://testaudience"},
            {"JwtExpiryInDays", "1"},
        };

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfig)
            .Build();

        _user = new AppUser { Email = "test@test.com", DisplayName = "Test User" };

        var store = Substitute.For<IUserStore<AppUser>>();
        _userManager = Substitute.For<UserManager<AppUser>>(store, null, null, null, null, null, null, null, null);

        _logger = Substitute.For<ILogger<TokenService>>();

        _tokenService = new TokenService(_config, _userManager, _logger);
    }

    [Fact]
    public async Task CreateToken_ShouldReturnToken_WhenInvoked()
    {
        // Act
        var token = await _tokenService.CreateToken(_user);

        // Assert
        token.Should().BeOfType<string>();
        token.Should().NotBeNullOrEmpty();
        token.Length.Should().BeGreaterThan(330); // should mean that at least SHA512 was used
    }

    [Fact]
    public async Task CreateToken_ShouldReturnTokenWithClaims_WhenInvoked()
    {
        // Act
        var token = await _tokenService.CreateToken(_user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        securityToken.Should().NotBeNull();
        securityToken.Issuer.Should().Be(_config["JwtIssuer"]);
        securityToken.Audiences.Should().Contain(_config["JwtAudience"]);
        securityToken.Claims.Should().NotBeEmpty();
        securityToken.Claims.Should().Contain(claim => claim.Type == JwtRegisteredClaimNames.Email && claim.Value == _user.Email);
        securityToken.Claims.Should().Contain(claim => claim.Type == JwtRegisteredClaimNames.GivenName && claim.Value == _user.DisplayName);
    }

    [Fact]
    public async Task ValidateJwtToken_ShouldReturnTrue_WhenInvokedWithValidTokenAndUserEmailExists()
    {
        // Arrange
        var token = await _tokenService.CreateToken(_user);

        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns(_user);
        _userManager.IsLockedOutAsync(Arg.Any<AppUser>()).Returns(false);

        // Act
        var result = await _tokenService.ValidateJwtToken(token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateJwtToken_ShouldReturnFalse_WhenUserEmailDoesNotExist()
    {
        // Arrange
        var token = await _tokenService.CreateToken(_user);

        //_userManager.FindByEmailAsync(Arg.Any<string>()).Returns(_user);

        // Act
        var result = await _tokenService.ValidateJwtToken(token);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateJwtToken_ShouldReturnFalse_WhenInvokedWithInvalidToken()
    {
        // Arrange
        var token = await _tokenService.CreateToken(_user);
        token = token.Remove(token.Length - 1); // should invoke a SecurityTokenException

        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns(_user);
        _userManager.IsLockedOutAsync(Arg.Any<AppUser>()).Returns(false);

        // Act
        var result = await _tokenService.ValidateJwtToken(token);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateJwtToken_ShouldReturnFalse_WhenInvokedWithEditedToken()
    {
        // Arrange
        var token = await _tokenService.CreateToken(_user);
        token = token.Replace("e", "x"); // retains the same length of token

        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns(_user);
        _userManager.IsLockedOutAsync(Arg.Any<AppUser>()).Returns(false);

        // Act
        var result = await _tokenService.ValidateJwtToken(token);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateJwtToken_ShouldReturnFalse_WhenInvokedWithValidTokenButUserIsLockedOut()
    {
        // Arrange
        var token = await _tokenService.CreateToken(_user);

        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns(_user);
        _userManager.IsLockedOutAsync(Arg.Any<AppUser>()).Returns(true);

        // Act
        var result = await _tokenService.ValidateJwtToken(token);

        // Assert
        result.Should().BeFalse();
    }
}
