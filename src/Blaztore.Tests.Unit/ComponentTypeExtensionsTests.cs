using Blaztore.Components;
using FluentAssertions;
using Microsoft.AspNetCore.Components;

namespace Blaztore.Tests.Unit;

public class ComponentTypeExtensionsTests
{
    [Fact]
    public void Get_parameter_properties_of_a_class()
    {
        var propertyNames = typeof(SomeComponent)
            .GetParameterProperties()
            .Select(x => x.Name);
        
        propertyNames
            .Should()
            .BeEquivalentTo(new[]
            {
                "Name",
                "Value"
            }, o => o.WithStrictOrdering());
    }
    
    [Fact]
    public void Getting_parameters_is_consistent()
    {
        var values = Enumerable.Range(0, 10)
            .Select(_ => typeof(SomeComponent).GetParameterProperties())
            .ToArray();

        values
            .All(x => values.All(x.Equals))
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void Getting_parameters_is_thread_safe()
    {
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(() => typeof(SomeComponent).GetParameterProperties()))
            .Cast<Task>()
            .ToArray();

        var action = () => Task.WaitAll(tasks);

        action
            .Should()
            .NotThrow();
    }
    
    [Fact]
    public void Getting_multiple_times_the_parameter_properties_of_the_same_type_uses_cache()
    {
        _ = typeof(SomeComponent).GetParameterProperties();

        new Action(() => typeof(SomeComponent).GetParameterProperties())
            .ExecutionTime()
            .Should()
            .BeLessThan(TimeSpan.FromMilliseconds(1));
    }

    private class SomeComponent
    {
        [Parameter] public string Name { get; set; } = string.Empty;
        [Parameter] public int Value { get; set; }
        public string Other { get; set; } = string.Empty;
    }
}