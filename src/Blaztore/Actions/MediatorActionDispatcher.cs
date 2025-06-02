using MediatR;

namespace Blaztore.Actions;

internal class MediatorActionDispatcher(IMediator mediator) : IActionDispatcher
{
    public Task Dispatch(IAction action) => mediator.Send(action);
}