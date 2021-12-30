using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Products.API.Contracts;
using Products.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.API.Controllers
{
    public class MountsController : BaseApiController
    {
        private readonly ILogger<MountsController> _logger;
        private readonly IMountRepository _mountRepository;

        public MountsController(ILogger<MountsController> logger, IMountRepository mountRepository)
        {
            _logger = logger;
            _mountRepository = mountRepository;
        }

        // GET api/mounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mount>>> GetMounts()
        {
            try
            {
                var mounts = await _mountRepository.ListAllAsync();

                return Ok(mounts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetMounts : {ex.Message}");

                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/mounts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetMountById(int id)
        {
            try
            {
                var mount = await _mountRepository.GetByIdAsync(id);

                if (mount == null)
                {
                    _logger.LogError($"Mount with id: {id}, not found");

                    return NotFound();
                }
                else
                {
                    return Ok(mount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetMountById : {ex.Message}");

                return StatusCode(500, "Internal server error");
            }
        }
    }
}
