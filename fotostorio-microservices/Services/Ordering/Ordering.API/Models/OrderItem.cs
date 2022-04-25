using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordering.API.Models;

public class OrderItem : BaseEntity
{
    public OrderItem()
    {
    }

    public OrderItem(ProductItemOrdered product, decimal total, int quantity)
    {
        Product = product;
        Total = total;
        Quantity = quantity;
    }

    public ProductItemOrdered Product { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    [Required]
    public int Quantity { get; set; }
}
