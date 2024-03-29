﻿using Store.BlazorWasm.DTOs;

namespace Store.BlazorWasm.Contracts;

public interface IOrderService
{
    Task<OrderDTO> CreateOrderAsync(OrderCreateDTO order);
    Task<List<OrderDetailsDTO>> GetOrdersForUserAsync();
    Task<OrderDetailsDTO> GetOrderByIdAsync(int orderId);
}
