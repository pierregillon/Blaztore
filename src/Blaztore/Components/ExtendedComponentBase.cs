using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace Blaztore.Components;

public class ExtendedComponentBase : ComponentBase
{
    protected virtual Task OnAfterInitialRenderAsync() => Task.CompletedTask;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await OnAfterInitialRenderAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        var initialValues = GetInitialParameterValues().OrderBy(x => x.Key);

        await base.SetParametersAsync(parameters);

        var newValues = parameters.ToDictionary().OrderBy(x => x.Key);

        List<ChangedParameters> changedParameters =
            (from element in initialValues.Zip(newValues)
                where !Equals(element.First.Value, element.Second.Value)
                select new ChangedParameters(element.First.Key, element.First.Value, element.Second.Value)
            ).ToList();

        if (changedParameters.Any())
        {
            await OnParametersChangedAsync(changedParameters);
        }
    }

    private Dictionary<string, object?> GetInitialParameterValues() =>
        GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => x.GetCustomAttribute<ParameterAttribute>() != null)
            .ToDictionary(x => x.Name, x => x.GetValue(this));

    protected virtual Task OnParametersChangedAsync(IReadOnlyCollection<ChangedParameters> parameters) =>
        Task.CompletedTask;
}

public record ChangedParameters(string ParameterName, object? PreviousValue, object NewValue);