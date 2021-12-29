using Products.API.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Products.API.Contracts
{
    public interface IRepositoryBase<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression);
    }
}
