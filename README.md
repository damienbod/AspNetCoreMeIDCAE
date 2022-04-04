[![.NET](https://github.com/damienbod/AspNetCoreAzureADCAE/actions/workflows/dotnet.yml/badge.svg)](https://github.com/damienbod/AspNetCoreAzureADCAE/actions/workflows/dotnet.yml)

# Razor Page Azure AD Continuous Access Evaluation

# History 

2022-04-03 initial version

## Azure app registration manifest

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