using Admin.BlazorServer.Models;

namespace Admin.BlazorServer.Contracts;

public interface IInventoryService
{
    Task<List<ProductStock>?> GetInventoryAsync();
    Task<List<ProductStock>?> GetInventoryAtOrBelowLevelAsync(int stockLevel);
    Task<UpdateStockResult?> CreateNewUpdate(UpdateStockModel stockUpdate);
    Task<UpdateStockResult?> CreateNewStock(ProductStock stock);
}
