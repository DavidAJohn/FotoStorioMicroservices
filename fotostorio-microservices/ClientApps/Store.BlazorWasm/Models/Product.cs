namespace Store.BlazorWasm.Models
{
    public class Product : BaseEntity
    {
        public string Sku { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal SalePrice { get; set; }

        public string ImageUrl { get; set; }

        public int BrandId { get; set; }

        public string Brand { get; set; }

        public int CategoryId { get; set; }

        public string Category { get; set; }

        public int MountId { get; set; }

        public string Mount { get; set; }

        public bool IsAvailable { get; set; }
    }
}
