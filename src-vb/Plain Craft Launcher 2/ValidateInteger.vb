Imports System
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class ValidateInteger
        Inherits Validate

        Public Property Min As Integer

        Public Property Max As Integer

        Public Sub New()
            Me.Max = Integer.MaxValue
        End Sub

        Public Sub New(Min As Integer, Max As Integer)
            Me.Max = Integer.MaxValue
            Me.Min = Min
            Me.Max = Max
        End Sub

        Public Overrides Function Validate(Str As String) As String
            Dim text As String
            Dim num As Integer
            If Str.Length > 9 Then
                text = "请输入一个大小合理的数字！"
            ElseIf Not Integer.TryParse(Str, num) Then
                text = "请输入一个整数！"
            ElseIf num > Me.Max Then
                text = "不可超过 " + Conversions.ToString(Me.Max) + "！"
            ElseIf num < Me.Min Then
                text = "不可低于 " + Conversions.ToString(Me.Min) + "！"
            Else
                text = ""
            End If
            Return text
        End Function
    End Class
End Namespace
