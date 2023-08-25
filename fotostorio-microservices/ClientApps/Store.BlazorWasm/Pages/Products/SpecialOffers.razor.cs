using Microsoft.AspNetCore.Components;
using Store.BlazorWasm.DTOs;
using Store.BlazorWasm.Models;
using Store.BlazorWasm.Shared;

namespace Store.BlazorWasm.Pages.Products;

public partial class SpecialOffers
{
    [CascadingParameter]
    public ErrorLogger Error { get; set; }

    [CascadingParameter]
    public AppState appState { get; set; }

    List<Product> products;

    List<DropdownItem> dropdownOptions = new();

    private bool collapseDropdown = true;

    

    private void ToggleDropdown()
    {
        collapseDropdown = !collapseDropdown;
    }

    private string SortBySelected = "Price (Highest to Lowest)";

    private ProductParameters productParams = new();

    private string errorMessage = "";

    private int itemQuantity = 1;

    protected override void OnInitialized()
    {
        SetSortByDropdownOptions();
    }

    protected override async Task OnInitializedAsync()
    {
        await GetProducts();
    }

    private async Task GetProducts()
    {
        try
        {
            string sortBy = string.IsNullOrEmpty(productParams.SortBy) ? "priceDesc" : productParams.SortBy;

            products = await productService.GetProductsOnSpecialOfferAsync(sortBy);

            errorMessage = "";
        }
        catch (Exception ex)
        {
            Error.ProcessError(ex, "Pages/Products/SpecialOffers.GetProducts()");
            errorMessage = "Could not retrieve list of products";
        }
    }

    private async Task AddSort(string SortBy, string SortByName)
    {
        productParams.SortBy = SortBy;
        SortBySelected = SortByName;
        ToggleDropdown();
        await GetProducts();
    }

    private void SetSortByDropdownOptions()
    {
        dropdownOptions.Add(new DropdownItem { Id = 1, OptionName = "Name (A-Z)", OptionRef = "nameAsc" });
        dropdownOptions.Add(new DropdownItem { Id = 2, OptionName = "Amount Saved (Highest First)", OptionRef = "savingDesc" });
        dropdownOptions.Add(new DropdownItem { Id = 3, OptionName = "Price (Highest to Lowest)", OptionRef = "priceDesc" });
        dropdownOptions.Add(new DropdownItem { Id = 4, OptionName = "Price (Lowest to Highest)", OptionRef = "priceAsc" });
        dropdownOptions.Add(new DropdownItem { Id = 5, OptionName = "Brand (A-Z)", OptionRef = "brandAsc" });
        dropdownOptions.Add(new DropdownItem { Id = 6, OptionName = "Name (Z-A)", OptionRef = "nameDesc" });
    }

    private void GoToHomePage()
    {
        navigationManager.NavigateTo("/");
    }

    private async Task AddProductToBasket(int productId)
    {
        var product = new Product();

        try
        {
            product = await productService.GetProductByIdAsync(productId);
            errorMessage = "";
        }
        catch (Exception ex)
        {
            Error.ProcessError(ex, "Pages/Products/SpecialOffers.AddItemToBasket()");
            errorMessage = "Could not add this product to your basket";
        }

        if (product != null)
        {
            var storedBasketId = await localStorage.GetItemAsync<string>("fs_basketId");

            if (String.IsNullOrEmpty(storedBasketId))
            {
                var newBasket = new Basket
                {
                    Id = Guid.NewGuid().ToString(),
                    BasketItems = { }
                };

                var storedBasket = await basketService.UpdateBasket(newBasket);
                storedBasketId = storedBasket.Id;

                await localStorage.SetItemAsync("fs_basketId", storedBasketId);
            }

            var currentBasket = await basketService.GetBasketByID(storedBasketId);

            var newProduct = new ProductDTO
            {
                Id = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Brand = product.Brand,
                Category = product.Category,
                Mount = product.Mount
            };

            var newItem = new BasketItem { Product = newProduct, Quantity = itemQuantity };
            currentBasket.BasketItems.Add(newItem);

            var updatedBasket = await basketService.UpdateBasket(currentBasket);

            if (updatedBasket == null)
            {
                toastService.ShowError("Sorry, there was a problem adding this item to your basket", "Basket Update Failed");
            }
            else
            {
                toastService.ShowSuccess("A new item has been added to your basket", "Basket Updated");

                // also set AppState.BasketItemCount to update the item count displayed in the BasketDropdown
                appState.BasketItemCount = updatedBasket.BasketItems.Count;
            }
        }
        else
        {
            errorMessage = "Could not add this product to your basket";
        }
    }
}
