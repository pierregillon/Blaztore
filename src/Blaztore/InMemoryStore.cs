using System.Collections.Concurrent;
using System.Reflection;

namespace Blaztore;

public class InMemoryStore : IStore
{
    private readonly IDictionary<(Type StateType, object? StateScope), IState> _states =
        new ConcurrentDictionary<(Type, object?), IState>();

    public T GetStateOrCreateDefault<T>(object? scope) where T : IState
    {
        var stateType = typeof(T);
        
        lock (_states)
        {
            if (_states.TryGetValue((stateType, scope), out var state))
            {
                return (T)state;
            }

            state = CreateDefaultState(stateType);
            
            _states.Add((stateType, scope), state);

            return (T)state;
        }
    }

    public T GetStateOrCreateDefault<T>() where T : IState =>
        GetStateOrCreateDefault<T>(DefaultScope.Value);

    public T? GetState<T>() where T : IState =>
        GetState<T>(DefaultScope.Value);

    public T? GetState<T>(object? scope) where T : IState =>
        (T?)GetState(typeof(T), scope);

    public IState? GetState(Type stateType) => 
        GetState(stateType, DefaultScope.Value);

    public IState? GetState(Type stateType, object? scope)
    {
        lock (_states)
        {
            return _states.TryGetValue((stateType, scope), out var state) 
                ? state 
                : null;
        }
    }

    private static IState CreateDefaultState(Type type)
    {
        var initializeMethod = type.GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);
        if (initializeMethod is null)
        {
            throw new InvalidOperationException(
                $"A static Initialize() method should be defined on type {type.Name} in order to have the initial state."
            );
        }

        return (IState)initializeMethod.Invoke(null, Array.Empty<object?>())!;
    }

    public void SetState<T>(T state, object? scope) where T : IState
    {
        lock (_states)
        {
            _states[(typeof(T), scope)] = state;
        }
    }

    public void SetState<T>(T state) where T : IState => SetState(state, DefaultScope.Value);
}