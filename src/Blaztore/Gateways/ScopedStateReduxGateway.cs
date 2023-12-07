using Blaztore.Actions;
using Blaztore.Components;
using Blaztore.States;

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

    public TState SubscribeToState(IComponentBase component, TScope? scope)
    {
        _subscriptions.Add(component, typeof(TState), scope);
        
        return _store.GetStateOrCreateDefault<TState>(scope);
    }

    public Task Dispatch(IScopedAction<TState, TScope> action) => _actionDispatcher.Dispatch(action);
    public void UnsubscribeFromState(IComponentBase stateComponent, TScope? scope)
    {
        _subscriptions.Remove(stateComponent);

        if (_subscriptions.NoMoreSubscribers(typeof(TState), scope))
        {
            _store.Remove<TState>(scope);
        }
    }
}