using Store.BlazorWasm.Models;

namespace Store.BlazorWasm.DTOs;

public class OrderDetailsDTO
{
    public int Id { get; set; }
    public string BuyerEmail { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public Address SendToAddress { get; set; }
    public List<OrderItemReceivedDTO> Items { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; }
}
