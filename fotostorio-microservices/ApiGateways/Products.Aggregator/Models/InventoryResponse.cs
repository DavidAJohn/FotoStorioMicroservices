namespace Products.Aggregator.Models;

public class InventoryResponse
{
    public string Sku { get; set; }
    public int CurrentStock { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}
