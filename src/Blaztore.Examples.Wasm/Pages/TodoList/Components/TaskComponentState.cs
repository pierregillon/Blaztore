using Blaztore.ActionHandling;
using Blaztore.Examples.Wasm.Services;

namespace Blaztore.Examples.Wasm.Pages.TodoList.Components;

public record TaskComponentState(bool IsDeleting) : IScopedState<Guid>
{
    public static TaskComponentState Initialize() => new(false);
    
    public record DeleteItem(Guid Id) : ITaskAction
    {
        private record Effector(IStore Store, ITodoListApi Api, IActionDispatcher ActionDispatcher)
            : IEffect<TaskComponentState, DeleteItem>
        {
            public async Task Effect(TaskComponentState state, DeleteItem action)
            {
                await ActionDispatcher.Dispatch(new StartDeleting(action.Id));
                await Api.Delete(action.Id);
                await ActionDispatcher.Dispatch(new EndDeleting(action.Id));
                await ActionDispatcher.Dispatch(new TodoListState.Load());
            }
        }
    }

    public record StartDeleting(Guid Id) : ITaskAction
    {
        private record Reducer(IStore Store) : IPureReducerNoAction<TaskComponentState, StartDeleting>
        {
            public TaskComponentState Reduce(TaskComponentState state) =>
                state with
                {
                    IsDeleting = true
                };
        }
    }

    public record EndDeleting(Guid Id) : ITaskAction
    {
        private record Reducer(IStore Store) : IPureReducerNoAction<TaskComponentState, EndDeleting>
        {
            public TaskComponentState Reduce(TaskComponentState state) =>
                state with
                {
                    IsDeleting = false
                };
        }
    }

    private interface ITaskAction : IScopedAction<TaskComponentState, Guid>
    {
        public Guid Id { get; }
        Guid IScopedAction<TaskComponentState, Guid>.Scope => Id;
    }
}