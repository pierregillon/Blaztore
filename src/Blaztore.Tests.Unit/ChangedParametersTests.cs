using Blaztore.Components;
using FluentAssertions;

namespace Blaztore.Tests.Unit;

public class ChangedParametersTests
{
    [Fact]
    public void Has_changed_returns_true_on_valid_parameter_name()
    {
        var parameters = new ChangedParameters(new[]
        {
            new ChangedParameter("Name", "old", "new")
        });

        parameters
            .HasChanged("Name")
            .Should()
            .BeTrue();
    }
}