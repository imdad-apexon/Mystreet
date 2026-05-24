using Mystreet.Application.DTOs.Orders;

namespace Mystreet.Application.Interfaces;

public interface IOrderService
{
    Task<Guid> CreateAsync(Guid userId, CreateOrderDto dto);
    Task<IEnumerable<object>> GetMineAsync(Guid userId);
    Task<object?> GetByIdAsync(Guid userId, Guid orderId, bool isAdmin);
    Task<bool> CancelAsync(Guid userId, Guid orderId, bool isAdmin);
}