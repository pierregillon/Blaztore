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

    public TState SubscribeToState(IStateComponent component)
    {
        var defaultScope = DefaultScope.Value;
        
        _subscriptions.Add(typeof(TState), defaultScope, component);
        
        return _store.GetStateOrCreateDefault<TState>(defaultScope);
    }

    public Task Dispatch(IAction<TState> action) => _actionDispatcher.Dispatch(action);
}

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
}

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
}