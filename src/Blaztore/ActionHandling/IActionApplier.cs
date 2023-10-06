namespace Blaztore.ActionHandling;

public interface IActionApplier<in TAction, TState> : IActionHandler<TAction, TState>
    where TState : IState
    where TAction : IAction<TState>
{
    public Task<TState> Apply(TAction action, TState state);

    async Task IActionHandler<TAction, TState>.Handle(TAction action, TState state)
    {
        if (action is IActionOnScopedState actionOnScopedState)
        {
            var newState = await Apply(action, state);

            Store.SetState(newState, actionOnScopedState.Scope);
        }
        else
        {
            var newState = await Apply(action, state);

            Store.SetState(newState);
        }
    }
}