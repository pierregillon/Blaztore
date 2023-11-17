using Blaztore.ActionHandling;
using Blaztore.Components;

namespace Blaztore.Tests.Unit.States;

public record TestComponentState(string Value) : IComponentState
{
    public static TestComponentState Initialize() => new(string.Empty);
        
    public record DefineState(ComponentId ComponentId, TestComponentState NewState) : IComponentAction<TestComponentState>
    {   
        public record Reducer(IStore Store) : IPureReducer<TestComponentState, DefineState>
        {
            public TestComponentState Reduce(TestComponentState state, DefineState action) => action.NewState;
        }

        public object Scope => ComponentId;
    }
}