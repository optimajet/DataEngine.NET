using System.Reflection;

namespace OptimaJet.DataEngine.Filters;

public static class Extensions
{
    public static string GetFullName(this MethodInfo method) => $"{method.DeclaringType}.{method.Name}";
}