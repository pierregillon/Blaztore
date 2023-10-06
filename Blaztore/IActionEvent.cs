namespace Blaztore;

public interface IActionEvent
{
}

public interface IActionEvent<TState> : IActionEvent where TState : IState
{
}