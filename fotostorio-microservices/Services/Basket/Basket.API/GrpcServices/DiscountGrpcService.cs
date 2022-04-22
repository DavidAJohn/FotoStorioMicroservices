using Discount.Grpc.Protos;

namespace Basket.API.GrpcServices;

public class DiscountGrpcService
{
    private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoService;

    public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountProtoService)
    {
        _discountProtoService = discountProtoService;
    }

    public async Task<DiscountModel> GetDiscount(string sku)
    {
        var discountRequest = new GetCurrentDiscountBySkuRequest { Sku = sku };

        return await _discountProtoService.GetCurrentDiscountBySkuAsync(discountRequest);
    }
}
