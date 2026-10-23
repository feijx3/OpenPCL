Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports MeloongCore.Extensions

Namespace PCLCS
    <NullableContext(1)>
    <Nullable(0)>
    Public Module McUtils
        Public Function CompareVersion(left As String, right As String) As Integer
            If left = right Then
                Return 0
            End If
            If Not(left = "未知版本") AndAlso Not(right = "未知版本") Then
                left = left.Lower()
                right = right.Lower()
                Dim list As List(Of String) = left.Replace("快照", "snapshot").Replace("预览版", "pre").RegexSearch("[a-z]+|[0-9]+", RegexOptions.None).ToList()
                Dim list2 As List(Of String) = right.Replace("快照", "snapshot").Replace("预览版", "pre").RegexSearch("[a-z]+|[0-9]+", RegexOptions.None).ToList()
                Dim num As Integer = 0
                While num < list.Count OrElse num < list2.Count
                    Dim text As String = If((num < list.Count), list(num), "0")
                    Dim text2 As String = If((num < list2.Count), list2(num), "0")
                    If Not(text = text2) Then
                        Dim num2 As Integer
                        If Not(text = "rc") Then
                            If Not(text = "pre") Then
                                If Not(text = "snapshot") Then
                                    If Not(text = "experimental") Then
                                        Dim num3 As Integer
                                        num2 = If(Integer.TryParse(text, NumberStyles.[Integer], CultureInfo.InvariantCulture, num3), num3, 0)
                                    Else
                                        num2 = -4
                                    End If
                                Else
                                    num2 = -3
                                End If
                            Else
                                num2 = -2
                            End If
                        Else
                            num2 = -1
                        End If
                        Dim num4 As Integer = num2
                        If Not(text2 = "rc") Then
                            If Not(text2 = "pre") Then
                                If Not(text2 = "snapshot") Then
                                    If Not(text2 = "experimental") Then
                                        Dim num5 As Integer
                                        num2 = If(Integer.TryParse(text2, NumberStyles.[Integer], CultureInfo.InvariantCulture, num5), num5, 0)
                                    Else
                                        num2 = -4
                                    End If
                                Else
                                    num2 = -3
                                End If
                            Else
                                num2 = -2
                            End If
                        Else
                            num2 = -1
                        End If
                        Dim num6 As Integer = num2
                        If num4 = 0 AndAlso num6 = 0 Then
                            Return String.Compare(text, text2, StringComparison.Ordinal)
                        End If
                        If num4 <> num6 Then
                            Return num4.CompareTo(num6)
                        End If
                    End If
                    num += 1
                End While
                Return String.Compare(left, right, StringComparison.Ordinal)
            End If
            If Not(left = "未知版本") Then
                Return -1
            End If
            Return 1
        End Function

        Public Function CompareVersionGE(left As String, right As String) As Boolean
            Return McUtils.CompareVersion(left, right) >= 0
        End Function
    End Module
End Namespace
