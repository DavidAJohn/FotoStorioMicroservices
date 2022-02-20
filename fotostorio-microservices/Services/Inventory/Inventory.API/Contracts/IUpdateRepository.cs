using Inventory.API.Entities;
using System.Threading.Tasks;

namespace Inventory.API.Contracts
{
    public interface IUpdateRepository : IBaseRepository<Update>
    {
        Task<Update> GetBySkuAsync(string sku);
    }
}
