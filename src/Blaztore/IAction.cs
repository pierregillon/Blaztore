using MediatR;

namespace Blaztore;

public interface IAction : IRequest
{
}

public interface IAction<TState> : IAction where TState : IState
{
    internal TState? GetState(IStore store)
    {
        if (store.CreateStateFromActionExecution)
        {
            return this switch
            {
                IScopedAction scoped => store.GetStateOrCreateDefault<TState>(scoped.Scope),
                _ => store.GetStateOrCreateDefault<TState>()
            };
        }

        return this switch
        {
            IScopedAction scoped => store.GetState<TState>(scoped.Scope),
            _ => store.GetState<TState>()
        };
    }

    internal void SetState(IStore store, TState newState)
    {
        switch (this)
        {
            case IScopedAction scopedAction:
                store.SetState(newState, scopedAction.Scope);
                break;
            
            default:
                store.SetState(newState);
                break;
        }
    }
}