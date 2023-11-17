using MediatR;

namespace Blaztore;

public interface IAction : IRequest
{
}

public interface IAction<TState> : IAction where TState : IState
{
    internal TState GetState(IStore store)
    {
        if (this is IActionOnScopedState scoped)
        {
            return store.GetStateOrCreateDefault<TState>(scoped.Scope);
        }

        return store.GetStateOrCreateDefault<TState>();
    }

    internal void SetState(IStore store, TState newState)
    {
        if (this is IActionOnScopedState actionOnScopedState)
        {
            store.SetState(newState, actionOnScopedState.Scope);
        }
        else
        {
            store.SetState(newState);
        }
    }
}