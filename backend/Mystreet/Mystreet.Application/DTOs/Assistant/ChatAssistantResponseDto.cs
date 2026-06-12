using Mystreet.Application.DTOs.Products;

namespace Mystreet.Application.DTOs.Assistant;

public class ChatAssistantResponseDto
{
    public string Reply { get; set; } = string.Empty;
    public List<ProductDto> RecommendedProducts { get; set; } = [];
}