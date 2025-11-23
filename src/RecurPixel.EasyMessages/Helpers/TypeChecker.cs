using System.Runtime.CompilerServices;

namespace RecurPixel.EasyMessages.Helpers;

/// <summary>
/// Type Extensions
/// </summary>
public static class TypeExtensions
{
    // Helper method to detect anonymous types

    /// <summary>
    /// Determines whether the specified type is an anonymous type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>boolean</returns>
    public static bool IsAnonymousType(this Type type)
    {
        if (type == null)
            return false;
        if (!type.IsClass)
            return false;

        // Check for CompilerGeneratedAttribute
        if (!type.IsDefined(typeof(CompilerGeneratedAttribute), false))
            return false;

        // Check for the name pattern "<>f__AnonymousType..."
        return type.Name.Contains("AnonymousType");
    }
}
