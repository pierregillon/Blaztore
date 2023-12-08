using Blaztore.Actions;
using Blaztore.Components;
using Blaztore.States;

namespace Blaztore.Gateways;

public interface IComponentStateReduxGateway<TState> where TState : IComponentState
{
    TState SubscribeToState(IComponentBase component);
    Task Dispatch(IComponentAction<TState> action);
    void UnsubscribeFromState(IComponentBase component);
}