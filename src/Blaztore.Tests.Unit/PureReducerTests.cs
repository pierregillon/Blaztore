using Blaztore.ActionHandling;
using Blaztore.Components;
using Blaztore.Gateways;
using Blaztore.States;
using Blaztore.Tests.Unit.States;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Blaztore.Tests.Unit;

public class PureReducerTests
{
    private readonly IScopedStateReduxGateway<ConcatState, Guid> _gateway;
    private readonly IStore _store;

    public PureReducerTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x with
            {
                ConfigureMediator = m => m.RegisterServicesFromAssemblyContaining<ConcatState>()
            })
            .BuildServiceProvider();

        _store = serviceProvider.GetRequiredService<IStore>();
        _gateway = serviceProvider.GetRequiredService<IScopedStateReduxGateway<ConcatState, Guid>>();
    }

    [Fact]
    public void Reducing_updates_scoped_state_based_on_action_scope()
    {
        var scope = Guid.NewGuid();

        _gateway.SubscribeToState(Substitute.For<IComponentBase>(), scope);
        
        _gateway.Dispatch(new ConcatState.Concat(scope, "Test"));
        _gateway.Dispatch(new ConcatState.Concat(scope, "Test1"));
        
        var state = _store.GetState<ConcatState>(scope);

        state!.Value.Should().Be("TestTest1");
    }

    [Fact]
    public void Reducing_does_not_update_state_with_other_scope()
    {
        var scope1 = Guid.NewGuid();
        _gateway.SubscribeToState(Substitute.For<IComponentBase>(), scope1);
        _gateway.Dispatch(new ConcatState.Concat(scope1, "Test1"));
        
        var scope2 = Guid.NewGuid();
        _gateway.SubscribeToState(Substitute.For<IComponentBase>(), scope2);
        _gateway.Dispatch(new ConcatState.Concat(scope2, "Test2"));
        
        var state1 = _store.GetState<ConcatState>(scope1);
        
        state1!.Value
            .Should()
            .Be("Test1");
        
        var state2 = _store.GetState<ConcatState>(scope2);
        
        state2!.Value
            .Should()
            .Be("Test2");
    }
}