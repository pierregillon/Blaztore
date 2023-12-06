using Blaztore.Components;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;

namespace Blaztore.Tests.Unit;

public class ExtendedComponentBaseTests : TestContext
{
    [Fact]
    public void Does_not_notify_parameter_changed_when_not_changed()
    {
        var component = RenderComponent<MyComponent>();
        
        component.SetParametersAndRender();

        component
            .Instance
            .ChangedParameters
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void Does_not_notify_parameter_changed_when_same_values()
    {
        var component = RenderComponent<MyComponent>();
        
        component.SetParametersAndRender(ComponentParameter.CreateParameter("Name", "Test"));
        
        component.Instance.ChangedParameters.Clear();
        
        component.SetParametersAndRender(ComponentParameter.CreateParameter("Name", "Test"));

        component
            .Instance
            .ChangedParameters
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void Notify_only_parameters_that_had_changed()
    {
        var component = RenderComponent<MyComponent>();
        
        component.SetParametersAndRender(ComponentParameter.CreateParameter("Name", "Test"));

        component
            .Instance
            .ChangedParameters
            .Should()
            .HaveCount(1)
            .And.Subject
            .Single()
            .Should()
            .BeEquivalentTo(new[]
            {
                new ChangedParameter("Name", string.Empty, "Test")
            });
    }
    
    [Fact]
    public void Notify_multiple_parameters_in_alphabetical_order()
    {
        var component = RenderComponent<MyComponent>();
        
        component.SetParametersAndRender(
            ComponentParameter.CreateParameter("Name", "Test"), 
            ComponentParameter.CreateParameter("Class", "mud-card")
        );

        component
            .Instance
            .ChangedParameters
            .Should()
            .HaveCount(1)
            .And.Subject
            .Single()
            .Should()
            .BeEquivalentTo(new[]
            {
                new ChangedParameter("Class", string.Empty, "mud-card"),
                new ChangedParameter("Name", string.Empty, "Test"),
            }, x => x.WithStrictOrdering());
    }

    public class MyComponent : ExtendedComponentBase
    {
        [Parameter] public DateTime Date { get; set; }
        [Parameter] public string Name { get; set; } = string.Empty;
        [Parameter] public string Class { get; set; } = string.Empty;
        
        public readonly List<IReadOnlyCollection<ChangedParameter>> ChangedParameters = new();

        protected override Task OnParametersChangedAsync(IReadOnlyCollection<ChangedParameter> parameters)
        {
            ChangedParameters.Add(parameters);

            return Task.CompletedTask;
        }
    }
}