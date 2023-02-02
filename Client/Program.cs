using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Tradibit.Client;
using Tradibit.Client.Shared;
using Tradibit.SharedUI.Extensions;
using Tradibit.SharedUI.Interfaces;
using Tradibit.SharedUI.Interfaces.API;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddBlazoredLocalStorage()

    .AddTransient<UserBearerAuthenticationHandler>()
    .AddScoped<ITokenProvider, TokenAuthenticationStateProvider>()
    .AddScoped<RequestExt>()
    .AddRefit<IAccountApi>(builder.HostEnvironment.BaseAddress, false)
    .AddRefit<IStrategiesApi>(builder.HostEnvironment.BaseAddress);

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("GoogleAuth", options.ProviderOptions);
});

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

await builder.Build().RunAsync();
