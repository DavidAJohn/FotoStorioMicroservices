using AutoMapper;
using Basket.API.Controllers;
using Basket.API.Entities;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute.ReturnsExtensions;
using Basket.API.GrpcServices;
using Discount.Grpc.Protos;

namespace Basket.UnitTests.Controllers;

public class BasketControllerTests : ControllerBase
{
    private readonly IBasketRepository _basketRepository = Substitute.For<IBasketRepository>();
    private readonly IMapper _mapper = BuildMapper();

    private readonly int _fakeProductPrice = 50;
    private readonly int _fakeDiscountPrice = 40;

    [Fact]
    public async Task GetBasketById_ShouldReturnCustomerBasket_WhenBasketIdExists()
    {
        //Arrange
        var fakeBasketId = "1";
        var fakeBasket = GetFakeCustomerBasket(fakeBasketId);

        //Act
        var basketController = new BasketController(_basketRepository, _mapper, null);
        _basketRepository.GetBasketAsync(fakeBasketId).Returns(fakeBasket);
        var result = (OkObjectResult)await basketController.GetBasketById(fakeBasketId);

        //Assert
        result.Value.Should().BeOfType<CustomerBasket>();
        result.Value.Should().BeEquivalentTo(fakeBasket);
    }

    [Fact]
    public async Task GetBasketById_ShouldReturnCustomerBasket_WhenBasketIdDoesNotExist()
    {
        //Arrange
        var fakeBasketId = "1";

        //Act
        var basketController = new BasketController(_basketRepository, _mapper, null);
        _basketRepository.GetBasketAsync(fakeBasketId).ReturnsNull();
        var result = (OkObjectResult)await basketController.GetBasketById(fakeBasketId);

        //Assert
        result.Value.Should().BeOfType<CustomerBasket>();
    }

    [Fact]
    public async Task UpdateBasket_ShouldReturnCustomerBasket_WhenBasketIdExistsAndDiscountPriceIsLower()
    {
        //Arrange
        var fakeBasketId = "1";
        var fakeBasket = GetFakeCustomerBasket(fakeBasketId);
        var fakeBasketDTO = GetFakeCustomerBasketDTO(fakeBasketId);

        //Act
        var discountGrpcService = Substitute.For<IDiscountGrpcService>();
        discountGrpcService.GetDiscount(Arg.Any<string>())
            .Returns(new DiscountModel { Id = 1, Sku = "fakeSku", SalePrice = _fakeDiscountPrice });

        var basketController = new BasketController(_basketRepository, _mapper, discountGrpcService);

        _basketRepository.GetBasketAsync(Arg.Any<string>()).Returns(fakeBasket);
        _basketRepository.UpdateBasketAsync(Arg.Any<CustomerBasket>()).Returns(fakeBasket);

        var result = (OkObjectResult)await basketController.UpdateBasket(fakeBasketDTO);

        //Assert
        result.Value.Should().BeOfType<CustomerBasket>();
        result.Value.Should().BeEquivalentTo(fakeBasket);
    }

    [Fact]
    public async Task UpdateBasket_ShouldReturnCustomerBasket_WhenBasketIdExistsWithNoDiscountPrice()
    {
        //Arrange
        var fakeBasketId = "1";
        var fakeBasket = GetFakeCustomerBasket(fakeBasketId);
        var fakeBasketDTO = GetFakeCustomerBasketDTO(fakeBasketId);

        //Act
        var discountGrpcService = Substitute.For<IDiscountGrpcService>();
        discountGrpcService.GetDiscount(Arg.Any<string>())
            .Returns(new DiscountModel { Id = 1, Sku = "fakeSku", SalePrice = _fakeProductPrice });

        var basketController = new BasketController(_basketRepository, _mapper, discountGrpcService);

        _basketRepository.GetBasketAsync(Arg.Any<string>()).Returns(fakeBasket);
        _basketRepository.UpdateBasketAsync(Arg.Any<CustomerBasket>()).Returns(fakeBasket);

        var result = (OkObjectResult)await basketController.UpdateBasket(fakeBasketDTO);

        //Assert
        result.Value.Should().BeOfType<CustomerBasket>();
        result.Value.Should().BeEquivalentTo(fakeBasket);
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

    private CustomerBasketDTO GetFakeCustomerBasketDTO(string basketId)
    {
        var basketItems = new List<BasketItem>();
        basketItems.Add(GetFakeBasketItem(1));
        basketItems.Add(GetFakeBasketItem(2));

        return new CustomerBasketDTO()
        {
            Id = basketId,
            BasketItems = basketItems
        };
    }

    private static IMapper BuildMapper()
    {
        var config = new MapperConfiguration(options =>
        {
            options.AddProfile(new AutoMapperProfiles());
        });

        return config.CreateMapper();
    }
}
