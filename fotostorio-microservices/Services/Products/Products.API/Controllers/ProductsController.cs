using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Products.API.Contracts;
using Products.API.Models;
using Products.API.Specifications;
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

        // GET api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                var spec = new ProductsWithDetailsSpecification();
                var products = await _productRepository.ListWithSpecificationAsync(spec);

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetProducts : {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/products/{id}
        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<ActionResult> GetProductById(int id)
        {
            try
            {
                var spec = new ProductsWithDetailsSpecification(id);
                var product = await _productRepository.GetEntityWithSpecification(spec);

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

        // POST api/products
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            try
            {
                await _productRepository.Create(product);
                return CreatedAtRoute(nameof(GetProductById), new { Id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateProduct : {ex.Message}");
                return BadRequest();
            }
        }

        // PUT api/products/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            var productToUpdate = await _productRepository.GetByIdAsync(id);

            if (productToUpdate == null)
            {
                return NotFound();
            }

            try
            {
                await _productRepository.Update(product);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UpdateProduct : {ex.Message}");
                return BadRequest();
            }
        }

        // DELETE api/products/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            try
            {
                await _productRepository.Delete(product);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteProduct : {ex.Message}");
                return BadRequest();
            }
        }
    }
}
