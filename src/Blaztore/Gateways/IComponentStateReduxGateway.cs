using Blaztore.Components;

namespace Blaztore.Gateways;

public interface IComponentStateReduxGateway<TState> where TState : IComponentState
{
    TState SubscribeToState(IComponentBase stateComponent);
    Task Dispatch(IComponentAction<TState> action);
    void UnsubscribeFromState(IComponentBase stateComponent);
}