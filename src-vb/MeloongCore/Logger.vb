Imports System
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Text

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Module Logger
        Public Property Instance As BaseLogger = New BaseLogger()

        Public Sub Trace(message As String, Optional behavior As LogBehavior = LogBehavior.None, <CallerFilePath()> Optional filePath As String = "")
            Logger.Log(Nothing, message, LogLevel.Trace, behavior, filePath)
        End Sub

        Public Sub Info(message As String, Optional behavior As LogBehavior = LogBehavior.None, <CallerFilePath()> Optional filePath As String = "")
            Logger.Log(Nothing, message, LogLevel.Info, behavior, filePath)
        End Sub

        Public Sub Warn(message As String, Optional behavior As LogBehavior = LogBehavior.ToastIfDebug, <CallerFilePath()> Optional filePath As String = "")
            Logger.Log(Nothing, message, LogLevel.Warn, behavior, filePath)
        End Sub

        Public Sub [Error](message As String, Optional behavior As LogBehavior = LogBehavior.AlertThenFeedback, <CallerFilePath()> Optional filePath As String = "")
            Logger.Log(Nothing, message, LogLevel.[Error], behavior, filePath)
        End Sub

        Public Sub Log(message As String, Optional level As LogLevel = LogLevel.Info, Optional behavior As LogBehavior = LogBehavior.None, <CallerFilePath()> Optional filePath As String = "")
            Logger.Log(Nothing, message, level, behavior, filePath)
        End Sub

        Public Sub Trace(ex As Exception, <Nullable(2)> Optional message As String = Nothing, Optional behavior As LogBehavior = LogBehavior.None, <CallerFilePath()> Optional filePath As String = "")
            Logger.Log(ex, message, LogLevel.Trace, behavior, filePath)
        End Sub

        Public Sub Info(ex As Exception, <Nullable(2)> Optional message As String = Nothing, Optional behavior As LogBehavior = LogBehavior.None, <CallerFilePath()> Optional filePath As String = "")
            Logger.Log(ex, message, LogLevel.Info, behavior, filePath)
        End Sub

        Public Sub Warn(ex As Exception, <Nullable(2)> Optional message As String = Nothing, Optional behavior As LogBehavior = LogBehavior.ToastIfDebug, <CallerFilePath()> Optional filePath As String = "")
            Logger.Log(ex, message, LogLevel.Warn, behavior, filePath)
        End Sub

        Public Sub [Error](ex As Exception, <Nullable(2)> Optional message As String = Nothing, Optional behavior As LogBehavior = LogBehavior.AlertThenFeedback, <CallerFilePath()> Optional filePath As String = "")
            Logger.Log(ex, message, LogLevel.[Error], behavior, filePath)
        End Sub

        <NullableContext(2)>
        Public Sub Log(ex As Exception, Optional message As String = Nothing, Optional level As LogLevel = LogLevel.Warn, Optional behavior As LogBehavior = LogBehavior.ToastIfDebug, <Nullable(1)> <CallerFilePath()> Optional filePath As String = "")
            Logger.Instance.Log(ex, message, level, behavior, filePath)
        End Sub

        <Conditional("DEBUG")>
        Public Sub SendToDebug(message As String, Optional level As LogLevel = LogLevel.[Error], <CallerFilePath()> Optional filePath As String = "")
        End Sub

        <Conditional("DEBUG")>
        Public Sub SendToDebug(ex As Exception, <Nullable(2)> Optional message As String = Nothing, Optional level As LogLevel = LogLevel.[Error], <CallerFilePath()> Optional filePath As String = "")
        End Sub

        Public Sub StackTrace(Optional level As LogLevel = LogLevel.Trace, <CallerFilePath()> Optional filePath As String = "")
            Logger.Instance.Log(Nothing, New StackTrace(1, True).ToString(), level, LogBehavior.None, filePath)
        End Sub

        Public Sub Log(<Nullable(New Byte() { 1, 2 })> messageFactory As Func(Of String), Optional level As LogLevel = LogLevel.Info, Optional behavior As LogBehavior = LogBehavior.None, <CallerFilePath()> Optional filePath As String = "")
            If Logger.Instance.MinLevel > level Then
                Return
            End If
            Logger.Instance.Log(Nothing, If(messageFactory(), String.Empty), level, behavior, filePath)
        End Sub

        Public Sub Trace(<Nullable(New Byte() { 1, 2 })> messageFactory As Func(Of String), Optional behavior As LogBehavior = LogBehavior.None, <CallerFilePath()> Optional filePath As String = "")
            Logger.Log(messageFactory, LogLevel.Trace, behavior, filePath)
        End Sub

        Public Sub Info(<Nullable(New Byte() { 1, 2 })> messageFactory As Func(Of String), Optional behavior As LogBehavior = LogBehavior.None, <CallerFilePath()> Optional filePath As String = "")
            Logger.Log(messageFactory, LogLevel.Info, behavior, filePath)
        End Sub

        Public Sub Warn(<Nullable(New Byte() { 1, 2 })> messageFactory As Func(Of String), Optional behavior As LogBehavior = LogBehavior.ToastIfDebug, <CallerFilePath()> Optional filePath As String = "")
            Logger.Log(messageFactory, LogLevel.Warn, behavior, filePath)
        End Sub

        Public Sub [Error](<Nullable(New Byte() { 1, 2 })> messageFactory As Func(Of String), Optional behavior As LogBehavior = LogBehavior.AlertThenFeedback, <CallerFilePath()> Optional filePath As String = "")
            Logger.Log(messageFactory, LogLevel.[Error], behavior, filePath)
        End Sub

        Public Sub Log(level As LogLevel, <InterpolatedStringHandlerArgument("level")> ByRef handler As Logger.LogInterpolatedStringHandler, Optional behavior As LogBehavior = LogBehavior.None, <CallerFilePath()> Optional filePath As String = "")
            If handler.isEnabled Then
                Logger.Instance.Log(Nothing, handler.ToStringAndClear(), level, behavior, filePath)
            End If
        End Sub

        <NullableContext(0)>
        Public Sub Trace(ByRef handler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel), Optional behavior As LogBehavior = LogBehavior.None, <Nullable(1)> <CallerFilePath()> Optional filePath As String = "")
            If handler.IsEnabled Then
                Logger.Instance.Log(Nothing, handler.ToStringAndClear(), LogLevel.Trace, behavior, filePath)
            End If
        End Sub

        <NullableContext(0)>
        Public Sub Info(ByRef handler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel), Optional behavior As LogBehavior = LogBehavior.None, <Nullable(1)> <CallerFilePath()> Optional filePath As String = "")
            If handler.IsEnabled Then
                Logger.Instance.Log(Nothing, handler.ToStringAndClear(), LogLevel.Info, behavior, filePath)
            End If
        End Sub

        <NullableContext(0)>
        Public Sub Warn(ByRef handler As Logger.LogInterpolatedStringHandler(Of Logger.WarnLogLevel), Optional behavior As LogBehavior = LogBehavior.ToastIfDebug, <Nullable(1)> <CallerFilePath()> Optional filePath As String = "")
            If handler.IsEnabled Then
                Logger.Instance.Log(Nothing, handler.ToStringAndClear(), LogLevel.Warn, behavior, filePath)
            End If
        End Sub

        <NullableContext(0)>
        Public Sub [Error](ByRef handler As Logger.LogInterpolatedStringHandler(Of Logger.ErrorLogLevel), Optional behavior As LogBehavior = LogBehavior.AlertThenFeedback, <Nullable(1)> <CallerFilePath()> Optional filePath As String = "")
            If handler.IsEnabled Then
                Logger.Instance.Log(Nothing, handler.ToStringAndClear(), LogLevel.[Error], behavior, filePath)
            End If
        End Sub

        <NullableContext(2)>
        <Nullable(0)>
        <CompilerFeatureRequired("RefStructs")>
        <InterpolatedStringHandler()>
        Public Structure LogInterpolatedStringHandler
            Public Sub New(literalLength As Integer, level As LogLevel, <System.Runtime.InteropServices.OutAttribute()> ByRef isEnabled As Boolean)
                Dim flag As Boolean = Logger.Instance.MinLevel <= level
                Dim flag2 As Boolean = flag
                isEnabled = flag
                Me.isEnabled = flag2
                Me._sb = If(Me.isEnabled, New StringBuilder(literalLength), Nothing)
            End Sub

            <NullableContext(1)>
            Public Sub AppendLiteral(value As String)
                Dim sb As StringBuilder = Me._sb
                If sb Is Nothing Then
                    Return
                End If
                sb.Append(value)
            End Sub

            <NullableContext(1)>
            Public Sub AppendFormatted(Of T)(value As T)
                Dim sb As StringBuilder = Me._sb
                If sb Is Nothing Then
                    Return
                End If
                sb.Append(value)
            End Sub

            Public Sub AppendFormatted(Of T)(<Nullable(1)> value As T, format As String)
                Dim sb As StringBuilder = Me._sb
                If sb Is Nothing Then
                    Return
                End If
                Dim formattable As IFormattable = TryCast(value, IFormattable)
                Dim text As String
                If formattable Is Nothing Then
                    Dim ptr As __ByRef(Of T) = value
                    Dim t As T = Nothing
                    If t Is Nothing Then
                        t = value
                        ptr = t
                        If t Is Nothing Then
                            text = Nothing
                            GoTo IL_0055
                        End If
                    End If
                    text = ptr.ToString()
                Else
                    text = formattable.ToString(format, Nothing)
                End If
                IL_0055:
                sb.Append(text)
            End Sub

            <NullableContext(1)>
            Public Sub AppendFormatted(Of T)(value As T, alignment As Integer)
                If Me._sb Is Nothing Then
                    Return
                End If
                Dim ptr As __ByRef(Of T) = value
                Dim t As T = Nothing
                Dim text As String
                If t Is Nothing Then
                    t = value
                    ptr = t
                    If t Is Nothing Then
                        text = Nothing
                        GoTo IL_003A
                    End If
                End If
                text = ptr.ToString()
                IL_003A:
                Dim text2 As String = If(text, String.Empty)
                Me._sb.Append(If((alignment > 0), text2.PadLeft(alignment), If((alignment < 0), text2.PadRight(-alignment), text2)))
            End Sub

            Public Sub AppendFormatted(Of T)(<Nullable(1)> value As T, alignment As Integer, format As String)
                If Me._sb Is Nothing Then
                    Return
                End If
                Dim formattable As IFormattable = TryCast(value, IFormattable)
                Dim text2 As String
                If formattable Is Nothing Then
                    Dim ptr As __ByRef(Of T) = value
                    Dim t As T = Nothing
                    Dim text As String
                    If t Is Nothing Then
                        t = value
                        ptr = t
                        If t Is Nothing Then
                            text = Nothing
                            GoTo IL_0049
                        End If
                    End If
                    text = ptr.ToString()
                    IL_0049:
                    text2 = If(text, String.Empty)
                Else
                    text2 = formattable.ToString(format, Nothing)
                End If
                Dim text3 As String = text2
                Me._sb.Append(If((alignment > 0), text3.PadLeft(alignment), If((alignment < 0), text3.PadRight(-alignment), text3)))
            End Sub

            Public Sub AppendFormatted(value As String)
                Dim sb As StringBuilder = Me._sb
                If sb Is Nothing Then
                    Return
                End If
                sb.Append(value)
            End Sub

            Public Sub AppendFormatted(value As Object, Optional alignment As Integer = 0, Optional format As String = Nothing)
                If Me._sb Is Nothing Then
                    Return
                End If
                Dim formattable As IFormattable = TryCast(value, IFormattable)
                Dim text As String = If((formattable IsNot Nothing), formattable.ToString(format, Nothing), (If(If((value IsNot Nothing), value.ToString(), Nothing), String.Empty)))
                Me._sb.Append(If((alignment > 0), text.PadLeft(alignment), If((alignment < 0), text.PadRight(-alignment), text)))
            End Sub

            <NullableContext(1)>
            Friend Function ToStringAndClear() As String
                Dim sb As StringBuilder = Me._sb
                Dim text As String = If(If((sb IsNot Nothing), sb.ToString(), Nothing), String.Empty)
                Me._sb = Nothing
                Return text
            End Function

            Private _sb As StringBuilder

            Public isEnabled As Boolean
        End Structure

        <Nullable(0)>
        <CompilerFeatureRequired("RefStructs")>
        <InterpolatedStringHandler()>
        Public Structure LogInterpolatedStringHandler(Of TLevel As{Structure, Logger.ILogLevelProvider})
            Public ReadOnly Property IsEnabled As Boolean
                Get
                    Return Me._inner.isEnabled
                End Get
            End Property

            Public Sub New(literalLength As Integer, formattedCount As Integer, <System.Runtime.InteropServices.OutAttribute()> ByRef isEnabled As Boolean)
                Dim tlevel As TLevel = Nothing
                Me._inner = New Logger.LogInterpolatedStringHandler(literalLength, tlevel.Level, isEnabled)
            End Sub

            Public Sub AppendLiteral(value As String)
                Me._inner.AppendLiteral(value)
            End Sub

            Public Sub AppendFormatted(Of T)(value As T)
                Me._inner.AppendFormatted(Of T)(value)
            End Sub

            <NullableContext(2)>
            Public Sub AppendFormatted(Of T)(<Nullable(1)> value As T, format As String)
                Me._inner.AppendFormatted(Of T)(value, format)
            End Sub

            Public Sub AppendFormatted(Of T)(value As T, alignment As Integer)
                Me._inner.AppendFormatted(Of T)(value, alignment)
            End Sub

            <NullableContext(2)>
            Public Sub AppendFormatted(Of T)(<Nullable(1)> value As T, alignment As Integer, format As String)
                Me._inner.AppendFormatted(Of T)(value, alignment, format)
            End Sub

            <NullableContext(2)>
            Public Sub AppendFormatted(value As String)
                Me._inner.AppendFormatted(value)
            End Sub

            <NullableContext(2)>
            Public Sub AppendFormatted(value As Object, Optional alignment As Integer = 0, Optional format As String = Nothing)
                Me._inner.AppendFormatted(value, alignment, format)
            End Sub

            Friend Function ToStringAndClear() As String
                Return Me._inner.ToStringAndClear()
            End Function

            Private _inner As Logger.LogInterpolatedStringHandler
        End Structure

        <NullableContext(0)>
        Public Structure TraceLogLevel
            Implements Logger.ILogLevelProvider

            Public ReadOnly Property Level As LogLevel Implements MeloongCore.Logger.ILogLevelProvider.Level
                Get
                    Return LogLevel.Trace
                End Get
            End Property
        End Structure

        <NullableContext(0)>
        Public Structure InfoLogLevel
            Implements Logger.ILogLevelProvider

            Public ReadOnly Property Level As LogLevel Implements MeloongCore.Logger.ILogLevelProvider.Level
                Get
                    Return LogLevel.Info
                End Get
            End Property
        End Structure

        <NullableContext(0)>
        Public Structure WarnLogLevel
            Implements Logger.ILogLevelProvider

            Public ReadOnly Property Level As LogLevel Implements MeloongCore.Logger.ILogLevelProvider.Level
                Get
                    Return LogLevel.Warn
                End Get
            End Property
        End Structure

        <NullableContext(0)>
        Public Structure ErrorLogLevel
            Implements Logger.ILogLevelProvider

            Public ReadOnly Property Level As LogLevel Implements MeloongCore.Logger.ILogLevelProvider.Level
                Get
                    Return LogLevel.[Error]
                End Get
            End Property
        End Structure

        Public Interface ILogLevelProvider
            ReadOnly Property Level As LogLevel
        End Interface
    End Module
End Namespace
