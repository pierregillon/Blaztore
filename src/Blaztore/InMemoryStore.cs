using System.Collections.Concurrent;
using System.Reflection;

namespace Blaztore;

public class InMemoryStore : IStore
{
    private readonly IDictionary<(Type StateType, object StateScope), IState> _states =
        new ConcurrentDictionary<(Type, object), IState>();

    public T GetState<T>(object scope) where T : IState
    {
        if (!_states.TryGetValue((typeof(T), scope), out var state))
        {
            lock (_states)
            {
                if (_states.TryGetValue((typeof(T), scope), out state))
                {
                    return (T)state;
                }

                state = CreateDefaultState<T>();
                _states.Add((typeof(T), scope), state);
            }
        }

        return (T)state;
    }

    public T GetState<T>() where T : IState => 
        GetState<T>(DefaultScope.Value);

    private static IState CreateDefaultState<T>() where T : IState
    {
        var initializeMethod = typeof(T).GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);
        if (initializeMethod is null)
        {
            throw new InvalidOperationException(
                $"A static Initialize() method should be defined on type {typeof(T).Name} in order to have the initial state.");
        }

        return (T)initializeMethod.Invoke(null, Array.Empty<object?>())!;
    }

    public void SetState<T>(T state, object scope) where T : IState => 
        _states[(typeof(T), scope)] = state;

    public void SetState<T>(T state) where T : IState => 
        SetState(state, DefaultScope.Value);

    public IReadOnlyCollection<T> GetAllStates<T>() where T : IState =>
        _states.Values.Where(x => x is T).Cast<T>().ToList();

    public void UpdateState<T>(T previousState, T newState) where T : IState
    {
        var keyPair = _states.Single(x => Equals(x.Value, previousState));

        _states.Remove(keyPair);

        _states.Add((typeof(T), keyPair.Key.StateScope), newState);
    }
}