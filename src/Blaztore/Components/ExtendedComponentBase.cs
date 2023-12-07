using Microsoft.AspNetCore.Components;

namespace Blaztore.Components;

public class ExtendedComponentBase : ComponentBase
{
    private bool HasBeenRendered { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            HasBeenRendered = true;
            await OnAfterInitialRenderAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        var newValues = parameters.ToDictionary();
        var currentValues = GetInitialParameterValues(newValues.Keys.ToList());

        await base.SetParametersAsync(parameters);

        var changedParameters = ChangedParameters.Build(currentValues, newValues);
        if (changedParameters.Any())
        {
            await RaiseOnParametersChangedAsync(changedParameters);
        }
    }

    private Dictionary<string, object?> GetInitialParameterValues(IReadOnlyCollection<string> keys) =>
        GetType()
            .GetParameterProperties()
            .Where(x => keys.Contains(x.Name))
            .ToDictionary(x => x.Name, x => x.GetValue(this));

    private async Task RaiseOnParametersChangedAsync(ChangedParameters changedParameters)
    {
        await OnParametersChangedAsync(changedParameters);
        
        if (HasBeenRendered)
        {
            await OnParametersChangedAfterComponentRendered(changedParameters);
        }
    }
    
    protected virtual Task OnAfterInitialRenderAsync() => 
        Task.CompletedTask;

    protected virtual Task OnParametersChangedAsync(ChangedParameters parameters) => 
        Task.CompletedTask;

    protected virtual Task OnParametersChangedAfterComponentRendered(ChangedParameters parameters) =>
        Task.CompletedTask;
}