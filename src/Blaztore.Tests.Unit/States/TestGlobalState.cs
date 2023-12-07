using Blaztore.ActionHandling;
using Blaztore.Actions;
using Blaztore.States;

namespace Blaztore.Tests.Unit.States;

public record TestGlobalState(string Value) : IGlobalState
{
    public static TestGlobalState Initialize() => new(string.Empty);

    public record DefineState(TestGlobalState NewState) : IAction<TestGlobalState>
    {
        public record Reducer(IStore Store) : IPureReducer<TestGlobalState, DefineState>
        {
            public TestGlobalState Reduce(TestGlobalState state, DefineState action) => action.NewState;
        }
    }
}