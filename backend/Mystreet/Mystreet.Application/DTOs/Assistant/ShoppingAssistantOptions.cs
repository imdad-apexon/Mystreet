namespace Mystreet.Application.DTOs.Assistant;

public class ShoppingAssistantOptions
{
    public string ShippingPolicy { get; set; } = "Standard delivery is 3-7 business days. Express shipping is 1-2 business days where available.";
    public string ReturnPolicy { get; set; } = "Returns are accepted within 30 days for unused products in original condition.";
    public string SupportPolicy { get; set; } = "Support is available Monday to Saturday, 9 AM to 6 PM.";
    public int MaxProductsInContext { get; set; } = 60;
}