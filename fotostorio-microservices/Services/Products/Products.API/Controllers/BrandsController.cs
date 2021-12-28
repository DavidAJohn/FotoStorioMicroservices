using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products.API.Data;
using Products.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.API.Controllers
{
    public class BrandsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public BrandsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Brand>> GetBrands()
        {
            var brands = await _context.Brands
                .ToListAsync();

            return brands;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(int id)
        {
            var brand = await _context.Brands
                .SingleOrDefaultAsync(b => b.Id == id);

            return Ok(brand);
        }
    }
}
