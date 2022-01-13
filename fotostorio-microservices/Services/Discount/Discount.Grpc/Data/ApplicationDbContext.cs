using Discount.Grpc.Models;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<ProductDiscount> ProductDiscounts { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductDiscount>()
            .Property(d => d.SalePrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<ProductDiscount>()
            .HasIndex(d => new { d.Sku, d.CampaignId }).IsUnique();
    }
}
