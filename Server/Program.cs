using Tradibit.Api.Providers;
using Tradibit.Common.DTO;
using Tradibit.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services
    .ConfigSection<MainTradingSettings>(builder.Configuration)
    .AddSingleton<RealtimeCandlesProvider>()
    .AddSingleton<HistoryCandlesProvider>()
    .AddScoped<CandlesProviderResolver>(sp => resolverEnum => resolverEnum switch
    {
        CandlesResolverEnum.Realtime => sp.GetService<RealtimeCandlesProvider>(),
        CandlesResolverEnum.History => sp.GetService<HistoryCandlesProvider>(),
        _ => throw new ArgumentOutOfRangeException(nameof(resolverEnum), resolverEnum, null)
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

await app.Services.GetService<RealtimeCandlesProvider>()!.Start();

app.Run();