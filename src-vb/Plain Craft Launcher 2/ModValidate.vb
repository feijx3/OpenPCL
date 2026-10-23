Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public NotInheritable Module ModValidate
        Public Function Validate(Text As String, ValidateRules As IEnumerable(Of Validate)) As String
            Dim text As String = ""
            Try
                For Each validate As Validate In ValidateRules
                    text = validate.Validate(Text)
                    If text Is Nothing Then
                        Return ""
                    End If
                    If Operators.CompareString(text, "", False) <> 0 Then
                        Return text
                    End If
                Next
            Finally
                Dim enumerator As IEnumerator(Of Validate)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            Return text
        End Function
    End Module
End Namespace
