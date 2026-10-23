Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public NotInheritable Module ModWatcher
        Private Sub WatcherStateChanged()
            Dim flag As Boolean = False
            Dim flag2 As Boolean = True
            Try
                For Each watcher As ModWatcher.Watcher In ModWatcher.McWatcherList
                    If watcher.State = ModWatcher.Watcher.MinecraftState.Loading OrElse watcher.State = ModWatcher.Watcher.MinecraftState.Running OrElse (watcher.State = ModWatcher.Watcher.MinecraftState.Crashed AndAlso Not watcher.GameProcess.HasExited) Then
                        flag = True
                        Exit For
                    End If
                    If watcher.State = ModWatcher.Watcher.MinecraftState.Crashed OrElse watcher.State = ModWatcher.Watcher.MinecraftState.Canceled Then
                        flag2 = False
                    End If
                Next
            Finally
                Dim enumerator As List(Of ModWatcher.Watcher).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            If ModWatcher.IsWatcherRunning <> flag Then
                ModWatcher.IsWatcherRunning = flag
                If ModWatcher.IsWatcherRunning Then
                    ModWatcher.MinecraftStart()
                    Return
                End If
                ModWatcher.MinecraftStop(flag2)
            End If
        End Sub

        Private Sub MinecraftStart()
            ModLaunch.McLaunchLog("[全局] 出现运行中的 Minecraft", LogLevel.Info)
            ModWatcher.HasRunningMinecraft = True
            ModMain.FrmMain.BtnExtraShutdown.ShowRefresh()
        End Sub

        Private Sub MinecraftStop(TriggerLauncherShutdown As Boolean)
            ModLaunch.McLaunchLog("[全局] 已无运行中的 Minecraft", LogLevel.Info)
            ModWatcher.HasRunningMinecraft = False
            ModMain.FrmMain.BtnExtraShutdown.ShowRefresh()
            If Settings.[Get](Of Boolean)("UiMusicStop", Nothing) Then
                Dim action As Action
                If ModWatcher._Closure$__.$I6-0 IsNot Nothing Then
                    action = ModWatcher._Closure$__.$I6-0
                Else
                    Dim action2 As Action = Sub()
                        If ModMusic.MusicResume() Then
                            Logger.Info("已根据设置，在结束后开始音乐播放", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModWatcher.vb")
                        End If
                    End Sub
                    action = action2
                    ModWatcher._Closure$__.$I6-0 = action2
                End If
                ModBase.RunInUi(action, False)
            ElseIf Settings.[Get](Of Boolean)("UiMusicStart", Nothing) Then
                Dim action3 As Action
                If ModWatcher._Closure$__.$I6-1 IsNot Nothing Then
                    action3 = ModWatcher._Closure$__.$I6-1
                Else
                    Dim action4 As Action = Sub()
                        If ModMusic.MusicPause() Then
                            Logger.Info("已根据设置，在结束后暂停音乐播放", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModWatcher.vb")
                        End If
                    End Sub
                    action3 = action4
                    ModWatcher._Closure$__.$I6-1 = action4
                End If
                ModBase.RunInUi(action3, False)
            End If
            Dim num As Integer = Settings.[Get](Of Integer)("LaunchArgumentVisible", Nothing)
            If num <> 2 Then
                If num <> 3 Then
                    Return
                End If
                Dim action5 As Action
                If ModWatcher._Closure$__.$I6-4 IsNot Nothing Then
                    action5 = ModWatcher._Closure$__.$I6-4
                Else
                    Dim action6 As Action = Sub()
                        ModMain.FrmMain.Hidden = False
                    End Sub
                    action5 = action6
                    ModWatcher._Closure$__.$I6-4 = action6
                End If
                ModBase.RunInUi(action5, False)
                Return
            Else
                If TriggerLauncherShutdown Then
                    Dim action7 As Action
                    If ModWatcher._Closure$__.$I6-2 IsNot Nothing Then
                        action7 = ModWatcher._Closure$__.$I6-2
                    Else
                        Dim action8 As Action = Sub()
                            ModMain.FrmMain.EndProgram(False)
                        End Sub
                        action7 = action8
                        ModWatcher._Closure$__.$I6-2 = action8
                    End If
                    ModBase.RunInUi(action7, False)
                    Return
                End If
                Dim action9 As Action
                If ModWatcher._Closure$__.$I6-3 IsNot Nothing Then
                    action9 = ModWatcher._Closure$__.$I6-3
                Else
                    Dim action10 As Action = Sub()
                        ModMain.FrmMain.Hidden = False
                    End Sub
                    action9 = action10
                    ModWatcher._Closure$__.$I6-3 = action10
                End If
                ModBase.RunInUi(action9, False)
                Return
            End If
        End Sub

        Public McWatcherList As List(Of ModWatcher.Watcher) = New List(Of ModWatcher.Watcher)()

        Private IsWatcherRunning As Boolean = False

        Public HasRunningMinecraft As Boolean = False

        Public Class Watcher
            Public Sub New(Loader As ModLoader.LoaderTask(Of Process, Integer), Instance As ModMinecraft.McInstance, WindowTitle As String)
                Dim CS$<>8__locals1 As ModWatcher.Watcher._Closure$__5-0 = New ModWatcher.Watcher._Closure$__5-0(CS$<>8__locals1)
                CS$<>8__locals1.$VB$Local_Loader = Loader
                CS$<>8__locals1.$VB$Local_WindowTitle = WindowTitle
                MyBase..ctor()
                CS$<>8__locals1.$VB$Me = Me
                Me.WindowTitle = ""
                Me._State = ModWatcher.Watcher.MinecraftState.Loading
                Me.WaitingLog = New List(Of String)(1000)
                Me.WaitingLogLock = RuntimeHelpers.GetObjectValue(New Object())
                Me.LatestLog = New ConcurrentQueue(Of String)()
                Me.LogProgress = 0
                Me.IsWindowAppeared = False
                Me.IsWindowFinished = False
                Me.Loader = CS$<>8__locals1.$VB$Local_Loader
                Me.Instance = Instance
                Me.WindowTitle = CS$<>8__locals1.$VB$Local_WindowTitle
                Me.PID = CS$<>8__locals1.$VB$Local_Loader.Input.Id
                Me.WatcherLog("开始 Minecraft 日志监控", LogLevel.Info)
                If Operators.CompareString(Me.WindowTitle, "", False) <> 0 Then
                    Me.WatcherLog("要求窗口标题：" + CS$<>8__locals1.$VB$Local_WindowTitle, LogLevel.Info)
                End If
                Dim list As List(Of ModWatcher.Watcher) = New List(Of ModWatcher.Watcher)()
                Try
                    For Each watcher As ModWatcher.Watcher In ModWatcher.McWatcherList
                        If watcher.State <> ModWatcher.Watcher.MinecraftState.Crashed AndAlso watcher.State <> ModWatcher.Watcher.MinecraftState.Ended AndAlso watcher.State <> ModWatcher.Watcher.MinecraftState.Canceled Then
                            list.Add(watcher)
                        End If
                    Next
                Finally
                    Dim enumerator As List(Of ModWatcher.Watcher).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                list.Add(Me)
                ModWatcher.McWatcherList = list
                ModWatcher.WatcherStateChanged()
                Me.GameProcess = CS$<>8__locals1.$VB$Local_Loader.Input
                Me.GameProcess.BeginOutputReadLine()
                Me.GameProcess.BeginErrorReadLine()
                AddHandler Me.GameProcess.OutputDataReceived, AddressOf Me.LogReceived
                AddHandler Me.GameProcess.ErrorDataReceived, AddressOf Me.LogReceived
                ModBase.RunInNewThread(Sub()
                    Try
                        While CS$<>8__locals1.$VB$Me.State <> ModWatcher.Watcher.MinecraftState.Ended AndAlso CS$<>8__locals1.$VB$Me.State <> ModWatcher.Watcher.MinecraftState.Canceled AndAlso (CS$<>8__locals1.$VB$Local_Loader.State <> ModBase.LoadState.Canceled OrElse CS$<>8__locals1.$VB$Me.State = ModWatcher.Watcher.MinecraftState.Crashed) AndAlso (CS$<>8__locals1.$VB$Me.State <> ModWatcher.Watcher.MinecraftState.Crashed OrElse Not CS$<>8__locals1.$VB$Me.GameProcess.HasExited)
                            CS$<>8__locals1.$VB$Me.TimerWindow()
                            CS$<>8__locals1.$VB$Me.TimerLog()
                            Dim num As Integer = 1
                            Do
                                If CS$<>8__locals1.$VB$Me.IsWindowFinished AndAlso CS$<>8__locals1.$VB$Me.IsWindowAppeared AndAlso Operators.CompareString(CS$<>8__locals1.$VB$Local_WindowTitle, "", False) <> 0 AndAlso CS$<>8__locals1.$VB$Me.State = ModWatcher.Watcher.MinecraftState.Running AndAlso Not CS$<>8__locals1.$VB$Me.GameProcess.HasExited Then
                                    Dim text As String = CS$<>8__locals1.$VB$Local_WindowTitle.Replace("{date}", DateTime.Now.ToString("yyyy'/'M'/'d")).Replace("{time}", DateTime.Now.ToString("HH':'mm':'ss"))
                                    Dim num2 As Integer = CInt(CS$<>8__locals1.$VB$Me.WindowHandle)
                                    Dim text2 As String = New String(text.ToCharArray())
                                    ModWatcher.Watcher.SetWindowText(num2, text2)
                                End If
                                Thread.Sleep(64)
                                num += 1
                            Loop While num <= 3
                        End While
                        If CS$<>8__locals1.$VB$Me.State = ModWatcher.Watcher.MinecraftState.Crashed Then
                            ModWatcher.WatcherStateChanged()
                        End If
                        CS$<>8__locals1.$VB$Me.WatcherLog("Minecraft 日志监控已退出", LogLevel.Info)
                    Catch ex As Exception
                        Logger.[Error](ex, "Minecraft 日志监控主循环出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModWatcher.vb")
                        CS$<>8__locals1.$VB$Me.State = ModWatcher.Watcher.MinecraftState.Ended
                    End Try
                End Sub, "Minecraft Watcher PID " + Conversions.ToString(Me.PID), ThreadPriority.Normal)
            End Sub

            Public Property State As ModWatcher.Watcher.MinecraftState
                Get
                    Return Me._State
                End Get
                Set(value As ModWatcher.Watcher.MinecraftState)
                    If Me._State <> value Then
                        Me._State = value
                        ModWatcher.WatcherStateChanged()
                    End If
                End Set
            End Property

            Private Sub LogReceived(sender As Object, e As DataReceivedEventArgs)
                Dim waitingLogLock As Object = Me.WaitingLogLock
                ObjectFlowControl.CheckForSyncLockOnValueType(waitingLogLock)
                SyncLock waitingLogLock
                    Me.WaitingLog.Add(e.Data)
                End SyncLock
            End Sub

            Private Sub TimerLog()
                Try
                    Dim list As List(Of String) = New List(Of String)()
                    Dim waitingLogLock As Object = Me.WaitingLogLock
                    ObjectFlowControl.CheckForSyncLockOnValueType(waitingLogLock)
                    SyncLock waitingLogLock
                        If Not Me.WaitingLog.Any() Then
                            Return
                        End If
                        list = Me.WaitingLog
                        Me.WaitingLog = New List(Of String)(1000)
                    End SyncLock
                    Try
                        For Each text As String In list
                            Me.GameLog(text)
                        Next
                    Finally
                        Dim enumerator As List(Of String).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                    If Me.State = ModWatcher.Watcher.MinecraftState.Loading Then
                        Me.ProgressUpdate()
                    End If
                    If Me.GameProcess.HasExited Then
                        Me.WatcherLog("Minecraft 已退出，返回值：" + Conversions.ToString(Me.GameProcess.ExitCode), LogLevel.Info)
                        If Me.State = ModWatcher.Watcher.MinecraftState.Loading Then
                            Me.WatcherLog("Minecraft 尚未加载完成，可能已崩溃", LogLevel.[Error])
                            Me.Crashed()
                        ElseIf Me.GameProcess.ExitCode <> 0 AndAlso Me.State = ModWatcher.Watcher.MinecraftState.Running AndAlso Me.Instance.ReleaseTime.Year >= 2012 Then
                            Me.WatcherLog("Minecraft 返回值异常，可能已崩溃", LogLevel.[Error])
                            Me.Crashed()
                        ElseIf Me.State <> ModWatcher.Watcher.MinecraftState.Crashed Then
                            Me.State = ModWatcher.Watcher.MinecraftState.Ended
                        End If
                    End If
                Catch ex As Exception
                    Logger.[Error](ex, "输出 Minecraft 日志失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModWatcher.vb")
                End Try
            End Sub

            Private Sub GameLog(Text As String)
                If Text IsNot Nothing Then
                    Text = Text.ReplaceLineEndings(vbCrLf, False)
                    Me.LatestLog.Enqueue(Text)
                    If Me.LatestLog.Count >= 501 Then
                        Dim latestLog As ConcurrentQueue(Of String) = Me.LatestLog
                        Dim text As String = Nothing
                        latestLog.TryDequeue(text)
                    End If
                    Logger.Trace(Text, LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModWatcher.vb")
                    If Me.LogProgress < 1 Then
                        Me.WatcherLog("日志 1/5：已出现日志输出", LogLevel.Info)
                        Me.LogProgress = 1
                    End If
                    If Me.LogProgress < 2 AndAlso Text.Contains("Setting user:") Then
                        Me.WatcherLog("日志 2/5：游戏用户已设置", LogLevel.Info)
                        Me.LogProgress = 2
                    ElseIf Me.LogProgress < 3 AndAlso Text.ContainsIgnoreCase("lwjgl version") Then
                        Me.WatcherLog("日志 3/5：LWJGL 版本已确认", LogLevel.Info)
                        Me.LogProgress = 3
                    ElseIf Me.LogProgress < 4 AndAlso (Text.Contains("OpenAL initialized") OrElse Text.Contains("Starting up SoundSystem")) Then
                        Me.WatcherLog("日志 4/5：OpenAL 已加载", LogLevel.Info)
                        Me.LogProgress = 4
                    ElseIf Me.LogProgress < 5 AndAlso ((Text.Contains("Created") AndAlso Text.Contains("textures") AndAlso Text.Contains("-atlas")) OrElse Text.Contains("Found animation info")) Then
                        Me.WatcherLog("日志 5/5：材质已加载", LogLevel.Info)
                        Me.LogProgress = 5
                    End If
                    If Not Text.Contains("[CHAT]") Then
                        If Text.Contains("Someone is closing me!") OrElse Text.Contains("Restarting Minecraft with command") Then
                            Me.WatcherLog("识别为关闭的 Log：" + Text, LogLevel.Info)
                            Me.State = ModWatcher.Watcher.MinecraftState.Ended
                            Return
                        End If
                        If Text.Contains("Crash report saved to") OrElse Text.Contains("This crash report has been saved to:") Then
                            Me.WatcherLog("识别为崩溃的 Log：" + Text, LogLevel.[Error])
                            Me.Crashed()
                            Return
                        End If
                        If Text.Contains("Could not save crash report to") Then
                            Me.WatcherLog("识别为崩溃的 Log：" + Text, LogLevel.[Error])
                            Me.Crashed()
                            Return
                        End If
                        If Text.Contains("/ERROR]: Unable to launch") OrElse Text.Contains("An exception was thrown, the game will display an error screen and halt.") Then
                            Me.WatcherLog("识别为崩溃的 Log：" + Text, LogLevel.[Error])
                            Me.Crashed()
                        End If
                    End If
                End If
            End Sub

            Private Sub WatcherLog(Text As String, Optional Level As LogLevel = LogLevel.Info)
                ModLaunch.McLaunchLog("[" + Conversions.ToString(Me.PID) + "] " + Text, Level)
            End Sub

            Private Sub ProgressUpdate()
                Dim num As Double
                If Me.IsWindowAppeared OrElse Me.LogProgress = 5 Then
                    num = 0.95
                    Me.WatcherLog("Minecraft 加载已完成", LogLevel.Info)
                    Me.State = ModWatcher.Watcher.MinecraftState.Running
                Else
                    num = CDbl(Math.Min(Me.LogProgress, 3)) / 3.0 * 0.9
                End If
                Me.Loader.Progress = num
            End Sub

            Private Sub TimerWindow()
                Try
                    If Not Me.GameProcess.HasExited Then
                        If Not Me.IsWindowFinished Then
                            Dim keyValuePair As KeyValuePair(Of IntPtr, String)? = Nothing
                            Try
                                keyValuePair = Me.TryGetMinecraftWindow()
                            Catch ex As Win32Exception
                                Logger.[Error](ex, "由于反作弊或安全软件拦截，PCL 无法操作游戏窗口", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModWatcher.vb")
                                Me.IsWindowFinished = True
                            End Try
                            If keyValuePair IsNot Nothing Then
                                Dim value As String = keyValuePair.Value.Value
                                Dim MinecraftWindowHandle As IntPtr = keyValuePair.Value.Key
                                If Not value.StartsWithF("FML", False) AndAlso Not value.StartsWithF("Quilt Loader", False) Then
                                    Me.WindowHandle = MinecraftWindowHandle
                                    Me.WatcherLog(String.Format("Minecraft 窗口已加载：{0}（{1}）", value, MinecraftWindowHandle.ToInt64()), LogLevel.Info)
                                    Me.IsWindowFinished = True
                                    If Settings.[Get](Of Integer)("LaunchArgumentWindowType", Nothing) = 4 Then
                                        ModBase.RunInNewThread(Sub()
                                            Try
                                                Thread.Sleep(2000)
                                                ModWatcher.Watcher.ShowWindow(Me.WindowHandle, 3UI)
                                                Me.WatcherLog(String.Format("已最大化 Minecraft 窗口：{0}", MinecraftWindowHandle.ToInt64()), LogLevel.Info)
                                            Catch ex3 As Exception
                                                Logger.Warn(ex3, "最大化 Minecraft 窗口时出现错误", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModWatcher.vb")
                                            End Try
                                        End Sub, "MinecraftWindowMaximize", ThreadPriority.Normal)
                                    End If
                                ElseIf Not Me.IsWindowAppeared Then
                                    Me.WatcherLog(String.Concat(New String() { "Mod Loader 窗口已加载：", value, "（", Conversions.ToString(MinecraftWindowHandle.ToInt64()), "）" }), LogLevel.Info)
                                End If
                                Me.IsWindowAppeared = True
                            End If
                        End If
                    End If
                Catch ex2 As Exception
                    Logger.[Error](ex2, "检查 Minecraft 窗口失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModWatcher.vb")
                End Try
            End Sub

            Private Function TryGetMinecraftWindow() As KeyValuePair(Of IntPtr, String)?
                Dim Result As Object = Nothing
                ModWatcher.Watcher.EnumWindows(Sub(hwnd As IntPtr, lParam As Integer)
                    If Result Is Nothing Then
                        Dim stringBuilder As StringBuilder = New StringBuilder(512)
                        ModWatcher.Watcher.GetClassName(CInt(hwnd), stringBuilder, stringBuilder.Capacity)
                        Dim text As String = stringBuilder.ToString()
                        If(Operators.CompareString(text, "GLFW30", False) = 0 OrElse Operators.CompareString(text, "SDL_app", False) = 0 OrElse Operators.CompareString(text, "LWJGL", False) = 0 OrElse Operators.CompareString(text, "SunAwtFrame", False) = 0) AndAlso (Operators.CompareString(text, "SDL_app", False) <> 0 OrElse ModWatcher.Watcher.IsWindowVisible(hwnd)) Then
                            stringBuilder = New StringBuilder(512)
                            ModWatcher.Watcher.GetWindowText(CInt(hwnd), stringBuilder, stringBuilder.Capacity)
                            Dim text2 As String = stringBuilder.ToString()
                            If Not String.IsNullOrWhiteSpace(text2) AndAlso Operators.CompareString(text2, "SDL_app", False) <> 0 AndAlso Operators.CompareString(text2, "PopupMessageWindow", False) <> 0 AndAlso Not text2.StartsWithF("GLFW", False) AndAlso Not text2.EndsWithF("Hidden Utility Window", False) Then
                                Dim num As Integer
                                ModWatcher.Watcher.GetWindowThreadProcessId(hwnd, num)
                                Try
                                    If DateTime.Compare(Process.GetProcessById(num).StartTime, Me.GameProcess.StartTime) < 0 Then
                                        Return
                                    End If
                                Catch ex As Exception
                                    Logger.Warn(ex, "枚举 Minecraft 窗口进程失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModWatcher.vb")
                                    Return
                                End Try
                                Result = New KeyValuePair(Of IntPtr, String)(hwnd, text2)
                            End If
                        End If
                    End If
                End Sub, 0)
                Return CType(Result, KeyValuePair(Of IntPtr, String)?)
            End Function

            Private Declare Ansi Function EnumWindows Lib "user32" (hWnd As ModWatcher.Watcher.EnumWindowsSub, lParam As Integer) As Boolean

            Private Declare Ansi Function GetClassName Lib "user32" Alias "GetClassNameA" (hWnd As Integer, str As StringBuilder, maxCount As Integer) As Integer

            Private Declare Ansi Function GetWindowText Lib "user32" Alias "GetWindowTextA" (hWnd As Integer, str As StringBuilder, maxCount As Integer) As Integer

            Private Declare Ansi Function SetWindowText Lib "user32" Alias "SetWindowTextA" (hWnd As Integer, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef str As String) As Boolean

            Private Declare Ansi Function ShowWindow Lib "user32" (hWnd As IntPtr, cmdWindow As UInteger) As Boolean

            Private Declare Ansi Function IsWindowVisible Lib "user32" (hWnd As IntPtr) As Boolean

            Private Declare Ansi Function GetWindowThreadProcessId Lib "user32" (hWnd As IntPtr, ByRef lpdwProcessId As Integer) As Integer

            Private Sub Crashed()
                If Me.State <> ModWatcher.Watcher.MinecraftState.Crashed AndAlso Me.State <> ModWatcher.Watcher.MinecraftState.Ended Then
                    Me.State = ModWatcher.Watcher.MinecraftState.Crashed
                    Me.WatcherLog("Minecraft 已崩溃，将在 2 秒后开始崩溃分析", LogLevel.[Error])
                    ModMain.Hint("检测到 Minecraft 出现错误，错误分析已开始……", ModMain.HintType.Red, True)
                    ModMain.FeedbackInfo()
                    ModBase.RunInNewThread(Sub()
                        Try
                            Thread.Sleep(2000)
                            Me.WatcherLog("崩溃分析开始", LogLevel.Info)
                            Dim crashAnalyzer As CrashAnalyzer = New CrashAnalyzer(Me.Instance)
                            crashAnalyzer.Collect(Me.Instance.PathIndie, Me.LatestLog.ToList())
                            crashAnalyzer.Prepare()
                            crashAnalyzer.Analyze()
                            crashAnalyzer.Output(False, New List(Of String)() From { Me.Instance.PathVersion + Me.Instance.Name + ".json", Paths.Base + "PCL\Log1.txt", Paths.Base + "PCL\LatestLaunch.bat" })
                        Catch ex As Exception
                            Logger.[Error](ex, "崩溃分析失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModWatcher.vb")
                        End Try
                    End Sub, "Crash Analyzer", ThreadPriority.Normal)
                End If
            End Sub

            Public Sub Kill()
                Me.State = ModWatcher.Watcher.MinecraftState.Canceled
                Me.WatcherLog("尝试强制结束 Minecraft 进程", LogLevel.Info)
                Try
                    If Not Me.GameProcess.HasExited Then
                        Me.GameProcess.Kill()
                    End If
                    Me.WatcherLog("已强制结束 Minecraft 进程", LogLevel.Info)
                Catch ex As Exception
                    Logger.[Error](ex, "强制结束 Minecraft 进程失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModWatcher.vb")
                End Try
            End Sub

            Public GameProcess As Process

            Public Instance As ModMinecraft.McInstance

            Private WindowTitle As String

            Private PID As Integer

            Public Loader As ModLoader.LoaderTask(Of Process, Integer)

            Private _State As ModWatcher.Watcher.MinecraftState

            Public WaitingLog As List(Of String)

            Private WaitingLogLock As Object

            Public LatestLog As ConcurrentQueue(Of String)

            Private LogProgress As Integer

            Private IsWindowAppeared As Boolean

            Private IsWindowFinished As Boolean

            Private WindowHandle As IntPtr

            Public Enum MinecraftState
                Loading
                Running
                Crashed
                Ended
                Canceled
            End Enum

            Private Delegate Sub EnumWindowsSub(hwnd As IntPtr, lParam As Integer)
        End Class
    End Module
End Namespace
