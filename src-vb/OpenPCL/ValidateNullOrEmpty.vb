Imports System
Imports Microsoft.VisualBasic

Namespace PCL
    Public Class ValidateNullOrEmpty
        Inherits Validate

        Public Overrides Function Validate(Str As String) As String
            Dim text As String
            If Information.IsNothing(Str) OrElse String.IsNullOrEmpty(Str) Then
                text = "输入内容不能为空！"
            Else
                text = ""
            End If
            Return text
        End Function
    End Class
End Namespace
