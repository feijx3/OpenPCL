Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class ValidateFileName
        Inherits Validate

        Public Property Name As String

        Public Property UseMinecraftCharCheck As Boolean

        Public Property IgnoreCase As Boolean

        Public Property ParentFolder As String

        Public Property RequireParentFolderExists As Object
            <CompilerGenerated()>
            Get
                Return Me._RequireParentFolderExists
            End Get
            <CompilerGenerated()>
            Set(value As Object)
                Me._RequireParentFolderExists = RuntimeHelpers.GetObjectValue(value)
            End Set
        End Property

        Public Sub New()
            Me.UseMinecraftCharCheck = True
            Me.IgnoreCase = True
            Me.ParentFolder = Nothing
            Me.RequireParentFolderExists = True
        End Sub

        Public Sub New(Name As String, Optional UseMinecraftCharCheck As Boolean = True, Optional IgnoreCase As Boolean = True)
            Me.UseMinecraftCharCheck = True
            Me.IgnoreCase = True
            Me.ParentFolder = Nothing
            Me.RequireParentFolderExists = True
            Me.Name = Name
            Me.IgnoreCase = IgnoreCase
            Me.UseMinecraftCharCheck = UseMinecraftCharCheck
        End Sub

        Public Overrides Function Validate(Str As String) As String
            Dim text2 As String
            Try
                Dim text As String = New ValidateNullOrWhiteSpace().Validate(Str)
                If Operators.CompareString(text, "", False) <> 0 Then
                    text2 = text
                ElseIf Str.StartsWithF(" ", False) Then
                    text2 = "文件名不能以空格开头！"
                ElseIf Str.EndsWithF(" ", False) Then
                    text2 = "文件名不能以空格结尾！"
                Else
                    text = New ValidateLength(1, 253).Validate(Str + Me.ParentFolder)
                    If Operators.CompareString(text, "", False) <> 0 Then
                        text2 = text
                    ElseIf Str.EndsWithF(".", False) Then
                        text2 = "文件名不能以小数点结尾！"
                    Else
                        Dim text3 As String = New ValidateExcept(New String(Path.GetInvalidFileNameChars()) + If(Me.UseMinecraftCharCheck, "!;", ""), "文件名不可包含 % 字符！").Validate(Str)
                        If Operators.CompareString(text3, "", False) <> 0 Then
                            text2 = text3
                        Else
                            Dim text4 As String = New ValidateExceptSame(New String() { "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" }, "文件名不可为 %！", True).Validate(Str)
                            If Operators.CompareString(text4, "", False) <> 0 Then
                                text2 = text4
                            ElseIf Str.RegexCheck(".{2,}~\d", RegexOptions.None) Then
                                text2 = "文件名不能包含这一特殊格式！"
                            Else
                                If Me.ParentFolder IsNot Nothing Then
                                    Dim info As DirectoryInfo = DirectoryUtils.GetInfo(Me.ParentFolder)
                                    If info.Exists Then
                                        Dim enumerable As IEnumerable(Of FileInfo) = info.EnumerateFiles("*")
                                        Dim func As Func(Of FileInfo, String)
                                        If ValidateFileName._Closure$__.$I22-0 IsNot Nothing Then
                                            func = ValidateFileName._Closure$__.$I22-0
                                        Else
                                            Dim func2 As Func(Of FileInfo, String) = Function(f As FileInfo) f.Name
                                            func = func2
                                            ValidateFileName._Closure$__.$I22-0 = func2
                                        End If
                                        Dim text5 As String = New ValidateExceptSame(enumerable.[Select](func), "不可与现有文件重名！", Me.IgnoreCase).Validate(Str)
                                        If Operators.CompareString(text5, "", False) <> 0 Then
                                            Return text5
                                        End If
                                    ElseIf Conversions.ToBoolean(Me.RequireParentFolderExists) Then
                                        Return String.Format("父文件夹不存在：{0}", Me.ParentFolder)
                                    End If
                                End If
                                text2 = ""
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                Logger.Warn(ex, "检查文件名出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModValidate.vb")
                text2 = "错误：" + ex.Message
            End Try
            Return text2
        End Function
    End Class
End Namespace
