using System.Linq;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseNoUnauthorizedRedirect(this IApplicationBuilder applicationBuilder, params string[] segments)
    {
        applicationBuilder.Use(async (httpContext, func) =>
        {
            if (segments.Any(s => httpContext.Request.Path.StartsWithSegments(s)))
            {
                httpContext.Request.Headers[HeaderNames.XRequestedWith] = "XMLHttpRequest"; 
            }

            await func();
        });

        return applicationBuilder;
    }
}