using System.ComponentModel.DataAnnotations.Schema;

namespace Ordering.API.Models
{
    public class ProductItemOrdered
    {
        public ProductItemOrdered()
        {
        }

        public ProductItemOrdered(int id, string sku, string name, string imageUrl, decimal price)
        {
            Id = id;
            Sku = sku;
            Name = name;
            ImageUrl = imageUrl;
            Price = price;
        }

        public int Id { get; set; }

        public string Sku { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }
    }
}
