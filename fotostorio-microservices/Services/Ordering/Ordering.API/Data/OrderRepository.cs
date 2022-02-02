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
        private readonly IPaymentService _paymentService;

        public OrderRepository(OrderDbContext orderDbContext, IPaymentService paymentService)
        {
            _orderDbContext = orderDbContext;
            _paymentService = paymentService;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            // verify the prices are accurate?
            // - get basket for redis again?
            // - get prices via store gateway/product aggregator?

            // calculate the total
            var orderTotal = order.Items.Sum(item => item.Total);

            // check for an existing order
            var existingOrder = await GetOrderByPaymentIntentIdAsync(order.PaymentIntentId);

            if (existingOrder != null)
            {
                await DeleteOrderAsync(existingOrder);

                var items = new List<BasketItem>();
                foreach (var item in order.Items)
                {
                    items.Add(new BasketItem
                    {
                        Quantity = item.Quantity,
                        Product = new Product
                        {
                            Id = item.Product.Id,
                            Sku = item.Product.Sku,
                            Name = item.Product.Name,
                            Price = item.Product.Price,
                            ImageUrl = item.Product.ImageUrl
                        }
                    });
                }

                var pi = new PaymentIntentCreateDTO
                {
                    Items = items,
                    PaymentIntentId = order.PaymentIntentId
                };

                await _paymentService.CreateOrUpdatePaymentIntent(pi);
            }

            // create the order and save changes
            var orderToCreate = new Order(order.Items, order.BuyerEmail, order.SendToAddress, orderTotal, order.PaymentIntentId);

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

        public async Task<Order> GetOrderByPaymentIntentIdAsync(string paymentIntentId)
        {
            var order = await _orderDbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntentId);

            if (order == null) return null;

            return order;
        }

        public async Task DeleteOrderAsync(Order order)
        {
            _orderDbContext.Orders.Remove(order);

            await _orderDbContext.SaveChangesAsync();
        }
    }
}
