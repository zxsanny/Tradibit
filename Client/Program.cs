using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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
    .AddScoped<RequestExt>()
    .AddRefit<IScenariosApi>(builder.HostEnvironment.BaseAddress)
    .AddScoped<ITokenProvider, TokenAuthenticationStateProvider>();

await builder.Build().RunAsync();
