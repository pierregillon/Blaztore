namespace Blaztore.States;

/// <summary>
/// Indicates the state is uniquely identified from a scope.
/// Multiple instances of the state can be instancied for each different scope.
/// Multiple components can subscribe to the same state by providing the same scope.
/// </summary>
/// <typeparam name="TScope"></typeparam>
public interface IScopedState<TScope> : IState
{
}