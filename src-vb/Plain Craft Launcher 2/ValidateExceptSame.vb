Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class ValidateExceptSame
        Inherits Validate

        Public Property Excepts As Collection(Of String)

        Public Property ErrorMessage As String

        Public Property IgnoreCase As Boolean

        Public Sub New()
            Me.Excepts = New Collection(Of String)()
            Me.IgnoreCase = False
        End Sub

        Public Sub New(Excepts As Collection(Of String), Optional ErrorMessage As String = "输入内容不能为 %", Optional IgnoreCase As Boolean = False)
            Me.Excepts = New Collection(Of String)()
            Me.IgnoreCase = False
            Me.Excepts = Excepts
            Me.ErrorMessage = ErrorMessage
            Me.IgnoreCase = IgnoreCase
        End Sub

        Public Sub New(Excepts As IEnumerable(Of String), Optional ErrorMessage As String = "输入内容不能为 %", Optional IgnoreCase As Boolean = False)
            Me.New(New Collection(Of String)(New List(Of String)(Excepts)), ErrorMessage, IgnoreCase)
        End Sub

        Public Sub New(Except As String, Optional ErrorMessage As String = "输入内容不能为 %", Optional IgnoreCase As Boolean = False)
            Me.New(New Collection(Of String)() From { Except }, ErrorMessage, IgnoreCase)
        End Sub

        Public Overrides Function Validate(Str As String) As String
            Dim text As String
            If Str Is Nothing Then
                text = Me.ErrorMessage.Replace("%", "null")
            Else
                Try
                    For Each text2 As String In Me.Excepts
                        If Me.IgnoreCase Then
                            If Operators.CompareString(Str.Lower(), text2.Lower(), False) = 0 Then
                                Return Me.ErrorMessage.Replace("%", text2)
                            End If
                        ElseIf Str.Equals(text2) Then
                            Return Me.ErrorMessage.Replace("%", text2)
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of String)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                text = ""
            End If
            Return text
        End Function
    End Class
End Namespace
