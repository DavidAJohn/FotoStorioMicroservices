using Admin.BlazorServer.DTOs;
using Admin.BlazorServer.Models;

namespace Admin.BlazorServer.Contracts;

public interface IOrderService
{
    Task<List<OrderDetailsDTO>?> GetLatestOrdersAsync(int days);
    Task<OrderDetailsDTO?> GetOrderByIdAsync(int orderId);
}
