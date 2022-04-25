using Ordering.API.Extensions;

namespace Ordering.API.Services;

public class HttpContextService : IHttpContextService
{
    public HttpContextService()
    {
    }

    public string GetJwtFromContext(HttpContext context)
    {
        return context.GetJwtFromContext();
    }

    public string GetClaimValueByType(HttpContext context, string claimType)
    {
        return context.GetClaimValueByType(claimType);
    }
}
