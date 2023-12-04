using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tradibit.Api.Services;
using Tradibit.DataAccess;
using Tradibit.Shared;
using Tradibit.Shared.DTO;
using Tradibit.Shared.DTO.Auth;
using Tradibit.Shared.DTO.SettingsDTO;
using Tradibit.Shared.Extensions;
using Tradibit.Shared.Interfaces;
using Tradibit.Shared.MappingProfiles;
using Tradibit.SharedUI;
using Tradibit.SharedUI.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services
    .ConfigSection<MainTradingSettings>(builder.Configuration)
    .ConfigSection<AuthConfig>(builder.Configuration)
    .ConfigSection<BinanceWatcherCredentials>(builder.Configuration)
    .ConfigSection<DatabaseOptions>(builder.Configuration)
    .AddHttpContextAccessor()
    .AddMediatR(AssemblyExt.GetAllOwnReferencedAssemblies())
    .AddAutoMapper(typeof(BinanceProfile).Assembly)

    .AddSingleton<IClientHolder, ClientHolder>()
    .AddSingleton<ICandlesProvider, CandlesProvider>()
    .AddSingleton<ICurrentUserProvider, CurrentUserProvider>()

    .AddDbContext<TradibitDb>((serviceProvider, optionsBuilder) =>
    {
        var dbOptions = serviceProvider.GetService<IOptions<DatabaseOptions>>()?.Value;
        if (dbOptions?.TimeoutSeconds is null || dbOptions.ConnectionString is null)
            throw new Exception("Please configure sections TimeoutSeconds and ConnectionStrings in DatabaseOptions in config");

        optionsBuilder.UseNpgsql(dbOptions.ConnectionString,
            sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
                sqlOptions.CommandTimeout(dbOptions.TimeoutSeconds);
            });
    });

var authConfig = builder.Configuration.GetSection<AuthConfig>();
if (string.IsNullOrEmpty(authConfig.ClientId) || string.IsNullOrEmpty(authConfig.ClientSecret) || string.IsNullOrEmpty(authConfig.JwtSecret))
    throw new Exception("Please ensure AuthConfig:ClientId and ClientSecret are presented in appsettings.json ");

builder.Services
    .AddAuthentication(o => o.DefaultScheme = Constants.APP_AUTH_SCHEME)
    .AddCookie(Constants.APP_AUTH_SCHEME, o => o.ExpireTimeSpan = TimeSpan.FromSeconds(authConfig.TokenExpireSeconds))
    .AddGoogle(o =>
    {
        o.SignInScheme = Constants.APP_AUTH_SCHEME;
        o.ClientId = authConfig.ClientId;
        o.ClientSecret = authConfig.ClientSecret;
        o.CorrelationCookie.SameSite = SameSiteMode.Lax;
        o.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    })
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.JwtSecret)),
            ValidateIssuerSigningKey = true
        };
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

app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

app.Services.GetService<IMediator>()!.Publish(new AppInitEvent());