using Blaztore.Components;
using NSubstitute;

namespace Blaztore.Tests.Unit;

public static class Components
{
    public static readonly IStateComponent SomeComponent = CreateComponent();
    
    public static IStateComponent CreateComponent()
    {
        var component = Substitute.For<IStateComponent>();

        component.Id.Returns(new ComponentId(Guid.NewGuid().ToString()));
        
        return component;
    }
}