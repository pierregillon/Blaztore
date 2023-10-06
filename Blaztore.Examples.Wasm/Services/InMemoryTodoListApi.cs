namespace Blaztore.Examples.Wasm.Services;

public class InMemoryTodoListApi : ITodoListApi
{
    private readonly List<TaskListItem> _tasks = new();

    public Task<IReadOnlyCollection<TaskListItem>> GetAll() =>
        Task.FromResult((IReadOnlyCollection<TaskListItem>)_tasks);

    public Task Create(Guid id, string description)
    {
        _tasks.Add(new TaskListItem(id, description));
        return Task.CompletedTask;
    }
}