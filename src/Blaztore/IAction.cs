using MediatR;

namespace Blaztore;

public interface IAction : IRequest
{
}

public interface IAction<TState> : IAction where TState : IState
{
}

public interface IActionOnScopedState : IAction
{
    object Scope { get; }
}