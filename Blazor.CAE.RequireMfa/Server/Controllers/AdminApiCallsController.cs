using System;
using System.Threading.Tasks;
using Blazor.CAE.RequireMfa.Server;
using Blazor.CAE.RequireMfa.Server.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace BlazorAzureADWithApis.Server.Controllers;

[ValidateAntiForgeryToken]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[AuthorizeForScopes(Scopes = new string[] { "api://7c839e15-096b-4abb-a869-df9e6b34027c/access_as_user" })]
[ApiController]
[Route("api/[controller]")]
public class AdminApiCallsController : ControllerBase
{
    private readonly AdminApiClientService _userApiClientService;
   
    public AdminApiCallsController(AdminApiClientService userApiClientService)
    {
        _userApiClientService = userApiClientService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            return Ok(await _userApiClientService.GetApiDataAsync());
        }
        catch (WebApiMsalUiRequiredException hex)
        {
            var claimChallenge = WwwAuthenticateParameters.GetClaimChallengeFromResponseHeaders(hex.Headers);
            return Unauthorized(claimChallenge);
        }

        return Ok(Array.Empty<string>());
    }
}