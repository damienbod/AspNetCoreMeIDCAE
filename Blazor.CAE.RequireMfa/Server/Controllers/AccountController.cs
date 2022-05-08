using Microsoft.AspNetCore.Authentication;
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
    public ActionResult Login(string returnUrl)
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/"
        });
    }

    [HttpGet("Cae")]
    public ActionResult Cae(string claimChallenge)
    {
        try
        {
            _consentHandler.ChallengeUser(new string[] { "user.read" }, claimChallenge);
            return Ok();

        }
        catch (Exception ex)
        {
            _consentHandler.HandleException(ex);
            return Ok();
        }
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
