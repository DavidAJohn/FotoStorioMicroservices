namespace Products.Aggregator.Services;

public interface IDiscountService
{
    Task<DiscountResponse> GetDiscountBySku(string sku);
    Task<List<DiscountResponse>> GetCurrentDiscounts();
}
