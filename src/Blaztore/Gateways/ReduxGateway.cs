using Blaztore.Components;

namespace Blaztore.Gateways;

internal class ReduxGateway<TState> : 
    ISharedStateReduxGateway<TState>, 
    IUniqueStateReduxGateway<TState>
    where TState : IState
{
    private readonly IActionDispatcher _actionDispatcher;
    private readonly IStore _store;
    private readonly Subscriptions _subscriptions;

    public ReduxGateway(IActionDispatcher actionDispatcher, IStore store, Subscriptions subscriptions)
    {
        _actionDispatcher = actionDispatcher;
        _store = store;
        _subscriptions = subscriptions;
    }

    public TState SubscribeToState(IStateComponent component)
    {
        var defaultScope = DefaultScope.Value;
        
        _subscriptions.Add(typeof(TState), defaultScope, component);
        
        return _store.GetState<TState>(defaultScope);
    }

    public Task Dispatch(IAction<TState> action) => _actionDispatcher.Dispatch(action);

    public Task Dispatch(IComponentAction<TState> action) => _actionDispatcher.Dispatch(action);
}

internal class ReduxGateway<TState, TScope> : 
    IScopedStateReduxGateway<TState, TScope>
    where TState : IState
{
    private readonly IActionDispatcher _actionDispatcher;
    private readonly IStore _store;
    private readonly Subscriptions _subscriptions;

    public ReduxGateway(IActionDispatcher actionDispatcher, IStore store, Subscriptions subscriptions)
    {
        _actionDispatcher = actionDispatcher;
        _store = store;
        _subscriptions = subscriptions;
    }

    public TState SubscribeToState(IStateComponent component, TScope scope)
    {
        _subscriptions.Add(typeof(TState), scope, component);
        
        return _store.GetState<TState>(scope);
    }

    public Task Dispatch(IScopedAction<TState, TScope> action) => _actionDispatcher.Dispatch(action);
}