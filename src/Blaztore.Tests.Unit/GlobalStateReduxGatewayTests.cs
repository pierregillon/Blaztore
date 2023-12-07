using Blaztore.Components;
using Blaztore.Gateways;
using Blaztore.States;
using Blaztore.Tests.Unit.States;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Blaztore.Tests.Unit;

public class GlobalStateReduxGatewayTests
{
    private readonly IGlobalStateReduxGateway<TestGlobalState> _gateway ;
    private readonly IStore _store;
    private readonly ServiceProvider _serviceProvider;

    public GlobalStateReduxGatewayTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x with
            {
                ConfigureMediator = configuration => configuration.RegisterServicesFromAssemblyContaining<TestGlobalState>()
            })
            .BuildServiceProvider();
        
        _gateway = _serviceProvider.GetRequiredService<IGlobalStateReduxGateway<TestGlobalState>>();
        _store = _serviceProvider.GetRequiredService<IStore>();
    }
    
    [Fact]
    public void Subscribing_to_global_state_stores_a_state_globally()
    {
        var state1 = _gateway.SubscribeToState(Components.SomeComponent);
        var state2 = _store.GetState<TestGlobalState>();
        
        state1
            .Should()
            .BeSameAs(state2);
    }
    
    [Fact]
    public async Task Does_not_execute_action_when_no_component_has_subscribed_to_state()
    {
        await _gateway.Dispatch(new TestGlobalState.DefineState(new TestGlobalState("my value")));

        var state = _store.GetState<TestGlobalState>();

        state.Should().BeNull();
    }
    
    [Fact]
    public async Task Does_not_execute_action_when_component_unsubscribed()
    {
        var stateComponent = Components.SomeComponent;
        
        _gateway.SubscribeToState(stateComponent);
        _gateway.UnsubscribeFromState(stateComponent);
        
        await _gateway.Dispatch(new TestGlobalState.DefineState(new TestGlobalState("my value")));

        var state = _store.GetState<TestGlobalState>();

        state.Should().BeNull();
    }
    
    [Fact]
    public async Task Dispatching_an_action_that_does_not_change_state_does_not_re_render_subscribed_components()
    {
        var stateComponent = Components.CreateComponent();

        var initialState = _gateway.SubscribeToState(stateComponent);
        
        await _gateway.Dispatch(new TestGlobalState.DefineState(initialState));
        
        stateComponent
            .Received(0)
            .ReRender();
    }
    
    [Fact]
    public async Task Dispatching_an_action_that_changes_state_to_an_equivalent_one_does_not_re_render_subscribed_components()
    {
        var stateComponent = Components.CreateComponent();

        var initialState = _gateway.SubscribeToState(stateComponent);
        
        await _gateway.Dispatch(new TestGlobalState.DefineState(new TestGlobalState(initialState.Value)));
        
        stateComponent
            .Received(0)
            .ReRender();
    }
    
    [Fact]
    public async Task Dispatching_an_action_that_changes_state_to_an_equivalent_one_re_renders_subscribed_components_when_changes_is_collection()
    {
        var stateComponent = Substitute.For<IComponentBase>();

        var gatewayOfCollectionState = _serviceProvider.GetRequiredService<IGlobalStateReduxGateway<TestCollectionGlobalState>>();
        
        _ = gatewayOfCollectionState.SubscribeToState(stateComponent);
        
        await gatewayOfCollectionState.Dispatch(new TestCollectionGlobalState.DefineState(new TestCollectionGlobalState(new List<string>{"Test"})));

        stateComponent.ClearReceivedCalls();

        await gatewayOfCollectionState.Dispatch(new TestCollectionGlobalState.DefineState(new TestCollectionGlobalState(new List<string>{"Test"})));
        
        stateComponent
            .Received(1)
            .ReRender();
    }
    
    [Fact]
    public async Task Dispatching_an_action_that_changes_state_re_renders_subscribed_components()
    {
        var stateComponents = Components.CreateComponents().ToArray();

        foreach (var stateComponent in stateComponents)
        {
            _gateway.SubscribeToState(stateComponent);
        }

        var newState = new TestGlobalState("hello world");
        
        await _gateway.Dispatch(new TestGlobalState.DefineState(newState));

        foreach (var stateComponent in stateComponents)
        {
            stateComponent
                .Received(1)
                .ReRender();
        }
    }

}