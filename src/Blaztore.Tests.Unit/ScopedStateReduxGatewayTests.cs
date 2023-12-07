using Blaztore.Gateways;
using Blaztore.States;
using Blaztore.Tests.Unit.States;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Blaztore.Tests.Unit;

public class ScopedStateReduxGatewayTests
{
    private readonly IScopedStateReduxGateway<ConcatState, Guid> _gateway ;
    private readonly IStore _store;

    public ScopedStateReduxGatewayTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x with
            {
                ConfigureMediator = m => m.RegisterServicesFromAssemblyContaining<ConcatState>()
            })
            .BuildServiceProvider();
        
        _gateway = serviceProvider.GetRequiredService<IScopedStateReduxGateway<ConcatState, Guid>>();
        _store = serviceProvider.GetRequiredService<IStore>();
    }
    
    [Fact]
    public void Subscribing_to_scoped_state_stores_state_with_scope()
    {
        var scope = Guid.NewGuid();

        var state = _gateway.SubscribeToState(Components.SomeComponent, scope);
        var state2 = _store.GetState<ConcatState>(scope);
        
        state
            .Should()
            .BeSameAs(state2);
    }
}