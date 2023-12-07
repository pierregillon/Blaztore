using Blaztore.ActionHandling;
using Blaztore.Actions;
using Blaztore.States;

namespace Blaztore.Tests.Unit.States;

public record ConcatState(string Value) : IScopedState<Guid>
{
    public static ConcatState Initialize() => new(string.Empty);

    public record Concat(Guid Scope, string Value) : IScopedAction<ConcatState, Guid>
    {
        public record Reducer(IStore Store) : IPureReducer<ConcatState, Concat>
        {
            public ConcatState Reduce(ConcatState state, Concat action) =>
                state with
                {
                    Value = state.Value + action.Value
                };
        }
    }
}