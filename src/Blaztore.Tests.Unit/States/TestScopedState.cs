using Blaztore.ActionHandling;
using Blaztore.Actions;
using Blaztore.States;

namespace Blaztore.Tests.Unit.States;

public record TestScopedState(string Value) : IScopedState<Guid>
{
    public static TestScopedState Initialize() => new(string.Empty);
        
    public record DefineState(Guid Scope, TestScopedState NewState) : IScopedAction<TestScopedState, Guid>
    {   
        public record Reducer(IStore Store) : IPureReducer<TestScopedState, DefineState>
        {
            public TestScopedState Reduce(TestScopedState state, DefineState action) => action.NewState;
        }
    }
}