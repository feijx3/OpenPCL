using System;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.CodeAnalysis
{
    [NullableContext(1)]
    [Nullable(0)]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    [ExcludeFromCodeCoverage]
    internal sealed class StringSyntaxAttribute : Attribute
    {
        public StringSyntaxAttribute(string syntax)
        {
            this.Syntax = syntax;
            this.Arguments = new object[0];
        }

        public StringSyntaxAttribute(string syntax, [Nullable(new byte[] { 1, 2 })] params object[] arguments)
        {
            this.Syntax = syntax;
            this.Arguments = arguments;
        }

        public string Syntax { get; }

        [Nullable(new byte[] { 1, 2 })]
        public object[] Arguments
        {
            [return: Nullable(new byte[] { 1, 2 })]
            get;
        }

        public const string CompositeFormat = "CompositeFormat";

        public const string DateOnlyFormat = "DateOnlyFormat";

        public const string DateTimeFormat = "DateTimeFormat";

        public const string EnumFormat = "EnumFormat";

        public const string GuidFormat = "GuidFormat";

        public const string Json = "Json";

        public const string NumericFormat = "NumericFormat";

        public const string Regex = "Regex";

        public const string TimeOnlyFormat = "TimeOnlyFormat";

        public const string TimeSpanFormat = "TimeSpanFormat";

        public const string Uri = "Uri";

        public const string Xml = "Xml";
    }
}
