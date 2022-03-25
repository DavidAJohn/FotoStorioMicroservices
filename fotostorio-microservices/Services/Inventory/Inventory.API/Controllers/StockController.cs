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
            try
            {
                var stock = await _stockRepository.ListAllAsync();

                if (stock == null) return NotFound();

                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetStock : {message}", ex.Message);

                return BadRequest();
            }
        }

        // GET api/stock/{sku}
        [HttpGet("{sku}", Name = "GetStockBySku")]
        public async Task<ActionResult<Stock>> GetStockBySku(string sku)
        {
            if (sku == null) return BadRequest();

            try
            {
                var stock = await _stockRepository.GetBySkuAsync(sku);

                if (stock == null) return NotFound();

                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetStockBySku : {message}", ex.Message);

                return BadRequest();
            }
        }

        // GET api/stock/level/{stockLevel:int}
        [HttpGet("level/{stockLevel:int}")]
        public async Task<ActionResult<IEnumerable<Stock>>> GetStockAtOrBelowLevel(int stockLevel)
        {
            try
            {
                var stock = await _stockRepository.GetByStockLevelAtOrBelow(stockLevel);

                if (stock == null) return NotFound();

                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetStockAtOrBelowLevel : {message}", ex.Message);

                return BadRequest();
            }
        }

        // POST api/stock
        [HttpPost]
        public async Task<ActionResult<Stock>> CreateNewStockEntry(Stock stock)
        {
            if (stock == null) return BadRequest();

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError("Error in CreateNewStockEntry : {message}", ex.Message);

                return BadRequest();
            }
        }
    }
}
