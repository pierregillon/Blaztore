namespace Blaztore.Examples.Wasm.Services;

public interface ITodoListApi
{
    Task<IReadOnlyCollection<TaskListItem>> GetAll();
    Task Create(Guid id, string description);
}

public record TaskListItem(Guid Id, string Description);