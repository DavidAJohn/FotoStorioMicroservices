using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Data;

public class UpdateRepository : BaseRepository<Update>, IUpdateRepository
{
    private readonly InventoryDbContext _context;

    public UpdateRepository(InventoryDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Update>> GetBySkuAsync(string sku)
    {
        return await _context.Set<Update>()
            .AsNoTracking()
            .Where(u => u.Sku == sku)
            .ToListAsync();
    }
}
