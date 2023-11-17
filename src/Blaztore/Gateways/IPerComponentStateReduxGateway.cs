using Blaztore.Components;

namespace Blaztore.Gateways;

public interface IPerComponentStateReduxGateway<TState> where TState : IState
{
    TState SubscribeToState(IStateComponent stateComponent);
    Task Dispatch(IComponentAction<TState> action);
}