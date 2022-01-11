using Store.BlazorWasm.Models;

namespace Store.BlazorWasm.Contracts
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
    }
}
