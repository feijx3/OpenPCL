using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
    [NullableContext(1)]
    [Nullable(0)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
    [ExcludeFromCodeCoverage]
    internal sealed class CollectionBuilderAttribute : Attribute
    {
        public CollectionBuilderAttribute(Type builderType, string methodName)
        {
            this.BuilderType = builderType;
            this.MethodName = methodName;
        }

        public Type BuilderType { get; }

        public string MethodName { get; }
    }
}
