using MassTransit;
using EventBus.Messages.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ordering.API.Contracts;
using Ordering.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Headers;

namespace Ordering.API.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _orderDbContext;
        private readonly IPaymentService _paymentService;
        private readonly IHttpClientFactory _httpClient;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderRepository(OrderDbContext orderDbContext, IPaymentService paymentService, IHttpClientFactory httpClient, IPublishEndpoint publishEndpoint)
        {
            _orderDbContext = orderDbContext;
            _paymentService = paymentService;
            _httpClient = httpClient;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Order> CreateOrderAsync(Order order, string token)
        {
            // check that jwt is valid
            var validToken = await IsTokenValid(token);
            if (!validToken) return null;

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

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail, string token)
        {
            // check that jwt is valid
            var validToken = await IsTokenValid(token);
            if (!validToken) return null;

            var order = await _orderDbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.BuyerEmail == buyerEmail);

            if (order == null) return null;

            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersForUserAsync(string token, string buyerEmail)
        {
            // check that jwt is valid
            var validToken = await IsTokenValid(token);
            if (!validToken) return null;
            
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

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            _orderDbContext.Orders.Update(order);
            var saved = await _orderDbContext.SaveChangesAsync();

            return saved > 0;
        }

        public async Task DeleteOrderAsync(Order order)
        {
            _orderDbContext.Orders.Remove(order);

            await _orderDbContext.SaveChangesAsync();
        }

        public async Task<Order> UpdateOrderPaymentStatus(string paymentIntentId, OrderStatus status)
        {
            var order = await GetOrderByPaymentIntentIdAsync(paymentIntentId);
            if (order == null) return null;

            order.Status = status;
            await UpdateOrderAsync(order);

            // if order status has changed to 'payment received' - send event(s) to the event bus (RabbitMq) using MassTransit
            if (order.Status == OrderStatus.PaymentReceived)
            {
                foreach (var item in order.Items)
                {
                    var eventMessage = new PaymentReceivedEvent // from ./AsyncMessaging/EventBus.Messages class library
                    {
                        Sku = item.Product.Sku,
                        QuantityOrdered = item.Quantity,
                        OrderId = order.Id
                    };

                    await _publishEndpoint.Publish(eventMessage);
                }
            }
            
            return order;
        }

        private async Task<bool> IsTokenValid(string token)
        {
            var client = _httpClient.CreateClient("IdentityAPI");
            HttpContent serializedContent = new StringContent(JsonSerializer.Serialize(token));
            serializedContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage tokenResponse = await client.PostAsync("/api/accounts/token", serializedContent);

            if (!tokenResponse.IsSuccessStatusCode) return false;

            return true;
        }
    }
}
