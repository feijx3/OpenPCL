using System;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.CodeAnalysis
{
    [NullableContext(2)]
    [Nullable(0)]
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    [ExcludeFromCodeCoverage]
    internal sealed class ConstantExpectedAttribute : Attribute
    {
        public object Min { get; set; }

        public object Max { get; set; }
    }
}
