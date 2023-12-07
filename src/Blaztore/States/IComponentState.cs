using Blaztore.Components;

namespace Blaztore.States;

/// <summary>
/// Represent a state that is unique for each component.
/// </summary>
public interface IComponentState : IScopedState<ComponentId>
{
    
}