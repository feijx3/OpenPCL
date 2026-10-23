using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Runtime.Versioning
{
    [NullableContext(2)]
    [Nullable(0)]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
    [ExcludeFromCodeCoverage]
    internal sealed class RequiresPreviewFeaturesAttribute : Attribute
    {
        public RequiresPreviewFeaturesAttribute()
        {
        }

        public RequiresPreviewFeaturesAttribute(string message)
        {
            this.Message = message;
        }

        public string Message { get; }

        public string Url { get; set; }
    }
}
