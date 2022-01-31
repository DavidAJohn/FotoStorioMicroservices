using Microsoft.EntityFrameworkCore;
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

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var order = await _orderDbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.BuyerEmail == buyerEmail);

            if (order == null) return null;

            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var orders = await _orderDbContext.Orders
                .Where(o => o.BuyerEmail == buyerEmail)
                .Include(o => o.Items)
                .ToListAsync();

            if (orders == null) return null;

            return orders;
        }
    }
}
