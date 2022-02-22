using AutoMapper;
using Microsoft.Extensions.Logging;
using Products.API.Contracts;
using Products.API.DTOs;
using Products.API.Models;
using System.Threading.Tasks;

namespace Products.API.Services
{
    public class ProductsService : IProductsService
    {
        private readonly ILogger<ProductsService> _logger;
        private readonly IProductRepository _productRepository;

        public ProductsService(ILogger<ProductsService> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task UpdateProductStockCount(string sku, int quantity)
        {
            var productToUpdate = await _productRepository.GetBySkuAsync(sku);

            if (productToUpdate != null)
            {
                var updateProduct = productToUpdate;
                updateProduct.IsAvailable = quantity == 0 ? false : true;

                await _productRepository.Update(updateProduct);

                _logger.LogInformation("Stock level for {sku} is {quantity} -> IsAvailable set to {status}", 
                    sku, quantity, quantity == 0 ? false : true);
            }
        }
    }
}
