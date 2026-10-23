Imports System
Imports System.Diagnostics.CodeAnalysis

Namespace System.Runtime.CompilerServices
    <NullableContext(1)>
    <Nullable(0)>
    <AttributeUsage(AttributeTargets.Parameter, AllowMultiple := False, Inherited := False)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class InterpolatedStringHandlerArgumentAttribute
        Inherits Attribute

        Public Sub New(argument As String)
            Me.Arguments = New String() { argument }
        End Sub

        Public Sub New(ParamArray arguments As String())
            Me.Arguments = arguments
        End Sub

        Public ReadOnly Property Arguments As String()
    End Class
End Namespace
