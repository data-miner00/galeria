using System;
using System.Linq;
using System.Reflection;

public static class ObjectExtensions
{
    public static bool HasAnyNonNullProperty(this object? myObject)
    {
        if (myObject == null)
        {
            return false; // An object that is null cannot have non-null properties
        }

        // Use LINQ Any() to check if at least one property value is not null
        return myObject.GetType()
                       .GetProperties() // Gets all public properties
                       .Any(pi => pi.GetValue(myObject) != null); // Checks if any property's value is non-null
    }
}
