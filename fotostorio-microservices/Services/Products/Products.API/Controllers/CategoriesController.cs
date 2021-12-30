using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Products.API.Contracts;
using Products.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.API.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ILogger<CategoriesController> logger, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        // GET api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            try
            {
                var categories = await _categoryRepository.ListAllAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetCategories : {ex.Message}");

                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/categories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);

                if (category == null)
                {
                    _logger.LogError($"Category with id: {id}, not found");

                    return NotFound();
                }
                else
                {
                    return Ok(category);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetCategoryById : {ex.Message}");

                return StatusCode(500, "Internal server error");
            }
        }
    }
}
