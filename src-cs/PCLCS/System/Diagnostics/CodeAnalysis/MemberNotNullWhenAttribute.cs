using System;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.CodeAnalysis
{
    [NullableContext(1)]
    [Nullable(0)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    [ExcludeFromCodeCoverage]
    internal sealed class MemberNotNullWhenAttribute : Attribute
    {
        public MemberNotNullWhenAttribute(bool returnValue, string member)
        {
            this.ReturnValue = returnValue;
            this.Members = new string[] { member };
        }

        public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
        {
            this.ReturnValue = returnValue;
            this.Members = members;
        }

        public bool ReturnValue { get; }

        public string[] Members { get; }
    }
}
