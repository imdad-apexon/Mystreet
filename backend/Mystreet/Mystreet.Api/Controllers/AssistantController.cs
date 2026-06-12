using Microsoft.AspNetCore.Mvc;
using Mystreet.Application.DTOs.Assistant;
using Mystreet.Application.Interfaces;

namespace Mystreet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssistantController : ControllerBase
{
    private readonly IShoppingAssistantService _assistant;

    public AssistantController(IShoppingAssistantService assistant)
    {
        _assistant = assistant;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat(ChatAssistantRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("message is required.");

        return Ok(await _assistant.AskAsync(request, cancellationToken));
    }
}