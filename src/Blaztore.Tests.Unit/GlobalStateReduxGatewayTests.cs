using Blaztore.ActionHandling;
using Blaztore.Components;
using Blaztore.Gateways;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Blaztore.Tests.Unit;

public class GlobalStateReduxGatewayTests
{
    private readonly IGlobalStateReduxGateway<TestState> _gateway ;
    private readonly IStore _store;
    private readonly IActionDispatcher _dispatcher;

    public GlobalStateReduxGatewayTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x.RegisterServicesFromAssemblyContaining<TestState>())
            .BuildServiceProvider();
        
        _gateway = serviceProvider.GetRequiredService<IGlobalStateReduxGateway<TestState>>();
        _store = serviceProvider.GetRequiredService<IStore>();
        _dispatcher = serviceProvider.GetRequiredService<IActionDispatcher>();
    }
    
    [Fact]
    public void Subscribing_to_global_state_stores_global_state()
    {
        var component = CreateComponent();

        _gateway.SubscribeToState(component)
            .Should()
            .BeSameAs(_store.GetState<TestState>());
    }

    private static IStateComponent CreateComponent()
    {
        var component = Substitute.For<IStateComponent>();

        component.Id.Returns(new ComponentId(Guid.NewGuid().ToString()));
        
        return component;
    }

    public record TestState(bool IsLoaded) : IGlobalState
    {
        public static TestState Initialize() => new(false);

        public record Load : IAction<TestState>
        {
            internal record Reducer(IStore Store) : IPureReducer<TestState, Load>
            {
                public TestState Reduce(TestState state, Load action) =>
                    state with { IsLoaded = true };
            }
        }
    }
}