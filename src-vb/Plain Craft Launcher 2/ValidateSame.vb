Imports System

Namespace PCL
    Public Class ValidateSame
        Inherits Validate

        Public Property Required As String

        Public Property ErrorMessage As String

        Public Property IgnoreCase As Boolean

        Public Sub New()
            Me.IgnoreCase = False
        End Sub

        Public Sub New(Required As String, Optional ErrorMessage As String = "输入内容有误", Optional IgnoreCase As Boolean = False)
            Me.IgnoreCase = False
            Me.Required = Required
            Me.ErrorMessage = ErrorMessage
            Me.IgnoreCase = IgnoreCase
        End Sub

        Public Overrides Function Validate(Str As String) As String
            If String.Compare(Str, Me.Required, Me.IgnoreCase) <> 0 Then
                Return Me.ErrorMessage
            End If
            Return ""
        End Function
    End Class
End Namespace
