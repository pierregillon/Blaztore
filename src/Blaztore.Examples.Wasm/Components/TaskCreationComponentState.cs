using Blaztore.ActionHandling;
using Blaztore.Examples.Wasm.Services;

namespace Blaztore.Examples.Wasm.Components;

public record TaskCreationState(bool IsAddingTask, string? NewTaskDescription) : IState
{
    public static TaskCreationState Initialize() => new(false, null);

    public record Load : IAction<TaskCreationState>
    {
        private record Reducer(IStore Store) : IPureReducerNoAction<TaskCreationState, Load>
        {
            public TaskCreationState Reduce(TaskCreationState state) =>
                state with
                {
                    IsAddingTask = false,
                    NewTaskDescription = null
                };
        }
    }

    public record StartAddingNewTask : IAction<TaskCreationState>
    {
        private record Reducer(IStore Store) : IPureReducerNoAction<TaskCreationState, StartAddingNewTask>
        {
            public TaskCreationState Reduce(TaskCreationState state) =>
                state with
                {
                    IsAddingTask = true
                };
        }
    }

    public record DefineNewDescription(string NewDescription) : IAction<TaskCreationState>
    {
        private record Reducer(IStore Store) : IPureReducer<TaskCreationState, DefineNewDescription>
        {
            public TaskCreationState Reduce(TaskCreationState state, DefineNewDescription action) =>
                state with
                {
                    NewTaskDescription = action.NewDescription
                };
        }
    }


    public record ExecuteTaskCreation : IAction<TaskCreationState>
    {
        private record Effector(IStore Store, ITodoListApi Api, IActionDispatcher ActionDispatcher)
            : IEffect<TaskCreationState, ExecuteTaskCreation>
        {
            public async Task Effect(TaskCreationState state, ExecuteTaskCreation action)
            {
                if (string.IsNullOrWhiteSpace(state.NewTaskDescription))
                {
                    return;
                }

                await Api.Create(Guid.NewGuid(), state.NewTaskDescription);
                await ActionDispatcher.Dispatch(new EndAddingNewTask());
                await ActionDispatcher.Dispatch(new TodoListState.Load());
            }
        }
    }

    public record EndAddingNewTask : IAction<TaskCreationState>
    {
        private record Reducer(IStore Store) : IPureReducerNoAction<TaskCreationState, EndAddingNewTask>
        {
            public TaskCreationState Reduce(TaskCreationState state) =>
                state with
                {
                    NewTaskDescription = null,
                    IsAddingTask = false
                };
        }
    }
}