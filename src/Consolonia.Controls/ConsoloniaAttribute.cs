using System;
using System.Linq;
using System.Reflection;

namespace Consolonia.Controls
{
    /// <summary>
    /// Marks an entity as part of the Consolonia framework.
    /// Used for reflection-based identification.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ConsoloniaAttribute : Attribute
    {
        public static bool IsDefined(Type type)
        {
            return type.GetCustomAttributes<ConsoloniaAttribute>().Any();
        }
    }
}