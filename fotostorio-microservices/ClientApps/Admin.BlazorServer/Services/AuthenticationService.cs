using Admin.BlazorServer.Contracts;
using Admin.BlazorServer.Models;
using Admin.BlazorServer.Providers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Admin.BlazorServer.Services;

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

    public async Task<LoginResult> Login(LoginModel loginModel)
    {
        if (loginModel == null)
        {
            return new LoginResult
            {
                Successful = false,
                Error = "Login failed"
            };
        }

        var client = _httpClient.CreateClient("IdentityAPI");
        var response = await client.PostAsJsonAsync("accounts/login", loginModel);

        var loginResult = JsonSerializer.Deserialize<LoginResult>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (loginResult == null)
        {
            return new LoginResult
            {
                Successful = false,
                Error = "Login failed"
            };
        }

        if (!response.IsSuccessStatusCode)
        {
            loginResult.Successful = false;
            loginResult.Error = "Invalid username or password";

            return loginResult;
        }

        loginResult.Successful = true;

        await _localStorage.SetItemAsync("authToken", loginResult.Token);

        if (loginModel.Email != null)
        {
            ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsAuthenticated(loginModel.Email);
        }

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
