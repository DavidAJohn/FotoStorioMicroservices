using System.ComponentModel.DataAnnotations;

namespace Products.API.Models;

public class Mount : BaseEntity
{
    [Required]
    public string Name { get; set; }
}
