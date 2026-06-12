using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mystreet.Application.DTOs.Assistant;
using Mystreet.Application.DTOs.Products;
using Mystreet.Application.Interfaces;
using Mystreet.Infrastructure.Data;

namespace Mystreet.Application.Services;

public class ShoppingAssistantService : IShoppingAssistantService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly AppDbContext _db;
    private readonly OllamaOptions _ollamaOptions;
    private readonly ShoppingAssistantOptions _assistantOptions;
    private readonly ILogger<ShoppingAssistantService> _logger;

    public ShoppingAssistantService(
        AppDbContext db,
        IOptions<OllamaOptions> ollamaOptions,
        IOptions<ShoppingAssistantOptions> assistantOptions,
        ILogger<ShoppingAssistantService> logger)
    {
        _db = db;
        _ollamaOptions = ollamaOptions.Value;
        _assistantOptions = assistantOptions.Value;
        _logger = logger;
    }

    public async Task<ChatAssistantResponseDto> AskAsync(ChatAssistantRequestDto request, CancellationToken cancellationToken = default)
    {
        var message = request.Message?.Trim() ?? string.Empty;
        if (message.Length == 0)
            throw new InvalidOperationException("Message is required.");

        var products = await _db.Products
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Take(Math.Clamp(_assistantOptions.MaxProductsInContext, 10, 200))
            .Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Brand = x.Brand,
                Description = x.Description,
                Price = x.Price,
                SizesCsv = x.SizesCsv,
                StockQty = x.StockQty,
                ImageUrl = x.ImageUrl,
                Category = x.Category
            })
            .ToListAsync(cancellationToken);

        var policies = BuildPolicies();
        var recommendedProducts = RecommendProducts(products, message, request.ProductLimit);

        var aiReply = await TryGenerateReplyWithOllamaAsync(
            message,
            products,
            policies,
            recommendedProducts,
            request.Model,
            cancellationToken);

        return new ChatAssistantResponseDto
        {
            Reply = aiReply ?? BuildFallbackReply(message, policies, recommendedProducts),
            RecommendedProducts = recommendedProducts
        };
    }

    private List<AssistantPolicyItemDto> BuildPolicies()
    {
        return
        [
            new AssistantPolicyItemDto { Title = "Shipping Policy", Content = _assistantOptions.ShippingPolicy },
            new AssistantPolicyItemDto { Title = "Return Policy", Content = _assistantOptions.ReturnPolicy },
            new AssistantPolicyItemDto { Title = "Support Policy", Content = _assistantOptions.SupportPolicy }
        ];
    }

    private static List<ProductDto> RecommendProducts(List<ProductDto> products, string message, int? preferredLimit)
    {
        var minPrice = TryParsePrice(message, @"(?:over|above|more\s+than|at\s+least|minimum|min)\s*\$?\s*(\d+(?:\.\d{1,2})?)");
        var maxPrice = TryParsePrice(message, @"(?:under|below|less\s+than|up\s*to|upto|maximum|max)\s*\$?\s*(\d+(?:\.\d{1,2})?)");

        if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
            (minPrice, maxPrice) = (maxPrice, minPrice);

        var keywords = message
            .ToLowerInvariant()
            .Split([' ', ',', '.', ';', ':', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => x.Length > 2)
            .Distinct()
            .ToList();

        var filteredProducts = products
            .Where(x => !minPrice.HasValue || x.Price >= minPrice.Value)
            .Where(x => !maxPrice.HasValue || x.Price <= maxPrice.Value)
            .ToList();

        var limit = Math.Clamp(preferredLimit ?? 4, 1, 8);
        if (keywords.Count == 0)
            return filteredProducts.Where(x => x.StockQty > 0).OrderBy(x => x.Price).Take(limit).ToList();

        var ranked = filteredProducts
            .Select(p => new
            {
                Product = p,
                Score = keywords.Sum(k =>
                    (p.Name.Contains(k, StringComparison.OrdinalIgnoreCase) ? 4 : 0)
                    + (p.Brand.Contains(k, StringComparison.OrdinalIgnoreCase) ? 3 : 0)
                    + (p.Category.Contains(k, StringComparison.OrdinalIgnoreCase) ? 2 : 0)
                    + (p.Description.Contains(k, StringComparison.OrdinalIgnoreCase) ? 1 : 0))
            })
            .Where(x => x.Score > 0 && x.Product.StockQty > 0)
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Product.Price)
            .Select(x => x.Product)
            .Take(limit)
            .ToList();

        if (ranked.Count > 0)
            return ranked;

        return filteredProducts.Where(x => x.StockQty > 0).OrderBy(x => x.Price).Take(limit).ToList();
    }

    private static decimal? TryParsePrice(string text, string pattern)
    {
        var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
        if (!match.Success) return null;
        return decimal.TryParse(match.Groups[1].Value, out var value) ? value : null;
    }

    private async Task<string?> TryGenerateReplyWithOllamaAsync(
        string message,
        List<ProductDto> products,
        List<AssistantPolicyItemDto> policies,
        List<ProductDto> recommendations,
        string? preferredModel,
        CancellationToken cancellationToken)
    {
        try
        {
            using var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(Math.Max(3, _ollamaOptions.TimeoutSeconds))
            };

            var model = string.IsNullOrWhiteSpace(preferredModel) ? _ollamaOptions.DefaultModel : preferredModel.Trim();
            var baseUrl = _ollamaOptions.BaseUrl.TrimEnd('/');

            var prompt = BuildPrompt(message, products, policies, recommendations);
            var response = await client.PostAsJsonAsync(
                $"{baseUrl}/api/generate",
                new OllamaGenerateRequest
                {
                    Model = model,
                    Prompt = prompt,
                    Stream = false,
                    Format = "json"
                },
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Shopping assistant call returned status code {StatusCode}", response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>(cancellationToken: cancellationToken);
            if (result is null || string.IsNullOrWhiteSpace(result.Response))
                return null;

            var parsed = JsonSerializer.Deserialize<AssistantModelResponse>(result.Response, JsonOptions);
            if (!string.IsNullOrWhiteSpace(parsed?.Answer))
                return parsed.Answer.Trim();

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Ollama unavailable for shopping assistant. Using fallback reply.");
            return null;
        }
    }

    private static string BuildPrompt(
        string message,
        List<ProductDto> products,
        List<AssistantPolicyItemDto> policies,
        List<ProductDto> recommendations)
    {
        var context = JsonSerializer.Serialize(new
        {
            products = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Brand,
                p.Category,
                p.Price,
                p.StockQty,
                p.Description
            }),
            policies
        });

        var recommendedContext = JsonSerializer.Serialize(recommendations.Select(x => new
        {
            x.Id,
            x.Name,
            x.Brand,
            x.Price,
            x.Category
        }));

        return
            "You are MyStreet Shopping Assistant. Answer customer questions about products, shipping policy, return policy, and recommendations. " +
            "Use only provided context. Be concise and practical. Output strict JSON only with schema {\"answer\":string}. " +
            $"User message: {message}\n" +
            $"Context: {context}\n" +
            $"Pre-ranked recommendations: {recommendedContext}";
    }

    private static string BuildFallbackReply(string message, List<AssistantPolicyItemDto> policies, List<ProductDto> recommendations)
    {
        var text = message.ToLowerInvariant();

        if (text.Contains("shipping"))
            return policies.First(x => x.Title == "Shipping Policy").Content;

        if (text.Contains("return") || text.Contains("refund"))
            return policies.First(x => x.Title == "Return Policy").Content;

        if (recommendations.Count == 0)
            return "I can help with product recommendations, shipping, and returns. Share what you need and your budget.";

        return $"Based on your request, I recommend: {string.Join(", ", recommendations.Select(x => x.Name))}.";
    }

    private sealed class OllamaGenerateRequest
    {
        public string Model { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
        public bool Stream { get; set; }
        public string Format { get; set; } = "json";
    }

    private sealed class OllamaGenerateResponse
    {
        public string Response { get; set; } = string.Empty;
    }

    private sealed class AssistantModelResponse
    {
        public string Answer { get; set; } = string.Empty;
    }
}