using Microsoft.AspNetCore.Mvc;

namespace Products.API.Controllers;

public class BrandsController : BaseApiController
{
    private readonly ILogger<BrandsController> _logger;
    private readonly IBrandRepository _brandRepository;

    public BrandsController(ILogger<BrandsController> logger, IBrandRepository brandRepository)
    {
        _logger = logger;
        _brandRepository = brandRepository;
    }

    // GET api/brands
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Brand>>> GetBrands()
    {
        try
        {
            var brands = await _brandRepository.ListAllAsync();

            return Ok(brands);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in GetBrands : {message}", ex.Message);

            return StatusCode(500, "Internal server error");
        }
    }

    // GET api/brands/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetBrandById(int id)
    {
        try
        {
            var brand = await _brandRepository.GetByIdAsync(id);

            if (brand == null)
            {
                _logger.LogError("Brand with id: {id}, not found", id);

                return NotFound();
            }
            else
            {
                return Ok(brand);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in GetBrandById : {message}", ex.Message);

            return StatusCode(500, "Internal server error");
        }
    }
}
