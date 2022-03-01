using Admin.BlazorServer.Models;

namespace Admin.BlazorServer.Contracts;

public interface IAuthenticationService
{
    Task<LoginResult> Login(LoginModel loginModel);
    Task Logout();
}
