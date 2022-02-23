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

        public async Task UpdateProductAvailability(string sku, bool status)
        {
            var productToUpdate = await _productRepository.GetBySkuAsync(sku);

            if (productToUpdate != null)
            {
                var updateProduct = productToUpdate;
                updateProduct.IsAvailable = status;

                var response = await _productRepository.Update(updateProduct);

                if (response)
                {
                    _logger.LogInformation("Product availability updated for {sku} -> now set to {status}", sku, status);
                }
                else
                {
                    _logger.LogInformation("Product availability update failed for {sku} -> should have been set to {status}", sku, status);
                }
            }
        }
    }
}
