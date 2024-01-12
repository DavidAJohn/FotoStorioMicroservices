using Microsoft.AspNetCore.Components;
using Store.BlazorWasm.DTOs;
using Store.BlazorWasm.Models;
using Store.BlazorWasm.Contracts;
using Store.BlazorWasm.Shared;

namespace Store.BlazorWasm.Pages.Products;

public partial class Details
{
    [Parameter]
    public int Id { get; set; }

    [CascadingParameter]
    public ErrorLogger Error { get; set; }

    private Product product;
    private string errorMessage = "";
    private int itemQuantity = 1;
    private int scarcityLevel = 3;

    [CascadingParameter]
    public AppState appState { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetProductById();
        itemQuantity = 1;
    }

    private async Task GetProductById()
    {
        try
        {
            product = await productService.GetProductByIdAsync(Id);
            errorMessage = "";
        }
        catch (Exception ex)
        {
            Error.ProcessError(ex, "Pages/Products/Details.GetProductById()");
            errorMessage = "Could not retrieve details for this product";
        }
    }

    private async void AddItemToBasket()
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
}
