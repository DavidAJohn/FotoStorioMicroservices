using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products.API.Data;
using Products.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.API.Controllers
{
    public class MountsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public MountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Mount>> GetMounts()
        {
            var mounts = await _context.Mounts
                .ToListAsync();

            return mounts;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMountById(int id)
        {
            var mount = await _context.Mounts
                .SingleOrDefaultAsync(m => m.Id == id);

            return Ok(mount);
        }
    }
}
