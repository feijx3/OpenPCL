Imports System

Namespace System.Diagnostics.CodeAnalysis
    <AttributeUsage(AttributeTargets.Parameter, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class DoesNotReturnIfAttribute
        Inherits Attribute

        Public Sub New(parameterValue As Boolean)
            Me.ParameterValue = parameterValue
        End Sub

        Public ReadOnly Property ParameterValue As Boolean
    End Class
End Namespace
