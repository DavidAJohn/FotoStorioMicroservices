using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;
        private readonly IHttpClientFactory _httpClient;

        public BasketRepository(IDistributedCache redisCache, IHttpClientFactory httpClient)
        {
            _redisCache = redisCache;
            _httpClient = httpClient;
        }

        public async Task<CustomerBasket> GetBasketAsync(string basketId, string token)
        {
            // check that jwt is valid
            var validToken = await IsTokenValid(token);
            if (!validToken) return null;

            var basket = await _redisCache.GetStringAsync(basketId);

            return string.IsNullOrEmpty(basket) ? null : JsonSerializer.Deserialize<CustomerBasket>(basket);
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket, string token)
        {
            // check that jwt is valid
            var validToken = await IsTokenValid(token);
            if (!validToken) return null;

            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(30)
            };

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await _redisCache.SetStringAsync(basket.Id, JsonSerializer.Serialize(basket), options, source.Token);

            return await GetBasketAsync(basket.Id, token);
        }

        public async Task DeleteBasketAsync(string basketId, string token)
        {
            // check that jwt is valid
            var validToken = await IsTokenValid(token);
            if (validToken)
            {
                await _redisCache.RemoveAsync(basketId);
            }
        }

        private async Task<bool> IsTokenValid(string token)
        {
            var client = _httpClient.CreateClient("IdentityAPI");
            HttpContent serializedContent = new StringContent(JsonSerializer.Serialize(token));
            serializedContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage tokenResponse = await client.PostAsync("/api/accounts/token", serializedContent);

            if (!tokenResponse.IsSuccessStatusCode) return false;

            return true;
        }
    }
}
