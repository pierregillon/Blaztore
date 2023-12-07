using System.Collections.Concurrent;
using System.Reflection;
using Blaztore.Gateways;

namespace Blaztore.States;

public class InMemoryStore : IStore
{
    private readonly BlaztoreConfiguration _configuration;

    private readonly IDictionary<(Type StateType, object? Key), IState> _states =
        new ConcurrentDictionary<(Type, object?), IState>();

    public InMemoryStore(BlaztoreConfiguration configuration) =>
        _configuration = configuration;

    public bool CanInitializeStateFromActionExecution => _configuration.CanInitializeStateFromActionExecution;

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
            throw new InvalidOperationException(
                $"A static Initialize() method should be defined on type {stateType.Name} in order to have the initial state."
            );
        }

        if (stateType.IsPersistentState() && stateType.IsComponentState())
        {
            throw new ComponentStateCannotBePersistentException();
        }

        return (IState)initializeMethod.Invoke(null, Array.Empty<object?>())!;
    }
}

internal class ComponentStateCannotBePersistentException : Exception
{
    public ComponentStateCannotBePersistentException()
        :base("A component state cannot be persistent because its lifecycle is by design bound to the component lifecycle.")
    {
        
    }
}