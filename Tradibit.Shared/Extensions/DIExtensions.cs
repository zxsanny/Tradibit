using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tradibit.Shared.Extensions;

public static class DiExtensions
{
    public static IServiceCollection ConfigSection<T>(this IServiceCollection services, IConfiguration config) where T : class =>
        services.Configure<T>(config.GetSection(typeof(T).Name));
    
    public static T GetSection<T>(this ConfigurationManager config) =>
        config.GetSection(typeof(T).Name).Get<T>();
    
}