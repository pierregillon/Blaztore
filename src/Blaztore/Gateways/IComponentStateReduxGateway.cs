using Blaztore.Components;

namespace Blaztore.Gateways;

public interface IComponentStateReduxGateway<TState> where TState : IComponentState
{
    TState SubscribeToState(IStateComponent stateComponent);
    Task Dispatch(IComponentAction<TState> action);
}