using Products.API.Contracts;
using Products.API.Models;

namespace Products.API.Data
{
    public class MountRepository : RepositoryBase<Mount>, IMountRepository
    {
        public MountRepository(ApplicationDbContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
