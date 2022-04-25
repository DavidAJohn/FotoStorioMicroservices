using EventBus.Messages.Events;
using MassTransit;

namespace Products.API.EventBusConsumer;

public class InventoryRestoredConsumer : IConsumer<InventoryRestoredEvent>
{
    private readonly ILogger<InventoryRestoredConsumer> _logger;
    private readonly IProductsService _productsService;
    private readonly IProductRepository _productRepository;

    public InventoryRestoredConsumer(ILogger<InventoryRestoredConsumer> logger, IProductsService productsService, IProductRepository productRepository)
    {
        _logger = logger;
        _productsService = productsService;
        _productRepository = productRepository;
    }

    public async Task Consume(ConsumeContext<InventoryRestoredEvent> context)
    {
        var message = context.Message;
        var productToUpdate = await _productRepository.GetBySkuAsync(message.Sku);

        if (productToUpdate != null)
        {
            await _productsService.UpdateProductAvailability(productToUpdate.Sku, true);

            _logger.LogInformation("InventoryRestoredEvent consumed successfully -> Sku: {sku}", productToUpdate.Sku);
        }
    }
}
