using Blaztore.Components;

namespace Blaztore;

internal class Subscriptions
{
    private readonly List<Subscription> _subscriptions = new();

    public void Add(Type stateType, object? stateScope, IStateComponent component)
    {
        lock (_subscriptions)
        {
            if (!_subscriptions.Any(subscription =>
                    subscription.StateType == stateType
                    && subscription.StateScope == stateScope
                    && subscription.ComponentId == component.Id))
            {
                var subscription = new Subscription(
                    component.Id,
                    stateType,
                    stateScope,
                    new WeakReference<IStateComponent>(component)
                );

                _subscriptions.Add(subscription);
            }
        }
    }

    public void Remove(IStateComponent stateComponent)
    {
        lock (_subscriptions)
        {
            _subscriptions.RemoveAll(subscription => subscription.ComponentId == stateComponent.Id);
        }
    }

    public void ReRenderSubscribers(Type stateType)
    {
        Subscription[] subscriptions;

        lock (_subscriptions)
        {
            subscriptions = _subscriptions
                .Where(subscription => subscription.StateType == stateType)
                .ToArray();
        }

        ReRender(subscriptions);
    }

    public void ReRenderSubscribers(Type stateType, object? stateScope)
    {
        Subscription[] subscriptions;

        lock (_subscriptions)
        {
            subscriptions = _subscriptions
                .Where(subscription =>
                    subscription.StateType == stateType && Equals(subscription.StateScope, stateScope))
                .ToArray();
        }

        ReRender(subscriptions);
    }

    private void ReRender(IReadOnlyCollection<Subscription> subscriptions)
    {
        foreach (var subscription in subscriptions)
        {
            if (subscription.ComponentReference.TryGetTarget(out var target))
            {
                target.ReRender();
            }
            else
            {
                lock (_subscriptions)
                {
                    _subscriptions.Remove(subscription);
                }
            }
        }
    }

    public async Task ExecuteOnSubscribedComponents<TState>(Func<IStateComponent, Task> action)
        where TState : IState
    {
        List<Subscription> subscriptions;

        lock (_subscriptions)
        {
            subscriptions = _subscriptions
                .Where(aRecord => aRecord.StateType == typeof(TState))
                .ToList();
        }

        foreach (var subscription in subscriptions)
        {
            if (subscription.ComponentReference.TryGetTarget(out var target))
            {
                await action(target);
            }
        }
    }

    private record Subscription(
        ComponentId ComponentId,
        Type StateType,
        object? StateScope,
        WeakReference<IStateComponent> ComponentReference
    );
}