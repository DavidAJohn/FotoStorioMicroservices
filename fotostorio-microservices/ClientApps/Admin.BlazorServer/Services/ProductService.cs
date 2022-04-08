using Admin.BlazorServer.Contracts;
using Admin.BlazorServer.DTOs;
using Admin.BlazorServer.Models;
using Blazored.LocalStorage;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Admin.BlazorServer.Services;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILogger<ProductService> _logger;
    private readonly ILocalStorageService _localStorage;

    public ProductService(IHttpClientFactory httpClient, ILogger<ProductService> logger, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _logger = logger;
        _localStorage = localStorage;
    }

    public async Task<List<Brand>?> GetProductBrandsAsync()
    {
        try
        {
            var client = _httpClient.CreateClient("AdminGateway");
            var brands = await client.GetFromJsonAsync<List<Brand>>("Brands");

            return brands;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Status Code: {code}, Message: {message}", ex.StatusCode, ex.Message);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<List<Category>?> GetProductCategoriesAsync()
    {
        try
        {
            var client = _httpClient.CreateClient("AdminGateway");
            var categories = await client.GetFromJsonAsync<List<Category>>("Categories");

            return categories;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Status Code: {code}, Message: {message}", ex.StatusCode, ex.Message);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<List<Mount>?> GetProductMountsAsync()
    {
        try
        {
            var client = _httpClient.CreateClient("AdminGateway");
            var mounts = await client.GetFromJsonAsync<List<Mount>>("Mounts");

            return mounts;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Status Code: {code}, Message: {message}", ex.StatusCode, ex.Message);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<ProductDTO?> CreateProductAsync(ProductCreateDTO productCreateDTO)
    {
        if (productCreateDTO == null)
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
            var client = _httpClient.CreateClient("AdminGateway");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);

            HttpContent serializedContent = new StringContent(JsonSerializer.Serialize(productCreateDTO));
            serializedContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsync("Admin/Catalog", serializedContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var createdProduct = JsonSerializer.Deserialize<ProductDTO>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                return createdProduct;
            }

            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Status Code: {code}, Message: {message}", ex.StatusCode, ex.Message);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }
}
