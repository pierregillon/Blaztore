namespace Blaztore.Components;

public interface IStateComponent
{
    void ReRender();
    ComponentId? Id { get; }
}