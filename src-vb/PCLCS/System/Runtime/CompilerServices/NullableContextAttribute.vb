Imports System
Imports Microsoft.CodeAnalysis

Namespace System.Runtime.CompilerServices
    <CompilerGenerated()>
    <Embedded()>
    <AttributeUsage(AttributeTargets.[Class] Or AttributeTargets.Struct Or AttributeTargets.Method Or AttributeTargets.[Interface] Or AttributeTargets.[Delegate], AllowMultiple := False, Inherited := False)>
    Friend NotInheritable Class NullableContextAttribute
        Inherits Attribute

        Public Sub New(A_1 As Byte)
            Me.Flag = A_1
        End Sub

        Public Flag As Byte
    End Class
End Namespace
