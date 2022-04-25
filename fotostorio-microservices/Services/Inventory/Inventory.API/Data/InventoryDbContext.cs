using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Stock> Stock { get; set; }
    public DbSet<Update> Updates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>()
            .HasKey(s => s.Sku);
    }
}
