namespace Blaztore;

public interface IStore
{
    T GetState<T>(object scope) where T : IState;
    T GetState<T>() where T : IState;
    void SetState<T>(T state, object scope) where T : IState;
    void SetState<T>(T state) where T : IState;
    IReadOnlyCollection<T> GetAllStates<T>() where T : IState;
    void UpdateState<T>(T previousState, T newState) where T : IState;
}