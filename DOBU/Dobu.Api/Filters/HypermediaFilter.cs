using System.Globalization;
using Dobu.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;

namespace Dobu.Api.Filters;

public sealed class HypermediaFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (!HttpMethods.IsGet(context.HttpContext.Request.Method) || context.Result is not ObjectResult { StatusCode: 200 } result) return;
        var request = context.HttpContext.Request;
        var root = $"{request.Scheme}://{request.Host}{request.PathBase}";
        var links = new List<string> { $"<{root}{request.Path}{request.QueryString}>; rel=\"self\"" };
        var resource = request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries).Take(2);
        if (resource is not null) links.Add($"<{root}/{string.Join('/', resource)}>; rel=\"collection\"");
        if (result.Value is IQueryMetadata metadata)
        {
            context.HttpContext.Response.Headers["X-Total-Count"] = metadata.Total.ToString(CultureInfo.InvariantCulture);
            if (metadata.Paginated)
            {
                var pages = Math.Max(1, (int)Math.Ceiling(metadata.Total / (double)metadata.PageSize));
                var parameters = request.Query.ToDictionary(pair => pair.Key, pair => (string?)pair.Value.ToString(), StringComparer.OrdinalIgnoreCase);
                parameters["pageSize"] = metadata.PageSize.ToString(CultureInfo.InvariantCulture);
                void AddPage(int page, string relation)
                {
                    parameters["page"] = page.ToString(CultureInfo.InvariantCulture);
                    links.Add($"<{QueryHelpers.AddQueryString(root + request.Path, parameters)}>; rel=\"{relation}\"");
                }
                AddPage(1, "first"); AddPage(pages, "last");
                if (metadata.Page > 1) AddPage(metadata.Page - 1, "prev");
                if (metadata.Page < pages) AddPage(metadata.Page + 1, "next");
            }
        }
        if (request.RouteValues.ContainsKey("id")) links.Add($"<{root}{request.Path}>; rel=\"edit\"; method=\"PUT\"");
        context.HttpContext.Response.Headers.Link = string.Join(", ", links);
    }
    public void OnResultExecuted(ResultExecutedContext context) { }
}
