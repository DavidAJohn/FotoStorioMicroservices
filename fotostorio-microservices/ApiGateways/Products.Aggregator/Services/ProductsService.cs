using Products.Aggregator.Extensions;
using Products.Aggregator.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Products.Aggregator.Services
{
    public class ProductsService : IProductsService
    {
        private readonly HttpClient _httpClient;

        public ProductsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ProductResponse>> GetProducts()
        {
            var response = await _httpClient.GetAsync("/api/products");

            return await response.ReadContentAs<List<ProductResponse>>();
        }
    }
}
