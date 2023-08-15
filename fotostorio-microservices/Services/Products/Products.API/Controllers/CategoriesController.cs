using Microsoft.AspNetCore.Mvc;

namespace Products.API.Controllers;

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
        var categories = await _categoryRepository.ListAllAsync();

        return Ok(categories);
    }

    // GET api/categories/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetCategoryById(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            _logger.LogError("Category with id: {id}, not found", id);

            return NotFound();
        }
        else
        {
            return Ok(category);
        }
    }
}
