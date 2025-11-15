using System;
using System.Reflection;

namespace PTD.Core.Interfaces
{
    public interface ISanitizedType
    {
        /// <summary>
        /// Returns the objects true type
        /// </summary>
        MemberInfo? Type { get; }
        /// <summary>
        /// Returns the underlying DataTypes name
        /// </summary>
        string? TypeName { get; }
        /// <summary>
        /// Returns the Name of the primitive or complex type
        /// </summary>
        string? Name { get; }
        /// <summary>
        /// Returns a string representation of access modifiers
        /// </summary>
        string? AccessModifiers { get; }

        string ToString(); // Override ToString for simple output
    }
}
