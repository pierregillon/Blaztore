using Blaztore.Gateways;
using Blaztore.States;
using Blaztore.Tests.Unit.States;
using FluentAssertions;

namespace Blaztore.Tests.Unit.Behaviours;

public abstract class StateLifecyleTests
{
    public class GlobalStateTests : TestBase
    {
        [Fact]
        public void By_default_a_state_is_destroyed_on_last_component_unsubscribed()
        {
            var components = Components.CreateComponents().ToArray();

            var gateway = GetService<IGlobalStateReduxGateway<TestGlobalState>>();
        
            foreach (var component in components)
            {
                gateway.SubscribeToState(component);
            }

            foreach (var component in components)
            {
                gateway.UnsubscribeFromState(component);
            }

            Store.GetState<TestGlobalState>()
                .Should()
                .BeNull();
        }
    
        [Fact]
        public void Persistent_state_are_preserved_on_last_component_unsubscribed()
        {
            var components = Components.CreateComponents().ToArray();

            var gateway = GetService<IGlobalStateReduxGateway<PersistentState>>();

            foreach (var component in components)
            {
                gateway.SubscribeToState(component);
            }

            foreach (var component in components)
            {
                gateway.UnsubscribeFromState(component);
            }

            Store.GetState<PersistentState>()
                .Should()
                .NotBeNull();
        }

        public record PersistentState : IGlobalState, IPersistentLifecycleState
        {
            public static PersistentState Initialize() => new();
        }
    }
    
    public class ScopedStateTests : TestBase
    {
        [Fact]
        public void By_default_a_scoped_state_is_destroyed_on_last_component_unsubscribed()
        {
            var scope = Guid.NewGuid();
            var components = Components.CreateComponents().ToArray();

            var gateway = GetService<IScopedStateReduxGateway<ConcatState, Guid>>();
        
            foreach (var component in components)
            {
                gateway.SubscribeToState(component, scope);
            }

            foreach (var component in components)
            {
                gateway.UnsubscribeFromState(component, scope);
            }

            Store.GetState<ConcatState>(scope)
                .Should()
                .BeNull();
        }
    
        [Fact]
        public void Persistent_state_are_preserved_on_last_component_unsubscribed()
        {
            var scope = Guid.NewGuid();
            var components = Components.CreateComponents().ToArray();

            var gateway = GetService<IScopedStateReduxGateway<PersistentState, Guid>>();

            foreach (var component in components)
            {
                gateway.SubscribeToState(component, scope);
            }

            foreach (var component in components)
            {
                gateway.UnsubscribeFromState(component, scope);
            }

            Store.GetState<PersistentState>(scope)
                .Should()
                .NotBeNull();
        }

        public record PersistentState : IScopedState<Guid>, IPersistentLifecycleState
        {
            public static PersistentState Initialize() => new();
        }
    }
    
    public class ComponentStateTests : TestBase
    {
        [Fact]
        public void By_default_a_component_state_is_destroyed_on_component_unsubscribed()
        {
            var component = Components.CreateComponent();

            var gateway = GetService<IComponentStateReduxGateway<TestComponentState>>();
        
            gateway.SubscribeToState(component);
            gateway.UnsubscribeFromState(component);

            Store.GetState<TestComponentState>()
                .Should()
                .BeNull();
        }
    
        [Fact]
        public void Component_state_can_never_be_persistent()
        {
            var component = Components.CreateComponent();

            var gateway = GetService<IComponentStateReduxGateway<WrongComponentPersistentState>>();
        
            Action action = () => gateway.SubscribeToState(component);

            action.Should().Throw<ComponentStateCannotBePersistentException>();
        }

        public record WrongComponentPersistentState : IComponentState, IPersistentLifecycleState
        {
            public static WrongComponentPersistentState Initialize() => new();
        }
    }
}