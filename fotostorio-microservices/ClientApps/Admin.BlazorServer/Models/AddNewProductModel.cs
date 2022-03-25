namespace Admin.BlazorServer.Models
{
    public class AddNewProductModel : BaseEntity
    {
        public string Sku { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public int BrandId { get; set; }

        public int CategoryId { get; set; }

        public int MountId { get; set; }

        public bool IsAvailable { get; set; }

        public int Stock { get; set; }
    }
}
