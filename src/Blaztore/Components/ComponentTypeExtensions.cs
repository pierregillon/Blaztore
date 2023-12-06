using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace Blaztore.Components;

public static class ComponentTypeExtensions
{
    private static readonly IDictionary<Type, IReadOnlyCollection<PropertyInfo>> TypeToParameters =
        new ConcurrentDictionary<Type, IReadOnlyCollection<PropertyInfo>>();
    
    public static IEnumerable<PropertyInfo> GetParameterProperties(this Type componentType)
    {
        if (!TypeToParameters.TryGetValue(componentType, out var parameters))
        {
            parameters = componentType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<ParameterAttribute>() != null)
                .ToArray();

            TypeToParameters.TryAdd(componentType, parameters);
        }

        return parameters;
    }
}