using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.API.Contracts
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> ListAllAsync();
        Task<T> Create(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
    }
}
