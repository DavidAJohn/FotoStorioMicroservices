using Inventory.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.API.Contracts
{
    public interface IStockRepository : IBaseRepository<Stock>
    {
        Task<Stock> GetBySkuAsync(string sku);
        Task<IEnumerable<Stock>> GetByStockLevelAtOrBelow(int stockLevel);
    }
}
