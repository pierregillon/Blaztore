using Blaztore.ActionHandling;
using Blaztore.Actions;
using Blaztore.Examples.Wasm.Services;

namespace Blaztore.Examples.Wasm.Pages.TodoList.Components.Creation.GenericActions;

public record CreateTask(string Description) : IAction
{
    public class Handler(ITodoListApi api) : IActionHandler<CreateTask>
    {
        public Task Handle(CreateTask action) =>
            api.Create(Guid.NewGuid(), action.Description);
    }
}