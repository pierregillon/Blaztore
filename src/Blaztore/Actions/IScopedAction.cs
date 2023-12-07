using Blaztore.States;

namespace Blaztore.Actions;

public interface IScopedAction<TState, out TScope> : IAction<TState>, IScopedAction 
    where TState : IScopedState<TScope>
{
    new TScope? Scope { get; }

    object? IScopedAction.Scope => Scope;
}

public interface IScopedAction : IAction
{
    object? Scope { get; }
}