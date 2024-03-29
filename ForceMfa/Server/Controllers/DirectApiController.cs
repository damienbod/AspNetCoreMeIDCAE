﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForceMfa.Server.Controllers;

[ValidateAntiForgeryToken]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "ca-mfa")]
[ApiController]
[Route("api/[controller]")]
public class DirectApiController : ControllerBase
{
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new List<string> { "some data", "more data", "loads of data" };
    }
}
