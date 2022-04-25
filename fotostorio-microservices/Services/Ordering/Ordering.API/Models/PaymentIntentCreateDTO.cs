namespace Ordering.API.Models;

public class PaymentIntentCreateDTO
{
    public List<BasketItem> Items { get; set; }
    public string PaymentIntentId { get; set; }
}
