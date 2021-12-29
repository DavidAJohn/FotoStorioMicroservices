using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Products.API.Contracts;
using Products.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductRepository _productRepository;

        public ProductsController(ILogger<ProductsController> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                var products = await _productRepository.GetAllProductsAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetProducts : {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productRepository.GetProductWithDetailsAsync(id);

                if (product == null)
                {
                    _logger.LogError($"Product with id: {id}, not found");
                    return NotFound();
                }
                else
                {
                    return Ok(product);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetProductById, from id {id} : {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
