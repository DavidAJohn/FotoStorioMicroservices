using Discount.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discount.API.Contracts
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<ProductDiscount>> GetAllDiscountsAsync();
        Task<IEnumerable<ProductDiscount>> GetCurrentDiscountsAsync();
        Task<ProductDiscount> GetCurrentDiscountByIdAsync(int id);
        Task<ProductDiscount> GetCurrentDiscountBySkuAsync(string sku);
        Task<bool> CreateDiscountAsync(ProductDiscount discount);
        Task<bool> UpdateDiscountAsync(ProductDiscount discount);
        Task<bool> DeleteDiscountAsync(int id);
    }
}
