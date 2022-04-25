using System.ComponentModel.DataAnnotations;

namespace Products.API.Models;

public class Category : BaseEntity
{
    [Required]
    public string Name { get; set; }
}
