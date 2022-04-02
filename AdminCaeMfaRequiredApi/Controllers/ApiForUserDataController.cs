using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminCaeMfaRequiredApi.Controllers;

[Authorize(Policy = "ValidateAccessTokenPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("[controller]")]
public class ApiForUserDataController : ControllerBase
{
    private readonly CAECliamsChallengeService _caeCliamsChallengeService;

    public ApiForUserDataController(CAECliamsChallengeService caeCliamsChallengeService)
    {
        _caeCliamsChallengeService = caeCliamsChallengeService;
    }

    [HttpGet]
    public IEnumerable<string> Get()
    {
        // returns unauthorized exception with WWW-Authenticate header if CAE claim missing in access token
        // handled in the caller to challenge
        _caeCliamsChallengeService.CheckForRequiredAuthContext(AuthContextId.C1, HttpContext);
        return new List<string> { "admin API CAE protected data 1", "admin API CAE protected  data 2" };
    }
}
