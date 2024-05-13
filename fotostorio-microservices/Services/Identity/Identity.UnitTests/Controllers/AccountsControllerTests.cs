using AutoMapper;
using Identity.API.Contracts;
using Identity.API.Controllers;
using Identity.API.Extensions;
using Identity.API.Helpers;
using Identity.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Identity.UnitTests.Controllers;

public class AccountsControllerTests
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AppUser _user;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AccountsController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AccountsController _accountsController;
    private readonly IUserManagerExtensionsWrapper _userManagerExtensionsWrapper;

    public AccountsControllerTests()
    {
        _user = new AppUser { Email = "test@test.com", DisplayName = "Test User" };
        _user.Address = new Address
        {
            FirstName = "Test",
            LastName = "User",
            Street = "123 Test Street",
            SecondLine = "Test Village",
            City = "Test City",
            County = "Test County",
            PostCode = "TE57 1NG"
        };

        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        var store = Substitute.For<IUserStore<AppUser>>();
        _userManager = Substitute.For<UserManager<AppUser>>(store, null, null, null, null, null, null, null, null);
        _signInManager = Substitute.For<SignInManager<AppUser>>(_userManager, _httpContextAccessor, Substitute.For<IUserClaimsPrincipalFactory<AppUser>>(), null, null, null, null);

        _tokenService = Substitute.For<ITokenService>();
        _mapper = BuildMap();
        _logger = Substitute.For<ILogger<AccountsController>>();
        _userManagerExtensionsWrapper = Substitute.For<IUserManagerExtensionsWrapper>();

        _accountsController = new AccountsController(
            _userManager,
            _signInManager,
            _tokenService,
            _logger,
            _mapper,
            _httpContextAccessor,
            _userManagerExtensionsWrapper
        );
    }

    protected static IMapper BuildMap()
    {
        var config = new MapperConfiguration(options =>
        {
            options.AddProfile(new AutoMapperProfiles());
        });

        return config.CreateMapper();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserIsNull()
    {
        // Arrange
        var loginModel = new LoginModel { Email = "test@test.com", Password = "Test1234" };
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns((AppUser)null);

        // Act
        var result = await _accountsController.Login(loginModel);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenSignInFails()
    {
        // Arrange
        var loginModel = new LoginModel { Email = "test@test.com", Password = "" };
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns(_user);

        _signInManager.CheckPasswordSignInAsync(_user, Arg.Any<string>(), Arg.Any<bool>())
            .Returns(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var result = (BadRequestObjectResult)await _accountsController.Login(loginModel);

        // Assert
        result.StatusCode.Should().Be(400);
        result.Value.Should().BeOfType<LoginResult>();
        ((LoginResult)result.Value).Successful.Should().BeFalse();
        ((LoginResult)result.Value).Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenSignInSucceeds()
    {
        // Arrange
        var test_jwt_token = "testtoken";

        var loginModel = new LoginModel { Email = "test@test.com", Password = "Test1234" };
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns(_user);

        _signInManager.CheckPasswordSignInAsync(_user, Arg.Any<string>(), Arg.Any<bool>())
            .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _tokenService.CreateToken(_user).Returns(test_jwt_token);

        // Act
        var result = (OkObjectResult)await _accountsController.Login(loginModel);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<LoginResult>();
        ((LoginResult)result.Value).Successful.Should().BeTrue();
        ((LoginResult)result.Value).Token.Should().Be(test_jwt_token);
    }
}
