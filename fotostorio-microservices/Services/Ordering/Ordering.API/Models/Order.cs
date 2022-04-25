using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ordering.API.Models;

public class Order : BaseEntity
{
    public Order()
    {
    }

    public Order(List<OrderItem> items, string buyerEmail, Address sendToAddress, decimal total, string paymentIntentId)
    {
        Items = items;
        BuyerEmail = buyerEmail;
        SendToAddress = sendToAddress;
        Total = total;
        PaymentIntentId = paymentIntentId;
    }

    public string BuyerEmail { get; set; }

    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;

    [Required]
    public Address SendToAddress { get; set; }

    [Required]
    public List<OrderItem> Items { get; set; }

    [Required]
    public decimal Total { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public string PaymentIntentId { get; set; }
}
