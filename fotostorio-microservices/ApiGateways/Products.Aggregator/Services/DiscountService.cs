namespace Products.Aggregator.Services;

public class DiscountService : IDiscountService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILogger<DiscountService> _logger;

    public DiscountService(IHttpClientFactory httpClient, ILogger<DiscountService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DiscountResponse> GetDiscountBySku(string sku)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/discounts/sku/{sku}");

            var client = _httpClient.CreateClient("Discounts");
            HttpResponseMessage response = await client.SendAsync(request);

            if (response == null)
            {
                _logger.LogWarning("Discount Service -> Response when attempting to GET discount for {sku} was NULL", sku);
                return null;
            }
            
            _logger.LogInformation("Discount Service -> Successfully retrieved discount for sku '{sku}'", sku);
            return await response.ReadContentAs<DiscountResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Discount Service -> Unable to get discount for sku '{sku}': {message}", sku, ex.Message);
            return null;
        }
    }

    public async Task<List<DiscountResponse>> GetCurrentDiscounts()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/discounts/current");

            var client = _httpClient.CreateClient("Discounts");
            HttpResponseMessage response = await client.SendAsync(request);

            if (response == null)
            {
                _logger.LogWarning("Discount Service -> Response when attempting to GET current discounts was NULL");
                return null;
            }

            _logger.LogInformation("Discount Service -> Successfully retrieved current discounts");
            return await response.ReadContentAs<List<DiscountResponse>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Discount Service -> Unable to get current discounts from api: {message}", ex.Message);
            return null;
        }
    }
}
