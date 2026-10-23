Imports System
Imports System.Runtime.CompilerServices

Namespace System.Diagnostics.CodeAnalysis
    <NullableContext(1)>
    <Nullable(0)>
    <AttributeUsage(AttributeTargets.Method Or AttributeTargets.[Property], Inherited := False, AllowMultiple := True)>
    <ExcludeFromCodeCoverage()>
    Friend NotInheritable Class MemberNotNullAttribute
        Inherits Attribute

        Public Sub New(member As String)
            Me.Members = New String() { member }
        End Sub

        Public Sub New(ParamArray members As String())
            Me.Members = members
        End Sub

        Public ReadOnly Property Members As String()
    End Class
End Namespace
