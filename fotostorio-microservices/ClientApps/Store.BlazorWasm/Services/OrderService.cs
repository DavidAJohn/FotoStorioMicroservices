using Blazored.LocalStorage;
using Store.BlazorWasm.Contracts;
using Store.BlazorWasm.DTOs;
using Store.BlazorWasm.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Store.BlazorWasm.Services;

public class OrderService : IOrderService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILocalStorageService _localStorage;

    public OrderService(IHttpClientFactory httpClient, ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public async Task<OrderDTO> CreateOrderAsync(OrderCreateDTO order)
    {
        if (order == null)
        {
            return null;
        }

        var storedToken = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(storedToken))
        {
            return null;
        }

        try
        {
            var client = _httpClient.CreateClient("StoreGateway");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);

            HttpContent serializedContent = new StringContent(JsonSerializer.Serialize(order));
            serializedContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsync("Orders", serializedContent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var createdOrder = JsonSerializer.Deserialize<Order>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                var returnOrder = new OrderDTO
                {
                    OrderId = createdOrder.Id.ToString(),
                    SendToAddress = createdOrder.SendToAddress
                };

                return returnOrder;
            }

            return null;
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<OrderDetailsDTO> GetOrderByIdAsync(int orderId)
    {
        var storedToken = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(storedToken))
        {
            return null;
        }

        try
        {
            var client = _httpClient.CreateClient("StoreGateway");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);

            var order = await client.GetFromJsonAsync<OrderDetailsDTO>($"Orders/{orderId}");

            return order;
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<List<OrderDetailsDTO>> GetOrdersForUserAsync()
    {
        var storedToken = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(storedToken))
        {
            return null;
        }

        try
        {
            var client = _httpClient.CreateClient("StoreGateway");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);

            var orders = await client.GetFromJsonAsync<List<OrderDetailsDTO>>("Orders");

            return orders;
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }
}
