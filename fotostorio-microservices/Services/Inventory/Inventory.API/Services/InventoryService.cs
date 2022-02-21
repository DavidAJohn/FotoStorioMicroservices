using Inventory.API.Contracts;
using Inventory.API.Entities;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Inventory.API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ILogger<InventoryService> _logger;
        private readonly IUpdateRepository _updateRepository;
        private readonly IStockRepository _stockRepository;

        public InventoryService(ILogger<InventoryService> logger, IUpdateRepository updateRepository, IStockRepository stockRepository)
        {
            _logger = logger;
            _updateRepository = updateRepository;
            _stockRepository = stockRepository;
        }

        public async Task CreateUpdateFromPaymentReceived(Update update)
        {
            var stockUpdate = await _updateRepository.Create(update);

            if (stockUpdate != null)
            {
                var skuToUpdate = await _stockRepository.GetBySkuAsync(stockUpdate.Sku);
                var originalStockCount = skuToUpdate.CurrentStock;

                if (skuToUpdate != null)
                {
                    int newStockCount = (skuToUpdate.CurrentStock - update.Removed) > 0 ? (skuToUpdate.CurrentStock - update.Removed) : 0;
                    skuToUpdate.CurrentStock = newStockCount;

                    await _stockRepository.Update(skuToUpdate);

                    // TODO: send InventoryZero event message to event bus if needed
                    //if (skuToUpdate.CurrentStock == 0)
                    //{
                    //}

                    _logger.LogInformation("Inventory updated after payment received -> Sku: {sku}, Previous Stock: {prev}, Updated Stock: {upd}",
                        skuToUpdate.Sku, originalStockCount, skuToUpdate.CurrentStock);
                }
            }
        }
    }
}
