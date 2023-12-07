using Blaztore.States;

namespace Blaztore.Gateways;

internal static class TypeExtensions
{
    public static bool Implements(this Type type, Type interfaceType) => 
        type.GetInterfaces().Contains(interfaceType);
    public static bool IsPersistentState(this Type type) => 
        type.Implements(typeof(IPersistentLifecycleState));
    public static bool IsGlobalState(this Type type) => 
        type.Implements(typeof(IGlobalState));
    public static bool IsScopedState(this Type type) => 
        type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IScopedState<>));
    public static bool IsComponentState(this Type type) => 
        type.Implements(typeof(IComponentState));
    public static bool IsInstanciableFromActionExecution(this Type type) => 
        type.Implements(typeof(IStateInstanciableFromActionExecution));
}