using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccountsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, ILogger<AccountsController> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _tokenService = tokenService;
        _logger = logger;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    /// GET api/accounts/login
    /// <summary>
    /// Authenticates a user
    /// </summary>
    /// <returns>LoginResult object</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Login(LoginModel login)
    {
        var user = await _userManager.FindByEmailAsync(login.Email);

        if (user == null)
        {
            return Unauthorized();
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);

        if (!result.Succeeded)
        {
            return BadRequest(new LoginResult { Successful = false, Error = "Login failed" });
        }

        return Ok(
            new LoginResult
            {
                Successful = true,
                Token = await _tokenService.CreateToken(user)
            }
        );
    }

    /// GET api/accounts/register
    /// <summary>
    /// Creates a new user account
    /// </summary>
    /// <returns>RegisterResult object</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Register(RegisterModel register)
    {
        if (CheckEmailExistsAsync(register.Email).Result.Value)
        {
            return new BadRequestObjectResult(
                new RegisterResult
                {
                    Successful = false,
                    Errors = new[] { "The email address is already in use" }
                }
            );
        }

        var user = new AppUser
        {
            DisplayName = register.DisplayName,
            Email = register.Email,
            UserName = register.Email
        };

        var result = await _userManager.CreateAsync(user, register.Password);

        if (!result.Succeeded)
        {
            return BadRequest();
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "User");

        if (!roleResult.Succeeded)
        {
            return BadRequest();
        }

        return Ok(
            new RegisterResult
            {
                Successful = true
            }
        );
    }

    [HttpGet("emailexists")]
    public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    /// GET api/accounts/address
    /// <summary>
    /// Returns the authenticated user's default address
    /// </summary>
    /// <returns>AddressDTO object</returns>
    //[Authorize]
    [HttpGet("address")]
    public async Task<ActionResult<AddressDTO>> GetUserAddress()
    {
        try
        {
            var userEmail = _httpContextAccessor.HttpContext.GetClaimValueByType("email");
            var user = await _userManager.FindUserByEmailWithAddressAsync(userEmail);

            if (user == null)
            {
                return new AddressDTO { };
            }

            return Ok(_mapper.Map<Address, AddressDTO>(user.Address));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in AccountsController.GetUserAddress: {message}", ex.Message);

            return new AddressDTO { };
        }
    }

    /// PUT api/accounts/address
    /// <summary>
    /// Updates the authenticated user's default address
    /// </summary>
    /// <returns>AddressDTO object</returns>
    //[Authorize]
    [HttpPut("address")]
    public async Task<ActionResult<AddressDTO>> UpdateUserAddress(AddressDTO addressDTO)
    {
        AppUser user = null;

        try
        {
            var userEmail = _httpContextAccessor.HttpContext.GetClaimValueByType("email");
            user = await _userManager.FindUserByEmailWithAddressAsync(userEmail);
            user.Address = _mapper.Map<AddressDTO, Address>(addressDTO);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(_mapper.Map<Address, AddressDTO>(user.Address));
            }

            return new AddressDTO { };
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in AccountsController.UpdateUserAddress for user '{userId}': {message}", user.Id, ex.Message);

            return new AddressDTO { };
        }
    }

    /// GET api/accounts/token
    /// <summary>
    /// Checks if a token is valid
    /// </summary>
    /// <returns>bool</returns>
    [HttpPost("token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> ValidateToken([FromBody] string token)
    {
        var valid = await _tokenService.ValidateJwtToken(token);

        if (!valid)
        {
            return Unauthorized();
        }

        return Ok(valid);
    }
}
