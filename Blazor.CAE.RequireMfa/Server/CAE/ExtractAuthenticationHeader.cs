using System.Linq;

namespace Blazor.CAE.RequireMfa.Server;

internal class ExtractAuthenticationHeader
{
    /// <summary>
    /// Extract claims from WwwAuthenticate header and returns the value.
    /// </summary>
    public static string ExtractHeaderValues(WebApiMsalUiRequiredException response)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && response.Headers.WwwAuthenticate.Any())
        {
            return AuthenticationHeaderHelper.ExtractClaimChallengeFromHttpHeader(response.Headers);
        }

        return string.Empty;
    }
}