namespace Blaztore.ActionHandling;

public interface IActionApplier<in TAction, TState> : IActionHandler<TAction, TState>
    where TState : IState
    where TAction : IAction<TState>
{
    public Task<TState> Apply(TAction action, TState state);

    async Task IActionHandler<TAction, TState>.Handle(TAction action, TState state) =>
        action.SetState(Store, await Apply(action, state));
}