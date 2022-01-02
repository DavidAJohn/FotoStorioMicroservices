using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Basket.API.Entities
{
    public class CustomerBasketDTO
    {
        [Required]
        public string Id { get; set; }
        public List<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
    }
}
