using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Store.BlazorWasm.DTOs;
using Store.BlazorWasm.Models;
using Store.BlazorWasm.Shared;

namespace Store.BlazorWasm.Pages.Checkout;

public partial class OrderSummary
{
    [CascadingParameter]
    public ErrorLogger Error { get; set; }

    [CascadingParameter]
    public AppState appState { get; set; }

    private List<BasketItem> basketItems { get; set; }
    private string basketId = "";
    private decimal basketTotal = 0;

    private AddressDTO addressDTO = new();
    private bool ShowErrors;
    private string ErrorMessage;

    private bool ShowPaymentOptions = false;
    private string SubmitSpinnerHidden = "hidden";
    private string AddressFormID = "address-form";
    private bool ShowAddressExplanationText = false;

    protected override async Task OnInitializedAsync()
    {
        await GetBasket();

        // populate the address form with the user's default address (if they have one)
        addressDTO = await accountService.GetUserAddressAsync();

        if (!string.IsNullOrWhiteSpace(addressDTO.Street))
        {
            ShowAddressExplanationText = true;
        }
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ShowPaymentOptions = true;
            var clientSecret = "";
            var storedBasketId = await localStorage.GetItemAsync<string>("fs_basketId");

            if (storedBasketId != null)
            {
                // get a payment intent from Stripe and update the basket
                await GetPaymentIntentResultAsync(storedBasketId);

                // get the client secret from the basket
                var basket = await basketService.GetBasketByID(storedBasketId);
                clientSecret = basket.ClientSecret;

                try
                {
                    var jsParams = new
                    {
                        Key = config["Stripe_PublishableKey"],
                        Secret = clientSecret
                    };

                    // initialise our custom stripe js file, so the card entry form is displayed 
                    await JsRuntime.InvokeVoidAsync("setupStripe", jsParams); // wwwroot/js/stripe.js
                }
                catch (JSException ex)
                {
                    Error.ProcessError(ex, "Pages/Checkout/OrderSummary.OnAfterRenderAsync()");
                    ErrorMessage = "Could not set up payment options. Try reloading the page and/or logging in again";
                }
            }

            firstRender = false;

            if (basketItems == null)
            {
                ShowPaymentOptions = false;
            }
        }
    }

    public async Task GetPaymentIntentResultAsync(string basketId)
    {
        var basket = await basketService.GetBasketByID(basketId);
        var result = await paymentService.CreateOrUpdatePaymentIntent(basket);

        if (result != null)
        {
            // add created payment intent details to the basket
            basket.PaymentIntentId = result.PaymentIntentId;
            basket.ClientSecret = result.ClientSecret;

            await basketService.UpdateBasket(basket);
        }
    }

    private async Task GetBasket()
    {
        var storedBasketId = await localStorage.GetItemAsync<string>("fs_basketId");

        if (!String.IsNullOrEmpty(storedBasketId))
        {
            basketId = storedBasketId;

            var basket = await basketService.GetBasketByID(basketId);

            if (basket != null)
            {
                basketItems = basket.BasketItems;
                basketTotal = basket.BasketTotal;
                ShowPaymentOptions = true;
            }
            else
            {
                basketItems = null;
            }
        }
        else
        {
            basketItems = null;
        }
    }

    private async Task EmptyBasket()
    {
        await basketService.DeleteBasket(basketId);
        await localStorage.RemoveItemAsync("fs_basketId");

        await GetBasket();
        appState.BasketItemCount = 0;
    }

    private async Task DeleteItem(BasketItem item)
    {
        var basket = await basketService.GetBasketByID(basketId);

        basket.BasketItems.Remove(basket.BasketItems.FirstOrDefault(
                i => i.Product.Name == item.Product.Name &&
                i.Quantity == item.Quantity
            ));

        await basketService.UpdateBasket(basket);
        await GetBasket();
    }

    private async Task PlaceOrder()
    {
        ShowErrors = false;
        SubmitSpinnerHidden = "";

        var orderAddress = new Address
        {
            FirstName = addressDTO.FirstName,
            LastName = addressDTO.LastName,
            Street = addressDTO.Street,
            SecondLine = addressDTO.SecondLine,
            City = addressDTO.City,
            County = addressDTO.County,
            PostCode = addressDTO.PostCode
        };

        var storedBasketId = await localStorage.GetItemAsync<string>("fs_basketId");
        var basket = await basketService.GetBasketByID(storedBasketId);

        var orderToCreate = new OrderCreateDTO
        {
            Items = basketItems,
            SendToAddress = orderAddress,
            PaymentIntentId = basket.PaymentIntentId
        };

        var newOrder = await orderService.CreateOrderAsync(orderToCreate);

        if (newOrder != null)
        {
            string paymentResult = "";
            SubmitSpinnerHidden = "hidden";

            try
            {
                // invoke js method to submit payment
                paymentResult = await JsRuntime.InvokeAsync<string>("payWithCard");
            }
            catch (JSException ex)
            {
                Error.ProcessError(ex, "Pages/Checkout/OrderSummary.PlaceOrder()");
                paymentResult = "There was a problem sending your payment details";
            }

            if (paymentResult != "success")
            {
                ShowErrors = true;
                ErrorMessage = paymentResult;
                toastService.ShowError(paymentResult, "Payment Failed");
            }
            else
            {
                ShowErrors = false;

                // empty the basket of items
                await EmptyBasket();

                // toast message
                toastService.ShowSuccess("Thanks, your order has been placed!", "Order Placed");

                // save the address to the user's account
                await accountService.SaveUserAddressAsync(addressDTO);

                // navigate away to confirmation page
                navigationManager.NavigateTo($"/checkout/success/{newOrder.OrderId}");
            }
        }
        else
        {
            ShowErrors = true;
            ErrorMessage = "Sorry, there was a problem creating your order";
            SubmitSpinnerHidden = "hidden";
        }
    }

    private void ClearAddress()
    {
        addressDTO = new AddressDTO { };
    }
}
