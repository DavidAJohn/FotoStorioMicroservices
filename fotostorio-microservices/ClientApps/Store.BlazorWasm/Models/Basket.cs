namespace Store.BlazorWasm.Models;

public class Basket
{
    public List<BasketItem> BasketItems { get; set; } = new();

    public Decimal BasketTotal {
        get {
            decimal total = (decimal)0.0;

            foreach (var item in BasketItems)
            {
                total += item.Total;
            }

            return total;
        }
    }
}
