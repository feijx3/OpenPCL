using System;

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    [ExcludeFromCodeCoverage]
    internal sealed class NotNullWhenAttribute : Attribute
    {
        public NotNullWhenAttribute(bool returnValue)
        {
            this.ReturnValue = returnValue;
        }

        public bool ReturnValue { get; }
    }
}
