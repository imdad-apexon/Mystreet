namespace Mystreet.Application.DTOs.Products;

public class OllamaOptions
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string DefaultModel { get; set; } = "llama3.1:8b";
    public int TimeoutSeconds { get; set; } = 20;
}