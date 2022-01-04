using AutoMapper;
using Discount.Api.Protos;
using Discount.API.Contracts;
using Discount.API.Models;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Discount.API.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _repository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<DiscountModel> GetCurrentDiscountById(GetCurrentDiscountByIdRequest request, ServerCallContext context)
        {
            var discount = await _repository.GetCurrentDiscountByIdAsync(request.Id);

            if (discount == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with Id={request.Id} not found."));
            }

            _logger.LogInformation("Discount retrieved for Id : {id}", discount.Id);

            var discountModel = _mapper.Map<DiscountModel>(discount);
            return discountModel;
        }

        public override async Task<DiscountModel> GetCurrentDiscountBySku(GetCurrentDiscountBySkuRequest request, ServerCallContext context)
        {
            var discount = await _repository.GetCurrentDiscountBySkuAsync(request.Sku);

            if (discount == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with Sku={request.Sku} not found."));
            }

            _logger.LogInformation("Discount retrieved for Sku : {sku}", discount.Sku);

            var discountModel = _mapper.Map<DiscountModel>(discount);
            return discountModel;
        }

        public override async Task<DiscountModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var discount = _mapper.Map<ProductDiscount>(request.Discount);

            await _repository.CreateDiscountAsync(discount);
            _logger.LogInformation("Discount was successfully created for Sku : {Sku}, SalePrice : {SalePrice}", discount.Sku, discount.SalePrice);

            var discountModel = _mapper.Map<DiscountModel>(discount);
            return discountModel;
        }

        public override async Task<DiscountModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var discount = _mapper.Map<ProductDiscount>(request.Discount);

            await _repository.UpdateDiscountAsync(discount);
            _logger.LogInformation("Discount was successfully updated - Sku : {Sku}, SalePrice : {SalePrice}", discount.Sku, discount.SalePrice);

            var discountModel = _mapper.Map<DiscountModel>(discount);
            return discountModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var deleted = await _repository.DeleteDiscountAsync(request.Id);

            var response = new DeleteDiscountResponse
            {
                Success = deleted
            };

            _logger.LogInformation("Discount with Id:{Id} was deleted", request.Id);

            return response;
        }
    }
}
