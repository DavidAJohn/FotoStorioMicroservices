using Products.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.API.Contracts
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int Id);
        Task<Product> GetProductWithDetailsAsync(int Id);
        Task<Product> CreateProduct(Product product);
        Task<bool> UpdateProduct(Product product);
        Task<bool> DeleteProduct(Product product);
    }
}
