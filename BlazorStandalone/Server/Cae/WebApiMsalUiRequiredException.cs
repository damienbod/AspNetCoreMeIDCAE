﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace BlazorBffAzureAD.Server;

/// <summary>
/// This exception class is used to pass HTTP CAE unauthorized responses from a Httpclient and 
/// return the WWWAuthenticate header with the required claims challenge. 
/// This is only required if using a downstream API
/// </summary>
public class WebApiMsalUiRequiredException : Exception
{
    private readonly HttpResponseMessage httpResponseMessage;

    public WebApiMsalUiRequiredException(string message, HttpResponseMessage response) : base(message)
    {
        httpResponseMessage = response;
    }

    public HttpStatusCode StatusCode
    {
        get { return httpResponseMessage.StatusCode; }
    }

    public HttpResponseHeaders Headers
    {
        get { return httpResponseMessage.Headers; }
    }

    public HttpResponseMessage HttpResponseMessage
    {
        get { return httpResponseMessage; }
    }
}