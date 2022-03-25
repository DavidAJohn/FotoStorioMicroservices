using Admin.BlazorServer.Contracts;
using Admin.BlazorServer.Models;
using Blazored.LocalStorage;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Admin.BlazorServer.Services;

public class InventoryService : IInventoryService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILocalStorageService _localStorage;

    public InventoryService(IHttpClientFactory httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<List<ProductStock>?> GetInventoryAsync()
    {
        try
        {
            var client = _httpClient.CreateClient("AdminGateway");
            var productStock = await client.GetFromJsonAsync<List<ProductStock>>("Stock");

            return productStock;
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<List<ProductStock>?> GetInventoryAtOrBelowLevelAsync(int stockLevel)
    {
        try
        {
            var client = _httpClient.CreateClient("AdminGateway");
            var productStock = await client.GetFromJsonAsync<List<ProductStock>>($"Stock/{stockLevel}");
            
            return productStock;
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<UpdateStockResult?> CreateNewStock(ProductStock stock)
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

            HttpContent serializedContent = new StringContent(JsonSerializer.Serialize(stock));
            serializedContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsync("Stock", serializedContent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var createdUpdate = JsonSerializer.Deserialize<Update>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                var result = new UpdateStockResult
                {
                    Successful = true,
                    Error = ""
                };

                return result;
            }

            return null;
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<UpdateStockResult?> CreateNewUpdate(UpdateStockModel stockUpdate)
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

            HttpContent serializedContent = new StringContent(JsonSerializer.Serialize(stockUpdate));
            serializedContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsync("Updates", serializedContent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var createdUpdate = JsonSerializer.Deserialize<Update>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                var result = new UpdateStockResult
                {
                    Successful = true,
                    Error = ""
                };

                return result;
            }

            return null;
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }
}
