using Ordering.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordering.API.Contracts
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(int id, string buyerEmail);
        Task<IEnumerable<Order>> GetOrdersForUserAsync(string token, string buyerEmail);
        Task<Order> GetOrderByPaymentIntentIdAsync(string paymentIntentId);
        Task<bool> UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(Order order);
        Task<Order> UpdateOrderPaymentStatus(string paymentIntentId, OrderStatus status);
    }
}
