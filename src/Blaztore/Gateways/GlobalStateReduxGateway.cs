using Blaztore.Actions;
using Blaztore.Components;
using Blaztore.States;

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
        _subscriptions.Add(component, typeof(TState));
        
        return _store.GetStateOrCreateDefault<TState>();
    }

    public Task Dispatch(IAction<TState> action) => _actionDispatcher.Dispatch(action);
    public void UnsubscribeFromState(IComponentBase component)
    {
        _subscriptions.Remove(component);
        
        if (!typeof(TState).IsPersistentState() && _subscriptions.NoMoreSubscribers(typeof(TState)))
        {
            _store.Remove<TState>();
        }
    }
}