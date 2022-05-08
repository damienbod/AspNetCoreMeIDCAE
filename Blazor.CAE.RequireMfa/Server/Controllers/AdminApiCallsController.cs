﻿using System;
using System.Collections.Generic;
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
    private readonly MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;

    public AdminApiCallsController(AdminApiClientService userApiClientService,
        MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler)
    {
        _userApiClientService = userApiClientService;
        _consentHandler = consentHandler;
    }

    [HttpGet]
    public async Task<IEnumerable<string>?> Get()
    {
        return await _userApiClientService.GetApiDataAsync();

        // TODO move this logic to the client
        //try
        //{
        //    return await _userApiClientService.GetApiDataAsync();
        //}
        //catch (WebApiMsalUiRequiredException hex)
        //{
        //    // Challenges the user if exception is thrown from Web API.
        //    try
        //    {
        //        var claimChallenge = WwwAuthenticateParameters.GetClaimChallengeFromResponseHeaders(hex.Headers);

        //        _consentHandler.ChallengeUser(new string[] { "user.read" }, claimChallenge);

        //        return Array.Empty<string>();
        //    }
        //    catch (Exception ex)
        //    {
        //        _consentHandler.HandleException(ex);
        //    }

        //    Console.WriteLine(hex.Message);
        //}

        return Array.Empty<string>();
    }
}