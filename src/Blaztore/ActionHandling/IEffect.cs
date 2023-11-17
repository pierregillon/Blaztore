using MediatR;

namespace Blaztore.ActionHandling;

public interface IEffect<in TState, in TAction> : IRequestHandler<TAction>
    where TState : IState
    where TAction : IAction<TState>
{
    Task IRequestHandler<TAction>.Handle(TAction action, CancellationToken _)
    {
        var state = action.GetState(Store);
        
        return state is not null 
            ? Effect(state, action) 
            : Task.CompletedTask;
    }

    IStore Store { get; }

    Task Effect(TState state, TAction action);
}