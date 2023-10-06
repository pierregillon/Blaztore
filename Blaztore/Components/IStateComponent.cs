namespace Blaztore.Components;

public interface IStateComponent
{
    void ReRender();
    string Id { get; }
}