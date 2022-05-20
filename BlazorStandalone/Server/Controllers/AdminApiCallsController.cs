using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorBffAzureAD.Server.Controllers;

[ValidateAntiForgeryToken]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class AdminApiCallsController : ControllerBase
{
    private readonly CaeClaimsChallengeService _caeClaimsChallengeService;

    public AdminApiCallsController(CaeClaimsChallengeService caeClaimsChallengeService)
    {
        _caeClaimsChallengeService = caeClaimsChallengeService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        // if CAE claim missing in id token, the required claims challenge is returned
        var claimsChallenge = _caeClaimsChallengeService
            .CheckForRequiredAuthContextIdToken(AuthContextId.C1, HttpContext);

        if (claimsChallenge != null)
        {
            return Unauthorized(claimsChallenge);
        }

        return Ok(new List<string>()
        {
            "Admin data 1",
            "Admin data 2"
        });
    }
}