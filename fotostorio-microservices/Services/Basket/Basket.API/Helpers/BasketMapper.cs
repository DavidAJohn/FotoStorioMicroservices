namespace Basket.API.Helpers;

public static class BasketMapper
{
    public static CustomerBasket ToCustomerBasket(this CustomerBasketDTO customerBasketDto)
    {
        var customerBasket = new CustomerBasket
        {
            Id = customerBasketDto.Id,
            BasketItems = customerBasketDto.BasketItems,
            ClientSecret = customerBasketDto.ClientSecret,
            PaymentIntentId = customerBasketDto.PaymentIntentId
        };

        return customerBasket;
    }
}
