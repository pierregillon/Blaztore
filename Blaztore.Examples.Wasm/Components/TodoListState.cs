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

    public record StartAddingNewTask : IAction<TodoListState>
    {
        private record Reducer(IStore Store) : IPureReducer<TodoListState, StartAddingNewTask>
        {
            public TodoListState Reduce(TodoListState state, StartAddingNewTask action) =>
                state with
                {
                    IsAddingTask = true
                };
        }
    }

    public record DefineNewDescription(string NewDescription) : IAction<TodoListState>
    {
        private record Reducer(IStore Store) : IPureReducer<TodoListState, DefineNewDescription>
        {
            public TodoListState Reduce(TodoListState state, DefineNewDescription action) =>
                state with
                {
                    NewTaskDescription = action.NewDescription
                };
        }
    }

    public record ExecuteTaskCreation : IAction<TodoListState>
    {
        private record Effector(IStore Store, ITodoListApi Api, IActionDispatcher ActionDispatcher)
            : IEffect<TodoListState, ExecuteTaskCreation>
        {
            public async Task Effect(TodoListState state, ExecuteTaskCreation action)
            {
                if (string.IsNullOrWhiteSpace(state.NewTaskDescription))
                {
                    return;
                }

                await Api.Create(Guid.NewGuid(), state.NewTaskDescription);
                await ActionDispatcher.Dispatch(new EndAddingNewTask());
                await ActionDispatcher.Dispatch(new Load());
            }
        }
    }

    public record EndAddingNewTask : IAction<TodoListState>
    {
        private record Reducer(IStore Store) : IPureReducer<TodoListState, EndAddingNewTask>
        {
            public TodoListState Reduce(TodoListState state, EndAddingNewTask action) =>
                state with
                {
                    NewTaskDescription = null,
                    IsAddingTask = false
                };
        }
    }
}