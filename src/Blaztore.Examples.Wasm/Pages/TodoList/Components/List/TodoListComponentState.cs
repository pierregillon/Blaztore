using Blaztore.ActionHandling;
using Blaztore.Actions;
using Blaztore.Events;
using Blaztore.Examples.Wasm.Pages.TodoList.Components.Creation;
using Blaztore.Examples.Wasm.Services;
using Blaztore.States;

namespace Blaztore.Examples.Wasm.Pages.TodoList.Components.List;

public record TodoListState(IReadOnlyCollection<TaskListItem> TodoListItems) : IGlobalState
{
    public static TodoListState Initialize() => new(new List<TaskListItem>());

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
    
    public record ToggleIsDone(Guid Id, bool IsDone) : IAction<TodoListState>
    {
        private record Effector(IStore Store, ITodoListApi Api, IActionDispatcher ActionDispatcher)
            : IEffect<TodoListState, ToggleIsDone>
        {
            public async Task Effect(TodoListState state, ToggleIsDone action)
            {
                if (action.IsDone)
                {
                    await Api.MarkAsDone(action.Id);
                }
                else
                {
                    await Api.MarkAsToDo(action.Id);
                }
                await ActionDispatcher.Dispatch(new Load());
            }
        }
    }

    public record ReloadOnTaskCreated(IActionDispatcher ActionDispatcher) : IEventListener<TaskCreated>
    {
        public async Task On(TaskCreated @event) =>
            await ActionDispatcher.Dispatch(new Load());
    }
}