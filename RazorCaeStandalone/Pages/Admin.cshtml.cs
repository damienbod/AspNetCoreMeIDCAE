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
                _caeCliamsChallengeService.CheckForRequiredAuthContext(AuthContextId.C1, HttpContext);

                Data = new List<string>()
                {
                    "Admin data 1",
                    "Admin data 2"
                };

                return Page();
            }
            catch (WebApiMsalUiRequiredException hex)
            {
                // Challenges the user if exception is thrown from Web API.
                try
                {
                    var claimChallenge = WwwAuthenticateParameters.GetClaimChallengeFromResponseHeaders(hex.Headers);

                    _consentHandler.ChallengeUser(new string[] { "user.read" }, claimChallenge);

                    return Page();
                }
                catch (Exception ex)
                {
                    _consentHandler.HandleException(ex);
                }            
            }

            return Page();
        }
    }
}