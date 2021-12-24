using System.ComponentModel.DataAnnotations;

namespace Products.API.Models
{
    public class Brand : BaseEntity
    {
        [Required]
        public string Name { get; set; }
    }
}
