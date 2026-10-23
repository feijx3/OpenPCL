Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text.RegularExpressions
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class ValidateFolderName
        Inherits Validate

        Public Property Folder As String

        Public Property UseMinecraftCharCheck As Boolean

        Public Sub New()
            Me.UseMinecraftCharCheck = True
        End Sub

        Public Sub New(Folder As String, Optional UseMinecraftCharCheck As Boolean = True, Optional IgnoreList As List(Of String) = Nothing)
            Me.UseMinecraftCharCheck = True
            Dim num2 As Integer
            Dim num4 As Integer
            Dim obj As Object
            Try
                IL_000D:
                Dim num As Integer = 1
                Me.Folder = Folder
                IL_0016:
                num = 2
                Me.UseMinecraftCharCheck = UseMinecraftCharCheck
                IL_001F:
                ProjectData.ClearProjectError()
                num2 = 1
                IL_0026:
                num = 4
                Me.PathIgnore = DirectoryUtils.EnumerateDirectories(Folder, False, "*")
                IL_003A:
                num = 5
                Me.IgnoreList = IgnoreList
                IL_0043:
                GoTo IL_00AA
                IL_0045:
                Dim num3 As Integer = num4 + 1
                num4 = 0
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3)
                IL_006B:
                GoTo IL_009F
                IL_006D:
                num4 = num
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num2)
                IL_007D:
            Catch obj2 When endfilter((TypeOf obj Is Exception) And (num2 <> 0) And (num4 = 0))
                Dim ex As Exception = CType(obj2, Exception)
                GoTo IL_006D
            End Try
            IL_009F:
            Throw ProjectData.CreateProjectError(-2146828237)
            IL_00AA:
            If num4 <> 0 Then
                ProjectData.ClearProjectError()
            End If
        End Sub

        Public Overrides Function Validate(Str As String) As String
            Dim text2 As String
            Try
                Dim text As String = New ValidateNullOrWhiteSpace().Validate(Str)
                If Operators.CompareString(text, "", False) <> 0 Then
                    text2 = text
                ElseIf Str.StartsWithF(" ", False) Then
                    text2 = "文件夹名不能以空格开头！"
                ElseIf Str.EndsWithF(" ", False) Then
                    text2 = "文件夹名不能以空格结尾！"
                Else
                    text = New ValidateLength(1, 100).Validate(Str)
                    If Operators.CompareString(text, "", False) <> 0 Then
                        text2 = text
                    ElseIf Str.EndsWithF(".", False) Then
                        text2 = "文件夹名不能以小数点结尾！"
                    Else
                        Dim text3 As String = New ValidateExcept(New String(Path.GetInvalidFileNameChars()) + If(Me.UseMinecraftCharCheck, "!;", ""), "文件夹名不可包含 % 字符！").Validate(Str)
                        If Operators.CompareString(text3, "", False) <> 0 Then
                            text2 = text3
                        Else
                            Dim text4 As String = New ValidateExceptSame(New String() { "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" }, "文件夹名不可为 %！", True).Validate(Str)
                            If Operators.CompareString(text4, "", False) <> 0 Then
                                text2 = text4
                            ElseIf Str.RegexCheck(".{2,}~\d", RegexOptions.None) Then
                                text2 = "文件夹名不能包含这一特殊格式！"
                            Else
                                Dim list As List(Of String) = New List(Of String)()
                                If Me.PathIgnore IsNot Nothing Then
                                    Try
                                        For Each text5 As String In Me.PathIgnore
                                            Dim lastPart As String = PathUtils.GetLastPart(text5)
                                            If Me.IgnoreList Is Nothing OrElse Not Me.IgnoreList.Contains(lastPart, StringComparer.OrdinalIgnoreCase) Then
                                                list.Add(lastPart)
                                            End If
                                        Next
                                    Finally
                                        Dim enumerator As IEnumerator(Of String)
                                        If enumerator IsNot Nothing Then
                                            enumerator.Dispose()
                                        End If
                                    End Try
                                End If
                                Dim text6 As String = New ValidateExceptSame(list, "不可与现有文件夹重名！", True).Validate(Str)
                                If Operators.CompareString(text6, "", False) <> 0 Then
                                    text2 = text6
                                Else
                                    text2 = ""
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                Logger.Warn(ex, "检查文件夹名出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModValidate.vb")
                text2 = "错误：" + ex.Message
            End Try
            Return text2
        End Function

        Private PathIgnore As IEnumerable(Of String)

        Private IgnoreList As List(Of String)
    End Class
End Namespace
