using System.Security.Claims;
using Dobu.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dobu.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/agendamentos/{agendamentoId:guid}/chat")]
public class ChatAgendamentoController(IChatAgendamentoService chat) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ChatMessageResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid agendamentoId, CancellationToken cancellationToken) =>
        Ok(await chat.ListAsync(agendamentoId, CurrentUserId(), cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(ChatMessageResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Post(Guid agendamentoId, ChatMessageRequest request, CancellationToken cancellationToken) =>
        Ok(await chat.SendAsync(agendamentoId, CurrentUserId(), request, cancellationToken));

    private Guid CurrentUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
        ? id : throw new UnauthorizedAccessException("Sessão inválida.");
}
