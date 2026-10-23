Imports System

Namespace System.Diagnostics.CodeAnalysis
    <AttributeUsage(AttributeTargets.Parameter, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class NotNullWhenAttribute
        Inherits Attribute

        Public Sub New(returnValue As Boolean)
            Me.ReturnValue = returnValue
        End Sub

        Public ReadOnly Property ReturnValue As Boolean
    End Class
End Namespace
