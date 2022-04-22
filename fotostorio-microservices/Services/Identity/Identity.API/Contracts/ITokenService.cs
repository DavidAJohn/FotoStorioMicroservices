namespace Identity.API.Contracts;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
    Task<bool> ValidateJwtToken(string token);
}
