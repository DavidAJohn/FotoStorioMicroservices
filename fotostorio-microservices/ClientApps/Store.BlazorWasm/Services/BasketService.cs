using Blazored.LocalStorage;
using Microsoft.Net.Http.Headers;
using Store.BlazorWasm.Contracts;
using Store.BlazorWasm.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Store.BlazorWasm.Services;

public class BasketService : IBasketService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILogger<BasketService> _logger;
    private readonly IConfiguration _config;
    private readonly ILocalStorageService _localStorage;

    public BasketService(IHttpClientFactory httpClient, ILogger<BasketService> logger, IConfiguration config, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _logger = logger;
        _config = config;
        _localStorage = localStorage;
    }

    public async Task<Basket> GetBasketByID(string id)
    {
        try
        {
            var authToken = await _localStorage.GetItemAsync<string>("authToken");

            HttpClient client = _httpClient.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            HttpResponseMessage response = await client.GetAsync(_config["ApiSettings:StoreGatewayUri"] + "/Basket?id=" + id);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var basket = await response.Content.ReadFromJsonAsync<Basket>();

                if (basket == null)
                {
                    return new Basket { };
                }

                return basket;
            }

            return new Basket { };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"Basket could not be retrieved: {id}");
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<Basket> UpdateBasket(Basket basket)
    {
        try
        {
            var authToken = await _localStorage.GetItemAsync<string>("authToken");

            HttpClient client = _httpClient.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            HttpContent content = new StringContent(JsonSerializer.Serialize(basket));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsync(_config["ApiSettings:StoreGatewayUri"] + "/Basket", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Basket could not be updated: {basket.Id}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Basket>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.StatusCode + " " + ex.Message);
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task DeleteBasket(string id)
    {
        try
        {
            var authToken = await _localStorage.GetItemAsync<string>("authToken");

            HttpClient client = _httpClient.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            HttpResponseMessage response = await client.DeleteAsync(_config["ApiSettings:StoreGatewayUri"] + "/Basket?id=" + id);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Basket could not be deleted: {id}");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.StatusCode + " " + ex.Message);
            throw new HttpRequestException(ex.Message);
        }
    }
}
