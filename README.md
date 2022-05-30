[![.NET](https://github.com/damienbod/AspNetCoreAzureADCAE/actions/workflows/dotnet.yml/badge.svg)](https://github.com/damienbod/AspNetCoreAzureADCAE/actions/workflows/dotnet.yml)

# Razor Page Azure AD Continuous Access Evaluation

[Implement Azure AD Continuous Access Evaluation in an ASP.NET Core Razor Page app using a Web API](https://damienbod.com/2022/04/20/implement-azure-ad-continuous-access-evaluation-in-an-asp-net-core-razor-page-app-using-a-web-api/)

[Implement Azure AD Continuous Access Evaluation (CAE) step up with ASP.NET Core Blazor using a Web API](https://damienbod.com/2022/05/23/implement-azure-ad-continuous-access-evaluation-cae-step-up-with-asp-net-core-blazor-using-a-web-api/)

[Implement Azure AD Continuous Access Evaluation (CAE) standalone with Blazor ASP.NET Core](https://damienbod.com/2022/05/30/implement-azure-ad-continuous-access-evaluation-cae-standalone-with-blazor-asp-net-core/)

# History 

2022-05-23 Improve code in Blazor applications

2022-05-20 Add standalone samples

2022-05-13 Add CAE Blazor example

2022-05-08 Update packages

2022-04-16 Update packages

2022-04-05 Added CAE admin tool using Microsoft Graph

2022-04-03 initial version

## Azure app registration manifest access token

```json
"optionalClaims": {
	"idToken": [],
	"accessToken": [
		{
			"name": "xms_cc",
			"source": null,
			"essential": false,
			"additionalProperties": []
		}
	],
	"saml2Token": []
},
```

## Azure app registration manifest id_token

```json
"optionalClaims": {
	"idToken": [
		{
			"name": "xms_cc",
			"source": null,
			"essential": false,
			"additionalProperties": []
		}
	],
	"accessToken": [],
	"saml2Token": []
},
```

## Claims challenge returned from the API

```json
{"access_token":{"acrs":{"essential":true,"value":"c1"}}}
```

## Access token

```csharp
{
  "aud": "7c839e15-096b-4abb-a869-df9e6b34027c",
  "iss": "https://login.microsoftonline.com/5698af84-5720-4ff0-bdc3-9d9195314244/v2.0",
  "iat": 1648841224,
  "nbf": 1648841224,
  "exp": 1648845383,
  "acrs": [
    "c1"
  ],
  "azp": "7c839e15-096b-4abb-a869-df9e6b34027c",
  "azpacr": "1",
  "ver": "2.0",
  "xms_cc": [
    "cp1"
  ],
  // plus more claims
}
```

## Links

https://github.com/Azure-Samples/ms-identity-ca-auth-context

https://github.com/Azure-Samples/ms-identity-dotnetcore-ca-auth-context-app

https://docs.microsoft.com/en-us/azure/active-directory/conditional-access/overview

https://github.com/Azure-Samples/ms-identity-dotnetcore-daemon-graph-cae

https://docs.microsoft.com/en-us/azure/active-directory/develop/developer-guide-conditional-access-authentication-context

https://docs.microsoft.com/en-us/azure/active-directory/develop/claims-challenge

https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-conditional-access-dev-guide

https://www.youtube.com/watch?v=_iO7CfoktTY

https://openid.net/wg/sse/

https://github.com/damienbod/Blazor.BFF.AzureAD.Template
