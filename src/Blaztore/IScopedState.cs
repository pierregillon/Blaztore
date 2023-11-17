using Blaztore.Components;

namespace Blaztore;

public interface IState
{
}

public interface IGlobalState : IState
{
    
}

public interface IComponentState : IScopedState<ComponentId>
{
    
}

public interface IScopedState<TScope> : IState
{
}