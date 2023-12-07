using Blaztore.Components;
using Blaztore.States;

namespace Blaztore.Events;

internal class OnlyActiveComponentActionEventPublisher : IActionEventPublisher
{
    private readonly Subscriptions _subscriptions;

    public OnlyActiveComponentActionEventPublisher(Subscriptions subscriptions) => _subscriptions = subscriptions;

    public async Task Publish<TEvent, TState>(TEvent @event)
        where TState : IState where TEvent : IActionEvent<TState> =>
        await _subscriptions.ExecuteOnSubscribedComponents<TState>(async component =>
        {
            if (component is IActionEventListener<TEvent> listener)
            {
                await listener.On(@event);
            }
        });
}