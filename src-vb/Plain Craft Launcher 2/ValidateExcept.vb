Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Linq
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic

Namespace PCL
    Public Class ValidateExcept
        Inherits Validate

        Public Property Excepts As Collection(Of String)

        Public Property ErrorMessage As String

        Public Sub New()
            Me.Excepts = New Collection(Of String)()
            Me.ErrorMessage = "输入内容不能包含 %"
        End Sub

        Public Sub New(Excepts As Collection(Of String), Optional ErrorMessage As String = "输入内容不能包含 %")
            Me.Excepts = New Collection(Of String)()
            Me.Excepts = Excepts
            Me.ErrorMessage = ErrorMessage
        End Sub

        Public Sub New(Excepts As IEnumerable(Of String), Optional ErrorMessage As String = "输入内容不能包含 %")
            Me.New(New Collection(Of String)(New List(Of String)(Excepts)), ErrorMessage)
        End Sub

        Public Sub New(Excepts As IEnumerable(Of Char), Optional ErrorMessage As String = "输入内容不能包含 %")
            Dim func As Func(Of Char, String)
            If ValidateExcept._Closure$__.$I11-0 IsNot Nothing Then
                func = ValidateExcept._Closure$__.$I11-0
            Else
                Dim func2 As Func(Of Char, String) = Function(c As Char) c.ToString()
                func = func2
                ValidateExcept._Closure$__.$I11-0 = func2
            End If
            Me..ctor(New Collection(Of String)(Excepts.[Select](func).ToList()), ErrorMessage)
        End Sub

        Public Overrides Function Validate(Str As String) As String
            Try
                For Each text As String In Me.Excepts
                    If Str.IndexOfF(text, True) >= 0 Then
                        If Information.IsNothing(Me.ErrorMessage) Then
                            Me.ErrorMessage = ""
                        End If
                        Return Me.ErrorMessage.Replace("%", text)
                    End If
                Next
            Finally
                Dim enumerator As IEnumerator(Of String)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            Return ""
        End Function
    End Class
End Namespace
