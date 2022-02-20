using System;

namespace Inventory.API.Entities
{
    public class Update
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Added { get; set; }
        public int Removed { get; set; }
        public int? OrderId { get; set; }
    }
}
