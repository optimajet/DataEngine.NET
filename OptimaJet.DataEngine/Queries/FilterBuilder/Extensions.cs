using System.Reflection;

namespace OptimaJet.DataEngine.Queries.FilterBuilder;

internal static class Extensions
{
    public static string GetFullName(this MethodInfo method) => $"{method.DeclaringType}.{method.Name}";
}