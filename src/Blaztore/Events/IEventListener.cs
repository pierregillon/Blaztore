using MediatR;

namespace Blaztore.Events;

public interface IEventListener<TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
{
    protected abstract Task On(TEvent @event);

    Task INotificationHandler<TEvent>.Handle(TEvent notification, CancellationToken cancellationToken) =>
        On(notification);
}