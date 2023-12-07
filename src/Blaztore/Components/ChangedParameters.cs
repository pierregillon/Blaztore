using System.Collections;

namespace Blaztore.Components;

public record ChangedParameters(IReadOnlyCollection<ChangedParameter> Values) : IEnumerable<ChangedParameter>
{
    private IReadOnlyCollection<ChangedParameter> Values { get; } = Values;

    public IEnumerator<ChangedParameter> GetEnumerator() =>
        Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public bool HasChanged(string name) =>
        Values.Any(x => x.ParameterName == name);

    public static ChangedParameters Build(
        IReadOnlyDictionary<string, object?> currentValues,
        IReadOnlyDictionary<string, object> newValues
    )
    {
        var changedParameters =
            (from element in currentValues.OrderBy(x => x.Key).Zip(newValues.OrderBy(x => x.Key))
                where !Equals(element.First.Value, element.Second.Value)
                select new ChangedParameter(element.First.Key, element.First.Value, element.Second.Value)
            ).ToList();
        
        return new(changedParameters);
    }
}