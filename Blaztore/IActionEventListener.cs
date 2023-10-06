namespace Blaztore;

public interface IActionEventListener<in TEvent> where TEvent : IActionEvent
{
    Task On(TEvent @event);
}