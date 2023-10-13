using Blaztore.Components;

namespace Blaztore.Gateways;

public interface IScopedStateReduxGateway<TState, in TScope> where TState : IState
{
    TState SubscribeToState(IStateComponent component, TScope scope);
    Task Dispatch(IScopedAction<TState, TScope> action);
}