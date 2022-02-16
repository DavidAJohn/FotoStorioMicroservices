﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ordering.API.Contracts;
using Ordering.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Ordering.API.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _orderDbContext;
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _client;

        public OrderRepository(OrderDbContext orderDbContext, IPaymentService paymentService, IConfiguration config, IHttpClientFactory client)
        {
            _orderDbContext = orderDbContext;
            _paymentService = paymentService;
            _config = config;
            _client = client;
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

        public async Task<IEnumerable<Order>> GetOrdersForUserAsync(string token, string buyerEmail)
        {
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

            return order;
        }

        private async Task<bool> IsTokenValid(string token)
        {
            var identityUri = _config["ApiSettings:IdentityUri"] + "/api/accounts/token";
            var client = _client.CreateClient();
            var tokenResponse = await client.PostAsJsonAsync(identityUri, token);

            if (!tokenResponse.IsSuccessStatusCode) return false;

            return true;
        }
    }
}
