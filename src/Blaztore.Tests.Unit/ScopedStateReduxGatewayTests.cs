using Blaztore.Gateways;
using Blaztore.States;
using Blaztore.Tests.Unit.States;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Blaztore.Tests.Unit;

public class ScopedStateReduxGatewayTests
{
    private readonly IScopedStateReduxGateway<ConcatState, Guid> _gateway ;
    private readonly IStore _store;

    public ScopedStateReduxGatewayTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x with
            {
                ConfigureMediator = m => m.RegisterServicesFromAssemblyContaining<ConcatState>()
            })
            .BuildServiceProvider();
        
        _gateway = serviceProvider.GetRequiredService<IScopedStateReduxGateway<ConcatState, Guid>>();
        _store = serviceProvider.GetRequiredService<IStore>();
    }
    
    [Fact]
    public void Subscribing_to_scoped_state_stores_state_with_scope()
    {
        var scope = Guid.NewGuid();

        var state = _gateway.SubscribeToState(Components.SomeComponent, scope);
        var state2 = _store.GetState<ConcatState>(scope);
        
        state
            .Should()
            .BeSameAs(state2);
    }
    
    [Fact]
    public async Task Does_not_execute_action_when_no_component_has_subscribed_to_state()
    {
        var scope = Guid.NewGuid();
        
        await _gateway.Dispatch(new ConcatState.Concat(scope, "my value"));

        var state = _store.GetState<TestGlobalState>(scope);

        state.Should().BeNull();
    }
    
    [Fact]
    public async Task Does_not_execute_action_when_component_unsubscribed()
    {
        var stateComponent = Components.SomeComponent;
        var scope = Guid.NewGuid();

        _gateway.SubscribeToState(stateComponent, scope);
        _gateway.UnsubscribeFromState(stateComponent, scope);
        
        await _gateway.Dispatch(new ConcatState.Concat(Guid.NewGuid(), "my value"));

        var state = _store.GetState<TestGlobalState>(scope);

        state.Should().BeNull();
    }
    
    [Fact]
    public async Task Does_not_execute_action_when_another_scoped_component_is_loaded()
    {
        var scope1 = Guid.NewGuid();
        var scope2 = Guid.NewGuid();
        
        _gateway.SubscribeToState(Components.SomeComponent, scope1);
        
        await _gateway.Dispatch(new ConcatState.Concat(scope2, "my value"));

        var state = _store.GetState<TestGlobalState>(scope2);

        state.Should().BeNull();
    }
    
    [Fact]
    public async Task Re_renders_only_subscribed_component_to_correct_scope()
    {
        var scope1 = Guid.NewGuid();
        var stateComponent1 = Components.CreateComponent();
        
        var scope2 = Guid.NewGuid();
        var stateComponent2 = Components.CreateComponent();

        _gateway.SubscribeToState(stateComponent1, scope1);
        _gateway.SubscribeToState(stateComponent2, scope2);

        await _gateway.Dispatch(new ConcatState.Concat(scope1, "Test"));

        stateComponent1
            .Received(1)
            .ReRender();
        
        stateComponent2
            .Received(0)
            .ReRender();
    }
}