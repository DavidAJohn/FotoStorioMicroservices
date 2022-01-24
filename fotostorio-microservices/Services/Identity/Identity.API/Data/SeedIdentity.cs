﻿using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Data
{
    public class SeedIdentity
    {
        private static IConfiguration _config;

        public static async Task SeedUsersAndRolesAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _config = config;
            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
        }

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                if (!await roleManager.RoleExistsAsync("Administrator"))
                {
                    var role = new IdentityRole
                    {
                        Name = "Administrator"
                    };

                    await roleManager.CreateAsync(role);
                }

                if (!await roleManager.RoleExistsAsync("Marketing"))
                {
                    var role = new IdentityRole
                    {
                        Name = "Marketing"
                    };

                    await roleManager.CreateAsync(role);
                }

                if (!await roleManager.RoleExistsAsync("User"))
                {
                    var role = new IdentityRole
                    {
                        Name = "User"
                    };

                    await roleManager.CreateAsync(role);
                }
            }
        }

        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                // create an admin user
                string adminEmail = _config["AdminAuthentication:Email"];
                string adminPassword = _config["AdminAuthentication:Password"];

                var adminUser = new AppUser
                {
                    DisplayName = "Admin",
                    Email = adminEmail,
                    UserName = adminEmail,
                    Address = new Address
                    {
                        FirstName = "Admin",
                        LastName = "User",
                        Street = "1 Admin Avenue",
                        SecondLine = "-",
                        City = "-",
                        County = "-",
                        PostCode = "-"
                    }
                };

                var adminResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (adminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }

                // create a marketing user
                string marketingEmail = _config["MarketingAuthentication:Email"];
                string marketingPassword = _config["MarketingAuthentication:Password"];

                var marketingUser = new AppUser
                {
                    DisplayName = "Marketing",
                    Email = marketingEmail,
                    UserName = marketingEmail,
                    Address = new Address
                    {
                        FirstName = "Marketing",
                        LastName = "User",
                        Street = "1 Marketing Avenue",
                        SecondLine = "-",
                        City = "-",
                        County = "-",
                        PostCode = "-"
                    }
                };

                var marketingResult = await userManager.CreateAsync(marketingUser, marketingPassword);

                if (marketingResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(marketingUser, "Marketing");
                }

                // // create a standard user/customer for development
                // var user = new AppUser
                // {
                //     DisplayName = "David",
                //     Email = "david@test.com",
                //     UserName = "david@test.com",
                //     Address = new Address
                //     {
                //         FirstName = "David",
                //         LastName = "User",
                //         Street = "1 High Road",
                //         SecondLine = "-",
                //         City = "-",
                //         County = "London",
                //         PostCode = "SW9 1DJ"
                //     }
                // };

                // var userResult = await userManager.CreateAsync(user, "Pa$$w0rd");

                // if (userResult.Succeeded)
                // {
                //     await userManager.AddToRoleAsync(user, "User");
                // }
            }
        }
    }
}
