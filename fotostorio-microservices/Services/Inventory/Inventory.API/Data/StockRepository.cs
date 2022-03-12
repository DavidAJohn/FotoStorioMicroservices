using Inventory.API.Contracts;
using Inventory.API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Data
{
    public class StockRepository : BaseRepository<Stock>, IStockRepository
    {
        private readonly InventoryDbContext _context;

        public StockRepository(InventoryDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Stock> GetBySkuAsync(string sku)
        {
            return await _context.Set<Stock>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Sku == sku);
        }

        public async Task<IEnumerable<Stock>> GetByStockLevelAtOrBelow(int stockLevel)
        {
            return await _context.Set<Stock>()
                .AsNoTracking()
                .Where(s => s.CurrentStock <= stockLevel)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}
