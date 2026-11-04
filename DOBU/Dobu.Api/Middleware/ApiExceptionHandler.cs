using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dobu.Api.Middleware;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            ArgumentException or ValidationException => (400, "Dados inválidos"),
            UnauthorizedAccessException => (401, "Autenticação necessária"),
            KeyNotFoundException => (404, "Registro não encontrado"),
            DbUpdateException => (409, "Conflito com os dados cadastrados"),
            MongoDB.Driver.MongoException => (503, "Banco de observações indisponível"),
            _ => (500, "Não foi possível concluir a operação")
        };
        logger.LogError(exception, "Falha {StatusCode} na requisição {CorrelationId}", status, context.TraceIdentifier);
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status, Title = title, Detail = status is 400 or 404 ? exception.Message : null,
            Extensions = { ["correlationId"] = context.TraceIdentifier }
        }, cancellationToken);
        return true;
    }
}
