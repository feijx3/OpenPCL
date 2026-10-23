Imports System

Namespace PCL
    Public Class ValidateFunc
        Inherits Validate

        Public Sub New(F As Func(Of String, String))
            Me._func = F
        End Sub

        Public Overrides Function Validate(Str As String) As String
            Return Me._func(Str)
        End Function

        Private _func As Func(Of String, String)
    End Class
End Namespace
