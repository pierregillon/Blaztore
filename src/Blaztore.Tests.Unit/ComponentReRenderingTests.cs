using Blaztore.ActionHandling;
using Blaztore.Components;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Blaztore.Tests.Unit;

public class PureReducerTests
{
    private readonly IStore _store;
    private readonly Subscriptions _subscriptions;
    private readonly IActionDispatcher _actionDispatcher;

    public PureReducerTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x.RegisterServicesFromAssemblyContaining<TestState>())
            .BuildServiceProvider();

        _store = serviceProvider.GetRequiredService<IStore>();
        _subscriptions = serviceProvider.GetRequiredService<Subscriptions>();
        _actionDispatcher = serviceProvider.GetRequiredService<IActionDispatcher>();
    }

    [Fact]
    public void Reducing_updates_scoped_state_based_on_action_scope()
    {
        var scope = Guid.NewGuid();
        
        _actionDispatcher.Dispatch(new TestState.Concat(scope, "Test"));
        _actionDispatcher.Dispatch(new TestState.Concat(scope, "Test1"));
        
        var state = _store.GetState<TestState>(scope);

        state.Value.Should().Be("TestTest1");
    }

    [Fact]
    public void Reducing_does_not_update_state_with_other_scope()
    {
        var scope1 = Guid.NewGuid();
        _actionDispatcher.Dispatch(new TestState.Concat(scope1, "Test1"));
        
        var scope2 = Guid.NewGuid();
        _actionDispatcher.Dispatch(new TestState.Concat(scope2, "Test2"));
        
        var state1 = _store.GetState<TestState>(scope1);
        state1.Value
            .Should()
            .Be("Test1");
        
        var state2 = _store.GetState<TestState>(scope2);
        state2.Value
            .Should()
            .Be("Test2");
    }
    
    public record TestState(string Value) : IState
    {
        public static TestState Initialize() => new(string.Empty);

        public record Concat(object Scope, string Value) : IAction<TestState>, IActionOnScopedState
        {
            public record Reducer(IStore Store) : IPureReducer<TestState, Concat>
            {
                public TestState Reduce(TestState state, Concat action) =>
                    state with
                    {
                        Value = state.Value + action.Value
                    };
            }
        }
    }
}

public class ComponentReRenderingTests
{
    private readonly IStore _store;
    private readonly Subscriptions _subscriptions;
    private readonly IActionDispatcher _actionDispatcher;

    public ComponentReRenderingTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x.RegisterServicesFromAssemblyContaining<TestState>())
            .BuildServiceProvider();

        _store = serviceProvider.GetRequiredService<IStore>();
        _subscriptions = serviceProvider.GetRequiredService<Subscriptions>();
        _actionDispatcher = serviceProvider.GetRequiredService<IActionDispatcher>();
    }
    
    [Fact]
    public void Dispatching_an_action_that_does_not_change_state_does_not_re_render_subscribed_components()
    {
        var stateComponent = Substitute.For<IStateComponent>();
        
        _subscriptions.Add(typeof(TestState), DefaultScope.Value, stateComponent);
        
        var state = _store.GetState<TestState>();
        
        _actionDispatcher.Dispatch(new TestState.DefineState(state));
        
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
            _subscriptions.Add(typeof(TestState), DefaultScope.Value, stateComponent);
        }

        var newState = new TestState("hello world");
        
        _actionDispatcher.Dispatch(new TestState.DefineState(newState));

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

        _subscriptions.Add(typeof(TestState), stateComponent1.Id, stateComponent1);
        _subscriptions.Add(typeof(TestState), stateComponent2.Id, stateComponent2);

        var newState = new TestState("hello world");
        
        _actionDispatcher.Dispatch(new TestState.DefineState2(stateComponent1.Id, newState));

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
        component.Id.Returns(new ComponentId(Guid.NewGuid().ToString(), 1));
        return component;
    }

    public record TestState(string Value) : IState
    {
        public static TestState Initialize() => new(string.Empty);

        public record DefineState(TestState NewState) : IAction<TestState>
        {
            public record Reducer(IStore Store) : IPureReducer<TestState, DefineState>
            {
                public TestState Reduce(TestState state, DefineState action) => action.NewState;
            }
        }
        
        public record DefineState2(ComponentId ComponentId, TestState NewState) : IAction<TestState>, IActionOnScopedState
        {   
            public record Reducer(IStore Store) : IPureReducer<TestState, DefineState2>
            {
                public TestState Reduce(TestState state, DefineState2 action) => action.NewState;
            }

            public object Scope => ComponentId;
        }
    }
}