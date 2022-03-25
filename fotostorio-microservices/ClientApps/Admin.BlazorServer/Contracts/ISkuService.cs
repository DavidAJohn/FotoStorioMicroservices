using Admin.BlazorServer.Models;

namespace Admin.BlazorServer.Contracts;

public interface ISkuService
{
    string GenerateSku(Brand brand, Category category, string name);
}
