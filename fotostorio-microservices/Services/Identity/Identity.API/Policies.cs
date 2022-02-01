using Microsoft.AspNetCore.Authorization;

namespace Identity.API
{
    public static class Policies
    {
        public const string IsAdmin = "IsAdmin";
        public const string IsMarketing = "IsMarketing";
        public const string IsUser = "IsUser";

        public static AuthorizationPolicy IsAdminPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("Administrator")
                .Build();
        }

        public static AuthorizationPolicy IsMarketingPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("Marketing")
                .Build();
        }

        public static AuthorizationPolicy IsUserPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("User")
                .Build();
        }
    }
}
