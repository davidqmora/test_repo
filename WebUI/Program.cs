using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using MyRadar.Products;
using NLog;
using NLog.Web;
using WebUI;

WebApplication BuildApp(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var services = builder.Services;
    var configuration = builder.Configuration;

    services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(configuration.GetSection("AzureAd"));

    services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Events.OnSignedOutCallbackRedirect = context =>
        {
            context.HttpContext.Response.Redirect(context.Options.SignedOutRedirectUri);
            context.HandleResponse();
            return Task.CompletedTask;
        };
    });
    
    services.AddControllersWithViews()
        .AddMicrosoftIdentityUI();
    
    services.AddRazorPages();
    services.AddServerSideBlazor()
        .AddMicrosoftIdentityConsentHandler();

    builder.Services.AddMudServices();

    services.AddApplicationInsightsTelemetry();

    services.AddProductServices(configuration.GetSection("TableStorage") );

    return builder.Build();
}

void RunApp(WebApplication application)
{
    if (!application.Environment.IsDevelopment())
    {
        application.UseExceptionHandler("/Error");
        application.UseHsts();
    }

    application.UseHttpsRedirection();

    application.UseStaticFiles();

    application.UseRouting();

    application.UseAuthentication();
    application.UseAuthorization();

    application.MapControllers();
    application.MapBlazorHub();
    application.MapFallbackToPage("/_Host");

    application.Run();
}


var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{
    var app = BuildApp(args);
    RunApp(app);
}
catch (Exception exception)
{
    logger.Error(exception, "Unhandled exception running Lamarr WebUI");
    throw;
}
finally
{
    LogManager.Shutdown();
}