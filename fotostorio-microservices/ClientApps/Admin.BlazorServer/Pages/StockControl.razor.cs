using Admin.BlazorServer.Components;
using Admin.BlazorServer.Models;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace Admin.BlazorServer.Pages;

public partial class StockControl
{
    [CascadingParameter]
    public IModalService Modal { get; set; }

    [Parameter]
    public string? searchTerm { get; set; }

    List<ProductStock>? products = null;

    List<DropdownItem> dropdownOptions = new();

    private bool collapseDropdown = true;

    private void ToggleDropdown()
    {
        collapseDropdown = !collapseDropdown;
    }

    private string FilterBySelected = "5";

    private string errorMessage = "";

    protected override async Task OnInitializedAsync()
    {
        await GetProducts();
    }

    protected override void OnInitialized()
    {
        SetSortByDropdownOptions();
    }

    private async Task GetProducts()
    {
        try
        {
            int result;
            var tryStockParse = int.TryParse(FilterBySelected, out result);
            var stockLevel = tryStockParse ? result : 0;

            if (stockLevel == 0)
            {
                products = await inventoryService.GetInventoryAsync();
            }
            else
            {
                products = await inventoryService.GetInventoryAtOrBelowLevelAsync(stockLevel);
            }

            errorMessage = "";
        }
        catch (Exception)
        {
            errorMessage = "Could not retrieve list of products";
        }
    }

    private async Task UpdateStockLevelFilter(string FilterBy, string FilterByName)
    {
        FilterBySelected = FilterByName;
        ToggleDropdown();
        await GetProducts();
    }

    private void SetSortByDropdownOptions()
    {
        dropdownOptions.Add(new DropdownItem { Id = 1, OptionName = "All Stock", OptionRef = "0" });
        dropdownOptions.Add(new DropdownItem { Id = 2, OptionName = "25", OptionRef = "25" });
        dropdownOptions.Add(new DropdownItem { Id = 3, OptionName = "10", OptionRef = "10" });
        dropdownOptions.Add(new DropdownItem { Id = 4, OptionName = "5", OptionRef = "5" });
    }

    private async Task AddStock(string? sku, string? name, int currentStock)
    {
        var parameters = new ModalParameters();
        parameters.Add(nameof(AddStockModal.Sku), sku);
        parameters.Add(nameof(AddStockModal.Name), name);
        parameters.Add(nameof(AddStockModal.CurrentStock), currentStock);

        var addStockModal = Modal.Show<AddStockModal>("", parameters);
        var result = await addStockModal.Result;

        if (!result.Cancelled)
        {
            var requestResponse = (UpdateStockResult)result.Data;

            if (requestResponse.Successful == false)
            {
                errorMessage = requestResponse.Error == null ? "There was an error updating this product" : requestResponse.Error;
            }
            else
            {
                errorMessage = "";
                ToastService.ShowSuccess($"The stock level for '{name}' has been updated", "Stock Updated");
                await GetProducts();
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    private async Task RemoveStock(string? sku, string? name, int currentStock)
    {
        var parameters = new ModalParameters();
        parameters.Add(nameof(RemoveStockModal.Sku), sku);
        parameters.Add(nameof(RemoveStockModal.Name), name);
        parameters.Add(nameof(RemoveStockModal.CurrentStock), currentStock);

        var removeStockModal = Modal.Show<RemoveStockModal>("", parameters);
        var result = await removeStockModal.Result;

        if (!result.Cancelled)
        {
            var requestResponse = (UpdateStockResult)result.Data;

            if (requestResponse.Successful == false)
            {
                errorMessage = requestResponse.Error == null ? "There was an error updating this product" : requestResponse.Error;
            }
            else
            {
                errorMessage = "";
                ToastService.ShowSuccess($"The stock level for '{name}' has been updated", "Stock Updated");
                await GetProducts();
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    private void GoToHomePage()
    {
        navigationManager.NavigateTo("/");
    }
}
