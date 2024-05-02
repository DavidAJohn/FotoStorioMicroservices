namespace Discount.minAPI.Data;

public class DiscountData : IDiscountData
{
    private readonly ISqlDiscountAccess _sqlDiscountAccess;

    public DiscountData(ISqlDiscountAccess sqlDiscountAccess)
    {
        _sqlDiscountAccess = sqlDiscountAccess;
    }

    public async Task<IEnumerable<ProductDiscount>> GetAllDiscounts() =>
        await _sqlDiscountAccess.GetAllDiscountsAsync();

    public async Task<IEnumerable<ProductDiscount>> GetCurrentDiscounts() =>
        await _sqlDiscountAccess.GetCurrentDiscountsAsync();

    public async Task<IEnumerable<ProductDiscount>> GetCurrentAndFutureDiscounts() =>
        await _sqlDiscountAccess.GetCurrentAndFutureDiscountsAsync();

    public async Task<ProductDiscount> GetCurrentDiscountById(int id) =>
        await _sqlDiscountAccess.GetCurrentDiscountByIdAsync(id);

    public async Task<ProductDiscount> GetCurrentDiscountBySku(string sku) =>
        await _sqlDiscountAccess.GetCurrentDiscountBySkuAsync(sku);

    public async Task<IEnumerable<ProductDiscount>> GetDiscountsForSkuByDate(string sku, DateTime date) =>
        await _sqlDiscountAccess.GetDiscountsForSkuByDateAsync(sku, date);

    public async Task<IEnumerable<ProductDiscount>> GetDiscountsByCampaignId(int id) =>
        await _sqlDiscountAccess.GetDiscountsByCampaignIdAsync(id);

    public async Task<bool> CreateDiscount(ProductDiscount discount) =>
        await _sqlDiscountAccess.CreateDiscountAsync(discount);

    public async Task<bool> UpdateDiscount(ProductDiscount discount) =>
        await _sqlDiscountAccess.UpdateDiscountAsync(discount);

    public async Task<bool> DeleteDiscount(int id) =>
        await _sqlDiscountAccess.DeleteDiscountAsync(id);
}
