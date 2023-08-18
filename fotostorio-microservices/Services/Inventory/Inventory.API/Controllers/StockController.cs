using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly ILogger<StockController> _logger;
    private readonly IStockRepository _stockRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpContextService _httpContextService;

    public StockController(ILogger<StockController> logger, IStockRepository stockRepository, IHttpContextAccessor httpContextAccessor, IHttpContextService httpContextService)
    {
        _logger = logger;
        _stockRepository = stockRepository;
        _httpContextAccessor = httpContextAccessor;
        _httpContextService = httpContextService;
    }

    // GET api/stock
    [HttpGet]
    public async Task<IActionResult> GetStock()
    {
        var stock = await _stockRepository.ListAllAsync();

        if (stock == null) return NotFound();

        return Ok(stock);
    }

    // GET api/stock/{sku}
    [HttpGet("{sku}", Name = "GetStockBySku")]
    public async Task<IActionResult> GetStockBySku(string sku)
    {
        if (sku == null) return BadRequest();

        var stock = await _stockRepository.GetBySkuAsync(sku);

        if (stock == null) return NotFound();

        return Ok(stock);
    }

    // GET api/stock/level/{stockLevel:int}
    [HttpGet("level/{stockLevel:int}")]
    public async Task<IActionResult> GetStockAtOrBelowLevel(int stockLevel)
    {
        var stock = await _stockRepository.GetByStockLevelAtOrBelow(stockLevel);

        if (stock is null || !stock.Any()) return NotFound();

        return Ok(stock);
    }

    // POST api/stock
    [HttpPost]
    public async Task<IActionResult> CreateNewStockEntry(Stock stock)
    {
        if (stock == null) return BadRequest();

        var token = _httpContextService.GetJwtFromContext(_httpContextAccessor.HttpContext);
        var role = _httpContextService.GetClaimValueByType(_httpContextAccessor.HttpContext, "role");

        if (role != "Administrator")
        {
            _logger.LogWarning("Stock Updates: CreateNewStockEntry called with role: '{role}', NOT 'Administrator'", role);
            return Unauthorized();
        }

        var createdStockEntry = await _stockRepository.Create(stock);

        if (createdStockEntry == null) return BadRequest("There was a problem adding stock for this Sku");

        return Ok(createdStockEntry);
    }
}
