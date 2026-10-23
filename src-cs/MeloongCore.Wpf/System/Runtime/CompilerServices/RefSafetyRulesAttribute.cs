using System;
using Microsoft.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
    [CompilerGenerated]
    [Embedded]
    [AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
    internal sealed class RefSafetyRulesAttribute : Attribute
    {
        public RefSafetyRulesAttribute(int A_1)
        {
            this.Version = A_1;
        }

        public readonly int Version;
    }
}
