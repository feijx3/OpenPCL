Imports System
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Threading.Tasks
Imports MeloongCore.Extensions

Namespace MeloongCore
    <NullableContext(2)>
    <Nullable(0)>
    Public MustInherit Class RedoableWorkerBase(Of TOut)
        Implements IWorker

        <NullableContext(1)>
        Protected Sub New(<Nullable(New Byte() { 1, 2, 1 })> workload As Func(Of CancellationToken?, ProgressProvider, TOut), <Nullable(2)> Optional progress As ProgressProvider = Nothing, <CallerMemberName()> Optional creatorMemberName As String = "")
        End Sub

        Public ReadOnly Property Running As Boolean Implements MeloongCore.IWorker.Running
            Get
                Dim flag2 As Boolean
                SyncLock Me
                    flag2 = Me.running
                End SyncLock
                Return flag2
            End Get
        End Property

        Public Event Started As Action Implements MeloongCore.IWorker.Started

        Public Event Stopped As Action Implements MeloongCore.IWorker.Stopped

        Public ReadOnly Property HasSucceeded As Boolean Implements MeloongCore.IWorker.HasSucceeded
            Get
                Dim flag2 As Boolean
                SyncLock Me
                    flag2 = Me.hasSucceeded
                End SyncLock
                Return flag2
            End Get
        End Property

        Public Event Succeeded As Action Implements MeloongCore.IWorker.Succeeded

        <Nullable(New Byte() { 2, 1 })>
        <Nullable(New Byte() { 2, 1 })>
        Public Event Failed As Action(Of Exception) Implements MeloongCore.IWorker.Failed

        Public Event Canceled As Action Implements MeloongCore.IWorker.Canceled

        Public ReadOnly Property LastSucceeded As Boolean Implements MeloongCore.IWorker.LastSucceeded
            Get
                Dim flag2 As Boolean
                SyncLock Me
                    flag2 = Me.lastSucceeded
                End SyncLock
                Return flag2
            End Get
        End Property

        <Nullable(1)>
        Public ReadOnly Property LastResult As TOut
            <NullableContext(1)>
            Get
                Dim tout2 As TOut
                SyncLock Me
                    If GetType(TOut) = GetType(NoType) Then
                        Throw New InvalidOperationException("该 Worker 没有返回值。")
                    End If
                    If Not Me.hasSucceeded Then
                        Throw New InvalidOperationException("从未成功完成过。")
                    End If
                    Dim tout As TOut
                    If Me.lastResult IsNot Nothing Then
                        tout = CType(CObj(Me.lastResult), TOut)
                    Else
                        tout2 = Nothing
                        tout = tout2
                    End If
                    tout2 = tout
                End SyncLock
                Return tout2
            End Get
        End Property

        <Nullable(1)>
        Public Property Progress As ProgressProvider Implements MeloongCore.IWorker.Progress = If(progress, New ProgressProvider())

        Public Sub Start(Optional cancellationToken As CancellationToken = Nothing, Optional progressOverride As ProgressProvider = Nothing) Implements MeloongCore.IWorker.Start
            SyncLock Me
                Me.lastToken = cancellationToken
                If progressOverride IsNot Nothing Then
                    Me.Progress = progressOverride
                End If
                If Me.running Then
                    Dim flag2 As Boolean
                    Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(7, 1, flag2)
                    If flag2 Then
                        logInterpolatedStringHandler.AppendFormatted(Me.<creatorMemberName>P)
                        logInterpolatedStringHandler.AppendLiteral("：接到重启请求")
                    End If
                    Logger.Info(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\Worker\RedoableWorker.cs")
                    Me.pendingRedo = True
                    Dim cancellationTokenSource As CancellationTokenSource = Me.realCts
                    If cancellationTokenSource Is Nothing Then
                        Return
                    End If
                    cancellationTokenSource.Cancel()
                    Return
                Else
                    Me.running = True
                    Me.idleEvent.Reset()
                    Me.realCts = CancellationTokenSource.CreateLinkedTokenSource(New CancellationToken() { Me.lastToken })
                End If
            End SyncLock
            New Thread(New ThreadStart(Me._Start)) With { .IsBackground = True, .Name = "W/" + Me.<creatorMemberName>P }.Start()
        End Sub

        Private Sub _Start()
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(5, 1, flag)
            If flag Then
                logInterpolatedStringHandler.AppendFormatted(Me.<creatorMemberName>P)
                logInterpolatedStringHandler.AppendLiteral("：运行开始")
            End If
            Logger.Info(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\Worker\RedoableWorker.cs")
            Dim started As Action = Me.Started
            If started IsNot Nothing Then
                started()
            End If
            While True
                Try
                    Me.realCts.Token.ThrowIfCancellationRequested()
                    Dim progress As ProgressProvider = Me.Progress
                    If progress IsNot Nothing Then
                        progress.Reset()
                    End If
                    Dim tout As TOut = Me.<workload>P(New CancellationToken?(Me.realCts.Token), Me.Progress)
                    Me.realCts.Token.ThrowIfCancellationRequested()
                    Dim redoableWorkerBase As RedoableWorkerBase(Of TOut) = Me
                    SyncLock redoableWorkerBase
                        Dim cancellationTokenSource As CancellationTokenSource = Me.realCts
                        If cancellationTokenSource IsNot Nothing Then
                            cancellationTokenSource.Dispose()
                        End If
                        If Me.pendingRedo Then
                            Me.pendingRedo = False
                            Me.realCts = CancellationTokenSource.CreateLinkedTokenSource(New CancellationToken() { Me.lastToken })
                            Continue For
                        End If
                        Me.realCts = Nothing
                        Me.lastSucceeded = True
                        Me.lastResult = tout
                        Me.hasSucceeded = True
                        Me.running = False
                        Me.idleEvent.[Set]()
                        Dim progress2 As ProgressProvider = Me.Progress
                        If progress2 IsNot Nothing Then
                            progress2.Finish(ProgressProvider.ChildrenAction.Finished)
                        End If
                    End SyncLock
                    Dim logInterpolatedStringHandler2 As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(5, 1, flag)
                    If flag Then
                        logInterpolatedStringHandler2.AppendFormatted(Me.<creatorMemberName>P)
                        logInterpolatedStringHandler2.AppendLiteral("：运行成功")
                    End If
                    Logger.Info(logInterpolatedStringHandler2, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\Worker\RedoableWorker.cs")
                    Dim succeeded As Action = Me.Succeeded
                    If succeeded IsNot Nothing Then
                        succeeded()
                    End If
                Catch ex As Exception
                    If Not ex.IsCanceled() Then
                        Logger.Log(ex, Me.<creatorMemberName>P + "：运行失败" + If(Me.pendingRedo, "，但即将重启，或可忽略", ""), If(Me.pendingRedo, LogLevel.Warn, LogLevel.[Error]), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\Worker\RedoableWorker.cs")
                    End If
                    Dim redoableWorkerBase As RedoableWorkerBase(Of TOut) = Me
                    SyncLock redoableWorkerBase
                        Dim cancellationTokenSource2 As CancellationTokenSource = Me.realCts
                        If cancellationTokenSource2 IsNot Nothing Then
                            cancellationTokenSource2.Dispose()
                        End If
                        If Me.pendingRedo Then
                            Dim flag2 As Boolean
                            Dim logInterpolatedStringHandler3 As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(5, 1, flag2)
                            If flag2 Then
                                logInterpolatedStringHandler3.AppendFormatted(Me.<creatorMemberName>P)
                                logInterpolatedStringHandler3.AppendLiteral("：重启开始")
                            End If
                            Logger.Info(logInterpolatedStringHandler3, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\Worker\RedoableWorker.cs")
                            Me.pendingRedo = False
                            Me.realCts = CancellationTokenSource.CreateLinkedTokenSource(New CancellationToken() { Me.lastToken })
                            Continue For
                        End If
                        Me.realCts = Nothing
                        Me.lastSucceeded = False
                        Me.running = False
                        Me.idleEvent.[Set]()
                        Dim progress3 As ProgressProvider = Me.Progress
                        If progress3 IsNot Nothing Then
                            progress3.Skip(ProgressProvider.ChildrenAction.Skiped)
                        End If
                    End SyncLock
                    If ex.IsCanceled() Then
                        Dim logInterpolatedStringHandler4 As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(6, 1, flag)
                        If flag Then
                            logInterpolatedStringHandler4.AppendFormatted(Me.<creatorMemberName>P)
                            logInterpolatedStringHandler4.AppendLiteral("：运行已取消")
                        End If
                        Logger.Info(logInterpolatedStringHandler4, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\Worker\RedoableWorker.cs")
                        Dim canceled As Action = Me.Canceled
                        If canceled IsNot Nothing Then
                            canceled()
                        End If
                    Else
                        Dim failed As Action(Of Exception) = Me.Failed
                        If failed IsNot Nothing Then
                            failed(ex)
                        End If
                    End If
                End Try
                Exit For
            End While
            Dim stopped As Action = Me.Stopped
            If stopped Is Nothing Then
                Return
            End If
            stopped()
        End Sub

        Public Sub Cancel() Implements MeloongCore.IWorker.Cancel
            SyncLock Me
                Me.pendingRedo = False
                Dim flag2 As Boolean
                Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(7, 1, flag2)
                If flag2 Then
                    logInterpolatedStringHandler.AppendFormatted(Me.<creatorMemberName>P)
                    logInterpolatedStringHandler.AppendLiteral("：接到取消请求")
                End If
                Logger.Info(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\Worker\RedoableWorker.cs")
                Dim cancellationTokenSource As CancellationTokenSource = Me.realCts
                If cancellationTokenSource IsNot Nothing Then
                    cancellationTokenSource.Cancel()
                End If
            End SyncLock
        End Sub

        Public Function WaitIfRunning(Optional millisecondsTimeout As Integer = -1, Optional cancellationToken As CancellationToken = Nothing) As Boolean Implements MeloongCore.IWorker.WaitIfRunning
            Return Me.idleEvent.Wait(millisecondsTimeout, cancellationToken)
        End Function

        <NullableContext(1)>
        Public Async Function WaitIfRunningAsync(Optional millisecondsTimeout As Integer = -1, Optional cancellationToken As CancellationToken = Nothing) As Task(Of Boolean) Implements MeloongCore.IWorker.WaitIfRunningAsync
            cancellationToken.ThrowIfCancellationRequested()
            Dim completionSource As TaskCompletionSource(Of Boolean) = New TaskCompletionSource(Of Boolean)(TaskCreationOptions.RunContinuationsAsynchronously)
            Dim waitHandle As RegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject(Me.idleEvent.WaitHandle, Sub(_ As Object, timedOut As Boolean)
                completionSource.TrySetResult(Not timedOut)
            End Sub, Nothing, millisecondsTimeout, True)
            Dim flag As Boolean
            Using cancellationToken.Register(Sub()
                completionSource.TrySetCanceled(cancellationToken)
            End Sub)
                Try
                    flag = Await completionSource.Task.NoCapture()
                Finally
                    waitHandle.Unregister(Nothing)
                End Try
            End Using
            Return flag
        End Function

        <Nullable(New Byte() { 1, 2, 1 })>
        <CompilerGenerated()>
        Private <workload>P As Func(Of CancellationToken?, ProgressProvider, TOut) = workload

        <Nullable(1)>
        <CompilerGenerated()>
        Private <creatorMemberName>P As String = creatorMemberName

        Private running As Boolean

        Private hasSucceeded As Boolean

        Private lastSucceeded As Boolean

        Private lastResult As Object

        Private pendingRedo As Boolean

        Private realCts As CancellationTokenSource

        Private lastToken As CancellationToken

        <Nullable(1)>
        Private idleEvent As ManualResetEventSlim = New ManualResetEventSlim(True)
    End Class
End Namespace
