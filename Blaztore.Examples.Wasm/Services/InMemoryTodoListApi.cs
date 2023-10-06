namespace Blaztore.Examples.Wasm.Services;

public class InMemoryTodoListApi : ITodoListApi
{
    private readonly List<TaskListItem> _tasks = new();

    public Task<IReadOnlyCollection<TaskListItem>> GetAll() =>
        Task.FromResult((IReadOnlyCollection<TaskListItem>)_tasks);
}