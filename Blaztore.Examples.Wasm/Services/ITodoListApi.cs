namespace Blaztore.Examples.Wasm.Services;

public interface ITodoListApi
{
    Task<IReadOnlyCollection<TaskListItem>> GetAll();
}

public record TaskListItem(Guid Id, string Description);