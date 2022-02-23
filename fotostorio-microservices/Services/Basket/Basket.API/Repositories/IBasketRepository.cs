using System.Threading.Tasks;
using Basket.API.Entities;

namespace Basket.API.Repositories
{
    public interface IBasketRepository
    {
        Task<CustomerBasket> GetBasketAsync(string basketId, string token);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket, string token);
        Task DeleteBasketAsync(string basketId, string token);
    }
}
