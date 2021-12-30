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
    }
}
