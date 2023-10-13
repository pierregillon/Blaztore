namespace Blaztore;

public interface IScopedAction<TState, out TScope> : IAction<TState>, IActionOnScopedState where TState : IState
{
    TScope? Id { get; }

    object? IActionOnScopedState.Scope => Id;

}