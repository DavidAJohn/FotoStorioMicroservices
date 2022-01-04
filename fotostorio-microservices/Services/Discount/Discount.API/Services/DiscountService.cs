using AutoMapper;
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
    }
}
