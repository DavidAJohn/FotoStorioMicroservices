using Admin.BlazorServer.DTOs;
using Admin.BlazorServer.Models;

namespace Admin.BlazorServer.Contracts;

public interface IProductService
{
    Task<List<Brand>?> GetProductBrandsAsync();
    Task<List<Category>?> GetProductCategoriesAsync();
    Task<List<Mount>?> GetProductMountsAsync();
    Task<ProductDTO?> CreateProductAsync(ProductCreateDTO productCreateDTO);
    Task<int> GetProductCountAsync();
}
