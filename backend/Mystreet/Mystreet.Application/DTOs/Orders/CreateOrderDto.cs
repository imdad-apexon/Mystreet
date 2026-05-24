namespace Mystreet.Application.DTOs.Orders;

public class CreateOrderDto
{
    public string ShippingAddress { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public List<CreateOrderItemDto> Items { get; set; } = new();
}