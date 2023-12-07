namespace Blaztore.States;

/// <summary>
/// Indicates the state is global (singleton). Every subscribed component are retrieving
/// a unique state.
/// </summary>
public interface IGlobalState : IState
{
    
}