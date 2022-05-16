using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace CaeApi.CAE;

/// <summary>
/// Claims challenges, claims requests, and client capabilities
/// 
/// https://docs.microsoft.com/en-us/azure/active-directory/develop/claims-challenge
/// 
/// Applications that use enhanced security features like Continuous Access Evaluation (CAE) 
/// and Conditional Access authentication context must be prepared to handle claims challenges.
/// </summary>
public class CAECliamsChallengeService
{
    private readonly IConfiguration _configuration;

    public CAECliamsChallengeService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    /// <summary>
    /// Retrieves the acrsValue from database for the request method.
    /// Checks if the access token has acrs claim with acrsValue.
    /// If does not exists then adds WWW-Authenticate and throws UnauthorizedAccessException exception.
    /// </summary>
    public void CheckForRequiredAuthContext(string authContextId, HttpContext context)
    {
        if (!string.IsNullOrEmpty(authContextId))
        {
            string authenticationContextClassReferencesClaim = "acrs";

            if (context == null || context.User == null || context.User.Claims == null || !context.User.Claims.Any())
            {
                throw new ArgumentNullException(nameof(context), "No Usercontext is available to pick claims from");
            }

            var acrsClaim = context.User.FindAll(authenticationContextClassReferencesClaim).FirstOrDefault(x => x.Value == authContextId);

            if (acrsClaim?.Value != authContextId)
            {
                if (IsClientCapableofClaimsChallenge(context))
                {
                    string clientId = _configuration.GetSection("AzureAd").GetSection("ClientId").Value;
                    var base64str = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"access_token\":{\"acrs\":{\"essential\":true,\"value\":\"" + authContextId + "\"}}}"));

                    context.Response.Headers.Append("WWW-Authenticate", $"Bearer realm=\"\", authorization_uri=\"https://login.microsoftonline.com/common/oauth2/authorize\", client_id=\"" + clientId + "\", error=\"insufficient_claims\", claims=\"" + base64str + "\", cc_type=\"authcontext\"");
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    string message = string.Format(CultureInfo.InvariantCulture, "The presented access tokens had insufficient claims. Please request for claims requested in the WWW-Authentication header and try again.");
                    context.Response.WriteAsync(message);
                    context.Response.CompleteAsync();
                    throw new UnauthorizedAccessException(message);
                }
                else
                {
                    throw new UnauthorizedAccessException("The caller does not meet the authentication  bar to carry our this operation. The service cannot allow this operation");
                }
            }
        }
    }

    /// <summary>
    /// Evaluates for the presence of the client capabilities claim (xms_cc) and accordingly returns a response if present.
    /// </summary>
    public bool IsClientCapableofClaimsChallenge(HttpContext context)
    {
        string clientCapabilitiesClaim = "xms_cc";

        if (context == null || context.User == null || context.User.Claims == null || !context.User.Claims.Any())
        {
            throw new ArgumentNullException(nameof(context), "No Usercontext is available to pick claims from");
        }

        var ccClaim = context.User.FindAll(clientCapabilitiesClaim).FirstOrDefault(x => x.Type == "xms_cc");

        if (ccClaim != null && ccClaim.Value == "cp1")
        {
            return true;
        }

        return false;
    }
}
