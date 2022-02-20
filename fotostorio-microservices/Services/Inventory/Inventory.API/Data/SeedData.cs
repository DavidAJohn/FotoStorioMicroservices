using Inventory.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Inventory.API.Data
{
    public class SeedData
    {
        //public static async Task CreateInventoryUpdateTriggerAsync(InventoryDbContext context)
        //{
        //    //if (await context.Updates.AnyAsync()) return;

        //    using (var transaction = context.Database.BeginTransaction())
        //    { 
        //        context.Database.ExecuteSqlRaw("CREATE TRIGGER Update_Stock AFTER INSERT ON Updates INSERT INTO Stock VALUES (NEW.Sku, NEW.Added, NEW.UpdatedAt);");
        //        context.SaveChanges();
        //        transaction.Commit();
        //    }
        //}

        public static async Task SeedStockDataAsync(InventoryDbContext context)
        {
            if (await context.Updates.AnyAsync()) return;

            var stockData = await File.ReadAllTextAsync("./Data/SeedData/stock.json");
            var updates = JsonSerializer.Deserialize<List<Update>>(stockData);
            var stock = JsonSerializer.Deserialize<List<Stock>>(stockData);

            using (var transaction = context.Database.BeginTransaction())
            {
                foreach (var item in stock)
                {
                    item.CurrentStock = 5;
                    item.LastUpdated = DateTime.Now;

                    context.Stock.Add(item);
                }

                foreach (var item in updates)
                {
                    item.UpdatedAt = DateTime.Now;
                    item.Added = 5;
                    item.Removed = 0;

                    context.Updates.Add(item);
                }

                context.SaveChanges();
                transaction.Commit();
            }
        }
    }
}
