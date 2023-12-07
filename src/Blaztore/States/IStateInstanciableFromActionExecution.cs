namespace Blaztore.States;

/// <summary>
/// By default, state action execution are ignored if no state has been created yet.
/// Implementing this interface allows the instanciation of a default state
/// on action execution requested and no state have being created yet.
/// </summary>
public interface IStateInstanciableFromActionExecution
{
    
}