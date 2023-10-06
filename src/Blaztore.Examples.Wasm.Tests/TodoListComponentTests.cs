using AutoFixture;
using Blaztore.Examples.Wasm.Components;
using Blaztore.Examples.Wasm.Services;
using Bunit;
using NSubstitute;

namespace Blaztore.Examples.Wasm.Tests;

public class TodoListComponentTests : BUnitTest
{
    private readonly TaskListItem[] _todoListItems;

    public TodoListComponentTests()
    {
        var fixture = new Fixture();

        _todoListItems = fixture
            .CreateMany<TaskListItem>()
            .ToArray();

        GetService<ITodoListApi>()
            .GetAll()
            .Returns(_todoListItems);
    }

    [Fact]
    public void Renders_todo_list_items()
    {
        var component = RenderComponent<TodoListComponent>();

        foreach (var todoListItem in _todoListItems)
        {
            component
                .Find($"li:contains('{todoListItem.Description}')")
                .Should()
                .NotBeNull();
        }
    }
}