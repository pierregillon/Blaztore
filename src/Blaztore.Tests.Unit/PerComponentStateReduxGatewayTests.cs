using Blaztore.Components;
using Blaztore.Gateways;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Blaztore.Tests.Unit;

public class PerComponentStateReduxGatewayTests
{
    private readonly IComponentStateReduxGateway<TestState> _gateway ;
    private readonly IStore _store;

    public PerComponentStateReduxGatewayTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x.RegisterServicesFromAssemblyContaining<TestState>())
            .BuildServiceProvider();
        
        _gateway = serviceProvider.GetRequiredService<IComponentStateReduxGateway<TestState>>();
        _store = serviceProvider.GetRequiredService<IStore>();
    }
    
    [Fact]
    public void Subscribing_to_per_component_state_stores_state_for_component_id()
    {
        var component = CreateComponent();

        _gateway.SubscribeToState(component)
            .Should()
            .BeSameAs(_store.GetState<TestState>(component.Id));
    }

    private static IStateComponent CreateComponent()
    {
        var component = Substitute.For<IStateComponent>();

        component.Id.Returns(new ComponentId(Guid.NewGuid().ToString()));
        
        return component;
    }

    public record TestState : IComponentState
    {
        public static TestState Initialize() => new();
    }
}