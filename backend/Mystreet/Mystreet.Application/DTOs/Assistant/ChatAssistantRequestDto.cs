namespace Mystreet.Application.DTOs.Assistant;

public class ChatAssistantRequestDto
{
    public string Message { get; set; } = string.Empty;
    public string? Model { get; set; }
    public int? ProductLimit { get; set; }
}