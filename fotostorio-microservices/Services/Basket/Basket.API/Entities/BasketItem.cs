namespace Basket.API.Entities
{
    public class BasketItem
    {
        public int Quantity { get; set; }
        public Product Product { get; set; }

        public decimal Total
        {
            get
            {
                return (Product.Price * Quantity);
            }
        }
    }
}
