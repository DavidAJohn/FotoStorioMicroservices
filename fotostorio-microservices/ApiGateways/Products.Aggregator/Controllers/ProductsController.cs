using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Products.Aggregator.Controllers;

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

        var discounts = await _discountService.GetCurrentDiscounts();

        var aggregatedProducts = new List<AggregatedProduct>();

        // check if there is a reduced sale price for each item
        foreach (var product in products.Items)
        {
            var aggregatedProduct = _mapper.Map<AggregatedProduct>(product);
            aggregatedProduct.SalePrice = product.Price; // by default, sale price = existing (full) price

            if (discounts != null)
            {
                // if sku is in the list of current discounts
                var discountedSku = discounts.FirstOrDefault(d => d.Sku == aggregatedProduct.Sku);

                if (discountedSku != null)
                {
                    // if there is a discounted price then use it,
                    // otherwise the sale price remains the same as the existing price
                    if (discountedSku.SalePrice != 0 && discountedSku.SalePrice < product.Price)
                    {
                        aggregatedProduct.SalePrice = discountedSku.SalePrice;
                    }
                }
            }
            
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

        _httpContextAccessor.HttpContext.Response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader));

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
        if (discount != null)
        {
            aggregatedProduct.SalePrice = discount.SalePrice != 0 && discount.SalePrice < product.Price
                ? discount.SalePrice : product.Price;
        }
        
        return Ok(aggregatedProduct);
    }

    [HttpGet("offers")]
    public async Task<ActionResult<IEnumerable<AggregatedProduct>>> GetSpecialOffers([FromQuery] ProductParameters productParams)
    {
        var discounts = await _discountService.GetCurrentDiscounts();
        var aggregatedProducts = new List<AggregatedProduct>();

        if (discounts != null)
        {
            // get product details for each discounted item
            foreach (var discount in discounts)
            {
                var product = await _productsService.GetProductBySkuAsync(discount.Sku);

                if (product != null)
                {
                    var aggregatedProduct = _mapper.Map<AggregatedProduct>(product);
                    aggregatedProduct.SalePrice = discount.SalePrice;

                    aggregatedProducts.Add(aggregatedProduct);
                }
            }

            // sort the results if required - if the 'sort=' querystring is present
            if (!string.IsNullOrEmpty(productParams.Sort))
            {
                var sortedList = new List<AggregatedProduct>();

                switch (productParams.Sort)
                {
                    case "nameAsc":
                        sortedList = aggregatedProducts.OrderBy(p => p.Name).ToList();
                        break;
                    case "nameDesc":
                        sortedList = aggregatedProducts.OrderByDescending(p => p.Name).ToList();
                        break;
                    case "brandAsc":
                        sortedList = aggregatedProducts.OrderBy(p => p.Brand).ToList();
                        break;
                    case "brandDesc":
                        sortedList = aggregatedProducts.OrderByDescending(p => p.Brand).ToList();
                        break;
                    case "savingAsc":
                        sortedList = aggregatedProducts.OrderBy(p => (p.Price - p.SalePrice)).ToList();
                        break;
                    case "savingDesc":
                        sortedList = aggregatedProducts.OrderByDescending(p => (p.Price - p.SalePrice)).ToList();
                        break;
                    case "priceAsc":
                        sortedList = aggregatedProducts.OrderBy(p => p.Price).ToList();
                        break;
                    case "priceDesc":
                        sortedList = aggregatedProducts.OrderByDescending(p => p.Price).ToList();
                        break;
                    case "idAsc":
                        sortedList = aggregatedProducts.OrderBy(p => p.Id).ToList();
                        break;
                    case "idDesc":
                        sortedList = aggregatedProducts.OrderByDescending(p => p.Id).ToList();
                        break;
                    default:
                        sortedList = aggregatedProducts.OrderByDescending(p => p.Price).ToList();
                        break;
                }

                return Ok(sortedList);
            }
            else
            {
                return Ok(aggregatedProducts);
            }
        }

        return NotFound();
    }
}
