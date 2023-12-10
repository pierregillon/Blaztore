using Blaztore.States;

namespace Blaztore.Components;

internal static class SubscriptionsExtensions
{
    public static void TryAdd(this Subscriptions subscriptions, IComponentBase componentBase, Type stateType) =>
        subscriptions.TryAdd(componentBase, stateType, DefaultScope.Value);
    
    public static bool NoMoreSubscribers(this Subscriptions subscriptions, Type stateType) =>
        subscriptions.NoMoreSubscribers(stateType, DefaultScope.Value);
}