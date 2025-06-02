using Blaztore.Actions;
using MediatR;

namespace Blaztore.ActionHandling;

public interface IActionHandler<in TAction> : IRequestHandler<TAction>
    where TAction : IAction
{
    public Task Handle(TAction action);
    
    async Task IRequestHandler<TAction>.Handle(TAction action, CancellationToken _) =>
        await Handle(action);
}