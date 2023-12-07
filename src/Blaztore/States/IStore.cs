namespace Blaztore.States;

public interface IStore
{
    T? GetState<T>(object? key) where T : IState;
    internal IState? GetState(Type stateType, object? key);
    internal T GetStateOrCreateDefault<T>(object? key) where T : IState;
    internal void SetState<T>(T state, object? key) where T : IState;
    internal void Remove<TState>(object? key) where TState : IState;
}