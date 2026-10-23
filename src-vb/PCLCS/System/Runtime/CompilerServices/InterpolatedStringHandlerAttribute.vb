Imports System
Imports System.Diagnostics.CodeAnalysis

Namespace System.Runtime.CompilerServices
    <AttributeUsage(AttributeTargets.[Class] Or AttributeTargets.Struct, AllowMultiple := False, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class InterpolatedStringHandlerAttribute
        Inherits Attribute

    End Class
End Namespace
