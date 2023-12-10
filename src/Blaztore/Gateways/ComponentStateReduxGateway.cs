using Blaztore.Actions;
using Blaztore.Components;
using Blaztore.States;

namespace Blaztore.Gateways;

internal class ComponentStateReduxGateway<TState>(
    IActionDispatcher actionDispatcher,
    IStore store,
    Subscriptions subscriptions
)
    : IComponentStateReduxGateway<TState>
    where TState : IComponentState
{
    public TState SubscribeToState(IComponentBase component)
    {
        subscriptions.TryAdd(component, typeof(TState), component.Id);
        
        return store.GetStateOrCreateDefault<TState>(component.Id);
    }

    public Task Dispatch(IComponentAction<TState> action) => actionDispatcher.Dispatch(action);
    
    public void UnsubscribeFromState(IComponentBase component)
    {
        subscriptions.Remove(component);
        
        if (subscriptions.NoMoreSubscribers(typeof(TState), component.Id))
        {
            store.Remove<TState>(component.Id);
        }
    }
}