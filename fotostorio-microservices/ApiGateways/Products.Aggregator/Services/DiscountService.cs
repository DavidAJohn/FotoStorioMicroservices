namespace Products.Aggregator.Services;

public class DiscountService : IDiscountService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DiscountService> _logger;

    public DiscountService(HttpClient httpClient, ILogger<DiscountService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DiscountResponse> GetDiscountBySku(string sku)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/discounts/sku/{sku}");

            if (response == null) return null;

            return await response.ReadContentAs<DiscountResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DiscountService -> Unable to get discount for sku '{sku}': {message}", sku, ex.Message);
            return null;
        }
    }

    public async Task<List<DiscountResponse>> GetCurrentDiscounts()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/discounts/current");

            if (response == null) return null;

            return await response.ReadContentAs<List<DiscountResponse>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DiscountService -> Unable to get current discounts from api: {message}", ex.Message);
            return null;
        }
    }
}
