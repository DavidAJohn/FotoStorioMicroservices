﻿using Discount.minAPI.Models;

namespace Discount.minAPI.Data
{
    public interface IDiscountData
    {
        Task<bool> CreateDiscount(ProductDiscount discount);
        Task<bool> DeleteDiscount(int id);
        Task<IEnumerable<ProductDiscount>> GetAllDiscounts();
        Task<ProductDiscount> GetCurrentDiscountById(int id);
        Task<ProductDiscount> GetCurrentDiscountBySku(string sku);
        Task<IEnumerable<ProductDiscount>> GetCurrentDiscounts();
        Task<bool> UpdateDiscount(ProductDiscount discount);
        Task<IEnumerable<ProductDiscount>> GetCurrentAndFutureDiscounts();
        Task<IEnumerable<ProductDiscount>> GetDiscountsForSkuByDate(string sku, DateTime date);
        Task<IEnumerable<ProductDiscount>> GetDiscountsByCampaignId(int id);
    }
}