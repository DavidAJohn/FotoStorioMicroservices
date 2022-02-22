using Inventory.API.Contracts;
using Inventory.API.Entities;
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

        public UpdatesController(ILogger<UpdatesController> logger, IInventoryService inventoryService)
        {
            _logger = logger;
            _inventoryService = inventoryService;
        }

        // GET api/updates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Update>>> GetUpdates()
        {
            try
            {
                var updates = await _inventoryService.GetUpdates();

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
                var updates = await _inventoryService.GetUpdatesBySku(sku);

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
                var createdUpdate = await _inventoryService.CreateUpdateFromAdmin(update);

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
