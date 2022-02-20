using Inventory.API.Contracts;
using Inventory.API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Inventory.API.Data
{
    public class UpdateRepository : BaseRepository<Update>, IUpdateRepository
    {
        private readonly InventoryDbContext _context;

        public UpdateRepository(InventoryDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Update> GetBySkuAsync(string sku)
        {
            return await _context.Set<Update>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Sku == sku);
        }
    }
}
