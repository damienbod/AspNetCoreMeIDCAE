using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using RazorPageUsingCaeApi.CAE;
using RazorPageUsingCaeApi.Services;

namespace RazorPageUsingCaeApi.Pages
{
    public class AdminModel : PageModel
    {
        private readonly ILogger<AdminModel> _logger;
        private readonly AdminApiClientService _userApiClientService;

        public AdminModel(AdminApiClientService userApiClientService,
            ILogger<AdminModel> logger)
        {
            _userApiClientService = userApiClientService;
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
                var claimsChallenge = WwwAuthenticateParameters.GetClaimChallengeFromResponseHeaders(hex.Headers);
                _logger.LogInformation("{hexMessage}", hex.Message);

                var properties = new AuthenticationProperties { RedirectUri = "/admin" };

                if (claimsChallenge != null)
                {
                    string jsonString = claimsChallenge.Replace("\\", "")
                        .Trim(new char[1] { '"' });

                    properties.Items["claims"] = jsonString;
                }

                return Challenge(properties);
            }
        }
    }
}