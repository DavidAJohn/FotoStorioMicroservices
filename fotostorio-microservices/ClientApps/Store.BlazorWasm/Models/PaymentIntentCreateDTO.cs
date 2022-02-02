namespace Store.BlazorWasm.Models;

public class PaymentIntentCreateDTO
{
    public List<BasketItem> Items { get; set; }
    public string PaymentIntentId { get; set; }
}
