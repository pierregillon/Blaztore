using Blaztore.Components;

namespace Blaztore.Gateways;

internal class ReduxGateway<TState> : 
    IGlobalStateReduxGateway<TState>, 
    IPerComponentStateReduxGateway<TState>
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

    TState IGlobalStateReduxGateway<TState>.SubscribeToState(IStateComponent component)
    {
        var defaultScope = DefaultScope.Value;
        
        _subscriptions.Add(typeof(TState), defaultScope, component);
        
        return _store.GetStateOrCreateDefault<TState>(defaultScope);
    }

    TState IPerComponentStateReduxGateway<TState>.SubscribeToState(IStateComponent component)
    {
        _subscriptions.Add(typeof(TState), component.Id, component);
        
        return _store.GetStateOrCreateDefault<TState>(component.Id);
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

    public TState SubscribeToState(IStateComponent component, TScope? scope)
    {
        _subscriptions.Add(typeof(TState), scope, component);
        
        return _store.GetStateOrCreateDefault<TState>(scope);
    }

    public Task Dispatch(IScopedAction<TState, TScope> action) => _actionDispatcher.Dispatch(action);
}