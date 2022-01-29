using Store.BlazorWasm.DTOs;

namespace Store.BlazorWasm.Contracts;

public interface IOrderService
{
    Task<OrderDTO> CreateOrderAsync(OrderCreateDTO order);
}
