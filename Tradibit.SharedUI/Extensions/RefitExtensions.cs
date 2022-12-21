using System.Net.Http.Headers;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualBasic;
using Refit;

namespace Tradibit.SharedUI.Extensions;

/// <summary> </summary>
public static class RefitExtensions
{
    private static IServiceCollection AddRefit<T>(this IServiceCollection services, string baseUrl, bool isBearer = false, bool isHmac = false) where T : class
    {
        if (isBearer) 
            services.TryAddTransient<UserBearerAuthenticationHandler>();

        if (isHmac)
            services.TryAddTransient<UserHmacAuthenticationHandler>();

        services.AddRefitClient<T>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
            .If(isBearer, b => b.AddHttpMessageHandler<UserBearerAuthenticationHandler>())
            .If(isHmac, b => b.AddHttpMessageHandler<UserHmacAuthenticationHandler>());
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

/// <summary> </summary>
public class UserHmacAuthenticationHandler : DelegatingHandler
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IPartnerSecretsProvider _secretsProvider;

    /// <summary> </summary>
    public UserHmacAuthenticationHandler(ICurrentUserProvider currentUserProvider, IPartnerSecretsProvider secretsProvider)
    {
        _currentUserProvider = currentUserProvider;
        _secretsProvider = secretsProvider;
    }
    
    /// <summary> </summary>
    [UnsupportedOSPlatform("browser")]
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var partnerId = _currentUserProvider.CurrentUser.PartnerId;
        var secret = _secretsProvider.GetByPartnerId(partnerId);

        string content;
        if (request.Method.Method.IsOneOf(HttpMethod.Get.Method, HttpMethod.Delete.Method) && request.RequestUri != null)
            content = request.RequestUri.ToString();
        else
        {
            if (request.Content != null)
                content = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            else
                throw new Exception("request.Content is NULL!");
        }
            
        var headerValue = new HmacHeader(content, partnerId, secret.PartnerSecret).ToString();

        request.Headers.Add(Constants.API_KEY_HEADER_NAME, headerValue);
        return await base.SendAsync(request, cancellationToken);
    }
}