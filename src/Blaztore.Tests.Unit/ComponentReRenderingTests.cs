using Blaztore.ActionHandling;
using Blaztore.Components;
using Blaztore.Gateways;
using Blaztore.Tests.Unit.States;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Blaztore.Tests.Unit;

public class ComponentReRenderingTests
{
    private readonly IGlobalStateReduxGateway<TestGlobalState> _gateway;
    private readonly IComponentStateReduxGateway<TestComponentState> _componentGateway;

    public ComponentReRenderingTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x with
            {
                ConfigureMediator = m => m.RegisterServicesFromAssemblyContaining<TestComponentState>()
            })
            .BuildServiceProvider();

        _gateway = serviceProvider.GetRequiredService<IGlobalStateReduxGateway<TestGlobalState>>();
        _componentGateway = serviceProvider.GetRequiredService<IComponentStateReduxGateway<TestComponentState>>();
    }
    
    [Fact]
    public void Dispatching_an_action_that_does_not_change_state_does_not_re_render_subscribed_components()
    {
        var stateComponent = Substitute.For<IStateComponent>();

        var initialState = _gateway.SubscribeToState(stateComponent);
        
        _gateway.Dispatch(new TestGlobalState.DefineState(initialState));
        
        stateComponent
            .Received(0)
            .ReRender();
    }
    
    [Fact]
    public void Dispatching_an_action_that_changes_state_re_renders_subscribed_components()
    {
        var stateComponents = CreateStateComponents().ToArray();

        foreach (var stateComponent in stateComponents)
        {
            _gateway.SubscribeToState(stateComponent);
        }

        var newState = new TestGlobalState("hello world");
        
        _gateway.Dispatch(new TestGlobalState.DefineState(newState));

        foreach (var stateComponent in stateComponents)
        {
            stateComponent
                .Received(1)
                .ReRender();
        }
    }
    
    [Fact]
    public void Re_renders_only_component_subscribed_to_state_with_valid_scope()
    {
        var stateComponent1 = CreateStateComponent();
        var stateComponent2 = CreateStateComponent();

        _componentGateway.SubscribeToState(stateComponent1);
        _componentGateway.SubscribeToState(stateComponent2);

        var newState = new TestComponentState("hello world");
        
        _componentGateway.Dispatch(new TestComponentState.DefineState(stateComponent1.Id, newState));

        stateComponent1
            .Received(1)
            .ReRender();
        
        stateComponent2
            .Received(0)
            .ReRender();
    }

    private static IEnumerable<IStateComponent> CreateStateComponents()
    {
        for (var i = 0; i < new Random((int)DateTime.Now.Ticks).Next(20); i++)
        {
            yield return CreateStateComponent();
        }
    }

    private static IStateComponent CreateStateComponent()
    {
        var component = Substitute.For<IStateComponent>();
        component.Id.Returns(ComponentId.New());
        return component;
    }
}