namespace Blaztore;

public interface IStore
{
    bool CanInitializeStateFromActionExecution { get; }

    T? GetState<T>(object? key) where T : IState;
    internal IState? GetState(Type stateType, object? key);
    internal T GetStateOrCreateDefault<T>(object? key) where T : IState;
    internal void SetState<T>(T state, object? key) where T : IState;
    internal void Remove<TState>(object? key) where TState : IState;
}

public static class StoreExtensions
{
    public static T? GetState<T>(this IStore store) where T : IState =>
        store.GetState<T>(DefaultScope.Value);

    internal static IState? GetState(this IStore store, Type stateType) =>
        store.GetState(stateType, DefaultScope.Value);

    internal static T GetStateOrCreateDefault<T>(this IStore store) where T : IState =>
        store.GetStateOrCreateDefault<T>(DefaultScope.Value);

    internal static void SetState<T>(this IStore store, T state) where T : IState =>
        store.SetState(state, DefaultScope.Value);

    internal static void Remove<T>(this IStore store) where T : IState =>
        store.Remove<T>(DefaultScope.Value);
}