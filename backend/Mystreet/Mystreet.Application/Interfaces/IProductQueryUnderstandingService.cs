using Mystreet.Application.DTOs.Products;

namespace Mystreet.Application.Interfaces;

public interface IProductQueryUnderstandingService
{
    Task<NaturalLanguageProductQuery> ParseAsync(string query, string? preferredModel = null, CancellationToken cancellationToken = default);
}