using Blaztore.Components;
using NSubstitute;

namespace Blaztore.Tests.Unit;

public static class Components
{
    public static readonly IComponentBase SomeComponent = CreateComponent();
    
    public static IComponentBase CreateComponent()
    {
        var component = Substitute.For<IComponentBase>();

        component.Id.Returns(new ComponentId(Guid.NewGuid().ToString()));
        
        return component;
    }

    public static IEnumerable<IComponentBase> CreateComponents()
    {
        for (var i = 0; i < new Random((int)DateTime.Now.Ticks).Next(20); i++)
        {
            yield return CreateComponent();
        }
    }
}