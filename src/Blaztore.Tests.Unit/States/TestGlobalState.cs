using Blaztore.ActionHandling;

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
public record TestCollectionGlobalState(IEnumerable<string> Values) : IGlobalState
{
    public static TestCollectionGlobalState Initialize() => new(new List<string>());

    public record DefineState(TestCollectionGlobalState NewState) : IAction<TestCollectionGlobalState>
    {
        public record Reducer(IStore Store) : IPureReducer<TestCollectionGlobalState, DefineState>
        {
            public TestCollectionGlobalState Reduce(TestCollectionGlobalState state, DefineState action) => action.NewState;
        }
    }
}