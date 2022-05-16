using Microsoft.Identity.Web;
using RazorPageUsingCaeApi.CAE;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RazorPageUsingCaeApi.Services;

public class AdminApiClientService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly string _adminApiBaseUrl;
    private readonly string _adminApiScope;

    public AdminApiClientService(
        ITokenAcquisition tokenAcquisition,
        IHttpClientFactory clientFactory,
        IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _tokenAcquisition = tokenAcquisition;
        _adminApiBaseUrl = configuration.GetSection("AdminApi")["BaseUrl"];
        _adminApiScope = configuration.GetSection("AdminApi")["Scope"];
    }

    public async Task<IEnumerable<string>?> GetApiDataAsync()
    {
        var client = _clientFactory.CreateClient();

        var scopes = new List<string> { _adminApiScope };
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);

        client.BaseAddress = new Uri(_adminApiBaseUrl);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("ApiForUserData");
        if (response.IsSuccessStatusCode)
        {
            var stream = await response.Content.ReadAsStreamAsync();
            var payload = await JsonSerializer.DeserializeAsync<List<string>>(stream);

            return payload;
        }

        // This exception can be used to handle a claims challenge
        throw new WebApiMsalUiRequiredException($"Unexpected status code in the HttpResponseMessage: {response.StatusCode}.", response);
    }
}