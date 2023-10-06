using MediatR;

namespace Blaztore.ActionHandling;

public interface IEffect<in TState, in TAction> : IRequestHandler<TAction>
    where TState : IState
    where TAction : IAction<TState>
{
    Task IRequestHandler<TAction>.Handle(TAction action, CancellationToken _) =>
        Effect(Store.GetState<TState>(), action);

    IStore Store { get; }

    Task Effect(TState state, TAction action);
}