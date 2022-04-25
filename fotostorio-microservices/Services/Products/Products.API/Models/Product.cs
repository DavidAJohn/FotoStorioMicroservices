using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Products.API.Models;

public class Product : BaseEntity
{
    [Required]
    public string Sku { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    public string ImageUrl { get; set; }

    [Required]
    public int BrandId { get; set; }

    public Brand Brand { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public Category Category { get; set; }

    [Required]
    public int MountId { get; set; }

    public Mount Mount { get; set; }

    [Required]
    public bool IsAvailable { get; set; } = true;
}
