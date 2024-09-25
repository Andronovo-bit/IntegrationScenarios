using Integration.Backend;
using Integration.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Integration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IItemOperationBackend, ItemOperationBackend>();
        services.AddSingleton<ItemIntegrationService>();
        services.AddSingleton<ItemIntegrationDistributedService>();

        var redisConnectionString = configuration.GetSection("Redis:ConnectionString").Value;
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
        }

        return services;
    }
}
