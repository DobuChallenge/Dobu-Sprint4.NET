using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dobu.Api.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/pagamentos")]
public class PagamentosController(DobuDbContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Pagamento>>> GetAll()
    {
        return Ok(await context.Pagamentos.AsNoTracking().Include(x => x.Consulta).ToListAsync());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Pagamento>> GetById(Guid id)
    {
        var pagamento = await context.Pagamentos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return pagamento is null ? NotFound() : Ok(pagamento);
    }

    [HttpGet("consulta/{consultaId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Pagamento>> GetByConsulta(Guid consultaId)
    {
        var pagamento = await context.Pagamentos.AsNoTracking().FirstOrDefaultAsync(x => x.ConsultaId == consultaId);
        return pagamento is null ? NotFound() : Ok(pagamento);
    }

    [HttpGet("forma/{formaPagamento}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Pagamento>>> GetByForma(string formaPagamento)
    {
        return Ok(await context.Pagamentos.AsNoTracking().Where(x => x.FormaPagamento == formaPagamento).ToListAsync());
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Pagamento>> Create(PagamentoRequest request)
    {
        if (await context.Consultas.CountAsync(x => x.Id == request.ConsultaId) == 0)
            return BadRequest("Consulta informada nao existe.");

        if (await context.Pagamentos.CountAsync(x => x.ConsultaId == request.ConsultaId) > 0)
            return BadRequest("Consulta ja possui pagamento cadastrado.");

        try
        {
            var pagamento = new Pagamento(request.Valor, request.FormaPagamento, request.DataPagamento, request.ConsultaId);
            context.Pagamentos.Add(pagamento);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = pagamento.Id }, pagamento);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Pagamento>> Update(Guid id, PagamentoRequest request)
    {
        var pagamento = await context.Pagamentos.FirstOrDefaultAsync(x => x.Id == id);
        if (pagamento is null)
            return NotFound();

        if (await context.Consultas.CountAsync(x => x.Id == request.ConsultaId) == 0)
            return BadRequest("Consulta informada nao existe.");

        try
        {
            pagamento.Atualizar(request.Valor, request.FormaPagamento, request.DataPagamento, request.ConsultaId);
            await context.SaveChangesAsync();
            return Ok(pagamento);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var pagamento = await context.Pagamentos.FirstOrDefaultAsync(x => x.Id == id);
        if (pagamento is null)
            return NotFound();

        context.Pagamentos.Remove(pagamento);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public record PagamentoRequest(decimal Valor, string FormaPagamento, DateTime DataPagamento, Guid ConsultaId);
