using Discount.Grpc.Protos;
using Discount.Grpc.Contracts;
using Discount.Grpc.Models;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Discount.Grpc.Helpers;

namespace Discount.Grpc.Services;

public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
{
    private readonly IDiscountRepository _repository;
    private readonly ILogger<DiscountService> _logger;

    public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override async Task<DiscountModel> GetCurrentDiscountById(GetCurrentDiscountByIdRequest request, ServerCallContext context)
    {
        var discount = await _repository.GetCurrentDiscountByIdAsync(request.Id);

        if (discount == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Discount with Id={request.Id} not found."));
        }

        _logger.LogInformation("Discount retrieved for Id : {id}", discount.Id);

        return discount.ToDiscountModel();
    }

    public override async Task<DiscountModel> GetCurrentDiscountBySku(GetCurrentDiscountBySkuRequest request, ServerCallContext context)
    {
        var discount = await _repository.GetCurrentDiscountBySkuAsync(request.Sku);

        if (discount == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Discount with Sku={request.Sku} not found."));
        }

        _logger.LogInformation("Discount retrieved for Sku : {sku}", discount.Sku);

        return discount.ToDiscountModel();
    }

    public override async Task<DiscountModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var discount = request.Discount.ToProductDiscount();

        await _repository.CreateDiscountAsync(discount);
        _logger.LogInformation("Discount was successfully created for Sku : {Sku}, SalePrice : {SalePrice}", discount.Sku, discount.SalePrice);

        return discount.ToDiscountModel();
    }

    public override async Task<DiscountModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var discount = request.Discount.ToProductDiscount();

        await _repository.UpdateDiscountAsync(discount);
        _logger.LogInformation("Discount was successfully updated - Sku : {Sku}, SalePrice : {SalePrice}", discount.Sku, discount.SalePrice);

        return discount.ToDiscountModel();
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
