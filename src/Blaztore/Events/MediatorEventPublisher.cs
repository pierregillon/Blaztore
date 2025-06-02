using MediatR;

namespace Blaztore.Events;

public class MediatorEventPublisher(IMediator mediator) : IEventPublisher
{
    public Task Publish(IEvent @event) =>
        mediator.Publish(@event);
}