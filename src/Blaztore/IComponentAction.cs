using Blaztore.Components;

namespace Blaztore;

public interface IComponentAction<TState> : IAction<TState>, IScopedAction
    where TState : IComponentState
{
    public ComponentId ComponentId { get; }
    object? IScopedAction.Scope => ComponentId;
}