using Inventory.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.API.Contracts
{
    public interface IInventoryService
    {
        Task CreateUpdateFromPaymentReceived(Update update);
        Task<Update> CreateUpdateFromAdmin(UpdateCreateDTO update);
        Task<IEnumerable<Update>> GetUpdates();
        Task<IEnumerable<Update>> GetUpdatesBySku(string sku);
    }
}
