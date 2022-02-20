using Inventory.API.Contracts;
using Inventory.API.Entities;
using Microsoft.EntityFrameworkCore;
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
    }
}
