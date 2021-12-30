using Products.API.Contracts;
using Products.API.Models;

namespace Products.API.Data
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
