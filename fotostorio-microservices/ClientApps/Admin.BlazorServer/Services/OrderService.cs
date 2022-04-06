using Admin.BlazorServer.Contracts;
using Admin.BlazorServer.DTOs;
using Admin.BlazorServer.Models;
using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace Admin.BlazorServer.Services;

public class OrderService : IOrderService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILogger<OrderService> _logger;
    private readonly ILocalStorageService _localStorage;

    public OrderService(IHttpClientFactory httpClient, ILogger<OrderService> logger, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _logger = logger;
        _localStorage = localStorage;
    }

    public async Task<List<OrderDetailsDTO>?> GetLatestOrdersAsync(int days)
    {
        var storedToken = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(storedToken))
        {
            return null;
        }

        try
        {
            var client = _httpClient.CreateClient("AdminGateway");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);

            var orders = await client.GetFromJsonAsync<List<OrderDetailsDTO>>($"Orders/Latest/{days}");

            return orders;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Status Code: {code}, Message: {message}", ex.StatusCode, ex.Message);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<OrderDetailsDTO?> GetOrderByIdAsync(int orderId)
    {
        var storedToken = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(storedToken))
        {
            return null;
        }

        try
        {
            var client = _httpClient.CreateClient("AdminGateway");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);

            var order = await client.GetFromJsonAsync<OrderDetailsDTO>($"Orders/{orderId}");

            return order;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Status Code: {code}, Message: {message}", ex.StatusCode, ex.Message);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }
}
