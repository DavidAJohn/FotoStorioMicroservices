using System.Globalization;

namespace Admin.BlazorServer.Pages;

public partial class SiteStatus
{
    string LastUpdated = "";
    bool AzureConfigured = false;
    string ApplicationStatusUrl = "";

    private string ImagePath = "";

    private string healthMessage = "Checking...";
    private string healthTextColour = "text-teal-600 ";
    private string ordersMessage = "Checking...";
    private string stockMessage = "Checking...";

    private string productCountToDisplay = "";
    private bool showProductCount = false;

    protected override void OnInitialized()
    {
        TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(config["DateTimeSettings:LocalTimeZone"] ?? "UTC");
        var dateTimeOffset = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, localTimeZone);
        LastUpdated = dateTimeOffset.ToString(config["DateTimeSettings:DateTimeFormat"] ?? "yyyy-MM-dd HH:mm:ss");

        ApplicationStatusUrl = $"{config["ApplicationStatusUrl"]}/hc-ui";

        ImagePath = config["ImageAssetsBaseURI"] ?? "http://localhost/images"; // default fallback image path

        CheckAzureConfig();
    }

    protected override async Task OnInitializedAsync()
    {
        await GetRecentOrderCount();
        await GetLowStockProductCount();
        await GetHealthCheckStatus();
        await GetProductCount();
    }

    private void CheckAzureConfig()
    {
        var storageAccountName = config["AzureSettings:AssetBaseUrl"];
        var maxFileUploadSize = config["AzureSettings:MaxFileUploadSize"];
        var fileUploadTypes = config["AzureSettings:FileUploadTypesAllowed"];

        if (string.IsNullOrWhiteSpace(storageAccountName) ||
            string.IsNullOrWhiteSpace(maxFileUploadSize) ||
            string.IsNullOrWhiteSpace(fileUploadTypes)
            )
        {
            AzureConfigured = false;
        }
        else
        {
            AzureConfigured = true;
        }
    }

    private async Task GetRecentOrderCount()
    {
        try
        {
            var days = config.GetValue<int>("StoreStatusSettings:RecentOrderDaysThreshold");
            var orders = await orderService.GetLatestOrdersAsync(days);

            if (orders != null)
            {
                string orderText = orders.Count == 1 ? "order" : "orders";
                ordersMessage = $"{orders.Count} {orderText} in last {days} days";
            }
            else
            {
                ordersMessage = "Unknown - try page refresh";
            }
        }
        catch (Exception)
        {
            ordersMessage = "Unknown - try page refresh";
        }
    }

    private async Task GetLowStockProductCount()
    {
        try
        {
            var lowStockThreshold = config.GetValue<int>("StoreStatusSettings:LowStockThreshold");

            if (lowStockThreshold >= 0)
            {
                var products = await inventoryService.GetInventoryAtOrBelowLevelAsync(lowStockThreshold);

                if (products != null)
                {
                    string productsText = products.Count == 1 ? "product" : "products";
                    stockMessage = $"{products.Count} {productsText}";
                }
            }
        }
        catch (Exception)
        {
            stockMessage = "Unknown - try page refresh";
        }
    }

    private async Task GetHealthCheckStatus()
    {
        try
        {
            var healthChecks = await healthService.GetHealthChecksAsync();

            if (healthChecks != null)
            {
                var problemCount = 0;

                foreach (var check in healthChecks)
                {
                    if (check.Status != "Healthy")
                    {
                        problemCount++;
                    }
                }

                if (problemCount == 0)
                {
                    healthTextColour = "text-green-600 ";
                    healthMessage = "Healthy";
                }
                else
                {
                    healthTextColour = problemCount > 1 ? "text-red-800 " : "text-yellow-600 ";
                    healthMessage = problemCount > 1 ? "Possible Issues" : "Possible Issue";
                }
            }
            else
            {
                healthMessage = "Unknown - check link below";
            }
        }
        catch (Exception)
        {
            stockMessage = "Unknown - check link below";
        }
    }

    private async Task GetProductCount()
    {
        var productCount = await productService.GetProductCountAsync();

        if (productCount == 0)
        {
            showProductCount = false;
        }
        else
        {
            productCountToDisplay = productCount.ToString();
            showProductCount = true;
        }
    }
}
