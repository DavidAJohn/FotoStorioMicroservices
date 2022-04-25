using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Inventory.API.Data;

public class SeedData
{
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
