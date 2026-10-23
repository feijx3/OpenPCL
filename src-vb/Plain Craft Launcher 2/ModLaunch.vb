Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Windows
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports PCL.My
Imports PCLCS

Namespace PCL
    Public NotInheritable Module ModLaunch
        Public Function McLaunchStart(Optional Options As ModLaunch.McLaunchOptions = Nothing) As Boolean
            ModLaunch.CurrentLaunchOptions = If(Options, New ModLaunch.McLaunchOptions())
            If Not ModBase.RunInUi() Then
                Throw New Exception("McLaunchStart 必须在 UI 线程调用！")
            End If
            Dim flag As Boolean
            If ModLaunch.McLaunchLoader.State = ModBase.LoadState.Loading Then
                ModMain.Hint("已有游戏正在启动中！", ModMain.HintType.Red, True)
                flag = False
            Else
                If ModLaunch.CurrentLaunchOptions.Instance IsNot Nothing AndAlso ModMinecraft.McInstanceSelected IsNot ModLaunch.CurrentLaunchOptions.Instance Then
                    ModLaunch.McLaunchLog("在启动前切换到版本 " + ModLaunch.CurrentLaunchOptions.Instance.Name, LogLevel.Info)
                    Try
                        ModLaunch.CurrentLaunchOptions.Instance.GetJsonPath()
                    Catch ex As Exception
                        ModMain.Hint(String.Format("无法启动 {0}：当前 Minecraft 文件夹中尚未安装该版本！", ModLaunch.CurrentLaunchOptions.Instance.Name), ModMain.HintType.Red, True)
                        Return False
                    End Try
                    ModLaunch.CurrentLaunchOptions.Instance.Load()
                    If ModLaunch.CurrentLaunchOptions.Instance.State = ModMinecraft.McInstance.McInstanceState.[Error] Then
                        ModMain.Hint(String.Format("无法启动 {0}：{1}", ModLaunch.CurrentLaunchOptions.Instance.Name, ModLaunch.CurrentLaunchOptions.Instance.Info), ModMain.HintType.Red, True)
                        Return False
                    End If
                    ModMinecraft.McInstanceSelected = ModLaunch.CurrentLaunchOptions.Instance
                    ModMain.FrmLaunchLeft.RefreshButtonsUI()
                    ModMain.FrmLaunchLeft.RefreshPage(False, False)
                End If
                If ModMinecraft.McInstanceSelected IsNot Nothing AndAlso ModMinecraft.McInstanceLock.IsLocked(ModMinecraft.McInstanceSelected) Then
                    ModMain.Hint(String.Format("版本 {0} 正在被修改，请稍后再启动！", ModMinecraft.McInstanceSelected.Name), ModMain.HintType.Red, True)
                    flag = False
                Else
                    ModMain.FrmMain.AprilGiveup()
                    Dim frmMain As FormMain = ModMain.FrmMain
                    Dim pageStack As IEnumerable(Of FormMain.PageStackData) = ModMain.FrmMain.PageStack
                    Dim func As Func(Of FormMain.PageStackData, Boolean)
                    If ModLaunch._Closure$__.$I3-0 IsNot Nothing Then
                        func = ModLaunch._Closure$__.$I3-0
                    Else
                        Dim func2 As Func(Of FormMain.PageStackData, Boolean) = Function(p As FormMain.PageStackData) p.Page <> FormMain.PageType.InstanceSelect
                        func = func2
                        ModLaunch._Closure$__.$I3-0 = func2
                    End If
                    frmMain.PageStack = pageStack.Where(func).ToList()
                    ModLaunch.McLaunchLoader.Start(Options, True)
                    flag = True
                End If
            End If
            Return flag
        End Function

        Public Sub McLaunchLog(Text As String, Optional Level As LogLevel = LogLevel.Info)
            Text = ModMinecraft.FilterUserName(ModMinecraft.FilterAccessToken(Text, "*"c), "*"c)
            If ModBase.ModeDebug Then
                ModBase.RunInUi(Sub()
                    ModMain.FrmLaunchRight.LabLog.AppendText(String.Format("[{0:HH':'mm':'ss'.'fff}] {1}{2}", DateTime.Now, Text, vbCrLf))
                End Sub, False)
            End If
            Logger.Log(Text, Level, LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
        End Sub

        Private Sub McLaunchState(Loader As ModLoader.LoaderTask(Of ModLaunch.McLaunchOptions, Object))
            Select Case ModLaunch.McLaunchLoader.State
                Case ModBase.LoadState.Waiting, ModBase.LoadState.Finished, ModBase.LoadState.Failed, ModBase.LoadState.Canceled
                    ModMain.FrmLaunchLeft.PageChangeToLogin()
                    Return
                Case ModBase.LoadState.Loading
                    ModMain.FrmLaunchRight.LabLog.Text = ""
                    Return
                Case Else
                    Return
            End Select
        End Sub

        Private Sub McLaunchStart(Loader As ModLoader.LoaderTask(Of ModLaunch.McLaunchOptions, Object))
            ModBase.RunInUiWait(AddressOf ModMain.FrmLaunchLeft.PageChangeToLaunching)
            Try
                ModLaunch.McLaunchPrecheck()
                ModLaunch.McLaunchLog("预检测已通过", LogLevel.Info)
            Catch ex As Exception
                If Not ex.IsCanceled() Then
                    ModMain.Hint(ex.Message, ModMain.HintType.Red, True)
                End If
                Throw
            End Try
            Dim currentLaunchOptions As ModLaunch.McLaunchOptions = ModLaunch.CurrentLaunchOptions
            Dim flag As Boolean = If((currentLaunchOptions IsNot Nothing), currentLaunchOptions.SaveBatch, Nothing) IsNot Nothing
            Try
                Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
                list.Add(New ModLoader.LoaderTask(Of Integer, Integer)("获取 Java", AddressOf ModLaunch.McLaunchJava, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 4.0, .Block = False })
                list.Add(ModLaunch.McLoginLoader)
                list.Add(New ModLoader.LoaderCombo(Of String)("补全文件", ModDownload.DlClientFix(ModMinecraft.McInstanceSelected, False, True)) With { .ProgressWeight = 15.0, .Show = False })
                list.Add(New ModLoader.LoaderTask(Of String, Integer)("获取启动参数", AddressOf ModLaunch.McLaunchArgumentMain, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 2.0 })
                list.Add(New ModLoader.LoaderTask(Of Integer, Integer)("解压文件", AddressOf ModLaunch.McLaunchNatives, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 2.0 })
                Dim text As String = "预启动处理"
                Dim action As Action(Of ModLoader.LoaderTask(Of Integer, Integer))
                If ModLaunch._Closure$__.$IR11-2 IsNot Nothing Then
                    action = ModLaunch._Closure$__.$IR11-2
                Else
                    Dim action2 As Action(Of ModLoader.LoaderTask(Of Integer, Integer)) = Sub(a0 As ModLoader.LoaderTask(Of Integer, Integer))
                        ModLaunch.McLaunchPrerun()
                    End Sub
                    action = action2
                    ModLaunch._Closure$__.$IR11-2 = action2
                End If
                list.Add(New ModLoader.LoaderTask(Of Integer, Integer)(text, action, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 1.0 })
                list.Add(New ModLoader.LoaderTask(Of Integer, Integer)("执行自定义命令", AddressOf ModLaunch.McLaunchCustom, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 1.0 })
                list.Add(New ModLoader.LoaderTask(Of Integer, Process)("启动进程", AddressOf ModLaunch.McLaunchRun, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 2.0 })
                list.Add(New ModLoader.LoaderTask(Of Process, Integer)("等待游戏窗口出现", AddressOf ModLaunch.McLaunchWait, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 1.0 })
                Dim text2 As String = "结束处理"
                Dim action3 As Action(Of ModLoader.LoaderTask(Of Integer, Integer))
                If ModLaunch._Closure$__.$IR11-3 IsNot Nothing Then
                    action3 = ModLaunch._Closure$__.$IR11-3
                Else
                    Dim action4 As Action(Of ModLoader.LoaderTask(Of Integer, Integer)) = Sub(a0 As ModLoader.LoaderTask(Of Integer, Integer))
                        ModLaunch.McLaunchEnd()
                    End Sub
                    action3 = action4
                    ModLaunch._Closure$__.$IR11-3 = action4
                End If
                list.Add(New ModLoader.LoaderTask(Of Integer, Integer)(text2, action3, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 1.0 })
                Dim list2 As List(Of ModLoader.LoaderBase) = list
                Select Case Settings.[Get](Of Integer)("VersionRamOptimize", ModMinecraft.McInstanceSelected)
                    Case 0
                        If Settings.[Get](Of Boolean)("LaunchArgumentRam", Nothing) Then
                            CType(list2(2), ModLoader.LoaderCombo(Of String)).Block = False
                            list2.Insert(3, New ModLoader.LoaderTask(Of Integer, Integer)("内存优化", AddressOf ModLaunch.McLaunchMemoryOptimize, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 30.0 })
                        End If
                    Case 1
                        CType(list2(2), ModLoader.LoaderCombo(Of String)).Block = False
                        list2.Insert(3, New ModLoader.LoaderTask(Of Integer, Integer)("内存优化", AddressOf ModLaunch.McLaunchMemoryOptimize, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 30.0 })
                End Select
                Dim loaderCombo As ModLoader.LoaderCombo(Of Object) = New ModLoader.LoaderCombo(Of Object)("Minecraft 启动", list2) With { .Show = False }
                If ModLaunch.McLoginLoader.State = ModBase.LoadState.Finished Then
                    ModLaunch.McLoginLoader.State = ModBase.LoadState.Waiting
                End If
                ModLaunch.McLaunchLoaderReal = loaderCombo
                ModLaunch.CanceledHint = Nothing
                loaderCombo.Start(Nothing, False)
                ModLoader.LoaderTaskbarAdd(Of Object)(loaderCombo)
                While loaderCombo.State = ModBase.LoadState.Loading
                    ModMain.FrmLaunchLeft.Dispatcher.Invoke(AddressOf ModMain.FrmLaunchLeft.LaunchingRefresh)
                    Thread.Sleep(100)
                End While
                ModMain.FrmLaunchLeft.Dispatcher.Invoke(AddressOf ModMain.FrmLaunchLeft.LaunchingRefresh)
                Select Case loaderCombo.State
                    Case ModBase.LoadState.Finished
                        ModMain.Hint(ModMinecraft.McInstanceSelected.Name + " 启动成功！", ModMain.HintType.Green, True)
                        If Not flag Then
                            ModMain.Telemetry("Minecraft 启动成功", New String() { "Version", ModMinecraft.McInstanceSelected.VersionDisplayName(), "LoginType", Settings.[Get](Of ModLaunch.McLoginType)("LoginType", Nothing).ToString() })
                        End If
                    Case ModBase.LoadState.Failed
                        Throw loaderCombo.[Error]
                    Case ModBase.LoadState.Canceled
                        If ModLaunch.CanceledHint Is Nothing Then
                            ModMain.Hint(If(flag, "已取消导出启动脚本！", "已取消启动！"), ModMain.HintType.Blue, True)
                        Else
                            ModMain.Hint(ModLaunch.CanceledHint, ModMain.HintType.Green, True)
                        End If
                    Case Else
                        Throw New Exception("错误的状态改变：" + loaderCombo.State.ToString())
                End Select
            Catch ex2 As Exception
                ex2.ThrowIfCanceled()
                Dim ex3 As Exception = ex2
                While Not ex3.Message.StartsWithF("$", False)
                    If ex3.InnerException Is Nothing Then
                        ModLaunch.McLaunchLog("错误：" + ex2.GetDisplay(True), LogLevel.Info)
                        Logger.[Error](ex2, If(flag, "导出启动脚本失败", "Minecraft 启动失败"), LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                        If Not flag Then
                            ModMain.Telemetry("Minecraft 启动失败", New String() { "Version", ModMinecraft.McInstanceSelected.VersionDisplayName(), "LoginType", Settings.[Get](Of ModLaunch.McLoginType)("LoginType", Nothing).ToString(), "Exception", ModMinecraft.FilterUserName(ModMinecraft.FilterAccessToken(ex2.GetDisplay(True), "*"c), "*"c) })
                        End If
                        Throw
                    End If
                    ex3 = ex3.InnerException
                End While
                ModMain.MyMsgBox(ex3.Message.TrimStart(New Char() { "$"c }), If(flag, "导出启动脚本失败", "启动失败"), "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
                Throw
            End Try
        End Sub

        Private Sub McLaunchMemoryOptimize(Loader As ModLoader.LoaderTask(Of Integer, Integer))
            Dim CS$<>8__locals1 As ModLaunch._Closure$__12-0 = New ModLaunch._Closure$__12-0(CS$<>8__locals1)
            ModLaunch.McLaunchLog("内存优化开始", LogLevel.Info)
            CS$<>8__locals1.$VB$Local_Finished = False
            ModBase.RunInNewThread(Sub()
                PageOtherTest.MemoryOptimize(False)
                CS$<>8__locals1.$VB$Local_Finished = True
            End Sub, "Launch Memory Optimize", ThreadPriority.Normal)
            While Not CS$<>8__locals1.$VB$Local_Finished AndAlso Not Loader.IsCanceled
                If Loader.Progress < 0.7 Then
                    Loader.Progress += 0.007
                Else
                    Loader.Progress += (0.95 - Loader.Progress) * 0.02
                End If
                Thread.Sleep(100)
            End While
        End Sub

        Private Sub McLaunchPrecheck()
            If Settings.[Get](Of Boolean)("SystemDebugDelay", Nothing) Then
                Thread.Sleep(ModBase.RandomInteger(100, 2000))
            End If
            If ModMinecraft.McInstanceSelected.PathIndie.Contains("!") OrElse ModMinecraft.McInstanceSelected.PathIndie.Contains(";") Then
                Throw New Exception("游戏路径中不可包含 ! 或 ;（" + ModMinecraft.McInstanceSelected.PathIndie + "）")
            End If
            If ModMinecraft.McInstanceSelected.PathVersion.Contains("!") OrElse ModMinecraft.McInstanceSelected.PathVersion.Contains(";") Then
                Throw New Exception("游戏路径中不可包含 ! 或 ;（" + ModMinecraft.McInstanceSelected.PathVersion + "）")
            End If
            If ModMinecraft.McInstanceSelected Is Nothing Then
                Throw New Exception("未选择 Minecraft 版本！")
            End If
            ModMinecraft.McInstanceSelected.Load()
            If ModMinecraft.McInstanceSelected.State = ModMinecraft.McInstance.McInstanceState.[Error] Then
                Throw New Exception("Minecraft 存在问题：" + ModMinecraft.McInstanceSelected.Info)
            End If
            Dim CheckResult As String = ""
            ModBase.RunInUiWait(Sub()
                CheckResult = ModLaunch.McLoginAble(ModLaunch.McLoginInput())
            End Sub)
            If Operators.CompareString(CheckResult, "", False) <> 0 Then
                Throw New ArgumentException(CheckResult)
            End If
            Dim currentLaunchOptions As ModLaunch.McLaunchOptions = ModLaunch.CurrentLaunchOptions
            If If((currentLaunchOptions IsNot Nothing), currentLaunchOptions.SaveBatch, Nothing) Is Nothing AndAlso Not Settings.[Get](Of Boolean)("HintBuy", Nothing) AndAlso Settings.[Get](Of ModLaunch.McLoginType)("LoginType", Nothing) <> ModLaunch.McLoginType.Ms Then
                If CultureInfo.CurrentCulture.Name.StartsWithF("zh", False) OrElse CultureInfo.CurrentUICulture.Name.StartsWithF("zh", False) Then
                    Dim action As Action
                    If ModLaunch._Closure$__.$I13-2 IsNot Nothing Then
                        action = ModLaunch._Closure$__.$I13-2
                    Else
                        Dim action2 As Action = Sub()
                            Dim num2 As Integer = Settings.[Get](Of Integer)("SystemLaunchCount", Nothing)
                            If num2 <= 280 Then
                                If num2 <= 70 Then
                                    If num2 <= 15 Then
                                        If num2 <> 3 AndAlso num2 <> 8 AndAlso num2 <> 15 Then
                                            Return
                                        End If
                                    ElseIf num2 <> 30 AndAlso num2 <> 50 AndAlso num2 <> 70 Then
                                        Return
                                    End If
                                ElseIf num2 <= 130 Then
                                    If num2 <> 90 AndAlso num2 <> 110 AndAlso num2 <> 130 Then
                                        Return
                                    End If
                                ElseIf num2 <> 180 AndAlso num2 <> 220 AndAlso num2 <> 280 Then
                                    Return
                                End If
                            ElseIf num2 <= 750 Then
                                If num2 <= 450 Then
                                    If num2 <> 330 AndAlso num2 <> 380 AndAlso num2 <> 450 Then
                                        Return
                                    End If
                                ElseIf num2 <> 550 AndAlso num2 <> 660 AndAlso num2 <> 750 Then
                                    Return
                                End If
                            ElseIf num2 <= 1100 Then
                                If num2 <> 880 AndAlso num2 <> 950 AndAlso num2 <> 1100 Then
                                    Return
                                End If
                            ElseIf num2 <= 1500 Then
                                If num2 <> 1300 AndAlso num2 <> 1500 Then
                                    Return
                                End If
                            ElseIf num2 <> 1700 AndAlso num2 <> 1900 Then
                                Return
                            End If
                            If ModMain.MyMsgBox("你已经启动了 " + Conversions.ToString(Settings.[Get](Of Integer)("SystemLaunchCount", Nothing)) + " 次 Minecraft 啦！" & vbCrLf & "如果觉得 Minecraft 还不错，可以购买正版支持一下，毕竟开发游戏也真的很不容易……不要一直白嫖啦。" & vbCrLf & vbCrLf & "在登录一次正版账号后，就不会再出现这个提示了！", "考虑一下正版？", "支持正版游戏！", "下次一定", "", False, True, False, Nothing, Nothing, Nothing) = 1 Then
                                ModBase.OpenWebsite("https://www.xbox.com/zh-cn/games/store/minecraft-java-bedrock-edition-for-pc/9nxp44l49shj")
                            End If
                        End Sub
                        action = action2
                        ModLaunch._Closure$__.$I13-2 = action2
                    End If
                    ModBase.RunInNewThread(action, "Buy Minecraft", ThreadPriority.Normal)
                    Return
                End If
                If Settings.[Get](Of ModLaunch.McLoginType)("LoginType", Nothing) = ModLaunch.McLoginType.Legacy Then
                    Dim text As String = "你必须先登录正版账号，才能进行离线登录！"
                    Dim text2 As String = "正版验证"
                    Dim text3 As String = "购买正版"
                    Dim text4 As String = "试玩"
                    Dim text5 As String = "返回"
                    Dim flag As Boolean = False
                    Dim flag2 As Boolean = True
                    Dim flag3 As Boolean = False
                    Dim action3 As Action
                    If ModLaunch._Closure$__.$I13-3 IsNot Nothing Then
                        action3 = ModLaunch._Closure$__.$I13-3
                    Else
                        Dim action4 As Action = Sub()
                            ModBase.OpenWebsite("https://www.xbox.com/zh-cn/games/store/minecraft-java-bedrock-edition-for-pc/9nxp44l49shj")
                        End Sub
                        action3 = action4
                        ModLaunch._Closure$__.$I13-3 = action4
                    End If
                    Dim num As Integer = ModMain.MyMsgBox(text, text2, text3, text4, text5, flag, flag2, flag3, action3, Nothing, Nothing)
                    If num = 2 Then
                        ModMain.Hint("游戏将以试玩模式启动！", ModMain.HintType.Red, True)
                        ModLaunch.CurrentLaunchOptions.ExtraGameArgs.Add("--demo")
                        Return
                    End If
                    If num <> 3 Then
                        Return
                    End If
                    Throw New OperationCanceledException()
                End If
            End If
        End Sub

        Public Function McLoginName() As String
            Select Case Settings.[Get](Of ModLaunch.McLoginType)("LoginType", Nothing)
                Case ModLaunch.McLoginType.Legacy
                    If Operators.CompareString(Settings.[Get](Of String)("LoginLegacyName", Nothing), "", False) <> 0 Then
                        Return Settings.[Get](Of String)("LoginLegacyName", Nothing).ToString().BeforeFirst("¨", False)
                    End If
                Case ModLaunch.McLoginType.Nide
                    If Operators.CompareString(Settings.[Get](Of String)("CacheNideName", Nothing), "", False) <> 0 Then
                        Return Settings.[Get](Of String)("CacheNideName", Nothing)
                    End If
                Case ModLaunch.McLoginType.Auth
                    If Operators.CompareString(Settings.[Get](Of String)("CacheAuthName", Nothing), "", False) <> 0 Then
                        Return Settings.[Get](Of String)("CacheAuthName", Nothing)
                    End If
                Case ModLaunch.McLoginType.Ms
                    If Operators.CompareString(Settings.[Get](Of String)("CacheMsV2Name", Nothing), "", False) <> 0 Then
                        Return Settings.[Get](Of String)("CacheMsV2Name", Nothing)
                    End If
            End Select
            Dim text As String
            If Operators.CompareString(Settings.[Get](Of String)("CacheMsV2Name", Nothing), "", False) <> 0 Then
                text = Settings.[Get](Of String)("CacheMsV2Name", Nothing)
            ElseIf Operators.CompareString(Settings.[Get](Of String)("CacheNideName", Nothing), "", False) <> 0 Then
                text = Settings.[Get](Of String)("CacheNideName", Nothing)
            ElseIf Operators.CompareString(Settings.[Get](Of String)("CacheAuthName", Nothing), "", False) <> 0 Then
                text = Settings.[Get](Of String)("CacheAuthName", Nothing)
            ElseIf Operators.CompareString(Settings.[Get](Of String)("LoginLegacyName", Nothing), "", False) <> 0 Then
                text = Settings.[Get](Of String)("LoginLegacyName", Nothing).ToString().BeforeFirst("¨", False)
            Else
                text = Nothing
            End If
            Return text
        End Function

        Public Function McLoginAble() As String
            Select Case Settings.[Get](Of ModLaunch.McLoginType)("LoginType", Nothing)
                Case ModLaunch.McLoginType.Legacy
                    Return ModMain.FrmLoginLegacy.IsVaild()
                Case ModLaunch.McLoginType.Nide
                    If Operators.CompareString(Settings.[Get](Of String)("CacheNideAccess", Nothing), "", False) = 0 Then
                        Return ModMain.FrmLoginNide.IsVaild()
                    End If
                    Return ""
                Case ModLaunch.McLoginType.Auth
                    If Operators.CompareString(Settings.[Get](Of String)("CacheAuthAccess", Nothing), "", False) = 0 Then
                        Return ModMain.FrmLoginAuth.IsVaild()
                    End If
                    Return ""
                Case ModLaunch.McLoginType.Ms
                    If Operators.CompareString(Settings.[Get](Of String)("CacheMsV2OAuthRefresh", Nothing), "", False) = 0 Then
                        Return ModMain.FrmLoginMs.IsVaild()
                    End If
                    Return ""
            End Select
            Return "未知的登录方式"
        End Function

        Public Function McLoginAble(LoginData As ModLaunch.McLoginData) As String
            Select Case LoginData.Type
                Case ModLaunch.McLoginType.Legacy
                    Return PageLoginLegacy.IsVaild(CType(LoginData, ModLaunch.McLoginLegacy))
                Case ModLaunch.McLoginType.Nide
                    Return PageLoginNide.IsVaild(CType(LoginData, ModLaunch.McLoginServer))
                Case ModLaunch.McLoginType.Auth
                    Return PageLoginAuth.IsVaild(CType(LoginData, ModLaunch.McLoginServer))
                Case ModLaunch.McLoginType.Ms
                    Return PageLoginMs.IsVaild(CType(LoginData, ModLaunch.McLoginMs))
            End Select
            Return "未知的登录方式"
        End Function

        Public Function McLoginInput() As ModLaunch.McLoginData
            Dim mcLoginData As ModLaunch.McLoginData = Nothing
            Dim mcLoginType As ModLaunch.McLoginType = Settings.[Get](Of ModLaunch.McLoginType)("LoginType", Nothing)
            Dim mcLoginData2 As ModLaunch.McLoginData
            Try
                Select Case mcLoginType
                    Case ModLaunch.McLoginType.Legacy
                        mcLoginData = PageLoginLegacy.GetLoginData()
                    Case ModLaunch.McLoginType.Nide
                        If Operators.CompareString(Settings.[Get](Of String)("CacheNideAccess", Nothing), "", False) = 0 Then
                            mcLoginData = PageLoginNide.GetLoginData()
                        Else
                            mcLoginData = PageLoginNideSkin.GetLoginData()
                        End If
                    Case ModLaunch.McLoginType.Auth
                        If Operators.CompareString(Settings.[Get](Of String)("CacheAuthAccess", Nothing), "", False) = 0 Then
                            mcLoginData = PageLoginAuth.GetLoginData()
                        Else
                            mcLoginData = PageLoginAuthSkin.GetLoginData()
                        End If
                    Case ModLaunch.McLoginType.Ms
                        If Operators.CompareString(Settings.[Get](Of String)("CacheMsV2OAuthRefresh", Nothing), "", False) = 0 Then
                            mcLoginData = PageLoginMs.GetLoginData()
                        Else
                            mcLoginData = PageLoginMsSkin.GetLoginData()
                        End If
                End Select
                mcLoginData2 = mcLoginData
            Catch ex As Exception
                Throw New Exception(String.Format("获取登录输入信息失败（{0}）", mcLoginType), ex)
            End Try
            Return mcLoginData2
        End Function

        Private Sub McLoginStart(Data As ModLoader.LoaderTask(Of ModLaunch.McLoginData, ModLaunch.McLoginResult))
            ModLaunch.McLaunchLog("登录加载已开始", LogLevel.Info)
            Dim text As String = ModLaunch.McLoginAble(Data.Input)
            If Operators.CompareString(text, "", False) <> 0 Then
                Throw New ArgumentException(text)
            End If
            Dim loaderBase As ModLoader.LoaderBase = Nothing
            Select Case Data.Input.Type
                Case ModLaunch.McLoginType.Legacy
                    loaderBase = ModLaunch.McLoginLegacyLoader
                Case ModLaunch.McLoginType.Nide
                    loaderBase = ModLaunch.McLoginNideLoader
                Case ModLaunch.McLoginType.Auth
                    loaderBase = ModLaunch.McLoginAuthLoader
                Case ModLaunch.McLoginType.Ms
                    loaderBase = ModLaunch.McLoginMsLoader
            End Select
            loaderBase.WaitForExit(Data.Input, ModLaunch.McLoginLoader, Data.IsForceRestarting, Nothing, Nothing)
            Dim obj As Object = NewLateBinding.LateGet(loaderBase, Nothing, "Output", New Object(-1) {}, Nothing, Nothing, Nothing)
            Data.Output = If((obj IsNot Nothing), CType(obj, ModLaunch.McLoginResult), Nothing)
            Dim action As Action
            If ModLaunch._Closure$__.$I25-0 IsNot Nothing Then
                action = ModLaunch._Closure$__.$I25-0
            Else
                Dim action2 As Action = Sub()
                    ModMain.FrmLaunchLeft.RefreshPage(True, False)
                End Sub
                action = action2
                ModLaunch._Closure$__.$I25-0 = action2
            End If
            ModBase.RunInUi(action, False)
            ModLaunch.McLaunchLog("登录加载已结束", LogLevel.Info)
        End Sub

        Private Sub McLoginMsStart(Data As ModLoader.LoaderTask(Of ModLaunch.McLoginMs, ModLaunch.McLoginResult))
            Dim input As ModLaunch.McLoginMs = Data.Input
            Dim userName As String = input.UserName
            ModLaunch.McLaunchLog("登录方式：正版（" + If((Operators.CompareString(userName, "", False) = 0), "尚未登录", userName) + "）", LogLevel.Info)
            Data.Progress = 0.05
            Dim num As Long = Settings.[Get](Of Long)("CacheMsV2Expires", Nothing)
            If Not Data.IsForceRestarting AndAlso num > 0L AndAlso num > ModBase.ToUnixTimestampUtc(DateTime.UtcNow) AndAlso Operators.CompareString(input.UserName, Settings.[Get](Of String)("CacheMsV2Name", Nothing), False) = 0 Then
                Data.Output = New ModLaunch.McLoginResult() With { .Name = input.UserName, .Type = "Microsoft", .AccessToken = Settings.[Get](Of String)("CacheMsV2Access", Nothing), .Uuid = Settings.[Get](Of String)("CacheMsV2Uuid", Nothing), .ClientToken = Settings.[Get](Of String)("CacheMsV2Uuid", Nothing), .ProfileJson = Settings.[Get](Of String)("CacheMsV2ProfileJson", Nothing) }
                ModLaunch.McLaunchLog("无需登录，AccessToken 尚未过期", LogLevel.Info)
            Else
                Dim valueTuple As ValueTuple(Of String, String)
                If Operators.CompareString(input.OAuthRefreshToken, "", False) <> 0 Then
                    valueTuple = ModLaunch.MsLoginStep1Refresh(input.OAuthRefreshToken)
                    If Operators.CompareString(valueTuple.Item1, "Relogin", False) <> 0 Then
                        GoTo IL_014F
                    End If
                End If
                valueTuple = ModLaunch.MsLoginStep1New(Data)
                IL_014F:
                If Data.IsCanceled Then
                    Throw New OperationCanceledException()
                End If
                Data.Progress = 0.25
                If Data.IsCanceled Then
                    Throw New OperationCanceledException()
                End If
                Dim item As String = valueTuple.Item1
                Dim item2 As String = valueTuple.Item2
                Dim text As String = ModLaunch.MsLoginStep2(item)
                Data.Progress = 0.4
                If Data.IsCanceled Then
                    Throw New OperationCanceledException()
                End If
                Dim valueTuple2 As ValueTuple(Of String, String) = ModLaunch.MsLoginStep3(text)
                Data.Progress = 0.55
                If Data.IsCanceled Then
                    Throw New OperationCanceledException()
                End If
                Dim valueTuple3 As ValueTuple(Of String, Integer) = ModLaunch.MsLoginStep4(valueTuple2)
                num = CLng(valueTuple3.Item2)
                Data.Progress = 0.7
                If Data.IsCanceled Then
                    Throw New OperationCanceledException()
                End If
                ModLaunch.MsLoginStep5(valueTuple3.Item1)
                Data.Progress = 0.85
                If Data.IsCanceled Then
                    Throw New OperationCanceledException()
                End If
                Dim valueTuple4 As ValueTuple(Of String, String, String) = ModLaunch.MsLoginStep6(valueTuple3.Item1)
                Data.Progress = 0.98
                Settings.[Set]("CacheMsV2OAuthRefresh", item2, Nothing)
                Settings.[Set]("CacheMsV2Access", valueTuple3.Item1, Nothing)
                Settings.[Set]("CacheMsV2Uuid", valueTuple4.Item1, Nothing)
                Settings.[Set]("CacheMsV2Name", valueTuple4.Item2, Nothing)
                Settings.[Set]("CacheMsV2ProfileJson", valueTuple4.Item3, Nothing)
                Settings.[Set]("CacheMsV2Expires", valueTuple3.Item2, Nothing)
                Dim jobject As JObject = CType(Settings.[Get](Of String)("LoginMsJson", Nothing).DeserializeJson(), JObject)
                jobject.Remove(input.UserName)
                jobject(valueTuple4.Item2) = item2
                Settings.[Set]("LoginMsJson", jobject.ToString(0, New JsonConverter(-1) {}), Nothing)
                Data.Output = New ModLaunch.McLoginResult() With { .AccessToken = valueTuple3.Item1, .Name = valueTuple4.Item2, .Uuid = valueTuple4.Item1, .Type = "Microsoft", .ClientToken = valueTuple4.Item1, .ProfileJson = valueTuple4.Item3 }
            End If
            ModLaunch.McLaunchLog(String.Format("微软登录结束，AccessToken 将在 {0} 过期", DateTimeOffset.FromUnixTimeSeconds(num).LocalDateTime), LogLevel.Info)
            Settings.[Set]("HintBuy", True, Nothing)
            If ModSecret.ThemeUnlock(10, False, Nothing) Then
                ModMain.MyMsgBox("感谢你对正版游戏的支持！" & vbCrLf & "隐藏主题 跳票红 已解锁！", "提示", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
            End If
        End Sub

        Private Sub McLoginServerStart(Data As ModLoader.LoaderTask(Of ModLaunch.McLoginServer, ModLaunch.McLoginResult))
            Dim input As ModLaunch.McLoginServer = Data.Input
            Dim flag As Boolean = False
            Dim flag2 As Boolean = False
            ModLaunch.McLaunchLog("登录方式：" + input.Description, LogLevel.Info)
            Data.Progress = 0.05
            If Data.Input.ForceReselectProfile OrElse Operators.CompareString(Settings.[Get](Of String)("Cache" + input.Token + "Username", Nothing), Data.Input.UserName, False) <> 0 OrElse Operators.CompareString(Settings.[Get](Of String)("Cache" + input.Token + "Pass", Nothing), Data.Input.Password, False) <> 0 OrElse Operators.CompareString(Settings.[Get](Of String)("Cache" + input.Token + "Access", Nothing), "", False) = 0 OrElse Operators.CompareString(Settings.[Get](Of String)("Cache" + input.Token + "Client", Nothing), "", False) = 0 OrElse Operators.CompareString(Settings.[Get](Of String)("Cache" + input.Token + "Uuid", Nothing), "", False) = 0 OrElse Operators.CompareString(Settings.[Get](Of String)("Cache" + input.Token + "Name", Nothing), "", False) = 0 Then
                GoTo IL_021C
            End If
            Try
                If Data.IsCanceled Then
                    Throw New OperationCanceledException()
                End If
                ModLaunch.McLoginRequestValidate(Data)
                GoTo IL_027B
            Catch ex As Exception
                Logger.Warn(ex, "验证登录失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                If ex.IsBadNetwork() Then
                    Throw New Exception("$登录失败：你的网络环境不佳，导致难以连接到海外服务器。" & vbCrLf & "请稍后再试，或使用加速器、VPN 改善网络环境。")
                End If
            End Try
            Data.Progress = 0.25
            IL_01AC:
            Try
                If Data.IsCanceled Then
                    Throw New OperationCanceledException()
                End If
                ModLaunch.McLoginRequestRefresh(Data, flag)
                GoTo IL_027B
            Catch ex2 As Exception
                ModLaunch.McLaunchLog("刷新登录失败：" + ex2.GetDisplay(True), LogLevel.Info)
                If flag2 Then
                    Throw New Exception("二轮刷新登录失败", ex2)
                End If
            End Try
            Data.Progress = If(flag, 0.85, 0.45)
            IL_021C:
            Try
                If Data.IsCanceled Then
                    Throw New OperationCanceledException()
                End If
                flag = ModLaunch.McLoginRequestLogin(Data)
            Catch ex3 As Exception
                ModLaunch.McLaunchLog("登录失败：" + ex3.GetDisplay(True), LogLevel.Info)
                Throw
            End Try
            If flag Then
                ModLaunch.McLaunchLog("重新进行刷新登录", LogLevel.Info)
                flag2 = True
                Data.Progress = 0.65
                GoTo IL_01AC
            End If
            IL_027B:
            Data.Progress = 0.95
            Dim dictionary As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Dim list As List(Of String) = New List(Of String)()
            Dim list2 As List(Of String) = New List(Of String)()
            Try
                If Operators.CompareString(Settings.[Get](Of String)("Login" + input.Token + "Email", Nothing), "", False) <> 0 Then
                    list.AddRange(Settings.[Get](Of String)("Login" + input.Token + "Email", Nothing).Split("¨", False))
                End If
                If Operators.CompareString(Settings.[Get](Of String)("Login" + input.Token + "Pass", Nothing), "", False) <> 0 Then
                    list2.AddRange(Settings.[Get](Of String)("Login" + input.Token + "Pass", Nothing).Split("¨", False))
                End If
                Dim num As Integer = list.Count - 1
                For i As Integer = 0 To num
                    dictionary.Add(list(i), list2(i))
                Next
                dictionary.Remove(input.UserName)
                list = New List(Of String)(dictionary.Keys)
                list.Insert(0, input.UserName)
                list2 = New List(Of String)(dictionary.Values)
                list2.Insert(0, input.Password)
                Settings.[Set]("Login" + input.Token + "Email", list.Join("¨"c), Nothing)
                Settings.[Set]("Login" + input.Token + "Pass", list2.Join("¨"c), Nothing)
            Catch ex4 As Exception
                Logger.[Error](ex4, "保存启动记录失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                Settings.[Set]("Login" + input.Token + "Email", "", Nothing)
                Settings.[Set]("Login" + input.Token + "Pass", "", Nothing)
            End Try
        End Sub

        Private Sub McLoginLegacyStart(Data As ModLoader.LoaderTask(Of ModLaunch.McLoginLegacy, ModLaunch.McLoginResult))
            Dim input As ModLaunch.McLoginLegacy = Data.Input
            ModLaunch.McLaunchLog("登录方式：离线（" + input.UserName + "）", LogLevel.Info)
            Data.Progress = 0.1
            Data.Output.Name = input.UserName
            Data.Output.Uuid = ModLaunch.McLoginLegacyUuidWithCustomSkin(input.UserName, input.SkinType, input.SkinName)
            Data.Output.Type = "Legacy"
            Data.Output.AccessToken = Data.Output.Uuid
            Data.Output.ClientToken = Data.Output.Uuid
            Dim list As List(Of String) = New List(Of String)()
            If Operators.CompareString(Settings.[Get](Of String)("LoginLegacyName", Nothing), "", False) <> 0 Then
                list.AddRange(Settings.[Get](Of String)("LoginLegacyName", Nothing).ToString().Split("¨", False))
            End If
            list.Remove(input.UserName)
            list.Insert(0, input.UserName)
            Settings.[Set]("LoginLegacyName", list.Join("¨"c), Nothing)
        End Sub

        Private Sub McLoginRequestValidate(ByRef Data As ModLoader.LoaderTask(Of ModLaunch.McLoginServer, ModLaunch.McLoginResult))
            ModLaunch.McLaunchLog("验证登录开始（Validate, " + Data.Input.Token + "）", LogLevel.Info)
            Dim text As String = Settings.[Get](Of String)("Cache" + Data.Input.Token + "Access", Nothing)
            Dim text2 As String = Settings.[Get](Of String)("Cache" + Data.Input.Token + "Client", Nothing)
            Dim text3 As String = Settings.[Get](Of String)("Cache" + Data.Input.Token + "Uuid", Nothing)
            Dim text4 As String = Settings.[Get](Of String)("Cache" + Data.Input.Token + "Name", Nothing)
            Dim jobject As JObject = New JObject(New Object() { New JProperty("accessToken", text), New JProperty("clientToken", text2), New JProperty("requestUser", True) })
            Dim text5 As String = Data.Input.BaseUrl + "/validate"
            Dim post As HttpMethod = HttpMethod.Post
            Dim text6 As String = Nothing
            Dim obj As Object = jobject.ToString(0, New JsonConverter(-1) {})
            Dim text7 As String = "application/json; charset=utf-8"
            Dim text8 As String = "*/*"
            Dim array As String(,) = New String(0, 1) {}
            array(0, 0) = "Accept-Language"
            array(0, 1) = "zh-CN"
            ModNet.NetRequestByClientRetry(text5, post, text6, obj, text7, text8, array, False, Nothing, False)
            Data.Output.AccessToken = text
            Data.Output.ClientToken = text2
            Data.Output.Uuid = text3
            Data.Output.Name = text4
            Data.Output.Type = Data.Input.Token
            ModLaunch.McLaunchLog("验证登录成功（Validate, " + Data.Input.Token + "）", LogLevel.Info)
        End Sub

        Private Sub McLoginRequestRefresh(ByRef Data As ModLoader.LoaderTask(Of ModLaunch.McLoginServer, ModLaunch.McLoginResult), RequestUser As Boolean)
            ModLaunch.McLaunchLog("刷新登录开始（Refresh, " + Data.Input.Token + "）", LogLevel.Info)
            Dim text As String = Data.Input.BaseUrl + "/refresh"
            Dim post As HttpMethod = HttpMethod.Post
            Dim text2 As String = Nothing
            Dim obj As Object = New JObject(New Object() { New JProperty("selectedProfile", New JObject(New Object() { New JProperty("name", Settings.[Get](Of String)(String.Format("Cache{0}Name", Data.Input.Token), Nothing)), New JProperty("id", Settings.[Get](Of String)(String.Format("Cache{0}Uuid", Data.Input.Token), Nothing)) })), New JProperty("accessToken", Settings.[Get](Of String)(String.Format("Cache{0}Access", Data.Input.Token), Nothing)), New JProperty("requestUser", True) }).ToString(0, New JsonConverter(-1) {})
            Dim text3 As String = "application/json; charset=utf-8"
            Dim text4 As String = "*/*"
            Dim array As String(,) = New String(0, 1) {}
            array(0, 0) = "Accept-Language"
            array(0, 1) = "zh-CN"
            Dim jobject As JObject = CType(ModNet.NetRequestByClientRetry(text, post, text2, obj, text3, text4, array, True, Nothing, False).DeserializeJson(), JObject)
            If jobject("selectedProfile") Is Nothing Then
                Throw New Exception("选择的角色 " + Settings.[Get](Of String)("Cache" + Data.Input.Token + "Name", Nothing) + " 无效！")
            End If
            Data.Output.AccessToken = jobject("accessToken").ToString()
            Data.Output.ClientToken = jobject("clientToken").ToString()
            Data.Output.Uuid = jobject("selectedProfile")("id").ToString()
            Data.Output.Name = jobject("selectedProfile")("name").ToString()
            Data.Output.Type = Data.Input.Token
            Settings.[Set]("Cache" + Data.Input.Token + "Access", Data.Output.AccessToken, Nothing)
            Settings.[Set]("Cache" + Data.Input.Token + "Client", Data.Output.ClientToken, Nothing)
            Settings.[Set]("Cache" + Data.Input.Token + "Uuid", Data.Output.Uuid, Nothing)
            Settings.[Set]("Cache" + Data.Input.Token + "Name", Data.Output.Name, Nothing)
            Settings.[Set]("Cache" + Data.Input.Token + "Username", Data.Input.UserName, Nothing)
            Settings.[Set]("Cache" + Data.Input.Token + "Pass", Data.Input.Password, Nothing)
            ModLaunch.McLaunchLog("刷新登录成功（Refresh, " + Data.Input.Token + "）", LogLevel.Info)
        End Sub

        Private Function McLoginRequestLogin(ByRef Data As ModLoader.LoaderTask(Of ModLaunch.McLoginServer, ModLaunch.McLoginResult)) As Boolean
            Dim flag2 As Boolean
            Try
                Dim CS$<>8__locals1 As ModLaunch._Closure$__35-0 = New ModLaunch._Closure$__35-0(CS$<>8__locals1)
                Dim flag As Boolean = False
                ModLaunch.McLaunchLog("登录开始（Login, " + Data.Input.Token + "）", LogLevel.Info)
                Dim jobject As JObject = New JObject(New Object() { New JProperty("agent", New JObject(New Object() { New JProperty("name", "Minecraft"), New JProperty("version", 1) })), New JProperty("username", Data.Input.UserName), New JProperty("password", Data.Input.Password), New JProperty("requestUser", True) })
                Dim CS$<>8__locals2 As ModLaunch._Closure$__35-0 = CS$<>8__locals1
                Dim text As String = Data.Input.BaseUrl + "/authenticate"
                Dim post As HttpMethod = HttpMethod.Post
                Dim text2 As String = Nothing
                Dim obj As Object = jobject.ToString(0, New JsonConverter(-1) {})
                Dim text3 As String = "application/json; charset=utf-8"
                Dim text4 As String = "*/*"
                Dim array As String(,) = New String(0, 1) {}
                array(0, 0) = "Accept-Language"
                array(0, 1) = "zh-CN"
                CS$<>8__locals2.$VB$Local_LoginJson = CType(ModNet.NetRequestByClientRetry(text, post, text2, obj, text3, text4, array, True, Nothing, False).DeserializeJson(), JObject)
                If CS$<>8__locals1.$VB$Local_LoginJson("availableProfiles").Count() = 0 Then
                    If Data.Input.ForceReselectProfile Then
                        ModMain.Hint("你还没有创建角色，无法更换！", ModMain.HintType.Red, True)
                    End If
                    Throw New Exception("$你还没有创建角色，请在创建角色后再试！")
                End If
                If Data.Input.ForceReselectProfile AndAlso CS$<>8__locals1.$VB$Local_LoginJson("availableProfiles").IsSingle() Then
                    ModMain.Hint("你的账户中只有一个角色，无法更换！", ModMain.HintType.Red, True)
                End If
                CS$<>8__locals1.$VB$Local_SelectedName = Nothing
                CS$<>8__locals1.$VB$Local_SelectedId = Nothing
                If(CS$<>8__locals1.$VB$Local_LoginJson("selectedProfile") Is Nothing OrElse Data.Input.ForceReselectProfile) AndAlso CS$<>8__locals1.$VB$Local_LoginJson("availableProfiles").Count() > 1 Then
                    flag = True
                    Dim text5 As String = Settings.[Get](Of String)("Cache" + Data.Input.Token + "Uuid", Nothing)
                    Try
                        For Each jtoken As JToken In CS$<>8__locals1.$VB$Local_LoginJson("availableProfiles")
                            If Operators.CompareString(jtoken("id").ToString(), text5, False) = 0 Then
                                CS$<>8__locals1.$VB$Local_SelectedName = jtoken("name").ToString()
                                CS$<>8__locals1.$VB$Local_SelectedId = jtoken("id").ToString()
                                ModLaunch.McLaunchLog("根据缓存选择的角色：" + CS$<>8__locals1.$VB$Local_SelectedName, LogLevel.Info)
                            End If
                        Next
                    Finally
                        Dim enumerator As IEnumerator(Of JToken)
                        If enumerator IsNot Nothing Then
                            enumerator.Dispose()
                        End If
                    End Try
                    If CS$<>8__locals1.$VB$Local_SelectedName Is Nothing Then
                        ModLaunch.McLaunchLog("要求玩家选择角色", LogLevel.Info)
                        ModBase.RunInUiWait(Sub()
                            Dim list As List(Of IMyRadio) = New List(Of IMyRadio)()
                            Dim list2 As List(Of JToken) = New List(Of JToken)()
                            Try
                                For Each jtoken2 As JToken In CS$<>8__locals1.$VB$Local_LoginJson("availableProfiles")
                                    list.Add(New MyRadioBox() With { .Text = jtoken2("name").ToString() })
                                    list2.Add(jtoken2)
                                Next
                            Finally
                                Dim enumerator2 As IEnumerator(Of JToken)
                                If enumerator2 IsNot Nothing Then
                                    enumerator2.Dispose()
                                End If
                            End Try
                            Dim value As Integer = ModMain.MyMsgBoxSelect(list, "选择使用的角色", "确定", "", False).Value
                            CS$<>8__locals1.$VB$Local_SelectedName = list2(value)("name").ToString()
                            CS$<>8__locals1.$VB$Local_SelectedId = list2(value)("id").ToString()
                        End Sub)
                        ModLaunch.McLaunchLog("玩家选择的角色：" + CS$<>8__locals1.$VB$Local_SelectedName, LogLevel.Info)
                    End If
                Else
                    CS$<>8__locals1.$VB$Local_SelectedName = CS$<>8__locals1.$VB$Local_LoginJson("selectedProfile")("name").ToString()
                    CS$<>8__locals1.$VB$Local_SelectedId = CS$<>8__locals1.$VB$Local_LoginJson("selectedProfile")("id").ToString()
                End If
                Data.Output.AccessToken = CS$<>8__locals1.$VB$Local_LoginJson("accessToken").ToString()
                Data.Output.ClientToken = CS$<>8__locals1.$VB$Local_LoginJson("clientToken").ToString()
                Data.Output.Name = CS$<>8__locals1.$VB$Local_SelectedName
                Data.Output.Uuid = CS$<>8__locals1.$VB$Local_SelectedId
                Data.Output.Type = Data.Input.Token
                Settings.[Set]("Cache" + Data.Input.Token + "Access", Data.Output.AccessToken, Nothing)
                Settings.[Set]("Cache" + Data.Input.Token + "Client", Data.Output.ClientToken, Nothing)
                Settings.[Set]("Cache" + Data.Input.Token + "Uuid", Data.Output.Uuid, Nothing)
                Settings.[Set]("Cache" + Data.Input.Token + "Name", Data.Output.Name, Nothing)
                Settings.[Set]("Cache" + Data.Input.Token + "Username", Data.Input.UserName, Nothing)
                Settings.[Set]("Cache" + Data.Input.Token + "Pass", Data.Input.Password, Nothing)
                ModLaunch.McLaunchLog("登录成功（Login, " + Data.Input.Token + "）", LogLevel.Info)
                flag2 = flag
            Catch ex As Exception
                Dim display As String = ex.GetDisplay(False)
                Logger.Info(ex, "登录失败原始错误信息", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                If TypeOf ex Is HttpRequestCodeException Then
                    Dim text6 As String = Nothing
                    Try
                        Dim response As String = CType(ex, HttpRequestCodeException).Response
                        If response.ContainsIgnoreCase("errorMessage") Then
                            text6 = response.DeserializeJson()("errorMessage").ToString()
                        End If
                    Catch ex2 As Exception
                    End Try
                    If Not String.IsNullOrWhiteSpace(text6) Then
                        If text6.Contains("密码错误") OrElse text6.ContainsIgnoreCase("Incorrect username or password") Then
                            ModLaunch.McLaunchLog("密码错误，退出登录", LogLevel.Info)
                            Dim type As ModLaunch.McLoginType = Data.Input.Type
                            If type <> ModLaunch.McLoginType.Nide Then
                                If type = ModLaunch.McLoginType.Auth Then
                                    ModBase.RunInUi(AddressOf PageLoginAuthSkin.ExitLogin, False)
                                End If
                            Else
                                ModBase.RunInUi(AddressOf PageLoginNideSkin.ExitLogin, False)
                            End If
                        End If
                        Throw New Exception("$登录失败：" + text6)
                    End If
                End If
                If display.Contains("(403)") Then
                    Dim type2 As ModLaunch.McLoginType = Data.Input.Type
                    If type2 = ModLaunch.McLoginType.Nide Then
                        Throw New Exception("$登录失败，以下为可能的原因：" & vbCrLf & " - 输入的账号或密码错误。" & vbCrLf & " - 密码错误次数过多，导致被暂时屏蔽。请不要操作，等待 10 分钟后再试。" & vbCrLf + If(Data.Input.UserName.Contains("@"), "", " - 登录账号应为邮箱或统一通行证账号，而非游戏角色 ID。" & vbCrLf) + " - 只注册了账号，但没有加入对应服务器。")
                    End If
                    If type2 = ModLaunch.McLoginType.Auth Then
                        Throw New Exception("$登录失败，以下为可能的原因：" & vbCrLf & " - 输入的账号或密码错误。" & vbCrLf & " - 登录尝试过于频繁，导致被暂时屏蔽。请不要操作，等待 10 分钟后再试。" & vbCrLf & " - 只注册了账号，但没有在皮肤站新建角色。")
                    End If
                    flag2 = False
                Else
                    If ex.IsBadNetwork() Then
                        Throw New Exception("$登录失败：你的网络环境不佳，导致难以连接到海外服务器。" & vbCrLf & "请稍后再试，或使用加速器、VPN 改善网络环境。")
                    End If
                    If ex.Message.StartsWithF("$", False) Then
                        Throw
                    End If
                    Throw New Exception("登录失败：" + ex.Message, ex)
                End If
            End Try
            Return flag2
        End Function

        Private Function MsLoginStep1New(Data As ModLoader.LoaderTask(Of ModLaunch.McLoginMs, ModLaunch.McLoginResult)) As <TupleElementNames(New String() { "OAuthAccessToken", "OAuthRefreshToken" })> ValueTuple(Of String, String)
            Dim myMsgBoxConverter As ModMain.MyMsgBoxConverter
            Dim text As String
            Dim text2 As String
            Dim text3 As String
            Dim text4 As String
            Dim text5 As String
            Dim flag As Boolean
            Dim flag2 As Boolean
            Dim flag3 As Boolean
            Dim action As Action
            Dim action2 As Action
            Do
                ModLaunch.McLaunchLog("开始微软登录步骤 1/6（原始登录）", LogLevel.Info)
                Dim jobject As JObject = CType(ModNet.NetRequestByClientRetry("https://login.microsoftonline.com/consumers/oauth2/v2.0/devicecode", HttpMethod.Post, Nothing, String.Format("client_id={0}&tenant=/consumers&scope=XboxLive.signin%20offline_access", "fe72edc2-3a6f-4280-90e8-e2beb64ce7e1"), "application/x-www-form-urlencoded", "*/*", Nothing, True, Nothing, False).DeserializeJson(), JObject)
                ModLaunch.McLaunchLog("网页登录地址：" + jobject("verification_uri").ToString(), LogLevel.Info)
                myMsgBoxConverter = New ModMain.MyMsgBoxConverter() With { .Content = jobject, .ForceWait = True, .Type = ModMain.MyMsgBoxType.Login }
                ModMain.WaitingMyMsgBox.Enqueue(myMsgBoxConverter)
                While myMsgBoxConverter.Result Is Nothing
                    Thread.Sleep(100)
                End While
                If Not(TypeOf myMsgBoxConverter.Result Is ModBase.RestartException) Then
                    GoTo IL_015E
                End If
                text = String.Format("请在登录时选择 {0}其他登录方法{1}，然后选择 {2}使用我的密码{3}。{4}如果没有该选项，请选择 {5}设置密码{6}，设置完毕后再登录。", New Object() { ModBase.vbLQ, ModBase.vbRQ, ModBase.vbLQ, ModBase.vbRQ, vbCrLf, ModBase.vbLQ, ModBase.vbRQ })
                text2 = "需要使用密码登录"
                text3 = "重新登录"
                text4 = "设置密码"
                text5 = "取消"
                flag = False
                flag2 = True
                flag3 = False
                action = Nothing
                If ModLaunch._Closure$__.$I36-0 IsNot Nothing Then
                    action2 = ModLaunch._Closure$__.$I36-0
                Else
                    Dim action3 As Action = Sub()
                        ModBase.OpenWebsite("https://account.live.com/password/Change")
                    End Sub
                    action2 = action3
                    ModLaunch._Closure$__.$I36-0 = action3
                End If
            Loop While ModMain.MyMsgBox(text, text2, text3, text4, text5, flag, flag2, flag3, action, action2, Nothing) = 1
            Throw New OperationCanceledException()
            IL_015E:
            If TypeOf myMsgBoxConverter.Result Is Exception Then
                Throw CType(myMsgBoxConverter.Result, Exception)
            End If
            Dim valueTuple As ValueTuple(Of String, String) = New ValueTuple(Of String, String)(Conversions.ToString(NewLateBinding.LateIndexGet(myMsgBoxConverter.Result, New Object() { 0 }, Nothing)), Conversions.ToString(NewLateBinding.LateIndexGet(myMsgBoxConverter.Result, New Object() { 1 }, Nothing)))
            Return valueTuple
        End Function

        Private Function MsLoginStep1Refresh(Code As String) As <TupleElementNames(New String() { "OAuthAccessToken", "OAuthRefreshToken" })> ValueTuple(Of String, String)
            ModLaunch.McLaunchLog("开始微软登录步骤 1/6（刷新登录）", LogLevel.Info)
            Dim text4 As String
            Dim valueTuple As ValueTuple(Of String, String)
            Try
                Dim text As String = "https://login.live.com/oauth20_token.srf"
                Dim post As HttpMethod = HttpMethod.Post
                Dim num As Integer = 2
                Dim obj As Object = String.Format("client_id={0}&refresh_token={1}&grant_type=refresh_token&scope=XboxLive.signin%20offline_access", "fe72edc2-3a6f-4280-90e8-e2beb64ce7e1", StringUtils.FormUrlEscape(Code))
                Dim text2 As String = "application/x-www-form-urlencoded"
                Dim text3 As String = "*/*"
                Dim array As String(,) = New String(1, 1) {}
                array(0, 0) = "Accept-Language"
                array(0, 1) = "en-US,en;q=0.5"
                array(1, 0) = "X-Requested-With"
                array(1, 1) = "XMLHttpRequest"
                text4 = ModNet.NetRequestByClientMultiple(text, post, num, obj, text2, text3, array, False, 25000, Nothing, False)
            Catch ex As HttpRequestCodeException
                Dim response As String = ex.Response
                If response.ContainsIgnoreCase("must sign in again") OrElse response.ContainsIgnoreCase("password expired") OrElse (response.Contains("refresh_token") AndAlso response.ContainsIgnoreCase("is not valid")) OrElse response.ContainsIgnoreCase("expired") Then
                    valueTuple = New ValueTuple(Of String, String)("Relogin", "")
                    Return valueTuple
                End If
                If response.Contains("Account security interrupt") Then
                    ModMain.MyMsgBox("该账号由于安全问题无法登陆，请前往微软账户页获取更多信息。", "登录失败", "我知道了", "", "", True, True, False, Nothing, Nothing, Nothing)
                    Throw New OperationCanceledException()
                End If
                If response.Contains("service abuse") Then
                    ModMain.MyMsgBox("非常抱歉，该账号已被微软封禁，无法登录。", "登录失败", "我知道了", "", "", True, True, False, Nothing, Nothing, Nothing)
                    Throw New OperationCanceledException()
                End If
                Throw
            End Try
            Dim jobject As JObject = CType(text4.DeserializeJson(), JObject)
            Dim text5 As String = jobject("access_token").ToString()
            Dim text6 As String = jobject("refresh_token").ToString()
            valueTuple = New ValueTuple(Of String, String)(text5, text6)
            Return valueTuple
        End Function

        Private Function MsLoginStep2(AccessToken As String) As String
            ModLaunch.McLaunchLog("开始微软登录步骤 2/6", LogLevel.Info)
            Dim text As String = "{" & vbCrLf & "           ""Properties"": {" & vbCrLf & "               ""AuthMethod"": ""RPS""," & vbCrLf & "               ""SiteName"": ""user.auth.xboxlive.com""," & vbCrLf & "               ""RpsTicket"": """ + If(AccessToken.StartsWithF("d=", False), "", "d=") + AccessToken + """" & vbCrLf & "           }," & vbCrLf & "           ""RelyingParty"": ""http://auth.xboxlive.com""," & vbCrLf & "           ""TokenType"": ""JWT""" & vbCrLf & "        }"
            Return ModNet.NetRequestByClientMultiple("https://user.auth.xboxlive.com/user/authenticate", HttpMethod.Post, 3, text, "application/json", "*/*", Nothing, True, 25000, Nothing, False).DeserializeJson()("Token").ToString()
        End Function

        Private Function MsLoginStep3(XBLToken As String) As <TupleElementNames(New String() { "XSTSToken", "UHS" })> ValueTuple(Of String, String)
            ModLaunch.McLaunchLog("开始微软登录步骤 3/6", LogLevel.Info)
            Dim text As String = "{" & vbCrLf & "           ""Properties"": {" & vbCrLf & "               ""SandboxId"": ""RETAIL""," & vbCrLf & "               ""UserTokens"": [""" + XBLToken + """]" & vbCrLf & "           }," & vbCrLf & "           ""RelyingParty"": ""rp://api.minecraftservices.com/""," & vbCrLf & "           ""TokenType"": ""JWT""" & vbCrLf & "        }"
            Dim text2 As String
            Try
                text2 = ModNet.NetRequestByClientMultiple("https://xsts.auth.xboxlive.com/xsts/authorize", HttpMethod.Post, 3, text, "application/json", "*/*", Nothing, True, 25000, Nothing, False)
            Catch ex As HttpRequestCodeException
                If ex.Response.Contains("2148916227") Then
                    ModMain.MyMsgBox("该账号似乎已被微软封禁，无法登录。", "登录失败", "我知道了", "", "", True, True, False, Nothing, Nothing, Nothing)
                    Throw New OperationCanceledException()
                End If
                If ex.Response.Contains("2148916233") Then
                    If ModMain.MyMsgBox("你尚未注册 Xbox 账户，请在注册后再登录。", "登录提示", "注册", "取消", "", False, True, False, Nothing, Nothing, Nothing) = 1 Then
                        ModBase.OpenWebsite("https://signup.live.com/signup")
                    End If
                    Throw New OperationCanceledException()
                End If
                If ex.Response.Contains("2148916235") Then
                    ModMain.MyMsgBox(String.Format("你的网络所在的国家或地区无法登录微软账号。{0}请使用加速器或 VPN，然后再试。", vbCrLf), "登录失败", "我知道了", "", "", False, True, False, Nothing, Nothing, Nothing)
                    Throw New OperationCanceledException()
                End If
                If ex.Response.Contains("2148916238") Then
                    If ModMain.MyMsgBox("该账号年龄不足，你需要先修改出生日期，然后才能登录。" & vbCrLf & "该账号目前填写的年龄是否在 13 岁以上？", "登录提示", "13 岁以上", "12 岁以下", "我不知道", False, True, False, Nothing, Nothing, Nothing) = 1 Then
                        ModBase.OpenWebsite("https://account.live.com/editprof.aspx")
                        ModMain.MyMsgBox("请在打开的网页中修改账号的出生日期（至少改为 18 岁以上）。" & vbCrLf & "在修改成功后等待一分钟，然后再回到 PCL，就可以正常登录了！", "登录提示", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
                    Else
                        ModBase.OpenWebsite("https://support.microsoft.com/zh-cn/account-billing/如何更改-microsoft-帐户上的出生日期-837badbc-999e-54d2-2617-d19206b9540a")
                        ModMain.MyMsgBox("请根据打开的网页的说明，修改账号的出生日期（至少改为 18 岁以上）。" & vbCrLf & "在修改成功后等待一分钟，然后再回到 PCL，就可以正常登录了！", "登录提示", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
                    End If
                    Throw New OperationCanceledException()
                End If
                Throw
            End Try
            Dim jobject As JObject = CType(text2.DeserializeJson(), JObject)
            Return New ValueTuple(Of String, String)(jobject("Token").ToString(), jobject("DisplayClaims")("xui")(0)("uhs").ToString())
        End Function

        Private Function MsLoginStep4(<TupleElementNames(New String() { "XSTSToken", "UHS" })> Tokens As ValueTuple(Of String, String)) As <TupleElementNames(New String() { "AccessToken", "ExpiresAt" })> ValueTuple(Of String, Integer)
            ModLaunch.McLaunchLog("开始微软登录步骤 4/6", LogLevel.Info)
            Dim text As String = New JObject(New JProperty("identityToken", String.Format("XBL3.0 x={0};{1}", Tokens.Item2, Tokens.Item1))).ToString(0, New JsonConverter(-1) {})
            Dim text2 As String
            Try
                text2 = ModNet.NetRequestByClientRetry("https://api.minecraftservices.com/authentication/login_with_xbox", HttpMethod.Post, Nothing, text, "application/json", "*/*", Nothing, True, Nothing, False)
            Catch ex As HttpRequestCodeException
                If ex.StatusCode = CType(429, HttpStatusCode) Then
                    Logger.Warn(ex, "微软登录第 4 步汇报 429", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                    Throw New Exception("$登录尝试太过频繁，请等待几分钟后再试！")
                End If
                If ex.StatusCode = HttpStatusCode.Forbidden Then
                    Logger.Warn(ex, "微软登录第 4 步汇报 403", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                    Throw New Exception("$当前 IP 的登录尝试异常。" & vbCrLf & "如果你使用了 VPN 或加速器，请把它们关掉或更换节点后再试！")
                End If
                If ex.StatusCode = HttpStatusCode.ServiceUnavailable Then
                    Logger.Warn(ex, "微软登录第 4 步汇报 503", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                    Throw New Exception("$Mojang 服务器出现问题，请稍后再试。" & vbCrLf & "你的网络是正常的，PCL 也是正常的，是 Mojang 出问题了……")
                End If
                Dim response As String = ex.Response
                If response IsNot Nothing AndAlso response.Contains("ACCOUNT_SUSPENDED") Then
                    ModMain.MyMsgBox("该账号似乎已被微软封禁，无法登录。", "登录失败", "我知道了", "", "", True, True, False, Nothing, Nothing, Nothing)
                    Throw New OperationCanceledException()
                End If
                Throw
            End Try
            Dim jobject As JObject = CType(text2.DeserializeJson(), JObject)
            Return New ValueTuple(Of String, Integer)(jobject("access_token").ToString(), CInt((CLng(jobject("expires_in").ToObject(Of Integer)()) + ModBase.GetUnixTimestampUtc() - 3600L)))
        End Function

        Private Sub MsLoginStep5(AccessToken As String)
            ModLaunch.McLaunchLog("开始微软登录步骤 5/6", LogLevel.Info)
            Dim text As String = "https://api.minecraftservices.com/entitlements/mcstore"
            Dim httpMethod As HttpMethod = Nothing
            Dim num As Integer = 2
            Dim obj As Object = Nothing
            Dim text2 As String = "application/json"
            Dim text3 As String = "*/*"
            Dim array As String(,) = New String(0, 1) {}
            array(0, 0) = "Authorization"
            array(0, 1) = "Bearer " + AccessToken
            Dim text4 As String = ModNet.NetRequestByClientMultiple(text, httpMethod, num, obj, text2, text3, array, True, 25000, Nothing, False)
            Try
                Dim jobject As JObject = CType(text4.DeserializeJson(), JObject)
                If Not jobject.ContainsKey("items") OrElse Not jobject("items").Any() Then
                    Dim num2 As Integer = ModMain.MyMsgBox("你尚未购买正版 Minecraft，或者 Xbox Game Pass 已到期。", "登录失败", "购买 Minecraft", "取消", "", False, True, False, Nothing, Nothing, Nothing)
                    If num2 = 1 Then
                        ModBase.OpenWebsite("https://www.xbox.com/zh-cn/games/store/minecraft-java-bedrock-edition-for-pc/9nxp44l49shj")
                    End If
                    Throw New OperationCanceledException()
                End If
            Catch ex As Exception
                Logger.Warn(ex, String.Format("微软登录第 5 步异常：{0}", text4), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                Throw
            End Try
        End Sub

        Private Function MsLoginStep6(AccessToken As String) As <TupleElementNames(New String() { "UUID", "UserName", "ProfileJson" })> ValueTuple(Of String, String, String)
            ModLaunch.McLaunchLog("开始微软登录步骤 6/6", LogLevel.Info)
            Dim text4 As String
            Try
                Dim text As String = "https://api.minecraftservices.com/minecraft/profile"
                Dim httpMethod As HttpMethod = Nothing
                Dim num As Integer = 2
                Dim obj As Object = Nothing
                Dim text2 As String = "application/json"
                Dim text3 As String = "*/*"
                Dim array As String(,) = New String(0, 1) {}
                array(0, 0) = "Authorization"
                array(0, 1) = "Bearer " + AccessToken
                text4 = ModNet.NetRequestByClientMultiple(text, httpMethod, num, obj, text2, text3, array, True, 25000, Nothing, False)
            Catch ex As HttpRequestCodeException
                Dim statusCode As HttpStatusCode = ex.StatusCode
                If statusCode = HttpStatusCode.NotFound Then
                    Logger.Warn(ex, "微软登录第 6 步汇报 404", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                    Dim action As Action
                    If ModLaunch._Closure$__.$I42-0 IsNot Nothing Then
                        action = ModLaunch._Closure$__.$I42-0
                    Else
                        Dim action2 As Action = Sub()
                            Dim num2 As Integer = ModMain.MyMsgBox("请先创建 Minecraft 玩家档案，然后再重新登录。", "登录失败", "创建档案", "取消", "", False, True, False, Nothing, Nothing, Nothing)
                            If num2 = 1 Then
                                ModBase.OpenWebsite("https://www.minecraft.net/zh-hans/msaprofile/mygames/editprofile")
                            End If
                        End Sub
                        action = action2
                        ModLaunch._Closure$__.$I42-0 = action2
                    End If
                    ModBase.RunInNewThread(action, "Login Failed: Create Profile", ThreadPriority.Normal)
                    Throw New OperationCanceledException()
                End If
                If statusCode = CType(429, HttpStatusCode) Then
                    Logger.Warn(ex, "微软登录第 6 步汇报 429", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                    Throw New Exception("$登录尝试太过频繁，请等待几分钟后再试！")
                End If
                Throw
            End Try
            Dim jobject As JObject = CType(text4.DeserializeJson(), JObject)
            Dim text5 As String = jobject("id").ToString()
            Dim text6 As String = jobject("name").ToString()
            Return New ValueTuple(Of String, String, String)(text5, text6, text4)
        End Function

        Private Function McLoginLegacyUuidWithCustomSkin(UserName As String, SkinType As Integer, SkinName As String) As String
            Dim text As String = Conversions.ToString(ModLaunch.McLoginLegacyUuid(UserName))
            Select Case SkinType
                Case 0
                    Return text
                Case 1
                    While Operators.CompareString(ModMinecraft.McSkinSex(text), "Steve", False) <> 0
                        If text.EndsWithF("FFFFF", False) Then
                            text = text.Substring(0, 27) + "00000"
                        End If
                        text = text.Substring(0, 27) + (Long.Parse(text.Substring(27), NumberStyles.AllowHexSpecifier) + 1L).ToString("X").PadLeft(5, "0"c)
                    End While
                    Return text
                Case 2
                    While Operators.CompareString(ModMinecraft.McSkinSex(text), "Alex", False) <> 0
                        If text.EndsWithF("FFFFF", False) Then
                            text = text.Substring(0, 27) + "00000"
                        End If
                        text = text.Substring(0, 27) + (Long.Parse(text.Substring(27), NumberStyles.AllowHexSpecifier) + 1L).ToString("X").PadLeft(5, "0"c)
                    End While
                    Return text
                Case 3
                    Try
                        If Operators.CompareString(SkinName, "", False) <> 0 AndAlso ModMinecraft.McInstanceSelected IsNot Nothing AndAlso ModMinecraft.McInstanceSelected.Version.Vanilla.Major < 20 Then
                            Logger.Info(String.Format("由于离线皮肤设置，使用正版 UUID：{0}", SkinName), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                            text = Conversions.ToString(ModLaunch.McLoginMojangUuid(SkinName, False))
                        End If
                        Return text
                    Catch ex As Exception
                        Logger.[Error](ex, "皮肤信息获取失败，游戏将以无皮肤的方式启动", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                        Return text
                    End Try
                Case 4
                    IL_01E6:
                    If Operators.CompareString(ModMinecraft.McSkinSex(text), If(Settings.[Get](Of Boolean)("LaunchSkinSlim", Nothing), "Alex", "Steve"), False) = 0 Then
                        Return text
                    End If
                Case Else
                    Return text
            End Select
            If text.EndsWithF("FFFFF", False) Then
                text = text.Substring(0, 27) + "00000"
            End If
            text = text.Substring(0, 27) + (Long.Parse(text.Substring(27), NumberStyles.AllowHexSpecifier) + 1L).ToString("X").PadLeft(5, "0"c)
            GoTo IL_01E6
        End Function

        Public Function McLoginMojangUuid(Name As String, ThrowOnNotFound As Boolean) As Object
            Dim obj As Object
            If Name.Trim().Length = 0 Then
                obj = New String("0"c, 32)
            Else
                Dim text As String = ModBase.ReadIni(ModBase.PathTemp + "Cache\Uuid\Mojang.ini", Name, "")
                If Strings.Len(text) = 32 Then
                    obj = text
                Else
                    Try
                        Dim jobject As JObject = CType(ModNet.NetRequestByClientRetry("https://api.mojang.com/users/profiles/minecraft/" + Name, Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False).DeserializeJson(), JObject)
                        If jobject Is Nothing Then
                            Throw New FileNotFoundException("正版玩家档案不存在（" + Name + "）")
                        End If
                        text = CStr((If(jobject("id"), "")))
                    Catch ex As Exception
                        Logger.Warn(ex, String.Format("从官网获取正版 Uuid 失败（{0}）", Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                        If ThrowOnNotFound OrElse Operators.CompareString(ex.[GetType]().Name, "FileNotFoundException", False) <> 0 Then
                            Throw New Exception("从官网获取正版 Uuid 失败", ex)
                        End If
                        text = Conversions.ToString(ModLaunch.McLoginLegacyUuid(Name))
                    End Try
                    If Strings.Len(text) <> 32 Then
                        Throw New Exception("获取的正版 Uuid 长度不足（" + text + "）")
                    End If
                    ModBase.WriteIni(ModBase.PathTemp + "Cache\Uuid\Mojang.ini", Name, text)
                    obj = text
                End If
            End If
            Return obj
        End Function

        Public Function McLoginLegacyUuid(Name As String) As Object
            Dim text As String = Name.Length.ToString("X").EnsureLength("0"c, 16) + Name.GetStableHashCode().ToString("X").EnsureLength("0"c, 16)
            Return String.Concat(New String() { text.Substring(0, 12), "3", text.Substring(13, 3), "9", text.Substring(17, 15) })
        End Function

        Private Sub McLaunchJava(Task As ModLoader.LoaderTask(Of Integer, Integer))
            ModLaunch.McLaunchJavaSelected = ModJava.SelectOrDownloadJava(ModMinecraft.McInstanceSelected, True, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 1.0))
            If Not Task.IsCanceled Then
                If ModLaunch.McLaunchJavaSelected Is Nothing Then
                    Throw New OperationCanceledException()
                End If
                If ModBase.ModeDebug Then
                    ModLaunch.McLaunchLog(String.Format("预期 Java 版本范围：{0}", ModJava.GetJavaRequirement(ModMinecraft.McInstanceSelected).Item1), LogLevel.Info)
                    ModLaunch.McLaunchLog(String.Format("Java 版本设置类别：{0}", Settings.[Get](Of Integer)("VersionArgumentJavaV2", ModMinecraft.McInstanceSelected)), LogLevel.Info)
                End If
                ModLaunch.McLaunchLog("选择的 Java：" + ModLaunch.McLaunchJavaSelected.ToString(), LogLevel.Info)
            End If
        End Sub

        Private Sub McLaunchArgumentMain(Loader As ModLoader.LoaderTask(Of String, Integer))
            Dim list As List(Of String) = New List(Of String)()
            ModLaunch.McLaunchLog("参数获取开始", LogLevel.Info)
            list.AddRange(ModLaunch.McLaunchArgumentsJVM())
            list.Add(CStr(ModMinecraft.McInstanceSelected.JsonObject("mainClass")))
            list.AddRange(ModLaunch.McLaunchArgumentsGame())
            ModLaunch.McLaunchLog("=================== 启动参数开始 ===================", LogLevel.Info)
            Dim list2 As List(Of ValueTuple(Of String, String)) = ModLaunch.McLaunchArgumentsReplace(list).ToList()
            Dim text As String = ""
            Try
                For Each text2 As String In list
                    Try
                        For Each valueTuple As ValueTuple(Of String, String) In list2
                            text2 = text2.Replace(valueTuple.Item1, valueTuple.Item2)
                        Next
                    Finally
                        Dim enumerator2 As List(Of ValueTuple(Of String, String)).Enumerator
                        CType(enumerator2, IDisposable).Dispose()
                    End Try
                    If text2.IndexOfAny(New Char() { "&"c, "|"c, "<"c, ">"c, "^"c, " "c }) >= 0 AndAlso Not text2.StartsWithF("""", False) AndAlso Not text2.EndsWithF("""", False) Then
                        text2 = String.Format("""{0}""", text2.Replace("""", "\"""))
                    End If
                    ModLaunch.McLaunchLog(text2, LogLevel.Info)
                    text = text + text2 + " "
                Next
            Finally
                Dim enumerator As List(Of String).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            ModLaunch.McLaunchLog("=================== 启动参数结束 ===================", LogLevel.Info)
            ModLaunch.McLaunchArgument = text.TrimEnd(New Char(-1) {})
        End Sub

        Private Function McLaunchArgumentsJVM() As List(Of String)
            Dim CS$<>8__locals1 As ModLaunch._Closure$__51-0 = New ModLaunch._Closure$__51-0(CS$<>8__locals1)
            Dim list As List(Of String) = New List(Of String)()
            If ModMinecraft.McInstanceSelected.JsonObject("arguments") IsNot Nothing AndAlso ModMinecraft.McInstanceSelected.JsonObject("arguments")("jvm") IsNot Nothing Then
                Dim mcInstance As ModMinecraft.McInstance = ModMinecraft.McInstanceSelected
                While True
                    ModLaunch.McLaunchLog(String.Format("新版 JVM 参数：从版本 JSON 获取（{0}）", mcInstance.Name), LogLevel.Info)
                    If mcInstance.JsonObject("arguments") IsNot Nothing AndAlso mcInstance.JsonObject("arguments")("jvm") IsNot Nothing Then
                        Try
                            For Each jtoken As JToken In mcInstance.JsonObject("arguments")("jvm")
                                Dim type As JTokenType = jtoken.Type
                                If type <> 1 Then
                                    If type = 8 Then
                                        list.Add(jtoken.ToString().Trim())
                                    End If
                                Else
                                    Dim jobject As JObject = CType(jtoken, JObject)
                                    If(Not jobject.ContainsKey("rules") OrElse ModMinecraft.McJsonRuleCheck(jobject("rules"))) AndAlso jobject.ContainsKey("value") Then
                                        If jobject("value").Type = 8 Then
                                            list.Add(jobject("value").ToString().Trim())
                                        Else
                                            Try
                                                For Each jtoken2 As JToken In jobject("value")
                                                    list.Add(jtoken2.ToString().Trim())
                                                Next
                                            Finally
                                                Dim enumerator2 As IEnumerator(Of JToken)
                                                If enumerator2 IsNot Nothing Then
                                                    enumerator2.Dispose()
                                                End If
                                            End Try
                                        End If
                                    End If
                                End If
                            Next
                        Finally
                            Dim enumerator As IEnumerator(Of JToken)
                            If enumerator IsNot Nothing Then
                                enumerator.Dispose()
                            End If
                        End Try
                    End If
                    If Operators.CompareString(mcInstance.InheritName, "", False) = 0 Then
                        Exit For
                    End If
                    mcInstance = New ModMinecraft.McInstance(mcInstance.InheritName)
                End While
                Dim enumerable As IEnumerable(Of String) = list
                Dim func As Func(Of String, String)
                If ModLaunch._Closure$__.$I51-0 IsNot Nothing Then
                    func = ModLaunch._Closure$__.$I51-0
                Else
                    Dim func2 As Func(Of String, String) = Function(a As String)
                        If Not a.Contains(" ") OrElse a.StartsWithF("""", False) OrElse a.EndsWithF("""", False) Then
                            Return a
                        End If
                        Return String.Format("""{0}""", a.Replace("""", "\"""))
                    End Function
                    func = func2
                    ModLaunch._Closure$__.$I51-0 = func2
                End If
                list = enumerable.[Select](func).ToList()
            Else
                ModLaunch.McLaunchLog("旧版 JVM 参数", LogLevel.Info)
                list.Add("-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump")
                list.Add("-Djava.library.path=${natives_directory}")
                list.Add("-cp")
                list.Add("${classpath}")
            End If
            Dim text As String = Settings.[Get](Of String)("VersionAdvanceJvm", ModMinecraft.McInstanceSelected)
            If Operators.CompareString(text, "", False) = 0 Then
                text = Settings.[Get](Of String)("LaunchAdvanceJvm", Nothing)
            End If
            If Operators.CompareString(text, "", False) <> 0 Then
                text = ModMain.ArgumentReplace(text, Nothing, True)
            End If
            list = ModLaunch.DeduplicateJavaArguments(ModLaunch.SplitJavaArguments(list.Join(" "c).Replace("McEmu= ", "McEmu=") + " " + text).ToList(), True)
            If Operators.CompareString(ModLaunch.McLoginLoader.Output.Type, "Nide", False) = 0 Then
                list.Add("-javaagent:""${pure_directory}\nide8auth.jar""=""" + Settings.[Get](Of String)("VersionServerNide", ModMinecraft.McInstanceSelected) + """")
            End If
            If Operators.CompareString(ModLaunch.McLoginLoader.Output.Type, "Auth", False) = 0 Then
                If ModLaunch.McLaunchJavaSelected.Version.Major >= 6 Then
                    list.Add("-Djavax.net.ssl.trustStoreType=WINDOWS-ROOT")
                End If
                Dim text2 As String = Settings.[Get](Of String)("VersionServerAuthServer", ModMinecraft.McInstanceSelected)
                Try
                    ModLaunch.McLaunchLog("开始 Authlib Injector Prefetch", LogLevel.Info)
                    Dim text3 As String = ModNet.NetRequestByClientRetry(text2, Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, False, Encoding.UTF8, False)
                    list.Add("-javaagent:""${pure_directory}\authlib-injector.jar""=""" + text2 + """")
                    list.Add("-Dauthlibinjector.side=client")
                    list.Add("-Dauthlibinjector.yggdrasil.prefetched=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(text3)))
                Catch ex As Exception
                    Throw New Exception("无法连接到第三方登录服务器（" + text2 + "）", ex)
                End Try
            End If
            Dim flag As Boolean = Not Settings.[Get](Of Boolean)("LaunchAdvanceDisableJLW", Nothing) AndAlso Not Settings.[Get](Of Boolean)("VersionAdvanceDisableJLW", ModMinecraft.McInstanceSelected) AndAlso Not ModBase.IsGBKEncoding
            If flag AndAlso text.Contains("-javaagent") Then
                flag = False
                ModLaunch.McLaunchLog("由于使用了自定义 javaagent，已禁用 JLW，这可能导致游戏崩溃！", LogLevel.Warn)
            End If
            If flag Then
                If ModLaunch.McLaunchJavaSelected.Version.Major >= 9 Then
                    list.Add("--add-exports")
                    list.Add("cpw.mods.bootstraplauncher/cpw.mods.bootstraplauncher=ALL-UNNAMED")
                End If
                list.Add("-Doolloo.jlw.tmpdir=${pure_directory}")
            End If
            ModLaunch.McLaunchLog("使用 JLW：" + Conversions.ToString(flag), LogLevel.Info)
            Dim flag2 As Boolean
            If Not Settings.[Get](Of Boolean)("LaunchAdvanceDisableLUA", Nothing) AndAlso Not Settings.[Get](Of Boolean)("VersionAdvanceDisableLUA", ModMinecraft.McInstanceSelected) Then
                Dim enumerable2 As IEnumerable(Of ModMinecraft.McLibEntry) = ModMinecraft.McLibListGet(ModMinecraft.McInstanceSelected, False)
                Dim func3 As Func(Of ModMinecraft.McLibEntry, String)
                If ModLaunch._Closure$__.$I51-1 IsNot Nothing Then
                    func3 = ModLaunch._Closure$__.$I51-1
                Else
                    Dim func4 As Func(Of ModMinecraft.McLibEntry, String) = Function(e As ModMinecraft.McLibEntry) e.OriginalName
                    func3 = func4
                    ModLaunch._Closure$__.$I51-1 = func4
                End If
                flag2 = enumerable2.[Select](func3).Contains("org.lwjgl:lwjgl:3.4.1")
            Else
                flag2 = False
            End If
            Dim flag3 As Boolean = flag2
            If flag3 Then
                list.Add(String.Format("-javaagent:""{0}""", LaunchUtils.ExtractPatch("Launch\LwjglUnsafeAgent.jar", ModBase.PathPure.Value)))
            End If
            ModLaunch.McLaunchLog("使用 LUA：" + Conversions.ToString(flag3), LogLevel.Info)
            ModLaunch.McLaunchLog("当前剩余内存：" + Conversions.ToString(Math.Round(MyWpfExtension.Computer.Info.AvailablePhysicalMemory / 1024.0 / 1024.0 / 1024.0 * 10.0) / 10.0) + "G", LogLevel.Info)
            Dim num As Integer = CInt(Math.Floor(PageInstanceSetup.GetRam(ModMinecraft.McInstanceSelected) * 1024.0))
            list.Add("-Xmx" + Conversions.ToString(num) + "m")
            Dim num2 As Integer = Settings.[Get](Of Integer)("LaunchAdvanceGC", Nothing)
            If ModMinecraft.McInstanceSelected IsNot Nothing AndAlso Settings.[Get](Of Integer)("VersionAdvanceGC", ModMinecraft.McInstanceSelected) > 0 Then
                num2 = Settings.[Get](Of Integer)("VersionAdvanceGC", ModMinecraft.McInstanceSelected) - 1
            End If
            If num2 <> 3 Then
                Dim flag4 As Boolean = False
                If(num2 = 0 AndAlso ModLaunch.McLaunchJavaSelected.Version.Major < 15) OrElse (num2 = 1 AndAlso ModLaunch.McLaunchJavaSelected.Version.Major < 21) OrElse num2 = 2 OrElse num2 = 4 Then
                    flag4 = True
                End If
                If Environment.OSVersion.Version.Major < 10 OrElse Environment.OSVersion.Version.Build < 17763 Then
                    flag4 = True
                End If
                ModLaunch.McLaunchLog(String.Format("GC 设置：{0}，选取 {1}，Java 版本：{2}", num2, If(flag4, "G1GC", "ZGC"), ModLaunch.McLaunchJavaSelected.Version.Major), LogLevel.Info)
                Dim enumerable3 As IEnumerable(Of String) = list
                Dim func5 As Func(Of String, Boolean)
                If ModLaunch._Closure$__.$I51-2 IsNot Nothing Then
                    func5 = ModLaunch._Closure$__.$I51-2
                Else
                    Dim func6 As Func(Of String, Boolean) = Function(a As String) Not a.RegexCheck("( )*-XX:[+-]?(Use\w+GC|ZGenerational|UseCompactObjectHeaders|G1\w+Percent|G1\w+Size|(Max|Min)(GCPauseMillis|HeapFreeRatio))", RegexOptions.None)
                    func5 = func6
                    ModLaunch._Closure$__.$I51-2 = func6
                End If
                list = enumerable3.Where(func5).ToList()
                list.Add("-XX:+UnlockExperimentalVMOptions")
                If ModLaunch.McLaunchJavaSelected.Version.Major >= 24 Then
                    list.Add("-XX:+UseCompactObjectHeaders")
                End If
                If flag4 AndAlso num2 = 4 Then
                    list.Add("-XX:+UseG1GC")
                    list.Add("-XX:G1NewSizePercent=20")
                    list.Add("-XX:G1ReservePercent=20")
                    list.Add("-XX:G1HeapRegionSize=32M")
                    list.Add("-XX:MaxGCPauseMillis=50")
                    If Not ModBase.ModeDebug Then
                        list.Add("-XX:+PerfDisableSharedMem")
                    End If
                    If ModLaunch.McLaunchJavaSelected.Version.Major <= 7 Then
                        list.Add("-XX:MaxPermSize=512m")
                    End If
                    If ModLaunch.McLaunchJavaSelected.Version.Major = 8 Then
                        list.Add("-XX:+ParallelRefProcEnabled")
                    End If
                    If ModLaunch.McLaunchJavaSelected.Version.Major >= 12 Then
                        list.Add("-XX:MinHeapFreeRatio=25")
                    End If
                    If ModLaunch.McLaunchJavaSelected.Version.Major >= 12 Then
                        list.Add("-XX:MaxHeapFreeRatio=40")
                    End If
                ElseIf flag4 Then
                    list.Add("-XX:+UseG1GC")
                    list.Add("-XX:G1NewSizePercent=20")
                    list.Add("-XX:G1ReservePercent=20")
                    list.Add("-XX:G1HeapRegionSize=32M")
                    list.Add("-XX:MaxGCPauseMillis=50")
                    If ModLaunch.McLaunchJavaSelected.Version.Major <= 7 Then
                        list.Add("-XX:MaxPermSize=512m")
                    End If
                Else
                    list.Add("-XX:+UseZGC")
                    If ModLaunch.McLaunchJavaSelected.Version.Major = 21 OrElse ModLaunch.McLaunchJavaSelected.Version.Major = 22 Then
                        list.Add("-XX:+ZGenerational")
                    End If
                End If
            End If
            list.Remove("-XX:MaxDirectMemorySize=256M")
            list.Add("-Dlog4j2.formatMsgNoLookups=true")
            Dim flag5 As Boolean = ModLaunch.McLaunchJavaSelected.Version.Major >= 18
            Dim text4 As String = If(flag5, ModLaunch.McLaunchJavaSelected.NativeEncodingName, ModLaunch.McLaunchJavaSelected.FileEncodingName)
            Try
                ModLaunch.McLaunchEncoding = Encoding.GetEncoding(If(text4.Equals("GBK", StringComparison.OrdinalIgnoreCase), "GB18030", text4))
            Catch ex2 As Exception
                Dim encoding As Encoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.ANSICodePage)
                ModLaunch.McLaunchLog(String.Format("无法识别 Java 编码 {0}，已回退为 {1}", If(text4, "Nothing"), encoding.WebName), LogLevel.Warn)
                ModLaunch.McLaunchEncoding = encoding
            End Try
            ModLaunch.McLaunchLog(String.Format("使用编码：{0}", ModLaunch.McLaunchEncoding.WebName), LogLevel.Info)
            Dim text5 As String = If((ModLaunch.McLaunchEncoding.CodePage = 54936), "GBK", ModLaunch.McLaunchEncoding.WebName)
            CS$<>8__locals1.$VB$Local_StdoutArgName = If((ModLaunch.McLaunchJavaSelected.Version.Major < 19), "sun.stdout.encoding", "stdout.encoding")
            CS$<>8__locals1.$VB$Local_StderrArgName = If((ModLaunch.McLaunchJavaSelected.Version.Major < 19), "sun.stderr.encoding", "stderr.encoding")
            If Not list.Any(Function(a As String) a.StartsWithF(String.Format("-D{0}=", CS$<>8__locals1.$VB$Local_StdoutArgName), False)) Then
                list.Add(String.Format("-D{0}={1}", CS$<>8__locals1.$VB$Local_StdoutArgName, text5))
            End If
            If Not list.Any(Function(a As String) a.StartsWithF(String.Format("-D{0}=", CS$<>8__locals1.$VB$Local_StderrArgName), False)) Then
                list.Add(String.Format("-D{0}={1}", CS$<>8__locals1.$VB$Local_StderrArgName, text5))
            End If
            If flag5 Then
                Dim enumerable4 As IEnumerable(Of String) = list
                Dim func7 As Func(Of String, Boolean)
                If ModLaunch._Closure$__.$I51-5 IsNot Nothing Then
                    func7 = ModLaunch._Closure$__.$I51-5
                Else
                    Dim func8 As Func(Of String, Boolean) = Function(a As String) a.StartsWithF("-Dfile.encoding=", False)
                    func7 = func8
                    ModLaunch._Closure$__.$I51-5 = func8
                End If
                If Not enumerable4.Any(func7) Then
                    list.Add("-Dfile.encoding=COMPAT")
                End If
            End If
            If flag Then
                list.Add("-jar")
                list.Add(LaunchUtils.ExtractPatch("Launch\JavaWrapper.jar", ModBase.PathPure.Value))
            End If
            Return ModLaunch.DeduplicateJavaArguments(list, True)
        End Function

        Private Function McLaunchArgumentsGame() As List(Of String)
            Dim CS$<>8__locals1 As ModLaunch._Closure$__52-0 = New ModLaunch._Closure$__52-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_Arg = ""
            Dim jtoken As JToken = ModMinecraft.McInstanceSelected.JsonObject("minecraftArguments")
            If jtoken IsNot Nothing AndAlso jtoken.ToString().Any() Then
                Dim text As String = ModMinecraft.McInstanceSelected.JsonObject("minecraftArguments").ToString()
                ModLaunch.McLaunchLog("旧版游戏参数：" + text, LogLevel.Info)
                text += " --height ${resolution_height} --width ${resolution_width}"
                CS$<>8__locals1.$VB$Local_Arg = CS$<>8__locals1.$VB$Local_Arg + " " + text
            End If
            If ModMinecraft.McInstanceSelected.JsonObject("arguments") IsNot Nothing AndAlso ModMinecraft.McInstanceSelected.JsonObject("arguments")("game") IsNot Nothing Then
                Dim mcInstance As ModMinecraft.McInstance = ModMinecraft.McInstanceSelected
                Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_18(Of String) = Sub(a As String)
                    CS$<>8__locals1.$VB$Local_Arg = CS$<>8__locals1.$VB$Local_Arg + " " + If((a.Contains(" ") AndAlso Not a.StartsWithF("""", False) AndAlso Not a.EndsWithF("""", False)), String.Format("""{0}""", a.Replace("""", "\""")), a)
                End Sub
                While True
                    ModLaunch.McLaunchLog(String.Format("新版游戏参数：从版本 JSON 获取（{0}）", mcInstance.Name), LogLevel.Info)
                    If mcInstance.JsonObject("arguments") IsNot Nothing AndAlso mcInstance.JsonObject("arguments")("game") IsNot Nothing Then
                        Try
                            For Each jtoken2 As JToken In mcInstance.JsonObject("arguments")("game")
                                Dim type As JTokenType = jtoken2.Type
                                If type <> 1 Then
                                    If type = 8 Then
                                        vb$AnonymousDelegate_(jtoken2.ToString())
                                    End If
                                Else
                                    Dim jobject As JObject = CType(jtoken2, JObject)
                                    If(Not jobject.ContainsKey("rules") OrElse ModMinecraft.McJsonRuleCheck(jobject("rules"))) AndAlso jobject.ContainsKey("value") Then
                                        If jobject("value").Type = 8 Then
                                            vb$AnonymousDelegate_(jobject("value").ToString())
                                        Else
                                            Try
                                                For Each jtoken3 As JToken In jobject("value")
                                                    vb$AnonymousDelegate_(jtoken3.ToString())
                                                Next
                                            Finally
                                                Dim enumerator2 As IEnumerator(Of JToken)
                                                If enumerator2 IsNot Nothing Then
                                                    enumerator2.Dispose()
                                                End If
                                            End Try
                                        End If
                                    End If
                                End If
                            Next
                        Finally
                            Dim enumerator As IEnumerator(Of JToken)
                            If enumerator IsNot Nothing Then
                                enumerator.Dispose()
                            End If
                        End Try
                    End If
                    If Operators.CompareString(mcInstance.InheritName, "", False) = 0 Then
                        Exit For
                    End If
                    mcInstance = New ModMinecraft.McInstance(mcInstance.InheritName)
                End While
            End If
            Dim text2 As String = Settings.[Get](Of String)("VersionAdvanceGame", ModMinecraft.McInstanceSelected)
            If Operators.CompareString(text2, "", False) = 0 Then
                text2 = Settings.[Get](Of String)("LaunchAdvanceGame", Nothing)
            End If
            If Operators.CompareString(text2, "", False) <> 0 Then
                text2 = ModMain.ArgumentReplace(text2, Nothing, True)
            End If
            CS$<>8__locals1.$VB$Local_Arg = CS$<>8__locals1.$VB$Local_Arg + " " + text2
            If(ModMinecraft.McInstanceSelected.Version.ModLoaders.HasFlagF(ModLoaders.Forge) OrElse ModMinecraft.McInstanceSelected.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader)) AndAlso ModMinecraft.McInstanceSelected.Version.HasOptiFine AndAlso Not ModMinecraft.McInstanceSelected.Version.OptiFineAsMod Then
                If CS$<>8__locals1.$VB$Local_Arg.Contains("--tweakClass optifine.OptiFineForgeTweaker") Then
                    ModLaunch.McLaunchLog("发现正确的 OptiFineForge TweakClass，目前的游戏参数：" + CS$<>8__locals1.$VB$Local_Arg, LogLevel.Info)
                    CS$<>8__locals1.$VB$Local_Arg = CS$<>8__locals1.$VB$Local_Arg.Replace(" --tweakClass optifine.OptiFineForgeTweaker", "") + " --tweakClass optifine.OptiFineForgeTweaker"
                End If
                If CS$<>8__locals1.$VB$Local_Arg.Contains("--tweakClass optifine.OptiFineTweaker") Then
                    ModLaunch.McLaunchLog("发现错误的 OptiFineForge TweakClass，目前的游戏参数：" + CS$<>8__locals1.$VB$Local_Arg, LogLevel.Info)
                    CS$<>8__locals1.$VB$Local_Arg = CS$<>8__locals1.$VB$Local_Arg.Replace(" --tweakClass optifine.OptiFineTweaker", "") + " --tweakClass optifine.OptiFineForgeTweaker"
                    Try
                        FileUtils.Write(ModMinecraft.McInstanceSelected.GetJsonPath(), FileUtils.ReadAsString(ModMinecraft.McInstanceSelected.GetJsonPath(), Nothing, Nothing).Replace("optifine.OptiFineTweaker", "optifine.OptiFineForgeTweaker"), Nothing)
                    Catch ex As Exception
                        Logger.Warn(ex, "替换 OptiFineForge TweakClass 失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                    End Try
                End If
            End If
            Dim list As List(Of String) = ModLaunch.DeduplicateJavaArguments(ModLaunch.SplitJavaArguments(CS$<>8__locals1.$VB$Local_Arg).ToList(), False)
            list.AddRange(ModLaunch.CurrentLaunchOptions.ExtraGameArgs)
            If Settings.[Get](Of Integer)("LaunchArgumentWindowType", Nothing) = 0 Then
                list.Add("--fullscreen")
            End If
            Dim text3 As String = If(String.IsNullOrEmpty(ModLaunch.CurrentLaunchOptions.ServerIp), Settings.[Get](Of String)("VersionServerEnter", ModMinecraft.McInstanceSelected), ModLaunch.CurrentLaunchOptions.ServerIp)
            If text3.Any() Then
                If DateTime.Compare(ModMinecraft.McInstanceSelected.ReleaseTime, New DateTime(2023, 4, 4)) > 0 Then
                    list.Add("--quickPlayMultiplayer")
                    list.Add(text3)
                Else
                    If text3.Contains(":") Then
                        list.Add("--server")
                        list.Add(text3.Split(":", False)(0))
                        list.Add("--port")
                        list.Add(text3.Split(":", False)(1))
                    Else
                        list.Add("--server")
                        list.Add(text3)
                        list.Add("--port")
                        list.Add("25565")
                    End If
                    If ModMinecraft.McInstanceSelected.Version.HasOptiFine Then
                        ModMain.Hint("OptiFine 与自动进入服务器可能不兼容，有概率导致材质丢失甚至游戏崩溃！", ModMain.HintType.Red, True)
                    End If
                End If
            End If
            Return ModLaunch.DeduplicateJavaArguments(list, False)
        End Function

        Private Function McLaunchArgumentsReplace(Arguments As List(Of String)) As <TupleElementNames(New String() { "Key", "Value" })> IEnumerable(Of ValueTuple(Of String, String))
            Dim vb$StateMachine_53_McLaunchArgumentsReplace As ModLaunch.VB$StateMachine_53_McLaunchArgumentsReplace = New ModLaunch.VB$StateMachine_53_McLaunchArgumentsReplace(-2)
            vb$StateMachine_53_McLaunchArgumentsReplace.$P_Arguments = Arguments
            Return vb$StateMachine_53_McLaunchArgumentsReplace
        End Function

        Private Function SplitJavaArguments(Arg As String) As IEnumerable(Of String)
            Dim vb$StateMachine_54_SplitJavaArguments As ModLaunch.VB$StateMachine_54_SplitJavaArguments = New ModLaunch.VB$StateMachine_54_SplitJavaArguments(-2)
            vb$StateMachine_54_SplitJavaArguments.$P_Arg = Arg
            Return vb$StateMachine_54_SplitJavaArguments
        End Function

        Private Function DeduplicateJavaArguments(Args As List(Of String), IsJVMArgs As Boolean) As List(Of String)
            Dim list As List(Of String) = New List(Of String)()
            Dim i As Integer = 0
            IL_018C:
            While i < Args.Count
                Dim text As String = Args(i)
                If Not text.StartsWithF("-", False) OrElse i + 1 >= Args.Count OrElse (Args(i + 1).StartsWithF("-", False) AndAlso ModBase.Val(Args(i + 1).AfterFirst("-", False)) = 0.0) Then
                    i += 1
                    If list.Contains(text) Then
                        ModLaunch.McLaunchLog(String.Format("已去重单个{0}参数：{1}", If(IsJVMArgs, " JVM ", "游戏"), text), LogLevel.Info)
                    Else
                        list.Add(text)
                    End If
                Else
                    Dim text2 As String = Args(i + 1)
                    i += 2
                    Dim num As Integer = list.Count - 2
                    For j As Integer = 0 To num
                        If Operators.CompareString(list(j), text, False) = 0 Then
                            If Not IsJVMArgs AndAlso Operators.CompareString(text, "--tweakClass", False) <> 0 Then
                                ModLaunch.McLaunchLog(String.Format("已覆盖重复的{0}参数：{1} {2} → {3} {4}", New Object() { If(IsJVMArgs, " JVM ", "游戏"), text, list(j + 1), text, text2 }), LogLevel.Info)
                                list(j + 1) = text2
                                GoTo IL_018C
                            End If
                            If Operators.CompareString(list(j + 1), text2, False) = 0 Then
                                ModLaunch.McLaunchLog(String.Format("已去重{0}参数对：{1} {2}", If(IsJVMArgs, " JVM ", "游戏"), text, text2), LogLevel.Info)
                                GoTo IL_018C
                            End If
                        End If
                    Next
                    list.Add(text)
                    list.Add(text2)
                End If
            End While
            Return list
        End Function

        Private Sub McLaunchNatives(Loader As ModLoader.LoaderTask(Of Integer, Integer))
            Dim nativesFolder As String = ModLaunch.GetNativesFolder(Nothing)
            DirectoryUtils.Create(nativesFolder)
            ModLaunch.McLaunchLog("正在解压 Natives 文件", LogLevel.Info)
            Dim list As List(Of String) = New List(Of String)()
            Try
                For Each mcLibEntry As ModMinecraft.McLibEntry In ModMinecraft.McLibListGet(ModMinecraft.McInstanceSelected, True)
                    If mcLibEntry.IsNatives Then
                        Dim zipArchive As ZipArchive
                        Try
                            zipArchive = FileUtils.OpenZip(mcLibEntry.LocalPath, Nothing)
                        Catch ex As InvalidDataException
                            Logger.Warn(ex, String.Format("打开 Natives 文件失败（{0}）", mcLibEntry.LocalPath), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                            FileUtils.Delete(mcLibEntry.LocalPath, False)
                            Throw New Exception("无法打开 Natives 文件（" + mcLibEntry.LocalPath + "），该文件可能已损坏，请重新尝试启动游戏")
                        End Try
                        Try
                            For Each zipArchiveEntry As ZipArchiveEntry In zipArchive.Entries
                                Dim fullName As String = zipArchiveEntry.FullName
                                If Operators.CompareString(PathUtils.GetExtension(fullName), "dll", False) = 0 Then
                                    Dim text As String = nativesFolder + fullName
                                    list.Add(text)
                                    Dim info As FileInfo = FileUtils.GetInfo(text)
                                    If info.Exists Then
                                        If info.Length = zipArchiveEntry.Length Then
                                            If ModBase.ModeDebug Then
                                                ModLaunch.McLaunchLog("无需解压：" + text, LogLevel.Info)
                                                Continue For
                                            End If
                                            Continue For
                                        Else
                                            Try
                                                FileUtils.Delete(text, False)
                                            Catch ex2 As UnauthorizedAccessException
                                                ModLaunch.McLaunchLog("删除原 dll 访问被拒绝，这通常代表有一个 MC 正在运行，跳过解压：" + text, LogLevel.Info)
                                                ModLaunch.McLaunchLog("实际的错误信息：" + ex2.GetDisplay(True), LogLevel.Info)
                                                Exit Try
                                            End Try
                                        End If
                                    End If
                                    FileUtils.Write(text, zipArchiveEntry.Open())
                                    ModLaunch.McLaunchLog("已解压：" + text, LogLevel.Info)
                                End If
                            Next
                        Finally
                            Dim enumerator2 As IEnumerator(Of ZipArchiveEntry)
                            If enumerator2 IsNot Nothing Then
                                enumerator2.Dispose()
                            End If
                        End Try
                        If zipArchive IsNot Nothing Then
                            zipArchive.Dispose()
                        End If
                    End If
                Next
            Finally
                Dim enumerator As List(Of ModMinecraft.McLibEntry).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            Try
                For Each text2 As String In DirectoryUtils.EnumerateFiles(nativesFolder, False, "*")
                    If Not list.Contains(text2) Then
                        Try
                            ModLaunch.McLaunchLog("删除：" + text2, LogLevel.Info)
                            FileUtils.Delete(text2, False)
                        Catch ex3 As UnauthorizedAccessException
                            ModLaunch.McLaunchLog("删除多余文件访问被拒绝，跳过删除步骤", LogLevel.Info)
                            ModLaunch.McLaunchLog("实际的错误信息：" + ex3.GetDisplay(True), LogLevel.Info)
                            Exit Try
                        End Try
                    End If
                Next
            Finally
                Dim enumerator3 As IEnumerator(Of String)
                If enumerator3 IsNot Nothing Then
                    enumerator3.Dispose()
                End If
            End Try
        End Sub

        Public Function GetNativesFolder(Optional Instance As ModMinecraft.McInstance = Nothing) As String
            If Instance Is Nothing Then
                Instance = ModMinecraft.McInstanceSelected
            End If
            Dim text As String = PathUtils.AddSlashSuffix(PathUtils.ToShortPath(Instance.PathVersion)) + Instance.Name + "-natives\"
            If Not ModBase.IsGBKEncoding AndAlso Not text.IsAsciiOnly() Then
                text = Paths.AppData + ".minecraft\bin\natives\"
                If Not text.IsAsciiOnly() Then
                    text = ModBase.OsDrive + "ProgramData\PCL\natives\"
                End If
            End If
            DirectoryUtils.Create(text)
            Return PathUtils.AddSlashSuffix(PathUtils.ToShortPath(text))
        End Function

        Private Sub McLaunchPrerun()
            If Settings.[Get](Of Boolean)("LaunchAdvanceGraphicCard", Nothing) Then
                Try
                    ModMain.SetGPUPreference(ModLaunch.McLaunchJavaSelected.JavaExePath)
                    ModMain.SetGPUPreference(ModBase.PathExe)
                Catch ex As Exception
                    If WindowsUtils.HasAdminRole() Then
                        Logger.Warn(ex, "直接调整显卡设置失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                    Else
                        Logger.Warn(ex, "直接调整显卡设置失败，将以管理员权限重启 PCL 再次尝试", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                        Try
                            If ModBase.RunAsAdmin(String.Format("--gpu ""{0}""", ModLaunch.McLaunchJavaSelected.JavaExePath)) <> 5 Then
                                Throw New Exception("调整过程中出现异常")
                            End If
                            ModLaunch.McLaunchLog("以管理员权限重启 PCL 并调整显卡设置成功", LogLevel.Info)
                        Catch ex2 As Exception
                            Logger.[Error](ex2, "调整显卡设置失败，Minecraft 可能会使用默认显卡运行", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                        End Try
                    End If
                End Try
            End If
            Try
                If Operators.CompareString(ModLaunch.McLoginLoader.Output.Type, "Microsoft", False) = 0 Then
                    ModMinecraft.McFolderLauncherProfilesJsonCreate(ModMinecraft.McFolderSelected)
                    Dim jobject As JObject = CType(String.Concat(New String() { vbCrLf & "            {" & vbCrLf & "              ""authenticationDatabase"": {" & vbCrLf & "                ""00000111112222233333444445555566"": {" & vbCrLf & "                  ""username"": """, ModLaunch.McLoginLoader.Output.Name.Replace("""", "-"), """," & vbCrLf & "                  ""profiles"": {" & vbCrLf & "                    ""66666555554444433333222221111100"": {" & vbCrLf & "                        ""displayName"": """, ModLaunch.McLoginLoader.Output.Name, """" & vbCrLf & "                    }" & vbCrLf & "                  }" & vbCrLf & "                }" & vbCrLf & "              }," & vbCrLf & "              ""clientToken"": """, ModLaunch.McLoginLoader.Output.ClientToken, """," & vbCrLf & "              ""selectedUser"": {" & vbCrLf & "                ""account"": ""00000111112222233333444445555566"", " & vbCrLf & "                ""profile"": ""66666555554444433333222221111100""" & vbCrLf & "              }" & vbCrLf & "            }" }).DeserializeJson(), JObject)
                    Dim jobject2 As JObject = CType(FileUtils.ReadAsJson(ModMinecraft.McFolderSelected + "launcher_profiles.json", Nothing, Nothing), JObject)
                    jobject2.Merge(jobject)
                    FileUtils.Write(ModMinecraft.McFolderSelected + "launcher_profiles.json", jobject2.ToString(), Encoding.GetEncoding("GB18030"))
                    ModLaunch.McLaunchLog("已更新 launcher_profiles.json", LogLevel.Info)
                End If
            Catch ex3 As Exception
                Logger.Warn(ex3, "更新 launcher_profiles.json 失败，将在删除文件后重试", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                Try
                    FileUtils.Delete(ModMinecraft.McFolderSelected + "launcher_profiles.json", False)
                    ModMinecraft.McFolderLauncherProfilesJsonCreate(ModMinecraft.McFolderSelected)
                    Dim jobject3 As JObject = CType(String.Concat(New String() { vbCrLf & "                    {" & vbCrLf & "                      ""authenticationDatabase"": {" & vbCrLf & "                        ""00000111112222233333444445555566"": {" & vbCrLf & "                          ""username"": """, ModLaunch.McLoginLoader.Output.Name.Replace("""", "-"), """," & vbCrLf & "                          ""profiles"": {" & vbCrLf & "                            ""66666555554444433333222221111100"": {" & vbCrLf & "                                ""displayName"": """, ModLaunch.McLoginLoader.Output.Name, """" & vbCrLf & "                            }" & vbCrLf & "                          }" & vbCrLf & "                        }" & vbCrLf & "                      }," & vbCrLf & "                      ""clientToken"": """, ModLaunch.McLoginLoader.Output.ClientToken, """," & vbCrLf & "                      ""selectedUser"": {" & vbCrLf & "                        ""account"": ""00000111112222233333444445555566"", " & vbCrLf & "                        ""profile"": ""66666555554444433333222221111100""" & vbCrLf & "                      }" & vbCrLf & "                    }" }).DeserializeJson(), JObject)
                    Dim jobject4 As JObject = CType(FileUtils.ReadAsJson(ModMinecraft.McFolderSelected + "launcher_profiles.json", Nothing, Nothing), JObject)
                    jobject4.Merge(jobject3)
                    FileUtils.Write(ModMinecraft.McFolderSelected + "launcher_profiles.json", jobject4.ToString(), Encoding.GetEncoding("GB18030"))
                    ModLaunch.McLaunchLog("已在删除后更新 launcher_profiles.json", LogLevel.Info)
                Catch ex4 As Exception
                    Logger.[Error](ex4, "更新 launcher_profiles.json 失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                End Try
            End Try
            Dim text As String = ModMinecraft.McInstanceSelected.PathIndie + "options.txt"
            If Not FileUtils.Exists(text) Then
                Dim text2 As String = ModMinecraft.McInstanceSelected.PathIndie + "config\yosbr\options.txt"
                If FileUtils.Exists(text2) Then
                    ModLaunch.McLaunchLog("将修改 Yosbr Mod 中的 options.txt", LogLevel.Info)
                    text = text2
                    ModBase.WriteIni(text, "lang", "none")
                End If
            End If
            Try
                Dim text3 As String = ModBase.ReadIni(text, "lang", "none")
                Dim text4 As String = If((Operators.CompareString(text3, "none", False) = 0 OrElse Not DirectoryUtils.Exists(ModMinecraft.McInstanceSelected.PathIndie + "saves")), If(Settings.[Get](Of Boolean)("ToolHelpChinese", Nothing), "zh_cn", "en_us"), text3.Lower())
                If Not ModMinecraft.McInstanceSelected.Version.Valid OrElse ModMinecraft.McInstanceSelected.Version.Vanilla.Major <= 10 Then
                    text4 = text4.Substring(0, text4.Length - 2) + text4.Substring(text4.Length - 2).Upper()
                End If
                If Operators.CompareString(text3, text4, False) = 0 Then
                    ModLaunch.McLaunchLog(String.Format("需要的语言为 {0}，当前语言为 {1}，无需修改", text4, text3), LogLevel.Info)
                Else
                    ModBase.WriteIni(text, "lang", "-")
                    ModBase.WriteIni(text, "lang", text4)
                    ModLaunch.McLaunchLog(String.Format("已将语言从 {0} 修改为 {1}", text3, text4), LogLevel.Info)
                End If
                Dim num As Integer = Settings.[Get](Of Integer)("LaunchArgumentWindowType", Nothing)
                If num <> 0 Then
                    If num <> 1 Then
                        ModBase.WriteIni(text, "fullscreen", "false")
                    End If
                Else
                    ModBase.WriteIni(text, "fullscreen", "true")
                End If
            Catch ex5 As Exception
                Logger.[Error](ex5, "更新 options.txt 失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
            End Try
            If ModMinecraft.McInstanceSelected.Version.Vanilla.Major <= 7 AndAlso ModMinecraft.McInstanceSelected.Version.Vanilla.Major >= 2 AndAlso ModLaunch.McLoginLoader.Input.Type = ModLaunch.McLoginType.Legacy AndAlso (Settings.[Get](Of Integer)("LaunchSkinType", Nothing) = 2 OrElse (Settings.[Get](Of Integer)("LaunchSkinType", Nothing) = 4 AndAlso Settings.[Get](Of Boolean)("LaunchSkinSlim", Nothing))) Then
                ModMain.Hint("此 Minecraft 版本尚不支持 Alex 皮肤，你的皮肤可能会显示为 Steve！", ModMain.HintType.Red, True)
            End If
            Try
                Dim text5 As String = ModMinecraft.McInstanceSelected.PathIndie + "resourcepacks\PCL2 Skin.zip"
                Dim flag As Boolean = ModMinecraft.McInstanceSelected.Version.Vanilla.Major >= 13 OrElse ModMinecraft.McInstanceSelected.Version.Vanilla.Major < 6
                If ModLaunch.McLoginLoader.Input.Type = ModLaunch.McLoginType.Legacy AndAlso Settings.[Get](Of Integer)("LaunchSkinType", Nothing) = 4 AndAlso FileUtils.Exists(Paths.AppDataThenName + "CustomSkin.png") Then
                    Dim text6 As String = ModBase.PathTemp + "pack.mcmeta"
                    Dim text7 As String = ModBase.PathTemp + "pack.png"
                    If ModMinecraft.McInstanceSelected.Version.Vanilla.Major < 6 Then
                        ModLaunch.McLaunchLog("Minecraft 版本过老，尚不支持自定义离线皮肤", LogLevel.Info)
                    Else
                        Dim jsonVersion As JObject = ModMinecraft.McInstanceSelected.JsonVersion
                        Dim jtoken As JToken = If((jsonVersion IsNot Nothing), jsonVersion("pack_version"), Nothing)
                        If TypeOf jtoken Is JObject Then
                            jtoken = CType(jtoken, JObject)("resource")
                        End If
                        Dim num2 As Integer = CInt(Math.Round(ModBase.Val(If((jtoken IsNot Nothing), jtoken.ToString(), Nothing))))
                        If num2 <= 0 Then
                            Select Case ModMinecraft.McInstanceSelected.Version.Vanilla.Major
                                Case 6, 7, 8
                                    num2 = 1
                                Case 9, 10
                                    num2 = 2
                                Case 11, 12
                                    num2 = 3
                                Case 13, 14
                                    num2 = 4
                                Case 15
                                    num2 = 5
                                Case 16
                                    num2 = If((ModMinecraft.McInstanceSelected.Version.Vanilla.Build <= 1), 5, 6)
                                Case 17
                                    num2 = 7
                                Case 18
                                    num2 = 8
                                Case Else
                                    num2 = 17
                            End Select
                        End If
                        ModLaunch.McLaunchLog(String.Format("正在构建自定义皮肤资源包，版本号 {0}，MC 版本 {1}", num2, ModMinecraft.McInstanceSelected.Version.Vanilla), LogLevel.Info)
                        New MyBitmap(ModBase.PathImage + "Heads/Logo.png").Save(text7)
                        FileUtils.Write(text6, "{""pack"":{""pack_format"":" + Conversions.ToString(num2) + ",""description"":""PCL 自定义离线皮肤资源包""}}", Nothing)
                        Dim myBitmap As MyBitmap = New MyBitmap(Paths.AppDataThenName + "CustomSkin.png")
                        If(ModMinecraft.McInstanceSelected.Version.Vanilla.Major = 6 OrElse ModMinecraft.McInstanceSelected.Version.Vanilla.Major = 7) AndAlso myBitmap.Pic.Height = 64 Then
                            ModLaunch.McLaunchLog("该 Minecraft 版本不支持双层皮肤，已进行裁剪", LogLevel.Info)
                            myBitmap = myBitmap.Clip(0, 0, 64, 32)
                        End If
                        myBitmap.Save(Paths.Base + "PCL\CustomSkin_Cliped.png")
                        Dim dictionary As Dictionary(Of String, String) = New Dictionary(Of String, String)() From { { "pack.mcmeta", text6 }, { "pack.png", text7 } }
                        If ModMinecraft.McInstanceSelected.Version.Vanilla >= New Version(19, 0, 3) Then
                            For Each text8 As String In New String() { "alex", "ari", "efe", "kai", "makena", "noor", "steve", "sunny", "zuri" }
                                dictionary.Add(String.Format("assets/minecraft/textures/entity/player/{0}/{1}.png", If(Settings.[Get](Of Boolean)("LaunchSkinSlim", Nothing), "slim", "wide"), text8), Paths.Base + "PCL\CustomSkin_Cliped.png")
                            Next
                        Else
                            dictionary.Add(String.Format("assets/minecraft/textures/entity/{0}", If(Settings.[Get](Of Boolean)("LaunchSkinSlim", Nothing), "alex.png", "steve.png")), Paths.Base + "PCL\CustomSkin_Cliped.png")
                        End If
                        FileUtils.CreateZipFromFiles(text5, dictionary)
                        FileUtils.Delete(Paths.Base + "PCL\CustomSkin_Cliped.png", False)
                        ModBase.IniClearCache(text)
                        Dim text9 As String = ModBase.ReadIni(text, "resourcePacks", "[]").TrimStart(New Char() { "["c }).TrimEnd(New Char() { "]"c })
                        If flag Then
                            If Operators.CompareString(text9, "", False) = 0 Then
                                text9 = """vanilla"""
                            End If
                            Dim list As List(Of String) = New List(Of String)(text9.Split(",", False))
                            Dim list2 As List(Of String) = New List(Of String)()
                            Try
                                For Each text10 As String In list
                                    If Operators.CompareString(text10, """file/PCL2 Skin.zip""", False) <> 0 AndAlso Operators.CompareString(text10, "", False) <> 0 Then
                                        list2.Add(text10)
                                    End If
                                Next
                            Finally
                                Dim enumerator As List(Of String).Enumerator
                                CType(enumerator, IDisposable).Dispose()
                            End Try
                            list2.Add("""file/PCL2 Skin.zip""")
                            Dim text11 As String = "[" + list2.Join(","c) + "]"
                            ModBase.WriteIni(text, "resourcePacks", text11)
                        Else
                            Dim list3 As List(Of String) = New List(Of String)(text9.Split(",", False))
                            Dim list4 As List(Of String) = New List(Of String)()
                            Try
                                For Each text12 As String In list3
                                    If Operators.CompareString(text12, """PCL2 Skin.zip""", False) <> 0 AndAlso Operators.CompareString(text12, "", False) <> 0 Then
                                        list4.Add(text12)
                                    End If
                                Next
                            Finally
                                Dim enumerator2 As List(Of String).Enumerator
                                CType(enumerator2, IDisposable).Dispose()
                            End Try
                            list4.Add("""PCL2 Skin.zip""")
                            Dim text13 As String = "[" + list4.Join(","c) + "]"
                            ModBase.WriteIni(text, "resourcePacks", text13)
                        End If
                    End If
                ElseIf FileUtils.Exists(text5) Then
                    ModLaunch.McLaunchLog("正在清空自定义皮肤资源包", LogLevel.Info)
                    FileUtils.Delete(text5, False)
                    ModBase.IniClearCache(text)
                    Dim text14 As String = ModBase.ReadIni(text, "resourcePacks", "[]").TrimStart(New Char() { "["c }).TrimEnd(New Char() { "]"c })
                    If flag Then
                        If Operators.CompareString(text14, "", False) = 0 Then
                            text14 = """vanilla"""
                        End If
                        Dim list5 As List(Of String) = New List(Of String)(text14.Split(",", False))
                        list5.Remove("""file/PCL2 Skin.zip""")
                        Dim text15 As String = "[" + list5.Join(","c) + "]"
                        ModBase.WriteIni(text, "resourcePacks", text15)
                    Else
                        Dim list6 As List(Of String) = New List(Of String)(text14.Split(",", False))
                        list6.Remove("""PCL2 Skin.zip""")
                        Dim text16 As String = "[" + list6.Join(","c) + "]"
                        ModBase.WriteIni(text, "resourcePacks", text16)
                    End If
                End If
            Catch ex6 As Exception
                Logger.[Error](ex6, "离线皮肤资源包设置失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
            End Try
        End Sub

        Private Sub McLaunchCustom(Loader As ModLoader.LoaderTask(Of Integer, Integer))
            Dim text As String = Settings.[Get](Of String)("LaunchAdvanceRun", Nothing)
            If Operators.CompareString(text, "", False) <> 0 Then
                text = ModMain.ArgumentReplace(text, Nothing, True)
            End If
            Dim text2 As String = Settings.[Get](Of String)("VersionAdvanceRun", ModMinecraft.McInstanceSelected)
            If Operators.CompareString(text2, "", False) <> 0 Then
                text2 = ModMain.ArgumentReplace(text2, Nothing, True)
            End If
            Try
                Dim text3 As String = String.Concat(New String() { String.Format("chcp {0}>nul", If((ModLaunch.McLaunchEncoding.CodePage = 54936 AndAlso ModBase.IsGBKEncoding), 936, ModLaunch.McLaunchEncoding.CodePage)), vbCrLf & "@echo off" & vbCrLf, String.Format("title 启动 - {0}", ModMinecraft.McInstanceSelected.Name), vbCrLf & "echo 游戏正在启动，请稍候。" & vbCrLf, String.Format("cd /D ""{0}""", PathUtils.ToShortPath(ModMinecraft.McInstanceSelected.PathIndie)), vbCrLf, text, vbCrLf, text2, vbCrLf, String.Format("""{0}"" {1}", ModLaunch.McLaunchJavaSelected.JavaExePath, ModLaunch.McLaunchArgument), vbCrLf & "echo 游戏已退出。" & vbCrLf & "pause" })
                FileUtils.Write(If(ModLaunch.CurrentLaunchOptions.SaveBatch, (Paths.Base + "PCL\LatestLaunch.bat")), ModMinecraft.FilterAccessToken(text3, "F"c).Replace("%", "%%"), ModLaunch.McLaunchEncoding)
                If ModLaunch.CurrentLaunchOptions.SaveBatch IsNot Nothing Then
                    ModLaunch.McLaunchLog("导出启动脚本完成，强制结束启动过程", LogLevel.Info)
                    ModLaunch.CanceledHint = "导出启动脚本成功！"
                    ModBase.OpenExplorer(ModLaunch.CurrentLaunchOptions.SaveBatch)
                    Loader.Parent.Cancel()
                    Return
                End If
            Catch ex As Exception
                Logger.Warn(ex, "输出启动脚本失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                If ModLaunch.CurrentLaunchOptions.SaveBatch IsNot Nothing Then
                    Throw ex
                End If
            End Try
            If Operators.CompareString(text, "", False) <> 0 Then
                ModLaunch.McLaunchLog("正在执行全局自定义命令：" + text, LogLevel.Info)
                Dim process As Process = Nothing
                Try
                    process = ModBase.StartProcess(New ProcessStartInfo() With { .FileName = "cmd.exe", .Arguments = String.Format("/c ""{0}""", text), .WorkingDirectory = ModMinecraft.McFolderSelected, .UseShellExecute = False, .CreateNoWindow = True })
                    If Settings.[Get](Of Boolean)("LaunchAdvanceRunWait", Nothing) Then
                        While Not process.HasExited AndAlso Not Loader.IsCanceled
                            Thread.Sleep(10)
                        End While
                    End If
                Catch ex2 As Exception
                    Logger.[Error](ex2, "执行全局自定义命令失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                Finally
                    If process IsNot Nothing AndAlso Not process.HasExited AndAlso Loader.IsCanceled Then
                        ModLaunch.McLaunchLog("由于取消启动，已强制结束自定义命令 CMD 进程", LogLevel.Info)
                        process.Kill()
                    End If
                End Try
            End If
            If Operators.CompareString(text2, "", False) <> 0 Then
                ModLaunch.McLaunchLog("正在执行版本自定义命令：" + text2, LogLevel.Info)
                Dim process2 As Process = Nothing
                Try
                    process2 = ModBase.StartProcess(New ProcessStartInfo() With { .FileName = "cmd.exe", .Arguments = String.Format("/c ""{0}""", text2), .WorkingDirectory = ModMinecraft.McFolderSelected, .UseShellExecute = False, .CreateNoWindow = True })
                    If Settings.[Get](Of Boolean)("VersionAdvanceRunWait", ModMinecraft.McInstanceSelected) Then
                        While Not process2.HasExited AndAlso Not Loader.IsCanceled
                            Thread.Sleep(10)
                        End While
                    End If
                Catch ex3 As Exception
                    Logger.[Error](ex3, "执行版本自定义命令失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                Finally
                    If process2 IsNot Nothing AndAlso Not process2.HasExited AndAlso Loader.IsCanceled Then
                        ModLaunch.McLaunchLog("由于取消启动，已强制结束自定义命令 CMD 进程", LogLevel.Info)
                        process2.Kill()
                    End If
                End Try
            End If
        End Sub

        Private Sub McLaunchRun(Loader As ModLoader.LoaderTask(Of Integer, Process))
            Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo(ModLaunch.McLaunchJavaSelected.JavaExePath)
            Dim text As String = processStartInfo.EnvironmentVariables("Path")
            Dim list As List(Of String) = New List(Of String)()
            If Not String.IsNullOrEmpty(text) Then
                list.AddRange(text.Split(";", False))
            End If
            list.Add(PathUtils.ToShortPath(ModLaunch.McLaunchJavaSelected.Folder))
            processStartInfo.EnvironmentVariables("Path") = list.Distinct().Join(";"c)
            processStartInfo.EnvironmentVariables("appdata") = PathUtils.ToShortPath(ModMinecraft.McFolderSelected)
            processStartInfo.WorkingDirectory = ModMinecraft.McInstanceSelected.PathIndie
            processStartInfo.UseShellExecute = False
            processStartInfo.StandardOutputEncoding = ModLaunch.McLaunchEncoding
            processStartInfo.StandardErrorEncoding = ModLaunch.McLaunchEncoding
            processStartInfo.RedirectStandardOutput = True
            processStartInfo.RedirectStandardError = True
            processStartInfo.CreateNoWindow = True
            processStartInfo.Arguments = ModLaunch.McLaunchArgument
            Dim process As Process = ModBase.StartProcess(processStartInfo)
            ModLaunch.McLaunchLog("已启动游戏进程：" + ModLaunch.McLaunchJavaSelected.JavaExePath, LogLevel.Info)
            If Loader.IsCanceled Then
                ModLaunch.McLaunchLog("由于取消启动，已强制结束游戏进程", LogLevel.Info)
                process.Kill()
                Return
            End If
            Loader.Output = process
            ModLaunch.McLaunchProcess = process
            Try
                process.PriorityBoostEnabled = True
                Dim num As Integer = Settings.[Get](Of Integer)("LaunchArgumentPriority", Nothing)
                If num <> 0 Then
                    If num = 2 Then
                        process.PriorityClass = ProcessPriorityClass.BelowNormal
                    End If
                Else
                    process.PriorityClass = ProcessPriorityClass.AboveNormal
                End If
            Catch ex As Exception
                Logger.[Error](ex, "设置进程优先级失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
            End Try
        End Sub

        Private Sub McLaunchWait(Loader As ModLoader.LoaderTask(Of Process, Integer))
            ModLaunch.McLaunchLog("", LogLevel.Info)
            ModLaunch.McLaunchLog("~ 基础参数 ~", LogLevel.Info)
            ModLaunch.McLaunchLog("PCL 版本：快照版 2.99.0.0 (" + Conversions.ToString(414) + ")", LogLevel.Info)
            ModLaunch.McLaunchLog(String.Format("游戏版本：{0}（{1}，Drop {2}{3}）", New Object() { ModMinecraft.McInstanceSelected.VersionDisplayName(), ModMinecraft.McInstanceSelected.Version.Vanilla, ModMinecraft.McInstanceSelected.Version.Drop, If(ModMinecraft.McInstanceSelected.Version.Reliable, "", "，无法完全确定") }), LogLevel.Info)
            ModLaunch.McLaunchLog("资源版本：" + ModMinecraft.McAssetsGetIndexName(ModMinecraft.McInstanceSelected), LogLevel.Info)
            ModLaunch.McLaunchLog("版本继承：" + If((Operators.CompareString(ModMinecraft.McInstanceSelected.InheritName, "", False) = 0), "无", ModMinecraft.McInstanceSelected.InheritName), LogLevel.Info)
            ModLaunch.McLaunchLog(String.Concat(New String() { "分配的内存：", Conversions.ToString(PageInstanceSetup.GetRam(ModMinecraft.McInstanceSelected)), " GB（", Conversions.ToString(Math.Round(PageInstanceSetup.GetRam(ModMinecraft.McInstanceSelected) * 1024.0)), " MB）" }), LogLevel.Info)
            ModLaunch.McLaunchLog("MC 文件夹：" + ModMinecraft.McFolderSelected, LogLevel.Info)
            ModLaunch.McLaunchLog("版本文件夹：" + ModMinecraft.McInstanceSelected.PathVersion, LogLevel.Info)
            ModLaunch.McLaunchLog("版本隔离：" + Conversions.ToString(Operators.CompareString(ModMinecraft.McInstanceSelected.PathIndie, ModMinecraft.McInstanceSelected.PathVersion, False) = 0), LogLevel.Info)
            ModLaunch.McLaunchLog("HMCL 格式：" + Conversions.ToString(ModMinecraft.McInstanceSelected.IsHmclFormatJson), LogLevel.Info)
            ModLaunch.McLaunchLog("Java 信息：" + If((ModLaunch.McLaunchJavaSelected IsNot Nothing), ModLaunch.McLaunchJavaSelected.ToString(), "无可用 Java"), LogLevel.Info)
            ModLaunch.McLaunchLog("Natives 文件夹：" + ModLaunch.GetNativesFolder(Nothing), LogLevel.Info)
            ModLaunch.McLaunchLog("", LogLevel.Info)
            ModLaunch.McLaunchLog("~ 登录参数 ~", LogLevel.Info)
            ModLaunch.McLaunchLog("玩家用户名：" + ModLaunch.McLoginLoader.Output.Name, LogLevel.Info)
            ModLaunch.McLaunchLog("AccessToken：" + ModLaunch.McLoginLoader.Output.AccessToken, LogLevel.Info)
            ModLaunch.McLaunchLog("ClientToken：" + ModLaunch.McLoginLoader.Output.ClientToken, LogLevel.Info)
            ModLaunch.McLaunchLog("UUID：" + ModLaunch.McLoginLoader.Output.Uuid, LogLevel.Info)
            ModLaunch.McLaunchLog("登录方式：" + ModLaunch.McLoginLoader.Output.Type, LogLevel.Info)
            ModLaunch.McLaunchLog("", LogLevel.Info)
            Dim text As String = Settings.[Get](Of String)("VersionArgumentTitle", ModMinecraft.McInstanceSelected)
            If Operators.CompareString(text, "", False) = 0 Then
                text = Settings.[Get](Of String)("LaunchArgumentTitle", Nothing)
            End If
            text = ModMain.ArgumentReplace(text, Nothing, False)
            Dim watcher As ModWatcher.Watcher = New ModWatcher.Watcher(Loader, ModMinecraft.McInstanceSelected, text)
            ModLaunch.McLaunchWatcher = watcher
            While watcher.State = ModWatcher.Watcher.MinecraftState.Loading
                Thread.Sleep(100)
            End While
            If watcher.State = ModWatcher.Watcher.MinecraftState.Crashed Then
                Throw New OperationCanceledException()
            End If
        End Sub

        Private Sub McLaunchEnd()
            ModLaunch.McLaunchLog("开始启动结束处理", LogLevel.Info)
            If Settings.[Get](Of Boolean)("UiMusicStop", Nothing) Then
                Dim action As Action
                If ModLaunch._Closure$__.$I62-0 IsNot Nothing Then
                    action = ModLaunch._Closure$__.$I62-0
                Else
                    Dim action2 As Action = Sub()
                        If ModMusic.MusicPause() Then
                            Logger.Info("已根据设置，在启动后暂停音乐播放", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                        End If
                    End Sub
                    action = action2
                    ModLaunch._Closure$__.$I62-0 = action2
                End If
                ModBase.RunInUi(action, False)
            ElseIf Settings.[Get](Of Boolean)("UiMusicStart", Nothing) Then
                Dim action3 As Action
                If ModLaunch._Closure$__.$I62-1 IsNot Nothing Then
                    action3 = ModLaunch._Closure$__.$I62-1
                Else
                    Dim action4 As Action = Sub()
                        If ModMusic.MusicResume() Then
                            Logger.Info("已根据设置，在启动后开始音乐播放", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModLaunch.vb")
                        End If
                    End Sub
                    action3 = action4
                    ModLaunch._Closure$__.$I62-1 = action4
                End If
                ModBase.RunInUi(action3, False)
            End If
            ModLaunch.McLaunchLog("启动器可见性：" + Conversions.ToString(Settings.[Get](Of Integer)("LaunchArgumentVisible", Nothing)), LogLevel.Info)
            Select Case Settings.[Get](Of Integer)("LaunchArgumentVisible", Nothing)
                Case 0
                    ModLaunch.McLaunchLog("已根据设置，在启动后关闭启动器", LogLevel.Info)
                    Dim action5 As Action
                    If ModLaunch._Closure$__.$I62-2 IsNot Nothing Then
                        action5 = ModLaunch._Closure$__.$I62-2
                    Else
                        Dim action6 As Action = Sub()
                            ModMain.FrmMain.EndProgram(False)
                        End Sub
                        action5 = action6
                        ModLaunch._Closure$__.$I62-2 = action6
                    End If
                    ModBase.RunInUi(action5, False)
                Case 2, 3
                    ModLaunch.McLaunchLog("已根据设置，在启动后隐藏启动器", LogLevel.Info)
                    Dim action7 As Action
                    If ModLaunch._Closure$__.$I62-3 IsNot Nothing Then
                        action7 = ModLaunch._Closure$__.$I62-3
                    Else
                        Dim action8 As Action = Sub()
                            ModMain.FrmMain.Hidden = True
                        End Sub
                        action7 = action8
                        ModLaunch._Closure$__.$I62-3 = action8
                    End If
                    ModBase.RunInUi(action7, False)
                Case 4
                    ModLaunch.McLaunchLog("已根据设置，在启动后最小化启动器", LogLevel.Info)
                    Dim action9 As Action
                    If ModLaunch._Closure$__.$I62-4 IsNot Nothing Then
                        action9 = ModLaunch._Closure$__.$I62-4
                    Else
                        Dim action10 As Action = Sub()
                            ModMain.FrmMain.WindowState = WindowState.Minimized
                        End Sub
                        action9 = action10
                        ModLaunch._Closure$__.$I62-4 = action10
                    End If
                    ModBase.RunInUi(action9, False)
            End Select
            Settings.[Set]("SystemLaunchCount", Settings.[Get](Of Integer)("SystemLaunchCount", Nothing) + 1, Nothing)
        End Sub

        Public CurrentLaunchOptions As ModLaunch.McLaunchOptions = Nothing

        Public McLaunchLoader As ModLoader.LoaderTask(Of ModLaunch.McLaunchOptions, Object) = New ModLoader.LoaderTask(Of ModLaunch.McLaunchOptions, Object)("Loader Launch", AddressOf ModLaunch.McLaunchStart, Nothing, ThreadPriority.Normal) With { .OnStateChanged = Sub(a0 As ModLoader.LoaderBase)
            ModLaunch.McLaunchState(CType(a0, ModLoader.LoaderTask(Of ModLaunch.McLaunchOptions, Object)))
        End Sub }

        Public McLaunchLoaderReal As ModLoader.LoaderCombo(Of Object)

        Public McLaunchProcess As Process

        Public McLaunchWatcher As ModWatcher.Watcher

        Private CanceledHint As String = Nothing

        Public McLoginLoader As ModLoader.LoaderTask(Of ModLaunch.McLoginData, ModLaunch.McLoginResult) = New ModLoader.LoaderTask(Of ModLaunch.McLoginData, ModLaunch.McLoginResult)("登录", AddressOf ModLaunch.McLoginStart, AddressOf ModLaunch.McLoginInput, ThreadPriority.BelowNormal) With { .ReloadTimeout = 1, .ProgressWeight = 15.0, .Block = False }

        Public McLoginMsLoader As ModLoader.LoaderTask(Of ModLaunch.McLoginMs, ModLaunch.McLoginResult) = New ModLoader.LoaderTask(Of ModLaunch.McLoginMs, ModLaunch.McLoginResult)("Loader Login Ms", AddressOf ModLaunch.McLoginMsStart, Nothing, ThreadPriority.Normal) With { .ReloadTimeout = 1 }

        Public McLoginLegacyLoader As ModLoader.LoaderTask(Of ModLaunch.McLoginLegacy, ModLaunch.McLoginResult) = New ModLoader.LoaderTask(Of ModLaunch.McLoginLegacy, ModLaunch.McLoginResult)("Loader Login Legacy", AddressOf ModLaunch.McLoginLegacyStart, Nothing, ThreadPriority.Normal)

        Public McLoginNideLoader As ModLoader.LoaderTask(Of ModLaunch.McLoginServer, ModLaunch.McLoginResult) = New ModLoader.LoaderTask(Of ModLaunch.McLoginServer, ModLaunch.McLoginResult)("Loader Login Nide", AddressOf ModLaunch.McLoginServerStart, Nothing, ThreadPriority.Normal) With { .ReloadTimeout = 600000 }

        Public McLoginAuthLoader As ModLoader.LoaderTask(Of ModLaunch.McLoginServer, ModLaunch.McLoginResult) = New ModLoader.LoaderTask(Of ModLaunch.McLoginServer, ModLaunch.McLoginResult)("Loader Login Auth", AddressOf ModLaunch.McLoginServerStart, Nothing, ThreadPriority.Normal) With { .ReloadTimeout = 600000 }

        Public McLaunchJavaSelected As Java = Nothing

        Private McLaunchArgument As String

        Private McLaunchEncoding As Encoding

        Public Class McLaunchOptions
            Public Sub New()
                Me.ServerIp = Nothing
                Me.SaveBatch = Nothing
                Me.Instance = Nothing
                Me.ExtraGameArgs = New List(Of String)()
            End Sub

            Public ServerIp As String

            Public SaveBatch As String

            Public Instance As ModMinecraft.McInstance

            Public ExtraGameArgs As List(Of String)
        End Class

        Public Enum McLoginType
            Legacy
            Nide = 2
            Auth
            Ms = 5
        End Enum

        Public MustInherit Class McLoginData
            Public Overrides Function Equals(obj As Object) As Boolean
                Return obj IsNot Nothing AndAlso obj.GetHashCode() = Me.GetHashCode()
            End Function

            Public Type As ModLaunch.McLoginType
        End Class

        Public Class McLoginServer
            Inherits ModLaunch.McLoginData

            Public Sub New(Type As ModLaunch.McLoginType)
                Me.ForceReselectProfile = False
                Me.Type = Type
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return Convert.ToInt32(Decimal.Remainder(New Decimal(String.Concat(New String() { Me.UserName, Me.Password, Me.BaseUrl, Me.Token, Conversions.ToString(CInt(Me.Type)) }).GetStableHashCode()), 2147483647D))
            End Function

            Public UserName As String

            Public Password As String

            Public BaseUrl As String

            Public Token As String

            Public Description As String

            Public ForceReselectProfile As Boolean
        End Class

        Public Class McLoginMs
            Inherits ModLaunch.McLoginData

            Public Sub New()
                Me.OAuthRefreshToken = ""
                Me.AccessToken = ""
                Me.Uuid = ""
                Me.UserName = ""
                Me.ProfileJson = ""
                Me.Type = ModLaunch.McLoginType.Ms
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return Convert.ToInt32(Decimal.Remainder(New Decimal((Me.OAuthRefreshToken + Me.UserName).GetStableHashCode()), 2147483647D))
            End Function

            Public OAuthRefreshToken As String

            Public AccessToken As String

            Public Uuid As String

            Public UserName As String

            Public ProfileJson As String
        End Class

        Public Class McLoginLegacy
            Inherits ModLaunch.McLoginData

            Public Sub New()
                Me.Type = ModLaunch.McLoginType.Legacy
            End Sub

            Public Overrides Function GetHashCode() As Integer
                Return Convert.ToInt32(Decimal.Remainder(New Decimal((Me.UserName + Conversions.ToString(Me.SkinType) + Me.SkinName + Conversions.ToString(CInt(Me.Type))).GetStableHashCode()), 2147483647D))
            End Function

            Public UserName As String

            Public SkinType As Integer

            Public SkinName As String
        End Class

        Public Structure McLoginResult
            Public Name As String

            Public Uuid As String

            Public AccessToken As String

            Public Type As String

            Public ClientToken As String

            Public ProfileJson As String
        End Structure
    End Module
End Namespace
