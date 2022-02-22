using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Products.API.Contracts;
using System.Threading.Tasks;

namespace Products.API.EventBusConsumer
{
    public class InventoryZeroConsumer : IConsumer<InventoryZeroEvent>
    {
        private readonly ILogger<InventoryZeroConsumer> _logger;
        private readonly IProductsService _productsService;
        private readonly IProductRepository _productRepository;

        public InventoryZeroConsumer(ILogger<InventoryZeroConsumer> logger, IProductsService productsService, IProductRepository productRepository)
        {
            _logger = logger;
            _productsService = productsService;
            _productRepository = productRepository;
        }

        public async Task Consume(ConsumeContext<InventoryZeroEvent> context)
        {
            var message = context.Message;
            var productToUpdate = await _productRepository.GetBySkuAsync(message.Sku);

            if (productToUpdate != null)
            {
                await _productsService.UpdateProductStockCount(productToUpdate.Sku, 0);

                _logger.LogInformation("InventoryZeroEvent consumed successfully -> Sku: {sku}", productToUpdate.Sku);
            }
        }
    }
}
