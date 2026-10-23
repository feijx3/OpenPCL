Imports System
Imports System.Runtime.CompilerServices

Namespace System.Diagnostics.CodeAnalysis
    <NullableContext(1)>
    <Nullable(0)>
    <AttributeUsage(AttributeTargets.[Property] Or AttributeTargets.Parameter Or AttributeTargets.ReturnValue, AllowMultiple := True, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class NotNullIfNotNullAttribute
        Inherits Attribute

        Public Sub New(parameterName As String)
            Me.ParameterName = parameterName
        End Sub

        Public ReadOnly Property ParameterName As String
    End Class
End Namespace
