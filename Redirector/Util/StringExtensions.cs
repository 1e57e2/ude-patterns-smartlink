using System.Globalization;
using System.Reflection;

namespace Redirector;

public static class StringExtensions
{
    public static bool TryParse(this string input, Type targetType, out object? result)
    {
        result = null;

        if (targetType == typeof(string))
        {
            result = input;
            return true;
        }

        var tryParseMethodWithCulture = targetType.GetMethod(
            "TryParse",
            BindingFlags.Public | BindingFlags.Static,
            null,
            [typeof(string), typeof(IFormatProvider), targetType.MakeByRefType()],
            null);

        if (tryParseMethodWithCulture is not null)
        {
            object?[] parameters = { input, CultureInfo.InvariantCulture, null };
            var success = (bool)tryParseMethodWithCulture.Invoke(null, parameters)!;

            if (!success) return false;
            result = parameters[2];
            return true;
        }

        var tryParseMethod = targetType.GetMethod(
            "TryParse",
            BindingFlags.Public | BindingFlags.Static,
            null,
            [typeof(string), targetType.MakeByRefType()],
            null);

        if (tryParseMethod is not null)
        {
            object?[] parameters = { input, null };
            var success = (bool)tryParseMethod.Invoke(null, parameters)!;

            if (!success) return false;
            result = parameters[1];
            return true;
        }
        
        return false;
    }
}