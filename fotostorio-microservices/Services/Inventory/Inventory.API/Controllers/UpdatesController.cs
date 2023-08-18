using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UpdatesController : ControllerBase
{
    private readonly ILogger<UpdatesController> _logger;
    private readonly IInventoryService _inventoryService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdatesController(ILogger<UpdatesController> logger, IInventoryService inventoryService, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _inventoryService = inventoryService;
        _httpContextAccessor = httpContextAccessor;
    }

    // GET api/updates
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Update>>> GetUpdates()
    {
        var token = _httpContextAccessor.HttpContext.GetJwtFromContext();
        var role = _httpContextAccessor.HttpContext.GetClaimValueByType("role");

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
    public async Task<ActionResult<IEnumerable<Update>>> GetUpdatesBySku(string sku)
    {
        if (sku == null) return BadRequest();

        var token = _httpContextAccessor.HttpContext.GetJwtFromContext();
        var role = _httpContextAccessor.HttpContext.GetClaimValueByType("role");

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
    public async Task<ActionResult<Update>> CreateStockUpdate(UpdateCreateDTO update)
    {
        if (update == null) return BadRequest();

        var token = _httpContextAccessor.HttpContext.GetJwtFromContext();
        var role = _httpContextAccessor.HttpContext.GetClaimValueByType("role");

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
