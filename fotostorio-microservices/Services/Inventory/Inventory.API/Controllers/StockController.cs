using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly ILogger<StockController> _logger;
    private readonly IStockRepository _stockRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public StockController(ILogger<StockController> logger, IStockRepository stockRepository, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _stockRepository = stockRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    // GET api/stock
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Stock>>> GetStock()
    {
        var stock = await _stockRepository.ListAllAsync();

        if (stock == null) return NotFound();

        return Ok(stock);
    }

    // GET api/stock/{sku}
    [HttpGet("{sku}", Name = "GetStockBySku")]
    public async Task<ActionResult<Stock>> GetStockBySku(string sku)
    {
        if (sku == null) return BadRequest();

        var stock = await _stockRepository.GetBySkuAsync(sku);

        if (stock == null) return NotFound();

        return Ok(stock);
    }

    // GET api/stock/level/{stockLevel:int}
    [HttpGet("level/{stockLevel:int}")]
    public async Task<ActionResult<IEnumerable<Stock>>> GetStockAtOrBelowLevel(int stockLevel)
    {
        var stock = await _stockRepository.GetByStockLevelAtOrBelow(stockLevel);

        if (stock == null) return NotFound();

        return Ok(stock);
    }

    // POST api/stock
    [HttpPost]
    public async Task<ActionResult<Stock>> CreateNewStockEntry(Stock stock)
    {
        if (stock == null) return BadRequest();

        var token = _httpContextAccessor.HttpContext.GetJwtFromContext();
        var role = _httpContextAccessor.HttpContext.GetClaimValueByType("role");

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
