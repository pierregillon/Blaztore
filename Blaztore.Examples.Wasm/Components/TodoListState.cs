using Blaztore.ActionHandling;
using Blaztore.Examples.Wasm.Services;

namespace Blaztore.Examples.Wasm.Components;

public record TodoListState(
    IReadOnlyCollection<TaskListItem> TodoListItems,
    bool IsAddingTask,
    string? NewTaskDescription
) : IState
{
    public static TodoListState Initialize() => new(new List<TaskListItem>(), false, null);

    public record Load : IAction<TodoListState>
    {
        private record Effector(IStore Store, ITodoListApi Api, IActionDispatcher ActionDispatcher)
            : IEffect<TodoListState, Load>
        {
            public async Task Effect(TodoListState state, Load action)
            {
                var items = await Api.GetAll();

                await ActionDispatcher.Dispatch(new TaskLoaded(items));
            }
        }
    }

    public record TaskLoaded(IReadOnlyCollection<TaskListItem> Payload) : IAction<TodoListState>
    {
        private record Reducer(IStore Store) : IPureReducer<TodoListState, TaskLoaded>
        {
            public TodoListState Reduce(TodoListState state, TaskLoaded action) =>
                state with
                {
                    TodoListItems = action.Payload
                };
        }
    }
}