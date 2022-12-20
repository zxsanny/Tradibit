using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tradibit.Api.Services;
using Tradibit.Client.Shared;
using Tradibit.Common.Extensions;
using Tradibit.Common.Interfaces;
using Tradibit.Common.SettingsDTO;
using Tradibit.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services
    .ConfigSection<MainTradingSettings>(builder.Configuration)
    .ConfigSection<AuthConfig>(builder.Configuration)
    
    .AddHttpContextAccessor()
    .AddMediatR(AssemblyExt.GetAllOwnReferencedAssemblies())
    
    .AddSingleton<IClientHolder, ClientHolder>()
    .AddSingleton<ICandlesProvider, CandlesProvider>()
    .AddSingleton<ICurrentUserProvider, CurrentUserProvider>()
    
    .AddDbContext<TradibitDb>((serviceProvider, optionsBuilder) =>
    {
        var dbOptions = serviceProvider.GetService<IOptions<DatabaseOptions>>()?.Value;
        if (dbOptions?.TimeoutSeconds is null || dbOptions.ConnectionString is null)
            throw new Exception("Please configure sections TimeoutSeconds and ConnectionStrings in DatabaseOptions in config");

        optionsBuilder.UseNpgsql(dbOptions.ConnectionString,
            sqlOptions => sqlOptions.CommandTimeout(dbOptions.TimeoutSeconds));
    })
    .AddScoped<RequestExt>();

var authConfig = builder.Configuration.GetSection<AuthConfig>();

builder.Services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddIdentityServerJwt()
    .AddGoogle(o =>
    {
        o.ClientId = authConfig.GoogleAuth.ClientId;
        o.ClientSecret = authConfig.GoogleAuth.ClientSecret;
    });
//     .AddJwtBearer(o =>
// {
//     o.SecurityTokenValidators.Clear();
//     o.SecurityTokenValidators.Add(new GoogleTokenValidator());
// });
    
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