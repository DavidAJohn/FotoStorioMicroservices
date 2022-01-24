using Store.BlazorWasm.Models;

namespace Store.BlazorWasm.Contracts;

public interface IAuthenticationService
{
    Task<LoginResult> Login(LoginModel loginModel);
    Task Logout();
    Task<RegisterResult> Register(RegisterModel registerModel);
}
