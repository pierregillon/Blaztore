using Blaztore.ActionHandling;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Blaztore.Tests.Unit;

public class PureReducerTests
{
    private readonly IStore _store;
    private readonly IActionDispatcher _actionDispatcher;

    public PureReducerTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x.RegisterServicesFromAssemblyContaining<TestState>())
            .BuildServiceProvider();

        _store = serviceProvider.GetRequiredService<IStore>();
        _actionDispatcher = serviceProvider.GetRequiredService<IActionDispatcher>();
    }

    [Fact]
    public void Reducing_updates_scoped_state_based_on_action_scope()
    {
        var scope = Guid.NewGuid();
        
        _actionDispatcher.Dispatch(new TestState.Concat(scope, "Test"));
        _actionDispatcher.Dispatch(new TestState.Concat(scope, "Test1"));
        
        var state = _store.GetState<TestState>(scope);

        state!.Value.Should().Be("TestTest1");
    }

    [Fact]
    public void Reducing_does_not_update_state_with_other_scope()
    {
        var scope1 = Guid.NewGuid();
        _actionDispatcher.Dispatch(new TestState.Concat(scope1, "Test1"));
        
        var scope2 = Guid.NewGuid();
        _actionDispatcher.Dispatch(new TestState.Concat(scope2, "Test2"));
        
        var state1 = _store.GetState<TestState>(scope1);
        
        state1!.Value
            .Should()
            .Be("Test1");
        
        var state2 = _store.GetState<TestState>(scope2);
        
        state2!.Value
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