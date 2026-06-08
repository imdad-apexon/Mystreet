using System.ComponentModel.DataAnnotations;

namespace Mystreet.Application.DTOs.Products;

public class CreateProductDto
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Brand { get; set; } = string.Empty;

    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal Price { get; set; }

    [StringLength(100)]
    public string SizesCsv { get; set; } = string.Empty;

    [Range(0, 10000)]
    public int StockQty { get; set; }

    [StringLength(2000)]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = "Sneakers";
}