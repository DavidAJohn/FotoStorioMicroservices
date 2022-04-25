namespace Ordering.API.Models;

public class Basket
{
    public Basket()
    {
    }

    public Basket(string id)
    {
        Id = id;
    }

    public string Id { get; set; }

    public List<BasketItem> BasketItems { get; set; } = new();

    public decimal BasketTotal
    {
        get
        {
            decimal total = (decimal)0.0;

            foreach (var item in BasketItems)
            {
                total += item.Total;
            }

            return total;
        }
    }
}
