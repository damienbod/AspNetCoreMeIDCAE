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

        private readonly MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;
        private readonly CaeCliamsChallengeService _caeCliamsChallengeService;


        public AdminModel(MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler,
            CaeCliamsChallengeService caeCliamsChallengeService)
        {
            _caeCliamsChallengeService = caeCliamsChallengeService;
            _consentHandler = consentHandler;
        }

        [BindProperty]
        public IEnumerable<string>? Data { get; private set; }

        public IActionResult OnGet()
        {
            try
            {
                // returns unauthorized exception with WWW-Authenticate header if CAE claim missing in access token
                // handled in the caller client exception with challenge returned if not ok
                var claimsChallenge = _caeCliamsChallengeService.CheckForRequiredAuthContextIdToken(AuthContextId.C1, HttpContext);

                if (claimsChallenge != null)
                {
                    _consentHandler.ChallengeUser(new string[] { "user.read" }, claimsChallenge);
                }

                Data = new List<string>()
                {
                    "Admin data 1",
                    "Admin data 2"
                };

                return Page();
            }
            catch (UnauthorizedAccessException ex)
            {
                _consentHandler.HandleException(ex);        
            }

            return Page();
        }
    }
}