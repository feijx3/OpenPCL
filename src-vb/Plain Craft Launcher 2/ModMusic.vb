Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports NAudio
Imports NAudio.Wave

Namespace PCL
    Public NotInheritable Module ModMusic
        Private Sub MusicListInit(ForceReload As Boolean, Optional PreventFirst As String = Nothing)
            If ForceReload Then
                ModMusic.MusicAllList = Nothing
            End If
            Try
                If ModMusic.MusicAllList Is Nothing Then
                    ModMusic.MusicAllList = New List(Of String)()
                    DirectoryUtils.Create(Paths.Base + "PCL\Musics\")
                    Try
                        For Each text As String In DirectoryUtils.EnumerateFiles(Paths.Base + "PCL\Musics\", True, "*")
                            If Not New String() { "ini", "jpg", "txt", "cfg", "lrc", "db", "png" }.Contains(PathUtils.GetExtension(text)) Then
                                ModMusic.MusicAllList.Add(text)
                            End If
                        Next
                    Finally
                        Dim enumerator As IEnumerator(Of String)
                        If enumerator IsNot Nothing Then
                            enumerator.Dispose()
                        End If
                    End Try
                End If
                ModMusic.MusicWaitingList = If(Settings.[Get](Of Boolean)("UiMusicRandom", Nothing), New List(Of String)(ModMusic.MusicAllList).Shuffle().ToList(), New List(Of String)(ModMusic.MusicAllList))
                If PreventFirst IsNot Nothing AndAlso Operators.CompareString(ModMusic.MusicWaitingList.FirstOrDefault(), PreventFirst, False) = 0 Then
                    ModMusic.MusicWaitingList.RemoveAt(0)
                    ModMusic.MusicWaitingList.Add(PreventFirst)
                End If
            Catch ex As Exception
                Logger.[Error](ex, "初始化音乐列表失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModMusic.vb")
            End Try
        End Sub

        Private Function DequeueNextMusicAddress() As String
            If ModMusic.MusicAllList Is Nothing OrElse Not ModMusic.MusicAllList.Any() OrElse Not ModMusic.MusicWaitingList.Any() Then
                ModMusic.MusicListInit(False, Nothing)
            End If
            Dim text As String
            If ModMusic.MusicWaitingList.Any() Then
                text = ModMusic.MusicWaitingList(0)
                ModMusic.MusicWaitingList.RemoveAt(0)
            Else
                text = Nothing
            End If
            If Not ModMusic.MusicWaitingList.Any() Then
                ModMusic.MusicListInit(False, text)
            End If
            Return text
        End Function

        Private Sub MusicRefreshUI()
            Dim action As Action
            If ModMusic._Closure$__.$I5-0 IsNot Nothing Then
                action = ModMusic._Closure$__.$I5-0
            Else
                Dim action2 As Action = Sub()
                    Try
                        If Not ModMusic.MusicAllList.Any() Then
                            ModMain.FrmMain.BtnExtraMusic.Show = False
                        Else
                            ModMain.FrmMain.BtnExtraMusic.Show = True
                            Dim text As String
                            If ModMusic.MusicState = ModMusic.MusicStates.Pause Then
                                ModMain.FrmMain.BtnExtraMusic.Logo = "M803.904 463.936a55.168 55.168 0 0 1 0 96.128l-463.616 264.448C302.848 845.888 256 819.136 256 776.448V247.616c0-42.752 46.848-69.44 84.288-48.064l463.616 264.384z"
                                ModMain.FrmMain.BtnExtraMusic.LogoScale = 0.8
                                text = "已暂停：" + PathUtils.GetFileNameWithoutExtension(ModMusic.MusicCurrent)
                                If ModMusic.MusicAllList.Count > 1 Then
                                    text += vbCrLf & "左键恢复播放，右键播放下一曲。"
                                Else
                                    text += vbCrLf & "左键恢复播放，右键重新从头播放。"
                                End If
                            Else
                                ModMain.FrmMain.BtnExtraMusic.Logo = "M348.293565 716.53287V254.797913c0-41.672348 28.004174-78.358261 68.919652-90.37913L815.994435 40.826435c62.775652-18.610087 125.907478 26.579478 125.907478 89.933913v539.158261c8.013913 42.25113-8.94887 89.177043-47.014956 127.109565a232.848696 232.848696 0 0 1-170.785392 65.758609c-61.885217-2.938435-111.081739-33.435826-129.113043-80.050087-18.031304-46.614261-2.137043-102.177391 41.672348-145.853218a232.848696 232.848696 0 0 1 170.785391-65.80313c21.014261 1.024 40.514783 5.164522 57.878261 12.065391V233.338435c0-12.109913-10.551652-20.034783-20.569044-20.034783a24.620522 24.620522 0 0 0-5.787826 0.934957L439.785739 338.18713a19.545043 19.545043 0 0 0-14.825739 19.144348v438.984348H423.846957c11.53113 43.987478-5.164522 94.208-45.412174 134.322087a232.848696 232.848696 0 0 1-170.785392 65.758609c-61.885217-2.938435-111.081739-33.435826-129.113043-80.050087-18.031304-46.614261-2.137043-102.177391 41.672348-145.853218a232.848696 232.848696 0 0 1 170.785391-65.80313c20.791652 1.024 40.069565 5.075478 57.299478 11.842783z"
                                ModMain.FrmMain.BtnExtraMusic.LogoScale = 1.0
                                text = "正在播放：" + PathUtils.GetFileNameWithoutExtension(ModMusic.MusicCurrent)
                                If ModMusic.MusicAllList.Count > 1 Then
                                    text += vbCrLf & "左键暂停，右键播放下一曲。"
                                Else
                                    text += vbCrLf & "左键暂停，右键重新从头播放。"
                                End If
                            End If
                            ModMain.FrmMain.BtnExtraMusic.ToolTip = text
                        End If
                        If ModMain.FrmSetupUI IsNot Nothing Then
                            ModMain.FrmSetupUI.MusicRefreshUI()
                        End If
                    Catch ex As Exception
                        Logger.[Error](ex, "刷新背景音乐 UI 失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModMusic.vb")
                    End Try
                End Sub
                action = action2
                ModMusic._Closure$__.$I5-0 = action2
            End If
            ModBase.RunInUi(action, False)
        End Sub

        Public Sub MusicControlPause()
            If ModMusic.MusicNAudio Is Nothing Then
                ModMain.Hint("音乐播放尚未开始！", ModMain.HintType.Red, True)
                Return
            End If
            Dim musicState As ModMusic.MusicStates = ModMusic.MusicState
            If musicState = ModMusic.MusicStates.Play Then
                ModMusic.MusicPause()
                Return
            End If
            If musicState = ModMusic.MusicStates.Pause Then
                ModMusic.MusicResume()
                Return
            End If
            Logger.Warn("音乐目前为停止状态，已强制尝试开始播放", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModMusic.vb")
            ModMusic.MusicRefreshPlay(False, False)
        End Sub

        Public Sub MusicControlNext()
            If ModMusic.MusicAllList.IsSingle() Then
                ModMusic.MusicStartPlay(ModMusic.MusicCurrent, False)
                ModMain.Hint("重新播放：" + PathUtils.GetLastPart(ModMusic.MusicCurrent), ModMain.HintType.Green, True)
            Else
                Dim text As String = ModMusic.DequeueNextMusicAddress()
                If text Is Nothing Then
                    ModMain.Hint("没有可以播放的音乐！", ModMain.HintType.Red, True)
                Else
                    ModMusic.MusicStartPlay(text, False)
                    ModMain.Hint("正在播放：" + PathUtils.GetLastPart(text), ModMain.HintType.Green, True)
                End If
            End If
            ModMusic.MusicRefreshUI()
        End Sub

        Public ReadOnly Property MusicState As ModMusic.MusicStates
            Get
                Dim musicStates As ModMusic.MusicStates
                If ModMusic.MusicNAudio Is Nothing Then
                    musicStates = ModMusic.MusicStates.[Stop]
                Else
                    Dim obj As Object = NewLateBinding.LateGet(ModMusic.MusicNAudio, Nothing, "PlaybackState", New Object(-1) {}, Nothing, Nothing, Nothing)
                    If Operators.ConditionalCompareObjectEqual(obj, 0, False) Then
                        musicStates = ModMusic.MusicStates.[Stop]
                    ElseIf Operators.ConditionalCompareObjectEqual(obj, 2, False) Then
                        musicStates = ModMusic.MusicStates.Pause
                    Else
                        musicStates = ModMusic.MusicStates.Play
                    End If
                End If
                Return musicStates
            End Get
        End Property

        Public Sub MusicRefreshPlay(ShowHint As Boolean, Optional IsFirstLoad As Boolean = False)
            Try
                ModMusic.MusicListInit(True, Nothing)
                If Not ModMusic.MusicAllList.Any() Then
                    If ModMusic.MusicNAudio Is Nothing Then
                        If ShowHint Then
                            ModMain.Hint("未检测到可用的背景音乐！", ModMain.HintType.Red, True)
                        End If
                    Else
                        ModMusic.MusicNAudio = Nothing
                        If ShowHint Then
                            ModMain.Hint("背景音乐已清除！", ModMain.HintType.Green, True)
                        End If
                    End If
                Else
                    Dim text As String = ModMusic.DequeueNextMusicAddress()
                    If text Is Nothing Then
                        If ShowHint Then
                            ModMain.Hint("没有可以播放的音乐！", ModMain.HintType.Red, True)
                        End If
                    Else
                        Try
                            ModMusic.MusicStartPlay(text, IsFirstLoad)
                            If ShowHint Then
                                ModMain.Hint("背景音乐已刷新：" + PathUtils.GetLastPart(text), ModMain.HintType.Green, False)
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                End If
                ModMusic.MusicRefreshUI()
            Catch ex2 As Exception
                Logger.[Error](ex2, "刷新背景音乐播放失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModMusic.vb")
            End Try
        End Sub

        Private Sub MusicStartPlay(Address As String, Optional IsFirstLoad As Boolean = False)
            If Address IsNot Nothing Then
                Logger.Info(String.Format("播放开始：{0}", Address), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModMusic.vb")
                ModMusic.MusicCurrent = Address
                ModBase.RunInNewThread(Sub()
                    ModMusic.MusicLoop(IsFirstLoad)
                End Sub, "Music", ThreadPriority.BelowNormal)
            End If
        End Sub

        Public Function MusicPause() As Boolean
            Dim flag As Boolean
            If ModMusic.MusicState = ModMusic.MusicStates.Play Then
                Dim action As Action
                If ModMusic._Closure$__.$I13-0 IsNot Nothing Then
                    action = ModMusic._Closure$__.$I13-0
                Else
                    Dim action2 As Action = Sub()
                        Logger.Info("已暂停播放", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModMusic.vb")
                        Dim musicNAudio As Object = ModMusic.MusicNAudio
                        If musicNAudio IsNot Nothing Then
                            NewLateBinding.LateCall(musicNAudio, Nothing, "Pause", New Object(-1) {}, Nothing, Nothing, Nothing, True)
                        End If
                        ModMusic.MusicRefreshUI()
                    End Sub
                    action = action2
                    ModMusic._Closure$__.$I13-0 = action2
                End If
                ModBase.RunInThread(action)
                flag = True
            Else
                Logger.Info(String.Format("无需暂停播放，当前状态为 {0}", ModMusic.MusicState), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModMusic.vb")
                flag = False
            End If
            Return flag
        End Function

        Public Function MusicResume() As Boolean
            Dim flag As Boolean
            If ModMusic.MusicState = ModMusic.MusicStates.Play OrElse Not ModMusic.MusicAllList.Any() Then
                Logger.Info(String.Format("无需继续播放，当前状态为 {0}", ModMusic.MusicState), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModMusic.vb")
                flag = False
            Else
                Dim action As Action
                If ModMusic._Closure$__.$I14-0 IsNot Nothing Then
                    action = ModMusic._Closure$__.$I14-0
                Else
                    Dim action2 As Action = Sub()
                        Logger.Info("已恢复播放", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModMusic.vb")
                        Try
                            Dim musicNAudio As Object = ModMusic.MusicNAudio
                            If musicNAudio IsNot Nothing Then
                                NewLateBinding.LateCall(musicNAudio, Nothing, "Play", New Object(-1) {}, Nothing, Nothing, Nothing, True)
                            End If
                        Catch ex As Exception
                            Dim musicNAudio2 As Object = ModMusic.MusicNAudio
                            If musicNAudio2 IsNot Nothing Then
                                NewLateBinding.LateCall(musicNAudio2, Nothing, "Stop", New Object(-1) {}, Nothing, Nothing, Nothing, True)
                            End If
                            Dim musicNAudio3 As Object = ModMusic.MusicNAudio
                            If musicNAudio3 IsNot Nothing Then
                                NewLateBinding.LateCall(musicNAudio3, Nothing, "Play", New Object(-1) {}, Nothing, Nothing, Nothing, True)
                            End If
                        End Try
                        ModMusic.MusicRefreshUI()
                    End Sub
                    action = action2
                    ModMusic._Closure$__.$I14-0 = action2
                End If
                ModBase.RunInThread(action)
                flag = True
            End If
            Return flag
        End Function

        Private Sub MusicLoop(Optional IsFirstLoad As Boolean = False)
            Dim waveOutEvent As WaveOutEvent = Nothing
            Dim waveStream As WaveStream = Nothing
            Try
                waveOutEvent = New WaveOutEvent()
                ModMusic.MusicNAudio = waveOutEvent
                waveOutEvent.DeviceNumber = -1
                Try
                    waveStream = New AudioFileReader(ModMusic.MusicCurrent)
                Catch ex As Exception
                    Logger.Warn(ex, "使用 AudioFileReader 加载音频文件失败，换用 MediaFoundationReader 重试", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModMusic.vb")
                    waveStream = New MediaFoundationReader(ModMusic.MusicCurrent)
                End Try
                waveOutEvent.Init(waveStream)
                waveOutEvent.Play()
                If IsFirstLoad AndAlso Not Settings.[Get](Of Boolean)("UiMusicAuto", Nothing) Then
                    waveOutEvent.Pause()
                End If
                ModMusic.MusicRefreshUI()
                Dim num As Integer = 0
                While waveOutEvent.Equals(RuntimeHelpers.GetObjectValue(ModMusic.MusicNAudio)) AndAlso waveOutEvent.PlaybackState IsNot Nothing
                    Dim CS$<>8__locals1 As ModMusic._Closure$__17-0 = New ModMusic._Closure$__17-0(CS$<>8__locals1)
                    If Settings.[Get](Of Integer)("UiMusicVolume", Nothing) <> num Then
                        num = Settings.[Get](Of Integer)("UiMusicVolume", Nothing)
                        waveOutEvent.Volume = CSng((CDbl(num) / 1000.0))
                    End If
                    CS$<>8__locals1.$VB$Local_Percent = waveStream.CurrentTime.TotalMilliseconds / waveStream.TotalTime.TotalMilliseconds
                    ModBase.RunInUi(Sub()
                        ModMain.FrmMain.BtnExtraMusic.Progress = CS$<>8__locals1.$VB$Local_Percent
                    End Sub, False)
                    Thread.Sleep(100)
                End While
                If waveOutEvent.PlaybackState Is Nothing AndAlso ModMusic.MusicAllList.Any() Then
                    ModMusic.MusicStartPlay(ModMusic.DequeueNextMusicAddress(), False)
                End If
            Catch ex2 As Exception
                Logger.Warn(ex2, String.Format("播放音乐出现内部错误（{0}）", ModMusic.MusicCurrent), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModMusic.vb")
                If TypeOf ex2 Is MmException AndAlso ex2.Message.Contains("AlreadyAllocated") Then
                    ModMain.Hint("你的音频设备正被其他程序占用。请在关闭占用的程序后重启 PCL，才能恢复音乐播放功能！", ModMain.HintType.Red, True)
                    Thread.Sleep(1000000000)
                End If
                If TypeOf ex2 Is MmException AndAlso (ex2.Message.Contains("NoDriver") OrElse ex2.Message.Contains("BadDeviceId")) Then
                    ModMain.Hint("由于音频设备变更，音乐播放功能在重启 PCL 后才能恢复！", ModMain.HintType.Red, True)
                    Thread.Sleep(1000000000)
                End If
                If Not New String() { "wav", "mp3", "flac" }.Contains(PathUtils.GetExtension(ModMusic.MusicCurrent)) OrElse ex2.Message.Contains("0xC00D36C4") Then
                    ModMain.Hint("播放音乐失败（" + PathUtils.GetLastPart(ModMusic.MusicCurrent) + "）：PCL 可能不支持此音乐格式，请将格式转换为 .wav、.mp3 或 .flac 后再试", ModMain.HintType.Red, True)
                Else
                    Logger.[Error](ex2, String.Format("播放音乐失败（{0}）", PathUtils.GetLastPart(ModMusic.MusicCurrent)), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModMusic.vb")
                End If
                ModMusic.MusicAllList.Remove(ModMusic.MusicCurrent)
                ModMusic.MusicWaitingList.Remove(ModMusic.MusicCurrent)
                ModMusic.MusicRefreshUI()
                Thread.Sleep(2000)
                If TypeOf ex2 Is FileNotFoundException Then
                    ModMusic.MusicRefreshPlay(True, IsFirstLoad)
                Else
                    ModMusic.MusicStartPlay(ModMusic.DequeueNextMusicAddress(), IsFirstLoad)
                End If
            Finally
                If waveOutEvent IsNot Nothing Then
                    waveOutEvent.Dispose()
                End If
                If waveStream IsNot Nothing Then
                    waveStream.Dispose()
                End If
                ModMusic.MusicRefreshUI()
            End Try
        End Sub

        Public MusicWaitingList As List(Of String) = Nothing

        Public MusicAllList As List(Of String) = Nothing

        Public MusicNAudio As Object = Nothing

        Private MusicCurrent As String = ""

        Public Enum MusicStates
            [Stop]
            Play
            Pause
        End Enum
    End Module
End Namespace
