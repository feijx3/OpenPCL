Imports System
Imports Microsoft.VisualBasic

Namespace PCL
    Public Class ValidateLength
        Inherits Validate

        Public Property Min As Integer

        Public Property Max As Integer

        Public Sub New()
            Me.Min = 0
            Me.Max = Integer.MaxValue
        End Sub

        Public Sub New(Min As Integer, Optional Max As Integer = 2147483647)
            Me.Min = 0
            Me.Max = Integer.MaxValue
            Me.Min = Min
            Me.Max = Max
        End Sub

        Public Overrides Function Validate(Str As String) As String
            Dim text As String
            If Strings.Len(Str) <> Me.Max AndAlso Me.Max = Me.Min Then
                text = String.Format("长度必须为 {0} 个字符！", Me.Max)
            ElseIf Strings.Len(Str) > Me.Max Then
                text = String.Format("长度最长为 {0} 个字符！", Me.Max)
            ElseIf Strings.Len(Str) < Me.Min Then
                text = String.Format("长度至少需 {0} 个字符！", Me.Min)
            Else
                text = ""
            End If
            Return text
        End Function
    End Class
End Namespace
