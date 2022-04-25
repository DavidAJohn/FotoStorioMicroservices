using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ordering.API.Models;

public class OrderCreateDTO
{
    public OrderCreateDTO()
    {
    }

    public OrderCreateDTO(List<OrderItem> items, Address sendToAddress)
    {
        Items = items;
        SendToAddress = sendToAddress;
    }

    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;

    [Required]
    public Address SendToAddress { get; set; }

    [Required]
    public List<OrderItem> Items { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public string PaymentIntentId { get; set; }
}
