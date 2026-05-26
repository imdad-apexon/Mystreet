using Mystreet.Application.DTOs.Orders;
using Mystreet.Domain.Enums;

namespace Mystreet.Application.Interfaces;

public interface IOrderService
{
    Task<Guid> CreateAsync(Guid userId, CreateOrderDto dto);
    Task<IEnumerable<object>> GetMineAsync(Guid userId);
    Task<IEnumerable<object>> GetAllAsync();
    Task<object?> GetByIdAsync(Guid userId, Guid orderId, bool isAdmin);
    Task<bool> CancelAsync(Guid userId, Guid orderId, bool isAdmin);
    Task<bool> UpdateStatusAsync(Guid orderId, OrderStatus status);
}