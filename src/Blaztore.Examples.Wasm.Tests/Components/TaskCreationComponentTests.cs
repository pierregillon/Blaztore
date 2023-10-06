using Blaztore.Examples.Wasm.Pages.TodoList.Components;
using Blaztore.Examples.Wasm.Services;
using Bunit;
using FluentAssertions;
using NSubstitute;

namespace Blaztore.Examples.Wasm.Tests.Components;

public class TaskCreationComponentTests : BUnitTest
{
    [Fact]
    public void Renders_new_task_button_by_default()
    {
        var component = RenderComponent<TaskCreationComponent>();

        component
            .Find("button:contains('New task')")
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void Clicking_on_new_task_button_hides_it()
    {
        var component = RenderComponent<TaskCreationComponent>();

        component
            .Find("button:contains('New task')")
            .Click();

        component
            .FindAll("button:contains('New task')")
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void Clicking_on_new_task_button_renders_input_and_validate_button()
    {
        var component = RenderComponent<TaskCreationComponent>();

        component
            .Find("button:contains('New task')")
            .Click();

        component
            .Find("input")
            .Should()
            .NotBeNull();

        component
            .Find("button:contains('Validate')")
            .Should()
            .NotBeNull()
            .And
            .BeDisabled();
    }

    [Fact]
    public void Filling_input_enables_validate_button()
    {
        var component = RenderComponent<TaskCreationComponent>();

        component
            .Find("button:contains('New task')")
            .Click();

        component
            .Find("input")
            .Input("task 1");

        component
            .Find("button:contains('Validate')")
            .Should()
            .BeEnabled();
    }

    [Fact]
    public void Validating_posts_request_to_api()
    {
        var component = RenderComponent<TaskCreationComponent>();

        CreateTask(component, "task 1");

        GetService<ITodoListApi>()
            .Received(1)
            .Create(Arg.Any<Guid>(), "task 1");
    }

    [Fact]
    public void Creating_new_task_reset_form_creation()
    {
        var component = RenderComponent<TaskCreationComponent>();

        CreateTask(component, "task 1");

        component
            .FindAll("input")
            .Should()
            .BeEmpty();

        component
            .FindAll("button:contains('Validate')")
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void Creating_new_task_reload_list()
    {
        var component = RenderComponent<TaskCreationComponent>();

        GetService<ITodoListApi>().ClearReceivedCalls();

        CreateTask(component, "task 1");

        GetService<ITodoListApi>()
            .Received(1)
            .GetAll();
    }

    private static void CreateTask(IRenderedFragment component, string description)
    {
        component
            .Find("button:contains('New task')")
            .Click();

        component
            .Find("input")
            .Input(description);

        component
            .Find("button:contains('Validate')")
            .Click();
    }
}