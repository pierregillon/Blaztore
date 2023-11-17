using Blaztore.Components;

namespace Blaztore.Gateways;

internal class ScopedStateReduxGateway<TState, TScope> : IScopedStateReduxGateway<TState, TScope> 
    where TState : IScopedState<TScope>
{
    private readonly IActionDispatcher _actionDispatcher;
    private readonly IStore _store;
    private readonly Subscriptions _subscriptions;

    public ScopedStateReduxGateway(IActionDispatcher actionDispatcher, IStore store, Subscriptions subscriptions)
    {
        _actionDispatcher = actionDispatcher;
        _store = store;
        _subscriptions = subscriptions;
    }

    public TState SubscribeToState(IStateComponent component, TScope? scope)
    {
        _subscriptions.Add(typeof(TState), scope, component);
        
        return _store.GetStateOrCreateDefault<TState>(scope);
    }

    public Task Dispatch(IScopedAction<TState, TScope> action) => _actionDispatcher.Dispatch(action);
    public void UnsubscribeFromState(IStateComponent stateComponent, TScope? scope)
    {
        _subscriptions.Remove(stateComponent);

        if (_subscriptions.NoMoreSubscribers(typeof(TState), scope))
        {
            _store.Remove<TState>(scope);
        }
    }
}