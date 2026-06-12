using Mystreet.Application.DTOs.Assistant;

namespace Mystreet.Application.Interfaces;

public interface IShoppingAssistantService
{
    Task<ChatAssistantResponseDto> AskAsync(ChatAssistantRequestDto request, CancellationToken cancellationToken = default);
}