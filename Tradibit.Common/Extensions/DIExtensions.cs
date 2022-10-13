using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tradibit.Common.Extensions;

public static class DIExtensions
{
    public static IServiceCollection ConfigSection<T>(this IServiceCollection services, IConfiguration config) where T : class =>
        services.Configure<T>(config.GetSection(typeof(T).Name));
}