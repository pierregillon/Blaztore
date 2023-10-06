using AutoFixture;
using Blaztore.Examples.Wasm.Components;
using Blaztore.Examples.Wasm.Services;
using Bunit;
using FluentAssertions;
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

    [Fact]
    public void Renders_new_task_button_by_default()
    {
        var component = RenderComponent<TodoListComponent>();

        component
            .Find("button:contains('New Task')")
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void Clicking_on_new_task_button_hides_it()
    {
        var component = RenderComponent<TodoListComponent>();

        component
            .Find("button:contains('New Task')")
            .Click();

        component
            .FindAll("button:contains('New Task')")
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void Clicking_on_new_task_button_renders_input_and_validate_button()
    {
        var component = RenderComponent<TodoListComponent>();

        component
            .Find("button:contains('New Task')")
            .Click();

        component
            .Find("input")
            .Should()
            .NotBeNull();

        component
            .Find("button:contains('Validate')")
            .Should()
            .NotBeNull();
    }
}