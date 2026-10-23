Imports System
Imports Microsoft.VisualBasic

Namespace PCL
    Public Class ValidateNullable
        Inherits Validate

        Public Overrides Function Validate(Str As String) As String
            Dim text As String
            If Information.IsNothing(Str) OrElse String.IsNullOrEmpty(Str) Then
                text = Nothing
            Else
                text = ""
            End If
            Return text
        End Function
    End Class
End Namespace
