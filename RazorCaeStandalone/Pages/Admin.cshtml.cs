using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;

namespace RazorCaeStandalone.Pages
{
    public class AdminModel : PageModel
    {
        private readonly CaeCliamsChallengeService _caeCliamsChallengeService;

        public AdminModel( CaeCliamsChallengeService caeCliamsChallengeService)
        {
            _caeCliamsChallengeService = caeCliamsChallengeService;
        }

        [BindProperty]
        public IEnumerable<string>? Data { get; private set; }

        public IActionResult OnGet()
        {

            // returns unauthorized exception with WWW-Authenticate header if CAE claim missing in access token
            // handled in the caller client exception with challenge returned if not ok
            var claimsChallenge = _caeCliamsChallengeService.CheckForRequiredAuthContextIdToken(AuthContextId.C1, HttpContext);

            if (claimsChallenge != null)
            {
                var properties = new AuthenticationProperties { RedirectUri = "/admin" };

                properties.Items["claims"] = claimsChallenge;
                return Challenge(properties);
            }

            Data = new List<string>()
            {
                "Admin data 1",
                "Admin data 2"
            };

            return Page();
        }
    }
}