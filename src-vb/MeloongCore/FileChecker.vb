Imports System
Imports System.IO
Imports System.Runtime.CompilerServices
Imports MeloongCore.Extensions

Namespace MeloongCore
    Public Class FileChecker
        <NullableContext(1)>
        Public Function Check(localPath As String) As <Nullable(2)> String
            Dim text As String
            Try
                Dim info As FileInfo = FileUtils.GetInfo(localPath)
                If Not info.Exists Then
                    text = "文件不存在：" + localPath
                Else
                    Dim length As Long = info.Length
                    If Me.ActualSize >= 0L AndAlso Me.ActualSize <> length Then
                        text = String.Format("文件大小应为 {0} B，实际为 {1} B", Me.ActualSize, length) + If((length < 2000L), ("，内容为：" + FileUtils.ReadAsString(localPath, Nothing, Nothing)), "")
                    ElseIf Me.MinSize >= 0L AndAlso Me.MinSize > length Then
                        text = String.Format("文件大小应至少为 {0} B，实际为 {1} B", Me.MinSize, length) + If((length < 2000L), ("，内容为：" + FileUtils.ReadAsString(localPath, Nothing, Nothing)), "")
                    Else
                        Dim text2 As String = Me.Hash
                        If Not String.IsNullOrEmpty(text2) Then
                            text2 = text2.Lower()
                            Dim length2 As Integer = text2.Length
                            If length2 <= 40 Then
                                If length2 <> 32 Then
                                    If length2 = 40 Then
                                        Dim text3 As String = CryptographyUtils.ComputeFileHash(localPath, CryptographyUtils.HashMethod.Sha1)
                                        If text2 <> text3 Then
                                            Return "文件 SHA1 应为 " + text2 + "，实际为 " + text3
                                        End If
                                        GoTo IL_01F4
                                    End If
                                Else
                                    Dim text4 As String = CryptographyUtils.ComputeFileHash(localPath, CryptographyUtils.HashMethod.Md5)
                                    If text2 <> text4 Then
                                        Return "文件 MD5 应为 " + text2 + "，实际为 " + text4
                                    End If
                                    GoTo IL_01F4
                                End If
                            ElseIf length2 <> 64 Then
                                If length2 = 128 Then
                                    Dim text5 As String = CryptographyUtils.ComputeFileHash(localPath, CryptographyUtils.HashMethod.Sha512)
                                    If text2 <> text5 Then
                                        Return "文件 SHA512 应为 " + text2 + "，实际为 " + text5
                                    End If
                                    GoTo IL_01F4
                                End If
                            Else
                                Dim text6 As String = CryptographyUtils.ComputeFileHash(localPath, CryptographyUtils.HashMethod.Sha256)
                                If text2 <> text6 Then
                                    Return "文件 SHA256 应为 " + text2 + "，实际为 " + text6
                                End If
                                GoTo IL_01F4
                            End If
                            Return String.Format("无法识别的 Hash 类型（长度为 {0}）：{1}", text2.Length, text2)
                        End If
                        IL_01F4:
                        If Me.IsJson Then
                            Dim text7 As String = FileUtils.ReadAsString(localPath, Nothing, Nothing)
                            If text7 = "" Then
                                Return "读取到的文件为空"
                            End If
                            text7.DeserializeJson()
                        End If
                        text = Nothing
                    End If
                End If
            Catch ex As Exception
                Logger.Warn(ex, "检查文件出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
                text = ex.GetDisplay(False)
            End Try
            Return text
        End Function

        Public ActualSize As Long = -1L

        Public MinSize As Long = -1L

        <Nullable(2)>
        Public Hash As String

        Public IsJson As Boolean
    End Class
End Namespace
