using EventBus.Messages.Events;
using Inventory.API.Contracts;
using Inventory.API.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Inventory.API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ILogger<InventoryService> _logger;
        private readonly IUpdateRepository _updateRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public InventoryService(ILogger<InventoryService> logger, IUpdateRepository updateRepository, IStockRepository stockRepository, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _updateRepository = updateRepository;
            _stockRepository = stockRepository;
            _publishEndpoint = publishEndpoint;
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

                    // Send an InventoryZero event message to event bus if needed
                    if (skuToUpdate.CurrentStock == 0)
                    {
                        await SendInventoryZeroEvent(skuToUpdate.Sku);
                    }

                    _logger.LogInformation("Inventory updated after payment received -> Sku: {sku}, Previous Stock: {prev}, Updated Stock: {upd}",
                        skuToUpdate.Sku, originalStockCount, skuToUpdate.CurrentStock);
                }
            }
        }

        private async Task SendInventoryZeroEvent(string sku)
        {
            // Send InventoryZeroEvent to the event bus (RabbitMq) using MassTransit

            var eventMessage = new InventoryZeroEvent // from ./AsyncMessaging/EventBus.Messages class library
            {
                Sku = sku
            };

            await _publishEndpoint.Publish(eventMessage);

            _logger.LogInformation("InventoryZeroEvent published to event bus for Sku: {sku}", sku);
        }
    }
}
