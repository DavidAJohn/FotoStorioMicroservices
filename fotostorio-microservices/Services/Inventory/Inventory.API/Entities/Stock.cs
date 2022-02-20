using System;

namespace Inventory.API.Entities
{
    public class Stock
    {
        public string Sku { get; set; }
        public int CurrentStock { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
