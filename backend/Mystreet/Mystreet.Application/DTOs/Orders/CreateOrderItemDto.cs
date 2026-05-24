namespace Mystreet.Application.DTOs.Orders;

public class CreateOrderItemDto
{
    public Guid ProductId { get; set; }
    public string Size { get; set; } = string.Empty;
    public int Quantity { get; set; }
}