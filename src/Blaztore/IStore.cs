namespace Blaztore;

public interface IStore
{
    T GetState<T>(object scope) where T : IState;
    T GetState<T>() where T : IState;
    internal void SetState<T>(T state, object scope) where T : IState;
    internal void SetState<T>(T state) where T : IState;
    internal object GetState(Type stateType);
    internal object GetState(Type stateType, object scope);
}