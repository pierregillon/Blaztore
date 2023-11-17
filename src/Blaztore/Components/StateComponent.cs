using Blaztore.Gateways;
using Microsoft.AspNetCore.Components;

namespace Blaztore.Components;

public abstract class StateComponent : BaseStateComponent, IDisposable
{
    [Inject] private IActionDispatcher ActionDispatcher { get; set; } = default!;
    [Inject] private IStore Store { get; set; } = default!;
    [Inject] private Subscriptions Subscriptions { get; set; } = default!;

    protected Task Dispatch(IAction action) => ActionDispatcher.Dispatch(action);

    protected T GetState<T>() where T : IState => GetState<T>(DefaultScope.Value);

    protected T GetState<T>(object? scope) where T : IState
    {
        var stateType = typeof(T);
        Subscriptions.Add(stateType, scope, this);
        return Store.GetStateOrCreateDefault<T>(scope);
    }

    public virtual void Dispose()
    {
        Subscriptions.Remove(this);
        GC.SuppressFinalize(this);
    }
}

public abstract class StateComponent<TState> : BaseStateComponent, IDisposable where TState : IGlobalState
{
    [Inject] private IGlobalStateReduxGateway<TState> Gateway { get; set; } = default!;
    [Inject] private Subscriptions Subscriptions { get; set; } = default!;

    protected Task Dispatch(IAction<TState> action) => Gateway.Dispatch(action);

    protected TState GetState() => Gateway.SubscribeToState(this);

    public virtual void Dispose()
    {
        Subscriptions.Remove(this);
        GC.SuppressFinalize(this);
    }
}