using Admin.BlazorServer.Contracts;
using Admin.BlazorServer.Models;

namespace Admin.BlazorServer.Services;

public class SkuService : ISkuService
{
    public SkuService()
    {
    }

    public string GenerateSku(Brand brand, Category category, string name)
    {
        // Sku: {brand name - first 3 chars} {name section} {category - 1 letter (camera/lense)}

        // prefix (first 3 letters of brand)
        var brandPrefix = !string.IsNullOrEmpty(brand.Name) ? brand.Name.Substring(0, 3).ToUpperInvariant() : "";

        // middle section (first 4 numbers)
        var numbersInNameSection = string.Join("", name.ToCharArray().Where(Char.IsDigit));
        var nameSection = numbersInNameSection.Length > 4 ? numbersInNameSection.Substring(0, 4) : numbersInNameSection;

        // suffix (C = camera, L = lens)
        var categorySuffix = category.Name.ToLowerInvariant() switch
        {
            "mirrorless cameras" => "C",
            "dslr cameras" => "C",
            "lenses" => "L",
            _ => ""
        };

        //var sku = brandPrefix + "-" + nameSection + "-" + categorySuffix;
        var sku = brandPrefix + nameSection + categorySuffix;

        return sku;
    }
}
