using Products.Aggregator.Extensions;
using Products.Aggregator.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Products.Aggregator.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly HttpClient _httpClient;

        public DiscountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DiscountResponse> GetDiscountBySku(string sku)
        {
            var response = await _httpClient.GetAsync($"/api/discounts/sku/{sku}");

            return await response.ReadContentAs<DiscountResponse>();
        }
    }
}
