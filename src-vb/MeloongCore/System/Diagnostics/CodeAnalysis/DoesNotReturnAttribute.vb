Imports System

Namespace System.Diagnostics.CodeAnalysis
    <AttributeUsage(AttributeTargets.Method, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class DoesNotReturnAttribute
        Inherits Attribute

    End Class
End Namespace
