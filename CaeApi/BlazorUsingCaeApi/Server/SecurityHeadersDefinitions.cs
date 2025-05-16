using Microsoft.AspNetCore.Builder;
using Microsoft.Graph.WindowsUpdates;

namespace Blazor.CAE.RequireMfa.Server;

public static class SecurityHeadersDefinitions
{
    public static HeaderPolicyCollection GetHeaderPolicyCollection(bool isDev, string? idpHost)
    {
        if (idpHost == null) throw new ArgumentNullException(idpHost);

        var policy = new HeaderPolicyCollection()
            .AddFrameOptionsDeny()
            .AddContentTypeOptionsNoSniff()
            .AddReferrerPolicyStrictOriginWhenCrossOrigin()
            .AddCrossOriginOpenerPolicy(builder => builder.SameOrigin())
            .AddCrossOriginResourcePolicy(builder => builder.SameOrigin())
            .AddCrossOriginEmbedderPolicy(builder => builder.RequireCorp()) // remove for dev if using hot reload
            .AddContentSecurityPolicy(builder =>
            {
                builder.AddObjectSrc().None();
                builder.AddBlockAllMixedContent();
                builder.AddImgSrc().Self().From("data:");
                builder.AddFormAction().Self().From(idpHost);
                builder.AddFontSrc().Self();
                builder.AddStyleSrc().Self();
                builder.AddBaseUri().Self();
                builder.AddFrameAncestors().None();

                // due to Blazor
                builder.AddScriptSrc()
                    .Self()
                    .WithHash256("v8v3RKRPmN4odZ1CWM5gw80QKPCCWMcpNeOmimNL2AA=")
                    .UnsafeEval();

                // disable script and style CSP protection if using Blazor hot reload
                // if using hot reload, DO NOT deploy with an insecure CSP
            })
            .RemoveServerHeader()
            .AddPermissionsPolicyWithDefaultSecureDirectives();

        if (!isDev)
        {
            // maxage = one year in seconds
            policy.AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 60 * 24 * 365);
        }

        return policy;
    }
}
