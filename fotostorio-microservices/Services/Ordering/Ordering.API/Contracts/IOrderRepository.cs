using Ordering.API.Entities;
using Ordering.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordering.API.Contracts
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(string buyerEmail, Basket basket, Address sendToAddress);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail);
        Task<Order> GetOrderByIdAsync(int id, string buyerEmail);
    }
}
