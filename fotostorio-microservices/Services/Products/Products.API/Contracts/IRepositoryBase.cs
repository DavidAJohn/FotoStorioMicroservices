using Products.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.API.Contracts
{
    public interface IRepositoryBase<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> ListAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> GetEntityWithSpecification(ISpecification<T> specification);
        Task<IEnumerable<T>> ListWithSpecificationAsync(ISpecification<T> specification);
        Task<T> Create(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
    }
}
