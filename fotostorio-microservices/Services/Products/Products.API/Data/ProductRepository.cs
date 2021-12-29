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
        public ProductRepository(ApplicationDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await GetAll()
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int Id)
        {
            return await GetByCondition(p => p.Id.Equals(Id))
                .FirstOrDefaultAsync();
        }

        public async Task<Product> GetProductWithDetailsAsync(int Id)
        {
            return await GetByCondition(p => p.Id.Equals(Id))
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Mount)
                .FirstOrDefaultAsync();
        }
    }
}
