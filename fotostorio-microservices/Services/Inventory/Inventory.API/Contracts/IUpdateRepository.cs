using Inventory.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.API.Contracts
{
    public interface IUpdateRepository : IBaseRepository<Update>
    {
        Task<IEnumerable<Update>> GetBySkuAsync(string sku);
    }
}
