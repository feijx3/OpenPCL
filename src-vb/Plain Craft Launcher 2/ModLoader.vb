Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows.Shell
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public NotInheritable Module ModLoader
        Public Sub LoaderTaskbarAdd(Of T)(Loader As ModLoader.LoaderCombo(Of T))
            If ModMain.FrmSpeedLeft IsNot Nothing Then
                ModMain.FrmSpeedLeft.TaskRemove(Loader)
            End If
            ModLoader.LoaderTaskbar.Add(Loader)
            Logger.Info(String.Format("{0} 已加入任务列表", Loader.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
        End Sub

        Public Sub LoaderTaskbarProgressRefresh()
            Try
                Dim num As Double = ModLoader.LoaderTaskbarProgressGet()
                Try
                    For Each loaderBase As ModLoader.LoaderBase In ModLoader.LoaderTaskbar
                        Dim loaderTaskbar As IEnumerable(Of ModLoader.LoaderBase) = ModLoader.LoaderTaskbar
                        Dim func As Func(Of ModLoader.LoaderBase, Boolean)
                        If ModLoader._Closure$__.$I9-0 IsNot Nothing Then
                            func = ModLoader._Closure$__.$I9-0
                        Else
                            Dim func2 As Func(Of ModLoader.LoaderBase, Boolean) = Function(l As ModLoader.LoaderBase) l.State <> ModBase.LoadState.Loading
                            func = func2
                            ModLoader._Closure$__.$I9-0 = func2
                        End If
                        If loaderTaskbar.All(func) OrElse loaderBase.State = ModBase.LoadState.Waiting OrElse loaderBase.State = ModBase.LoadState.Canceled Then
                            Dim frmSpeedLeft As PageSpeedLeft = ModMain.FrmSpeedLeft
                            If frmSpeedLeft IsNot Nothing Then
                                frmSpeedLeft.TaskRefresh(loaderBase)
                            End If
                            ModLoader.LoaderTaskbar.Remove(loaderBase)
                            Logger.Info(String.Format("{0} 已移出任务列表", loaderBase.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of ModLoader.LoaderBase)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                If num <= 0.0 OrElse num >= 1.0 OrElse ModLoader.LoaderTaskbarProgress > num Then
                    ModLoader.LoaderTaskbarProgress = num
                Else
                    ModLoader.LoaderTaskbarProgress = ModLoader.LoaderTaskbarProgress * 0.9 + num * 0.1
                End If
                Dim action As Action
                If ModLoader._Closure$__.$I9-1 IsNot Nothing Then
                    action = ModLoader._Closure$__.$I9-1
                Else
                    Dim action2 As Action = Sub()
                        ModMain.FrmMain.BtnExtraDownload.Progress = ModLoader.LoaderTaskbarProgress
                    End Sub
                    action = action2
                    ModLoader._Closure$__.$I9-1 = action2
                End If
                ModBase.RunInUi(action, False)
                Dim taskbarItemProgressState As TaskbarItemProgressState
                If Not ModLoader.LoaderTaskbar.Any() OrElse ModLoader.LoaderTaskbarProgress = 1.0 Then
                    taskbarItemProgressState = TaskbarItemProgressState.None
                ElseIf ModLoader.LoaderTaskbarProgress < 0.015 Then
                    taskbarItemProgressState = TaskbarItemProgressState.Indeterminate
                Else
                    taskbarItemProgressState = TaskbarItemProgressState.Normal
                    ModMain.FrmMain.TaskbarItemInfo.ProgressValue = ModLoader.LoaderTaskbarProgress
                End If
                If ModLoader.LoaderTaskbarProgressLast <> taskbarItemProgressState Then
                    ModLoader.LoaderTaskbarProgressLast = taskbarItemProgressState
                    ModMain.FrmMain.TaskbarItemInfo.ProgressState = taskbarItemProgressState
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                End If
            Catch ex As Exception
                Logger.[Error](ex, "刷新任务栏进度显示失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
            End Try
        End Sub

        Public Function LoaderTaskbarProgressGet() As Double
            Dim num As Double
            Try
                If Not ModLoader.LoaderTaskbar.Any() Then
                    num = 1.0
                Else
                    Dim loaderTaskbar As IEnumerable(Of ModLoader.LoaderBase) = ModLoader.LoaderTaskbar
                    Dim func As Func(Of ModLoader.LoaderBase, Double)
                    If ModLoader._Closure$__.$I10-0 IsNot Nothing Then
                        func = ModLoader._Closure$__.$I10-0
                    Else
                        Dim func2 As Func(Of ModLoader.LoaderBase, Double) = Function(l As ModLoader.LoaderBase) l.Progress
                        func = func2
                        ModLoader._Closure$__.$I10-0 = func2
                    End If
                    num = loaderTaskbar.[Select](func).Average().Clamp(0.0, 1.0)
                End If
            Catch ex As Exception
                Logger.[Error](ex, "获取任务栏进度出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                num = 0.5
            End Try
            Return num
        End Function

        Public Function LoaderFolderRun(Loader As ModLoader.LoaderBase, FolderPath As String, Type As ModLoader.LoaderFolderRunType, Optional MaxDepth As Integer = 0, Optional ExtraPath As String = "", Optional WaitForExit As Boolean = False) As Boolean
            Dim loaderFolderDictionaryEntry As ModLoader.LoaderFolderDictionaryEntry = Nothing
            loaderFolderDictionaryEntry.FolderPath = FolderPath + ExtraPath
            loaderFolderDictionaryEntry.LastCheckTime = Nothing
            Try
                Dim info As DirectoryInfo = DirectoryUtils.GetInfo(FolderPath + ExtraPath)
                loaderFolderDictionaryEntry.LastCheckTime = If(info.Exists, New DateTime?(ModLoader.GetActualLastWriteTimeUtc(info, MaxDepth)), CType(Nothing, DateTime?))
                If Type = ModLoader.LoaderFolderRunType.RunOnUpdated AndAlso ModLoader.LoaderFolderDictionary.ContainsKey(Loader) Then
                    If info.Exists Then
                        Dim loaderFolderDictionaryEntry2 As ModLoader.LoaderFolderDictionaryEntry = ModLoader.LoaderFolderDictionary(Loader)
                        If loaderFolderDictionaryEntry2.LastCheckTime IsNot Nothing AndAlso loaderFolderDictionaryEntry.Equals(ModLoader.LoaderFolderDictionary(Loader)) Then
                            Return False
                        End If
                    Else
                        Dim loaderFolderDictionaryEntry2 As ModLoader.LoaderFolderDictionaryEntry = ModLoader.LoaderFolderDictionary(Loader)
                        If loaderFolderDictionaryEntry2.LastCheckTime Is Nothing Then
                            Return False
                        End If
                    End If
                End If
            Catch ex As Exception
                Logger.Warn(ex, "文件夹加载器启动检测出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
            End Try
            ModLoader.LoaderFolderDictionary(Loader) = loaderFolderDictionaryEntry
            Dim flag As Boolean
            If Type = ModLoader.LoaderFolderRunType.UpdateOnly Then
                flag = False
            Else
                If WaitForExit Then
                    Loader.WaitForExit(FolderPath, Nothing, True, Nothing, Nothing)
                Else
                    Loader.Start(FolderPath, True)
                End If
                flag = True
            End If
            Return flag
        End Function

        Private Function GetActualLastWriteTimeUtc(FolderInfo As DirectoryInfo, MaxDepth As Integer) As DateTime
            Dim dateTime As DateTime = FolderInfo.LastWriteTimeUtc
            If MaxDepth > 0 Then
                Try
                    For Each directoryInfo As DirectoryInfo In FolderInfo.EnumerateDirectories()
                        ' The following expression was wrapped in a checked-expression
                        Dim actualLastWriteTimeUtc As DateTime = ModLoader.GetActualLastWriteTimeUtc(directoryInfo, MaxDepth - 1)
                        If DateTime.Compare(actualLastWriteTimeUtc, dateTime) > 0 Then
                            dateTime = actualLastWriteTimeUtc
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of DirectoryInfo)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
            End If
            Return dateTime
        End Function

        Public LoaderTaskbar As ConcurrentList(Of ModLoader.LoaderBase) = New ConcurrentList(Of ModLoader.LoaderBase)()

        Public LoaderTaskbarProgress As Double = 0.0

        Private LoaderTaskbarProgressLast As TaskbarItemProgressState = TaskbarItemProgressState.None

        Private LoaderFolderDictionary As Dictionary(Of ModLoader.LoaderBase, ModLoader.LoaderFolderDictionaryEntry) = New Dictionary(Of ModLoader.LoaderBase, ModLoader.LoaderFolderDictionaryEntry)()

        Public MustInherit Class LoaderBase
            Implements ILoadingTrigger

            Protected Sub New()
                Me.IsLoader = True
                Me.Uuid = ModBase.GetUuid()
                Me.Name = "未命名任务 " + Conversions.ToString(Me.Uuid) + "#"
                Me.LockState = RuntimeHelpers.GetObjectValue(New Object())
                Me.Parent = Nothing
                Me.HasOnStateChangedThread = False
                Me._State = ModBase.LoadState.Waiting
                Me._LoadingState = MyLoading.MyLoadingState.[Stop]
                Me.[Error] = Nothing
                Me.Block = True
                Me.Show = True
                Me.IsForceRestarting = False
                Me._Progress = -1.0
                Me.ProgressWeight = 1.0
            End Sub

            Public ReadOnly Property IsLoader As Boolean Implements PCL.ILoadingTrigger.IsLoader

            Public ReadOnly Property RealParent As ModLoader.LoaderBase
                Get
                    Dim loaderBase As ModLoader.LoaderBase
                    Try
                        loaderBase = Me.Parent
                        While loaderBase IsNot Nothing AndAlso loaderBase.Parent IsNot Nothing
                            loaderBase = loaderBase.Parent
                        End While
                    Catch ex As Exception
                        Logger.[Error](ex, String.Format("获取父加载器失败（{0}）", Me.Name), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                        loaderBase = Nothing
                    End Try
                    Return loaderBase
                End Get
            End Property

            Public Overridable Sub InitParent(Parent As ModLoader.LoaderBase)
                Me.Parent = Parent
            End Sub

            Public Event OnStateChangedThread As ModLoader.LoaderBase.OnStateChangedThreadEventHandler

            Public Event OnStateChangedUi As ModLoader.LoaderBase.OnStateChangedUiEventHandler

            Public WriteOnly Property OnStateChanged As Action(Of ModLoader.LoaderBase)
                Set(value As Action(Of ModLoader.LoaderBase))
                    AddHandler Me.OnStateChangedUi, Sub(Loader As ModLoader.LoaderBase, NewState As ModBase.LoadState, OldState As ModBase.LoadState)
                        value(Loader)
                    End Sub
                End Set
            End Property

            Public Event PreviewFinish As ModLoader.LoaderBase.PreviewFinishEventHandler

            Protected Sub RaisePreviewFinish()
                Dim previewFinishEvent As ModLoader.LoaderBase.PreviewFinishEventHandler = Me.PreviewFinishEvent
                If previewFinishEvent IsNot Nothing Then
                    previewFinishEvent(Me)
                End If
            End Sub

            Public Property State As ModBase.LoadState
                Get
                    Return Me._State
                End Get
                Set(value As ModBase.LoadState)
                    If Me._State <> value Then
                        Dim OldState As ModBase.LoadState = Me._State
                        If value = ModBase.LoadState.Finished AndAlso Settings.[Get](Of Boolean)("SystemDebugDelay", Nothing) Then
                            Thread.Sleep(ModBase.RandomInteger(100, 2000))
                        End If
                        Me._State = value
                        If value = ModBase.LoadState.Canceled Then
                            Dim cancellationTokenSource As CancellationTokenSource = Me.CancellationTokenSource
                            If cancellationTokenSource IsNot Nothing Then
                                cancellationTokenSource.Cancel()
                            End If
                        End If
                        Logger.Log(String.Format("加载器 {0} {1} 状态改变：{2}", MyBase.[GetType]().Name, Me.Name, value), If((value = ModBase.LoadState.Failed), LogLevel.[Error], LogLevel.Info), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                        ModBase.RunInUi(Sub()
                            Dim $VB$Local_value As ModBase.LoadState = value
                            If $VB$Local_value <> ModBase.LoadState.Loading Then
                                If $VB$Local_value <> ModBase.LoadState.Failed Then
                                    Me.LoadingState = MyLoading.MyLoadingState.[Stop]
                                Else
                                    Me.LoadingState = MyLoading.MyLoadingState.[Error]
                                End If
                            Else
                                Me.LoadingState = MyLoading.MyLoadingState.Run
                            End If
                            Dim onStateChangedUiEvent As ModLoader.LoaderBase.OnStateChangedUiEventHandler = Me.OnStateChangedUiEvent
                            If onStateChangedUiEvent IsNot Nothing Then
                                onStateChangedUiEvent(Me, value, OldState)
                            End If
                        End Sub, False)
                        If Me.HasOnStateChangedThread Then
                            ModBase.RunInThread(Sub()
                                Dim onStateChangedThreadEvent As ModLoader.LoaderBase.OnStateChangedThreadEventHandler = Me.OnStateChangedThreadEvent
                                If onStateChangedThreadEvent IsNot Nothing Then
                                    onStateChangedThreadEvent(Me, value, OldState)
                                End If
                            End Sub)
                        End If
                    End If
                End Set
            End Property

            Public Property LoadingState As MyLoading.MyLoadingState Implements PCL.ILoadingTrigger.LoadingState
                Get
                    Return Me._LoadingState
                End Get
                Set(value As MyLoading.MyLoadingState)
                    If Me._LoadingState <> value Then
                        Dim loadingState As MyLoading.MyLoadingState = Me._LoadingState
                        Me._LoadingState = value
                        Dim loadingStateChangedEvent As ILoadingTrigger.LoadingStateChangedEventHandler = Me.LoadingStateChangedEvent
                        If loadingStateChangedEvent IsNot Nothing Then
                            loadingStateChangedEvent(value, loadingState)
                        End If
                    End If
                End Set
            End Property

            Public Event LoadingStateChanged As ILoadingTrigger.LoadingStateChangedEventHandler Implements PCL.ILoadingTrigger.LoadingStateChanged

            Public Property [Error] As Exception

            Public Overridable Property Progress As Double
                Get
                    Dim state As ModBase.LoadState = Me.State
                    Dim num As Double
                    If state <> ModBase.LoadState.Waiting Then
                        If state <> ModBase.LoadState.Loading Then
                            num = 1.0
                        Else
                            num = If((Me._Progress = -1.0), 0.0, Me._Progress)
                        End If
                    Else
                        num = 0.0
                    End If
                    Return num
                End Get
                Set(value As Double)
                    If Me._Progress <> value Then
                        Dim progress As Double = Me._Progress
                        Me._Progress = value
                        Dim progressChangedEvent As ILoadingTrigger.ProgressChangedEventHandler = Me.ProgressChangedEvent
                        If progressChangedEvent IsNot Nothing Then
                            progressChangedEvent(value, progress)
                        End If
                    End If
                End Set
            End Property

            Public Event ProgressChanged As ILoadingTrigger.ProgressChangedEventHandler Implements PCL.ILoadingTrigger.ProgressChanged

            Public Function CreateSyncProgressProvider(Optional FromPercentage As Double = 0.0, Optional ToPercentage As Double = 1.0) As ProgressProvider
                Dim Provider As ProgressProvider = New ProgressProvider()
                AddHandler Provider.ProgressChanged, Sub(<TupleElementNames(New String() { "actual", "skiped" })> ChangedProgress As ValueTuple(Of Double, Double))
                    Dim raw As ValueTuple(Of Double, Double) = Provider.GetRaw()
                    Me.Progress = FromPercentage + (raw.Item1 + raw.Item2) * (ToPercentage - FromPercentage)
                End Sub
                Return Provider
            End Function

            Public Property ProgressWeight As Double

            Public MustOverride Sub Start(Optional Input As Object = Nothing, Optional IsForceRestart As Boolean = False)

            Public MustOverride Sub Cancel()

            Public MustOverride Sub Failed(Ex As Exception)

            Public Function CreateCancellationToken() As CancellationToken
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                Dim token As CancellationToken
                SyncLock lockState
                    If Me.CancellationTokenSource Is Nothing OrElse Me.CancellationTokenSource.IsCancellationRequested Then
                        Me.CancellationTokenSource = New CancellationTokenSource()
                        Me.CancellationTokenSource.Token.Register(AddressOf Me.Cancel)
                    End If
                    If Me.State = ModBase.LoadState.Canceled Then
                        Me.CancellationTokenSource.Cancel()
                    End If
                    token = Me.CancellationTokenSource.Token
                End SyncLock
                Return token
            End Function

            Public Sub WaitForExit(Optional Input As Object = Nothing, Optional LoaderToSyncProgress As ModLoader.LoaderBase = Nothing, Optional IsForceRestart As Boolean = False, Optional c As CancellationToken = Nothing, Optional p As ProgressProvider = Nothing)
                Me.Start(RuntimeHelpers.GetObjectValue(Input), IsForceRestart)
                While Me.State = ModBase.LoadState.Loading
                    If LoaderToSyncProgress IsNot Nothing Then
                        LoaderToSyncProgress.Progress = Me.Progress
                    End If
                    If p IsNot Nothing Then
                        p.[Set](Me.Progress, False, ProgressProvider.ChildrenAction.Finished)
                    End If
                    c.ThrowIfCancellationRequested()
                    Thread.Sleep(10)
                End While
                If Me.State = ModBase.LoadState.Finished Then
                    Return
                End If
                If Me.State = ModBase.LoadState.Canceled Then
                    Throw New OperationCanceledException("加载器执行已中断")
                End If
                If Information.IsNothing(Me.[Error]) Then
                    Throw New Exception("未知错误！")
                End If
                Throw New Exception(Me.[Error].Message, Me.[Error])
            End Sub

            Public Sub WaitForExitTime(Timeout As Integer, Optional Input As Object = Nothing, Optional TimeoutMessage As String = "等待加载器执行超时。", Optional LoaderToSyncProgress As Object = Nothing, Optional IsForceRestart As Boolean = False)
                Me.Start(RuntimeHelpers.GetObjectValue(Input), IsForceRestart)
                While Me.State = ModBase.LoadState.Loading
                    If LoaderToSyncProgress IsNot Nothing Then
                        NewLateBinding.LateSet(LoaderToSyncProgress, Nothing, "Progress", New Object() { Me.Progress }, Nothing, Nothing)
                    End If
                    Thread.Sleep(10)
                    Timeout -= 10
                    If Timeout < 0 Then
                        Throw New TimeoutException(TimeoutMessage)
                    End If
                End While
                If Me.State = ModBase.LoadState.Finished Then
                    Return
                End If
                If Me.State = ModBase.LoadState.Canceled Then
                    Throw New OperationCanceledException("加载器执行已中断")
                End If
                If Information.IsNothing(Me.[Error]) Then
                    Throw New Exception("未知错误！")
                End If
                Throw Me.[Error]
            End Sub

            Public Overrides Function Equals(obj As Object) As Boolean
                Dim loaderBase As ModLoader.LoaderBase = TryCast(obj, ModLoader.LoaderBase)
                Return loaderBase IsNot Nothing AndAlso Me.Uuid = loaderBase.Uuid
            End Function

            Public Uuid As Integer

            Public Name As String

            Public LockState As Object

            Public Parent As ModLoader.LoaderBase

            Public HasOnStateChangedThread As Boolean

            Private _State As ModBase.LoadState

            Private _LoadingState As MyLoading.MyLoadingState

            Public Block As Boolean

            Public Show As Boolean

            Public IsForceRestarting As Boolean

            Private _Progress As Double

            Private CancellationTokenSource As CancellationTokenSource

            Public Const WaitForExitTimeoutMessage As String = "等待加载器执行超时。"

            Public Delegate Sub OnStateChangedThreadEventHandler(Loader As ModLoader.LoaderBase, NewState As ModBase.LoadState, OldState As ModBase.LoadState)

            Public Delegate Sub OnStateChangedUiEventHandler(Loader As ModLoader.LoaderBase, NewState As ModBase.LoadState, OldState As ModBase.LoadState)

            Public Delegate Sub PreviewFinishEventHandler(Loader As ModLoader.LoaderBase)
        End Class

        Public Class LoaderTask(Of TIn, TOut)
            Inherits ModLoader.LoaderBase

            Public ReadOnly Property IsCanceled As Boolean
                Get
                    Return Me.IsCanceledWithThread(Thread.CurrentThread)
                End Get
            End Property

            Public Function IsCanceledWithThread(Thread As Thread) As Boolean
                Return Me.LastRunningThread Is Nothing OrElse Not Object.ReferenceEquals(Thread, Me.LastRunningThread) OrElse MyBase.State = ModBase.LoadState.Canceled
            End Function

            Public Function StartGetInput(Optional Input As TIn = Nothing, Optional InputDelegate As Func(Of TIn) = Nothing) As TIn
                Dim CS$<>8__locals1 As ModLoader.LoaderTask(Of TIn, TOut)._Closure$__11-0 = New ModLoader.LoaderTask(Of TIn, TOut)._Closure$__11-0()
                CS$<>8__locals1.$VB$Local_Input = Input
                CS$<>8__locals1.$VB$Local_InputDelegate = InputDelegate
                If CS$<>8__locals1.$VB$Local_InputDelegate Is Nothing Then
                    CS$<>8__locals1.$VB$Local_InputDelegate = CType(Me.InputDelegate, Func(Of TIn))
                End If
                Dim tin As TIn = Nothing
                If CS$<>8__locals1.$VB$Local_Input IsNot Nothing Then
                    If tin Is Nothing Then
                        GoTo IL_0094
                    End If
                    Dim CS$<>8__locals2 As ModLoader.LoaderTask(Of TIn, TOut)._Closure$__11-0 = CS$<>8__locals1
                    Dim ptr As __ByRef(Of TIn) = CS$<>8__locals2.$VB$Local_Input
                    If Nothing Is Nothing Then
                        Dim $VB$Local_Input As TIn = CS$<>8__locals2.$VB$Local_Input
                        ptr = $VB$Local_Input
                    End If
                    If Not ptr.Equals(tin) Then
                        GoTo IL_0094
                    End If
                End If
                If CS$<>8__locals1.$VB$Local_InputDelegate IsNot Nothing Then
                    ModBase.RunInUiWait(Sub()
                        CS$<>8__locals1.$VB$Local_Input = CS$<>8__locals1.$VB$Local_InputDelegate()
                    End Sub)
                End If
                IL_0094:
                Return CS$<>8__locals1.$VB$Local_Input
            End Function

            Public Function ShouldStart(ByRef Input As TIn, Optional IsForceRestart As Boolean = False, Optional IgnoreReloadTimeout As Boolean = False) As Boolean
                Try
                    Input = Me.StartGetInput(Input, Nothing)
                Catch ex As Exception
                    Logger.[Error](ex, String.Format("加载输入获取失败（{0}）", Me.Name), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                    MyBase.[Error] = ex
                    Dim lockState As Object = Me.LockState
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                    SyncLock lockState
                        MyBase.State = ModBase.LoadState.Failed
                    End SyncLock
                End Try
                Dim flag2 As Boolean
                If IsForceRestart Then
                    flag2 = True
                Else
                    If Input Is Nothing = (Me.Input Is Nothing) Then
                        If Input IsNot Nothing Then
                            Dim ptr As __ByRef(Of TIn) = Input
                            If Nothing Is Nothing Then
                                Dim tin As TIn = Input
                                ptr = tin
                            End If
                            If Not ptr.Equals(Me.Input) Then
                                GoTo IL_00D8
                            End If
                        End If
                        Return(MyBase.State <> ModBase.LoadState.Loading AndAlso MyBase.State <> ModBase.LoadState.Finished) OrElse (Not IgnoreReloadTimeout AndAlso Me.ReloadTimeout <> -1 AndAlso Me.LastFinishedTime <> 0L AndAlso ModBase.GetTimeMs() - Me.LastFinishedTime >= CLng(Me.ReloadTimeout))
                    End If
                    IL_00D8:
                    flag2 = True
                End If
                Return flag2
            End Function

            Public Overrides Sub Start(Optional Input As Object = Nothing, Optional IsForceRestart As Boolean = False)
                Dim tin As TIn = Conversions.ToGenericParameter(Of TIn)(Input)
                Dim flag As Boolean = Me.ShouldStart(tin, IsForceRestart, False)
                Input = tin
                If flag Then
                    If MyBase.State = ModBase.LoadState.Loading Then
                        Me.TriggerThreadInterrupt()
                    End If
                    Me.Input = Conversions.ToGenericParameter(Of TIn)(Input)
                    Dim lockState As Object = Me.LockState
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                    SyncLock lockState
                        MyBase.State = ModBase.LoadState.Loading
                        Me.Progress = -1.0
                    End SyncLock
                    Me.LastRunningThread = New Thread(Sub()
                        Try
                            Me.IsForceRestarting = IsForceRestart
                            Logger.Trace(Function() String.Format("加载线程 {0} 已{1}启动", Me.Name, If(Me.IsForceRestarting, "强制", "")), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                            Me.LoadDelegate(Me)
                            If Me.IsCanceled Then
                                Logger.Warn(String.Format("加载线程 {0} 已中断但线程正常运行至结束，输出被弃用（最新线程：{1}）", Me.Name, If((Me.LastRunningThread Is Nothing), (-1), Me.LastRunningThread.ManagedThreadId)), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                            Else
                                Logger.Trace(Function() String.Format("加载线程 {0} 已完成", Me.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                                Me.RaisePreviewFinish()
                                Me.State = ModBase.LoadState.Finished
                                Me.LastFinishedTime = ModBase.GetTimeMs()
                            End If
                        Catch ex As Exception
                            If ex.IsCanceled() Then
                                Logger.Trace(ex, String.Format("加载线程 {0} 已取消，当前已完成 {1}%", Me.Name, Math.Round(Me.Progress * 100.0)), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                                If Not Me.IsCanceled Then
                                    Me.State = ModBase.LoadState.Canceled
                                End If
                            Else
                                Me.Failed(ex)
                            End If
                        End Try
                    End Sub) With { .Name = "L/" + Me.Name, .Priority = Me.ThreadPriority }
                    Try
                        Me.LastRunningThread.Start()
                    Catch ex As ThreadStateException
                        Thread.Sleep(500)
                        Me.LastRunningThread.Start()
                    End Try
                End If
            End Sub

            Public Overrides Sub Failed(ex As Exception)
                MyBase.[Error] = ex
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    If Me.IsCanceled OrElse MyBase.State >= ModBase.LoadState.Finished Then
                        Return
                    End If
                    MyBase.State = ModBase.LoadState.Failed
                End SyncLock
                Logger.Warn(ex, String.Format("加载线程 {0} 出错，已完成 {1}%", Me.Name, Math.Round(Me.Progress * 100.0)), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                Me.TriggerThreadInterrupt()
            End Sub

            Public Overrides Sub Cancel()
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    If MyBase.State <> ModBase.LoadState.Loading Then
                        Return
                    End If
                    MyBase.State = ModBase.LoadState.Canceled
                End SyncLock
                Me.TriggerThreadInterrupt()
            End Sub

            Private Sub TriggerThreadInterrupt()
                If Me.LastRunningThread IsNot Nothing Then
                    If Me.LastRunningThread.IsAlive Then
                        Me.LastRunningThread.Interrupt()
                        Logger.Trace(Function() String.Format("加载线程 {0} 已中断", Me.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                    End If
                    Me.LastRunningThread = Nothing
                End If
            End Sub

            Public Sub New()
                Me.ReloadTimeout = -1
                Me.LastFinishedTime = 0L
                Me.LastRunningThread = Nothing
                Me.Input = Nothing
                Me.Output = Nothing
            End Sub

            Public Sub New(Name As String, LoadDelegate As Action(Of ModLoader.LoaderTask(Of TIn, TOut)), Optional InputDelegate As Func(Of TIn) = Nothing, Optional Priority As ThreadPriority = ThreadPriority.Normal)
                Me.ReloadTimeout = -1
                Me.LastFinishedTime = 0L
                Me.LastRunningThread = Nothing
                Me.Input = Nothing
                Me.Output = Nothing
                Me.Name = Name
                Me.LoadDelegate = LoadDelegate
                Me.InputDelegate = InputDelegate
                Me.ThreadPriority = Priority
            End Sub

            Protected Friend ThreadPriority As ThreadPriority

            Protected Friend LoadDelegate As Action(Of ModLoader.LoaderTask(Of TIn, TOut))

            Protected Friend InputDelegate As Func(Of Object)

            Public ReloadTimeout As Integer

            Public LastFinishedTime As Long

            Public LastRunningThread As Thread

            Public Input As TIn

            Public Output As TOut
        End Class

        Public Class LoaderWorker
            Inherits ModLoader.LoaderBase

            Public Overridable Property Worker As IWorker
                <CompilerGenerated()>
                Get
                    Return Me._Worker
                End Get
                <CompilerGenerated()>
                <MethodImpl(MethodImplOptions.Synchronized)>
                Set(value As IWorker)
                    Dim action As Action = AddressOf Me.Worker_Succeeded
                    Dim action2 As Action = AddressOf Me.Cancel
                    Dim action3 As Action(Of Exception) = AddressOf Me.Failed
                    Dim worker As IWorker = Me._Worker
                    If worker IsNot Nothing Then
                        RemoveHandler worker.Succeeded, action
                        RemoveHandler worker.Canceled, action2
                        RemoveHandler worker.Failed, action3
                    End If
                    Me._Worker = value
                    worker = Me._Worker
                    If worker IsNot Nothing Then
                        AddHandler worker.Succeeded, action
                        AddHandler worker.Canceled, action2
                        AddHandler worker.Failed, action3
                    End If
                End Set
            End Property

            Public Sub New(Name As String, Worker As IWorker)
                Me.Name = Name
                Me.Worker = Worker
                AddHandler Worker.Progress.ProgressChanged, Sub(<TupleElementNames(New String() { "actual", "skiped" })> Raw As ValueTuple(Of Double, Double))
                    Me.Progress = Raw.Item1 + Raw.Item2
                End Sub
            End Sub

            Public Overrides Sub Start(Optional Input As Object = Nothing, Optional IsForceRestart As Boolean = False)
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    MyBase.State = ModBase.LoadState.Loading
                End SyncLock
                Logger.Trace(Function() String.Format("Worker {0} 已启动", Me.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                Me.IsForceRestarting = IsForceRestart
                Me.Worker.Start(Nothing, Nothing)
            End Sub

            Private Sub Worker_Succeeded()
                Logger.Trace(Function() String.Format("Worker {0} 已完成", Me.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                MyBase.RaisePreviewFinish()
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    MyBase.State = ModBase.LoadState.Finished
                End SyncLock
            End Sub

            Public Overrides Sub Cancel()
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    If MyBase.State <> ModBase.LoadState.Loading Then
                        Return
                    End If
                    MyBase.State = ModBase.LoadState.Canceled
                End SyncLock
                Logger.Trace(Function() String.Format("Worker {0} 已取消，当前已完成 {1}%", Me.Name, Math.Round(Me.Progress * 100.0)), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                Me.Worker.Cancel()
            End Sub

            Public Overrides Sub Failed(Ex As Exception)
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    If MyBase.State >= ModBase.LoadState.Finished Then
                        Return
                    End If
                    MyBase.State = ModBase.LoadState.Failed
                End SyncLock
                Logger.Warn(Ex, String.Format("Worker {0} 出错，已完成 {1}%", Me.Name, Math.Round(Me.Progress * 100.0)), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                MyBase.[Error] = Ex
                Me.Worker.Cancel()
            End Sub
        End Class

        Public Class LoaderCombo(Of InputType)
            Inherits ModLoader.LoaderBase

            Public Overrides Property Progress As Double
                Get
                    Dim state As ModBase.LoadState = MyBase.State
                    Dim num As Double
                    If state <> ModBase.LoadState.Waiting Then
                        If state <> ModBase.LoadState.Loading Then
                            num = 1.0
                        Else
                            Dim num2 As Double = 0.0
                            Dim num3 As Double = 0.0
                            Try
                                For Each loaderBase As ModLoader.LoaderBase In Me.Loaders
                                    num2 += loaderBase.ProgressWeight
                                    num3 += loaderBase.ProgressWeight * loaderBase.Progress
                                Next
                            Finally
                                Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                                CType(enumerator, IDisposable).Dispose()
                            End Try
                            If num2 = 0.0 Then
                                num = 0.0
                            Else
                                num = num3 / num2
                            End If
                        End If
                    Else
                        num = 0.0
                    End If
                    Return num
                End Get
                Set(value As Double)
                    Throw New Exception("多重加载器不支持设置进度")
                End Set
            End Property

            Public Sub New(Name As String, Loaders As IEnumerable(Of ModLoader.LoaderBase))
                Me.Loaders = New List(Of ModLoader.LoaderBase)()
                Me.Loaders.Clear()
                Try
                    For Each loaderBase As ModLoader.LoaderBase In Loaders
                        If loaderBase IsNot Nothing Then
                            Me.Loaders.Add(loaderBase)
                            AddHandler loaderBase.OnStateChangedThread, Me.SubTaskStateChanged
                            loaderBase.HasOnStateChangedThread = True
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of ModLoader.LoaderBase)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                Me.InitParent(Nothing)
                Me.Name = Name
            End Sub

            Public Overrides Sub InitParent(Parent As ModLoader.LoaderBase)
                Me.Parent = Parent
                Try
                    For Each loaderBase As ModLoader.LoaderBase In Me.Loaders
                        loaderBase.InitParent(Me)
                    Next
                Finally
                    Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
            End Sub

            Public Overrides Sub Start(Optional Input As Object = Nothing, Optional IsForceRestart As Boolean = False)
                Me.IsForceRestarting = IsForceRestart
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    If MyBase.State = ModBase.LoadState.Loading Then
                        Return
                    End If
                    MyBase.State = ModBase.LoadState.Loading
                End SyncLock
                Me.Input = Conversions.ToGenericParameter(Of InputType)(Input)
                If IsForceRestart Then
                    Try
                        For Each loaderBase As ModLoader.LoaderBase In Me.Loaders
                            loaderBase.State = ModBase.LoadState.Waiting
                        Next
                    Finally
                        Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                End If
                ModBase.RunInThread(New Action(Me.Update))
            End Sub

            Public Overrides Sub Cancel()
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    If MyBase.State <> ModBase.LoadState.Loading AndAlso MyBase.State <> ModBase.LoadState.Waiting Then
                        Return
                    End If
                    MyBase.State = ModBase.LoadState.Canceled
                End SyncLock
                ModBase.RunInThread(Sub()
                    Try
                        For Each loaderBase As ModLoader.LoaderBase In Me.Loaders
                            loaderBase.Cancel()
                        Next
                    Finally
                        Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                End Sub)
            End Sub

            Public Overrides Sub Failed(Ex As Exception)
                MyBase.[Error] = Ex
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    If MyBase.State >= ModBase.LoadState.Finished Then
                        Return
                    End If
                    MyBase.State = ModBase.LoadState.Failed
                End SyncLock
                Try
                    For Each loaderBase As ModLoader.LoaderBase In Me.Loaders
                        loaderBase.Cancel()
                    Next
                Finally
                    Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
            End Sub

            Private Sub SubTaskStateChanged(Loader As ModLoader.LoaderBase, NewState As ModBase.LoadState, OldState As ModBase.LoadState)
                Select Case NewState
                    Case ModBase.LoadState.Waiting, ModBase.LoadState.Loading
                    Case ModBase.LoadState.Finished
                        Me.Update()
                        Return
                    Case ModBase.LoadState.Failed
                        Me.Failed(New Exception(Loader.Name + "失败", Loader.[Error]))
                    Case ModBase.LoadState.Canceled
                        Me.Cancel()
                        Return
                    Case Else
                        Return
                End Select
            End Sub

            Private Sub Update()
                If MyBase.State <> ModBase.LoadState.Finished AndAlso MyBase.State <> ModBase.LoadState.Failed AndAlso MyBase.State <> ModBase.LoadState.Canceled Then
                    Dim flag As Boolean = True
                    Dim flag2 As Boolean = False
                    Dim obj As Object = Me.Input
                    Try
                        For Each loaderBase As ModLoader.LoaderBase In Me.Loaders
                            Dim state As ModBase.LoadState = loaderBase.State
                            If state = ModBase.LoadState.Loading Then
                                If GeneralUtils.IsGenericInstanceOf(loaderBase, GetType(ModLoader.LoaderTask(Of, ))) Then
                                    Dim obj2 As Object = loaderBase
                                    Dim type As Type = Nothing
                                    Dim text As String = "ShouldStart"
                                    Dim array As Object() = New Object() { Nothing, If((obj IsNot Nothing AndAlso loaderBase.[GetType]().GenericTypeArguments.First() Is obj.[GetType]()), obj, Nothing) }
                                    array(0) = True
                                    If Conversions.ToBoolean(NewLateBinding.LateGet(obj2, type, text, array, New String() { "IgnoreReloadTimeout" }, Nothing, Nothing)) Then
                                        Logger.Warn(String.Format("由于输入条件变更，重启进行中的加载器 {0}", loaderBase.Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                                        GoTo IL_01B9
                                    End If
                                End If
                                flag = False
                                flag2 = True
                                Continue For
                            End If
                            If state = ModBase.LoadState.Finished Then
                                If GeneralUtils.IsGenericInstanceOf(loaderBase, GetType(ModLoader.LoaderTask(Of, ))) Then
                                    Dim obj3 As Object = loaderBase
                                    Dim type2 As Type = Nothing
                                    Dim text2 As String = "ShouldStart"
                                    Dim array2 As Object() = New Object() { Nothing, If((obj IsNot Nothing AndAlso loaderBase.[GetType]().GenericTypeArguments.First() Is obj.[GetType]()), obj, Nothing) }
                                    array2(0) = True
                                    If Conversions.ToBoolean(NewLateBinding.LateGet(obj3, type2, text2, array2, New String() { "IgnoreReloadTimeout" }, Nothing, Nothing)) Then
                                        Logger.Info(String.Format("由于输入条件变更，重启已完成的加载器 {0}", loaderBase.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModLoader.vb")
                                        GoTo IL_01B9
                                    End If
                                    obj = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(loaderBase, Nothing, "Output", New Object(-1) {}, Nothing, Nothing, Nothing))
                                End If
                                If loaderBase.Block AndAlso Not flag Then
                                    flag2 = True
                                    Continue For
                                End If
                                Continue For
                            End If
                            IL_01B9:
                            flag = False
                            If Not flag2 Then
                                If obj IsNot Nothing Then
                                    If GeneralUtils.IsGenericInstanceOf(loaderBase, GetType(ModLoader.LoaderTask(Of, ))) OrElse GeneralUtils.IsGenericInstanceOf(loaderBase, GetType(ModLoader.LoaderCombo(Of))) Then
                                        loaderBase.Start(RuntimeHelpers.GetObjectValue(If((loaderBase.[GetType]().GenericTypeArguments.First() Is obj.[GetType]()), obj, Nothing)), Me.IsForceRestarting)
                                    ElseIf TypeOf loaderBase Is ModLoader.LoaderWorker Then
                                        loaderBase.Start(Nothing, Me.IsForceRestarting)
                                    Else
                                        If Not(TypeOf loaderBase Is ModNet.LoaderDownload) Then
                                            Throw New Exception("未知的加载器类型（" + loaderBase.[GetType]().Name + "）")
                                        End If
                                        loaderBase.Start(RuntimeHelpers.GetObjectValue(If((TypeOf obj Is List(Of ModNet.NetFile)), obj, Nothing)), Me.IsForceRestarting)
                                    End If
                                Else
                                    loaderBase.Start(Nothing, Me.IsForceRestarting)
                                End If
                                If loaderBase.Block Then
                                    flag2 = True
                                End If
                            End If
                        Next
                    Finally
                        Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                    If flag Then
                        MyBase.RaisePreviewFinish()
                        MyBase.State = ModBase.LoadState.Finished
                        ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                    End If
                End If
            End Sub

            Public Shared Sub GetLoaderList(Loader As Object, ByRef List As List(Of ModLoader.LoaderBase), Optional RequireShow As Boolean = True)
                Try
                    For Each obj As Object In CType(NewLateBinding.LateGet(Loader, Nothing, "Loaders", New Object(-1) {}, Nothing, Nothing, Nothing), IEnumerable)
                        Dim objectValue As Object = RuntimeHelpers.GetObjectValue(obj)
                        If Conversions.ToBoolean(Conversions.ToBoolean(NewLateBinding.LateGet(objectValue, Nothing, "Show", New Object(-1) {}, Nothing, Nothing, Nothing)) OrElse Not RequireShow) Then
                            List.Add(CType(objectValue, ModLoader.LoaderBase))
                        End If
                        If GeneralUtils.IsGenericInstanceOf(RuntimeHelpers.GetObjectValue(objectValue), GetType(ModLoader.LoaderCombo(Of))) Then
                            ModLoader.LoaderCombo(Of InputType).GetLoaderList(RuntimeHelpers.GetObjectValue(objectValue), List, True)
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator
                    If TypeOf enumerator Is IDisposable Then
                        TryCast(enumerator, IDisposable).Dispose()
                    End If
                End Try
            End Sub

            Public Sub GetLoaderList(ByRef List As List(Of ModLoader.LoaderBase), Optional RequireShow As Boolean = True)
                ModLoader.LoaderCombo(Of InputType).GetLoaderList(Me, List, RequireShow)
            End Sub

            Public Function GetLoaderList(Optional RequireShow As Boolean = True) As List(Of ModLoader.LoaderBase)
                Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
                Me.GetLoaderList(list, RequireShow)
                Return list
            End Function

            Public Loaders As List(Of ModLoader.LoaderBase)

            Public Input As InputType
        End Class

        Private Structure LoaderFolderDictionaryEntry
            Public Overrides Function Equals(obj As Object) As Boolean
                Dim flag As Boolean
                If Not(TypeOf obj Is ModLoader.LoaderFolderDictionaryEntry) Then
                    flag = False
                Else
                    Dim loaderFolderDictionaryEntry As ModLoader.LoaderFolderDictionaryEntry = CType(obj, ModLoader.LoaderFolderDictionaryEntry)
                    flag = EqualityComparer(Of DateTime?).[Default].Equals(Me.LastCheckTime, loaderFolderDictionaryEntry.LastCheckTime) AndAlso Operators.CompareString(Me.FolderPath, loaderFolderDictionaryEntry.FolderPath, False) = 0
                End If
                Return flag
            End Function

            Public LastCheckTime As DateTime?

            Public FolderPath As String
        End Structure

        Public Enum LoaderFolderRunType
            RunOnUpdated
            ForceRun
            UpdateOnly
        End Enum
    End Module
End Namespace
