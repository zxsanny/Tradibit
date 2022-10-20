using Microsoft.AspNetCore.Authentication.JwtBearer;
using Tradibit.Api.Auth;
using Tradibit.Api.Services;
using Tradibit.Api.Services.Candles;
using Tradibit.Common.DTO;
using Tradibit.Common.Extensions;
using Tradibit.Common.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services
    .ConfigSection<MainTradingSettings>(builder.Configuration)
    .AddSingleton<RealtimeCandlesService>()
    .AddSingleton<HistoryCandlesService>()
    .AddScoped<CandlesProviderResolver>(sp => resolverEnum => resolverEnum switch
    {
        CandlesResolverEnum.Realtime => sp.GetService<RealtimeCandlesService>(),
        CandlesResolverEnum.History => sp.GetService<HistoryCandlesService>(),
        _ => throw new ArgumentOutOfRangeException(nameof(resolverEnum), resolverEnum, null)
    })
    .AddScoped<IUserBrokerService, UserBrokerService>()
    .AddScoped<ICoinsService, CoinsService>()
    .AddSingleton<ICurrentUserProvider, CurrentUserProvider>();;

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.SecurityTokenValidators.Clear();
    o.SecurityTokenValidators.Add(new GoogleTokenValidator());
});
    
builder.Services.AddAuthorization();

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

app.Run();