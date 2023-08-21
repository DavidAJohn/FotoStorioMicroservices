using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UpdatesController : ControllerBase
{
    private readonly ILogger<UpdatesController> _logger;
    private readonly IInventoryService _inventoryService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpContextService _httpContextService;

    public UpdatesController(ILogger<UpdatesController> logger, IInventoryService inventoryService, IHttpContextAccessor httpContextAccessor, IHttpContextService httpContextService)
    {
        _logger = logger;
        _inventoryService = inventoryService;
        _httpContextAccessor = httpContextAccessor;
        _httpContextService = httpContextService;
    }

    // GET api/updates
    [HttpGet]
    public async Task<IActionResult> GetUpdates()
    {
        var token = _httpContextService.GetJwtFromContext(_httpContextAccessor.HttpContext);
        var role = _httpContextService.GetClaimValueByType(_httpContextAccessor.HttpContext, "role");

        if (role != "Administrator")
        {
            _logger.LogWarning("Stock Updates: GetUpdates called with role: '{role}', NOT 'Administrator'", role);
            return Unauthorized();
        }

        var updates = await _inventoryService.GetUpdates(token);

        if (updates == null) return NotFound();

        return Ok(updates);
    }

    // GET api/updates/{sku}
    [HttpGet("{sku}", Name = "GetUpdatesBySku")]
    public async Task<IActionResult> GetUpdatesBySku(string sku)
    {
        if (sku == null) return BadRequest();

        var token = _httpContextService.GetJwtFromContext(_httpContextAccessor.HttpContext);
        var role = _httpContextService.GetClaimValueByType(_httpContextAccessor.HttpContext, "role");

        if (role != "Administrator")
        {
            _logger.LogWarning("Stock Updates: GetUpdatesBySku called with role: '{role}', NOT 'Administrator'", role);
            return Unauthorized();
        }

        var updates = await _inventoryService.GetUpdatesBySku(sku, token);

        if (updates == null) return NotFound();

        return Ok(updates);
    }

    // POST api/updates
    [HttpPost]
    public async Task<IActionResult> CreateStockUpdate(UpdateCreateDTO update)
    {
        if (update == null) return BadRequest();

        var token = _httpContextService.GetJwtFromContext(_httpContextAccessor.HttpContext);
        var role = _httpContextService.GetClaimValueByType(_httpContextAccessor.HttpContext, "role");

        if (role != "Administrator")
        {
            _logger.LogWarning("Stock Updates: CreateStockUpdate called with role: '{role}', NOT 'Administrator'", role);
            return Unauthorized();
        }

        var createdUpdate = await _inventoryService.CreateUpdateFromAdmin(update, token);

        if (createdUpdate == null) return BadRequest("There was a problem updating stock for this Sku");

        return Ok(createdUpdate);
    }
}
