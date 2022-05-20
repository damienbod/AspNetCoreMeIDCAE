using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BlazorBffAzureAD.Server.Controllers;

[ValidateAntiForgeryToken]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class AdminApiCallsController : ControllerBase
{
    private readonly CaeClaimsChallengeService _caeCliamsChallengeService;

    public AdminApiCallsController(CaeClaimsChallengeService caeCliamsChallengeService)
    {
        _caeCliamsChallengeService = caeCliamsChallengeService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        // if CAE claim missing in id token, the required claims challenge is returned
        var claimsChallenge = _caeCliamsChallengeService
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