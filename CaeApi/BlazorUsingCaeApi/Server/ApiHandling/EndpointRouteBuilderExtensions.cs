using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Blazor.CAE.RequireMfa.Server.ApiHandling;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapNotFound(this IEndpointRouteBuilder endpointRouteBuilder, string pattern)
    {
        endpointRouteBuilder.Map(pattern, context =>
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        });

        return endpointRouteBuilder;
    }
}
