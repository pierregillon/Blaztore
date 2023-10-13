using Blaztore.Components;
using Blaztore.Gateways;
using Blaztore.Pipelines;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Blaztore;

public static class DependencyInjection
{
    public static IServiceCollection AddBlaztore(this IServiceCollection services, Action<MediatRServiceConfiguration> mediatorConfiguration)
    {
        services
            .AddMediatR(mediatorConfiguration)
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(RenderSubscriptionsPipeline<,>))
            .AddScoped<IActionDispatcher, MediatorActionDispatcher>()
            .AddScoped<IStore, InMemoryStore>()
            .AddScoped<Subscriptions>()
            .AddScoped<IActionEventPublisher, OnlyActiveComponentActionEventPublisher>()
            .AddScoped(typeof(ISharedStateReduxGateway<>), typeof(ReduxGateway<>))
            .AddScoped(typeof(IScopedStateReduxGateway<,>), typeof(ReduxGateway<,>))
            .AddScoped(typeof(IUniqueStateReduxGateway<>), typeof(ReduxGateway<>))
            ;

        return services;
    }
}