using System.ComponentModel.DataAnnotations;

namespace Basket.API.Entities;

public class CustomerBasketDTO
{
    [Required]
    public string Id { get; set; }
    public List<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
    public string ClientSecret { get; set; }
    public string PaymentIntentId { get; set; }
}
