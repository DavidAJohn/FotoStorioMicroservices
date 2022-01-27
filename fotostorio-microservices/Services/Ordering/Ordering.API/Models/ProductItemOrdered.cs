namespace Ordering.API.Models
{
    public class ProductItemOrdered
    {
        public ProductItemOrdered()
        {
        }

        public ProductItemOrdered(int productItemId, string productSku, string productName, string imageUrl)
        {
            ProductItemId = productItemId;
            ProductSku = productSku;
            ProductName = productName;
            ImageUrl = imageUrl;
        }

        public int ProductItemId { get; set; }
        public string ProductSku { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
    }
}
