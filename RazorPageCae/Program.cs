using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using RazorPageCae;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<MsGraphService>();
builder.Services.AddScoped<AdminApiClientService>();

builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd", subscribeToOpenIdConnectMiddlewareDiagnosticsEvents: true)
    .EnableTokenAcquisitionToCallDownstreamApi(new[] { "api://7c839e15-096b-4abb-a869-df9e6b34027c/access_as_user" })
    .AddMicrosoftGraph(builder.Configuration.GetSection("GraphBeta"))
    .AddInMemoryTokenCaches();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
