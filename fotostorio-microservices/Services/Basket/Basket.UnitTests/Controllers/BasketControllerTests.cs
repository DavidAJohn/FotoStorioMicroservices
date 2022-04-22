using AutoMapper;
using Basket.API.Controllers;
using Basket.API.Entities;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Basket.UnitTests.Controllers;

public class BasketControllerTests : ControllerBase
{
    private readonly Mock<IBasketRepository> _basketRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;

    public BasketControllerTests()
    {
        _basketRepositoryMock = new Mock<IBasketRepository>();
        _mapperMock = new Mock<IMapper>();
    }

    [Fact]
    public async Task GetBasketById_success()
    {
        //Arrange
        var fakeBasketId = "1";
        var fakeBasket = GetFakeCustomerBasket(fakeBasketId);

        //Act
        var basketController = new BasketController(_basketRepositoryMock.Object, _mapperMock.Object, null);
        var actionResult = await basketController.GetBasketById(fakeBasketId);

        //Assert
        Assert.IsType<ActionResult<CustomerBasket>>(actionResult);
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
            Price = 50,
            ImageUrl = "fakeUrl",
            Brand = "fakeBrand",
            Category = "fakeCategory",
            Mount = "fakeMount"
        };
    }
}
