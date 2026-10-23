Imports System
Imports System.IO
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Class FileLogger
        Inherits BaseLogger
        Implements IDisposable

        Public Overrides Sub Init()
            Logger.Info("开始轮转日志文件", LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\Logger.cs")
            Try
                For i As Integer = 4 To 1 Step -1
                    Dim text As String = Path.Combine(Me.logFolder, String.Format("{0}{1}{2}", Me.fileNamePrefix, i + 1, Me.fileNameSuffix))
                    Dim text2 As String = Path.Combine(Me.logFolder, String.Format("{0}{1}{2}", Me.fileNamePrefix, i, Me.fileNameSuffix))
                    If FileUtils.Exists(text2) Then
                        FileUtils.Copy(text2, text)
                        FileUtils.Delete(text2, False)
                    End If
                Next
            Catch ex As IOException
                Logger.Warn(ex, "可能同时开启了多个程序，这或许会导致未知问题", LogBehavior.Toast, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\Logger.cs")
            Catch ex2 As Exception
                Logger.Warn(ex2, "整理日志文件失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\Logger.cs")
            End Try
            Dim text3 As String = PathUtils.ForApi(Path.Combine(Me.logFolder, Me.fileNamePrefix + "1" + Me.fileNameSuffix))
            DirectoryUtils.Create(Me.logFolder)
            Me.writer = New StreamWriter(text3, True) With { .AutoFlush = True }
        End Sub

        Public Overrides Sub Output(entry As BaseLogger.LogEntry)
            MyBase.Output(entry)
            Dim streamWriter As StreamWriter = Me.writer
            If streamWriter Is Nothing Then
                Return
            End If
            streamWriter.WriteLine(entry.message)
        End Sub

        Public Sub Dispose() Implements System.IDisposable.Dispose
            Me.available = False
            If Me.writer IsNot Nothing Then
                CType(Me.writer, IDisposable).Dispose()
            End If
            Me.queuedEvent.Dispose()
            GC.SuppressFinalize(Me)
        End Sub

        Public logFolder As String = Path.Combine(Paths.Base, "logs")

        Public fileNamePrefix As String = "Log"

        Public fileNameSuffix As String = ".txt"

        <Nullable(2)>
        Private writer As StreamWriter
    End Class
End Namespace
