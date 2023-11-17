using System.Collections.Concurrent;
using Blaztore.Gateways;
using Microsoft.AspNetCore.Components;

namespace Blaztore.Components;

public abstract class BlaztoreComponentBase : ExtendedComponentBase, IComponentBase
{
    private static readonly ConcurrentDictionary<string, int> InstanceCounts = new();

    protected BlaztoreComponentBase()
    {
        var name = GetType().Name;
        
        var count = InstanceCounts.AddOrUpdate(name, 1, (_, value) => value + 1);

        Id = ComponentId.FromNameAndNumber(name, count);
    }
    
    public ComponentId Id { get; }
    public void ReRender() => _ = InvokeAsync(StateHasChanged);
}

public abstract class BlaztoreComponentBase<TState> : BlaztoreComponentBase, IDisposable 
    where TState : IGlobalState
{
    [Inject] public IGlobalStateReduxGateway<TState> Gateway { get; set; } = default!;
    
    protected TState State => Gateway.SubscribeToState(this);
    protected Task Dispatch(IAction<TState> action) => Gateway.Dispatch(action);

    public void Dispose()
    {
        Gateway.UnsubscribeFromState(this);
        GC.SuppressFinalize(this);
    }
}

public abstract class BlaztoreComponentBaseWithComponentState<TState> : BlaztoreComponentBase, IDisposable 
    where TState : IComponentState
{
    [Inject] public IComponentStateReduxGateway<TState> Gateway { get; set; } = default!;
    
    protected TState State => Gateway.SubscribeToState(this);
    protected Task Dispatch(IComponentAction<TState> action) => Gateway.Dispatch(action);

    public void Dispose()
    {
        Gateway.UnsubscribeFromState(this);
        GC.SuppressFinalize(this);
    }
}

public abstract class BlaztoreComponentBaseWithScopedState<TState, TScope> : BlaztoreComponentBase, IDisposable 
    where TState : IScopedState<TScope>
{
    [Inject] public IScopedStateReduxGateway<TState, TScope> Gateway { get; set; } = default!;
    
    protected abstract TScope Scope { get; }
    
    protected TState State => Gateway.SubscribeToState(this, Scope);
    protected Task Dispatch(IScopedAction<TState, TScope> action) => Gateway.Dispatch(action);

    public virtual void Dispose()
    {
        Gateway.UnsubscribeFromState(this, Scope);
        GC.SuppressFinalize(this);
    }
}