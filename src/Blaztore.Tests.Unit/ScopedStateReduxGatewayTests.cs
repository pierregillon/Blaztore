using Blaztore.Components;
using Blaztore.Gateways;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Blaztore.Tests.Unit;

public class ScopedStateReduxGatewayTests
{
    private readonly IScopedStateReduxGateway<TestState, Guid> _gateway ;
    private readonly IStore _store;

    public ScopedStateReduxGatewayTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x.RegisterServicesFromAssemblyContaining<TestState>())
            .BuildServiceProvider();
        
        _gateway = serviceProvider.GetRequiredService<IScopedStateReduxGateway<TestState, Guid>>();
        _store = serviceProvider.GetRequiredService<IStore>();
    }
    
    [Fact]
    public void Subscribing_to_scoped_state_stores_state_with_scope()
    {
        var component = CreateComponent();

        var scope = Guid.NewGuid();
        
        _gateway.SubscribeToState(component, scope)
            .Should()
            .BeSameAs(_store.GetState<TestState>(scope));
    }

    private static IStateComponent CreateComponent()
    {
        var component = Substitute.For<IStateComponent>();

        component.Id.Returns(new ComponentId(Guid.NewGuid().ToString()));
        
        return component;
    }

    public record TestState : IState
    {
        public static TestState Initialize() => new();
    }
}