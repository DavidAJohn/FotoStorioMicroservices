using Microsoft.EntityFrameworkCore;

namespace Products.API.Data;

public class ProductRepository : RepositoryBase<Product>, IProductRepository
{
    private readonly ApplicationDbContext _repositoryContext;

    public ProductRepository(ApplicationDbContext repositoryContext) : base(repositoryContext)
    {
        _repositoryContext = repositoryContext;
    }

    public async Task<Product> GetBySkuAsync(string sku)
    {
        return await _repositoryContext.Set<Product>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Sku == sku);
    }
}
