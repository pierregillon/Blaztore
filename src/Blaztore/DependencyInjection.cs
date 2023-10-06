using Blaztore.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace Blaztore;

public static class DependencyInjection
{
    public static IServiceCollection AddBlaztore(this IServiceCollection services, Action<MediatRServiceConfiguration> mediatorConfiguration)
    {
        services
            .AddMediatR(x => mediatorConfiguration(
                x.AddOpenRequestPostProcessor(typeof(RenderSubscriptionsPostProcessor<,>))
            ))
            .AddScoped<IActionDispatcher, MediatorActionDispatcher>()
            .AddScoped<IStore, InMemoryStore>()
            .AddScoped<Subscriptions>()
            ;

        return services;
    }
}