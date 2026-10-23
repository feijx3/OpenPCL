Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Linq
Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Interop
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Effects
Imports System.Windows.Shapes
Imports MeloongCore
Imports MeloongCore.Extensions
Imports MeloongCore.Wpf.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports PCL.My

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class FormMain
        Inherits Window

        Private Sub ShowUpdateLog(LastVersion As Integer)
            Dim CS$<>8__locals1 As FormMain._Closure$__1-0 = New FormMain._Closure$__1-0(CS$<>8__locals1)
            Dim num As Integer = 0
            Dim num2 As Integer = 0
            Dim list As List(Of KeyValuePair(Of Integer, String)) = New List(Of KeyValuePair(Of Integer, String))()
            If LastVersion < 414 Then
                list.Add(New KeyValuePair(Of Integer, String)(5, "删除：为遵守相关法律法规，PCL 暂时不再提供 MC 下载功能，非常抱歉 qwq……"))
            End If
            If LastVersion < 412 Then
                list.Add(New KeyValuePair(Of Integer, String)(4, "新增：允许修改、添加、移除、更新已安装版本的各个组件，可从版本设置中进入"))
                list.Add(New KeyValuePair(Of Integer, String)(2, "优化：启动游戏时允许按 Esc 取消"))
                list.Add(New KeyValuePair(Of Integer, String)(2, "优化：允许复制主页的启动日志中的内容"))
                num += 19
                num2 += 26
            End If
            If LastVersion < 410 Then
                If LastVersion = 409 Then
                    list.Add(New KeyValuePair(Of Integer, String)(1, "修复：更新 Mod 时可能更新成另一个 Mod 加载器的版本"))
                End If
                num += 2
                num2 += 6
            End If
            If LastVersion < 409 Then
                list.Add(New KeyValuePair(Of Integer, String)(2, "优化：版本设置中，添加打开截图文件夹的快捷方式"))
                list.Add(New KeyValuePair(Of Integer, String)(2, "优化：导出整合包时，可以勾选导出光影设置文件"))
                num += 21
                num2 += 22
            End If
            If LastVersion < 407 Then
                If LastVersion = 405 Then
                    list.Add(New KeyValuePair(Of Integer, String)(1, "修复：使用部分主页预设时崩溃"))
                    list.Add(New KeyValuePair(Of Integer, String)(1, "修复：无法删除部分文件夹"))
                    list.Add(New KeyValuePair(Of Integer, String)(1, "修复：版本设置中指定的 Java 无法被保存"))
                    num += 1
                    num2 += 3
                End If
                num += 2
            End If
            If LastVersion < 405 Then
                list.Add(New KeyValuePair(Of Integer, String)(4, "新增：重做 Java 管理与相关设置，允许调整 Java 优先级、指定 Java 版本范围等"))
                list.Add(New KeyValuePair(Of Integer, String)(3, "优化：导出整合包时允许自动导出版本文件夹中的 Java"))
                list.Add(New KeyValuePair(Of Integer, String)(3, "优化：重做弹出提示的样式以及动画，以更符合现代 UI 审美"))
                list.Add(New KeyValuePair(Of Integer, String)(1, "修复：可能无法安装 OptiFine 26.1.2"))
                num += 35
                num2 += 17
            End If
            If LastVersion < 403 Then
                list.Add(New KeyValuePair(Of Integer, String)(1, "修复：Windows 7 无法正常联网"))
                num += 1
                num2 += 2
            End If
            If LastVersion < 401 Then
                list.Add(New KeyValuePair(Of Integer, String)(1, "修复：可能无法更换披风"))
                num += 8
                num2 += 6
            End If
            If LastVersion < 399 Then
                list.Add(New KeyValuePair(Of Integer, String)(2, "优化：升级框架到 .NET Framework 4.8"))
                num += 28
                num2 += 19
            End If
            If LastVersion < 397 AndAlso LastVersion >= 393 AndAlso LastVersion < 397 Then
                list.Add(New KeyValuePair(Of Integer, String)(1, "修复：无法使用自定义隐藏主题"))
            End If
            If LastVersion < 395 AndAlso LastVersion = 393 Then
                list.Add(New KeyValuePair(Of Integer, String)(2, "修复：部分个性化功能失效"))
            End If
            If LastVersion < 393 Then
                list.Add(New KeyValuePair(Of Integer, String)(2, "优化：允许永久关闭启动页面的快照版提示"))
                list.Add(New KeyValuePair(Of Integer, String)(2, "优化：下载游戏、整合包的稳定性"))
                num += 1
                num2 += 5
            End If
            If LastVersion < 390 Then
                list.Add(New KeyValuePair(Of Integer, String)(1, "修复：会为部分 Forge / NeoForge 选择不支持的 Java"))
                num2 += 4
            End If
            If LastVersion < 388 Then
                If LastVersion = 386 Then
                    list.Add(New KeyValuePair(Of Integer, String)(2, "修复：无法使用 Java 25+ 启动 Forge"))
                End If
                num += 4
                num2 += 3
            End If
            If LastVersion < 386 Then
                list.Add(New KeyValuePair(Of Integer, String)(2, "修复：Minecraft 使用 Java 25+ 时的一个性能问题"))
                If LastVersion = 384 OrElse LastVersion = 385 Then
                    list.Add(New KeyValuePair(Of Integer, String)(2, "修复：无法正常重命名游戏版本"))
                End If
                If LastVersion = 385 Then
                    list.Add(New KeyValuePair(Of Integer, String)(1, "修复：无法显示 Modrinth 整合包的版本列表"))
                End If
                num2 += 1
            End If
            If LastVersion < 385 Then
                list.Add(New KeyValuePair(Of Integer, String)(2, "修复：无法下载 NeoForge 26.1"))
                If LastVersion = 384 Then
                    list.Add(New KeyValuePair(Of Integer, String)(2, "修复：下载可能失败，提示任务管理刷新线程出错"))
                    list.Add(New KeyValuePair(Of Integer, String)(2, "修复：无法保存选择的 Minecraft 文件夹"))
                    list.Add(New KeyValuePair(Of Integer, String)(1, "修复：使用中文搜索 Mod 时，部分结果会忽略筛选条件"))
                End If
                num += 3
                num2 += 6
            End If
            If LastVersion < 384 Then
                list.Add(New KeyValuePair(Of Integer, String)(3, "优化：降低 Minecraft 的内存占用"))
                list.Add(New KeyValuePair(Of Integer, String)(2, "修复：部分关键词搜不到资源，特别是中文 Mod 搜索经常没有结果"))
                list.Add(New KeyValuePair(Of Integer, String)(1, "修复：老版本 Windows 无法启动游戏"))
                list.Add(New KeyValuePair(Of Integer, String)(1, "修复：连接 Mojang 的服务可能失败，提示错误码 421"))
                list.Add(New KeyValuePair(Of Integer, String)(1, "修复：无法访问爱发电"))
                num += 21
                num2 += 14
            End If
            If LastVersion < 382 Then
                list.Add(New KeyValuePair(Of Integer, String)(2, "修复：下载可能完全卡住，或是下载进度反复回退"))
                list.Add(New KeyValuePair(Of Integer, String)(1, "新增：内存管理设置，可以选择 G1GC、ZGC 或分代 ZGC"))
                list.Add(New KeyValuePair(Of Integer, String)(1, "优化：来自 Modrinth 的文件下载速度"))
                num += 15
                num2 += 16
            End If
            If LastVersion < 380 Then
                num += 3
                num2 += 13
            End If
            If LastVersion < 378 AndAlso LastVersion >= 377 Then
                list.Add(New KeyValuePair(Of Integer, String)(1, "修复：在版本列表中存在 OptiFine 时可能无法加载版本列表"))
                num2 += 2
            End If
            If LastVersion < 377 Then
                If LastVersion >= 373 Then
                    list.Add(New KeyValuePair(Of Integer, String)(5, "删除：暂时隐藏联机入口……不过只是暂时关闭，它还会回来的！"))
                End If
                list.Add(New KeyValuePair(Of Integer, String)(3, "新增：适配新的 Minecraft 版本号系统与 Unobfuscated 版本"))
                list.Add(New KeyValuePair(Of Integer, String)(2, "优化：巨幅优化各个下载页面和 Mod 管理页面的性能"))
                list.Add(New KeyValuePair(Of Integer, String)(1, "优化：更换披风时会显示当前使用的披风"))
                num += 33
                num2 += 29
            End If
            If LastVersion < 375 Then
                num += 16
                num2 += 4
            End If
            If LastVersion < 374 Then
                num += 9
                num2 += 7
            End If
            If LastVersion < 373 Then
                num += 7
                num2 += 10
            End If
            Dim list2 As List(Of String) = New List(Of String)()
            Dim enumerable As IEnumerable(Of KeyValuePair(Of Integer, String)) = list
            Dim func As Func(Of KeyValuePair(Of Integer, String), Integer)
            If FormMain._Closure$__.$I1-0 IsNot Nothing Then
                func = FormMain._Closure$__.$I1-0
            Else
                Dim func2 As Func(Of KeyValuePair(Of Integer, String), Integer) = Function(f As KeyValuePair(Of Integer, String)) f.Key
                func = func2
                FormMain._Closure$__.$I1-0 = func2
            End If
            Dim list3 As List(Of KeyValuePair(Of Integer, String)) = enumerable.OrderByDescending(func).ToList()
            If Not list3.Any() AndAlso num = 0 AndAlso num2 = 0 Then
                list2.Add("没有更新日志……")
            End If
            Dim num3 As Integer = Math.Min(9, list3.Count - 1)
            For i As Integer = 0 To num3
                list2.Add(list3(i).Value)
            Next
            If list3.Count > 10 Then
                num += list3.Count - 10
            End If
            If num > 0 OrElse num2 > 0 Then
                list2.Add(If((num > 0), (Conversions.ToString(num) + " 项小调整与修改"), "") + If((num > 0 AndAlso num2 > 0), "，", "") + If((num2 > 0), ("修复了 " + Conversions.ToString(num2) + " 个 Bug"), "") + "，详见完整更新日志")
            End If
            CS$<>8__locals1.$VB$Local_Content = "· " + list2.Join(vbCrLf & "· ")
            ModBase.RunInNewThread(Sub()
                If ModMain.MyMsgBox(CS$<>8__locals1.$VB$Local_Content, "PCL 已更新至快照版 2.99.0.0", "确定", "完整更新日志", "", False, True, False, Nothing, Nothing, Nothing) = 2 Then
                    ModBase.OpenWebsite("https://meloong.com/afd/a/LTCat?tab=feed")
                End If
            End Sub, "UpdateLog Output", ThreadPriority.Normal)
        End Sub

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.FormMain_Loaded
            AddHandler MyBase.Closing, AddressOf Me.FormMain_Closing
            AddHandler MyBase.SizeChanged, Sub(a0 As Object, a1 As SizeChangedEventArgs)
                Me.FormMain_SizeChanged()
            End Sub
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.FormMain_SizeChanged()
            End Sub
            AddHandler MyBase.KeyDown, AddressOf Me.FormMain_KeyDown
            AddHandler MyBase.MouseDown, AddressOf Me.FormMain_MouseDown
            AddHandler MyBase.Activated, Sub(a0 As Object, a1 As EventArgs)
                Me.FormMain_Activated()
            End Sub
            AddHandler MyBase.Drop, AddressOf Me.FrmMain_Drop
            AddHandler MyBase.MouseMove, AddressOf Me.FormMain_MouseMove
            Me.IsWindowLoadFinished = False
            Me.IsSizeSaveable = False
            Me.IsSystemTimeChanged = False
            Me._Hidden = False
            Me.PageCurrent = FormMain.PageType.Launch
            Me.PageLast = FormMain.PageType.Launch
            Me.PageStack = New List(Of FormMain.PageStackData)()
            Me.IsChangingPage = False
            Me.lastMouseArg = Nothing
            ModBase.ApplicationStartTick = ModBase.GetTimeMs()
            ModMain.FrmMain = Me
            ModMain.FrmLaunchLeft = New PageLaunchLeft()
            ModMain.FrmLaunchRight = New PageLaunchRight()
            Dim num As Integer = Settings.[Get](Of Integer)("SystemLastVersionReg", Nothing)
            If num < 414 Then
                Me.UpgradeSub(num)
            ElseIf num > 414 Then
                Me.DowngradeSub(num)
            End If
            If Not Settings.HasSaved("LaunchArgumentIndieV2", Nothing) Then
                If Settings.HasSaved("LaunchArgumentIndie", Nothing) Then
                    Logger.Info("从老 PCL 迁移版本隔离", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                    Settings.[Set]("LaunchArgumentIndieV2", Settings.[Get](Of Integer)("LaunchArgumentIndie", Nothing), Nothing)
                ElseIf ModBase.HasIniKey("Setup", "LaunchVersionSelect") Then
                    Logger.Info("从老 PCL 升级，但此前未调整版本隔离，使用老的版本隔离默认值", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                    Settings.[Set]("LaunchArgumentIndieV2", Settings.GetDefault("LaunchArgumentIndie"), Nothing)
                Else
                    Logger.Info("全新的 PCL，使用新的版本隔离默认值", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                    Settings.[Set]("LaunchArgumentIndieV2", Settings.GetDefault("LaunchArgumentIndieV2"), Nothing)
                End If
            End If
            ModSecret.ThemeCheckAll(False)
            ModSecret.ThemeRefresh(Settings.[Get](Of Integer)("UiLauncherTheme", Nothing))
            MyBase.[AddHandler](DragDrop.DragEnterEvent, AddressOf Me.HandleDrag, True)
            MyBase.[AddHandler](DragDrop.DragOverEvent, AddressOf Me.HandleDrag, True)
            Me.InitializeComponent()
            MyBase.Opacity = 0.0
            If WindowsUtils.HasAdminRole() Then
                If Me.$STATIC$.ctor$2001$Helper$Init Is Nothing Then
                    Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$.ctor$2001$Helper$Init, New StaticLocalInitFlag(), Nothing)
                End If
                Dim flag As Boolean = False
                Try
                    Monitor.Enter(Me.$STATIC$.ctor$2001$Helper$Init, flag)
                    If Me.$STATIC$.ctor$2001$Helper$Init.State = 0S Then
                        Me.$STATIC$.ctor$2001$Helper$Init.State = 2S
                        Me.$STATIC$.ctor$2001$Helper = New DragHelper()
                    ElseIf Me.$STATIC$.ctor$2001$Helper$Init.State = 2S Then
                        Throw New IncompleteInitialization()
                    End If
                Finally
                    Me.$STATIC$.ctor$2001$Helper$Init.State = 1S
                    If flag Then
                        Monitor.[Exit](Me.$STATIC$.ctor$2001$Helper$Init)
                    End If
                End Try
                AddHandler MyBase.SourceInitialized, Sub(a0 As Object, a1 As EventArgs)
                    Me._Lambda$__3-0()
                End Sub
                AddHandler MyBase.Closing, Sub(a0 As Object, a1 As CancelEventArgs)
                    Me._Lambda$__3-1()
                End Sub
                AddHandler Me.$STATIC$.ctor$2001$Helper.DragDrop, Sub(a0 As Object, a1 As EventArgs)
                    Me._Lambda$__3-2()
                End Sub
            End If
            If Not Information.IsNothing(ModMain.FrmLaunchLeft.Parent) Then
                ModMain.FrmLaunchLeft.SetValue(ContentPresenter.ContentProperty, Nothing)
            End If
            If Not Information.IsNothing(ModMain.FrmLaunchRight.Parent) Then
                ModMain.FrmLaunchRight.SetValue(ContentPresenter.ContentProperty, Nothing)
            End If
            Me.PanMainLeft.Child = ModMain.FrmLaunchLeft
            Me.PageLeft = ModMain.FrmLaunchLeft
            Me.PanMainRight.Child = ModMain.FrmLaunchRight
            Me.PageRight = ModMain.FrmLaunchRight
            ModMain.FrmLaunchRight.PageState = MyPageRight.PageStates.ContentStay
            If ModBase.ModeDebug Then
                ModMain.Hint("[调试模式] PCL 正以调试模式运行，这可能会导致性能下降，若无必要请不要开启！", ModMain.HintType.Blue, True)
            End If
            ModMinecraft.McFolderListLoader.Start(0, False)
            Logger.Info(String.Format("第二阶段加载用时：{0} ms", ModBase.GetTimeMs() - ModBase.ApplicationStartTick), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
        End Sub

        Private Sub FormMain_Loaded(sender As Object, e As RoutedEventArgs)
            ModBase.ApplicationStartTick = ModBase.GetTimeMs()
            ModBase.Handle = New WindowInteropHelper(Me).Handle
            Me.UpdateBackgroundAndTitleBar()
            PageSetupUI.HiddenRefresh()
            PageSetupUI.BackgroundRefresh(False, True)
            ModMusic.MusicRefreshPlay(False, True)
            Me.BtnExtraDownload.ShowCheck = AddressOf Me.BtnExtraDownload_ShowCheck
            Me.BtnExtraBack.ShowCheck = AddressOf Me.BtnExtraBack_ShowCheck
            Me.BtnExtraApril.ShowCheck = AddressOf Me.BtnExtraApril_ShowCheck
            Me.BtnExtraShutdown.ShowCheck = AddressOf Me.BtnExtraShutdown_ShowCheck
            Me.BtnExtraApril.ShowRefresh()
            Dim myResizer As MyResizer = New MyResizer(Me)
            myResizer.addResizerDown(Me.ResizerB)
            myResizer.addResizerLeft(Me.ResizerL)
            myResizer.addResizerLeftDown(Me.ResizerLB)
            myResizer.addResizerLeftUp(Me.ResizerLT)
            myResizer.addResizerRight(Me.ResizerR)
            myResizer.addResizerRightDown(Me.ResizerRB)
            myResizer.addResizerRightUp(Me.ResizerRT)
            myResizer.addResizerUp(Me.ResizerT)
            If ModBase.RandomInteger(1, 1000) = 233 Then
                Me.ShapeTitleLogo.Data = CType(New GeometryConverter().ConvertFromString("M26,29 v-25 h6 a7,7 180 0 1 0,14 h-6 M83,6.5 a10,11.5 180 1 0 0,18 M48,2.5 v24.5 h13.5"), Geometry)
            End If
            ModSecret.ThemeRefreshMain()
            Try
                MyBase.Height = CDbl(Settings.[Get](Of Integer)("WindowHeight", Nothing))
                MyBase.Width = CDbl(Settings.[Get](Of Integer)("WindowWidth", Nothing))
            Catch ex As Exception
                Logger.[Error](ex, "读取窗口默认大小失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                MyBase.Height = MyBase.MinHeight + 100.0
                MyBase.Width = MyBase.MinWidth + 100.0
            End Try
            MyBase.Topmost = False
            If ModMain.FrmStart IsNot Nothing Then
                ' The following expression was wrapped in a checked-expression
                ModMain.FrmStart.Close(New TimeSpan(0, 0, 0, 0, CInt(Math.Round(400.0 / ModAnimation.AniSpeed))))
            End If
            MyBase.Top = (ModBase.GetWPFSize(CDbl(MyWpfExtension.Computer.Screen.WorkingArea.Height)) - MyBase.Height) / 2.0
            MyBase.Left = (ModBase.GetWPFSize(CDbl(MyWpfExtension.Computer.Screen.WorkingArea.Width)) - MyBase.Width) / 2.0
            Me.IsSizeSaveable = True
            Me.ShowWindowToTop()
            CType(PresentationSource.FromVisual(Me), HwndSource).AddHook(AddressOf Me.WndProc)
            Dim array As ModAnimation.AniData() = New ModAnimation.AniData(4) {}
            Dim num As Integer = 0
            Dim threadStart As ThreadStart
            If FormMain._Closure$__.$I4-0 IsNot Nothing Then
                threadStart = FormMain._Closure$__.$I4-0
            Else
                ' The following expression was wrapped in a checked-expression
                Dim threadStart2 As ThreadStart = Sub()
                    ModAnimation.AniControlEnabled -= 1
                End Sub
                threadStart = threadStart2
                FormMain._Closure$__.$I4-0 = threadStart2
            End If
            array(num) = ModAnimation.AaCode(threadStart, 50, False)
            array(1) = ModAnimation.AaOpacity(Me, CDbl(Settings.[Get](Of Integer)("UiLauncherTransparent", Nothing)) / 1000.0 + 0.4, 250, 100, Nothing, False)
            array(2) = ModAnimation.AaDouble(Sub(i As Object)
                Dim transformPos As TranslateTransform = Me.TransformPos
                Dim translateTransform As TranslateTransform = transformPos
                transformPos.Y = Conversions.ToDouble(Operators.AddObject(translateTransform.Y, i))
            End Sub, -Me.TransformPos.Y, 600, 100, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), False)
            array(3) = ModAnimation.AaDouble(Sub(i As Object)
                Dim transformRotate As RotateTransform = Me.TransformRotate
                Dim rotateTransform As RotateTransform = transformRotate
                transformRotate.Angle = Conversions.ToDouble(Operators.AddObject(rotateTransform.Angle, i))
            End Sub, -Me.TransformRotate.Angle, 500, 100, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), False)
            array(4) = ModAnimation.AaCode(Sub()
                Me.PanBack.RenderTransform = Nothing
                Me.IsWindowLoadFinished = True
                Logger.Info(String.Format("DPI：{0}，系统版本：{1}，PCL 位置：{2}", ModBase.DPI, Environment.OSVersion.VersionString, ModBase.PathExe), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            End Sub, 0, True)
            ModAnimation.AniStart(array, "Form Show", False)
            ModAnimation.AniStart()
            ModMain.TimerMainStart()
            ModBase.RunInNewThread(Sub()
                If Settings.[Get](Of Integer)("SystemEulaVersion", Nothing) < 2 Then
                    Dim text As String = If((Settings.[Get](Of Integer)("SystemEulaVersion", Nothing) = 0), "在使用 PCL 前，请先阅读用户协议与免责声明。", String.Format("PCL 的用户协议与免责声明已更新。{0}请阅读更新后的用户协议与免责声明。", vbCrLf))
                    Dim text2 As String = "协议授权"
                    Dim text3 As String = "同意"
                    Dim text4 As String = "拒绝"
                    Dim text5 As String = "查看用户协议与免责声明"
                    Dim flag As Boolean = False
                    Dim flag2 As Boolean = True
                    Dim flag3 As Boolean = False
                    Dim action As Action = Nothing
                    Dim action2 As Action = Nothing
                    Dim action3 As Action
                    If FormMain._Closure$__.$I4-5 IsNot Nothing Then
                        action3 = FormMain._Closure$__.$I4-5
                    Else
                        Dim action4 As Action = Sub()
                            ModBase.OpenWebsite("https://shimo.im/docs/rGrd8pY8xWkt6ryW")
                        End Sub
                        action3 = action4
                        FormMain._Closure$__.$I4-5 = action4
                    End If
                    Dim num2 As Integer = ModMain.MyMsgBox(text, text2, text3, text4, text5, flag, flag2, flag3, action, action2, action3)
                    If num2 <> 1 Then
                        If num2 = 2 Then
                            Me.EndProgram(False)
                        End If
                    Else
                        Settings.[Set]("SystemEulaVersion", 2, Nothing)
                    End If
                End If
                Try
                    ModJava.JavaInit()
                    Thread.Sleep(100)
                    ModDownload.DlClientListMojangLoader.Start(1, False)
                    Me.RunCountSub()
                    ModSecret.ServerLoader.Start(Nothing, False)
                    ModBase.RunInNewThread(AddressOf ModMain.TryClearTaskTemp, "TryClearTaskTemp", ThreadPriority.BelowNormal)
                Catch ex2 As Exception
                    Logger.[Error](ex2, "初始化加载池运行失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                End Try
                Try
                    FileUtils.Delete(Paths.Base + "PCL\Plain Craft Launcher 2.exe", False)
                Catch ex3 As Exception
                    Logger.Warn(ex3, "清理自动更新文件失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                End Try
                ModMain.Telemetry("启动", New String(-1) {})
            End Sub, "初始化", ThreadPriority.Lowest)
            Logger.Info(String.Format("第三阶段加载用时：{0} ms", ModBase.GetTimeMs() - ModBase.ApplicationStartTick), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
        End Sub

        Private Sub RunCountSub()
            ' The following expression was wrapped in a checked-expression
            Settings.[Set]("SystemCount", Settings.[Get](Of Integer)("SystemCount", Nothing) + 1, Nothing)
            Dim num As Integer = Settings.[Get](Of Integer)("SystemCount", Nothing)
            If num = 1 Then
                ModMain.MyMsgBox("欢迎使用 PCL 快照版！" & vbCrLf & "快照版包含尚未在正式版发布的测试性功能，仅用于赞助者本人尝鲜。所以请不要发给其他人或者用于制作整合包哦！" & vbCrLf & "如果你并非通过赞助或赞助者本人邀请进群获得的本程序，那么可能是有人在违规传播，记得提醒他一下啦。", "快照版使用说明", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
            End If
            If Settings.[Get](Of Integer)("SystemCount", Nothing) >= 99 AndAlso ModSecret.ThemeUnlock(6, False, Nothing) Then
                ModMain.MyMsgBox("你已经使用了 99 次 PCL 啦，感谢你长期以来的支持！" & vbCrLf & "隐藏主题 铁杆粉 已解锁！", "提示", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
            End If
        End Sub

        Private Sub UpgradeSub(LastVersionCode As Integer)
            Logger.Info(String.Format("版本号从 {0} 升高到 {1}", LastVersionCode, 414), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            Settings.[Set]("SystemLastVersionReg", 414, Nothing)
            Dim num As Integer = Settings.[Get](Of Integer)("SystemHighestAlphaVersionReg", Nothing)
            If num < 414 Then
                Settings.[Set]("SystemHighestAlphaVersionReg", 414, Nothing)
                Logger.Info(String.Format("最高版本号从 {0} 升高到 {1}", num, 414), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            End If
            If Settings.[Get](Of Integer)("LaunchArgumentWindowType", Nothing) = 5 Then
                Settings.[Set]("LaunchArgumentWindowType", 1, Nothing)
            End If
            If num <= 207 Then
                Dim list As List(Of String) = New List(Of String)() From { "2" }
                list.AddRange(New List(Of String)(Settings.[Get](Of String)("UiLauncherThemeHide", Nothing).ToString().Split("|", False)))
                list.AddRange(New List(Of String)(Settings.[Get](Of String)("UiLauncherThemeHide2", Nothing).ToString().Split("|", False)))
                Settings.[Set]("UiLauncherThemeHide2", list.Distinct().ToList().Join("|"c), Nothing)
            End If
            If LastVersionCode <= 115 AndAlso Settings.[Get](Of String)("UiLauncherThemeHide2", Nothing).ToString().Split("|", False).Contains("13") Then
                Dim list2 As List(Of String) = New List(Of String)(Settings.[Get](Of String)("UiLauncherThemeHide2", Nothing).ToString().Split("|", False))
                list2.Remove("13")
                Settings.[Set]("UiLauncherThemeHide2", list2.Join("|"c), Nothing)
                ModMain.MyMsgBox("由于新版 PCL 修改了欧皇彩的解锁方式，你需要重新解锁欧皇彩。" & vbCrLf & "多谢各位的理解啦！", "重新解锁提醒", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
            End If
            If LastVersionCode <= 152 AndAlso Settings.[Get](Of String)("UiLauncherThemeHide2", Nothing).ToString().Split("|", False).Contains("12") Then
                Dim list3 As List(Of String) = New List(Of String)(Settings.[Get](Of String)("UiLauncherThemeHide2", Nothing).ToString().Split("|", False))
                list3.Remove("12")
                Settings.[Set]("UiLauncherThemeHide2", list3.Join("|"c), Nothing)
                ModMain.MyMsgBox("由于新版 PCL 修改了滑稽彩的解锁方式，你需要重新解锁滑稽彩。" & vbCrLf & "多谢各位的理解啦！", "重新解锁提醒", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
            End If
            If LastVersionCode <= 161 AndAlso FileUtils.Exists(Paths.Base + "PCL\CustomSkin.png") AndAlso Not FileUtils.Exists(Paths.AppDataThenName + "CustomSkin.png") Then
                FileUtils.Copy(Paths.Base + "PCL\CustomSkin.png", Paths.AppDataThenName + "CustomSkin.png")
                Logger.Info("已移动离线自定义皮肤 (162)", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            End If
            If LastVersionCode <= 263 AndAlso FileUtils.Exists(ModBase.PathTemp + "CustomSkin.png") AndAlso Not FileUtils.Exists(Paths.AppDataThenName + "CustomSkin.png") Then
                FileUtils.Copy(ModBase.PathTemp + "CustomSkin.png", Paths.AppDataThenName + "CustomSkin.png")
                Logger.Info("已移动离线自定义皮肤 (264)", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            End If
            If LastVersionCode <= 205 Then
                Settings.[Set]("UiHiddenOtherHelp", False, Nothing)
                Logger.Info("已解除帮助页面的隐藏", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            End If
            If Not Settings.[Get](Of Boolean)("CacheMsV2Migrated", Nothing) Then
                Settings.[Set]("CacheMsV2Migrated", True, Nothing)
                Settings.[Set]("CacheMsV2OAuthRefresh", Settings.[Get](Of String)("CacheMsOAuthRefresh", Nothing), Nothing)
                Settings.[Set]("CacheMsV2Access", Settings.[Get](Of String)("CacheMsAccess", Nothing), Nothing)
                Settings.[Set]("CacheMsV2ProfileJson", Settings.[Get](Of String)("CacheMsProfileJson", Nothing), Nothing)
                Settings.[Set]("CacheMsV2Uuid", Settings.[Get](Of String)("CacheMsUuid", Nothing), Nothing)
                Settings.[Set]("CacheMsV2Name", Settings.[Get](Of String)("CacheMsName", Nothing), Nothing)
                Logger.Info("已从老版本迁移微软登录结果", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            End If
            If Settings.HasSaved("ToolDownloadTranslate", Nothing) AndAlso Not Settings.HasSaved("ToolDownloadTranslateV2", Nothing) Then
                ' The following expression was wrapped in a checked-expression
                Settings.[Set]("ToolDownloadTranslateV2", Settings.[Get](Of Integer)("ToolDownloadTranslate", Nothing) + 1, Nothing)
                Logger.Info("已从老版本迁移 Mod 命名设置", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            End If
            If LastVersionCode <= 381 AndAlso Settings.HasSaved("LaunchAdvanceJvm", Nothing) AndAlso Operators.CompareString(Settings.[Get](Of String)("LaunchAdvanceJvm", Nothing).ToString().Replace("-XX:+UseG1GC ", "").Replace("-XX:-UseAdaptiveSizePolicy ", "").Trim(), Settings.GetDefault("LaunchAdvanceJvm"), False) = 0 Then
                Settings.Reset("LaunchAdvanceJvm", Nothing)
                Logger.Info("已重置 JVM 参数设置", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            End If
            If LastVersionCode > 0 AndAlso num < 414 Then
                Me.ShowUpdateLog(num)
            End If
        End Sub

        Private Sub DowngradeSub(LastVersionCode As Integer)
            Logger.Info(String.Format("版本号从 {0} 降低到 {1}", LastVersionCode, 414), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            Settings.[Set]("SystemLastVersionReg", 414, Nothing)
        End Sub

        Private Sub FormMain_Closing(sender As Object, e As CancelEventArgs)
            Me.EndProgram(True)
            e.Cancel = True
        End Sub

        Public Sub EndProgram(SendWarning As Boolean)
            If ModNet.HasDownloadingTask(False) Then
                If SendWarning AndAlso ModMain.MyMsgBox("还有下载任务尚未完成，是否确定退出？", "提示", "确定", "取消", "", False, True, False, Nothing, Nothing, Nothing) = 2 Then
                    Return
                End If
                Dim action As Action
                If FormMain._Closure$__.$I9-0 IsNot Nothing Then
                    action = FormMain._Closure$__.$I9-0
                Else
                    Dim action2 As Action = Sub()
                        Logger.Info("正在强行停止任务", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                        Try
                            For Each loaderBase As ModLoader.LoaderBase In ModLoader.LoaderTaskbar.ToList()
                                loaderBase.Cancel()
                            Next
                        Finally
                            Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                            CType(enumerator, IDisposable).Dispose()
                        End Try
                    End Sub
                    action = action2
                    FormMain._Closure$__.$I9-0 = action2
                End If
                ModBase.RunInNewThread(action, "强行停止下载任务", ThreadPriority.Normal)
            End If
            Dim frmLinkMain As PageLinkMain = ModMain.FrmLinkMain
            If frmLinkMain Is Nothing OrElse Not frmLinkMain.TryExit(Not SendWarning, True) Then
                ModBase.RunInUiWait(Sub()
                    MyBase.IsHitTestVisible = False
                    If Me.PanBack.RenderTransform Is Nothing Then
                        Dim CS$<>8__locals1 As FormMain._Closure$__9-0 = New FormMain._Closure$__9-0(CS$<>8__locals1)
                        CS$<>8__locals1.$VB$Local_TransformPos = New TranslateTransform(0.0, 0.0)
                        CS$<>8__locals1.$VB$Local_TransformRotate = New RotateTransform(0.0)
                        CS$<>8__locals1.$VB$Local_TransformScale = New ScaleTransform(1.0, 1.0)
                        Me.PanBack.RenderTransform = New TransformGroup() With { .Children = New TransformCollection(New Transform() { CS$<>8__locals1.$VB$Local_TransformRotate, CS$<>8__locals1.$VB$Local_TransformPos, CS$<>8__locals1.$VB$Local_TransformScale }) }
                        Dim array As ModAnimation.AniData() = New ModAnimation.AniData(5) {}
                        array(0) = ModAnimation.AaOpacity(Me, -MyBase.Opacity, 140, 40, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False)
                        array(1) = ModAnimation.AaDouble(Sub(i As Object)
                            Dim $VB$Local_TransformScale As ScaleTransform = CS$<>8__locals1.$VB$Local_TransformScale
                            Dim scaleTransform As ScaleTransform = $VB$Local_TransformScale
                            $VB$Local_TransformScale.ScaleX = Conversions.ToDouble(Operators.AddObject(scaleTransform.ScaleX, i))
                            Dim $VB$Local_TransformScale2 As ScaleTransform = CS$<>8__locals1.$VB$Local_TransformScale
                            scaleTransform = $VB$Local_TransformScale2
                            $VB$Local_TransformScale2.ScaleY = Conversions.ToDouble(Operators.AddObject(scaleTransform.ScaleY, i))
                        End Sub, 0.88 - CS$<>8__locals1.$VB$Local_TransformScale.ScaleX, 180, 0, Nothing, False)
                        array(2) = ModAnimation.AaDouble(Sub(i As Object)
                            Dim $VB$Local_TransformPos As TranslateTransform = CS$<>8__locals1.$VB$Local_TransformPos
                            Dim translateTransform As TranslateTransform = $VB$Local_TransformPos
                            $VB$Local_TransformPos.Y = Conversions.ToDouble(Operators.AddObject(translateTransform.Y, i))
                        End Sub, 20.0 - CS$<>8__locals1.$VB$Local_TransformPos.Y, 180, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False)
                        array(3) = ModAnimation.AaDouble(Sub(i As Object)
                            Dim $VB$Local_TransformRotate As RotateTransform = CS$<>8__locals1.$VB$Local_TransformRotate
                            Dim rotateTransform As RotateTransform = $VB$Local_TransformRotate
                            $VB$Local_TransformRotate.Angle = Conversions.ToDouble(Operators.AddObject(rotateTransform.Angle, i))
                        End Sub, 0.6 - CS$<>8__locals1.$VB$Local_TransformRotate.Angle, 180, 0, New ModAnimation.AniEaseInoutFluent(ModAnimation.AniEasePower.Weak, 0.5), False)
                        array(4) = ModAnimation.AaCode(Sub()
                            MyBase.IsHitTestVisible = False
                            MyBase.Top = -10000.0
                            MyBase.ShowInTaskbar = False
                        End Sub, 210, False)
                        Dim num As Integer = 5
                        Dim threadStart As ThreadStart
                        If FormMain._Closure$__.$IR9-7 IsNot Nothing Then
                            threadStart = FormMain._Closure$__.$IR9-7
                        Else
                            Dim threadStart2 As ThreadStart = Sub()
                                FormMain.EndProgramForce(ModBase.ProcessReturnValues.Success)
                            End Sub
                            threadStart = threadStart2
                            FormMain._Closure$__.$IR9-7 = threadStart2
                        End If
                        array(num) = ModAnimation.AaCode(threadStart, 230, False)
                        ModAnimation.AniStart(array, "Form Close", False)
                    Else
                        FormMain.EndProgramForce(ModBase.ProcessReturnValues.Success)
                    End If
                    Logger.Info("收到关闭指令", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                End Sub)
            End If
        End Sub

        Public Shared Sub EndProgramForce(Optional ReturnCode As ModBase.ProcessReturnValues = ModBase.ProcessReturnValues.Success)
            Dim num As Integer
            Dim num4 As Integer
            Dim obj As Object
            Try
                IL_0000:
                ProjectData.ClearProjectError()
                num = 1
                IL_0007:
                Dim num2 As Integer = 2
                ModBase.IsProgramEnding = True
                IL_000F:
                num2 = 3
                ModAnimation.AniControlEnabled += 1
                IL_001D:
                num2 = 4
                If Not ModSecret.IsUpdateWaitingRestart Then
                    GoTo IL_002E
                End If
                IL_0026:
                num2 = 5
                ModSecret.UpdateRestart(False)
                IL_002E:
                num2 = 6
                If ReturnCode <> ModBase.ProcessReturnValues.Exception Then
                    GoTo IL_008A
                End If
                IL_0034:
                num2 = 7
                If FormMain.IsLogShown Then
                    GoTo IL_007D
                End If
                IL_003D:
                num2 = 8
                ModMain.FeedbackInfo()
                IL_0044:
                num2 = 9
                Logger.Info("请在 https://github.com/Meloong-Git/PCL/issues 提交错误报告，以便于作者解决此问题！", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                IL_0057:
                num2 = 10
                FormMain.IsLogShown = True
                IL_0060:
                num2 = 11
                ModBase.StartProcess(Paths.Base + "PCL\Log1.txt", "")
                IL_007D:
                num2 = 12
                Thread.Sleep(500)
                IL_008A:
                num2 = 13
                Logger.Info(String.Format("程序已退出，返回值：{0}", ReturnCode), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                IL_00A8:
                num2 = 14
                ConfigUtils.SaveAll()
                IL_00B0:
                num2 = 15
                Logger.Instance.Flush()
                IL_00BD:
                num2 = 16
                If ReturnCode = ModBase.ProcessReturnValues.Success Then
                    GoTo IL_00CC
                End If
                IL_00C3:
                num2 = 17
                Environment.[Exit](CInt(ReturnCode))
                IL_00CC:
                num2 = 18
                Process.GetCurrentProcess().Kill()
                IL_00D9:
                GoTo IL_0177
                IL_00DE:
                Dim num3 As Integer = num4 + 1
                num4 = 0
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3)
                IL_0138:
                GoTo IL_016C
                IL_013A:
                num4 = num2
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num)
                IL_014A:
            Catch obj2 When endfilter((TypeOf obj Is Exception) And (num <> 0) And (num4 = 0))
                Dim ex As Exception = CType(obj2, Exception)
                GoTo IL_013A
            End Try
            IL_016C:
            Throw ProjectData.CreateProjectError(-2146828237)
            IL_0177:
            If num4 <> 0 Then
                ProjectData.ClearProjectError()
            End If
        End Sub

        Private Sub BtnTitleClose_Click(sender As Object, e As RoutedEventArgs)
            Me.EndProgram(True)
        End Sub

        Private Sub FormDragMove(sender As Object, e As MouseButtonEventArgs)
            Dim num As Integer
            Dim num4 As Integer
            Dim obj As Object
            Try
                IL_0000:
                ProjectData.ClearProjectError()
                num = 1
                IL_0007:
                Dim num2 As Integer = 2
                If Not Conversions.ToBoolean(NewLateBinding.LateGet(sender, Nothing, "IsMouseDirectlyOver", New Object(-1) {}, Nothing, Nothing, Nothing)) Then
                    GoTo IL_002D
                End If
                IL_0025:
                num2 = 3
                MyBase.DragMove()
                IL_002D:
                GoTo IL_008C
                IL_002F:
                Dim num3 As Integer = num4 + 1
                num4 = 0
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3)
                IL_004D:
                GoTo IL_0081
                IL_004F:
                num4 = num2
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num)
                IL_005F:
            Catch obj2 When endfilter((TypeOf obj Is Exception) And (num <> 0) And (num4 = 0))
                Dim ex As Exception = CType(obj2, Exception)
                GoTo IL_004F
            End Try
            IL_0081:
            Throw ProjectData.CreateProjectError(-2146828237)
            IL_008C:
            If num4 <> 0 Then
                ProjectData.ClearProjectError()
            End If
        End Sub

        Private Sub FormMain_SizeChanged()
            If Me.IsSizeSaveable Then
                Settings.[Set]("WindowHeight", MyBase.Height, Nothing)
                Settings.[Set]("WindowWidth", MyBase.Width, Nothing)
            End If
            Me.RectForm.Rect = New Rect(0.0, 0.0, Me.BorderForm.ActualWidth, Me.BorderForm.ActualHeight)
            Me.PanForm.Width = Me.BorderForm.ActualWidth + 0.001
            Me.PanForm.Height = Me.BorderForm.ActualHeight + 0.001
            Me.PanMain.Width = Me.PanForm.Width
            Me.PanMain.Height = Math.Max(0.0, Me.PanForm.Height - Me.PanTitle.ActualHeight)
            If MyBase.WindowState = WindowState.Maximized Then
                MyBase.WindowState = WindowState.Normal
            End If
        End Sub

        Private Sub BtnTitleMin_Click()
            MyBase.WindowState = WindowState.Minimized
        End Sub

        Public Shared Sub UpdateBackgroundAndTitleBar(Value As Object)
            If ModMain.FrmMain IsNot Nothing AndAlso ModMain.FrmMain.IsLoaded Then
                ModMain.FrmMain.UpdateBackgroundAndTitleBar()
            End If
        End Sub

        Public Sub UpdateBackgroundAndTitleBar()
            Logger.Info("从设置更新背景图片与标题栏样式", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            Me.ImgBack.Opacity = CDbl(Settings.[Get](Of Integer)("UiBackgroundOpacity", Nothing)) / 1000.0
            Dim num As Double = CDbl((Settings.[Get](Of Integer)("UiBackgroundBlur", Nothing) + 1))
            If num = 1.0 Then
                Me.ImgBack.Effect = Nothing
            Else
                Me.ImgBack.Effect = New BlurEffect() With { .Radius = num }
            End If
            Me.ImgBack.Margin = New Thickness(-num / 1.8)
            Dim num2 As Integer = Settings.[Get](Of Integer)("UiBackgroundSuit", Nothing)
            If Me.ImgBack.Background IsNot Nothing Then
                Dim imageBrush As ImageBrush = CType(Me.ImgBack.Background, ImageBrush)
                If num2 = 0 Then
                    If imageBrush.ImageSource.Width < Me.PanMain.ActualWidth / 2.0 AndAlso imageBrush.ImageSource.Height < Me.PanMain.ActualHeight / 2.0 Then
                        num2 = 4
                    Else
                        num2 = 2
                    End If
                End If
                imageBrush.Stretch = Stretch.UniformToFill
                imageBrush.TileMode = TileMode.None
                imageBrush.Viewport = New Rect(0.0, 0.0, 1.0, 1.0)
                imageBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox
                imageBrush.AlignmentX = AlignmentX.Center
                imageBrush.AlignmentY = AlignmentY.Center
                Select Case num2
                    Case 1
                        imageBrush.Stretch = Stretch.None
                    Case 3
                        imageBrush.Stretch = Stretch.Fill
                    Case 4
                        imageBrush.Stretch = Stretch.None
                        imageBrush.TileMode = TileMode.Tile
                        imageBrush.Viewport = New Rect(0.0, 0.0, imageBrush.ImageSource.Width, imageBrush.ImageSource.Height)
                        imageBrush.ViewportUnits = BrushMappingMode.Absolute
                    Case 5
                        imageBrush.AlignmentX = AlignmentX.Left
                        imageBrush.AlignmentY = AlignmentY.Top
                    Case 6
                        imageBrush.AlignmentX = AlignmentX.Right
                        imageBrush.AlignmentY = AlignmentY.Top
                    Case 7
                        imageBrush.AlignmentX = AlignmentX.Left
                        imageBrush.AlignmentY = AlignmentY.Bottom
                    Case 8
                        imageBrush.AlignmentX = AlignmentX.Right
                        imageBrush.AlignmentY = AlignmentY.Bottom
                    Case 9
                        imageBrush.AlignmentX = AlignmentX.Left
                        imageBrush.AlignmentY = AlignmentY.Center
                    Case 10
                        imageBrush.AlignmentX = AlignmentX.Right
                        imageBrush.AlignmentY = AlignmentY.Center
                    Case 11
                        imageBrush.AlignmentX = AlignmentX.Center
                        imageBrush.AlignmentY = AlignmentY.Top
                    Case 12
                        imageBrush.AlignmentX = AlignmentX.Center
                        imageBrush.AlignmentY = AlignmentY.Bottom
                End Select
            End If
            Select Case Settings.[Get](Of Integer)("UiLogoType", Nothing)
                Case 0
                    Me.ShapeTitleLogo.Visibility = Visibility.Collapsed
                    Me.LabTitleLogo.Visibility = Visibility.Collapsed
                    Me.ImageTitleLogo.Visibility = Visibility.Collapsed
                    If Not Information.IsNothing(ModMain.FrmSetupUI) Then
                        ModMain.FrmSetupUI.CheckLogoLeft.Visibility = Visibility.Visible
                        ModMain.FrmSetupUI.PanLogoText.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.PanLogoChange.Visibility = Visibility.Collapsed
                    End If
                Case 1
                    Me.ShapeTitleLogo.Visibility = Visibility.Visible
                    Me.LabTitleLogo.Visibility = Visibility.Collapsed
                    Me.ImageTitleLogo.Visibility = Visibility.Collapsed
                    If Not Information.IsNothing(ModMain.FrmSetupUI) Then
                        ModMain.FrmSetupUI.CheckLogoLeft.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.PanLogoText.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.PanLogoChange.Visibility = Visibility.Collapsed
                    End If
                Case 2
                    Me.ShapeTitleLogo.Visibility = Visibility.Collapsed
                    Me.LabTitleLogo.Visibility = Visibility.Visible
                    Me.ImageTitleLogo.Visibility = Visibility.Collapsed
                    If Not Information.IsNothing(ModMain.FrmSetupUI) Then
                        ModMain.FrmSetupUI.CheckLogoLeft.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.PanLogoText.Visibility = Visibility.Visible
                        ModMain.FrmSetupUI.PanLogoChange.Visibility = Visibility.Collapsed
                    End If
                Case 3
                    Me.ShapeTitleLogo.Visibility = Visibility.Collapsed
                    Me.LabTitleLogo.Visibility = Visibility.Collapsed
                    Me.ImageTitleLogo.Visibility = Visibility.Visible
                    If Not Information.IsNothing(ModMain.FrmSetupUI) Then
                        ModMain.FrmSetupUI.CheckLogoLeft.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.PanLogoText.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.PanLogoChange.Visibility = Visibility.Visible
                    End If
                    Try
                        Me.ImageTitleLogo.Source = Paths.Base + "PCL\Logo.png"
                    Catch ex As Exception
                        Me.ImageTitleLogo.Source = Nothing
                        Logger.[Error](ex, "显示标题栏图片失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                    End Try
            End Select
            Dim frmSetupUI As PageSetupUI = ModMain.FrmSetupUI
            If frmSetupUI IsNot Nothing Then
                frmSetupUI.CardLogo.TriggerForceResize()
            End If
            Me.LabTitleLogo.Text = Settings.[Get](Of String)("UiLogoText", Nothing)
            Me.PanTitleMain.ColumnDefinitions(0).Width = New GridLength(CDbl(If((Settings.[Get](Of Boolean)("UiLogoLeft", Nothing) AndAlso Settings.[Get](Of Integer)("UiLogoType", Nothing) = 0), 0, 1)), GridUnitType.Star)
        End Sub

        Private Sub FormMain_KeyDown(sender As Object, e As KeyEventArgs)
            If Not e.IsRepeat Then
                If e.SystemKey = Key.LeftAlt OrElse e.SystemKey = Key.RightAlt Then
                    e.Handled = True
                End If
                If Me.PanMsg.Children.Any() Then
                    If e.Key = Key.[Return] Then
                        NewLateBinding.LateCall(Me.PanMsg.Children(0), Nothing, "Btn1_Click", New Object(-1) {}, Nothing, Nothing, Nothing, True)
                        Return
                    End If
                    If e.Key = Key.Escape Then
                        Dim obj As Object = Me.PanMsg.Children(0)
                        If Not(TypeOf obj Is MyMsgInput) AndAlso Not(TypeOf obj Is MyMsgSelect) AndAlso Operators.ConditionalCompareObjectEqual(NewLateBinding.LateGet(NewLateBinding.LateGet(obj, Nothing, "Btn3", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Visibility", New Object(-1) {}, Nothing, Nothing, Nothing), Visibility.Visible, False) Then
                            NewLateBinding.LateCall(obj, Nothing, "Btn3_Click", New Object(-1) {}, Nothing, Nothing, Nothing, True)
                            Return
                        End If
                        If Operators.ConditionalCompareObjectEqual(NewLateBinding.LateGet(NewLateBinding.LateGet(obj, Nothing, "Btn2", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Visibility", New Object(-1) {}, Nothing, Nothing, Nothing), Visibility.Visible, False) Then
                            NewLateBinding.LateCall(obj, Nothing, "Btn2_Click", New Object(-1) {}, Nothing, Nothing, Nothing, True)
                            Return
                        End If
                        NewLateBinding.LateCall(obj, Nothing, "Btn1_Click", New Object(-1) {}, Nothing, Nothing, Nothing, True)
                        Return
                    End If
                Else
                    If e.Key = Key.Escape Then
                        If Me.PageCurrent Is FormMain.PageType.Launch AndAlso ModLaunch.McLaunchLoader.State = ModBase.LoadState.Loading Then
                            ModMain.FrmLaunchLeft.BtnCancel_Click()
                            Return
                        End If
                        Me.TriggerPageBack()
                    End If
                    If e.Key = Key.F11 AndAlso Me.PageCurrent Is FormMain.PageType.InstanceSelect Then
                        ModMain.FrmSelectRight.ShowHidden = Not ModMain.FrmSelectRight.ShowHidden
                        ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
                        Return
                    End If
                    If e.Key = Key.F12 Then
                        PageSetupUI.HiddenForceShow = Not PageSetupUI.HiddenForceShow
                        If PageSetupUI.HiddenForceShow Then
                            ModMain.Hint("功能隐藏设置已暂时关闭！", ModMain.HintType.Green, True)
                        Else
                            ModMain.Hint("功能隐藏设置已重新开启！", ModMain.HintType.Green, True)
                        End If
                        PageSetupUI.HiddenRefresh()
                        Return
                    End If
                    If e.Key = Key.F5 Then
                        If TypeOf Me.PageLeft Is IRefreshable Then
                            CType(Me.PageLeft, IRefreshable).Refresh()
                        End If
                        If TypeOf Me.PageRight Is IRefreshable Then
                            CType(Me.PageRight, IRefreshable).Refresh()
                            Return
                        End If
                    ElseIf e.Key = Key.[Return] AndAlso Me.PageCurrent Is FormMain.PageType.Launch Then
                        If ModMain.IsAprilEnabled AndAlso Not ModMain.IsAprilGiveup Then
                            ModMain.Hint("木大！", ModMain.HintType.Blue, True)
                            Return
                        End If
                        ModMain.FrmLaunchLeft.LaunchButtonClick()
                    End If
                End If
            End If
        End Sub

        Private Sub FormMain_MouseDown(sender As Object, e As MouseButtonEventArgs)
            If ModMain.FrmMain.PanMsg.Children.Count <= 0 AndAlso Not ModMain.WaitingMyMsgBox.Any() AndAlso (e.ChangedButton = MouseButton.XButton1 OrElse e.ChangedButton = MouseButton.XButton2) Then
                Me.TriggerPageBack()
            End If
        End Sub

        Private Sub TriggerPageBack()
            If Me.PageCurrent Is FormMain.PageType.Download AndAlso Me.PageCurrentSub = FormMain.PageSubType.[Default] AndAlso ModMain.FrmDownloadInstall.IsInSelectPage Then
                ModMain.FrmDownloadInstall.ExitSelectPage()
                Return
            End If
            Me.PageBack()
        End Sub

        Private Sub FormMain_Activated()
            Try
                If Me.PageCurrent Is FormMain.PageType.InstanceSetup AndAlso Me.PageCurrentSub = FormMain.PageSubType.DownloadPack Then
                    ModMain.FrmInstanceMod.ReloadModList(False)
                ElseIf Me.PageCurrent Is FormMain.PageType.InstanceSetup AndAlso (Me.PageCurrentSub = FormMain.PageSubType.SetupLink OrElse Me.PageCurrentSub = FormMain.PageSubType.DownloadResourcePack) Then
                    PageInstanceLeft.ReloadCurrentJava()
                ElseIf Me.PageCurrent Is FormMain.PageType.InstanceSelect Then
                    ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.RunOnUpdated, 1, "versions\", False)
                End If
            Catch ex As Exception
                Logger.[Error](ex, "切回窗口时出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            End Try
        End Sub

        Private Sub HandleDrag(sender As Object, e As DragEventArgs)
            Try
                If Not e.Handled OrElse e.Effects = DragDropEffects.None Then
                    e.Handled = True
                    If e.Data Is Me.$STATIC$HandleDrag$20211C1284DD$PrevData Then
                        e.Effects = Me.$STATIC$HandleDrag$20211C1284DD$PrevEffects
                    Else
                        e.Effects = DragDropEffects.None
                        If e.Data.GetDataPresent(DataFormats.Text) Then
                            Dim text As String = Conversions.ToString(e.Data.GetData(DataFormats.Text))
                            If text.StartsWithF("authlib-injector:yggdrasil-server:", False) Then
                                e.Effects = DragDropEffects.Copy
                            ElseIf text.StartsWithF("file:///", False) Then
                                e.Effects = DragDropEffects.Copy
                            End If
                        ElseIf e.Data.GetDataPresent(DataFormats.FileDrop) Then
                            Dim array As String() = CType(e.Data.GetData(DataFormats.FileDrop), String())
                            If array IsNot Nothing AndAlso array.Length > 0 Then
                                e.Effects = DragDropEffects.Link
                            End If
                        End If
                        Me.$STATIC$HandleDrag$20211C1284DD$PrevData = e.Data
                        Me.$STATIC$HandleDrag$20211C1284DD$PrevEffects = e.Effects
                        Logger.Info(String.Format("设置拖放类型：{0}", e.Effects), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, "处理拖放时出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            End Try
        End Sub

        Private Sub FrmMain_Drop(sender As Object, e As DragEventArgs)
            Try
                Me.ShowWindowToTop()
                If e.Data.GetDataPresent(DataFormats.Text) Then
                    Try
                        Dim text As String = Conversions.ToString(e.Data.GetData(DataFormats.Text))
                        Logger.Info(String.Format("接受文本拖拽：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                        If text.StartsWithF("authlib-injector:yggdrasil-server:", False) Then
                            e.Handled = True
                            e.Effects = DragDropEffects.Copy
                            Dim text2 As String = StringUtils.FormUrlUnescape(text.Substring("authlib-injector:yggdrasil-server:".Length))
                            Logger.Info(String.Format("Authlib 拖拽：{0}", text2), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                            If Not String.IsNullOrEmpty(New ValidateHttp().Validate(text2)) Then
                                ModMain.Hint(String.Format("输入的 Authlib 验证服务器不符合网址格式（{0}）！", text2), ModMain.HintType.Red, True)
                            Else
                                Dim mcInstance As ModMinecraft.McInstance = If((Me.PageCurrent Is FormMain.PageType.InstanceSetup), PageInstanceLeft.Instance, ModMinecraft.McInstanceSelected)
                                If mcInstance Is Nothing Then
                                    ModMain.Hint("请先下载游戏，再设置第三方登录！", ModMain.HintType.Red, True)
                                Else
                                    If Operators.CompareString(text2, "https://littleskin.cn/api/yggdrasil", False) = 0 Then
                                        If ModMain.MyMsgBox(String.Format("是否要在版本 {0} 中开启 LittleSkin 登录？", mcInstance.Name) + vbCrLf & "你可以在 版本设置 → 设置 → 服务器选项 中修改登录方式。", "第三方登录开启确认", "确定", "取消", "", False, True, False, Nothing, Nothing, Nothing) = 2 Then
                                            Return
                                        End If
                                        Settings.[Set]("VersionServerLogin", 4, mcInstance)
                                        Settings.[Set]("VersionServerAuthServer", "https://littleskin.cn/api/yggdrasil", mcInstance)
                                        Settings.[Set]("VersionServerAuthRegister", "https://littleskin.cn/auth/register", mcInstance)
                                        Settings.[Set]("VersionServerAuthName", "LittleSkin 登录", mcInstance)
                                    Else
                                        If ModMain.MyMsgBox(String.Format("是否要在版本 {0} 中开启第三方登录？", mcInstance.Name) + vbCrLf + String.Format("登录服务器：{0}", text2) + vbCrLf & vbCrLf & "你可以在 版本设置 → 设置 → 服务器选项 中修改登录方式。", "第三方登录开启确认", "确定", "取消", "", False, True, False, Nothing, Nothing, Nothing) = 2 Then
                                            Return
                                        End If
                                        Settings.[Set]("VersionServerLogin", 4, mcInstance)
                                        Settings.[Set]("VersionServerAuthServer", text2, mcInstance)
                                        Settings.[Set]("VersionServerAuthRegister", text2.Replace("api/yggdrasil", "auth/register"), mcInstance)
                                        Settings.[Set]("VersionServerAuthName", "", mcInstance)
                                    End If
                                    If Me.PageCurrent Is FormMain.PageType.InstanceSetup AndAlso Me.PageCurrentSub = FormMain.PageSubType.SetupLink Then
                                        ModMain.FrmInstanceSetup.Reload()
                                    ElseIf Me.PageCurrent Is FormMain.PageType.Launch Then
                                        ModMain.FrmLaunchLeft.RefreshPage(True, False)
                                    End If
                                End If
                            End If
                        ElseIf text.StartsWithF("file:///", False) Then
                            Dim text3 As String = WebUtility.UrlDecode(text).Substring("file:///".Length).Replace("/", "\")
                            e.Handled = True
                            e.Effects = DragDropEffects.Copy
                            Me.FileDrag(New List(Of String)() From { text3 })
                        End If
                        GoTo IL_0335
                    Catch ex As Exception
                        Logger.Warn(ex, "无法接取文本拖拽事件", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                        Return
                    End Try
                End If
                If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                    Dim objectValue As Object = RuntimeHelpers.GetObjectValue(e.Data.GetData(DataFormats.FileDrop))
                    If objectValue Is Nothing Then
                        ModMain.Hint("请将文件解压后再拖入！", ModMain.HintType.Red, True)
                    Else
                        e.Handled = True
                        e.Effects = DragDropEffects.Link
                        Me.FileDrag(CType(objectValue, IEnumerable(Of String)))
                    End If
                End If
                IL_0335:
            Catch ex2 As Exception
                Logger.[Error](ex2, "接取拖拽事件失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            End Try
        End Sub

        Private Sub FileDrag(FilePathList As IEnumerable(Of String))
            Dim CS$<>8__locals1 As FormMain._Closure$__25-0 = New FormMain._Closure$__25-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_FilePathList = FilePathList
            ModBase.RunInNewThread(Sub()
                Dim CS$<>8__locals2 As FormMain._Closure$__25-1 = New FormMain._Closure$__25-1(CS$<>8__locals2)
                Dim text As String = CS$<>8__locals1.$VB$Local_FilePathList.First()
                Logger.Warn(String.Format("接受文件拖拽：{0}{1}", text, If(CS$<>8__locals1.$VB$Local_FilePathList.Any(), String.Format(" 等 {0} 个文件", CS$<>8__locals1.$VB$Local_FilePathList.Count()), "")), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                If DirectoryUtils.Exists(CS$<>8__locals1.$VB$Local_FilePathList.First()) AndAlso Not FileUtils.Exists(CS$<>8__locals1.$VB$Local_FilePathList.First()) Then
                    ModMain.Hint("请拖入一个文件，而非文件夹！", ModMain.HintType.Red, True)
                    Return
                End If
                If Not FileUtils.Exists(CS$<>8__locals1.$VB$Local_FilePathList.First()) Then
                    ModMain.Hint("拖入的文件不存在：" + CS$<>8__locals1.$VB$Local_FilePathList.First(), ModMain.HintType.Red, True)
                    Return
                End If
                Dim list As List(Of String) = CS$<>8__locals1.$VB$Local_FilePathList.ToList()
                If CS$<>8__locals1.$VB$Local_FilePathList.Count() > 1 Then
                    Try
                        For Each text2 As String In list
                            If Not New String() { "jar", "litemod", "disabled", "old" }.Contains(PathUtils.GetExtension(text2)) Then
                                ModMain.Hint("一次请只拖入一个文件！", ModMain.HintType.Red, True)
                                Return
                            End If
                        Next
                    Finally
                        Dim enumerator As List(Of String).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                End If
                CS$<>8__locals2.$VB$Local_Extension = PathUtils.GetExtension(text)
                If Operators.CompareString(CS$<>8__locals2.$VB$Local_Extension, "xaml", False) = 0 Then
                    Logger.Info("文件后缀为 XAML，作为主页加载", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                    If Not FileUtils.Exists(Paths.Base + "PCL\Custom.xaml") OrElse ModMain.MyMsgBox("已存在一个主页文件，是否要将它覆盖？", "覆盖确认", "覆盖", "取消", "", False, True, False, Nothing, Nothing, Nothing) <> 2 Then
                        FileUtils.Copy(text, Paths.Base + "PCL\Custom.xaml")
                        Dim action As Action
                        If FormMain._Closure$__.$I25-1 IsNot Nothing Then
                            action = FormMain._Closure$__.$I25-1
                        Else
                            Dim action2 As Action = Sub()
                                Settings.[Set]("UiCustomType", 1, Nothing)
                                ModMain.FrmLaunchRight.ForceRefresh()
                                ModMain.Hint("已加载主页自定义文件！", ModMain.HintType.Green, True)
                            End Sub
                            action = action2
                            FormMain._Closure$__.$I25-1 = action2
                        End If
                        ModBase.RunInUi(action, False)
                        Return
                    End If
                ElseIf Not PageInstanceMod.InstallMods(list) Then
                    If New String() { "zip", "rar", "mrpack" }.Any(Function(t As String) Operators.CompareString(t, CS$<>8__locals2.$VB$Local_Extension, False) = 0) Then
                        ModMain.Hint("正在读取压缩包……", ModMain.HintType.Blue, True)
                        Try
                            Logger.Info("文件为压缩包，尝试作为整合包安装", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                            ModModpack.ModpackInstall(text, Nothing, Nothing)
                            Dim action3 As Action
                            If FormMain._Closure$__.$I25-3 IsNot Nothing Then
                                action3 = FormMain._Closure$__.$I25-3
                            Else
                                Dim action4 As Action = Sub()
                                    ModMain.FrmMain.PageChange(FormMain.PageType.TaskManager, FormMain.PageSubType.[Default])
                                End Sub
                                action3 = action4
                                FormMain._Closure$__.$I25-3 = action4
                            End If
                            ModBase.RunInUi(action3, False)
                            Return
                        Catch ex As Exception
                            If Operators.CompareString(ex.Message, "为遵守相关法律法规，PCL 暂时不再提供 MC 下载功能，非常抱歉 qwq……", False) = 0 Then
                                ModMain.Hint(ex.Message, ModMain.HintType.Red, True)
                                Return
                            End If
                            If ex.IsCanceled() Then
                                Return
                            End If
                        End Try
                    End If
                    If Operators.CompareString(CS$<>8__locals2.$VB$Local_Extension, "rar", False) = 0 Then
                        ModMain.Hint("PCL 无法处理 rar 格式的压缩包，请在解压后重新压缩为 zip 格式再试！", ModMain.HintType.Red, True)
                        Return
                    End If
                    Try
                        Logger.Info("尝试进行错误报告分析", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                        Dim crashAnalyzer As CrashAnalyzer = New CrashAnalyzer(Nothing)
                        crashAnalyzer.Import(text)
                        If crashAnalyzer.Prepare() Then
                            crashAnalyzer.Analyze()
                            crashAnalyzer.Output(True, New List(Of String)())
                            Return
                        End If
                    Catch ex2 As Exception
                        Logger.[Error](ex2, "自主错误报告分析失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                    End Try
                    ModMain.Hint("PCL 无法确定适合的操作……", ModMain.HintType.Red, True)
                End If
            End Sub, "文件拖拽", ThreadPriority.Normal)
        End Sub

        Private Function WndProc(hwnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr, ByRef handled As Boolean) As IntPtr
            If msg = 30 Then
                Dim now As DateTime = DateTime.Now
                If DateTime.Compare(now.[Date], ModBase.ApplicationOpenTime.[Date]) = 0 Then
                    Logger.Info(String.Format("系统时间微调为：{0} {1}", now.ToLongDateString(), now.ToLongTimeString()), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                    Me.IsSystemTimeChanged = False
                Else
                    Logger.Info(String.Format("系统时间修改为：{0} {1}", now.ToLongDateString(), now.ToLongTimeString()), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                    Me.IsSystemTimeChanged = True
                End If
            ElseIf msg = 6402 Then
                Logger.Info(String.Format("收到置顶信息：{0}", hwnd.ToInt64()), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                If Not Me.IsWindowLoadFinished Then
                    Logger.Info("窗口尚未加载完成，忽略置顶请求", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                    Return IntPtr.Zero
                End If
                Me.ShowWindowToTop()
                handled = True
            End If
            Return IntPtr.Zero
        End Function

        Public Property Hidden As Boolean
            Get
                Return Me._Hidden
            End Get
            Set(value As Boolean)
                If Me._Hidden <> value Then
                    Me._Hidden = value
                    If value Then
                        MyBase.Left -= 10000.0
                        MyBase.ShowInTaskbar = False
                        MyBase.Visibility = Visibility.Hidden
                        Logger.Info(String.Format("窗口已隐藏，位置：({0},{1})", MyBase.Left, MyBase.Top), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                        Return
                    End If
                    If MyBase.Left < -2000.0 Then
                        MyBase.Left += 10000.0
                    End If
                    Me.ShowWindowToTop()
                End If
            End Set
        End Property

        Public Sub ShowWindowToTop()
            ModBase.RunInUi(Sub()
                MyBase.Visibility = Visibility.Visible
                MyBase.ShowInTaskbar = True
                MyBase.WindowState = WindowState.Normal
                Me.Hidden = False
                MyBase.Topmost = True
                MyBase.Topmost = False
                ModMain.SetForegroundWindow(ModBase.Handle)
                MyBase.Focus()
                Logger.Info(String.Format("窗口已置顶，位置：({0}, {1}), {2} x {3}", New Object() { MyBase.Left, MyBase.Top, MyBase.Width, MyBase.Height }), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            End Sub, False)
        End Sub

        Private Function PageNameGet(Stack As FormMain.PageStackData) As String
            Dim text As String
            Select Case Stack.Page
                Case FormMain.PageType.InstanceSelect
                    text = "版本选择"
                Case FormMain.PageType.TaskManager
                    text = "任务管理"
                Case FormMain.PageType.InstanceSetup
                    text = "版本设置 - " + If((PageInstanceLeft.Instance Is Nothing), "未知版本", PageInstanceLeft.Instance.Name)
                Case FormMain.PageType.ResourceDetail
                    text = CType(NewLateBinding.LateIndexGet(Stack.Additional, New Object() { 0 }, Nothing), ResourceProject).TranslatedName
                Case FormMain.PageType.HelpDetail
                    text = CType(NewLateBinding.LateIndexGet(Stack.Additional, New Object() { 0 }, Nothing), ModMain.HelpEntry).Title
                Case Else
                    text = ""
            End Select
            Return text
        End Function

        Public Sub PageNameRefresh(Type As FormMain.PageStackData)
            Me.LabTitleInner.Text = Me.PageNameGet(Type)
        End Sub

        Public Sub PageNameRefresh()
            Me.PageNameRefresh(Me.PageCurrent)
        End Sub

        Public ReadOnly Property PageCurrentSub As FormMain.PageSubType
            Get
                Dim pageCurrent As FormMain.PageStackData = Me.PageCurrent
                Dim pageSubType As FormMain.PageSubType
                If pageCurrent Is FormMain.PageType.Download Then
                    If ModMain.FrmDownloadLeft Is Nothing Then
                        ModMain.FrmDownloadLeft = New PageDownloadLeft()
                    End If
                    pageSubType = ModMain.FrmDownloadLeft.PageID
                ElseIf pageCurrent Is FormMain.PageType.Setup Then
                    If ModMain.FrmSetupLeft Is Nothing Then
                        ModMain.FrmSetupLeft = New PageSetupLeft()
                    End If
                    pageSubType = ModMain.FrmSetupLeft.PageID
                ElseIf pageCurrent Is FormMain.PageType.Other Then
                    If ModMain.FrmOtherLeft Is Nothing Then
                        ModMain.FrmOtherLeft = New PageOtherLeft()
                    End If
                    pageSubType = ModMain.FrmOtherLeft.PageID
                ElseIf pageCurrent Is FormMain.PageType.InstanceSetup Then
                    If ModMain.FrmInstanceLeft Is Nothing Then
                        ModMain.FrmInstanceLeft = New PageInstanceLeft()
                    End If
                    pageSubType = ModMain.FrmInstanceLeft.PageID
                Else
                    pageSubType = FormMain.PageSubType.[Default]
                End If
                Return pageSubType
            End Get
        End Property

        Public Sub PageChange(Stack As FormMain.PageStackData, Optional SubType As FormMain.PageSubType = FormMain.PageSubType.[Default])
            If Operators.CompareString(Me.PageNameGet(Stack), "", False) = 0 Then
                Me.PageChangeExit()
                Me.IsChangingPage = True
                CType(Me.PanTitleSelect.Children(CInt(Stack)), MyRadioButton).SetChecked(True, True, Operators.CompareString(Me.PageNameGet(Me.PageCurrent), "", False) = 0)
                Me.IsChangingPage = False
                Select Case Stack.Page
                    Case FormMain.PageType.Download
                        If ModMain.FrmDownloadLeft Is Nothing Then
                            ModMain.FrmDownloadLeft = New PageDownloadLeft()
                        End If
                        CType(ModMain.FrmDownloadLeft.PanItem.Children(CInt(SubType)), MyListItem).SetChecked(True, True, Stack Is Me.PageCurrent)
                    Case FormMain.PageType.Setup
                        If ModMain.FrmSetupLeft Is Nothing Then
                            ModMain.FrmSetupLeft = New PageSetupLeft()
                        End If
                        CType(ModMain.FrmSetupLeft.PanItem.Children(CInt(SubType)), MyListItem).SetChecked(True, True, Stack Is Me.PageCurrent)
                    Case FormMain.PageType.Other
                        If ModMain.FrmOtherLeft Is Nothing Then
                            ModMain.FrmOtherLeft = New PageOtherLeft()
                        End If
                        CType(ModMain.FrmOtherLeft.PanItem.Children(CInt(SubType)), MyListItem).SetChecked(True, True, Stack Is Me.PageCurrent)
                End Select
                Me.PageChangeActual(Stack, SubType)
                Return
            End If
            Dim page As FormMain.PageType = Stack.Page
            If page = FormMain.PageType.InstanceSetup Then
                If ModMain.FrmInstanceLeft Is Nothing Then
                    ModMain.FrmInstanceLeft = New PageInstanceLeft()
                End If
                CType(ModMain.FrmInstanceLeft.PanItem.Children(CInt(SubType)), MyListItem).SetChecked(True, True, Stack Is Me.PageCurrent)
            End If
            Me.PageChangeActual(Stack, SubType)
        End Sub

        Private Sub BtnTitleSelect_Click(sender As MyRadioButton, raiseByMouse As Boolean)
            If Not Me.IsChangingPage Then
                ' The following expression was wrapped in a checked-expression
                Me.PageChangeActual(CType(Math.Round(ModBase.Val(RuntimeHelpers.GetObjectValue(sender.Tag))), FormMain.PageType), CType((-1), FormMain.PageSubType))
            End If
        End Sub

        Public Sub PageBack()
            If Me.PageStack.Any() Then
                Me.PageChangeActual(Me.PageStack(0), CType((-1), FormMain.PageSubType))
                Return
            End If
            Me.PageChange(FormMain.PageType.Launch, FormMain.PageSubType.[Default])
        End Sub

        Private Sub PageChangeActual(Stack As FormMain.PageStackData, Optional SubType As FormMain.PageSubType = CType((-1), FormMain.PageSubType))
            ' The following expression was wrapped in a checked-statement
            If Not(Me.PageCurrent Is Stack) OrElse (Me.PageCurrentSub <> SubType AndAlso SubType <> CType((-1), FormMain.PageSubType)) Then
                If Stack.Page = FormMain.PageType.InstanceSetup AndAlso PageInstanceLeft.Instance IsNot Nothing AndAlso ModMinecraft.McInstanceLock.IsLocked(PageInstanceLeft.Instance) Then
                    ModMain.Hint("该版本正在被修改，暂时无法进入版本设置！", ModMain.HintType.Red, True)
                    Return
                End If
                ModAnimation.AniControlEnabled += 1
                Try
                    Dim CS$<>8__locals1 As FormMain._Closure$__50-0 = New FormMain._Closure$__50-0(CS$<>8__locals1)
                    CS$<>8__locals1.$VB$Me = Me
                    CS$<>8__locals1.$VB$Local_PageName = Me.PageNameGet(Stack)

                        If Operators.CompareString(CS$<>8__locals1.$VB$Local_PageName, "", False) = 0 Then
                            Me.PageChangeExit()
                        ElseIf Me.PageStack.Any() Then
                            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.LabTitleInner, -Me.LabTitleInner.Opacity, 130, 0, Nothing, False), ModAnimation.AaCode(Sub()
                                CS$<>8__locals1.$VB$Me.LabTitleInner.Text = CS$<>8__locals1.$VB$Local_PageName
                            End Sub, 0, True), ModAnimation.AaOpacity(Me.LabTitleInner, 1.0, 150, 30, Nothing, False) }, "FrmMain Titlebar SubLayer", False)
                            If Me.PageStack.Contains(Stack) Then
                                While Me.PageStack.Contains(Stack)
                                    Me.PageStack.RemoveAt(0)
                                End While
                            Else
                                Me.PageStack.Insert(0, Me.PageCurrent)
                            End If
                        Else
                            Me.PanTitleInner.Visibility = Visibility.Visible
                            Me.PanTitleMain.IsHitTestVisible = False
                            Me.PanTitleInner.IsHitTestVisible = True
                            Me.PageNameRefresh(Stack)
                            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.PanTitleMain, -Me.PanTitleMain.Opacity, 150, 0, Nothing, False), ModAnimation.AaX(Me.PanTitleMain, 12.0 - Me.PanTitleMain.Margin.Left, 150, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaOpacity(Me.PanTitleInner, 1.0 - Me.PanTitleInner.Opacity, 150, 200, Nothing, False), ModAnimation.AaX(Me.PanTitleInner, -Me.PanTitleInner.Margin.Left, 350, 200, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaCode(Sub()
                                Me.PanTitleMain.Visibility = Visibility.Collapsed
                            End Sub, 0, True) }, "FrmMain Titlebar FirstLayer", False)
                            Me.PageStack.Insert(0, Me.PageCurrent)
                        End If
                        Me.PageLast = Me.PageCurrent
                        Me.PageCurrent = Stack
                        Select Case Stack.Page
                            Case FormMain.PageType.Launch
                                Me.PageChangeAnim(ModMain.FrmLaunchLeft, ModMain.FrmLaunchRight)
                            Case FormMain.PageType.Download
                                If ModMain.FrmDownloadLeft Is Nothing Then
                                    ModMain.FrmDownloadLeft = New PageDownloadLeft()
                                End If
                                Me.PageChangeAnim(ModMain.FrmDownloadLeft, CType(ModMain.FrmDownloadLeft.PageGet(SubType), FrameworkElement))
                            Case FormMain.PageType.Link
                                If ModMain.FrmLinkMain Is Nothing Then
                                    ModMain.FrmLinkMain = New PageLinkMain()
                                End If
                                Me.PageChangeAnim(New MyPageLeft(), ModMain.FrmLinkMain)
                            Case FormMain.PageType.Setup
                                If ModMain.FrmSetupLeft Is Nothing Then
                                    ModMain.FrmSetupLeft = New PageSetupLeft()
                                End If
                                Me.PageChangeAnim(ModMain.FrmSetupLeft, CType(ModMain.FrmSetupLeft.PageGet(SubType), FrameworkElement))
                            Case FormMain.PageType.Other
                                If ModMain.FrmOtherLeft Is Nothing Then
                                    ModMain.FrmOtherLeft = New PageOtherLeft()
                                End If
                                Me.PageChangeAnim(ModMain.FrmOtherLeft, CType(ModMain.FrmOtherLeft.PageGet(SubType), FrameworkElement))
                            Case FormMain.PageType.InstanceSelect
                                If ModMain.FrmSelectLeft Is Nothing Then
                                    ModMain.FrmSelectLeft = New PageSelectLeft()
                                End If
                                If ModMain.FrmSelectRight Is Nothing Then
                                    ModMain.FrmSelectRight = New PageSelectRight()
                                End If
                                Me.PageChangeAnim(ModMain.FrmSelectLeft, ModMain.FrmSelectRight)
                            Case FormMain.PageType.TaskManager
                                If ModMain.FrmSpeedLeft Is Nothing Then
                                    ModMain.FrmSpeedLeft = New PageSpeedLeft()
                                End If
                                If ModMain.FrmSpeedRight Is Nothing Then
                                    ModMain.FrmSpeedRight = New PageSpeedRight()
                                End If
                                Me.PageChangeAnim(ModMain.FrmSpeedLeft, ModMain.FrmSpeedRight)
                            Case FormMain.PageType.InstanceSetup
                                If ModMain.FrmInstanceLeft Is Nothing Then
                                    ModMain.FrmInstanceLeft = New PageInstanceLeft()
                                End If
                                Me.PageChangeAnim(ModMain.FrmInstanceLeft, CType(ModMain.FrmInstanceLeft.PageGet(SubType), FrameworkElement))
                            Case FormMain.PageType.ResourceDetail
                                If ModMain.FrmDownloadResourceDetail Is Nothing Then
                                    ModMain.FrmDownloadResourceDetail = New PageDownloadResourceDetail()
                                End If
                                Me.PageChangeAnim(New MyPageLeft(), ModMain.FrmDownloadResourceDetail)
                            Case FormMain.PageType.HelpDetail
                                Me.PageChangeAnim(New MyPageLeft(), CType(NewLateBinding.LateIndexGet(Stack.Additional, New Object() { 1 }, Nothing), FrameworkElement))
                        End Select
                        Me.BtnExtraDownload.ShowRefresh()
                        Me.BtnExtraApril.ShowRefresh()
                        Logger.Info(String.Format("切换主要页面：{0}, {1}", Stack.Page, SubType), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")

                Catch ex As Exception
                    Logger.[Error](ex, String.Format("切换主要页面失败（ID {0}）", Me.PageCurrent.Page), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
                Finally
                    ModAnimation.AniControlEnabled -= 1
                End Try
            End If
        End Sub

        Private Sub PageChangeAnim(TargetLeft As FrameworkElement, TargetRight As FrameworkElement)
            ModAnimation.AniStop("FrmMain LeftChange")
            ModAnimation.AniStop("PageLeft PageChange")
            ModAnimation.AniControlEnabled += 1
            If TargetLeft.Parent IsNot Nothing Then
                TargetLeft.SetValue(ContentPresenter.ContentProperty, Nothing)
            End If
            If TargetRight IsNot Nothing AndAlso TargetRight.Parent IsNot Nothing Then
                TargetRight.SetValue(ContentPresenter.ContentProperty, Nothing)
            End If
            Me.PageLeft = CType(TargetLeft, MyPageLeft)
            Me.PageRight = CType(TargetRight, MyPageRight)
            CType(Me.PanMainLeft.Child, MyPageLeft).TriggerHideAnimation()
            CType(Me.PanMainRight.Child, MyPageRight).PageOnExit()
            ModAnimation.AniControlEnabled -= 1
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaCode(Sub()
                ModAnimation.AniControlEnabled += 1
                Me.PanMainLeft.Child = Me.PageLeft
                Me.PageLeft.Opacity = 0.0
                Me.PanMainLeft.Background = Nothing
                ModAnimation.AniControlEnabled -= 1
                ModBase.RunInUi(Sub()
                    Me.PanMainLeft_Resize(Me.PanMainLeft.ActualWidth)
                End Sub, True)
            End Sub, 110, False), ModAnimation.AaCode(Sub()
                Me.PageLeft.Opacity = 1.0
                Me.PageLeft.TriggerShowAnimation()
            End Sub, 30, True) }, "FrmMain PageChangeLeft", False)
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaCode(Sub()
                ModAnimation.AniControlEnabled += 1
                CType(Me.PanMainRight.Child, MyPageRight).PageOnForceExit()
                Me.PanMainRight.Child = Me.PageRight
                Me.PageRight.Opacity = 0.0
                Me.PanMainRight.Background = Nothing
                ModAnimation.AniControlEnabled -= 1
                ModBase.RunInUi(Sub()
                    Me.BtnExtraBack.ShowRefresh()
                End Sub, True)
            End Sub, 110, False), ModAnimation.AaCode(Sub()
                Me.PageRight.Opacity = 1.0
                Me.PageRight.PageOnEnter()
            End Sub, 30, True) }, "FrmMain PageChangeRight", False)
        End Sub

        Private Sub PageChangeExit()
            If Me.PageStack.Any() Then
                Me.PanTitleMain.Visibility = Visibility.Visible
                Me.PanTitleMain.IsHitTestVisible = True
                Me.PanTitleInner.IsHitTestVisible = False
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.PanTitleInner, -Me.PanTitleInner.Opacity, 150, 0, Nothing, False), ModAnimation.AaX(Me.PanTitleInner, -18.0 - Me.PanTitleInner.Margin.Left, 150, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaOpacity(Me.PanTitleMain, 1.0 - Me.PanTitleMain.Opacity, 150, 200, Nothing, False), ModAnimation.AaX(Me.PanTitleMain, -Me.PanTitleMain.Margin.Left, 350, 200, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaCode(Sub()
                    Me.PanTitleInner.Visibility = Visibility.Collapsed
                End Sub, 0, True) }, "FrmMain Titlebar FirstLayer", False)
                Me.PageStack.Clear()
            End If
        End Sub

        Private Sub PanMainLeft_SizeChanged(sender As Object, e As SizeChangedEventArgs)
            If e.WidthChanged Then
                Me.PanMainLeft_Resize(e.NewSize.Width)
            End If
        End Sub

        Private Sub PanMainLeft_Resize(NewWidth As Double)
            If Math.Abs(NewWidth - Me.RectLeftBackground.Width) <= 0.1 OrElse ModAnimation.AniControlEnabled <> 0 Then
                Me.RectLeftBackground.Width = NewWidth
                Me.PanMainLeft.IsHitTestVisible = True
                ModAnimation.AniStop("FrmMain LeftChange")
                Return
            End If
            If Me.PanMain.Opacity < 0.1 Then
                Me.PanMainLeft.IsHitTestVisible = False
            End If
            If NewWidth > 0.0 Then
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaWidth(Me.RectLeftBackground, NewWidth - Me.RectLeftBackground.Width, 180, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.ExtraStrong), False), ModAnimation.AaOpacity(Me.RectLeftShadow, 1.0 - Me.RectLeftShadow.Opacity, 180, 0, Nothing, False), ModAnimation.AaCode(Sub()
                    Me.PanMainLeft.IsHitTestVisible = True
                End Sub, 150, False) }, "FrmMain LeftChange", True)
                Return
            End If
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaWidth(Me.RectLeftBackground, -Me.RectLeftBackground.Width, 180, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaOpacity(Me.RectLeftShadow, -Me.RectLeftShadow.Opacity, 180, 0, Nothing, False), ModAnimation.AaCode(Sub()
                Me.PanMainLeft.IsHitTestVisible = True
            End Sub, 150, False) }, "FrmMain LeftChange", True)
        End Sub

        Public Sub DragTick()
            If ModMain.DragControl IsNot Nothing AndAlso Mouse.LeftButton <> MouseButtonState.Pressed Then
                Me.DragStop()
            End If
        End Sub

        Public Sub DragDoing()
            If ModMain.DragControl IsNot Nothing Then
                If Mouse.LeftButton = MouseButtonState.Pressed Then
                    NewLateBinding.LateCall(ModMain.DragControl, Nothing, "DragDoing", New Object(-1) {}, Nothing, Nothing, Nothing, True)
                    Return
                End If
                Me.DragStop()
            End If
        End Sub

        Public Sub DragStop()
            Dim action As Action
            If FormMain._Closure$__.$I57-0 IsNot Nothing Then
                action = FormMain._Closure$__.$I57-0
            Else
                Dim action2 As Action = Sub()
                    If ModMain.DragControl IsNot Nothing Then
                        Dim objectValue As Object = RuntimeHelpers.GetObjectValue(ModMain.DragControl)
                        ModMain.DragControl = Nothing
                        NewLateBinding.LateCall(objectValue, Nothing, "DragStop", New Object(-1) {}, Nothing, Nothing, Nothing, True)
                    End If
                End Sub
                action = action2
                FormMain._Closure$__.$I57-0 = action2
            End If
            ModBase.RunInUi(action, False)
        End Sub

        Private Sub BtnExtraMusic_Click(sender As Object, e As EventArgs)
            ModMusic.MusicControlPause()
        End Sub

        Private Sub BtnExtraMusic_RightClick(sender As Object, e As EventArgs)
            ModMusic.MusicControlNext()
        End Sub

        Private Sub BtnExtraDownload_Click(sender As Object, e As EventArgs)
            Me.PageChange(FormMain.PageType.TaskManager, FormMain.PageSubType.[Default])
        End Sub

        Private Function BtnExtraDownload_ShowCheck() As Boolean
            Return ModNet.HasDownloadingTask(False) AndAlso Not(Me.PageCurrent Is FormMain.PageType.TaskManager)
        End Function

        Public Sub AprilGiveup()
            If ModMain.IsAprilEnabled AndAlso Not ModMain.IsAprilGiveup Then
                ModMain.Hint("=D", ModMain.HintType.Green, True)
                ModMain.IsAprilGiveup = True
                ModMain.FrmLaunchLeft.AprilScaleTrans.ScaleX = 1.0
                ModMain.FrmLaunchLeft.AprilScaleTrans.ScaleY = 1.0
                Me.BtnExtraApril.ShowRefresh()
            End If
        End Sub

        Public Function BtnExtraApril_ShowCheck() As Boolean
            Return ModMain.IsAprilEnabled AndAlso Not ModMain.IsAprilGiveup AndAlso Me.PageCurrent Is FormMain.PageType.Launch
        End Function

        Public Sub BtnExtraShutdown_Click()
            Try
                If ModLaunch.McLaunchLoaderReal IsNot Nothing Then
                    ModLaunch.McLaunchLoaderReal.Cancel()
                End If
                Try
                    For Each watcher As ModWatcher.Watcher In ModWatcher.McWatcherList
                        watcher.Kill()
                    Next
                Finally
                    Dim enumerator As List(Of ModWatcher.Watcher).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                ModMain.Hint("已关闭运行中的 Minecraft！", ModMain.HintType.Green, True)
            Catch ex As Exception
                Logger.[Error](ex, "强制关闭所有 Minecraft 失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
            End Try
        End Sub

        Public Function BtnExtraShutdown_ShowCheck() As Boolean
            Return ModWatcher.HasRunningMinecraft
        End Function

        Public Sub BackToTop()
            Dim myScrollViewer As MyScrollViewer = Me.BtnExtraBack_GetRealChild()
            If myScrollViewer IsNot Nothing Then
                myScrollViewer.PerformVerticalOffsetDelta(-myScrollViewer.VerticalOffset)
                Return
            End If
            Logger.[Error]("无法返回顶部，未找到合适的 RealScroll", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\FormMain.xaml.vb")
        End Sub

        Private Function BtnExtraBack_ShowCheck() As Boolean
            Dim myScrollViewer As MyScrollViewer = Me.BtnExtraBack_GetRealChild()
            Return myScrollViewer IsNot Nothing AndAlso myScrollViewer.Visibility = Visibility.Visible AndAlso myScrollViewer.VerticalOffset > MyBase.Height + CDbl(If(Me.BtnExtraBack.Show, 0, 700))
        End Function

        Private Function BtnExtraBack_GetRealChild() As MyScrollViewer
            Dim myScrollViewer As MyScrollViewer
            If Me.PanMainRight.Child Is Nothing OrElse Not(TypeOf Me.PanMainRight.Child Is MyPageRight) Then
                myScrollViewer = Nothing
            Else
                Dim myPageRight As MyPageRight = CType(Me.PanMainRight.Child, MyPageRight)
                myScrollViewer = CType(myPageRight.FindName(myPageRight.PanScroll), MyScrollViewer)
            End If
            Return myScrollViewer
        End Function

        Private Sub FormMain_MouseMove(sender As Object, e As MouseEventArgs)
            Me.lastMouseArg = e
        End Sub

        Friend Overridable Property WindMain As FormMain

        Friend Overridable Property PanBack As Grid
            <CompilerGenerated()>
            Get
                Return Me._PanBack
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Grid)
                Dim mouseEventHandler As MouseEventHandler = Sub(a0 As Object, a1 As MouseEventArgs)
                    Me.DragDoing()
                End Sub
                Dim grid As Grid = Me._PanBack
                If grid IsNot Nothing Then
                    RemoveHandler grid.MouseMove, mouseEventHandler
                End If
                Me._PanBack = value
                grid = Me._PanBack
                If grid IsNot Nothing Then
                    AddHandler grid.MouseMove, mouseEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property TransformRotate As RotateTransform

        Friend Overridable Property TransformPos As TranslateTransform

        Friend Overridable Property ResizerT As Rectangle

        Friend Overridable Property ResizerB As Rectangle

        Friend Overridable Property ResizerR As Rectangle

        Friend Overridable Property ResizerL As Rectangle

        Friend Overridable Property ResizerLT As Rectangle

        Friend Overridable Property ResizerLB As Rectangle

        Friend Overridable Property ResizerRB As Rectangle

        Friend Overridable Property ResizerRT As Rectangle

        Friend Overridable Property BorderForm As Border

        Friend Overridable Property RectForm As RectangleGeometry

        Friend Overridable Property PanForm As Grid

        Friend Overridable Property ImgBack As Canvas

        Friend Overridable Property PanTitle As Grid
            <CompilerGenerated()>
            Get
                Return Me._PanTitle
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Grid)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = AddressOf Me.FormDragMove
                Dim grid As Grid = Me._PanTitle
                If grid IsNot Nothing Then
                    RemoveHandler grid.MouseLeftButtonDown, mouseButtonEventHandler
                End If
                Me._PanTitle = value
                grid = Me._PanTitle
                If grid IsNot Nothing Then
                    AddHandler grid.MouseLeftButtonDown, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ImgTitle As MyImage

        Friend Overridable Property BtnTitleClose As MyIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnTitleClose
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.BtnTitleClose_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                Dim myIconButton As MyIconButton = Me._BtnTitleClose
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnTitleClose = value
                myIconButton = Me._BtnTitleClose
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnTitleMin As MyIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnTitleMin
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.BtnTitleMin_Click()
                End Sub
                Dim myIconButton As MyIconButton = Me._BtnTitleMin
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnTitleMin = value
                myIconButton = Me._BtnTitleMin
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanTitleMain As Grid

        Friend Overridable Property ShapeTitleLogo As Path

        Friend Overridable Property LabTitleLogo As TextBlock

        Friend Overridable Property ImageTitleLogo As MyImage

        Friend Overridable Property PanTitleSelect As StackPanel

        Friend Overridable Property BtnTitleSelect0 As MyRadioButton
            <CompilerGenerated()>
            Get
                Return Me._BtnTitleSelect0
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyRadioButton)
                Dim checkEventHandler As MyRadioButton.CheckEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.BtnTitleSelect_Click(CType(a0, MyRadioButton), a1)
                End Sub
                Dim myRadioButton As MyRadioButton = Me._BtnTitleSelect0
                If myRadioButton IsNot Nothing Then
                    RemoveHandler myRadioButton.Check, checkEventHandler
                End If
                Me._BtnTitleSelect0 = value
                myRadioButton = Me._BtnTitleSelect0
                If myRadioButton IsNot Nothing Then
                    AddHandler myRadioButton.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnTitleSelect1 As MyRadioButton
            <CompilerGenerated()>
            Get
                Return Me._BtnTitleSelect1
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyRadioButton)
                Dim checkEventHandler As MyRadioButton.CheckEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.BtnTitleSelect_Click(CType(a0, MyRadioButton), a1)
                End Sub
                Dim myRadioButton As MyRadioButton = Me._BtnTitleSelect1
                If myRadioButton IsNot Nothing Then
                    RemoveHandler myRadioButton.Check, checkEventHandler
                End If
                Me._BtnTitleSelect1 = value
                myRadioButton = Me._BtnTitleSelect1
                If myRadioButton IsNot Nothing Then
                    AddHandler myRadioButton.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnTitleSelect2 As MyRadioButton
            <CompilerGenerated()>
            Get
                Return Me._BtnTitleSelect2
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyRadioButton)
                Dim checkEventHandler As MyRadioButton.CheckEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.BtnTitleSelect_Click(CType(a0, MyRadioButton), a1)
                End Sub
                Dim myRadioButton As MyRadioButton = Me._BtnTitleSelect2
                If myRadioButton IsNot Nothing Then
                    RemoveHandler myRadioButton.Check, checkEventHandler
                End If
                Me._BtnTitleSelect2 = value
                myRadioButton = Me._BtnTitleSelect2
                If myRadioButton IsNot Nothing Then
                    AddHandler myRadioButton.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnTitleSelect3 As MyRadioButton
            <CompilerGenerated()>
            Get
                Return Me._BtnTitleSelect3
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyRadioButton)
                Dim checkEventHandler As MyRadioButton.CheckEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.BtnTitleSelect_Click(CType(a0, MyRadioButton), a1)
                End Sub
                Dim myRadioButton As MyRadioButton = Me._BtnTitleSelect3
                If myRadioButton IsNot Nothing Then
                    RemoveHandler myRadioButton.Check, checkEventHandler
                End If
                Me._BtnTitleSelect3 = value
                myRadioButton = Me._BtnTitleSelect3
                If myRadioButton IsNot Nothing Then
                    AddHandler myRadioButton.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnTitleSelect4 As MyRadioButton
            <CompilerGenerated()>
            Get
                Return Me._BtnTitleSelect4
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyRadioButton)
                Dim checkEventHandler As MyRadioButton.CheckEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.BtnTitleSelect_Click(CType(a0, MyRadioButton), a1)
                End Sub
                Dim myRadioButton As MyRadioButton = Me._BtnTitleSelect4
                If myRadioButton IsNot Nothing Then
                    RemoveHandler myRadioButton.Check, checkEventHandler
                End If
                Me._BtnTitleSelect4 = value
                myRadioButton = Me._BtnTitleSelect4
                If myRadioButton IsNot Nothing Then
                    AddHandler myRadioButton.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanTitleInner As Grid

        Friend Overridable Property BtnTitleInner As MyIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnTitleInner
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.PageBack()
                End Sub
                Dim myIconButton As MyIconButton = Me._BtnTitleInner
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnTitleInner = value
                myIconButton = Me._BtnTitleInner
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabTitleInner As TextBlock

        Friend Overridable Property PanLeft As Grid

        Friend Overridable Property RectLeftBackground As Rectangle

        Friend Overridable Property RectLeftShadow As Rectangle

        Friend Overridable Property PanMain As Grid

        Friend Overridable Property PanMainRight As Border

        Friend Overridable Property PanMainLeft As Border
            <CompilerGenerated()>
            Get
                Return Me._PanMainLeft
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Border)
                Dim sizeChangedEventHandler As SizeChangedEventHandler = AddressOf Me.PanMainLeft_SizeChanged
                Dim border As Border = Me._PanMainLeft
                If border IsNot Nothing Then
                    RemoveHandler border.SizeChanged, sizeChangedEventHandler
                End If
                Me._PanMainLeft = value
                border = Me._PanMainLeft
                If border IsNot Nothing Then
                    AddHandler border.SizeChanged, sizeChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanHint As StackPanel

        Friend Overridable Property BtnExtraBack As MyExtraButton
            <CompilerGenerated()>
            Get
                Return Me._BtnExtraBack
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyExtraButton)
                Dim clickEventHandler As MyExtraButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.BackToTop()
                End Sub
                Dim myExtraButton As MyExtraButton = Me._BtnExtraBack
                If myExtraButton IsNot Nothing Then
                    RemoveHandler myExtraButton.Click, clickEventHandler
                End If
                Me._BtnExtraBack = value
                myExtraButton = Me._BtnExtraBack
                If myExtraButton IsNot Nothing Then
                    AddHandler myExtraButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnExtraDownload As MyExtraButton
            <CompilerGenerated()>
            Get
                Return Me._BtnExtraDownload
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyExtraButton)
                Dim clickEventHandler As MyExtraButton.ClickEventHandler = AddressOf Me.BtnExtraDownload_Click
                Dim myExtraButton As MyExtraButton = Me._BtnExtraDownload
                If myExtraButton IsNot Nothing Then
                    RemoveHandler myExtraButton.Click, clickEventHandler
                End If
                Me._BtnExtraDownload = value
                myExtraButton = Me._BtnExtraDownload
                If myExtraButton IsNot Nothing Then
                    AddHandler myExtraButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnExtraApril As MyExtraButton
            <CompilerGenerated()>
            Get
                Return Me._BtnExtraApril
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyExtraButton)
                Dim clickEventHandler As MyExtraButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.AprilGiveup()
                End Sub
                Dim myExtraButton As MyExtraButton = Me._BtnExtraApril
                If myExtraButton IsNot Nothing Then
                    RemoveHandler myExtraButton.Click, clickEventHandler
                End If
                Me._BtnExtraApril = value
                myExtraButton = Me._BtnExtraApril
                If myExtraButton IsNot Nothing Then
                    AddHandler myExtraButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnExtraShutdown As MyExtraButton
            <CompilerGenerated()>
            Get
                Return Me._BtnExtraShutdown
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyExtraButton)
                Dim clickEventHandler As MyExtraButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.BtnExtraShutdown_Click()
                End Sub
                Dim myExtraButton As MyExtraButton = Me._BtnExtraShutdown
                If myExtraButton IsNot Nothing Then
                    RemoveHandler myExtraButton.Click, clickEventHandler
                End If
                Me._BtnExtraShutdown = value
                myExtraButton = Me._BtnExtraShutdown
                If myExtraButton IsNot Nothing Then
                    AddHandler myExtraButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnExtraMusic As MyExtraButton
            <CompilerGenerated()>
            Get
                Return Me._BtnExtraMusic
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyExtraButton)
                Dim clickEventHandler As MyExtraButton.ClickEventHandler = AddressOf Me.BtnExtraMusic_Click
                Dim rightClickEventHandler As MyExtraButton.RightClickEventHandler = AddressOf Me.BtnExtraMusic_RightClick
                Dim myExtraButton As MyExtraButton = Me._BtnExtraMusic
                If myExtraButton IsNot Nothing Then
                    RemoveHandler myExtraButton.Click, clickEventHandler
                    RemoveHandler myExtraButton.RightClick, rightClickEventHandler
                End If
                Me._BtnExtraMusic = value
                myExtraButton = Me._BtnExtraMusic
                If myExtraButton IsNot Nothing Then
                    AddHandler myExtraButton.Click, clickEventHandler
                    AddHandler myExtraButton.RightClick, rightClickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanMsg As Grid
            <CompilerGenerated()>
            Get
                Return Me._PanMsg
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Grid)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = AddressOf Me.FormDragMove
                Dim grid As Grid = Me._PanMsg
                If grid IsNot Nothing Then
                    RemoveHandler grid.MouseLeftButtonDown, mouseButtonEventHandler
                End If
                Me._PanMsg = value
                grid = Me._PanMsg
                If grid IsNot Nothing Then
                    AddHandler grid.MouseLeftButtonDown, mouseButtonEventHandler
                End If
            End Set
        End Property

        Private IsWindowLoadFinished As Boolean

        Private Shared IsLogShown As Boolean = False

        Public IsSizeSaveable As Boolean

        Public IsSystemTimeChanged As Boolean

        Private _Hidden As Boolean

        Public PageCurrent As FormMain.PageStackData

        Public PageLast As FormMain.PageStackData

        Public PageStack As List(Of FormMain.PageStackData)

        Public PageLeft As MyPageLeft

        Public PageRight As MyPageRight

        Private IsChangingPage As Boolean

        Public lastMouseArg As MouseEventArgs

        Private $STATIC$.ctor$2001$Helper As DragHelper

        Private $STATIC$.ctor$2001$Helper$Init As StaticLocalInitFlag

        Private $STATIC$HandleDrag$20211C1284DD$PrevData As IDataObject

        Private $STATIC$HandleDrag$20211C1284DD$PrevEffects As DragDropEffects

        Public Enum PageType
            Launch
            Download
            Link
            Setup
            Other
            InstanceSelect
            TaskManager
            InstanceSetup
            ResourceDetail
            HelpDetail
        End Enum

        Public Enum PageSubType
            [Default]
            DownloadInstall = 0
            DownloadMod = 2
            DownloadPack
            DownloadDataPack
            DownloadResourcePack
            DownloadShader
            SetupLaunch = 0
            SetupLink
            SetupUI
            SetupSystem
            LinkMain = 0
            OtherHelp = 0
            OtherAbout
            OtherTest
            InstanceOverall = 0
            InstanceSetup
            InstanceModify
            InstanceMod
            InstanceModDisabled
            InstanceExport
        End Enum

        Public Class PageStackData
            Public Overrides Function Equals(other As Object) As Boolean
                Dim flag As Boolean
                If other Is Nothing Then
                    flag = False
                ElseIf TypeOf other Is FormMain.PageStackData Then
                    Dim pageStackData As FormMain.PageStackData = CType(other, FormMain.PageStackData)
                    If Me.Page <> pageStackData.Page Then
                        flag = False
                    ElseIf Me.Additional Is Nothing Then
                        flag = pageStackData.Additional Is Nothing
                    Else
                        flag = pageStackData.Additional IsNot Nothing AndAlso Me.Additional.Equals(RuntimeHelpers.GetObjectValue(pageStackData.Additional))
                    End If
                Else
                    flag = TypeOf other Is Integer AndAlso Not Operators.ConditionalCompareObjectNotEqual(Me.Page, other, False) AndAlso Me.Additional Is Nothing
                End If
                Return flag
            End Function

            Public Shared Operator =(left As FormMain.PageStackData, right As FormMain.PageStackData) As Boolean
                Return EqualityComparer(Of FormMain.PageStackData).[Default].Equals(left, right)
            End Operator

            Public Shared Operator <>(left As FormMain.PageStackData, right As FormMain.PageStackData) As Boolean
                Return Not(left Is right)
            End Operator

            Public Shared Widening Operator CType(Value As FormMain.PageType) As FormMain.PageStackData
                Return New FormMain.PageStackData() With { .Page = Value }
            End Operator

            Public Shared Widening Operator CType(Value As FormMain.PageStackData) As FormMain.PageType
                Return Value.Page
            End Operator

            Public Page As FormMain.PageType

            Public Additional As Object
        End Class
    End Class
End Namespace
