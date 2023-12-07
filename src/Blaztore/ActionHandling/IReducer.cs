using Blaztore.Actions;
using Blaztore.States;
using MediatR;

namespace Blaztore.ActionHandling;

public interface IReducer<TState, in TAction> : IRequestHandler<TAction>
    where TState : IState
    where TAction : IAction<TState>
{
    IStore Store { get; }

    public Task<TState> Reduce(TState state, TAction action);

    async Task IRequestHandler<TAction>.Handle(TAction action, CancellationToken _)
    {
        var initialState = action.GetState(Store);
        if (initialState is not null)
        {
            var newState = await Reduce(initialState, action);
            action.SetState(Store, newState);
        }
    }
}