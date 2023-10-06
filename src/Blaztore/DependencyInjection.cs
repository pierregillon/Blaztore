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
            ;

        return services;
    }
}