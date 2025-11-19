using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RecurPixel.EasyMessages.Helpers;

public static class TypeExtensions
{
    // Helper method to detect anonymous types
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
