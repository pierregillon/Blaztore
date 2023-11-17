using Blaztore.ActionHandling;
using Blaztore.Components;
using Blaztore.Gateways;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Blaztore.Tests.Unit;

public class EffectTests
{
    private readonly IScopedStateReduxGateway<TestState,object> _gateway;
    private readonly Repository _repository;

    public EffectTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x with
            {
                ConfigureMediator = m => m.RegisterServicesFromAssemblyContaining<TestState>()
            })
            .AddSingleton<Repository>()
            .BuildServiceProvider();

        _gateway = serviceProvider.GetRequiredService<IScopedStateReduxGateway<TestState, object>>();
        _repository = serviceProvider.GetRequiredService<Repository>();
    }

    [Fact]
    public void An_effect_use_correct_scoped_state()
    {
        var scope = Guid.NewGuid();

        _gateway.SubscribeToState(Substitute.For<IComponentBase>(), scope);
        
        _gateway.Dispatch(new TestState.SetStateValue(scope, "state value"));
        _gateway.Dispatch(new TestState.SetInRepo(scope, "action value"));

        _repository
            .GetValue()
            .Should()
            .Be(new RepositoryValue("state value", "action value"));
    }
    
    public record TestState(string Value) : IScopedState<object>
    {
        public static TestState Initialize() => new(string.Empty);

        public record SetInRepo(object Scope, string Value) : IScopedAction<TestState, object>
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
        
        public record SetStateValue(object Scope, string Value) : IScopedAction<TestState, object>
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

        public RepositoryValue GetValue() => new(_stateValue, _actionValue);
    }

    public record RepositoryValue(string? StateValue, string? ActionValue);
}