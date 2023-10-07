using Blaztore.ActionHandling;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Blaztore.Tests.Unit;

public class EffectTests
{
    private readonly IActionDispatcher _actionDispatcher;
    private readonly ServiceProvider _serviceProvider;

    public EffectTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x.RegisterServicesFromAssemblyContaining<TestState>())
            .AddSingleton<Repository>()
            .BuildServiceProvider();

        _actionDispatcher = _serviceProvider.GetRequiredService<IActionDispatcher>();
    }

    [Fact]
    public void An_effect_use_correct_scoped_state()
    {
        var scope = Guid.NewGuid();
        
        _actionDispatcher.Dispatch(new TestState.SetStateValue(scope, "state value"));
        _actionDispatcher.Dispatch(new TestState.SetInRepo(scope, "action value"));

        _serviceProvider.GetRequiredService<Repository>()
            .GetActionValue()
            .Should()
            .Be("action value");

        _serviceProvider.GetRequiredService<Repository>()
            .GetStateValue()
            .Should()
            .Be("state value");
    }
    
    public record TestState(string Value) : IState
    {
        public static TestState Initialize() => new(string.Empty);

        public record SetInRepo(object Scope, string Value) : IAction<TestState>, IActionOnScopedState
        {
            public record Effector(IStore Store, Repository Repository) : IEffect<TestState, SetInRepo>
            {
                public Task Effect(TestState state, SetInRepo action)
                {
                    Repository.Set(state.Value, action.Value);
                    return Task.CompletedTask;
                }
            }
        }
        
        public record SetStateValue(object Scope, string Value) : IAction<TestState>, IActionOnScopedState
        {
            public record Reducer(IStore Store) : IPureReducer<TestState, SetStateValue>
            {
                public TestState Reduce(TestState state, SetStateValue action) =>
                    state with
                    {
                        Value = state.Value + action.Value
                    };
            }
        }
    }

    public class Repository
    {
        private string? _stateValue;
        private string? _actionValue;

        public void Set(string stateValue, string actionValue)
        {
            _stateValue = stateValue;
            _actionValue = actionValue;
        }

        public string? GetActionValue() => _actionValue;
        public string? GetStateValue() => _stateValue;
    }
}