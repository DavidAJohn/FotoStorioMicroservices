using Store.BlazorWasm.Models;

namespace Store.BlazorWasm.Contracts;

public interface IBasketService
{
    Task<Basket> GetBasketByID(string id);
    Task<Basket> UpdateBasket(Basket basket);
    Task DeleteBasket(string id);
}
