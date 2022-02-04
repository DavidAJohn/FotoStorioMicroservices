using Store.BlazorWasm.Models;

namespace Store.BlazorWasm.DTOs;

public class OrderItemReceivedDTO
{
    public OrderItemReceivedDTO(Product product, decimal price, int quantity)
    {
        Product = product;
        Price = price;
        Quantity = quantity;
    }

    public Product Product { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }
}
