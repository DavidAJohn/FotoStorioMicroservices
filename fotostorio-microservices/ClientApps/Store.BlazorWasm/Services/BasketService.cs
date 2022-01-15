using Store.BlazorWasm.Contracts;
using Store.BlazorWasm.Models;
using System.Net.Http.Json;

namespace Store.BlazorWasm.Services;

public class BasketService : IBasketService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILogger<BasketService> _logger;

    public BasketService(IHttpClientFactory httpClient, ILogger<BasketService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Basket> GetBasketByID(string id)
    {
        try
        {
            var client = _httpClient.CreateClient("BasketAPI");
            var basket = await client.GetFromJsonAsync<Basket>($"Basket?id={id}");

            if (basket == null)
            {
                return new Basket{ };
            }

            return basket;
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
            var client = _httpClient.CreateClient("BasketAPI");
            var response = await client.PostAsJsonAsync($"Basket", basket);

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
            var client = _httpClient.CreateClient("BasketAPI");
            await client.DeleteAsync($"Basket?id={id}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.StatusCode + " " + ex.Message);
            throw new HttpRequestException(ex.Message);
        }
    }
}
