using Microsoft.EntityFrameworkCore;
using Products.API.Contracts;
using Products.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Products.API.Data
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _repositoryContext;

        public RepositoryBase(ApplicationDbContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _repositoryContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByCondition(Expression<Func<T, bool>> expression)
        {
            return await _repositoryContext.Set<T>()
                .Where(expression)
                .OrderBy(t => t.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<T> Create(T entity)
        {
            _repositoryContext.Set<T>().Add(entity);
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
    }
}
