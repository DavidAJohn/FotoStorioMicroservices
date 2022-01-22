using Microsoft.AspNetCore.WebUtilities;
using Store.BlazorWasm.Contracts;
using Store.BlazorWasm.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

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

    public async Task<PagedList<Product>> GetProductsAsync(ProductParameters productParams)
    {
        try
        {
            var request = new HttpRequestMessage();

            if (productParams != null)
            {
                var queryStringParams = new Dictionary<string, string>
                {
                    ["pageIndex"] = productParams.PageIndex < 1 ? "1" : productParams.PageIndex.ToString(),
                };

                // conditionally add a page size param (number of items to return)
                if (productParams.PageSize != 0)
                {
                    queryStringParams.Add("pageSize", productParams.PageSize.ToString());
                };

                // conditionally add a search term
                if (!String.IsNullOrEmpty(productParams.Search))
                {
                    queryStringParams.Add("search", productParams.Search.ToString());
                };

                // conditionally add a categoryId param
                if (productParams.CategoryId != 0)
                {
                    queryStringParams.Add("categoryId", productParams.CategoryId.ToString());
                };

                // conditionally add a brandId param
                if (productParams.BrandId != 0)
                {
                    queryStringParams.Add("brandId", productParams.BrandId.ToString());
                };

                // conditionally add a sort param
                if (!String.IsNullOrEmpty(productParams.SortBy))
                {
                    queryStringParams.Add("sort", productParams.SortBy.ToString());
                };

                request = new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString("Catalog", queryStringParams));
            }
            else
            {
                request = new HttpRequestMessage(HttpMethod.Get, "Catalog");
            }

            var client = _httpClient.CreateClient("CatalogAPI");
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();

                var pagedResponse = new PagedList<Product>
                {
                    Items = JsonSerializer.Deserialize<List<Product>>(
                        content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    ),
                    Metadata = JsonSerializer.Deserialize<PagingMetadata>(
                        response.Headers.GetValues("Pagination")
                        .First(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    )
                };

                return pagedResponse;
            }

            return null;
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
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

    public async Task<List<Product>> GetProductsOnSpecialOfferAsync(string sortBy = "priceDesc")
    {
        try
        {
            var client = _httpClient.CreateClient("CatalogAPI");
            var products = await client.GetFromJsonAsync<List<Product>>($"Catalog/SpecialOffers?sort={sortBy}");

            return products;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.StatusCode + " " + ex.Message);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }
}
