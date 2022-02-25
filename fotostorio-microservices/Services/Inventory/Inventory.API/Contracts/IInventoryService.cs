using Inventory.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.API.Contracts
{
    public interface IInventoryService
    {
        Task<bool> CreateUpdateFromPaymentReceived(Update update);
        Task<Update> CreateUpdateFromAdmin(UpdateCreateDTO update, string token);
        Task<IEnumerable<Update>> GetUpdates(string token);
        Task<IEnumerable<Update>> GetUpdatesBySku(string sku, string token);
    }
}
