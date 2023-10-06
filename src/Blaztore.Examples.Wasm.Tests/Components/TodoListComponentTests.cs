using AutoFixture;
using Blaztore.Examples.Wasm.Pages.TodoList.Components;
using Blaztore.Examples.Wasm.Services;
using Bunit;
using NSubstitute;

namespace Blaztore.Examples.Wasm.Tests.Components;

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
                .Find($"*:contains('{todoListItem.Description}')")
                .Should()
                .NotBeNull();
        }
    }
    
    [Fact]
    public void Renders_for_each_item_a_delete_button()
    {
        var component = RenderComponent<TodoListComponent>();

        foreach (var todoListItem in _todoListItems)
        {
            component
                .Find($"*:contains('{todoListItem.Description}') button:contains('Delete')")
                .Should()
                .NotBeNull();
        }
    }
    
    [Fact]
    public void Clicking_on_delete_button_post_delete_to_api()
    {
        var component = RenderComponent<TodoListComponent>();

        var firstItem = _todoListItems.First();

        component
            .Find($"*:contains('{firstItem.Description}') button:contains('Delete')")
            .Click();

        GetService<ITodoListApi>()
            .Received(1)
            .Delete(firstItem.Id);
    }
    
    [Fact]
    public void Deleting_an_item_reloads_list()
    {
        var component = RenderComponent<TodoListComponent>();

        var firstItem = _todoListItems.First();
        
        GetService<ITodoListApi>().ClearReceivedCalls();

        component
            .Find($"*:contains('{firstItem.Description}') button:contains('Delete')")
            .Click();

        GetService<ITodoListApi>()
            .Received(1)
            .GetAll();
    }
    
    [Fact]
    public void Renders_for_each_item_a_check_box()
    {
        var component = RenderComponent<TodoListComponent>();

        foreach (var todoListItem in _todoListItems)
        {
            component
                .Find($"*:contains('{todoListItem.Description}') input[type='checkbox']")
                .Should()
                .NotBeNull();
        }
    }
    
    [Fact]
    public void Checking_item_box_mark_as_done_on_api()
    {
        var component = RenderComponent<TodoListComponent>();

        var firstItem = _todoListItems.First();

        component
            .Find($"*:contains('{firstItem.Description}') input[type='checkbox']")
            .Input(true);

        GetService<ITodoListApi>()
            .Received(1)
            .MarkAsDone(firstItem.Id);
    }
    
    [Fact]
    public void Unchecking_item_box_mark_as_to_do_on_api()
    {
        var component = RenderComponent<TodoListComponent>();

        var firstItem = _todoListItems.First();

        component
            .Find($"*:contains('{firstItem.Description}') input[type='checkbox']")
            .Input(false);

        GetService<ITodoListApi>()
            .Received(1)
            .MarkAsToDo(firstItem.Id);
    }
}