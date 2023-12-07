using Blaztore.ActionHandling;
using Blaztore.Actions;
using Blaztore.Components;
using Blaztore.Gateways;
using Blaztore.States;
using Blaztore.Tests.Unit.States;
using FluentAssertions;

namespace Blaztore.Tests.Unit.Behaviours;

public abstract class StateInstanciableFromActionExecutionTests
{
    public class GlobalStateTests : TestBase
    {
        [Fact]
        public async Task By_default_does_not_execute_action_when_no_component_has_subscribed_to_state()
        {
            var gateway = GetService<IGlobalStateReduxGateway<TestGlobalState>>();
            
            await gateway.Dispatch(new TestGlobalState.DefineState(new TestGlobalState("my value")));

            var state = Store.GetState<TestGlobalState>();

            state.Should().BeNull();
        }
        
        [Fact]
        public async Task By_default_does_not_execute_action_when_state_destroyed_after_last_component_unsubscribed()
        {
            var stateComponent = Components.SomeComponent;
        
            var gateway = GetService<IGlobalStateReduxGateway<TestGlobalState>>();
            
            gateway.SubscribeToState(stateComponent);
            gateway.UnsubscribeFromState(stateComponent);
        
            await gateway.Dispatch(new TestGlobalState.DefineState(new TestGlobalState("my value")));

            var state = Store.GetState<TestGlobalState>();

            state.Should().BeNull();
        }

        [Fact]
        public async Task Can_execute_action_when_state_is_persistent_after_last_component_unsubscribed()
        {
            var stateComponent = Components.SomeComponent;
        
            var gateway = GetService<IGlobalStateReduxGateway<PersistentState>>();
            
            gateway.SubscribeToState(stateComponent);
            gateway.UnsubscribeFromState(stateComponent);

            var newState = new PersistentState("my value");
            
            await gateway.Dispatch(new PersistentState.DefineState(newState));

            var state = Store.GetState<PersistentState>();

            state.Should().Be(newState);
        }

        [Fact]
        public async Task Execute_action_on_default_state_when_state_is_instanciable_from_action_execution()
        {
            var gateway = GetService<IGlobalStateReduxGateway<StateInstanciableFromActionExecution>>();

            var newState = new StateInstanciableFromActionExecution("my value");
            
            await gateway.Dispatch(new StateInstanciableFromActionExecution.DefineState(newState));

            var state = Store.GetState<StateInstanciableFromActionExecution>();

            state.Should().Be(newState);
        }
        
        private record PersistentState(string Value) : IGlobalState, IPersistentLifecycleState
        {
            public static PersistentState Initialize() => new(string.Empty);

            public record DefineState(PersistentState NewState) : IAction<PersistentState>
            {
                public record Reducer(IStore Store) : IPureReducer<PersistentState, DefineState>
                {
                    public PersistentState Reduce(PersistentState state, DefineState action) => action.NewState;
                }
            }
        }
        private record StateInstanciableFromActionExecution(string Value) : IGlobalState, IStateInstanciableFromActionExecution
        {
            public static StateInstanciableFromActionExecution Initialize() => new(string.Empty);

            public record DefineState(StateInstanciableFromActionExecution NewState) : IAction<StateInstanciableFromActionExecution>
            {
                public record Reducer(IStore Store) : IPureReducer<StateInstanciableFromActionExecution, DefineState>
                {
                    public StateInstanciableFromActionExecution Reduce(StateInstanciableFromActionExecution state, DefineState action) => action.NewState;
                }
            }
        }
    }
    
    public class ScopedStateTests : TestBase
    {
        [Fact]
        public async Task By_default_does_not_execute_action_when_no_component_has_subscribed_to_state()
        {
            var scope = Guid.NewGuid();
            
            var gateway = GetService<IScopedStateReduxGateway<ConcatState, Guid>>();
            
            await gateway.Dispatch(new ConcatState.Concat(scope, "my value"));

            var state = Store.GetState<ConcatState>(scope);

            state.Should().BeNull();
        }
        
        [Fact]
        public async Task By_default_does_not_execute_action_when_state_destroyed_after_last_component_unsubscribed()
        {
            var scope = Guid.NewGuid();
            
            var stateComponent = Components.SomeComponent;
        
            var gateway = GetService<IScopedStateReduxGateway<ConcatState, Guid>>();
            
            gateway.SubscribeToState(stateComponent, scope);
            gateway.UnsubscribeFromState(stateComponent, scope);
        
            await gateway.Dispatch(new ConcatState.Concat(scope, "my value"));

            var state = Store.GetState<ConcatState>(scope);

            state.Should().BeNull();
        }

        [Fact]
        public async Task Can_execute_action_when_state_is_persistent_after_last_component_unsubscribed()
        {
            var scope = Guid.NewGuid();
            
            var stateComponent = Components.SomeComponent;
        
            var gateway = GetService<IScopedStateReduxGateway<PersistentState, Guid>>();
            
            gateway.SubscribeToState(stateComponent, scope);
            gateway.UnsubscribeFromState(stateComponent, scope);

            var newState = new PersistentState("my value");
            
            await gateway.Dispatch(new PersistentState.DefineState(scope, newState));

            var state = Store.GetState<PersistentState>(scope);

            state.Should().Be(newState);
        }

        [Fact]
        public async Task Execute_action_on_default_state_when_state_is_instanciable_from_action_execution()
        {
            var scope = Guid.NewGuid();
            
            var gateway = GetService<IScopedStateReduxGateway<StateInstanciableFromActionExecution, Guid>>();

            var newState = new StateInstanciableFromActionExecution("my value");
            
            await gateway.Dispatch(new StateInstanciableFromActionExecution.DefineState(scope, newState));

            var state = Store.GetState<StateInstanciableFromActionExecution>(scope);

            state.Should().Be(newState);
        }
        
        private record PersistentState(string Value) : IScopedState<Guid>, IPersistentLifecycleState
        {
            public static PersistentState Initialize() => new(string.Empty);

            public record DefineState(Guid Scope, PersistentState NewState) : IScopedAction<PersistentState, Guid>
            {
                public record Reducer(IStore Store) : IPureReducer<PersistentState, DefineState>
                {
                    public PersistentState Reduce(PersistentState state, DefineState action) => action.NewState;
                }
            }
        }
        private record StateInstanciableFromActionExecution(string Value) : IScopedState<Guid>, IStateInstanciableFromActionExecution
        {
            public static StateInstanciableFromActionExecution Initialize() => new(string.Empty);

            public record DefineState(Guid Scope, StateInstanciableFromActionExecution NewState) : IScopedAction<StateInstanciableFromActionExecution, Guid>
            {
                public record Reducer(IStore Store) : IPureReducer<StateInstanciableFromActionExecution, DefineState>
                {
                    public StateInstanciableFromActionExecution Reduce(StateInstanciableFromActionExecution state, DefineState action) => action.NewState;
                }
            }
        }
    }
    
    public class ComponentStateTests : TestBase
    {
        [Fact]
        public async Task By_default_does_not_execute_action_when_no_component_has_subscribed_to_state()
        {
            var component = Components.CreateComponent();
            
            var gateway = GetService<IComponentStateReduxGateway<TestComponentState>>();

            await gateway.Dispatch(new TestComponentState.DefineState(component.Id, new TestComponentState("my value")));

            var state = Store.GetState<ConcatState>(component.Id);

            state.Should().BeNull();
        }
        
        [Fact]
        public async Task By_default_does_not_execute_action_when_state_destroyed_after_last_component_unsubscribed()
        {
            var component = Components.CreateComponent();
        
            var gateway = GetService<IComponentStateReduxGateway<TestComponentState>>();
            
            gateway.SubscribeToState(component);
            gateway.UnsubscribeFromState(component);
        
            await gateway.Dispatch(new TestComponentState.DefineState(component.Id, new TestComponentState("my value")));

            var state = Store.GetState<ConcatState>(component.Id);

            state.Should().BeNull();
        }
    
        [Fact]
        public async Task By_default_does_not_execute_action_when_the_target_component_has_no_state()
        {
            var component1 = Components.CreateComponent();
            var component2 = Components.CreateComponent();
        
            var gateway = GetService<IComponentStateReduxGateway<TestComponentState>>();
            
            gateway.SubscribeToState(component1);
            gateway.SubscribeToState(component2);
        
            gateway.UnsubscribeFromState(component2);
            
            await gateway.Dispatch(new TestComponentState.DefineState(component2.Id, new TestComponentState("Test")));

            var state = Store.GetState<TestComponentState>(component2.Id);

            state.Should().BeNull();
        }

        [Fact]
        public async Task Execute_action_on_default_state_when_state_is_instanciable_from_action_execution()
        {
            var component = Components.CreateComponent();
            
            var gateway = GetService<IComponentStateReduxGateway<StateInstanciableFromActionExecution>>();

            var newState = new StateInstanciableFromActionExecution("my value");

            await gateway.Dispatch(new StateInstanciableFromActionExecution.DefineState(component.Id, newState));

            var state = Store.GetState<StateInstanciableFromActionExecution>(component.Id);

            state.Should().Be(newState);
        }
        
        private record StateInstanciableFromActionExecution(string Value) : IComponentState, IStateInstanciableFromActionExecution
        {
            public static StateInstanciableFromActionExecution Initialize() => new(string.Empty);

            public record DefineState(ComponentId ComponentId, StateInstanciableFromActionExecution NewState) : IComponentAction<StateInstanciableFromActionExecution>
            {
                public record Reducer(IStore Store) : IPureReducer<StateInstanciableFromActionExecution, DefineState>
                {
                    public StateInstanciableFromActionExecution Reduce(StateInstanciableFromActionExecution state, DefineState action) => action.NewState;
                }
            }
        }
    }
}