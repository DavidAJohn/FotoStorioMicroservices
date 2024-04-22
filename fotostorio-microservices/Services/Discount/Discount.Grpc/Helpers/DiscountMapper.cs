using Discount.Grpc.Models;
using Discount.Grpc.Protos;

namespace Discount.Grpc.Helpers;

public static class DiscountMapper
{
    public static DiscountModel ToDiscountModel(this ProductDiscount productDiscount)
    {
        return new DiscountModel
        {
            Id = productDiscount.Id,
            Sku = productDiscount.Sku,
            SalePrice = (double)productDiscount.SalePrice
        };
    }

    public static ProductDiscount ToProductDiscount(this DiscountModel discountModel)
    {
        return new ProductDiscount
        {
            Id = discountModel.Id,
            Sku = discountModel.Sku,
            SalePrice = (decimal)discountModel.SalePrice
        };
    }
}
