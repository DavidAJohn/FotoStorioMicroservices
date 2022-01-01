using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
        }

        public async Task<CustomerBasket> GetBasketAsync(string basketId)
        {
            var basket = await _redisCache.GetStringAsync(basketId);

            return string.IsNullOrEmpty(basket) ? null : JsonSerializer.Deserialize<CustomerBasket>(basket);
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(30)
            };

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await _redisCache.SetStringAsync(basket.Id, JsonSerializer.Serialize(basket), options, source.Token);

            return await GetBasketAsync(basket.Id);
        }

        public async Task DeleteBasketAsync(string basketId)
        {
            await _redisCache.RemoveAsync(basketId);
        }
    }
}
