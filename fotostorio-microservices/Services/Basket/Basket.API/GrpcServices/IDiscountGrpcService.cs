using Discount.Grpc.Protos;

namespace Basket.API.GrpcServices;

public interface IDiscountGrpcService
{
    Task<DiscountModel> GetDiscount(string sku);
}
