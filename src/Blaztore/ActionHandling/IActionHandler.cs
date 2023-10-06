using MediatR;

namespace Blaztore.ActionHandling;

public interface IActionHandler<in TAction, in TState> : IRequestHandler<TAction>
    where TState : IState
    where TAction : IAction<TState>
{
    Task IRequestHandler<TAction>.Handle(TAction action, CancellationToken _) =>
        Handle(action, action.GetState(Store));

    IStore Store { get; }

    Task Handle(TAction action, TState state);
}

public interface IActionHandler<in TAction> : IRequestHandler<TAction>
    where TAction : IAction
{
    Task IRequestHandler<TAction>.Handle(TAction action, CancellationToken _) => Handle(action);

    Task Handle(TAction action);
}