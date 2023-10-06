namespace Blaztore.ActionHandling;

public interface IPureReducerNoAction<TState, in TAction> : IReducer<TState, TAction> 
    where TAction : IAction<TState> 
    where TState : IState
{
    public TState Reduce(TState state);
    
    Task<TState> IReducer<TState, TAction>.Reduce(TState state, TAction _) =>
        Task.FromResult(Reduce(state));
}