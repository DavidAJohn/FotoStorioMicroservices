using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Products.Aggregator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Products.Aggregator.Services
{
    public class ProductsService : IProductsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductsService> _logger;

        public ProductsService(HttpClient httpClient, ILogger<ProductsService> logger)
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

                HttpResponseMessage response = await _httpClient.SendAsync(request);

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

                    return pagedResponse;
                }

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
                var response = await _httpClient.GetAsync($"/api/products/{id}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var product = JsonSerializer.Deserialize<ProductResponse>
                        (content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return product;
                }

                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error in Products Service -> GetProductByIdAsync, Id = {id}");
                throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
            }
        }

        public async Task<ProductResponse> GetProductBySkuAsync(string sku)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/products/sku/{sku}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var product = JsonSerializer.Deserialize<ProductResponse>
                        (content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return product;
                }

                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error in Products Service -> GetProductBySkuAsync, Sku = {sku}");
                throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
            }
        }
    }
}
