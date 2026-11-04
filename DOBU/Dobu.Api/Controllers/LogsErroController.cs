using Dobu.Domain.Entities;
using Dobu.Application.Interfaces.Repositories;
using Dobu.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Dobu.Api.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/logs-erro")]
public class LogsErroController(IDobuUnitOfWork context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LogErro>>> GetAll([FromQuery] QueryParameters query)
    {
        return Ok(await context.LogsErro.ListAsync(query, includes: [x => x.Usuario]));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LogErro>> GetById(Guid id)
    {
        var log = await context.LogsErro.FirstOrDefaultAsync(x => x.Id == id, tracking: false);
        return log is null ? NotFound() : Ok(log);
    }

    [HttpGet("usuario/{usuarioId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LogErro>>> GetByUsuario(Guid usuarioId, [FromQuery] QueryParameters query)
    {
        return Ok(await context.LogsErro.ListAsync(query, predicate: x => x.UsuarioId == usuarioId));
    }

    [HttpGet("procedure/{nomeProcedure}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LogErro>>> GetByProcedure(string nomeProcedure, [FromQuery] QueryParameters query)
    {
        return Ok(await context.LogsErro.ListAsync(query, predicate: x => x.NomeProcedure == nomeProcedure));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LogErro>> Create(LogErroRequest request)
    {
        if (await context.Usuarios.CountAsync(x => x.Id == request.UsuarioId) == 0)
            return BadRequest("Usuario informado nao existe.");

        var log = new LogErro(request.NomeProcedure, request.DescricaoErro, request.DataErro, request.UsuarioId);
        context.LogsErro.Add(log);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = log.Id }, log);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LogErro>> Update(Guid id, LogErroRequest request)
    {
        var log = await context.LogsErro.FirstOrDefaultAsync(x => x.Id == id);
        if (log is null)
            return NotFound();

        if (await context.Usuarios.CountAsync(x => x.Id == request.UsuarioId) == 0)
            return BadRequest("Usuario informado nao existe.");

        log.Atualizar(request.NomeProcedure, request.DescricaoErro, request.DataErro, request.UsuarioId);
        await context.SaveChangesAsync();
        return Ok(log);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var log = await context.LogsErro.FirstOrDefaultAsync(x => x.Id == id);
        if (log is null)
            return NotFound();

        context.LogsErro.Remove(log);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public record LogErroRequest(string NomeProcedure, string DescricaoErro, DateTime DataErro, Guid UsuarioId);
