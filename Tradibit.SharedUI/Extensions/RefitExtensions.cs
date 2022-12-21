using System.Net.Http.Headers;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualBasic;
using Refit;

namespace Tradibit.SharedUI.Extensions;

public static class RefitExtensions
{
    private static IServiceCollection AddRefit<T>(this IServiceCollection services, string baseUrl, bool isBearer = false, bool isHmac = false) where T : class
    {
        if (isBearer) 
            services.TryAddTransient<UserBearerAuthenticationHandler>();

        services.AddRefitClient<T>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<UserBearerAuthenticationHandler>();
        return services;
    }
}

/// <summary> </summary>
public class UserBearerAuthenticationHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;

    /// <summary> </summary>
    public UserBearerAuthenticationHandler(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }
    
    /// <summary> </summary>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenProvider.GetToken());
        return await base.SendAsync(request, cancellationToken);
    }
}