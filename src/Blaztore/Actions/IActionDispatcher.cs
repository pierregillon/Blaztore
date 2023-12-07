namespace Blaztore.Actions;

public interface IActionDispatcher
{
    Task Dispatch<TAction>(TAction action) where TAction : IAction;
}