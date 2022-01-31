using Store.BlazorWasm.DTOs;

namespace Store.BlazorWasm.Models;

public class BasketItem
{
    public int Quantity { get; set; }
    public ProductDTO Product { get; set; }

    public decimal Total {
        get {
            return Product.Price * Quantity;
        }
    }
}
