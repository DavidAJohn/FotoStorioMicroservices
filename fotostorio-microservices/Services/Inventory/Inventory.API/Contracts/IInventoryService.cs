using Inventory.API.Entities;
using System.Threading.Tasks;

namespace Inventory.API.Contracts
{
    public interface IInventoryService
    {
        Task CreateUpdateFromPaymentReceived(Update update);
        Task<Update> CreateUpdateFromAdmin(Update update);
    }
}
