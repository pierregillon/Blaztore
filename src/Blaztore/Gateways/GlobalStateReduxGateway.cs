using Blaztore.Actions;
using Blaztore.Components;
using Blaztore.States;

namespace Blaztore.Gateways;

internal class GlobalStateReduxGateway<TState>(
    IActionDispatcher actionDispatcher,
    IStore store,
    Subscriptions subscriptions
)
    : IGlobalStateReduxGateway<TState>
    where TState : IGlobalState
{
    public TState SubscribeToState(IComponentBase component)
    {
        subscriptions.TryAdd(component, typeof(TState));
        
        return store.GetStateOrCreateDefault<TState>();
    }

    public Task Dispatch(IAction<TState> action) => actionDispatcher.Dispatch(action);
    public void UnsubscribeFromState(IComponentBase component)
    {
        subscriptions.Remove(component);
        
        if (!typeof(TState).IsPersistentState() && subscriptions.NoMoreSubscribers(typeof(TState)))
        {
            store.Remove<TState>();
        }
    }
}