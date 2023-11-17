using Blaztore.Components;

namespace Blaztore.Gateways;

public interface IGlobalStateReduxGateway<TState> where TState : IGlobalState
{
    TState SubscribeToState(IStateComponent component);
    Task Dispatch(IAction<TState> action);
    void UnsubscribeFromState(IStateComponent stateComponent);
}