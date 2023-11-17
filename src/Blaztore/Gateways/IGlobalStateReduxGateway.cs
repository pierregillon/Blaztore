using Blaztore.Components;

namespace Blaztore.Gateways;

public interface IGlobalStateReduxGateway<TState> where TState : IGlobalState
{
    TState SubscribeToState(IComponentBase component);
    Task Dispatch(IAction<TState> action);
    void UnsubscribeFromState(IComponentBase stateComponent);
}