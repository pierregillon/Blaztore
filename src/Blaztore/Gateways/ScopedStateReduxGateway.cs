using Blaztore.Actions;
using Blaztore.Components;
using Blaztore.States;

namespace Blaztore.Gateways;

internal class ScopedStateReduxGateway<TState, TScope>(
    IActionDispatcher actionDispatcher,
    IStore store,
    Subscriptions subscriptions
)
    : IScopedStateReduxGateway<TState, TScope>
    where TState : IScopedState<TScope>
{
    public TState SubscribeToState(IComponentBase component, TScope? scope)
    {
        subscriptions.TryAdd(component, typeof(TState), scope);
        
        return store.GetStateOrCreateDefault<TState>(scope);
    }

    public Task Dispatch(IScopedAction<TState, TScope> action) => actionDispatcher.Dispatch(action);
    public void UnsubscribeFromState(IComponentBase component, TScope? scope)
    {
        subscriptions.Remove(component);

        if (!typeof(TState).IsPersistentState() && subscriptions.NoMoreSubscribers(typeof(TState), scope))
        {
            store.Remove<TState>(scope);
        }
    }
}