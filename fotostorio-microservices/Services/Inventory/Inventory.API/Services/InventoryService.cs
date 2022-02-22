using EventBus.Messages.Events;
using Inventory.API.Contracts;
using Inventory.API.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        public async Task<bool> CreateUpdateFromPaymentReceived(Update update)
        {
            // note: this method should only be called from the payment received consumer class - via the event bus
            // and only after a new order has been created and paid for, so a stock entry for this sku must already exist

            try
            {
                // ...but let's make sure a stock entry exists for this sku
                var skuToUpdate = await _stockRepository.GetBySkuAsync(update.Sku);

                if (skuToUpdate == null)
                {
                    _logger.LogInformation("Inventory update attempted for non-existent Sku: {sku}", update.Sku);
                    return false;
                }

                // create an update table entry
                var stockUpdate = await _updateRepository.Create(update);

                if (stockUpdate == null)
                {
                    _logger.LogInformation("Inventory update failed for Sku: {sku}", update.Sku);
                    return false;
                }

                // keep the original stock count for later use
                var originalStockCount = skuToUpdate.CurrentStock;

                // calculate what the new stock level should be after the update
                int newStockCount = (skuToUpdate.CurrentStock - update.Removed) > 0 ? (skuToUpdate.CurrentStock - update.Removed) : 0;
                skuToUpdate.CurrentStock = newStockCount;

                skuToUpdate.LastUpdated = DateTime.Now;

                var updateSucceeded = await _stockRepository.Update(skuToUpdate);

                if (!updateSucceeded)
                {
                    _logger.LogInformation("Inventory stock entry failed for Sku: {sku}", update.Sku);
                    await _updateRepository.Delete(stockUpdate); // remove the now orphaned update entry we created above
                    return false;
                }

                // Send an InventoryZero event message to event bus if needed
                if (skuToUpdate.CurrentStock == 0)
                {
                    await SendInventoryZeroEvent(skuToUpdate.Sku);
                }

                _logger.LogInformation("Inventory successfully updated after payment received -> Sku: {sku}, Previous Stock: {prev}, Updated Stock: {upd}",
                    skuToUpdate.Sku, originalStockCount, skuToUpdate.CurrentStock);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating stock update for Sku: {sku} -> {message}", update.Sku, ex.Message);
                return false;
            }
        }

        public async Task<Update> CreateUpdateFromAdmin(UpdateCreateDTO update)
        {
            // check for an existing entry in the stock table for this sku
            var skuToUpdate = await _stockRepository.GetBySkuAsync(update.Sku);
            if (skuToUpdate == null) return null;

            // if there is an existing stock entry for the sku, create an update entry
            var updateToCreate = new Update
            {
                Sku = update.Sku,
                UpdatedAt = DateTime.Now,
                Added = update.Added,
                Removed = update.Removed
            };

            var stockUpdate = await _updateRepository.Create(updateToCreate);
            if (stockUpdate == null) return null;

            // keep the original stock count for later use
            var originalStockCount = skuToUpdate.CurrentStock;

            // after creating a stock update, calculate what the new stock level should be after the update
            if (update.Added > 0)
            {
                int newStockCount = (skuToUpdate.CurrentStock + update.Added);
                skuToUpdate.CurrentStock = newStockCount;
            }

            if (update.Removed > 0)
            {
                int newStockCount = (skuToUpdate.CurrentStock - update.Removed) > 0 ? (skuToUpdate.CurrentStock - update.Removed) : 0;
                skuToUpdate.CurrentStock = newStockCount;
            }

            skuToUpdate.LastUpdated = DateTime.Now;

            var updateSucceeded = await _stockRepository.Update(skuToUpdate);

            if (!updateSucceeded)
            {
                _logger.LogInformation("Inventory stock entry failed for Sku: {sku}", update.Sku);
                await _updateRepository.Delete(stockUpdate); // remove the now orphaned update entry we created above
                return null;
            }

            // Send an InventoryZero event message to event bus if needed
            if (skuToUpdate.CurrentStock == 0)
            {
                await SendInventoryZeroEvent(skuToUpdate.Sku);
            }

            _logger.LogInformation("Inventory updated -> Sku: {sku}, Previous Stock: {prev}, Updated Stock: {upd}",
                skuToUpdate.Sku, originalStockCount, skuToUpdate.CurrentStock);
            
            return stockUpdate;
        }

        public async Task<IEnumerable<Update>> GetUpdates()
        {
            var updates = await _updateRepository.ListAllAsync();
            if (updates == null) return null;

            return updates;
        }

        public async Task<IEnumerable<Update>> GetUpdatesBySku(string sku)
        {
            var updates = await _updateRepository.GetBySkuAsync(sku);
            if (updates == null) return null;

            return updates;
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
