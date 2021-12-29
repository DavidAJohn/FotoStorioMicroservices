using Microsoft.EntityFrameworkCore;
using Products.API.Contracts;
using Products.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.API.Data
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _repositoryContext;

        public ProductRepository(ApplicationDbContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await GetAll();
        }

        public async Task<Product> GetProductByIdAsync(int Id)
        {
            return await GetByCondition(p => p.Id.Equals(Id));
        }

        public async Task<Product> GetProductWithDetailsAsync(int Id)
        {
            return await GetByCondition(p => p.Id.Equals(Id));
        }

        public async Task<Product> CreateProduct(Product product)
        {
            await Create(product);
            return product;
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            return await Update(product);
        }

        public async Task<bool> DeleteProduct(Product product)
        {
            return await Delete(product);
        }
    }
}
