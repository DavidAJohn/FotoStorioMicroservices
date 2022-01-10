using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Products.Aggregator.Models;
using Products.Aggregator.Services;
using System.Collections.Generic;
using System.Text.Json;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductsController(IProductsService productsService, IDiscountService discountService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _productsService = productsService;
            _discountService = discountService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AggregatedProduct>>> GetProducts([FromQuery] ProductParameters productParams)
        {
            var products = await _productsService.GetProductsAsync(productParams);
            var productsMetadata = products.Metadata;

            if (products == null) return NotFound();

            var aggregatedProducts = new List<AggregatedProduct>();

            // check if there is a reduced sale price for each item via a request to the discount api
            foreach (var product in products.Items)
            {
                var aggregatedProduct = _mapper.Map<AggregatedProduct>(product);
                var discount = await _discountService.GetDiscountBySku(product.Sku);

                // if there is a discounted price then use it,
                // otherwise set the sale price to the same as the existing price
                aggregatedProduct.SalePrice = discount.SalePrice != 0 && discount.SalePrice < product.Price 
                    ? discount.SalePrice : product.Price;

                aggregatedProducts.Add(aggregatedProduct);
            }

            // also pass on the pagination header data that has come from the products api
            var paginationHeader = new PaginationResponseHeader
            {
                TotalCount = productsMetadata.TotalCount,
                PageIndex = productsMetadata.PageIndex,
                PageSize = productsMetadata.PageSize,
                TotalPages = productsMetadata.TotalPages,
                HasPreviousPage = productsMetadata.HasPreviousPage,
                HasNextPage = productsMetadata.HasNextPage
            };

            _httpContextAccessor.HttpContext.Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader));

            return Ok(aggregatedProducts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AggregatedProduct>> GetProductById(int id)
        {
            var product = await _productsService.GetProductByIdAsync(id);

            if (product == null) return NotFound();

            // check if there is a reduced sale price for the item via a request to the discount api
            var aggregatedProduct = _mapper.Map<AggregatedProduct>(product);
            var discount = await _discountService.GetDiscountBySku(product.Sku);

            // if there is a discounted price then use it,
            // otherwise set the sale price to the same as the existing price
            aggregatedProduct.SalePrice = discount.SalePrice != 0 && discount.SalePrice < product.Price
                ? discount.SalePrice : product.Price;

            return Ok(aggregatedProduct);
        }
    }
}
