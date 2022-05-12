﻿using Microsoft.AspNetCore.Components;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.CAE.RequireMfa.Client.Services;

// orig src https://github.com/berhir/BlazorWebAssemblyCookieAuth
public class AuthorizedHandler : DelegatingHandler
{
    private readonly HostAuthenticationStateProvider _authenticationStateProvider;
    private readonly NavigationManager _navigation;

    public AuthorizedHandler(HostAuthenticationStateProvider authenticationStateProvider,
          NavigationManager navigation)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _navigation = navigation;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        HttpResponseMessage responseMessage;
        if (authState.User.Identity!= null && !authState.User.Identity.IsAuthenticated)
        {
            // if user is not authenticated, immediately set response status to 401 Unauthorized
            responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
        else
        {
            responseMessage = await base.SendAsync(request, cancellationToken);
        }

        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        {
            var content = await responseMessage.Content.ReadAsStringAsync();

            // if server returned 401 Unauthorized, redirect to login page
            if (content != null && content.Contains("acr")) // CAE
            {
                //var uri = $"/{_navigation.ToBaseRelativePath(_navigation.Uri)}";
                _authenticationStateProvider.CaeStepUp(content);
            }
            else // standard
            {
                _authenticationStateProvider.SignIn();
            }
        }

        return responseMessage;
    }
}
