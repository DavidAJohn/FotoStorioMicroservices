using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Products.Aggregator.Models;
using Products.Aggregator.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Aggregator.Controllers
{
    [ApiController]
    [Route("api/aggr/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly IDiscountService _discountService;
        private readonly IMapper _mapper;

        public ProductsController(IProductsService productsService, IDiscountService discountService, IMapper mapper)
        {
            _productsService = productsService;
            _discountService = discountService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AggregatedProduct>>> GetProducts()
        {
            var products = await _productsService.GetProducts();

            if (products == null) return NotFound();

            var aggregatedProducts = new List<AggregatedProduct>();

            foreach (var product in products)
            {
                var aggregatedProduct = _mapper.Map<AggregatedProduct>(product);
                var discount = await _discountService.GetDiscountBySku(product.Sku);

                aggregatedProduct.SalePrice = discount.SalePrice != 0 && discount.SalePrice < product.Price 
                    ? discount.SalePrice : product.Price;

                aggregatedProducts.Add(aggregatedProduct);
            }

            return Ok(aggregatedProducts);
        }
    }
}
