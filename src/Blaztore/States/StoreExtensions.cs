namespace Blaztore.States;

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