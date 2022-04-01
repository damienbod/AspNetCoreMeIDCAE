﻿using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazor.CAE.RequireMfa.Server.Services;

public class UserApiClientService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ITokenAcquisition _tokenAcquisition;

    public UserApiClientService(
        ITokenAcquisition tokenAcquisition,
        IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _tokenAcquisition = tokenAcquisition;
    }

    public async Task<IEnumerable<string>?> GetApiDataAsync()
    {
        var client = _clientFactory.CreateClient();

        var scopes = new List<string> { "api://7c839e15-096b-4abb-a869-df9e6b34027c/access_as_user" };
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);

        client.BaseAddress = new Uri("https://localhost:44395");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("ApiForUserData");
        if (response.IsSuccessStatusCode)
        {
            var stream = await response.Content.ReadAsStreamAsync();
            var payload = await JsonSerializer.DeserializeAsync<List<string>>(stream);

            return payload;
        }

        throw new ApplicationException("oh no...");
    }
}