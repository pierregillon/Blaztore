using Blaztore.ActionHandling;
using Blaztore.Actions;
using Blaztore.Components;
using Blaztore.Gateways;
using Blaztore.States;
using Blaztore.Tests.Unit.States;
using NSubstitute;

namespace Blaztore.Tests.Unit;

public abstract class ReRenderTests
{
    public class GlobalStateTests : TestBase
    {
        [Fact]
        public async Task Dispatching_an_action_does_not_re_render_unsubscribed_component()
        {
            var component = Components.CreateComponent();

            var gateway = GetService<IGlobalStateReduxGateway<TestGlobalState>>();

            var initialState = gateway.SubscribeToState(component);
            gateway.UnsubscribeFromState(component);

            await gateway.Dispatch(new TestGlobalState.DefineState(initialState));

            component
                .Received(0)
                .ReRender();
        }

        [Fact]
        public async Task Dispatching_an_action_that_does_not_change_state_does_not_re_render_subscribed_components()
        {
            var component = Components.CreateComponent();

            var gateway = GetService<IGlobalStateReduxGateway<TestGlobalState>>();

            var initialState = gateway.SubscribeToState(component);

            await gateway.Dispatch(new TestGlobalState.DefineState(initialState));

            component
                .Received(0)
                .ReRender();
        }

        [Fact]
        public async Task
            Dispatching_an_action_that_changes_state_to_an_equivalent_one_does_not_re_render_subscribed_components()
        {
            var component = Components.CreateComponent();

            var gateway = GetService<IGlobalStateReduxGateway<TestGlobalState>>();

            var initialState = gateway.SubscribeToState(component);

            var equivalentState = new TestGlobalState(initialState.Value);

            await gateway.Dispatch(new TestGlobalState.DefineState(equivalentState));

            component
                .Received(0)
                .ReRender();
        }

        [Fact]
        public async Task
            Dispatching_an_action_that_changes_state_to_an_equivalent_one_re_renders_subscribed_components_when_changes_is_collection()
        {
            var component = Substitute.For<IComponentBase>();

            var gateway = GetService<IGlobalStateReduxGateway<TestCollectionGlobalState>>();

            _ = gateway.SubscribeToState(component);

            var firstState = new TestCollectionGlobalState(new List<string> { "Test" });

            await gateway.Dispatch(new TestCollectionGlobalState.DefineState(firstState));

            component.ClearReceivedCalls();

            var secondState = new TestCollectionGlobalState(new List<string> { "Test" });

            await gateway.Dispatch(new TestCollectionGlobalState.DefineState(secondState));

            component
                .Received(1)
                .ReRender();
        }

        [Fact]
        public async Task Dispatching_an_action_that_changes_state_re_renders_all_subscribed_components()
        {
            var components = Components.CreateComponents().ToArray();

            var gateway = GetService<IGlobalStateReduxGateway<TestGlobalState>>();
            
            foreach (var component in components)
            {
                gateway.SubscribeToState(component);
            }

            var newState = new TestGlobalState("hello world");

            await gateway.Dispatch(new TestGlobalState.DefineState(newState));

            foreach (var component in components)
            {
                component
                    .Received(1)
                    .ReRender();
            }
        }
        
        private record TestCollectionGlobalState(IEnumerable<string> Values) : IGlobalState
        {
            public static TestCollectionGlobalState Initialize() => new(new List<string>());

            public record DefineState(TestCollectionGlobalState NewState) : IAction<TestCollectionGlobalState>
            {
                public record Reducer(IStore Store) : IPureReducer<TestCollectionGlobalState, DefineState>
                {
                    public TestCollectionGlobalState Reduce(TestCollectionGlobalState state, DefineState action) => action.NewState;
                }
            }
        }
    }

    public class ScopedStateTests : TestBase
    {
        [Fact]
        public async Task Dispatching_an_action_does_not_re_render_unsubscribed_component()
        {
            var components = Components.CreateComponent();

            var scope = Guid.NewGuid();

            var gateway = GetService<IScopedStateReduxGateway<TestScopedState, Guid>>();

            var initialState = gateway.SubscribeToState(components, scope);
            gateway.UnsubscribeFromState(components, scope);

            await gateway.Dispatch(new TestScopedState.DefineState(scope, initialState));

            components
                .Received(0)
                .ReRender();
        }

        [Fact]
        public async Task Dispatching_an_action_that_does_not_change_state_does_not_re_render_subscribed_components()
        {
            var component = Components.CreateComponent();

            var scope = Guid.NewGuid();

            var gateway = GetService<IScopedStateReduxGateway<TestScopedState, Guid>>();

            var initialState = gateway.SubscribeToState(component, scope);

            await gateway.Dispatch(new TestScopedState.DefineState(scope, initialState));

            component
                .Received(0)
                .ReRender();
        }

        [Fact]
        public async Task
            Dispatching_an_action_that_changes_state_to_an_equivalent_one_does_not_re_render_subscribed_components()
        {
            var component = Components.CreateComponent();

            var scope = Guid.NewGuid();

            var gateway = GetService<IScopedStateReduxGateway<TestScopedState, Guid>>();

            var initialState = gateway.SubscribeToState(component, scope);

            var equivalentState = new TestScopedState(initialState.Value);

            await gateway.Dispatch(new TestScopedState.DefineState(scope, equivalentState));

            component
                .Received(0)
                .ReRender();
        }

        [Fact]
        public async Task
            Dispatching_an_action_that_changes_state_to_an_equivalent_one_re_renders_subscribed_components_when_changes_is_collection()
        {
            var component = Substitute.For<IComponentBase>();

            var scope = Guid.NewGuid();

            var gateway = GetService<IScopedStateReduxGateway<TestCollectionGlobalState, Guid>>();

            _ = gateway.SubscribeToState(component, scope);

            var firstState = new TestCollectionGlobalState(new List<string> { "Test" });

            await gateway.Dispatch(new TestCollectionGlobalState.DefineState(scope, firstState));

            component.ClearReceivedCalls();

            var secondState = new TestCollectionGlobalState(new List<string> { "Test" });

            await gateway.Dispatch(new TestCollectionGlobalState.DefineState(scope, secondState));

            component
                .Received(1)
                .ReRender();
        }

        [Fact]
        public async Task Dispatching_an_action_that_changes_state_re_renders_all_subscribed_components()
        {
            var components = Components.CreateComponents().ToArray();

            var scope = Guid.NewGuid();
            
            var gateway = GetService<IScopedStateReduxGateway<TestScopedState, Guid>>();
            
            foreach (var component in components)
            {
                gateway.SubscribeToState(component, scope);
            }

            var newState = new TestScopedState("hello world");

            await gateway.Dispatch(new TestScopedState.DefineState(scope, newState));

            foreach (var component in components)
            {
                component
                    .Received(1)
                    .ReRender();
            }
        }

        [Fact]
        public async Task Dispatching_an_action_that_changes_state_does_not_re_renders_component_under_another_scope()
        {
            var scope1 = Guid.NewGuid();
            var component1 = Components.CreateComponent();
            var scope2 = Guid.NewGuid();
            var component2 = Components.CreateComponent();

            var gateway = GetService<IScopedStateReduxGateway<TestScopedState, Guid>>();
            
            gateway.SubscribeToState(component1, scope1);
            gateway.SubscribeToState(component2, scope2);

            var newState = new TestScopedState("hello world");

            await gateway.Dispatch(new TestScopedState.DefineState(scope2, newState));

            component1
                .Received(0)
                .ReRender();
            
            component2
                .Received(1)
                .ReRender();
        }
        
        private record TestCollectionGlobalState(IEnumerable<string> Values) : IScopedState<Guid>
        {
            public static TestCollectionGlobalState Initialize() => new(new List<string>());

            public record DefineState(Guid Scope, TestCollectionGlobalState NewState) : IScopedAction<TestCollectionGlobalState, Guid>
            {
                public record Reducer(IStore Store) : IPureReducer<TestCollectionGlobalState, DefineState>
                {
                    public TestCollectionGlobalState Reduce(TestCollectionGlobalState state, DefineState action) => action.NewState;
                }
            }
        }
    }

    public class ComponentStateTests : TestBase
    {
        [Fact]
        public async Task Dispatching_an_action_does_not_re_render_unsubscribed_component()
        {
            var component = Components.CreateComponent();

            var gateway = GetService<IComponentStateReduxGateway<TestComponentState>>();

            var initialState = gateway.SubscribeToState(component);
            gateway.UnsubscribeFromState(component);

            await gateway.Dispatch(new TestComponentState.DefineState(component.Id, initialState));

            component
                .Received(0)
                .ReRender();
        }

        [Fact]
        public async Task Dispatching_an_action_that_does_not_change_state_does_not_re_render_subscribed_components()
        {
            var component = Components.CreateComponent();

            var gateway = GetService<IComponentStateReduxGateway<TestComponentState>>();

            var initialState = gateway.SubscribeToState(component);

            await gateway.Dispatch(new TestComponentState.DefineState(component.Id, initialState));

            component
                .Received(0)
                .ReRender();
        }

        [Fact]
        public async Task
            Dispatching_an_action_that_changes_state_to_an_equivalent_one_does_not_re_render_subscribed_components()
        {
            var component = Components.CreateComponent();

            var gateway = GetService<IComponentStateReduxGateway<TestComponentState>>();

            var initialState = gateway.SubscribeToState(component);

            var equivalentState = new TestComponentState(initialState.Value);

            await gateway.Dispatch(new TestComponentState.DefineState(component.Id, equivalentState));

            component
                .Received(0)
                .ReRender();
        }

        [Fact]
        public async Task
            Dispatching_an_action_that_changes_state_to_an_equivalent_one_re_renders_subscribed_components_when_changes_is_collection()
        {
            var component = Components.CreateComponent();

            var gateway = GetService<IComponentStateReduxGateway<TestCollectionState>>();

            _ = gateway.SubscribeToState(component);

            var firstState = new TestCollectionState(new List<string> { "Test" });

            await gateway.Dispatch(new TestCollectionState.DefineState(component.Id, firstState));

            component.ClearReceivedCalls();

            var secondState = new TestCollectionState(new List<string> { "Test" });

            await gateway.Dispatch(new TestCollectionState.DefineState(component.Id, secondState));

            component
                .Received(1)
                .ReRender();
        }

        [Fact]
        public async Task Dispatching_an_action_that_changes_state_re_renders_only_the_correct_component()
        {
            var components = Components.CreateComponents().ToArray();

            var gateway = GetService<IComponentStateReduxGateway<TestComponentState>>();
            
            foreach (var component in components)
            {
                gateway.SubscribeToState(component);
            }

            var newState = new TestComponentState("hello world");

            await gateway.Dispatch(new TestComponentState.DefineState(components.First().Id, newState));

            components
                .First()
                .Received(1)
                .ReRender();
            
            foreach (var component in components.Skip(1))
            {
                component
                    .Received(0)
                    .ReRender();
            }
        }

        [Fact]
        public async Task Dispatching_an_action_that_changes_state_does_not_re_renders_component_under_another_scope()
        {
            var scope1 = Guid.NewGuid();
            var component1 = Components.CreateComponent();
            var scope2 = Guid.NewGuid();
            var component2 = Components.CreateComponent();

            var gateway = GetService<IScopedStateReduxGateway<TestScopedState, Guid>>();
            
            gateway.SubscribeToState(component1, scope1);
            gateway.SubscribeToState(component2, scope2);

            var newState = new TestScopedState("hello world");

            await gateway.Dispatch(new TestScopedState.DefineState(scope2, newState));

            component1
                .Received(0)
                .ReRender();
            
            component2
                .Received(1)
                .ReRender();
        }
        
        private record TestCollectionState(IEnumerable<string> Values) : IComponentState
        {
            public static TestCollectionState Initialize() => new(new List<string>());

            public record DefineState(ComponentId ComponentId, TestCollectionState NewState) : IComponentAction<TestCollectionState>
            {
                public record Reducer(IStore Store) : IPureReducer<TestCollectionState, DefineState>
                {
                    public TestCollectionState Reduce(TestCollectionState state, DefineState action) => action.NewState;
                }
            }
        }
    }
}