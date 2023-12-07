using Blaztore.ActionHandling;
using Blaztore.Actions;
using Blaztore.Components;
using Blaztore.Gateways;
using Blaztore.States;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

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