namespace Blaztore.Examples.Wasm.Services;

public interface ITodoListApi
{
    Task<IReadOnlyCollection<TaskListItem>> GetAll();
    Task Create(Guid id, string description);
    Task Delete(Guid id);
    Task MarkAsDone(Guid id);
    Task MarkAsToDo(Guid id);
}

public record TaskListItem(Guid Id, string Description, bool IsDone);