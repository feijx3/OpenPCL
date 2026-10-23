Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Threading
Imports MeloongCore.Extensions

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Class BaseLogger
        Public Property MinLevel As LogLevel

        <NullableContext(2)>
        Public Overridable Sub Log(ex As Exception, message As String, level As LogLevel, behavior As LogBehavior, <Nullable(1)> filePath As String)
            If ex IsNot Nothing AndAlso ex.IsCanceled() Then
                Me.Log(Nothing, "操作已取消（" + message + "）", LogLevel.Info, LogBehavior.None, filePath)
                Return
            End If
            If Me.MinLevel > level Then
                Return
            End If
            Try
                Dim text As String = If((ex Is Nothing), (If(message, "")), (If((message Is Nothing), "", (message + "：")) + ex.GetDisplay(True)))
                text = Me.Format(text, level, filePath, ex)
                If Me.available OrElse Me.queue.Count < 100 Then
                    Me.queue.Enqueue(New BaseLogger.LogEntry(level, text))
                    Me.queuedEvent.[Set]()
                End If
                Me.HandleBehavior(message, text, behavior, ex)
            Catch ex2 As Exception
            End Try
        End Sub

        Public Overridable Function Format(text As String, level As LogLevel, filePath As String, <Nullable(2)> ex As Exception) As String
            Dim prefix As String = BaseLogger.GetLogPrefix(level, filePath)
            If String.IsNullOrEmpty(text) Then
                Return prefix
            End If
            Return text.ReplaceLineEndings(vbLf, True).Split(vbLf, True).[Select](Function(t As String) prefix + t).Join(vbCrLf)
        End Function

        Public Shared Function GetLogPrefix(level As LogLevel, filePath As String) As String
            Return String.Format("{0:HH':'mm':'ss'.'fff} {1} {2}[{3}] ", New Object() { DateTime.Now, level.ToString().First(), If((Thread.CurrentThread.Name IsNot Nothing), String.Format("<{0} · {1}> ", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name), If(Thread.CurrentThread.IsThreadPoolThread, "<Async> ", "")), PathUtils.GetLastPart(filePath).BeforeFirst(".", False) })
        End Function

        <NullableContext(2)>
        Public Overridable Sub HandleBehavior(rawMessage As String, <Nullable(1)> formattedMessage As String, behavior As LogBehavior, ex As Exception)
        End Sub

        Public Overridable Sub Output(entry As BaseLogger.LogEntry)
        End Sub

        Public Overridable Sub Init()
        End Sub

        Public Sub New()
            New Thread(Sub()
                Try
                    Me.Init()
                    Me.available = True
                    Dim flag As Boolean
                    Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(8, 1, flag)
                    If flag Then
                        logInterpolatedStringHandler.AppendFormatted(MyBase.[GetType]().Name)
                        logInterpolatedStringHandler.AppendLiteral(" 日志初始化结束")
                    End If
                    Logger.Info(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\Logger.cs")
                    While Me.available
                        Me.queuedEvent.WaitOne()
                        Me.Flush()
                    End While
                Catch ex As Exception
                    Me.available = False
                End Try
            End Sub) With { .Name = "Logger", .Priority = ThreadPriority.Lowest, .IsBackground = True }.Start()
        End Sub

        Public Sub Flush()
            Try
                If Me.available Then
                    Dim obj As Object = Me.flushLock
                    SyncLock obj
                        Dim logEntry As BaseLogger.LogEntry
                        While Me.queue.TryDequeue(logEntry)
                            Me.Output(logEntry)
                        End While
                    End SyncLock
                End If
            Catch ex As Exception
            End Try
        End Sub

        Friend available As Boolean

        Friend queue As ConcurrentQueue(Of BaseLogger.LogEntry) = New ConcurrentQueue(Of BaseLogger.LogEntry)()

        Friend queuedEvent As AutoResetEvent = New AutoResetEvent(False)

        Private flushLock As Object = New Object()

        <Nullable(0)>
        Public Structure LogEntry
            Implements IEquatable(Of BaseLogger.LogEntry)

            Public Sub New(Level As LogLevel, message As String)
                Me.Level = Level
                Me.message = message
            End Sub

            Public Property Level As LogLevel

            Public Property message As String

            <NullableContext(0)>
            <CompilerGenerated()>
            Public Overrides Function ToString() As String
                Dim stringBuilder As StringBuilder = New StringBuilder()
                stringBuilder.Append("LogEntry")
                stringBuilder.Append(" { ")
                If Me.PrintMembers(stringBuilder) Then
                    stringBuilder.Append(" "c)
                End If
                stringBuilder.Append("}"c)
                Return stringBuilder.ToString()
            End Function

            <NullableContext(0)>
            <CompilerGenerated()>
            Private Function PrintMembers(builder As StringBuilder) As Boolean
                builder.Append("Level = ")
                builder.Append(Me.Level.ToString())
                builder.Append(", message = ")
                builder.Append(Me.message)
                Return True
            End Function

            <CompilerGenerated()>
            Public Shared Operator <>(left As BaseLogger.LogEntry, right As BaseLogger.LogEntry) As Boolean
                Return Not(left = right)
            End Operator

            <CompilerGenerated()>
            Public Shared Operator =(left As BaseLogger.LogEntry, right As BaseLogger.LogEntry) As Boolean
                Return left.Equals(right)
            End Operator

            <CompilerGenerated()>
            Public Overrides Function GetHashCode() As Integer
                Return EqualityComparer(Of LogLevel).[Default].GetHashCode(Me.<Level>k__BackingField) * -1521134295 + EqualityComparer(Of String).[Default].GetHashCode(Me.<message>k__BackingField)
            End Function

            <NullableContext(0)>
            <CompilerGenerated()>
            Public Overrides Overloads Function Equals(obj As Object) As Boolean
                Return TypeOf obj Is BaseLogger.LogEntry AndAlso Me.Equals(CType(obj, BaseLogger.LogEntry))
            End Function

            <CompilerGenerated()>
            Public Overloads Function Equals(other As BaseLogger.LogEntry) As Boolean Implements System.IEquatable(Of MeloongCore.BaseLogger.LogEntry).Equals
                Return EqualityComparer(Of LogLevel).[Default].Equals(Me.<Level>k__BackingField, other.<Level>k__BackingField) AndAlso EqualityComparer(Of String).[Default].Equals(Me.<message>k__BackingField, other.<message>k__BackingField)
            End Function

            <CompilerGenerated()>
            Public Sub Deconstruct(<System.Runtime.InteropServices.OutAttribute()> ByRef Level As LogLevel, <System.Runtime.InteropServices.OutAttribute()> ByRef message As String)
                Level = Me.Level
                message = Me.message
            End Sub
        End Structure
    End Class
End Namespace
