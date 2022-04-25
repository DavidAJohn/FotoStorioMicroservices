namespace Products.API.Data;

public class BrandRepository : RepositoryBase<Brand>, IBrandRepository
{
    public BrandRepository(ApplicationDbContext repositoryContext) : base(repositoryContext)
    {
    }
}
