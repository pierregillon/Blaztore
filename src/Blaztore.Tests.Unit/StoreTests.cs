using Blaztore.States;
using FluentAssertions;

namespace Blaztore.Tests.Unit;

public class StoreTests
{
    [Fact]
    public void Cannot_instanciate_a_state_without_initialize_method()
    {
        var store = new InMemoryStore();

        Action action = () => store.GetStateOrCreateDefault<SomeStateWithoutInitializeMethod>();

        action
            .Should()
            .Throw<MissingInitializeMethodException>();
    }
    
    [Fact]
    public void Cannot_instanciate_a_state_without_a_specific_state_usage_defined()
    {
        var store = new InMemoryStore();

        Action action = () => store.GetStateOrCreateDefault<SomeStateWithoutSpecificStateInterface>();

        action
            .Should()
            .Throw<MissingStateUsageException>();
    }

    private record SomeStateWithoutInitializeMethod : IGlobalState;

    private record SomeStateWithoutSpecificStateInterface : IState
    {
        public static SomeStateWithoutSpecificStateInterface Initialize() => new();
    }
}