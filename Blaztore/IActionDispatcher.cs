namespace Blaztore;

public interface IActionDispatcher
{
    Task Dispatch<TAction>(TAction action) where TAction : IAction;
}