using Blaztore.Components;

namespace Blaztore.Gateways;

public interface IScopedStateReduxGateway<TState, in TScope> where TState : IScopedState<TScope>
{
    TState SubscribeToState(IComponentBase component, TScope? scope);
    Task Dispatch(IScopedAction<TState, TScope> action);
    void UnsubscribeFromState(IComponentBase stateComponent, TScope? scope);
}