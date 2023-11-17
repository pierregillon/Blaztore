namespace Blaztore;

public interface IStore
{
    internal T GetStateOrCreateDefault<T>(object? scope) where T : IState;
    internal T GetStateOrCreateDefault<T>() where T : IState;
    T? GetState<T>(object? scope) where T : IState;
    T? GetState<T>() where T : IState;
    internal void SetState<T>(T state, object? scope) where T : IState;
    internal void SetState<T>(T state) where T : IState;
    internal IState? GetState(Type stateType);
    internal IState? GetState(Type stateType, object? scope);

}