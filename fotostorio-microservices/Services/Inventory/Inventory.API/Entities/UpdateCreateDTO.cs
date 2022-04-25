namespace Inventory.API.Entities;

public class UpdateCreateDTO
{
    public string Sku { get; set; }
    public int Added { get; set; }
    public int Removed { get; set; }
}
