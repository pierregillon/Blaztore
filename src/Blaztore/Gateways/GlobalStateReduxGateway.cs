using Blaztore.Components;

namespace Blaztore.Gateways;

internal class GlobalStateReduxGateway<TState> : IGlobalStateReduxGateway<TState> 
    where TState : IGlobalState
{
    private readonly IActionDispatcher _actionDispatcher;
    private readonly IStore _store;
    private readonly Subscriptions _subscriptions;

    public GlobalStateReduxGateway(IActionDispatcher actionDispatcher, IStore store, Subscriptions subscriptions)
    {
        _actionDispatcher = actionDispatcher;
        _store = store;
        _subscriptions = subscriptions;
    }

    public TState SubscribeToState(IComponentBase component)
    {
        _subscriptions.Add(typeof(TState), DefaultScope.Value, component);
        
        return _store.GetStateOrCreateDefault<TState>(DefaultScope.Value);
    }

    public Task Dispatch(IAction<TState> action) => _actionDispatcher.Dispatch(action);
    public void UnsubscribeFromState(IComponentBase stateComponent)
    {
        _subscriptions.Remove(stateComponent);
        
        if (_subscriptions.NoMoreSubscribers(typeof(TState), DefaultScope.Value))
        {
            _store.Remove<TState>();
        }
    }
}