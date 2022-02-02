using Ordering.API.Entities;
using System.Collections.Generic;

namespace Ordering.API.Models
{
    public class PaymentIntentCreateDTO
    {
        public List<BasketItem> Items { get; set; }
        public string PaymentIntentId { get; set; }
    }
}
