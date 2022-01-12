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

    public async Task<List<Product>> GetProductsByBrandAsync(int brandId)
    {
        try
        {
            var client = _httpClient.CreateClient("CatalogAPI");
            var products = await client.GetFromJsonAsync<List<Product>>($"Catalog?brandId={brandId}");

            return products;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.StatusCode + " " + ex.Message);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        try
        {
            var client = _httpClient.CreateClient("CatalogAPI");
            var products = await client.GetFromJsonAsync<List<Product>>($"Catalog?categoryId={categoryId}");

            return products;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.StatusCode + " " + ex.Message);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<List<Product>> GetProductsByMountAsync(int mountId)
    {
        try
        {
            var client = _httpClient.CreateClient("CatalogAPI");
            var products = await client.GetFromJsonAsync<List<Product>>($"Catalog?mountId={mountId}");

            return products;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.StatusCode + " " + ex.Message);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<List<Brand>> GetProductBrandsAsync()
    {
        try
        {
            var client = _httpClient.CreateClient("CatalogAPI");
            var brands = await client.GetFromJsonAsync<List<Brand>>("Brands");

            return brands;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.StatusCode + " " + ex.Message);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<List<Category>> GetProductCategoriesAsync()
    {
        try
        {
            var client = _httpClient.CreateClient("CatalogAPI");
            var categories = await client.GetFromJsonAsync<List<Category>>("Categories");

            return categories;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.StatusCode + " " + ex.Message);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<List<Mount>> GetProductMountsAsync()
    {
        try
        {
            var client = _httpClient.CreateClient("CatalogAPI");
            var mounts = await client.GetFromJsonAsync<List<Mount>>("Mounts");

            return mounts;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.StatusCode + " " + ex.Message);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }
}
