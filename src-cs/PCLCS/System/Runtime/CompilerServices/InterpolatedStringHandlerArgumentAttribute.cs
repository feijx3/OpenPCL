using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
    [NullableContext(1)]
    [Nullable(0)]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    [ExcludeFromCodeCoverage]
    internal sealed class InterpolatedStringHandlerArgumentAttribute : Attribute
    {
        public InterpolatedStringHandlerArgumentAttribute(string argument)
        {
            this.Arguments = new string[] { argument };
        }

        public InterpolatedStringHandlerArgumentAttribute(params string[] arguments)
        {
            this.Arguments = arguments;
        }

        public string[] Arguments { get; }
    }
}
