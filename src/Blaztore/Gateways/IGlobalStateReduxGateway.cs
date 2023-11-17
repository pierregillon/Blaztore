using Blaztore.Components;

namespace Blaztore.Gateways;

public interface IGlobalStateReduxGateway<TState> where TState : IState
{
    TState SubscribeToState(IStateComponent component);
    Task Dispatch(IAction<TState> action);
}