using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MoneySaver.SPA;
using MoneySaver.SPA.AuthProviders;
using MoneySaver.SPA.Models.Configurations;
using MoneySaver.SPA.Services;
using Polly;
using Polly.Extensions.Http;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
IConfigurationSection spaConfig = builder.Configuration.GetSection(nameof(SpaSettings));
builder.Services.Configure<SpaSettings>(spaConfig);

//var spaSettings = new SpaSettings();
//builder.Configuration.Bind(nameof(SpaSettings), spaSettings);
//builder.Services.AddSingleton(spaSettings);
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();

builder.Services.AddHttpClient("apiCallsTransactions", client => 
{ 
    client.BaseAddress = new Uri(spaConfig[nameof(SpaSettings.DataApiAddress)]);
})
    .AddHttpMessageHandler<CustomeAuthorization>()
    .AddPolicyHandler((serviceProvider, msg) => {
        Random _jitterer = new Random();
        return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                  + TimeSpan.FromMilliseconds(_jitterer.Next(0, 100)),
                    onRetry: (result, span, index, ctx) =>
                    {
                        ;
                        //var logger = serviceProvider.GetService<ILogger<object>>();
                        //logger.LogWarning($"tentative #{index}, received {result.Result.StatusCode}, retrying...");
                    });
    });

//builder.Services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("apiCallsTransactions"));

builder.Services.AddScoped<CustomeAuthorization>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
//builder.Services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("api"));
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IReportDataService, ReportsDataService>();

await builder.Build().RunAsync();


//TODO: Move to another place
public class CustomeAuthorization : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    private readonly IAuthenticationService _authStateProvider;

    public CustomeAuthorization(ILocalStorageService localStorage, IAuthenticationService authStateProvider)
    {
        this._localStorage = localStorage;
        this._authStateProvider = authStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await this._localStorage.GetItemAsStringAsync("authToken");
        if (token == null)
        {
            await this._authStateProvider.Logout();
            return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
        }

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
