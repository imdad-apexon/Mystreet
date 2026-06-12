namespace Mystreet.Application.DTOs.Products;

public class NaturalLanguageProductQuery
{
    public string OriginalQuery { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public string? Size { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<string> Keywords { get; set; } = [];
}