using Blaztore.Components;

namespace Blaztore.Gateways;

internal class ComponentStateReduxGateway<TState> : IComponentStateReduxGateway<TState> 
    where TState : IComponentState
{
    private readonly IActionDispatcher _actionDispatcher;
    private readonly IStore _store;
    private readonly Subscriptions _subscriptions;

    public ComponentStateReduxGateway(IActionDispatcher actionDispatcher, IStore store, Subscriptions subscriptions)
    {
        _actionDispatcher = actionDispatcher;
        _store = store;
        _subscriptions = subscriptions;
    }

    public TState SubscribeToState(IStateComponent component)
    {
        _subscriptions.Add(typeof(TState), component.Id, component);
        
        return _store.GetStateOrCreateDefault<TState>(component.Id);
    }

    public Task Dispatch(IComponentAction<TState> action) => _actionDispatcher.Dispatch(action);
    
    public void UnsubscribeFromState(IStateComponent stateComponent)
    {
        _subscriptions.Remove(stateComponent);
        
        if (_subscriptions.NoMoreSubscribers(typeof(TState), stateComponent.Id))
        {
            _store.Remove<TState>(stateComponent.Id);
        }
    }
}