using Inventory.API.Contracts;
using Inventory.API.Entities;
using Inventory.API.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.API.Controllers
{
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
            try
            {
                var token = _httpContextAccessor.HttpContext.GetJwtFromContext();

                var updates = await _inventoryService.GetUpdates(token);

                if (updates == null) return NotFound();

                return Ok(updates);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetUpdates : {ex.Message}");

                return BadRequest();
            }
        }

        // GET api/updates/{sku}
        [HttpGet("{sku}", Name = "GetUpdatesBySku")]
        public async Task<ActionResult<IEnumerable<Update>>> GetUpdatesBySku(string sku)
        {
            if (sku == null) return BadRequest();

            try
            {
                var token = _httpContextAccessor.HttpContext.GetJwtFromContext();

                var updates = await _inventoryService.GetUpdatesBySku(sku, token);

                if (updates == null) return NotFound();

                return Ok(updates);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetUpdatesBySku : {ex.Message}");

                return BadRequest();
            }
        }

        // POST api/updates
        [HttpPost]
        public async Task<ActionResult<Update>> CreateStockUpdate(UpdateCreateDTO update)
        {
            if (update == null) return BadRequest();

            try
            {
                var token = _httpContextAccessor.HttpContext.GetJwtFromContext();

                var createdUpdate = await _inventoryService.CreateUpdateFromAdmin(update, token);

                if (createdUpdate == null) return BadRequest("There was a problem updating stock for this Sku");

                return Ok(createdUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateUpdate : {ex.Message}");

                return BadRequest();
            }
        }
    }
}
