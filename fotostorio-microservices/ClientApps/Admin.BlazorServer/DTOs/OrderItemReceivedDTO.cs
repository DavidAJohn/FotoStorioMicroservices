using Admin.BlazorServer.Models;

namespace Admin.BlazorServer.DTOs;

public class OrderItemReceivedDTO
{
    public OrderItemReceivedDTO(ProductDTO product, decimal price, int quantity)
    {
        Product = product;
        Price = price;
        Quantity = quantity;
    }

    public ProductDTO Product { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }
}
