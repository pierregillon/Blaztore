using Microsoft.AspNetCore.Components;

namespace Blaztore.Components;

public class StateComponent : BaseStateComponent, IDisposable
{
    [Inject] private IActionDispatcher ActionDispatcher { get; set; } = default!;
    [Inject] private IStore Store { get; set; } = default!;
    [Inject] private Subscriptions Subscriptions { get; set; } = default!;

    protected Task Dispatch(IAction action) => ActionDispatcher.Dispatch(action);

    protected T GetState<T>() where T : IState => GetState<T>(DefaultScope.Value);

    protected T GetState<T>(object scope) where T : IState
    {
        var stateType = typeof(T);
        Subscriptions.Add(stateType, scope, this);
        return Store.GetState<T>(scope);
    }

    public virtual void Dispose()
    {
        Subscriptions.Remove(this);
        GC.SuppressFinalize(this);
    }
}

public class StateComponent<TState> : BaseStateComponent, IDisposable where TState : IState
{
    [Inject] private IActionDispatcher ActionDispatcher { get; set; } = default!;
    [Inject] private IStore Store { get; set; } = default!;
    [Inject] private Subscriptions Subscriptions { get; set; } = default!;

    protected Task Dispatch(IAction<TState> action) => ActionDispatcher.Dispatch(action);

    protected TState GetState() => GetState(DefaultScope.Value);

    protected TState GetState(object scope)
    {
        var stateType = typeof(TState);
        Subscriptions.Add(stateType, scope, this);
        return Store.GetState<TState>(scope);
    }

    public virtual void Dispose()
    {
        Subscriptions.Remove(this);
        GC.SuppressFinalize(this);
    }
}