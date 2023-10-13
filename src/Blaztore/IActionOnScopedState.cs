namespace Blaztore;

public interface IActionOnScopedState : IAction
{
    object? Scope { get; }
}