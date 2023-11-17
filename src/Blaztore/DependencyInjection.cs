using Blaztore.Gateways;
using Blaztore.Pipelines;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Blaztore;

public static class DependencyInjection
{
    public static IServiceCollection AddBlaztore(this IServiceCollection services, Func<BlaztoreConfiguration, BlaztoreConfiguration> configure)
    {
        var defaultConfiguration = new BlaztoreConfiguration(
            _ => {}, 
            DisableActionExecutionWhenNoComponentSubscribed: true
        );
        
        defaultConfiguration = configure(defaultConfiguration);
        
        services
            .AddMediatR(defaultConfiguration.ConfigureMediator)
            .AddScoped<BlaztoreConfiguration>(_ => defaultConfiguration)
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(RenderSubscriptionsPipeline<,>))
            .AddScoped<IActionDispatcher, MediatorActionDispatcher>()
            .AddScoped<IStore, InMemoryStore>()
            .AddScoped<Subscriptions>()
            .AddScoped<IActionEventPublisher, OnlyActiveComponentActionEventPublisher>()
            .AddScoped(typeof(IGlobalStateReduxGateway<>), typeof(GlobalStateReduxGateway<>))
            .AddScoped(typeof(IScopedStateReduxGateway<,>), typeof(ScopedStateReduxGateway<,>))
            .AddScoped(typeof(IComponentStateReduxGateway<>), typeof(ComponentStateReduxGateway<>))
            ;

        return services;
    }
}

public record BlaztoreConfiguration(
    Action<MediatRServiceConfiguration> ConfigureMediator, 
    bool DisableActionExecutionWhenNoComponentSubscribed
);