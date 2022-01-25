using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Store.BlazorWasm.Contracts;
using Store.BlazorWasm.Models;
using Store.BlazorWasm.Providers;

namespace Store.BlazorWasm.Services;
public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;
    public AuthenticationService(IHttpClientFactory httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public async Task<RegisterResult> Register(RegisterModel registerModel)
    {
        var client = _httpClient.CreateClient("IdentityAPI");
        var response = await client.PostAsJsonAsync("accounts/register", registerModel);

        if (response.IsSuccessStatusCode)
        {
            return new RegisterResult
            {
                Successful = true
            };
        }

        return new RegisterResult
        {
            Successful = false,
            Errors = new[] { "Registration failed" }
        };
    }

    public async Task<LoginResult> Login(LoginModel loginModel)
    {
        var client = _httpClient.CreateClient("IdentityAPI");
        var response = await client.PostAsJsonAsync("accounts/login", loginModel);

        var loginResult = JsonSerializer.Deserialize<LoginResult>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (!response.IsSuccessStatusCode)
        {
            loginResult.Successful = false;
            loginResult.Error = "Invalid username or password";

            return loginResult;
        }

        loginResult.Successful = true;

        await _localStorage.SetItemAsync("authToken", loginResult.Token);

        ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsAuthenticated(loginModel.Email);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

        return loginResult;
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");

        ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsLoggedOut();

        var client = _httpClient.CreateClient("IdentityAPI");
        client.DefaultRequestHeaders.Authorization = null;
    }
}
