using Mystreet.Domain.Enums;

namespace Mystreet.Application.DTOs.Orders;

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}
