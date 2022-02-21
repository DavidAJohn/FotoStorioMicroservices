using EventBus.Messages.Events;
using Inventory.API.Contracts;
using Inventory.API.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Inventory.API.EventBusConsumer
{
    public class PaymentReceivedConsumer : IConsumer<PaymentReceivedEvent>
    {
        private readonly ILogger<PaymentReceivedConsumer> _logger;
        private readonly IInventoryService _inventoryService;

        public PaymentReceivedConsumer(ILogger<PaymentReceivedConsumer> logger, IInventoryService inventoryService)
        {
            _logger = logger;
            _inventoryService = inventoryService;
        }

        public async Task Consume(ConsumeContext<PaymentReceivedEvent> context)
        {
            var message = context.Message;

            var stockUpdate = new Update
            {
                Sku = message.Sku,
                UpdatedAt = DateTime.Now,
                Added = 0,
                Removed = message.QuantityOrdered,
                OrderId = message.OrderId
            };

            // send the stockUpdate object to the InventoryService to create a new update
            await _inventoryService.CreateUpdateFromPaymentReceived(stockUpdate);

            _logger.LogInformation("PaymentReceivedEvent consumed successfully -> OrderId: {orderid}, Sku: {sku}, Quantity: {quantity}", 
                message.OrderId, message.Sku, message.QuantityOrdered);
        }
    }
}
