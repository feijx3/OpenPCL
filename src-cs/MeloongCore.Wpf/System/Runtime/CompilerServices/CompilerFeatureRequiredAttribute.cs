using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
    [NullableContext(1)]
    [Nullable(0)]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    [ExcludeFromCodeCoverage]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName)
        {
            this.FeatureName = featureName;
        }

        public string FeatureName { get; }

        public bool IsOptional { get; set; }

        public const string RefStructs = "RefStructs";

        public const string RequiredMembers = "RequiredMembers";
    }
}
