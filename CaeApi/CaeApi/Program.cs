using CaeApi.CAE;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;
using System;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<CaeClaimsChallengeService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration)
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddMicrosoftGraph(builder.Configuration.GetSection("GraphBeta"))
    .AddDistributedTokenCaches();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
//IdentityModelEventSource.ShowPII = true;

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

var azpValidClientId = builder.Configuration["AzpValidClientId"];
if (azpValidClientId == null) throw new ArgumentNullException(azpValidClientId);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ValidateAccessTokenPolicy", validateAccessTokenPolicy =>
    {
        // Validate id of application for which the token was created
        // In this case the UI application 
        validateAccessTokenPolicy.RequireClaim("azp", azpValidClientId);

        // only allow tokens which used "Private key JWT Client authentication"
        // // https://docs.microsoft.com/en-us/azure/active-directory/develop/access-tokens
        // Indicates how the client was authenticated. For a public client, the value is "0". 
        // If client ID and client secret are used, the value is "1". 
        // If a client certificate was used for authentication, the value is "2".
        validateAccessTokenPolicy.RequireClaim("azpacr", "1");
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapOpenApi("/openapi/v1/openapi.json");

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1/openapi.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
