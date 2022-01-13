using Discount.Grpc.Models;

namespace Discount.Grpc.Contracts;

public interface IDiscountRepository
{
    Task<ProductDiscount> GetCurrentDiscountByIdAsync(int id);
    Task<ProductDiscount> GetCurrentDiscountBySkuAsync(string sku);
    Task<bool> CreateDiscountAsync(ProductDiscount discount);
    Task<bool> UpdateDiscountAsync(ProductDiscount discount);
    Task<bool> DeleteDiscountAsync(int id);
}
