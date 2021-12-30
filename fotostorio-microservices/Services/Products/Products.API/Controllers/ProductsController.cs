using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Products.API.Contracts;
using Products.API.DTOs;
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
        private readonly IMapper _mapper;

        public ProductsController(ILogger<ProductsController> logger, IProductRepository productRepository, IMapper mapper)
        {
            _logger = logger;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        // GET api/products
        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>List of ProductDTO</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            try
            {
                var spec = new ProductsWithDetailsSpecification();
                var products = await _productRepository.ListWithSpecificationAsync(spec);

                return Ok(_mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(products));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetProducts : {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/products/{id}
        /// <summary>
        /// Get a product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ProductDTO</returns>
        [HttpGet("{id}", Name = "GetProductById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
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
                    return _mapper.Map<Product, ProductDTO>(product);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetProductById, from id {id} : {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/products
        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <returns>ProductDTO</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromBody] ProductCreateDTO productCreateDTO)
        {
            if (productCreateDTO == null)
            {
                return BadRequest();
            }

            try
            {
                var product = _mapper.Map<Product>(productCreateDTO);
                await _productRepository.Create(product);
                var productDTO = _mapper.Map<Product, ProductDTO>(product);

                return CreatedAtRoute(nameof(GetProductById), new { Id = productDTO.Id }, productDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateProduct : {ex.Message}");

                return BadRequest();
            }
        }

        // PUT api/products/{id}
        /// <summary>
        /// Update a product
        /// </summary>
        /// <param name="id"></param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDTO productUpdateDTO)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            try
            {
                _mapper.Map(productUpdateDTO, product);
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
        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
