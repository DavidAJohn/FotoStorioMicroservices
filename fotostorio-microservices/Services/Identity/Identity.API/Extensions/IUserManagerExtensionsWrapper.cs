using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Identity.API.Extensions;

public interface IUserManagerExtensionsWrapper
{
    Task<AppUser> FindUserByClaimsPrincipalWithAddressAsync(UserManager<AppUser> userManager, ClaimsPrincipal principal);
}
