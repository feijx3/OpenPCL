using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    [ExcludeFromCodeCoverage]
    internal sealed class ParamCollectionAttribute : Attribute
    {
    }
}
