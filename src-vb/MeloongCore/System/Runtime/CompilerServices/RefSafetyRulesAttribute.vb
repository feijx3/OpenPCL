Imports System
Imports Microsoft.CodeAnalysis

Namespace System.Runtime.CompilerServices
    <CompilerGenerated()>
    <Embedded()>
    <AttributeUsage(AttributeTargets.[Module], AllowMultiple := False, Inherited := False)>
    Friend NotInheritable Class RefSafetyRulesAttribute
        Inherits Attribute

        Public Sub New(A_1 As Integer)
            Me.Version = A_1
        End Sub

        Public Version As Integer
    End Class
End Namespace
