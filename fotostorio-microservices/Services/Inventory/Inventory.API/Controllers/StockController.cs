﻿using Inventory.API.Contracts;
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
                _logger.LogError($"Error in GetStockAtOrBelowLevel : {ex.Message}");

                return BadRequest();
            }
        }
    }
}
