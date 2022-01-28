using Store.BlazorWasm.Models;

namespace Store.BlazorWasm.DTOs;

public class OrderDTO
{
    public string OrderId { get; set; }
    public Address SendToAddress { get; set; }
}
