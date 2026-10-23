using System;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.CodeAnalysis
{
    [NullableContext(1)]
    [Nullable(0)]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
    [ExcludeFromCodeCoverage]
    internal sealed class ExperimentalAttribute : Attribute
    {
        public ExperimentalAttribute(string diagnosticId)
        {
            this.DiagnosticId = diagnosticId;
        }

        public string DiagnosticId { get; }

        [Nullable(2)]
        public string UrlFormat
        {
            [NullableContext(2)]
            get;
            [NullableContext(2)]
            set;
        }
    }
}
