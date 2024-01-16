using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;
    private readonly IMapper _mapper;
    private readonly IDiscountGrpcService _discountGrpcService;

    public BasketController(IBasketRepository basketRepository, IMapper mapper, IDiscountGrpcService discountGrpcService)
    {
        _mapper = mapper;
        _discountGrpcService = discountGrpcService;
        _basketRepository = basketRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetBasketById(string id)
    {
        var basket = await _basketRepository.GetBasketAsync(id);

        return Ok(basket ?? new CustomerBasket(id));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateBasket([FromBody] CustomerBasketDTO basket)
    {
        // for each basket item, check if there is a discounted price from the Discount.Grpc service
        foreach (var item in basket.BasketItems)
        {
            var discount = await _discountGrpcService.GetDiscount(item.Product.Sku);

            // if discount.SalePrice = 0, item has no current discount
            item.Product.Price = ((decimal)discount.SalePrice < item.Product.Price && (decimal)discount.SalePrice != 0) 
                ? (decimal)discount.SalePrice : item.Product.Price;
        }

        var customerBasket = _mapper.Map<CustomerBasketDTO, CustomerBasket>(basket);
        var updatedBasket = await _basketRepository.UpdateBasketAsync(customerBasket);

        return Ok(updatedBasket);
    }

    [HttpDelete]
    public async Task DeleteBasket(string id)
    {
        await _basketRepository.DeleteBasketAsync(id);
    }
}
