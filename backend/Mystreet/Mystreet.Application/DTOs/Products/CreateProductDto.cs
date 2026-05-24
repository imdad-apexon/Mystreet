namespace Mystreet.Application.DTOs.Products;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string SizesCsv { get; set; } = string.Empty;
    public int StockQty { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Category { get; set; } = "Sneakers";
}