using System.ComponentModel.DataAnnotations;

namespace Admin.BlazorServer.Models;

public class UpdateStockModel
{
    public string? Sku { get; set; }
    public int Added { get; set; }
    public int Removed { get; set; }
}
