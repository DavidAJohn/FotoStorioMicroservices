using System.Security.Claims;

namespace Admin.BlazorServer.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetDisplayNameFromPrincipal(this ClaimsPrincipal user)
    {
        string userName = "";

        foreach (var claim in user.Claims)
        {
            if (claim.Type.ToString() == "given_name")
            {
                userName = claim.Value;
            }
        }

        return userName;
    }

    public static string GetRoleFromPrincipal(this ClaimsPrincipal user)
    {
        string role = "";

        foreach (var claim in user.Claims)
        {
            if (claim.Type.ToString() == "role")
            {
                role = claim.Value;
            }
        }

        return role;
    }
    public static string GetEmailFromPrincipal(this ClaimsPrincipal user)
    {
        string email = "";

        foreach (var claim in user.Claims)
        {
            if (claim.Type.ToString() == "email")
            {
                email = claim.Value;
            }
        }

        return email;
    }
}
