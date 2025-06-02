namespace Blaztore.Events;

public interface IEventPublisher
{
    Task Publish(IEvent @event);
}