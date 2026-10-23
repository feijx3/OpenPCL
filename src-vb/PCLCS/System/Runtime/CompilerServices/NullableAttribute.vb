Imports System
Imports Microsoft.CodeAnalysis

Namespace System.Runtime.CompilerServices
    <CompilerGenerated()>
    <Embedded()>
    <AttributeUsage(AttributeTargets.[Class] Or AttributeTargets.[Property] Or AttributeTargets.Field Or AttributeTargets.[Event] Or AttributeTargets.Parameter Or AttributeTargets.ReturnValue Or AttributeTargets.GenericParameter, AllowMultiple := False, Inherited := False)>
    Friend NotInheritable Class NullableAttribute
        Inherits Attribute

        Public Sub New(A_1 As Byte)
            Me.NullableFlags = New Byte() { A_1 }
        End Sub

        Public Sub New(A_1 As Byte())
            Me.NullableFlags = A_1
        End Sub

        Public NullableFlags As Byte()
    End Class
End Namespace
