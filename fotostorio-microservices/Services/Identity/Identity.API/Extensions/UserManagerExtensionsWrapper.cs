using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Identity.API.Models;

namespace Identity.API.Extensions;

public class UserManagerExtensionsWrapper : IUserManagerExtensionsWrapper
{
    public async Task<AppUser> FindUserByClaimsPrincipalWithAddressAsync(UserManager<AppUser> userManager, ClaimsPrincipal principal)
    {
        return await userManager.FindUserByClaimsPrincipalWithAddressAsync(principal);
    }
}
