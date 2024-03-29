﻿namespace Ordering.API.Contracts;

public interface IOrderRepository
{
    Task<Order> CreateOrderAsync(Order order, string token);
    Task<Order> GetOrderByIdAsync(int id, string buyerEmail, string token);
    Task<Order> GetOrderByIdForAdminAsync(int id, string token);
    Task<IEnumerable<Order>> GetOrdersForUserAsync(string token, string buyerEmail);
    Task<IEnumerable<Order>> GetLatestOrdersAsync(string token, int days);
    Task<Order> GetOrderByPaymentIntentIdAsync(string paymentIntentId);
    Task<bool> UpdateOrderAsync(Order order);
    Task DeleteOrderAsync(Order order);
    Task<Order> UpdateOrderPaymentStatus(string paymentIntentId, OrderStatus status);
}
