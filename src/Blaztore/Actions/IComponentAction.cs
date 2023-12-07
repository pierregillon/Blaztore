using Blaztore.Components;
using Blaztore.States;

namespace Blaztore.Actions;

public interface IComponentAction<TState> : IAction<TState>, IScopedAction
    where TState : IComponentState
{
    public ComponentId ComponentId { get; }
    object? IScopedAction.Scope => ComponentId;
}