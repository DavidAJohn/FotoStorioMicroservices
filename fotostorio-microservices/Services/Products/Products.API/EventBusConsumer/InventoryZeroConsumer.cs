using EventBus.Messages.Events;
using MassTransit;

namespace Products.API.EventBusConsumer;

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
            await _productsService.UpdateProductAvailability(productToUpdate.Sku, false);

            _logger.LogInformation("InventoryZeroEvent consumed successfully -> Sku: {sku}", productToUpdate.Sku);
        }
    }
}
