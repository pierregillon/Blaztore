using System.Collections.Concurrent;
using System.Reflection;
using Blaztore.Gateways;

namespace Blaztore.States;

public class InMemoryStore : IStore
{
    private readonly IDictionary<(Type StateType, object? Key), IState> _states =
        new ConcurrentDictionary<(Type, object?), IState>();

    public T? GetState<T>(object? key) where T : IState =>
        (T?)GetState(typeof(T), key);

    public IState? GetState(Type stateType, object? key)
    {
        lock (_states)
        {
            return _states.TryGetValue((stateType, key), out var state) 
                ? state 
                : null;
        }
    }

    public T GetStateOrCreateDefault<T>(object? key) where T : IState
    {
        var stateType = typeof(T);
        
        lock (_states)
        {
            if (_states.TryGetValue((stateType, key), out var state))
            {
                return (T)state;
            }

            state = CreateDefaultState(stateType);
            
            _states.Add((stateType, key), state);

            return (T)state;
        }
    }

    public void Remove<TState>(object? key) where TState : IState
    {
        lock (_states)
        {
            _states.Remove((typeof(TState), key));
        }
    }

    public void SetState<T>(T state, object? key) where T : IState
    {
        lock (_states)
        {
            _states[(typeof(T), key)] = state;
        }
    }

    private static IState CreateDefaultState(Type stateType)
    {
        var initializeMethod = stateType.GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);
        if (initializeMethod is null)
        {
            throw new MissingInitializeMethodException(stateType);
        }

        if (stateType.IsPersistentState() && stateType.IsComponentState())
        {
            throw new ComponentStateCannotBePersistentException();
        }

        if (!stateType.IsGlobalState() && !stateType.IsScopedState() && !stateType.IsComponentState())
        {
            throw new MissingStateUsageException(stateType);
        }

        return (IState)initializeMethod.Invoke(null, Array.Empty<object?>())!;
    }
}

internal class MissingStateUsageException : Exception
{
    public MissingStateUsageException(Type stateType) : base($"The state {stateType.Name} must implement '{nameof(IGlobalState)}' or '{typeof(IScopedState<>).Name}' or '{nameof(IComponentState)}', in order to provide a state access.")
    {
        
    }
}

internal class MissingInitializeMethodException : Exception
{
    public MissingInitializeMethodException(Type stateType) : base($"A static Initialize() method should be defined on type {stateType.Name} in order to have the initial state.")
    {
        
    }
}

internal class ComponentStateCannotBePersistentException : Exception
{
    public ComponentStateCannotBePersistentException()
        :base("A component state cannot be persistent because its lifecycle is by design bound to the component lifecycle.")
    {
        
    }
}