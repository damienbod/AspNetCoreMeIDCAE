using AdminCaeMfaRequiredApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<CAECliamsChallengeService>();

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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ValidateAccessTokenPolicy", validateAccessTokenPolicy =>
    {
        // Validate id of application for which the token was created
        // In this case the UI application 
        validateAccessTokenPolicy.RequireClaim("azp", builder.Configuration["AzpValidClientId"]);

        // only allow tokens which used "Private key JWT Client authentication"
        // // https://docs.microsoft.com/en-us/azure/active-directory/develop/access-tokens
        // Indicates how the client was authenticated. For a public client, the value is "0". 
        // If client ID and client secret are used, the value is "1". 
        // If a client certificate was used for authentication, the value is "2".
        validateAccessTokenPolicy.RequireClaim("azpacr", "1");
    });
});

builder.Services.AddSwaggerGen(c =>
{
    // add JWT Authentication
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {securityScheme, Array.Empty<string>()}
        });

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Admin AAD CAE protected API",
        Version = "v1",
        Description = "User API One",
        Contact = new OpenApiContact
        {
            Name = "damienbod",
            Email = string.Empty,
            Url = new Uri("https://damienbod.com/"),
        },
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User API One");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
