﻿using Discount.minAPI.Models;

namespace Discount.minAPI.DbAccess;

public interface ISqlDiscountAccess
{
    Task<bool> CreateDiscountAsync(ProductDiscount discount);
    Task<bool> DeleteDiscountAsync(int id);
    Task<IEnumerable<ProductDiscount>> GetAllDiscountsAsync();
    Task<ProductDiscount> GetCurrentDiscountByIdAsync(int id);
    Task<ProductDiscount> GetCurrentDiscountBySkuAsync(string sku);
    Task<IEnumerable<ProductDiscount>> GetCurrentDiscountsAsync();
    Task<bool> UpdateDiscountAsync(ProductDiscount discount);
    Task<IEnumerable<ProductDiscount>> GetCurrentAndFutureDiscountsAsync();
    Task<IEnumerable<ProductDiscount>> GetDiscountsForSkuByDateAsync(string sku, DateTime date);
    Task<IEnumerable<ProductDiscount>> GetDiscountsByCampaignIdAsync(int id);
}
