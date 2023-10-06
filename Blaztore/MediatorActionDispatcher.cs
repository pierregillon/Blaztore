using MediatR;

namespace Blaztore;

internal class MediatorActionDispatcher : IActionDispatcher
{
    private readonly IMediator _mediator;

    public MediatorActionDispatcher(IMediator mediator) => _mediator = mediator;

    public Task Dispatch<TAction>(TAction action) where TAction : IAction => _mediator.Send(action);
}