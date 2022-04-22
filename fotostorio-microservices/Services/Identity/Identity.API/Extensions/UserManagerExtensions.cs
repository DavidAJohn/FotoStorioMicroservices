using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Extensions;

public static class UserManagerExtensions
{
    /// <summary>
    ///  Finds an AppUser (and their postal address) from their email address, using Identity.UserManager
    /// </summary>
    /// <returns>An AppUser object</returns>
    public static async Task<AppUser> FindUserByEmailWithAddressAsync(this UserManager<AppUser> userManager, string userEmail)
    {
        return await userManager.Users
            .Include(u => u.Address)
            .SingleOrDefaultAsync(u => u.Email == userEmail);
    }
}
