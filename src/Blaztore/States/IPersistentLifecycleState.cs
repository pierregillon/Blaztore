namespace Blaztore.States;

/// <summary>
/// By default, every state are destroyed once the last component has unubscribed.
/// Implementing this interface allows the state to persist after the last component
/// being unsubscribed. It allows you to execute actions on the state even if no more
/// component uses it.
/// </summary>
public interface IPersistentLifecycleState : IState
{
}