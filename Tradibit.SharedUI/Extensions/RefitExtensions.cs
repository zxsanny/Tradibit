using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Refit;
using Tradibit.SharedUI.Interfaces;

namespace Tradibit.SharedUI.Extensions;

public static class RefitExtensions
{
    public static IServiceCollection AddRefit<T>(this IServiceCollection services, string apiUrl, bool requireAuthToken = true) where T : class
    {
        services.TryAddTransient<UserBearerAuthenticationHandler>();

        services.AddRefitClient<T>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiUrl))
            .If(requireAuthToken, b => b.AddHttpMessageHandler<UserBearerAuthenticationHandler>());
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
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenProvider.GetToken(cancellationToken));
        return await base.SendAsync(request, cancellationToken);
    }
}