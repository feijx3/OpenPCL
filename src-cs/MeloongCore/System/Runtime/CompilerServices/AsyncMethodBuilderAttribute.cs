using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
    [NullableContext(1)]
    [Nullable(0)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
    [ExcludeFromCodeCoverage]
    internal sealed class AsyncMethodBuilderAttribute : Attribute
    {
        public AsyncMethodBuilderAttribute(Type builderType)
        {
            this.BuilderType = builderType;
        }

        public Type BuilderType { get; }
    }
}
