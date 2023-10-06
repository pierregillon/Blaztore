namespace Blaztore.Examples.Wasm.Services;

public class InMemoryTodoListApi : ITodoListApi
{
    private IReadOnlyCollection<TaskListItem> _tasks = new List<TaskListItem>();

    public Task<IReadOnlyCollection<TaskListItem>> GetAll() =>
        Task.FromResult(_tasks);

    public Task Create(Guid id, string description)
    {
        _tasks = _tasks
            .Append(new TaskListItem(id, description, false))
            .ToList();
        
        return Task.CompletedTask;
    }

    public async Task Delete(Guid id)
    {
        _tasks = _tasks
            .Where(x => x.Id != id)
            .ToList();

        // Simulate a long operation
        await Task.Delay(500);
    }

    public Task MarkAsDone(Guid id)
    {
        _tasks = _tasks
            .Select(x => x.Id == id ? x with { IsDone = true } : x)
            .ToList();

        return Task.CompletedTask;
    }

    public Task MarkAsToDo(Guid id)
    {
        _tasks = _tasks
            .Select(x => x.Id == id ? x with { IsDone = false } : x)
            .ToList();

        return Task.CompletedTask;
    }
}