﻿using EventBus.Messages.Events;
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
            // note: this method should only be called from the payment received consumer class - via the event bus
            // and only after a new order has been created and paid for, so a stock entry for this sku must already exist

            // ...but let's make sure
            var stockEntryExists = await _stockRepository.GetBySkuAsync(update.Sku);

            if (stockEntryExists != null)
            {
                // create an update table entry
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
            else
            {
                _logger.LogInformation("Inventory update attempted for non-existent Sku: {sku}", update.Sku);
            }
        }

        public async Task<Update> CreateUpdateFromAdmin(Update update)
        {
            // check for an existing entry in the stock table for this sku
            var skuToUpdate = await _stockRepository.GetBySkuAsync(update.Sku);
            if (skuToUpdate == null) return null;

            // if there is an existing stock entry for the sku, create an update entry
            var stockUpdate = await _updateRepository.Create(update);
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

            await _stockRepository.Update(skuToUpdate);

            // Send an InventoryZero event message to event bus if needed
            if (skuToUpdate.CurrentStock == 0)
            {
                await SendInventoryZeroEvent(skuToUpdate.Sku);
            }

            _logger.LogInformation("Inventory updated -> Sku: {sku}, Previous Stock: {prev}, Updated Stock: {upd}",
                skuToUpdate.Sku, originalStockCount, skuToUpdate.CurrentStock);
            
            return stockUpdate;
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
