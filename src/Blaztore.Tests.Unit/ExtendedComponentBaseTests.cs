using Blaztore.Components;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;

namespace Blaztore.Tests.Unit;

public class ExtendedComponentBaseTests
{
    public class OnParametersChangedAsyncTests : TestContext
    {
        [Fact]
        public void Does_not_notify_parameter_changed_when_not_changed()
        {
            var component = RenderComponent<OnParametersChangedComponent>();

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
            var component = RenderComponent<OnParametersChangedComponent>();

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
        public void Notify_on_parameter_set_before_rendering()
        {
            var component = RenderComponent<OnParametersChangedComponent>(builder => builder.Add(x => x.Name, "Test"));

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
        public void Notify_only_parameters_that_had_changed()
        {
            var component = RenderComponent<OnParametersChangedComponent>();

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
            var component = RenderComponent<OnParametersChangedComponent>();

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
                    new ChangedParameter("Name", string.Empty, "Test")
                }, x => x.WithStrictOrdering());
        }
        
        [Fact]
        public void Notify_parameters_changed_with_correct_previous_value()
        {
            var component = RenderComponent<OnParametersChangedComponent>();

            component.SetParametersAndRender(ComponentParameter.CreateParameter("Name", "Test"));
            
            component.Instance.ChangedParameters.Clear();
            
            component.SetParametersAndRender(ComponentParameter.CreateParameter("Name", "Test 2"));

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
                    new ChangedParameter("Name", "Test", "Test 2")
                });
        }
        
        public class OnParametersChangedComponent : ExtendedComponentBase
        {
            [Parameter] public DateTime Date { get; set; }
            [Parameter] public string Name { get; set; } = string.Empty;
            [Parameter] public string Class { get; set; } = string.Empty;

            public readonly List<ChangedParameters> ChangedParameters = new();

            protected override Task OnParametersChangedAsync(ChangedParameters parameters)
            {
                ChangedParameters.Add(parameters);

                return Task.CompletedTask;
            }
        }
    }
    
    public class OnParametersChangedAfterComponentRenderedTests : TestContext
    {
        [Fact]
        public void Does_not_notify_parameter_changed_when_not_changed()
        {
            var component = RenderComponent<OnParametersChangedAfterComponentRenderedComponent>();

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
            var component = RenderComponent<OnParametersChangedAfterComponentRenderedComponent>();

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
        public void Does_not_notify_on_parameter_set_before_rendering()
        {
            var component = RenderComponent<OnParametersChangedAfterComponentRenderedComponent>(
                builder => builder.Add(x => x.Name, "Test")
            );

            component
                .Instance
                .ChangedParameters
                .Should()
                .BeEmpty();
        }
        
        [Fact]
        public void Notify_only_parameters_that_had_changed()
        {
            var component = RenderComponent<OnParametersChangedAfterComponentRenderedComponent>();

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
            var component = RenderComponent<OnParametersChangedAfterComponentRenderedComponent>();

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
                    new ChangedParameter("Name", string.Empty, "Test")
                }, x => x.WithStrictOrdering());
        }
        
        [Fact]
        public void Notify_parameters_changed_with_correct_previous_value()
        {
            var component = RenderComponent<OnParametersChangedAfterComponentRenderedComponent>();

            component.SetParametersAndRender(ComponentParameter.CreateParameter("Name", "Test"));
            
            component.Instance.ChangedParameters.Clear();
            
            component.SetParametersAndRender(ComponentParameter.CreateParameter("Name", "Test 2"));

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
                    new ChangedParameter("Name", "Test", "Test 2")
                });
        }
        
        public class OnParametersChangedAfterComponentRenderedComponent : ExtendedComponentBase
        {
            [Parameter] public DateTime Date { get; set; }
            [Parameter] public string Name { get; set; } = string.Empty;
            [Parameter] public string Class { get; set; } = string.Empty;

            public readonly List<ChangedParameters> ChangedParameters = new();

            protected override Task OnParametersChangedAfterComponentRendered(ChangedParameters parameters)
            {
                ChangedParameters.Add(parameters);

                return Task.CompletedTask;
            }
        }
    }
}