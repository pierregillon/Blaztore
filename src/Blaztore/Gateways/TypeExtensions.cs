using Blaztore.States;

namespace Blaztore.Gateways;

public static class TypeExtensions
{
    public static bool Implements(this Type type, Type interfaceType) => type.GetInterfaces().Contains(interfaceType);
    public static bool IsPersistentState(this Type type) => type.Implements(typeof(IPersistentLifecycleState));
    public static bool IsComponentState(this Type type) => type.Implements(typeof(IComponentState));
}