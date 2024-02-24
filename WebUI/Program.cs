using System.Net;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using NLog;
using NLog.Web;
using MyRadar.Accounts;using Polly;
using Polly.Extensions.Http;
using WebUI.Services;

WebApplication BuildApp(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog(new NLogAspNetCoreOptions
    {
        LoggingConfigurationSectionName = "NLog",
        RemoveLoggerFactoryFilter = true
    });

    var services = builder.Services;
    var configuration = builder.Configuration;

    services.AddMicrosoftIdentityWebAppAuthentication(configuration, Constants.AzureAdB2C)
        .EnableTokenAcquisitionToCallDownstreamApi(new[] { configuration["AuthorizePhotoshare:ApiScope"]! })
        .AddMicrosoftGraph(configuration.GetSection("Graph"))
        .AddInMemoryTokenCaches();
    
    services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Events.OnSignedOutCallbackRedirect = context =>
        {
            context.HttpContext.Response.Redirect(context.Options.SignedOutRedirectUri);
            context.HandleResponse();
            return Task.CompletedTask;
        };
    });
    services.Configure<OpenIdConnectOptions>(configuration.GetSection("AzureAdB2C"));
    
    services.AddControllersWithViews()
        .AddMicrosoftIdentityUI();

    services.AddRazorPages();
    services.AddServerSideBlazor()
        .AddMicrosoftIdentityConsentHandler();

    services.AddMudServices();

    services.AddApplicationInsightsTelemetry();

    services.AddAccountServices(sourceBuilder =>
    {
        sourceBuilder
            .AddCosmosDb(configuration.GetSection("AccountsDB"))
            .Build();
    });
    services.AddLocalServices(GetRetryPolicy());

    services.AddOptions();
    
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

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}


var logger = LogManager.Setup()
    .LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();
try
{
    var app = BuildApp(args);
    RunApp(app);
}
catch (Exception exception)
{
    logger.Error(exception, "Unhandled exception running Identity Hub WebUI");
    throw;
}
finally
{
    LogManager.Shutdown();
}