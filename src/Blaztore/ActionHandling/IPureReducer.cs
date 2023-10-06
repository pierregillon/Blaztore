namespace Blaztore.ActionHandling;

public interface IPureReducer<TState, in TAction> : IReducer<TState, TAction> 
    where TAction : IAction<TState> 
    where TState : IState
{
    public new TState Reduce(TState state, TAction action);
    
    Task<TState> IReducer<TState, TAction>.Reduce(TState state, TAction action) =>
        Task.FromResult(Reduce(state, action));
}