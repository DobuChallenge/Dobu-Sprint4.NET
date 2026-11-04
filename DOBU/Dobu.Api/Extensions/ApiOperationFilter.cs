using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Dobu.Api.Extensions;

public sealed class ApiOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        if (metadata.OfType<IAllowAnonymous>().Any()) operation.Security = new List<OpenApiSecurityRequirement>();
        if (context.ApiDescription.HttpMethod != "GET" || !operation.Responses.TryGetValue("200", out var response)) return;
        response.Headers["Link"] = new OpenApiHeader { Description = "Links HATEOAS: self, collection e navegação entre páginas.", Schema = new OpenApiSchema { Type = "string" } };
        if (context.ApiDescription.ParameterDescriptions.Any(parameter => parameter.Name.Equals("Page", StringComparison.OrdinalIgnoreCase)))
            response.Headers["X-Total-Count"] = new OpenApiHeader { Description = "Quantidade de registros filtrados antes da paginação.", Schema = new OpenApiSchema { Type = "integer" } };
    }
}
