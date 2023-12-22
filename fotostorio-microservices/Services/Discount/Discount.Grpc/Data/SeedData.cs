using Discount.Grpc.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Discount.Grpc.Data;

public class SeedData
{
    private readonly ApplicationDbContext _context;

    public SeedData(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task MigrateDatabaseAsync()
    {
        await _context.Database.MigrateAsync();
    }
    
    public async Task SeedCampaignDataAsync()
    {
        if (await _context.Campaigns.AnyAsync()) return;

        var campaignsData = await File.ReadAllTextAsync("./Data/SeedData/campaigns.json");
        var campaigns = JsonSerializer.Deserialize<List<Campaign>>(campaignsData);

        var strategy = _context.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                foreach (var campaign in campaigns)
                {
                    switch (campaign.Name)
                    {
                        case "Previous Month Discount":
                            campaign.StartDate = DateTime.Today.AddDays(-31);
                            campaign.EndDate = DateTime.Today.AddDays(-1);
                            break;
                        case "Next Month Discount":
                            campaign.StartDate = DateTime.Today.AddDays(30);
                            campaign.EndDate = DateTime.Today.AddDays(60);
                            break;
                        default:
                            campaign.StartDate = DateTime.Today;
                            campaign.EndDate = DateTime.Today.AddDays(28);
                            break;
                    };

                    _context.Campaigns.Add(campaign);
                }

                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Campaigns ON");
                _context.SaveChanges();
                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Campaigns OFF");
                transaction.Commit();
            }
        });
    }

    public async Task SeedProductDiscountDataAsync()
    {
        if (await _context.ProductDiscounts.AnyAsync()) return;

        var discountsData = await File.ReadAllTextAsync("./Data/SeedData/discounts.json");
        var discounts = JsonSerializer.Deserialize<List<ProductDiscount>>(discountsData);

        foreach (var discount in discounts)
        {
            _context.ProductDiscounts.Add(discount);
        }

        await _context.SaveChangesAsync();
    }

    public async Task CreateStoredProceduresAsync()
    {
        using (var transaction = _context.Database.BeginTransaction())
        {
            var sqlCommand = await File.ReadAllTextAsync("./Data/SqlScripts/Discount_Sprocs.sql");

            _context.Database.ExecuteSqlRaw(sqlCommand);
            _context.SaveChanges();
            transaction.Commit();
        }
    }
}
