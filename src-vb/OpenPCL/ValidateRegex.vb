Imports System
Imports System.Text.RegularExpressions
Imports MeloongCore.Extensions

Namespace PCL
    Public Class ValidateRegex
        Inherits Validate

        Public Property Regex As String

        Public Property ErrorDescription As String

        Public Sub New()
            Me.ErrorDescription = "正则检查失败！"
        End Sub

        Public Sub New(Regex As String, Optional ErrorDescription As String = "正则检查失败！")
            Me.ErrorDescription = "正则检查失败！"
            Me.Regex = Regex
            Me.ErrorDescription = ErrorDescription
        End Sub

        Public Overrides Function Validate(Str As String) As String
            Dim text As String
            If Not Str.RegexCheck(Me.Regex, RegexOptions.None) Then
                text = Me.ErrorDescription
            Else
                text = ""
            End If
            Return text
        End Function
    End Class
End Namespace
