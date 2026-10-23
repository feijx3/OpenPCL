Imports System
Imports System.Runtime.CompilerServices

Namespace System.Diagnostics.CodeAnalysis
    <NullableContext(1)>
    <Nullable(0)>
    <AttributeUsage(AttributeTargets.Method Or AttributeTargets.[Property], Inherited := False, AllowMultiple := True)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class MemberNotNullWhenAttribute
        Inherits Attribute

        Public Sub New(returnValue As Boolean, member As String)
            Me.ReturnValue = returnValue
            Me.Members = New String() { member }
        End Sub

        Public Sub New(returnValue As Boolean, ParamArray members As String())
            Me.ReturnValue = returnValue
            Me.Members = members
        End Sub

        Public ReadOnly Property ReturnValue As Boolean

        Public ReadOnly Property Members As String()
    End Class
End Namespace
