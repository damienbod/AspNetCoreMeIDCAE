using System.Collections.Generic;
using CaeApi.CAE;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaeApi.Controllers;

[Authorize(Policy = "ValidateAccessTokenPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("[controller]")]
public class ApiForUserDataController : ControllerBase
{
    private readonly CaeClaimsChallengeService _caeClaimsChallengeService;

    public ApiForUserDataController(CaeClaimsChallengeService caeClaimsChallengeService)
    {
        _caeClaimsChallengeService = caeClaimsChallengeService;
    }

    [HttpGet]
    public IEnumerable<string> Get()
    {
        // returns unauthorized exception with WWW-Authenticate header if CAE claim missing in access token
        // handled in the caller client exception with challenge returned if not ok
        _caeClaimsChallengeService.CheckForRequiredAuthContext(AuthContextId.C1, HttpContext);
        return new List<string> { "admin API CAE protected data 1", "admin API CAE protected  data 2" };
    }
}
