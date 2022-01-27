using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ordering.API.Models
{
    public class Order : BaseEntity
    {
        public Order()
        {
        }

        public Order(IReadOnlyList<OrderItem> orderItems, string buyerEmail, Address sendToAddress, decimal subtotal, string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            SendToAddress = sendToAddress;
            OrderItems = orderItems;
            Subtotal = subtotal;
            PaymentIntentId = paymentIntentId;
        }

        [Required]
        public string BuyerEmail { get; set; }

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;

        [Required]
        public Address SendToAddress { get; set; }

        [Required]
        public IReadOnlyList<OrderItem> OrderItems { get; set; }

        [Required]
        public decimal Subtotal { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string PaymentIntentId { get; set; }
    }
}
