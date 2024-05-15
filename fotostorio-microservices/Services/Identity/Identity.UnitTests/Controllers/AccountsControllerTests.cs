using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Identity.UnitTests.Controllers;

public class AccountsControllerTests
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AppUser _user;
    private readonly AppUser _noAddressUser;
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

        _noAddressUser = new AppUser { Email = "test@test.com", DisplayName = "Test User With No Address" };

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

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenEmailAddressAlreadyExists()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            DisplayName = "Test User",
            Email = "test@test.com",
            Password = "Test1234",
            ConfirmPassword = "Test1234"
        };

        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns(_user);

        // Act
        var result = (BadRequestObjectResult)await _accountsController.Register(registerModel);

        // Assert
        result.StatusCode.Should().Be(400);
        result.Value.Should().BeOfType<RegisterResult>();
        ((RegisterResult)result.Value).Successful.Should().BeFalse();
        //((RegisterResult)result.Value).Error.Should().Be("The email address is already in use");
        ((RegisterResult)result.Value).Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenUserCouldNotBeCreated()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            DisplayName = "Test User",
            Email = "test@test.com",
            Password = "Test1234",
            ConfirmPassword = "Test1234"
        };

        _userManager.CreateAsync(Arg.Any<AppUser>(), registerModel.Password)
            .Returns(IdentityResult.Failed());

        // Act
        var result = (BadRequestResult)await _accountsController.Register(registerModel);

        // Assert
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenRoleCouldNotBeAdded()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            DisplayName = "Test User",
            Email = "test@test.com",
            Password = "Test1234",
            ConfirmPassword = "Test1234"
        };

        _userManager.CreateAsync(Arg.Any<AppUser>(), registerModel.Password)
            .Returns(IdentityResult.Success);

        _userManager.AddToRoleAsync(Arg.Any<AppUser>(), "User")
            .Returns(IdentityResult.Failed());

        // Act
        var result = (BadRequestResult)await _accountsController.Register(registerModel);

        // Assert
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenPasswordIsEmpty()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            DisplayName = "Test User",
            Email = "test@test.com",
            Password = "",
            ConfirmPassword = "Test1234"
        };

        _userManager.CreateAsync(Arg.Any<AppUser>(), registerModel.Password).Returns(IdentityResult.Failed());

        // Act
        var result = (BadRequestObjectResult)await _accountsController.Register(registerModel);

        // Assert
        result.StatusCode.Should().Be(400);
        result.Value.Should().BeOfType<RegisterResult>();
        ((RegisterResult)result.Value).Successful.Should().BeFalse();
        //((RegisterResult)result.Value).Error.Should().Be("Password fields must be complete and matching");
        ((RegisterResult)result.Value).Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            DisplayName = "Test User",
            Email = "test@test.com",
            Password = "Test1234",
            ConfirmPassword = "Test1235"
        };

        _userManager.CreateAsync(Arg.Any<AppUser>(), registerModel.Password).Returns(IdentityResult.Failed());

        // Act
        var result = (BadRequestObjectResult)await _accountsController.Register(registerModel);

        // Assert
        result.StatusCode.Should().Be(400);
        result.Value.Should().BeOfType<RegisterResult>();
        ((RegisterResult)result.Value).Successful.Should().BeFalse();
        //((RegisterResult)result.Value).Error.Should().Be("Passwords do not match");
        ((RegisterResult)result.Value).Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            DisplayName = "Test User",
            Email = "test@test.com",
            Password = "Test1234",
            ConfirmPassword = "Test1234"
        };

        _userManager.CreateAsync(Arg.Any<AppUser>(), registerModel.Password)
            .Returns(IdentityResult.Success);

        _userManager.AddToRoleAsync(Arg.Any<AppUser>(), "User")
            .Returns(IdentityResult.Success);

        // Act
        var result = (OkObjectResult)await _accountsController.Register(registerModel);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<RegisterResult>();
        ((RegisterResult)result.Value).Successful.Should().BeTrue();
    }

    [Fact]
    public async Task GetUserAddress_ShouldReturnUserAddressDTO_WhenUserIsAuthenticated()
    {
        // Arrange
        IHttpContextAccessor httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext();
        httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, "test@test.com")
        }));

        var store = Substitute.For<IUserStore<AppUser>>();
        var userManager = Substitute.For<UserManager<AppUser>>(store, null, null, null, null, null, null, null, null);
        var signInManager = Substitute.For<SignInManager<AppUser>>(userManager, httpContextAccessor, Substitute.For<IUserClaimsPrincipalFactory<AppUser>>(), null, null, null, null);

        var sut = new AccountsController(
            userManager,
            signInManager,
            _tokenService,
            _logger,
            _mapper,
            httpContextAccessor,
            _userManagerExtensionsWrapper
        );

        _userManagerExtensionsWrapper
            .FindUserByClaimsPrincipalWithAddressAsync(Arg.Any<UserManager<AppUser>>(), Arg.Any<ClaimsPrincipal>())
            .Returns(_user);

        // Act
        var result = (OkObjectResult)await sut.GetUserAddress();

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<AddressDTO>();
        result.Value.Should().BeEquivalentTo(_mapper.Map<Address, AddressDTO>(_user.Address));
    }

    [Fact]
    public async Task GetUserAddress_ShouldReturnEmptyAddressDTO_WhenUserIsAuthenticatedAndHasNoAddress()
    {
        // Arrange
        IHttpContextAccessor httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext();
        httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, "test@test.com")
        }));

        var store = Substitute.For<IUserStore<AppUser>>();
        var userManager = Substitute.For<UserManager<AppUser>>(store, null, null, null, null, null, null, null, null);
        var signInManager = Substitute.For<SignInManager<AppUser>>(userManager, httpContextAccessor, Substitute.For<IUserClaimsPrincipalFactory<AppUser>>(), null, null, null, null);

        var sut = new AccountsController(
            userManager,
            signInManager,
            _tokenService,
            _logger,
            _mapper,
            httpContextAccessor,
            _userManagerExtensionsWrapper
        );

        _userManagerExtensionsWrapper
            .FindUserByClaimsPrincipalWithAddressAsync(Arg.Any<UserManager<AppUser>>(), Arg.Any<ClaimsPrincipal>())
            .Returns(_noAddressUser);

        // Act
        var result = (OkObjectResult)await sut.GetUserAddress();

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<AddressDTO>();
    }

    [Fact]
    public async Task GetUserAddress_ShouldReturnNotFound_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _httpContextAccessor.HttpContext = new DefaultHttpContext();
        _httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        var sut = new AccountsController(
            _userManager,
            _signInManager,
            _tokenService,
            _logger,
            _mapper,
            _httpContextAccessor,
            _userManagerExtensionsWrapper
        );

        // Act
        var result = (NotFoundObjectResult)await sut.GetUserAddress();

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetUserAddress_ShouldReturnEmptyAddressDTO_WhenUserIsNotFound()
    {
        // Arrange
        _httpContextAccessor.HttpContext = new DefaultHttpContext();
        _httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, "")
        }));

        var sut = new AccountsController(
            _userManager,
            _signInManager,
            _tokenService,
            _logger,
            _mapper,
            _httpContextAccessor,
            _userManagerExtensionsWrapper
        );

        //_userManagerExtensionsWrapper.FindUserByClaimsPrincipalWithAddressAsync();

        // Act
        var result = (NotFoundObjectResult)await sut.GetUserAddress();

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetUserAddress_ShouldReturnNotFoundWithEmptyAddressDTO_WhenExceptionIsThrown()
    {
        // Arrange
        _httpContextAccessor.HttpContext = new DefaultHttpContext();
        _httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        var sut = new AccountsController(
            _userManager,
            _signInManager,
            _tokenService,
            _logger,
            _mapper,
            _httpContextAccessor,
            _userManagerExtensionsWrapper
        );

        _userManagerExtensionsWrapper
            .FindUserByClaimsPrincipalWithAddressAsync(Arg.Any<UserManager<AppUser>>(), Arg.Any<ClaimsPrincipal>())
            .ThrowsAsync(new Exception("Simulated exception"));

        // Act
        var result = (NotFoundObjectResult)await sut.GetUserAddress();

        // Assert
        result.StatusCode.Should().Be(404);
        result.Value.Should().BeOfType<AddressDTO>();
        result.Value.Should().BeEquivalentTo(new AddressDTO());
    }

    [Fact]
    public async Task UpdateUserAddress_ShouldUpdateUserAddress_WhenAddressUpdateIsSuccessful()
    {
        // Arrange
        IHttpContextAccessor httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext();
        httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, "test@test.com")
        }));

        var store = Substitute.For<IUserStore<AppUser>>();
        var userManager = Substitute.For<UserManager<AppUser>>(store, null, null, null, null, null, null, null, null);
        var signInManager = Substitute.For<SignInManager<AppUser>>(userManager, httpContextAccessor, Substitute.For<IUserClaimsPrincipalFactory<AppUser>>(), null, null, null, null);

        var sut = new AccountsController(
            userManager,
            signInManager,
            _tokenService,
            _logger,
            _mapper,
            httpContextAccessor,
            _userManagerExtensionsWrapper
        );

        var updatedAddressDTO = new AddressDTO
        {
            FirstName = "Updated Test",
            LastName = "User",
            Street = "Updated Street",
            SecondLine = "Test Village",
            City = "Test City",
            County = "Test County",
            PostCode = "TE57 1NG"
        };

        _userManagerExtensionsWrapper
            .FindUserByClaimsPrincipalWithAddressAsync(userManager, Arg.Any<ClaimsPrincipal>())
            .Returns(_user);

        userManager.UpdateAsync(_user).Returns(IdentityResult.Success);

        // Act
        var result = (OkObjectResult)await sut.UpdateUserAddress(updatedAddressDTO);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<AddressDTO>();
        result.Value.Should().BeEquivalentTo(updatedAddressDTO);
    }

    [Fact]
    public async Task UpdateUserAddress_ShouldUpdateUserAddress_WhenAddressUpdateIsNotSuccessful()
    {
        // Arrange
        IHttpContextAccessor httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext();
        httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, "test@test.com")
        }));

        var store = Substitute.For<IUserStore<AppUser>>();
        var userManager = Substitute.For<UserManager<AppUser>>(store, null, null, null, null, null, null, null, null);
        var signInManager = Substitute.For<SignInManager<AppUser>>(userManager, httpContextAccessor, Substitute.For<IUserClaimsPrincipalFactory<AppUser>>(), null, null, null, null);

        var sut = new AccountsController(
            userManager,
            signInManager,
            _tokenService,
            _logger,
            _mapper,
            httpContextAccessor,
            _userManagerExtensionsWrapper
        );

        var updatedAddressDTO = new AddressDTO
        {
            FirstName = "Updated Test",
            LastName = "User",
            Street = "Updated Street",
            SecondLine = "Test Village",
            City = "Test City",
            County = "Test County",
            PostCode = "TE57 1NG"
        };

        _userManagerExtensionsWrapper
            .FindUserByClaimsPrincipalWithAddressAsync(userManager, Arg.Any<ClaimsPrincipal>())
            .Returns(_user);

        userManager.UpdateAsync(_user).Returns(IdentityResult.Failed(Array.Empty<IdentityError>()));

        // Act
        var result = (OkObjectResult)await sut.UpdateUserAddress(updatedAddressDTO);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<AddressDTO>();
        result.Value.Should().BeEquivalentTo(new AddressDTO());
    }

    [Fact]
    public async Task UpdateUserAddress_ShouldReturnEmptyAddressDTO_WhenExceptionIsThrown()
    {
        // Arrange
        IHttpContextAccessor httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext();
        httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, "test@test.com")
        }));

        var store = Substitute.For<IUserStore<AppUser>>();
        var userManager = Substitute.For<UserManager<AppUser>>(store, null, null, null, null, null, null, null, null);
        var signInManager = Substitute.For<SignInManager<AppUser>>(userManager, httpContextAccessor, Substitute.For<IUserClaimsPrincipalFactory<AppUser>>(), null, null, null, null);

        var sut = new AccountsController(
            userManager,
            signInManager,
            _tokenService,
            _logger,
            _mapper,
            httpContextAccessor,
            _userManagerExtensionsWrapper
        );

        var updatedAddressDTO = new AddressDTO
        {
            FirstName = "Updated Test",
            LastName = "User",
            Street = "Updated Street",
            SecondLine = "Test Village",
            City = "Test City",
            County = "Test County",
            PostCode = "TE57 1NG"
        };

        _userManagerExtensionsWrapper
            .FindUserByClaimsPrincipalWithAddressAsync(userManager, Arg.Any<ClaimsPrincipal>())
            .ThrowsAsync(new Exception("Simulated exception"));

        userManager.UpdateAsync(_user).Returns(IdentityResult.Success);

        // Act
        var result = (OkObjectResult)await sut.UpdateUserAddress(updatedAddressDTO);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<AddressDTO>();
        result.Value.Should().BeEquivalentTo(new AddressDTO());
    }
}
