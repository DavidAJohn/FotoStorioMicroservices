using Ordering.API.Contracts;
using Ordering.API.Entities;
using Ordering.API.Models;
using System.Collections.Generic;
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

        public async Task<Order> CreateOrderAsync(string buyerEmail, Basket basket, Address sendToAddress)
        {
            var orderItems = new List<OrderItem>();

            foreach (var basketItem in basket.BasketItems)
            {
                var productItemOrdered = new ProductItemOrdered
                {
                    ProductItemId = basketItem.Product.Id,
                    ProductSku = basketItem.Product.Sku,
                    ProductName = basketItem.Product.Name
                };
                
                orderItems.Add(new OrderItem
                {
                    ItemOrdered = productItemOrdered,
                    Price = basketItem.Product.Price,
                    Quantity = basketItem.Quantity
                });
            }

            var order = new Order(orderItems, buyerEmail, sendToAddress, basket.BasketTotal, "");

            await _orderDbContext.Orders.AddAsync(order);
            var saved = await _orderDbContext.SaveChangesAsync();

            if (saved > 0) return order;

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
