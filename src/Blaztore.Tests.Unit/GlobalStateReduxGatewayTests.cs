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

    public GlobalStateReduxGatewayTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x with
            {
                ConfigureMediator = configuration => configuration.RegisterServicesFromAssemblyContaining<TestGlobalState>()
            })
            .BuildServiceProvider();
        
        _gateway = serviceProvider.GetRequiredService<IGlobalStateReduxGateway<TestGlobalState>>();
        _store = serviceProvider.GetRequiredService<IStore>();
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

}