using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Basket.UnitTests.Repositories;

public class BasketRepositoryTests
{
    private readonly IDistributedCache _redisCache = Substitute.For<IDistributedCache>();
    private readonly int _fakeProductPrice = 50;

    [Fact]
    public async Task GetBasketAsync_ShouldCallGetStringAsyncOnRedisCache_WhenCalled()
    {
        // Arrange
        var basketId = "123";
        var expectedBasket = GetFakeCustomerBasket(basketId);
        var expectedBasketJson = JsonSerializer.Serialize(expectedBasket);

        var basketRepository = new BasketRepository(_redisCache);

        // Act
        await basketRepository.GetBasketAsync(basketId);

        // Assert
        await _redisCache.Received().GetStringAsync(basketId);
    }

    [Fact]
    public async Task GetBasketAsync_ShouldReturnNull_WhenBasketDoesNotExist()
    {
        // Arrange
        var basketId = "123";
        var expectedBasket = GetFakeCustomerBasket(basketId);
        var expectedBasketJson = JsonSerializer.Serialize(expectedBasket);

        var basketRepository = new BasketRepository(_redisCache);

        // Act
        var actualBasket = await basketRepository.GetBasketAsync(basketId);

        // Assert
        actualBasket.Should().BeNull();
    }

    [Fact]
    public async Task UpdateBasketAsync_ShouldCallMethodsOnRedisCache_WhenCalled()
    {
        // Arrange
        var basketId = "123";
        var expectedBasket = GetFakeCustomerBasket(basketId);
        var expectedBasketJson = JsonSerializer.Serialize(expectedBasket);

        var basketRepository = new BasketRepository(_redisCache);

        // Act
        await basketRepository.UpdateBasketAsync(expectedBasket);

        // Assert
        await _redisCache.ReceivedWithAnyArgs().SetStringAsync(basketId, expectedBasketJson, Arg.Any<DistributedCacheEntryOptions>(), Arg.Any<CancellationToken>());
        await _redisCache.Received().SetAsync(basketId, Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteBasketAsync_ShouldCallRemoveAsyncOnRedisCache_WhenCalled()
    {
        // Arrange
        var basketId = "123";
        var expectedBasket = GetFakeCustomerBasket(basketId);
        var expectedBasketJson = JsonSerializer.Serialize(expectedBasket);

        var basketRepository = new BasketRepository(_redisCache);

        // Act
        await basketRepository.DeleteBasketAsync(basketId);

        // Assert
        await _redisCache.Received().RemoveAsync(basketId);
    }

    private CustomerBasket GetFakeCustomerBasket(string basketId)
    {
        var basketItems = new List<BasketItem>();
        basketItems.Add(GetFakeBasketItem(1));
        basketItems.Add(GetFakeBasketItem(2));

        return new CustomerBasket()
        {
            Id = basketId,
            BasketItems = basketItems
        };
    }

    private BasketItem GetFakeBasketItem(int quantity)
    {
        return new BasketItem()
        {
            Quantity = quantity,
            Product = GetFakeProduct(1)
        };
    }

    private Product GetFakeProduct(int id)
    {
        return new Product()
        {
            Id = id,
            Sku = "fakeSku",
            Name = "fakeName",
            Price = _fakeProductPrice,
            ImageUrl = "fakeUrl",
            Brand = "fakeBrand",
            Category = "fakeCategory",
            Mount = "fakeMount"
        };
    }
}
