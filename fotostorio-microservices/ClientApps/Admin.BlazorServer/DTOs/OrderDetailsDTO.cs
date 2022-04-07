using Admin.BlazorServer.DTOs;
using Admin.BlazorServer.Models;

namespace Admin.BlazorServer.DTOs;

public class OrderDetailsDTO
{
    public int Id { get; set; }
    public string BuyerEmail { get; set; } = string.Empty;
    public DateTimeOffset OrderDate { get; set; }
    public Address? SendToAddress { get; set; }
    public List<OrderItemReceivedDTO> Items { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
}
