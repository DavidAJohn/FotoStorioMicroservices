namespace Admin.BlazorServer.Models;

public class ProductStock
{
    public string? Sku { get; set; }
    public string? Name { get; set; }
    public int CurrentStock { get; set; }
    public DateTime LastUpdated { get; set; }
}
