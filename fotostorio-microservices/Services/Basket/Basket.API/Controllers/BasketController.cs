using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;
        private readonly DiscountGrpcService _discountGrpcService;

        public BasketController(IBasketRepository basketRepository, IMapper mapper, DiscountGrpcService discountGrpcService)
        {
            _mapper = mapper;
            _discountGrpcService = discountGrpcService;
            _basketRepository = basketRepository;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);

            return Ok(basket ?? new CustomerBasket(id));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket([FromBody] CustomerBasketDTO basket)
        {
            // for each basket item, check if there is a discounted price from the Discount.API gRPC service
            foreach (var item in basket.BasketItems)
            {
                var discount = await _discountGrpcService.GetDiscount(item.Product.Sku);

                // if discount.SalePrice = 0, item has no current discount
                item.Product.Price = (discount.SalePrice < item.Product.Price && discount.SalePrice != 0) 
                    ? discount.SalePrice : item.Product.Price;
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
}
