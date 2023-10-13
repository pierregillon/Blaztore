using Blaztore.Components;

namespace Blaztore.Gateways;

public interface ISharedStateReduxGateway<TState> where TState : IState
{
    TState SubscribeToState(IStateComponent component);
    Task Dispatch(IAction<TState> action);
}