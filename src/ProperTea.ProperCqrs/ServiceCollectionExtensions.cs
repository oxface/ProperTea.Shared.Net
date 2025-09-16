using Microsoft.Extensions.DependencyInjection;

namespace ProperTea.ProperCqrs;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProperCqrs(this IServiceCollection services)
    {
        services.AddSingleton<ICommandBus, CommandBus>();
        services.AddSingleton<IQueryBus, QueryBus>();

        return services;
    }
}