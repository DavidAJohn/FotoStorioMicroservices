namespace Store.BlazorWasm.DTOs;

public class ProductDTO
{
    public int Id { get; set; }
    public string Sku { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public decimal Price { get; set; }
    public string Brand { get; set; }
    public string Category { get; set; }
    public string Mount { get; set; }
}
