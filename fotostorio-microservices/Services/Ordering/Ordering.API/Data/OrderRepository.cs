using Ordering.API.Contracts;
using Ordering.API.Entities;
using Ordering.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _orderDbContext;

        public OrderRepository(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            // verify the prices are accurate?
            // - get basket for redis again?
            // - get prices via store gateway/product aggregator?

            // calculate the total
            var orderTotal = order.Items.Sum(item => item.Total);

            var orderToCreate = new Order(order.Items, order.BuyerEmail, order.SendToAddress, orderTotal, "");

            await _orderDbContext.Orders.AddAsync(orderToCreate);
            var saved = await _orderDbContext.SaveChangesAsync();

            if (saved > 0) return orderToCreate;

            return null;
        }

        public Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            throw new System.NotImplementedException();
        }

        public Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            throw new System.NotImplementedException();
        }
    }
}
