using Store.BlazorWasm.Contracts;
using Store.BlazorWasm.Models;
using System.Net.Http.Json;

namespace Store.BlazorWasm.Services;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IHttpClientFactory httpClient, ILogger<ProductService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        try
        {
            var client = _httpClient.CreateClient("CatalogAPI");
            var products = await client.GetFromJsonAsync<List<Product>>("Catalog");

            return products;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.StatusCode + " " + ex.Message);
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        try
        {
            var client = _httpClient.CreateClient("CatalogAPI");
            var product = await client.GetFromJsonAsync<Product>($"Catalog/{id}");

            return product;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.StatusCode + " " + ex.Message);
            throw new HttpRequestException(ex.Message);
        }
    }
}
