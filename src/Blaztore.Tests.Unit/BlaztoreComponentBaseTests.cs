using Blaztore.ActionHandling;
using Blaztore.Actions;
using Blaztore.Components;
using Blaztore.States;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Blaztore.Tests.Unit;

public class BlaztoreComponentBaseTests
{
    public class GlobalStateTests : TestContext
    {
        public GlobalStateTests() =>
            Services.AddBlaztore(x => x with
            {
                ConfigureMediator = c => c.RegisterServicesFromAssemblyContaining<BlaztoreComponentBaseTests>()
            });

        [Fact]
        public void Rendering_a_component_not_using_state_allows_command_dispatch()
        {
            _ = RenderComponent<SomeComponent>();

            GetState<SomeState>()
                .Should()
                .Be(new SomeState(true));
        }

        [Fact]
        public void Rendering_a_component_using_state_allows_command_dispatch()
        {
            _ = RenderComponent<SomeComponent>(builder => builder.Add(c => c.UseState, true));

            GetState<SomeState>()
                .Should()
                .Be(new SomeState(true));
        }

        private T? GetState<T>() where T : IState =>
            Services.GetRequiredService<IStore>()
                .GetState<T>();

        private class SomeComponent : BlaztoreComponentBase<SomeState>
        {
            [Parameter] public bool UseState { get; set; }

            protected override Task OnAfterInitialRenderAsync()
            {
                if (UseState)
                {
                    _ = State;
                }

                return Dispatch(new SomeState.Load());
            }
        }

        private record SomeState(bool IsLoaded) : IGlobalState
        {
            public static SomeState Initialize() => new(false);

            public record Load : IAction<SomeState>
            {
                internal record Reducer(IStore Store) : IPureReducer<SomeState, Load>
                {
                    public SomeState Reduce(SomeState state, Load action) =>
                        state with { IsLoaded = true };
                }
            }
        }
    }

    public class ScopedStateTests : TestContext
    {
        public ScopedStateTests() =>
            Services.AddBlaztore(x => x with
            {
                ConfigureMediator = c => c.RegisterServicesFromAssemblyContaining<BlaztoreComponentBaseTests>()
            });

        [Fact]
        public void Rendering_a_component_not_using_state_allows_command_dispatch()
        {
            const int scope = 1;
            
            _ = RenderComponent<SomeComponent>();

            GetState<SomeState>(scope)
                .Should()
                .Be(new SomeState(true));
        }

        [Fact]
        public void Rendering_a_component_using_state_allows_command_dispatch()
        {
            const int scope = 1;
            
            _ = RenderComponent<SomeComponent>(builder => builder.Add(c => c.UseState, true));

            GetState<SomeState>(scope)
                .Should()
                .Be(new SomeState(true));
        }

        private T? GetState<T>(object? scope) where T : IState =>
            Services.GetRequiredService<IStore>()
                .GetState<T>(scope);

        private class SomeComponent : BlaztoreComponentBaseWithScopedState<SomeState, int>
        {
            [Parameter] public bool UseState { get; set; }

            protected override Task OnAfterInitialRenderAsync()
            {
                if (UseState)
                {
                    _ = State;
                }

                return Dispatch(new SomeState.Load(1));
            }

            protected override int Scope => 1;
        }

        private record SomeState(bool IsLoaded) : IScopedState<int>
        {
            public static SomeState Initialize() => new(false);

            public record Load(int Scope) : IScopedAction<SomeState, int>
            {
                internal record Reducer(IStore Store) : IPureReducer<SomeState, Load>
                {
                    public SomeState Reduce(SomeState state, Load action) =>
                        state with { IsLoaded = true };
                }
            }
        }
    }

    public class ComponentStateTests : TestContext
    {
        public ComponentStateTests() =>
            Services.AddBlaztore(x => x with
            {
                ConfigureMediator = c => c.RegisterServicesFromAssemblyContaining<BlaztoreComponentBaseTests>()
            });

        [Fact]
        public void Rendering_a_component_not_using_state_allows_command_dispatch()
        {
            var component = RenderComponent<SomeComponent>();

            GetState<SomeState>(component.Instance.Id)
                .Should()
                .Be(new SomeState(true));
        }

        [Fact]
        public void Rendering_a_component_using_state_allows_command_dispatch()
        {
            var component = RenderComponent<SomeComponent>(builder => builder.Add(c => c.UseState, true));

            GetState<SomeState>(component.Instance.Id)
                .Should()
                .Be(new SomeState(true));
        }

        private T? GetState<T>(object? scope) where T : IState =>
            Services.GetRequiredService<IStore>()
                .GetState<T>(scope);

        private class SomeComponent : BlaztoreComponentBaseWithComponentState<SomeState>
        {
            [Parameter] public bool UseState { get; set; }

            protected override Task OnAfterInitialRenderAsync()
            {
                if (UseState)
                {
                    _ = State;
                }

                return Dispatch(new SomeState.Load(Id));
            }
        }

        private record SomeState(bool IsLoaded) : IComponentState
        {
            public static SomeState Initialize() => new(false);

            public record Load(ComponentId ComponentId) : IComponentAction<SomeState>
            {
                internal record Reducer(IStore Store) : IPureReducer<SomeState, Load>
                {
                    public SomeState Reduce(SomeState state, Load action) =>
                        state with { IsLoaded = true };
                }
            }
        }
    }
}