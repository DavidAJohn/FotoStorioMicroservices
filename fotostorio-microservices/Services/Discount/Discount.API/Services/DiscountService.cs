using Discount.Api.Protos;
using Discount.API.Contracts;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discount.API.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _repository;
        private readonly ILogger<DiscountService> _logger;

        public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public override async Task<GetAllDiscountsResponse> GetAllDiscounts(GetAllDiscountsRequest request, ServerCallContext context)
        {
            var discounts = await _repository.GetAllDiscountsAsync();

            if (discounts == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discounts not found (all)"));
            }

            _logger.LogInformation("Discounts retrieved via GetAllDiscountsAsync method");

            return discounts;
        }

        public override async Task<GetCurrentDiscountsResponse> GetCurrentDiscounts(GetCurrentDiscountsRequest request, ServerCallContext context)
        {
            var discounts = await _repository.GetCurrentDiscountsAsync();

            if (discounts == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discounts not found (current)"));
            }

            _logger.LogInformation("Discounts retrieved via GetCurrentDiscountsAsync method");

            return discounts;
        }

        public override async Task<DiscountModel> GetCurrentDiscountById(GetCurrentDiscountByIdRequest request, ServerCallContext context)
        {
            var discount = await _repository.GetCurrentDiscountByIdAsync(request.Id);

            if (discount == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with Id={request.Id} not found."));
            }

            _logger.LogInformation("Discount retrieved for Id : {id}", discount.Id);

            return discount;
        }

        public override async Task<DiscountModel> GetCurrentDiscountBySku(GetCurrentDiscountBySkuRequest request, ServerCallContext context)
        {
            var discount = await _repository.GetCurrentDiscountBySkuAsync(request.Sku);

            if (discount == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with Sku={request.Sku} not found."));
            }

            _logger.LogInformation("Discount retrieved for Sku : {sku}", discount.Sku);

            return discount;
        }
    }
}
