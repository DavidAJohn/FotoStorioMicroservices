using System.Net.Http;
using System.Net;
using System.Text.Json;

namespace Products.Aggregator.Services;

public class InventoryService : IInventoryService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(IHttpClientFactory httpClient, ILogger<InventoryService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<InventoryResponse> GetStockBySkuAsync(string sku)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/stock/{sku}");

            var client = _httpClient.CreateClient("Inventory");
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                var inventory = JsonSerializer.Deserialize<InventoryResponse>(content, _jsonSerializerOptions);

                _logger.LogInformation("Inventory Service -> Successfully retrieved inventory for product sku '{sku}'", sku);
                return inventory;
            }

            _logger.LogWarning("Inventory Service -> Response when attempting to GET inventory for product sku '{sku}' was '{statuscode}', rather than OK", sku, response.StatusCode.ToString());
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError("Error in Inventory Service -> GetStockBySkuAsync, Sku = {sku}", sku);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in Inventory Service -> GetStockBySkuAsync, Sku = {sku}", sku);
            throw new Exception(ex.Message, ex.InnerException);
        }
    }

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
