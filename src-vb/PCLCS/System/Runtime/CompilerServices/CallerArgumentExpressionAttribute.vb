Imports System
Imports System.Diagnostics.CodeAnalysis

Namespace System.Runtime.CompilerServices
    <NullableContext(1)>
    <Nullable(0)>
    <AttributeUsage(AttributeTargets.Parameter, AllowMultiple := False, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class CallerArgumentExpressionAttribute
        Inherits Attribute

        Public Sub New(parameterName As String)
            Me.ParameterName = parameterName
        End Sub

        Public ReadOnly Property ParameterName As String
    End Class
End Namespace
