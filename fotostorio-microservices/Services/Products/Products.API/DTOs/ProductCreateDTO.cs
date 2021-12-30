using System.ComponentModel.DataAnnotations;

namespace Products.API.DTOs
{
    public class ProductCreateDTO
    {
        [Required]
        public string Sku { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int MountId { get; set; }
    }
}
