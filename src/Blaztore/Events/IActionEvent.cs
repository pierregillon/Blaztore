using Blaztore.States;

namespace Blaztore.Events;

public interface IActionEvent
{
}

public interface IActionEvent<TState> : IActionEvent where TState : IState
{
}