using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mystreet.Application.DTOs.Products;
using Mystreet.Application.Interfaces;

namespace Mystreet.Application.Services;

public class OllamaProductQueryUnderstandingService : IProductQueryUnderstandingService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly HashSet<string> StopWords =
    [
        "a", "an", "and", "for", "the", "to", "of", "with", "who", "that", "is", "in", "on", "at", "from",
        "under", "below", "less", "than", "over", "above", "more", "gift", "best", "good", "comfortable"
    ];

    private readonly OllamaOptions _options;
    private readonly ILogger<OllamaProductQueryUnderstandingService> _logger;

    public OllamaProductQueryUnderstandingService(
        IOptions<OllamaOptions> options,
        ILogger<OllamaProductQueryUnderstandingService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<NaturalLanguageProductQuery> ParseAsync(string query, string? preferredModel = null, CancellationToken cancellationToken = default)
    {
        var cleanQuery = query?.Trim() ?? string.Empty;
        if (cleanQuery.Length == 0)
            return new NaturalLanguageProductQuery();

        var parsed = await TryParseWithOllamaAsync(cleanQuery, preferredModel, cancellationToken);
        if (parsed is not null)
            return Normalize(parsed, cleanQuery);

        return ParseByRules(cleanQuery);
    }

    private async Task<NaturalLanguageProductQuery?> TryParseWithOllamaAsync(string query, string? preferredModel, CancellationToken cancellationToken)
    {
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(Math.Max(3, _options.TimeoutSeconds));

            var model = string.IsNullOrWhiteSpace(preferredModel) ? _options.DefaultModel : preferredModel.Trim();
            var baseUrl = _options.BaseUrl.TrimEnd('/');

            var prompt = BuildPrompt(query);
            var request = new OllamaGenerateRequest
            {
                Model = model,
                Prompt = prompt,
                Stream = false,
                Format = "json"
            };

            var response = await client.PostAsJsonAsync($"{baseUrl}/api/generate", request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Ollama parse returned status code {StatusCode}", response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>(cancellationToken: cancellationToken);
            if (result is null || string.IsNullOrWhiteSpace(result.Response))
                return null;

            return JsonSerializer.Deserialize<NaturalLanguageProductQuery>(result.Response, JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Ollama unavailable for product parsing. Using rule-based parser.");
            return null;
        }
    }

    private static string BuildPrompt(string query)
    {
        return
            "You are a product-search query parser. Convert user text into JSON with this exact schema: " +
            "{\"brand\":string|null,\"category\":string|null,\"size\":string|null,\"minPrice\":number|null,\"maxPrice\":number|null,\"keywords\":string[]}. " +
            "Rules: keep only concrete purchase intent words in keywords, do not invent values, return valid compact JSON only. " +
            $"User query: {query}";
    }

    private static NaturalLanguageProductQuery ParseByRules(string query)
    {
        var normalized = query.Trim();

        decimal? maxPrice = TryParsePrice(normalized, @"(?:under|below|less\s+than|upto|up\s*to)\s*\$?\s*(\d+(?:\.\d{1,2})?)");
        decimal? minPrice = TryParsePrice(normalized, @"(?:over|above|more\s+than|at\s+least)\s*\$?\s*(\d+(?:\.\d{1,2})?)");

        var keywords = Regex.Matches(normalized.ToLowerInvariant(), @"[a-z0-9]+")
            .Select(m => m.Value)
            .Where(w => !StopWords.Contains(w) && w.Length > 1)
            .Distinct()
            .Take(8)
            .ToList();

        return new NaturalLanguageProductQuery
        {
            OriginalQuery = normalized,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Keywords = keywords
        };
    }

    private static NaturalLanguageProductQuery Normalize(NaturalLanguageProductQuery parsed, string originalQuery)
    {
        parsed.OriginalQuery = originalQuery;
        parsed.Brand = NormalizeField(parsed.Brand);
        parsed.Category = NormalizeField(parsed.Category);
        parsed.Size = NormalizeField(parsed.Size);

        parsed.Keywords = (parsed.Keywords ?? [])
            .Select(k => k.Trim().ToLowerInvariant())
            .Where(k => k.Length > 1 && !StopWords.Contains(k))
            .Distinct()
            .Take(12)
            .ToList();

        if (parsed.MinPrice.HasValue && parsed.MaxPrice.HasValue && parsed.MinPrice > parsed.MaxPrice)
            (parsed.MinPrice, parsed.MaxPrice) = (parsed.MaxPrice, parsed.MinPrice);

        return parsed;
    }

    private static string? NormalizeField(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return value.Trim().ToLowerInvariant();
    }

    private static decimal? TryParsePrice(string text, string pattern)
    {
        var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
        if (!match.Success) return null;

        return decimal.TryParse(match.Groups[1].Value, out var value) ? value : null;
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
}