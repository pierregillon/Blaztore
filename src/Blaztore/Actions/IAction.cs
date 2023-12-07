using Blaztore.Gateways;
using Blaztore.States;
using MediatR;

namespace Blaztore.Actions;

public interface IAction : IRequest
{
}

public interface IAction<TState> : IAction where TState : IState
{
    internal TState? GetState(IStore store)
    {
        if (typeof(TState).IsInstanciableFromActionExecution())
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