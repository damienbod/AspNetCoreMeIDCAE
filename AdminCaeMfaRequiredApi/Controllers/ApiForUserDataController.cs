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
        //_caeCliamsChallengeService.CheckForRequiredAuthContext(AuthContextId.C1, HttpContext);
        return new List<string> { "user API data 1", "user API data 2" };
    }
}
