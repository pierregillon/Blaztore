namespace Blaztore;

public interface IActionEventPublisher
{
    Task Publish<TEvent, TState>(TEvent @event) where TState : IState where TEvent : IActionEvent<TState>;
}