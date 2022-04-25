namespace Ordering.API.Contracts;

public interface IHttpContextService
{
    string GetJwtFromContext(HttpContext context);
    string GetClaimValueByType(HttpContext context, string claimType);
}
