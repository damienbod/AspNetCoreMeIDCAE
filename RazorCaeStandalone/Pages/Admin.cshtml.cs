﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorCaeStandalone.Cae;

namespace RazorCaeStandalone.Pages;

public class AdminModel : PageModel
{
    private readonly CaeClaimsChallengeService _caeClaimsChallengeService;

    public AdminModel(CaeClaimsChallengeService caeClaimsChallengeService)
    {
        _caeClaimsChallengeService = caeClaimsChallengeService;
    }

    [BindProperty]
    public IEnumerable<string>? Data { get; private set; }

    public IActionResult OnGet()
    {
        // if CAE claim missing in id token, the required claims challenge is returned
        var claimsChallenge = _caeClaimsChallengeService
            .CheckForRequiredAuthContextIdToken(AuthContextId.C1, HttpContext);

        if (claimsChallenge != null)
        {
            var properties = new AuthenticationProperties { RedirectUri = "/admin" };

            properties.Items["claims"] = claimsChallenge;
            return Challenge(properties);
        }

        Data =
        [
            "Admin data 1",
            "Admin data 2"
        ];

        return Page();
    }
}