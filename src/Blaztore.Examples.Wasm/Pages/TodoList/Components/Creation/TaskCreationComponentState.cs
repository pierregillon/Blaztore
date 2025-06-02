using Blaztore.ActionHandling;
using Blaztore.Actions;
using Blaztore.Events;
using Blaztore.Examples.Wasm.Pages.TodoList.Components.Creation.GenericActions;
using Blaztore.Examples.Wasm.Services;
using Blaztore.States;

namespace Blaztore.Examples.Wasm.Pages.TodoList.Components.Creation;

public record TaskCreationState(bool IsAddingTask, string? NewTaskDescription) : IGlobalState
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
        private record Effector(IStore Store, ITodoListApi Api, IActionDispatcher ActionDispatcher, IEventPublisher EventPublisher)
            : IEffect<TaskCreationState, ExecuteTaskCreation>
        {
            public async Task Effect(TaskCreationState state, ExecuteTaskCreation action)
            {
                if (string.IsNullOrWhiteSpace(state.NewTaskDescription))
                {
                    return;
                }

                await ActionDispatcher.Dispatch(
                    new CreateTask(state.NewTaskDescription), 
                    new EndAddingNewTask()
                );
                
                await EventPublisher.Publish(new TaskCreated(state.NewTaskDescription));
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

public record TaskCreated(string Description) : IEvent;