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
    public class StockController : ControllerBase
    {
        private readonly ILogger<StockController> _logger;
        private readonly IStockRepository _stockRepository;

        public StockController(ILogger<StockController> logger, IStockRepository stockRepository)
        {
            _logger = logger;
            _stockRepository = stockRepository;
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
                _logger.LogError($"Error in GetStock : {ex.Message}");

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
                _logger.LogError($"Error in GetStockBySku : {ex.Message}");

                return BadRequest();
            }
        }

        // POST api/stock
        [HttpPost]
        public async Task<ActionResult<Stock>> CreateStock(Stock stock)
        {
            if (stock == null) return BadRequest();

            try
            {
                // TODO: check sku doesn't already exist

                var createdStock = await _stockRepository.Create(stock);

                if (createdStock == null) return BadRequest();

                return Ok(createdStock);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateStock : {ex.Message}");

                return BadRequest();
            }
        }

        // PUT api/stock/{sku}
        [HttpPut("{sku}")]
        public async Task<ActionResult> UpdateStock(string sku, [FromBody] Stock stock)
        {
            var stockToUpdate = await _stockRepository.GetBySkuAsync(sku);

            if (stockToUpdate == null)
            {
                return NotFound();
            }

            try
            {
                // TODO: add custom stock checks here

                await _stockRepository.Update(stock);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UpdateStock : {ex.Message}");

                return BadRequest();
            }
        }

        // DELETE api/stock/{sku}
        [HttpDelete("{sku}")]
        public async Task<ActionResult> DeleteStock(string sku)
        {
            var stockToDelete = await _stockRepository.GetBySkuAsync(sku);

            if (stockToDelete == null)
            {
                return NotFound();
            }

            try
            {
                await _stockRepository.Delete(stockToDelete);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteStock : {ex.Message}");

                return BadRequest();
            }
        }
    }
}
