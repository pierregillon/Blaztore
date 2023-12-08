using Blaztore.Actions;
using Blaztore.Components;
using Blaztore.States;

namespace Blaztore.Gateways;

public interface IGlobalStateReduxGateway<TState> where TState : IGlobalState
{
    TState SubscribeToState(IComponentBase component);
    Task Dispatch(IAction<TState> action);
    void UnsubscribeFromState(IComponentBase component);
}