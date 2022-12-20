using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Net;
using System.Text.Json;

namespace Products.Aggregator.Services;

public class ProductsService : IProductsService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILogger<ProductsService> _logger;

    public ProductsService(IHttpClientFactory httpClient, ILogger<ProductsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedList<ProductResponse>> GetProductsAsync(ProductParameters productParams)
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

                // conditionally add a mountId param
                if (productParams.MountId != 0)
                {
                    queryStringParams.Add("mountId", productParams.MountId.ToString());
                };

                // conditionally add a sort param
                if (!String.IsNullOrEmpty(productParams.Sort))
                {
                    queryStringParams.Add("sort", productParams.Sort.ToString());
                };

                request = new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString("/api/products", queryStringParams));
            }
            else
            {
                request = new HttpRequestMessage(HttpMethod.Get, "/api/products");
            }

            var client = _httpClient.CreateClient("Products");
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();

                var pagedResponse = new PagedList<ProductResponse>
                {
                    Items = JsonSerializer.Deserialize<List<ProductResponse>>(
                        content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    ),
                    Metadata = JsonSerializer.Deserialize<PagingMetadata>(
                        response.Headers.GetValues("Pagination")
                        .First(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    )
                };

                _logger.LogInformation("Products Service -> Successfully retrieved products");
                return pagedResponse;
            }

            _logger.LogWarning("Products Service -> Response when attempting to GET products was '{statuscode}', rather than OK", response.StatusCode.ToString());
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError("Error in Products Service -> GetProductsAsync");
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<ProductResponse> GetProductByIdAsync(int id)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/products/{id}");

            var client = _httpClient.CreateClient("Products");
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductResponse>
                    (content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _logger.LogInformation("Products Service -> Successfully retrieved product id '{id}'", id);
                return product;
            }

            _logger.LogWarning("Products Service -> Response when attempting to GET product id '{id}' was '{statuscode}', rather than OK", id, response.StatusCode.ToString());
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError("Error in Products Service -> GetProductByIdAsync, Id = {id}", id);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }

    public async Task<ProductResponse> GetProductBySkuAsync(string sku)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/products/sku/{sku}");

            var client = _httpClient.CreateClient("Products");
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductResponse>
                    (content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _logger.LogInformation("Products Service -> Successfully retrieved product sku '{sku}'", sku);
                return product;
            }

            _logger.LogWarning("Products Service -> Response when attempting to GET product sku '{sku}' was '{statuscode}', rather than OK", sku, response.StatusCode.ToString());
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError("Error in Products Service -> GetProductBySkuAsync, Sku = {sku}", sku);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }
}
