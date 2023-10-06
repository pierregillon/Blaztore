namespace Blaztore.ActionHandling;

public interface IPureReducer<TState, in TAction> : IActionApplier<TAction, TState>
    where TState : IState
    where TAction : IAction<TState>
{
    public TState Reduce(TState state, TAction action);

    Task<TState> IActionApplier<TAction, TState>.Apply(TAction action, TState state) =>
        Task.FromResult(Reduce(state, action));
}