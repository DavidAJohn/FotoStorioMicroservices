namespace Products.Aggregator.Services;

public interface IInventoryService
{
    Task<InventoryResponse> GetStockBySkuAsync(string sku);
}
