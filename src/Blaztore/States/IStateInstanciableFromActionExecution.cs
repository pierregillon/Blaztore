namespace Blaztore.States;

/// <summary>
/// By default, state action execution are ignored if no state has been created yet.
/// Implementing this interface allows the instanciation of a default state
/// in order to execute the action. The newly created state is stored and will be used
/// by components.
/// </summary>
public interface IStateInstanciableFromActionExecution : IState
{
    
}