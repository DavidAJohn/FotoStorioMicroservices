using Blazored.LocalStorage;
using Store.BlazorWasm.Contracts;
using Store.BlazorWasm.DTOs;
using Store.BlazorWasm.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Store.BlazorWasm.Services;

public class AccountService : IAccountService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILocalStorageService _localStorage;

    public AccountService(IHttpClientFactory httpClient, ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public async Task<AddressDTO> GetUserAddressAsync()
    {
        var storedToken = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(storedToken))
        {
            return new AddressDTO { };
        }

        try
        {
            var client = _httpClient.CreateClient("IdentityAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);

            var address = await client.GetFromJsonAsync<AddressDTO>("accounts/address");

            if (address == null)
            {
                return new AddressDTO { };
            }

            return address;
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<AddressDTO> SaveUserAddressAsync(AddressDTO address)
    {
        var savedToken = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(savedToken))
        {
            return null;
        }

        try
        {
            var client = _httpClient.CreateClient("IdentityAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);

            HttpResponseMessage response = await client.PutAsJsonAsync<AddressDTO>("accounts/address", address);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(content))
                {
                    var newAddress = JsonSerializer.Deserialize<AddressDTO>(content, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (newAddress != null) return newAddress;

                    return new AddressDTO { };
                }

                return new AddressDTO { };
            }

            return new AddressDTO { };
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }
}
