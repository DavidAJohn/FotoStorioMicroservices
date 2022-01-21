using Store.BlazorWasm.Models;

namespace Store.BlazorWasm.Contracts
{
    public interface IProductService
    {
        Task<PagedList<Product>> GetProductsAsync(ProductParameters productParameters);
        Task<Product> GetProductByIdAsync(int id);
        Task<List<Product>> GetProductsByBrandAsync(int brandId);
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<List<Product>> GetProductsByMountAsync(int mountId);
        Task<List<Brand>> GetProductBrandsAsync();
        Task<List<Category>> GetProductCategoriesAsync();
        Task<List<Mount>> GetProductMountsAsync();
        Task<List<Product>> GetProductsOnSpecialOfferAsync();
    }
}
