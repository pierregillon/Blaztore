using System.Collections;
using Microsoft.AspNetCore.Components;

namespace Blaztore.Components;

public class ExtendedComponentBase : ComponentBase
{
    protected virtual Task OnAfterInitialRenderAsync() => Task.CompletedTask;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            IsRendered = true;
            await OnAfterInitialRenderAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public bool IsRendered { get; private set; }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        var newValues = parameters.ToDictionary();
        var initialValues = GetInitialParameterValues(newValues.Keys.ToList());

        await base.SetParametersAsync(parameters);

        var changedParameters =
            (from element in initialValues.OrderBy(x => x.Key).Zip(newValues.OrderBy(x => x.Key))
                where !Equals(element.First.Value, element.Second.Value)
                select new ChangedParameter(element.First.Key, element.First.Value, element.Second.Value)
            ).ToList();

        if (changedParameters.Any())
        {
            await OnParametersChangedAsync(new ChangedParameters(changedParameters));
        }
    }

    private Dictionary<string, object?> GetInitialParameterValues(IReadOnlyCollection<string> keys) =>
        GetType()
            .GetParameterProperties()
            .Where(x => keys.Contains(x.Name))
            .ToDictionary(x => x.Name, x => x.GetValue(this));


    protected virtual async Task OnParametersChangedAsync(ChangedParameters parameters)
    {
        if (IsRendered)
        {
            await OnParametersChangedAfterComponentRendered(parameters);
        }
    }

    protected virtual Task OnParametersChangedAfterComponentRendered(ChangedParameters parameters) =>
        Task.CompletedTask;
}

public record ChangedParameter(string ParameterName, object? PreviousValue, object NewValue);

public record ChangedParameters(IReadOnlyCollection<ChangedParameter> Values) : IEnumerable<ChangedParameter>
{
    private IReadOnlyCollection<ChangedParameter> Values { get; } = Values;

    public IEnumerator<ChangedParameter> GetEnumerator() =>
        Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public bool HasChanged(string name) =>
        Values.Any(x => x.ParameterName == name);
}