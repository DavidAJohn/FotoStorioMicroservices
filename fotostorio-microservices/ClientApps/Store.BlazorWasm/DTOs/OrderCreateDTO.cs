using Store.BlazorWasm.Models;
using System.ComponentModel.DataAnnotations;

namespace Store.BlazorWasm.DTOs;

public class OrderCreateDTO
{
    [Required]
    public List<BasketItem> Items { get; set; }

    [Required]
    public Address SendToAddress { get; set; }

    [Required]
    public string PaymentIntentId { get; set; }
}
