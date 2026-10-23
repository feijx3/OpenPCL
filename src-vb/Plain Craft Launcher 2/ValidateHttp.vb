Imports System
Imports System.Text.RegularExpressions
Imports MeloongCore.Extensions

Namespace PCL
    Public Class ValidateHttp
        Inherits Validate

        Public Overrides Function Validate(Str As String) As String
            If Str.EndsWithF("/", False) Then
                ' The following expression was wrapped in a checked-expression
                Str = Str.Substring(0, Str.Length - 1)
            End If
            Dim text As String
            If Not Str.RegexCheck("^(http[s]?)\://", RegexOptions.None) Then
                text = "输入的网址无效！"
            Else
                text = ""
            End If
            Return text
        End Function
    End Class
End Namespace
