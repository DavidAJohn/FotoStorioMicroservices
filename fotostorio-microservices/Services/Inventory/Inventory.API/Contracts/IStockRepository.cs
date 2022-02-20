using Inventory.API.Entities;
using System.Threading.Tasks;

namespace Inventory.API.Contracts
{
    public interface IStockRepository : IBaseRepository<Stock>
    {
        Task<Stock> GetBySkuAsync(string sku);
    }
}
