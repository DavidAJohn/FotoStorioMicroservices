using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Identity.API.Extensions;

public static class UserManagerExtensions
{
    public async static Task<AppUser> FindUserByClaimsPrincipalWithAddressAsync(this UserManager<AppUser> input, ClaimsPrincipal user)
    {
        var email = user?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        return await input.Users
            .Include(x => x.Address)
            .SingleOrDefaultAsync(x => x.Email == email);
    }
}
