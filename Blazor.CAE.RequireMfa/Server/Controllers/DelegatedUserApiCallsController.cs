using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.CAE.RequireMfa.Server.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BlazorAzureADWithApis.Server.Controllers;

[ValidateAntiForgeryToken]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[AuthorizeForScopes(Scopes = new string[] { "api://7c839e15-096b-4abb-a869-df9e6b34027c/access_as_user" })]
[ApiController]
[Route("[controller]")]
public class DelegatedUserApiCallsController : ControllerBase
{
    private readonly UserApiClientService _userApiClientService;

    public DelegatedUserApiCallsController(UserApiClientService userApiClientService)
    {
        _userApiClientService = userApiClientService;
    }

    [HttpGet]
    public async Task<IEnumerable<string>?> Get()
    {
        return await _userApiClientService.GetApiDataAsync();
    }
}