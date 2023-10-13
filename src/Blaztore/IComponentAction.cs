using Blaztore.Components;

namespace Blaztore;

public interface IComponentAction<TState> : IAction<TState>, IActionOnScopedState
    where TState : IState
{
    public ComponentId? ComponentId { get; }
    object? IActionOnScopedState.Scope => ComponentId;

}