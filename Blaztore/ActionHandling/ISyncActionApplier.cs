namespace Blaztore.ActionHandling;

public interface ISyncActionApplier<in TAction, TState> : IActionApplier<TAction, TState>
    where TState : IState
    where TAction : IAction<TState>
{
    public new TState Apply(TAction action, TState state);

    Task<TState> IActionApplier<TAction, TState>.Apply(TAction action, TState state) =>
        Task.FromResult(Apply(action, state));
}