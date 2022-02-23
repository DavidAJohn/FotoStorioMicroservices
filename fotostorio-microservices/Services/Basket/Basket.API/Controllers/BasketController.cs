using AutoMapper;
using Basket.API.Entities;
using Basket.API.Extensions;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BasketController(IBasketRepository basketRepository, IMapper mapper, DiscountGrpcService discountGrpcService, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _discountGrpcService = discountGrpcService;
            _httpContextAccessor = httpContextAccessor;
            _basketRepository = basketRepository;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var token = _httpContextAccessor.HttpContext.GetJwtFromContext();

            var basket = await _basketRepository.GetBasketAsync(id, token);

            return Ok(basket ?? new CustomerBasket(id));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket([FromBody] CustomerBasketDTO basket)
        {
            var token = _httpContextAccessor.HttpContext.GetJwtFromContext();

            // for each basket item, check if there is a discounted price from the Discount.Grpc service
            foreach (var item in basket.BasketItems)
            {
                var discount = await _discountGrpcService.GetDiscount(item.Product.Sku);

                // if discount.SalePrice = 0, item has no current discount
                item.Product.Price = ((decimal)discount.SalePrice < item.Product.Price && (decimal)discount.SalePrice != 0) 
                    ? (decimal)discount.SalePrice : item.Product.Price;
            }

            var customerBasket = _mapper.Map<CustomerBasketDTO, CustomerBasket>(basket);
            var updatedBasket = await _basketRepository.UpdateBasketAsync(customerBasket, token);

            return Ok(updatedBasket);
        }

        [HttpDelete]
        public async Task DeleteBasket(string id)
        {
            var token = _httpContextAccessor.HttpContext.GetJwtFromContext();

            await _basketRepository.DeleteBasketAsync(id, token);
        }
    }
}
