using Discount.Grpc.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Discount.Grpc.Data;

public static class SeedData
{
    public static async Task SeedCampaignDataAsync(ApplicationDbContext context)
    {
        if (await context.Campaigns.AnyAsync()) return;

        var campaignsData = await File.ReadAllTextAsync("./Data/SeedData/campaigns.json");
        var campaigns = JsonSerializer.Deserialize<List<Campaign>>(campaignsData);

        using (var transaction = context.Database.BeginTransaction())
        {
            foreach (var campaign in campaigns)
            {
                switch (campaign.Name)
                {
                    case "Previous Month Discount":
                        campaign.StartDate = DateTime.Now.AddDays(-31);
                        campaign.EndDate = DateTime.Now.AddDays(-1);
                        break;
                    case "Next Month Discount":
                        campaign.StartDate = DateTime.Now.AddDays(30);
                        campaign.EndDate = DateTime.Now.AddDays(60);
                        break;
                    default:
                        campaign.StartDate = DateTime.Now;
                        campaign.EndDate = DateTime.Now.AddDays(28);
                        break;
                };

                context.Campaigns.Add(campaign);
            }

            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Campaigns ON");
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Campaigns OFF");
            transaction.Commit();
        }
    }

    public static async Task SeedProductDiscountDataAsync(ApplicationDbContext context)
    {
        if (await context.ProductDiscounts.AnyAsync()) return;

        var discountsData = await File.ReadAllTextAsync("./Data/SeedData/discounts.json");
        var discounts = JsonSerializer.Deserialize<List<ProductDiscount>>(discountsData);

        foreach (var discount in discounts)
        {
            context.ProductDiscounts.Add(discount);
        }

        await context.SaveChangesAsync();
    }

    public static async Task CreateStoredProceduresAsync(ApplicationDbContext context)
    {
        using (var transaction = context.Database.BeginTransaction())
        {
            var sqlCommand = await File.ReadAllTextAsync("./Data/SqlScripts/Discount_Sprocs.sql");

            context.Database.ExecuteSqlRaw(sqlCommand);
            context.SaveChanges();
            transaction.Commit();
        }
    }
}
