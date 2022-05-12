﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System;

namespace Blazor.CAE.RequireMfa.Server.Controllers;

// orig src https://github.com/berhir/BlazorWebAssemblyCookieAuth
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;

    public AccountController(MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler)
    {
        _consentHandler = consentHandler;
    }

    [HttpGet("Login")]
    public ActionResult Login(string? returnUrl, string? claimsChallenge)
    {
        // TODO read claims from query parameter
        var claims = "{\"access_token\":{\"acrs\":{\"essential\":true,\"value\":\"c1\"}}}";
        var redirectUri = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/";

        var properties = new AuthenticationProperties { RedirectUri = redirectUri };

        if(claimsChallenge != null)
        {
            properties.Items["claims"] = claims;
        }

        return Challenge(properties);
    }

    // [ValidateAntiForgeryToken] // not needed explicitly due the the Auto global definition.
    [Authorize]
    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        return SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}
