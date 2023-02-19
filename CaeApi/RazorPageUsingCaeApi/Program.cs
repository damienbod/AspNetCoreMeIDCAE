using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using RazorPageUsingCaeApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<MsGraphService>();
builder.Services.AddScoped<AdminApiClientService>();

var downstreamScopes = new List<string>();
var scope = builder.Configuration.GetSection("AdminApi")["Scope"];
if (scope != null) downstreamScopes.Add(scope);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd", subscribeToOpenIdConnectMiddlewareDiagnosticsEvents: true)
    .EnableTokenAcquisitionToCallDownstreamApi(downstreamScopes)
    .AddMicrosoftGraph(builder.Configuration.GetSection("GraphBeta"))
    .AddDistributedTokenCaches();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

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
