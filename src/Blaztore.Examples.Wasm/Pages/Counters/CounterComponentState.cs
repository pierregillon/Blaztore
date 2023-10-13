using Blaztore.ActionHandling;
using Blaztore.Components;

namespace Blaztore.Examples.Wasm.Pages.Counters;

public record CounterComponentState(TimeSpan CurrentTime, bool IsStarted, Timer? Timer) : IState
{
    public static CounterComponentState Initialize() => new(TimeSpan.Zero, false, null);

    public record Start(ComponentId? ComponentId) : IComponentAction<CounterComponentState>
    {
        internal record Effector(IStore Store, IActionDispatcher ActionDispatcher) : IPureReducer<CounterComponentState, Start>
        {
            public CounterComponentState Reduce(CounterComponentState state, Start action)
            {
                const int milliseconds = 10;

                return state with
                {
                    Timer = new Timer(_ => ActionDispatcher.Dispatch(new Count(action.ComponentId, milliseconds)), null, milliseconds, milliseconds),
                    IsStarted = true
                };
            }
        }
    }

    public record Count(ComponentId? ComponentId, int Milliseconds) : IComponentAction<CounterComponentState>
    {
        internal record Effector(IStore Store) : IPureReducer<CounterComponentState, Count>
        {
            public CounterComponentState Reduce(CounterComponentState state, Count action) =>
                state with
                {
                    CurrentTime = state.CurrentTime + TimeSpan.FromMilliseconds(action.Milliseconds)
                };
        }
    }

    public record Stop(ComponentId? ComponentId) : IComponentAction<CounterComponentState>
    {
        internal record Effector(IStore Store) : IPureReducerNoAction<CounterComponentState, Stop>
        {
            public CounterComponentState Reduce(CounterComponentState state)
            {
                state.Timer?.Dispose();
                
                return state with
                {
                    Timer = null,
                    IsStarted = false
                };
            }
        }
    }
    
    public record Reset(ComponentId? ComponentId) : IComponentAction<CounterComponentState>
    {
        internal record Effector(IStore Store) : IPureReducerNoAction<CounterComponentState, Reset>
        {
            public CounterComponentState Reduce(CounterComponentState state)
            {
                return state with
                {
                    CurrentTime = TimeSpan.Zero
                };
            }
        }
    }
}