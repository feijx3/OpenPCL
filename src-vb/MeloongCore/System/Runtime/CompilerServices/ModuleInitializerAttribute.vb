Imports System
Imports System.Diagnostics.CodeAnalysis

Namespace System.Runtime.CompilerServices
    <AttributeUsage(AttributeTargets.Method, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class ModuleInitializerAttribute
        Inherits Attribute

    End Class
End Namespace
