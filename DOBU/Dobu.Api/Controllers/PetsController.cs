using Dobu.Application.Services;
using Dobu.Domain.Entities;
using Dobu.Application.Interfaces.Repositories;
using Dobu.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Dobu.Api.Controllers;

[ApiController]
[Route("api/pets")]
[Authorize]
public class PetsController(IDobuUnitOfWork context, PetService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pet>>> GetAll([FromQuery] QueryParameters query)
    {
        return Ok(await context.Pets.ListAsync(query, includes: [x => x.Raca, x => x.Responsavel]));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Pet>> GetById(Guid id)
    {
        var pet = await context.Pets.FirstOrDefaultAsync(x => x.Id == id, tracking: false, includes: [x => x.Raca, x => x.Responsavel]);

        return pet is null ? NotFound() : Ok(pet);
    }

    [HttpGet("responsavel/{responsavelId:guid}")]
    public async Task<ActionResult<IEnumerable<Pet>>> GetByResponsavel(Guid responsavelId, [FromQuery] QueryParameters query)
    {
        return Ok(await context.Pets.ListAsync(query, predicate: x => x.ResponsavelId == responsavelId));
    }

    [HttpGet("raca/{racaId:guid}")]
    public async Task<ActionResult<IEnumerable<Pet>>> GetByRaca(Guid racaId, [FromQuery] QueryParameters query)
    {
        return Ok(await context.Pets.ListAsync(query, predicate: x => x.RacaId == racaId));
    }

    [HttpGet("nome/{nome}")]
    public async Task<ActionResult<IEnumerable<Pet>>> GetByNome(string nome, [FromQuery] QueryParameters query)
    {
        return Ok(await context.Pets.ListAsync(query, predicate: x => x.Nome.Contains(nome)));
    }

    [HttpPost]
    public async Task<ActionResult<Pet>> Create(PetRequest request)
    {
        var pet = await service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Pet>> Update(Guid id, PetRequest request)
    {
        return Ok(await service.UpdateAsync(id, request));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }
}
