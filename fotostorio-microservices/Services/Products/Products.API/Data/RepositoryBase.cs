using Microsoft.EntityFrameworkCore;
using Products.API.Contracts;
using Products.API.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Products.API.Data
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _repositoryContext;

        public RepositoryBase(ApplicationDbContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public IQueryable<T> GetAll()
        {
            return _repositoryContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression)
        {
            return _repositoryContext.Set<T>()
                .Where(expression).AsNoTracking();
        }
    }
}
