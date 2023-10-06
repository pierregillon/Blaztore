using Blaztore.Components;
using NSubstitute;

namespace Blaztore.Tests.Unit;

public class SubscriptionsTests
{
    private readonly Subscriptions _subscriptions;

    public SubscriptionsTests()
    {
        _subscriptions = new Subscriptions();
    }
    
    [Fact]
    public void Re_renders_subscribed_component()
    {
        var component = CreateComponent();
        
        _subscriptions.Add(typeof(TestState), DefaultScope.Value, component);
        
        _subscriptions.ReRenderSubscribers(typeof(TestState));
        
        component
            .Received(1)
            .ReRender();
    }
    
    [Fact]
    public void Does_not_subscribe_same_component_multiple_times()
    {
        var component = CreateComponent();
        
        _subscriptions.Add(typeof(TestState), DefaultScope.Value, component);
        _subscriptions.Add(typeof(TestState), DefaultScope.Value, component);
        
        _subscriptions.ReRenderSubscribers(typeof(TestState));
        
        component
            .Received(1)
            .ReRender();
    }
    
    [Fact]
    public void Do_not_re_render_component_that_has_been_removed()
    {
        var component = CreateComponent();
        
        _subscriptions.Add(typeof(TestState), DefaultScope.Value, component);

        _subscriptions.Remove(component);
        
        _subscriptions.ReRenderSubscribers(typeof(TestState));
        
        component
            .Received(0)
            .ReRender();
    }

    private static IStateComponent CreateComponent()
    {
        var stateComponent = Substitute.For<IStateComponent>();
        stateComponent.Id.Returns(new ComponentId(Guid.NewGuid().ToString(), 1));
        return stateComponent;
    }

    public record TestState(string Value) : IState
    {
        public static TestState Initialize() => new(string.Empty);
    }
}