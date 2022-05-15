using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace RazorPageCae.Pages
{
    public class AdminModel : PageModel
    {
        private readonly ILogger<AdminModel> _logger;
        private readonly AdminApiClientService _userApiClientService;

        private readonly MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;

        public AdminModel(AdminApiClientService userApiClientService,
            ILogger<AdminModel> logger,
            MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler)
        {
            _userApiClientService = userApiClientService;
            _consentHandler = consentHandler;
            _logger = logger;
        }

        [BindProperty]
        public IEnumerable<string>? Data { get; private set; }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                Data = await _userApiClientService.GetApiDataAsync();
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

                _logger.LogInformation("{hexMessage}", hex.Message);
            }

            return Page();
        }
    }
}