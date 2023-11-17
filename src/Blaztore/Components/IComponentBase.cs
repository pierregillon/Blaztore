namespace Blaztore.Components;

public interface IComponentBase
{
    ComponentId Id { get; }
    void ReRender();
}