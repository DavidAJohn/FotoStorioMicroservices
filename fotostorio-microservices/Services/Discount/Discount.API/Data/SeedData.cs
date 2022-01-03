using Discount.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Discount.API.Data
{
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
                    campaign.StartDate = DateTime.Now;
                    campaign.EndDate = DateTime.Now.AddDays(90);

                    context.Campaigns.Add(campaign);
                }

                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT Campaigns ON;");
                context.SaveChanges();
                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT Campaigns OFF");
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
    }
}
