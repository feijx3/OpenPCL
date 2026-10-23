using System;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.CodeAnalysis
{
    [NullableContext(1)]
    [Nullable(0)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    [ExcludeFromCodeCoverage]
    internal sealed class MemberNotNullAttribute : Attribute
    {
        public MemberNotNullAttribute(string member)
        {
            this.Members = new string[] { member };
        }

        public MemberNotNullAttribute(params string[] members)
        {
            this.Members = members;
        }

        public string[] Members { get; }
    }
}
