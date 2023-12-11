using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace Blaztore.Components;

public static class ComponentTypeExtensions
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<PropertyInfo>> TypeToParameters = new();
    
    public static IEnumerable<PropertyInfo> GetParameterProperties(this Type componentType)
    {
        if (TypeToParameters.TryGetValue(componentType, out var parameters))
        {
            return parameters;
        }

        parameters = componentType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => x.GetCustomAttribute<ParameterAttribute>() != null)
            .ToArray();

        TypeToParameters.TryAdd(componentType, parameters);

        return parameters;
    }
}