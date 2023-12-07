using System.ComponentModel;
using Blaztore.ActionHandling;
using Blaztore.Components;
using Blaztore.Gateways;
using Blaztore.Tests.Unit.States;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Blaztore.Tests.Unit;

public class ComponentStateReduxGatewayTests
{
    private readonly IComponentStateReduxGateway<CounterState> _gateway ;
    private readonly IStore _store;

    public ComponentStateReduxGatewayTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x with
            {
                ConfigureMediator = m => m.RegisterServicesFromAssemblyContaining<CounterState>()
            })
            .BuildServiceProvider();
        
        _gateway = serviceProvider.GetRequiredService<IComponentStateReduxGateway<CounterState>>();
        _store = serviceProvider.GetRequiredService<IStore>();
    }
    
    [Fact]
    public void Subscribing_to_component_state_stores_state_for_component_id()
    {
        var component = Components.CreateComponent();

        var subscribeToState = _gateway.SubscribeToState(component);
        
        var counterState = _store.GetState<CounterState>(component.Id);

        subscribeToState
            .Should()
            .BeSameAs(counterState);
    }
    
    [Fact]
    public async Task Executes_action_when_no_component_has_subscribed_to_state_when_overriden_in_configuration()
    {
        var serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x with
            {
                ConfigureMediator = m => m.RegisterServicesFromAssemblyContaining<CounterState>(),
                CanInitializeStateFromActionExecution = true
            })
            .BuildServiceProvider();
        
        var gateway = serviceProvider.GetRequiredService<IComponentStateReduxGateway<CounterState>>();
        var store = serviceProvider.GetRequiredService<IStore>();
        
        var componentId = ComponentId.New();
        
        await gateway.Dispatch(new CounterState.Increment(componentId));

        var state = store.GetState<CounterState>(componentId);

        state.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Does_not_execute_action_when_no_component_has_subscribed_to_state()
    {
        var componentId = ComponentId.New();
        
        await _gateway.Dispatch(new CounterState.Increment(componentId));

        var state = _store.GetState<CounterState>(componentId);

        state.Should().BeNull();
    }
    
    [Fact]
    public async Task Does_not_execute_action_when_component_unsubscribed()
    {
        var stateComponent = Components.SomeComponent;

        _gateway.SubscribeToState(stateComponent);
        _gateway.UnsubscribeFromState(stateComponent);
        
        await _gateway.Dispatch(new CounterState.Increment(stateComponent.Id));

        var state = _store.GetState<CounterState>(stateComponent.Id);

        state.Should().BeNull();
    }
    
    [Fact]
    public async Task Does_not_execute_action_when_the_target_component_is_not_loaded()
    {
        var component1 = Components.CreateComponent();
        var component2 = Components.CreateComponent();
        
        _gateway.SubscribeToState(component1);
        _gateway.SubscribeToState(component2);
        
        _gateway.UnsubscribeFromState(component2);
        await _gateway.Dispatch(new CounterState.Increment(component2.Id));

        var state = _store.GetState<CounterState>(component2.Id);

        state.Should().BeNull();
    }

    [Fact]
    public async Task Re_renders_only_subscribed_component()
    {
        var stateComponent1 = Components.CreateComponent();
        var stateComponent2 = Components.CreateComponent();

        _gateway.SubscribeToState(stateComponent1);
        _gateway.SubscribeToState(stateComponent2);

        await _gateway.Dispatch(new CounterState.Increment(stateComponent1.Id));

        stateComponent1
            .Received(1)
            .ReRender();
        
        stateComponent2
            .Received(0)
            .ReRender();
    }

    public record CounterState(int Value) : IComponentState
    {
        public static CounterState Initialize() => new(0);

        public record Increment(ComponentId ComponentId) : IComponentAction<CounterState>
        {
            internal record Reducer(IStore Store) : IPureReducer<CounterState, Increment>
            {
                public CounterState Reduce(CounterState state, Increment action) =>
                    state with { Value = state.Value + 1 };
            }
        }
    }
}