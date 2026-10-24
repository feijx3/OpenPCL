Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text.RegularExpressions
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class ValidateFolderPath
        Inherits Validate

        Public Property UseMinecraftCharCheck As Boolean

        Public Sub New()
            Me.UseMinecraftCharCheck = True
        End Sub

        Public Sub New(UseMinecraftCharCheck As Boolean)
            Me.UseMinecraftCharCheck = True
            Me.UseMinecraftCharCheck = UseMinecraftCharCheck
        End Sub

        Public Overrides Function Validate(Str As String) As String
            Str = Str.Replace("/", "\")
            If Not PathUtils.RemoveSlashSuffix(Str).EndsWithF(":", False) Then
                Str = PathUtils.RemoveSlashSuffix(Str)
            End If
            Dim text As String = New ValidateNullOrWhiteSpace().Validate(Str)
            Dim text2 As String
            If Operators.CompareString(text, "", False) <> 0 Then
                text2 = text
            Else
                text = New ValidateLength(1, 254).Validate(Str)
                If Operators.CompareString(text, "", False) <> 0 Then
                    text2 = text
                Else
                    Dim text3 As String = Str.RegexSeek(".{2,}~\d", RegexOptions.None)
                    If text3 IsNot Nothing Then
                        text2 = String.Format("文件夹名不能包含 {0} 这一特殊格式！", text3)
                    Else
                        If Not Str.StartsWithF("\\Mac\", False) Then
                            Try
                                Dim logicalDrives As IEnumerable(Of String) = Environment.GetLogicalDrives()
                                Dim func As Func(Of String, Boolean)
                                If ValidateFolderPath._Closure$__.$I6-0 IsNot Nothing Then
                                    func = ValidateFolderPath._Closure$__.$I6-0
                                Else
                                    Dim func2 As Func(Of String, Boolean) = Function(p As String) DirectoryUtils.Exists(p)
                                    func = func2
                                    ValidateFolderPath._Closure$__.$I6-0 = func2
                                End If
                                For Each text4 As String In logicalDrives.Where(func)
                                    Dim text5 As String = Conversions.ToString(text4.Upper().First()) + ":\"
                                    If Operators.CompareString(Str.Upper(), text5, False) = 0 Then
                                        Return ""
                                    End If
                                    If Str.StartsWithF(text5, True) Then
                                        GoTo IL_0146
                                    End If
                                Next
                            Finally
                                Dim enumerator As IEnumerator(Of String)
                                If enumerator IsNot Nothing Then
                                    enumerator.Dispose()
                                End If
                            End Try
                            Return "文件夹路径头存在错误！"
                        End If
                        IL_0146:
                        Dim num As Integer = If(Str.StartsWithF("\\Mac\", False), 2, 1)
                        Dim num2 As Integer = Str.Split("\", False).Count() - 1
                        For i As Integer = num To num2
                            Dim text6 As String = Str.Split("\", False)(i)
                            If Operators.CompareString(New ValidateNullOrWhiteSpace().Validate(text6), "", False) <> 0 Then
                                Return "文件夹路径存在错误！"
                            End If
                            Dim text7 As String = New ValidateExcept(New String(Path.GetInvalidFileNameChars()) + If(Me.UseMinecraftCharCheck, "!;", ""), "路径中存在无效字符！").Validate(text6)
                            If Operators.CompareString(text7, "", False) <> 0 Then
                                Return text7
                            End If
                            If text6.StartsWithF(" ", False) Then
                                Return "文件夹名不能以空格开头！"
                            End If
                            If text6.EndsWithF(" ", False) Then
                                Return "文件夹名不能以空格结尾！"
                            End If
                            If text6.EndsWithF(".", False) Then
                                Return "文件夹名不能以小数点结尾！"
                            End If
                            Dim text8 As String = New ValidateExceptSame(New String() { "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" }, "文件夹名不可为 %！", False).Validate(text6)
                            If Operators.CompareString(text8, "", False) <> 0 Then
                                Return text8
                            End If
                        Next
                        text2 = ""
                    End If
                End If
            End If
            Return text2
        End Function
    End Class
End Namespace
