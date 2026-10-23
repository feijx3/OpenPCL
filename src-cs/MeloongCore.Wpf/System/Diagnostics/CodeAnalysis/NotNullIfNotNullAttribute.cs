using System;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.CodeAnalysis
{
    [NullableContext(1)]
    [Nullable(0)]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
    [ExcludeFromCodeCoverage]
    internal sealed class NotNullIfNotNullAttribute : Attribute
    {
        public NotNullIfNotNullAttribute(string parameterName)
        {
            this.ParameterName = parameterName;
        }

        public string ParameterName { get; }
    }
}
