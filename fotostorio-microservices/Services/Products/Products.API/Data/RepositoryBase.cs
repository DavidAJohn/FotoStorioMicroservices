using Microsoft.EntityFrameworkCore;

namespace Products.API.Data;

public class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _repositoryContext;

    public RepositoryBase(ApplicationDbContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
    }

    public async Task<IEnumerable<T>> ListAllAsync()
    {
        return await _repositoryContext.Set<T>()
            .ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _repositoryContext.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<T> GetEntityWithSpecification(ISpecification<T> specification)
    {
        return await ApplySpecification(specification).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> ListWithSpecificationAsync(ISpecification<T> specification)
    {
        return await ApplySpecification(specification).ToListAsync();
    }

    public async Task<int> CountAsync(ISpecification<T> specification)
    {
        return await ApplySpecification(specification).CountAsync();
    }

        public async Task<T> Create(T entity)
    {
        await _repositoryContext.Set<T>().AddAsync(entity);
        await Save();
        return entity;
    }

    public async Task<bool> Update(T entity)
    {
        _repositoryContext.Set<T>().Update(entity);
        return await Save();
    }

    public async Task<bool> Delete(T entity)
    {
        _repositoryContext.Set<T>().Remove(entity);
        return await Save();
    }

    public async Task<bool> Save()
    {
        var changes = await _repositoryContext.SaveChangesAsync();
        return changes > 0;
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> specification)
    {
        return SpecificationEvaluator<T>
            .GetQuery(_repositoryContext
                .Set<T>()
                .AsQueryable(), specification
            );
    }
}
