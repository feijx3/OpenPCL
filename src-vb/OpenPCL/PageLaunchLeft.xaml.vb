Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports System.Windows.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageLaunchLeft
        Inherits OurPageLeft

        Public Sub New()
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.PageLaunchLeft_Loaded()
            End Sub
            Me.IsLoad = False
            Me.IsLoadFinished = False
            Me.PageCurrent = PageLaunchLeft.PageType.None
            Me.BtnLaunchState = 0
            Me.BtnLaunchInstance = Nothing
            Me.ShowProgress = 0.0
            Me.IsWidthAnimating = False
            Me.IsHeightAnimating = False
            Me.InitializeComponent()
        End Sub

        Public Sub PageLaunchLeft_Loaded()
            If Me.IsLoad Then
                Me.RefreshPage(True, False)
            End If
            Me.AprilPosTrans.X = 0.0
            Me.AprilPosTrans.Y = 0.0
            If Not Me.IsLoad Then
                Me.IsLoad = True
                ModAnimation.AniControlEnabled += 1
                AddHandler ModMinecraft.McInstanceListLoader.LoadingStateChanged, Sub(a0 As OurLoading.OurLoadingState, a1 As OurLoading.OurLoadingState)
                    Me.RefreshButtonsUI()
                End Sub
                AddHandler ModMinecraft.McFolderListLoader.LoadingStateChanged, Sub(a0 As OurLoading.OurLoadingState, a1 As OurLoading.OurLoadingState)
                    Me.RefreshButtonsUI()
                End Sub
                Me.RefreshButtonsUI()
                ModBase.RunInNewThread(Sub()
                    Dim text As String = Nothing
                    If FileUtils.Exists(Paths.Base + "modpack.zip") Then
                        text = Paths.Base + "modpack.zip"
                    End If
                    If FileUtils.Exists(Paths.Base + "modpack.mrpack") Then
                        text = Paths.Base + "modpack.mrpack"
                    End If
                    If text IsNot Nothing Then
                        Logger.Warn(String.Format("需自动安装整合包：{0}", text), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                        PageSelectLeft.CreateMcFolderInCurrentPath()
                        ModMinecraft.McFolderListLoader.WaitForExit(Nothing, Nothing, False, Nothing, Nothing)
                    End If
                    If Operators.CompareString(ModMinecraft.McFolderSelected, "", False) = 0 OrElse Not DirectoryUtils.Exists(ModMinecraft.McFolderSelected) Then
                        If Operators.CompareString(ModMinecraft.McFolderSelected, "", False) = 0 Then
                            Logger.Info("没有已储存的 Minecraft 文件夹", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                        Else
                            Logger.Warn(String.Format("Minecraft 文件夹无效，该文件夹已不存在：{0}", ModMinecraft.McFolderSelected), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                        End If
                        ModMinecraft.McFolderListLoader.WaitForExit(Nothing, Nothing, True, Nothing, Nothing)
                        ModMinecraft.McFolderSelected = ModMinecraft.McFolderList.First().Location
                    End If
                    If Settings.[Get](Of Boolean)("SystemDebugDelay", Nothing) Then
                        Thread.Sleep(ModBase.RandomInteger(500, 3000))
                    End If
                    If text IsNot Nothing Then
                        Try
                            Dim loaderCombo As ModLoader.LoaderCombo(Of String) = ModModpack.ModpackInstall(text, Nothing, Nothing)
                            Logger.Info(String.Format("自动安装整合包已开始：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                            loaderCombo.WaitForExit(Nothing, Nothing, False, Nothing, Nothing)
                            If loaderCombo.State = ModBase.LoadState.Finished Then
                                Logger.Info(String.Format("自动安装整合包成功，清理安装包：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                                FileUtils.Delete(text, False)
                            End If
                        Catch ex As Exception
                            If ex.IsCanceled() Then
                                Logger.Info(String.Format("自动安装整合包已取消：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                            Else
                                Logger.[Error](ex, String.Format("自动安装整合包失败：{0}", text), LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                            End If
                        End Try
                    End If
                    Dim text2 As String = ModBase.ReadIni(ModMinecraft.McFolderSelected + "PCL.ini", "Version", "")
                    Dim Instance As ModMinecraft.McInstance = If((Operators.CompareString(text2, "", False) = 0), Nothing, New ModMinecraft.McInstance(text2))
                    If Instance Is Nothing OrElse Not PathUtils.IsParentOf(ModMinecraft.McFolderSelected, Instance.PathVersion) OrElse Not Instance.Check() Then
                        Logger.Info(String.Format("当前选择的 Minecraft 版本无效：{0}", If((Instance Is Nothing), "null", Instance.PathVersion)), If(Information.IsNothing(Instance), LogBehavior.None, LogBehavior.ToastIfDebug), "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                        If ModMinecraft.McInstanceListLoader.State <> ModBase.LoadState.Finished Then
                            ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", True)
                        End If
                        If Not ModMinecraft.McInstanceList.Any() OrElse ModMinecraft.McInstanceList.First().Value(0).Logo.Contains("RedstoneBlock") Then
                            Instance = Nothing
                            Logger.Info("无可用 Minecraft 版本", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                        Else
                            Instance = ModMinecraft.McInstanceList.First().Value(0)
                            Logger.Info(String.Format("自动选择 Minecraft 版本：{0}", Instance.PathVersion), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                        End If
                    End If
                    ModBase.RunInUi(Sub()
                        ModMinecraft.McInstanceSelected = Instance
                        Me.IsLoadFinished = True
                        Me.RefreshButtonsUI()
                        Me.RefreshPage(False, False)
                        If Operators.CompareString(ModLaunch.McLoginAble(), "", False) = 0 Then
                            ModLaunch.McLoginLoader.Start(Nothing, False)
                        End If
                        If Environment.CommandLine.Contains("--test") Then
                            ModBase.RunInThread(Sub()
                                Thread.Sleep(500)
                                Dim action As Action
                                If PageLaunchLeft._Closure$__.$I4-3 IsNot Nothing Then
                                    action = PageLaunchLeft._Closure$__.$I4-3
                                Else
                                    Dim action2 As Action = Sub()
                                        ModMain.FrmMain.PageChange(FormMain.PageType.Download, FormMain.PageSubType.[Default])
                                    End Sub
                                    action = action2
                                    PageLaunchLeft._Closure$__.$I4-3 = action2
                                End If
                                ModBase.RunInUi(action, False)
                                Thread.Sleep(500)
                                Dim action3 As Action
                                If PageLaunchLeft._Closure$__.$I4-4 IsNot Nothing Then
                                    action3 = PageLaunchLeft._Closure$__.$I4-4
                                Else
                                    Dim action4 As Action = Sub()
                                        ModMain.FrmMain.PageChange(FormMain.PageType.Download, FormMain.PageSubType.DownloadMod)
                                    End Sub
                                    action3 = action4
                                    PageLaunchLeft._Closure$__.$I4-4 = action4
                                End If
                                ModBase.RunInUi(action3, False)
                                Thread.Sleep(500)
                                Dim action5 As Action
                                If PageLaunchLeft._Closure$__.$I4-5 IsNot Nothing Then
                                    action5 = PageLaunchLeft._Closure$__.$I4-5
                                Else
                                    Dim action6 As Action = Sub()
                                        ModMain.FrmMain.PageChange(FormMain.PageType.Setup, FormMain.PageSubType.[Default])
                                    End Sub
                                    action5 = action6
                                    PageLaunchLeft._Closure$__.$I4-5 = action6
                                End If
                                ModBase.RunInUi(action5, False)
                                Thread.Sleep(500)
                                Dim action7 As Action
                                If PageLaunchLeft._Closure$__.$I4-6 IsNot Nothing Then
                                    action7 = PageLaunchLeft._Closure$__.$I4-6
                                Else
                                    Dim action8 As Action = Sub()
                                        ModMain.FrmMain.PageChange(FormMain.PageType.Other, FormMain.PageSubType.[Default])
                                    End Sub
                                    action7 = action8
                                    PageLaunchLeft._Closure$__.$I4-6 = action8
                                End If
                                ModBase.RunInUi(action7, False)
                                Thread.Sleep(500)
                                ModBase.RunInUi(Sub()
                                    Me.BtnVersion_Click()
                                End Sub, False)
                                Thread.Sleep(500)
                                ModBase.RunInUi(Sub()
                                    Me.BtnMore_Click()
                                End Sub, False)
                                Thread.Sleep(500)
                                FormMain.EndProgramForce(CType(123, ModBase.ProcessReturnValues))
                            End Sub)
                        End If
                    End Sub, False)
                End Sub, "Version Check", ThreadPriority.AboveNormal)
                Dim mcLoginType As ModLaunch.McLoginType = Settings.[Get](Of ModLaunch.McLoginType)("LoginType", Nothing)
                If mcLoginType = ModLaunch.McLoginType.Legacy OrElse mcLoginType = ModLaunch.McLoginType.Ms Then
                    CType(MyBase.FindName("RadioLoginType" + Conversions.ToString(CInt(mcLoginType))), OurRadioButton).Checked = True
                End If
                Me.RefreshPage(False, False)
                ModAnimation.AniControlEnabled -= 1
            End If
        End Sub

        Public Sub PageChangeToLaunching()
            Select Case Settings.[Get](Of ModLaunch.McLoginType)("LoginType", Nothing)
                Case ModLaunch.McLoginType.Legacy
                    Me.LabLaunchingMethod.Text = "离线登录"
                Case ModLaunch.McLoginType.Nide
                    Me.LabLaunchingMethod.Text = "统一通行证"
                Case ModLaunch.McLoginType.Auth
                    Me.LabLaunchingMethod.Text = "Authlib-Injector"
                Case ModLaunch.McLoginType.Ms
                    Me.LabLaunchingMethod.Text = "正版登录"
            End Select
            Me.LabLaunchingName.Text = ModMinecraft.McInstanceSelected.Name
            Me.LabLaunchingStage.Text = "初始化"
            Dim labLaunchingTitle As TextBlock = Me.LabLaunchingTitle
            Dim currentLaunchOptions As ModLaunch.McLaunchOptions = ModLaunch.CurrentLaunchOptions
            labLaunchingTitle.Text = If((If((currentLaunchOptions IsNot Nothing), currentLaunchOptions.SaveBatch, Nothing) Is Nothing), "正在启动游戏", "正在导出启动脚本")
            Me.LabLaunchingProgress.Text = "0.00 %"
            Me.LabLaunchingProgress.Opacity = 1.0
            Me.LabLaunchingDownload.Visibility = Visibility.Visible
            Me.LabLaunchingProgressLeft.Opacity = 0.6
            Me.LabLaunchingDownload.Visibility = Visibility.Visible
            Me.LabLaunchingDownload.Text = "0 B/s"
            Me.LabLaunchingDownload.Opacity = 0.0
            Me.LabLaunchingDownload.Visibility = Visibility.Collapsed
            Me.LabLaunchingDownloadLeft.Opacity = 0.0
            Me.LabLaunchingDownloadLeft.Visibility = Visibility.Collapsed
            Me.ProgressLaunchingFinished.Width = New GridLength(0.0, GridUnitType.Star)
            Me.ProgressLaunchingUnfinished.Width = New GridLength(1.0, GridUnitType.Star)
            Me.PanLaunchingHint.Opacity = 0.0
            Me.PanLaunchingHint.Visibility = Visibility.Collapsed
            Me.PanLaunchingInfo.Width = Double.NaN
            ModLaunch.McLaunchProcess = Nothing
            ModLaunch.McLaunchWatcher = Nothing
            Me.LabLaunchingHint.Text = PageOtherTest.GetRandomHint()
            Me.PanInput.IsHitTestVisible = False
            Me.PanLaunching.IsHitTestVisible = False
            Me.LoadLaunching.State.LoadingState = OurLoading.OurLoadingState.Run
            Me.PanLaunching.Visibility = Visibility.Visible
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.PanInput, 0.0, 50, 0, Nothing, False), ModAnimation.AaOpacity(Me.PanInput, -Me.PanInput.Opacity, 110, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), True), ModAnimation.AaScaleTransform(Me.PanInput, 1.2 - CType(Me.PanInput.RenderTransform, ScaleTransform).ScaleX, 160, 0, Nothing, False), ModAnimation.AaOpacity(Me.PanLaunching, 1.0 - Me.PanLaunching.Opacity, 150, 100, Nothing, False), ModAnimation.AaScaleTransform(Me.PanLaunching, 1.0 - CType(Me.PanLaunching.RenderTransform, ScaleTransform).ScaleX, 500, 100, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaCode(Sub()
                Me.PanLaunching.IsHitTestVisible = True
            End Sub, 150, False) }, "Launch State Page", False)
        End Sub

        Public Sub PageChangeToLogin()
            NewLateBinding.LateCall(Me.PageGet(Me.PageCurrent), Nothing, "Reload", New Object() { False }, New String() { "KeepInput" }, Nothing, Nothing, True)
            Me.PanInput.IsHitTestVisible = False
            Me.PanLaunching.IsHitTestVisible = False
            Me.LoadLaunching.State.LoadingState = OurLoading.OurLoadingState.[Stop]
            Me.PanInput.Visibility = Visibility.Visible
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.PanLaunching, -Me.PanLaunching.Opacity, 150, 0, Nothing, False), ModAnimation.AaScaleTransform(Me.PanLaunching, 0.8 - CType(Me.PanLaunching.RenderTransform, ScaleTransform).ScaleX, 150, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaOpacity(Me.PanInput, 1.0 - Me.PanInput.Opacity, 250, 50, Nothing, False), ModAnimation.AaScaleTransform(Me.PanInput, 1.0 - CType(Me.PanInput.RenderTransform, ScaleTransform).ScaleX, 300, 50, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaCode(Sub()
                Me.PanInput.IsHitTestVisible = True
            End Sub, 200, False) }, "Launch State Page", True)
        End Sub

        Private Function PageGet(Type As PageLaunchLeft.PageType) As Object
            Dim obj As Object
            Select Case Type
                Case PageLaunchLeft.PageType.Legacy
                    If Information.IsNothing(ModMain.FrmLoginLegacy) Then
                        ModMain.FrmLoginLegacy = New PageLoginLegacy()
                    End If
                    obj = ModMain.FrmLoginLegacy
                Case PageLaunchLeft.PageType.Nide
                    If Information.IsNothing(ModMain.FrmLoginNide) Then
                        ModMain.FrmLoginNide = New PageLoginNide()
                    End If
                    obj = ModMain.FrmLoginNide
                Case PageLaunchLeft.PageType.NideSkin
                    If Information.IsNothing(ModMain.FrmLoginNideSkin) Then
                        ModMain.FrmLoginNideSkin = New PageLoginNideSkin()
                    End If
                    obj = ModMain.FrmLoginNideSkin
                Case PageLaunchLeft.PageType.Auth
                    If Information.IsNothing(ModMain.FrmLoginAuth) Then
                        ModMain.FrmLoginAuth = New PageLoginAuth()
                    End If
                    obj = ModMain.FrmLoginAuth
                Case PageLaunchLeft.PageType.AuthSkin
                    If Information.IsNothing(ModMain.FrmLoginAuthSkin) Then
                        ModMain.FrmLoginAuthSkin = New PageLoginAuthSkin()
                    End If
                    obj = ModMain.FrmLoginAuthSkin
                Case PageLaunchLeft.PageType.Ms
                    If Information.IsNothing(ModMain.FrmLoginMs) Then
                        ModMain.FrmLoginMs = New PageLoginMs()
                    End If
                    obj = ModMain.FrmLoginMs
                Case PageLaunchLeft.PageType.MsSkin
                    If Information.IsNothing(ModMain.FrmLoginMsSkin) Then
                        ModMain.FrmLoginMsSkin = New PageLoginMsSkin()
                    End If
                    obj = ModMain.FrmLoginMsSkin
                Case Else
                    Throw New ArgumentOutOfRangeException("Type", "即将切换的登录分页编号越界")
            End Select
            Return obj
        End Function

        Private Function PageChange(Type As PageLaunchLeft.PageType, Anim As Boolean) As Object
            Dim PageNew As Object = ModMain.FrmLoginMs
            Dim obj As Object
            Try
                If Me.PageCurrent = Type Then
                    obj = PageNew
                Else
                    PageNew = RuntimeHelpers.GetObjectValue(Me.PageGet(Type))
                    ModAnimation.AniStop("FrmLogin PageChange")
                    If Not Information.IsNothing(RuntimeHelpers.GetObjectValue(PageNew)) AndAlso Not Information.IsNothing(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(PageNew, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing))) Then
                        Dim $VB$Local_PageNew As Object = PageNew
                        Dim type As Type = Nothing
                        Dim text As String = "SetValue"
                        Dim array As Object() = New Object(1) {}
                        array(0) = ContentPresenter.ContentProperty
                        NewLateBinding.LateCall($VB$Local_PageNew, type, text, array, Nothing, Nothing, Nothing, True)
                    End If
                    If Anim Then
                        Dim $I1 As ThreadStart
                        MyBase.Dispatcher.Invoke(Sub()
                            Dim array2 As ModAnimation.AniData() = New ModAnimation.AniData(2) {}
                            array2(0) = ModAnimation.AaOpacity(Me.PanLogin, -Me.PanLogin.Opacity, 100, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False)
                            Dim num As Integer = 1
                            Dim threadStart As ThreadStart
                            If $I1 IsNot Nothing Then
                                threadStart = $I1
                            Else
                                Dim threadStart2 As ThreadStart = Sub()
                                    ModAnimation.AniControlEnabled += 1
                                    Me.PanLogin.Children.Clear()
                                    Me.PanLogin.Children.Add(CType(PageNew, UIElement))
                                    ModAnimation.AniControlEnabled -= 1
                                End Sub
                                Dim threadStart3 As ThreadStart = threadStart2
                                $I1 = threadStart2
                                threadStart = threadStart3
                            End If
                            array2(num) = ModAnimation.AaCode(threadStart, 100, False)
                            array2(2) = ModAnimation.AaOpacity(Me.PanLogin, 1.0, 100, 120, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False)
                            ModAnimation.AniStart(array2, "FrmLogin PageChange", False)
                        End Sub, DispatcherPriority.Render)
                    Else
                        ModAnimation.AniControlEnabled += 1
                        Me.PanLogin.Children.Clear()
                        Me.PanLogin.Children.Add(CType(PageNew, UIElement))
                        ModAnimation.AniControlEnabled -= 1
                    End If
                    Me.PageCurrent = Type
                    obj = PageNew
                End If
            Catch ex As Exception
                Logger.[Error](ex, String.Format("切换登录分页失败（{0}）", Type), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                obj = PageNew
            End Try
            Return obj
        End Function

        Public Sub RefreshPage(KeepInput As Boolean, Anim As Boolean)
            Dim num As Integer
            If ModMinecraft.McInstanceSelected IsNot Nothing Then
                num = Settings.[Get](Of Integer)("VersionServerLogin", ModMinecraft.McInstanceSelected)
                Settings.[Set]("LoginPageType", num, Nothing)
            Else
                num = Settings.[Get](Of Integer)("LoginPageType", Nothing)
            End If
            Dim pageType As PageLaunchLeft.PageType
            Select Case num
                Case 0
                Case 1
                    If Operators.CompareString(Settings.[Get](Of String)("CacheMsV2Access", Nothing), "", False) = 0 Then
                        pageType = PageLaunchLeft.PageType.Ms
                    Else
                        pageType = PageLaunchLeft.PageType.MsSkin
                    End If
                    Settings.[Set]("LoginType", ModLaunch.McLoginType.Ms, Nothing)
                    Me.PanType.Visibility = Visibility.Collapsed
                    Me.PanTypeOne.Visibility = Visibility.Visible
                    Me.PathTypeOne.Data = CType(New GeometryConverter().ConvertFromString("M511.488256 95.184408c35.310345 22.516742 95.184408 55.78011 167.34033 84.437781 75.738131 29.681159 148.405797 40.93953 191.392304 45.033483v353.615193c0 73.691154-50.662669 164.781609-136.123938 244.101949C649.65917 901.181409 558.568716 942.12094 512 942.12094c-46.568716 0-137.65917-40.93953-222.096952-119.748126C204.441779 742.54073 153.77911 651.450275 153.77911 577.247376v-353.103448c42.474763-4.093953 116.165917-15.352324 191.904048-45.545227 75.226387-30.192904 133.565217-63.456272 165.805098-83.414293M512 0c-4.093953 0-8.187906 1.535232-11.258371 3.582209l-14.84058 10.234882c-1.023488 0.511744-67.550225 47.592204-170.410794 88.531735-100.813593 39.916042-198.556722 41.963018-199.58021 41.963018l-25.075462 0.511744c-10.746627 0.511744-18.934533 8.187906-18.934533 18.422789v414.000999c0 216.97951 286.064968 446.24088 440.09995 446.24088s440.09995-229.261369 440.09995-445.729136V163.758121c0-10.234883-8.69965-18.422789-18.934533-18.422789l-24.563718-0.511744c-1.023488 0-98.766617-2.046977-199.58021-41.963018-103.372314-40.93953-170.410795-88.01999-170.922538-88.531734L523.258371 3.582209c-3.070465-2.558721-7.164418-3.582209-11.258371-3.582209z M743.308346 410.930535l-260.477761 260.477761c-15.864068 15.864068-41.963018 15.864068-57.827087 0l-144.823588-144.823588c-15.864068-15.864068-15.864068-41.963018 0-57.827087 8.187906-8.187906 18.422789-11.770115 29.169415-11.770115 10.234883 0 20.981509 4.093953 29.169416 11.770115l115.654173 115.654173L685.993003 352.591704c15.864068-15.864068 41.963018-15.864068 57.827087 0 15.352324 16.375812 15.352324 42.474763-0.511744 58.338831z"), Geometry)
                    Me.LabTypeOne.Text = "正版登录"
                    Me.RadioLoginType5.Visibility = Visibility.Visible
                    Me.RadioLoginType0.Visibility = Visibility.Collapsed
                    GoTo IL_0326
                Case 2
                    pageType = PageLaunchLeft.PageType.Legacy
                    Settings.[Set]("LoginType", ModLaunch.McLoginType.Legacy, Nothing)
                    Me.PanType.Visibility = Visibility.Collapsed
                    Me.PanTypeOne.Visibility = Visibility.Visible
                    Me.PathTypeOne.Data = CType(New GeometryConverter().ConvertFromString("M533.293176 788.841412a60.235294 60.235294 0 1 1 85.202824 85.202823l-42.616471 42.586353c-129.355294 129.385412-339.124706 129.385412-468.510117 0-129.385412-129.385412-129.385412-339.124706 0-468.510117l42.586353-42.616471a60.235294 60.235294 0 1 1 85.202823 85.202824l-42.61647 42.586352a210.823529 210.823529 0 1 0 298.164706 298.164706l42.586352-42.61647z m255.548236-255.548236l42.61647-42.586352a210.823529 210.823529 0 1 0-298.164706-298.164706l-42.586352 42.61647a60.235294 60.235294 0 1 1-85.202824-85.202823l42.616471-42.586353c129.355294-129.385412 339.124706-129.385412 468.510117 0 129.385412 129.385412 129.385412 339.124706 0 468.510117l-42.586353 42.616471a60.235294 60.235294 0 1 1-85.202823-85.202824zM192.542118 192.542118a60.235294 60.235294 0 0 1 85.202823 0l553.712941 553.712941a60.235294 60.235294 0 0 1-85.202823 85.202823L192.542118 277.744941a60.235294 60.235294 0 0 1 0-85.202823z"), Geometry)
                    Me.LabTypeOne.Text = "离线登录"
                    GoTo IL_0326
                Case 3
                    If Operators.CompareString(Settings.[Get](Of String)("CacheNideAccess", Nothing), "", False) = 0 Then
                        pageType = PageLaunchLeft.PageType.Nide
                    Else
                        pageType = PageLaunchLeft.PageType.NideSkin
                    End If
                    Settings.[Set]("LoginType", ModLaunch.McLoginType.Nide, Nothing)
                    Me.PanType.Visibility = Visibility.Collapsed
                    Me.PanTypeOne.Visibility = Visibility.Visible
                    Me.PathTypeOne.Data = CType(New GeometryConverter().ConvertFromString("M834.5 684.1c-31.2-70.4-98.9-120.9-179.1-127.3 63.5-8.5 112.6-63 112.6-128.8 0-71.8-58.2-130-130-130s-130 58.2-130 130c0 65.9 49 120.3 112.6 128.8-80.2 6.4-148 57-179.1 127.3-8.7 19.7 6 42 27.6 42 12.1 0 22.7-7.5 27.7-18.5 24.3-53.9 78.5-91.5 141.3-91.5s117 37.6 141.3 91.5c5 11.1 15.6 18.5 27.7 18.5 21.4 0 36.1-22.3 27.4-42zM567.9 427.9c0-38.6 31.4-70 70-70s70 31.4 70 70-31.4 70-70 70-70-31.4-70-70zM460.3 347.9H216.9c-16.6 0-30 13.4-30 30s13.4 30 30 30h243.3c16.6 0 30-13.4 30-30 0.1-16.5-13.4-30-29.9-30zM367.4 459.6H216.9c-16.6 0-30 13.4-30 30s13.4 30 30 30h150.4c16.6 0 30-13.4 30-30 0.1-16.6-13.4-30-29.9-30zM297.4 571.2H217c-16.6 0-30 13.4-30 30s13.4 30 30 30h80.4c16.6 0 30-13.4 30-30 0-16.5-13.5-30-30-30zM900 236v552H124V236h776m0-60H124c-33.1 0-60 26.9-60 60v552c0 33.1 26.9 60 60 60h776c33.1 0 60-26.9 60-60V236c0-33.1-26.9-60-60-60z"), Geometry)
                    Me.LabTypeOne.Text = "统一通行证登录"
                    GoTo IL_0326
                Case 4
                    If Operators.CompareString(Settings.[Get](Of String)("CacheAuthAccess", Nothing), "", False) = 0 Then
                        pageType = PageLaunchLeft.PageType.Auth
                    Else
                        pageType = PageLaunchLeft.PageType.AuthSkin
                    End If
                    Settings.[Set]("LoginType", ModLaunch.McLoginType.Auth, Nothing)
                    Me.PanType.Visibility = Visibility.Collapsed
                    Me.PanTypeOne.Visibility = Visibility.Visible
                    Me.PathTypeOne.Data = CType(New GeometryConverter().ConvertFromString("M834.5 684.1c-31.2-70.4-98.9-120.9-179.1-127.3 63.5-8.5 112.6-63 112.6-128.8 0-71.8-58.2-130-130-130s-130 58.2-130 130c0 65.9 49 120.3 112.6 128.8-80.2 6.4-148 57-179.1 127.3-8.7 19.7 6 42 27.6 42 12.1 0 22.7-7.5 27.7-18.5 24.3-53.9 78.5-91.5 141.3-91.5s117 37.6 141.3 91.5c5 11.1 15.6 18.5 27.7 18.5 21.4 0 36.1-22.3 27.4-42zM567.9 427.9c0-38.6 31.4-70 70-70s70 31.4 70 70-31.4 70-70 70-70-31.4-70-70zM460.3 347.9H216.9c-16.6 0-30 13.4-30 30s13.4 30 30 30h243.3c16.6 0 30-13.4 30-30 0.1-16.5-13.4-30-29.9-30zM367.4 459.6H216.9c-16.6 0-30 13.4-30 30s13.4 30 30 30h150.4c16.6 0 30-13.4 30-30 0.1-16.6-13.4-30-29.9-30zM297.4 571.2H217c-16.6 0-30 13.4-30 30s13.4 30 30 30h80.4c16.6 0 30-13.4 30-30 0-16.5-13.5-30-30-30zM900 236v552H124V236h776m0-60H124c-33.1 0-60 26.9-60 60v552c0 33.1 26.9 60 60 60h776c33.1 0 60-26.9 60-60V236c0-33.1-26.9-60-60-60z"), Geometry)
                    Me.LabTypeOne.Text = If((ModMinecraft.McInstanceSelected Is Nothing), Settings.[Get](Of String)("CacheAuthServerName", Nothing), Settings.[Get](Of String)("VersionServerAuthName", ModMinecraft.McInstanceSelected))
                    If Operators.CompareString(Me.LabTypeOne.Text, "", False) = 0 Then
                        Me.LabTypeOne.Text = "第三方登录"
                        GoTo IL_0326
                    End If
                    GoTo IL_0326
                Case Else
                    Logger.[Error](String.Format("未知的登录页面：{0}", num), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
            End Select
            If Me.RadioLoginType5.Checked Then
                If Operators.CompareString(Settings.[Get](Of String)("CacheMsV2Access", Nothing), "", False) = 0 Then
                    pageType = PageLaunchLeft.PageType.Ms
                Else
                    pageType = PageLaunchLeft.PageType.MsSkin
                End If
                Settings.[Set]("LoginType", ModLaunch.McLoginType.Ms, Nothing)
            Else
                pageType = PageLaunchLeft.PageType.Legacy
                Settings.[Set]("LoginType", ModLaunch.McLoginType.Legacy, Nothing)
            End If
            Me.PanType.Visibility = Visibility.Visible
            Me.PanTypeOne.Visibility = Visibility.Collapsed
            Me.RadioLoginType5.Visibility = Visibility.Visible
            Me.RadioLoginType0.Visibility = Visibility.Visible
            IL_0326:
            If Me.PageCurrent <> pageType Then
                Dim obj As Object = Me.PageChange(pageType, Anim)
                Dim type As Type = Nothing
                Dim text As String = "Reload"
                Dim array As Object() = New Object() { KeepInput }
                Dim array2 As Object() = array
                Dim array3 As String() = Nothing
                Dim array4 As Type() = Nothing
                Dim array5 As Boolean() = New Boolean() { True }
                Dim array6 As Boolean() = array5
                NewLateBinding.LateCall(obj, type, text, array, array3, array4, array5, True)
                If array6(0) Then
                    KeepInput = CBool(Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2(0)), GetType(Boolean)))
                End If
                Dim myRadioButton As OurRadioButton = CType(MyBase.FindName("RadioLoginType" + Conversions.ToString(CInt(Settings.[Get](Of ModLaunch.McLoginType)("LoginType", Nothing)))), OurRadioButton)
                If myRadioButton IsNot Nothing Then
                    myRadioButton.Checked = True
                End If
            End If
        End Sub

        Private Sub RadioLoginType_Change(sender As Object, raiseByMouse As Boolean)
            If raiseByMouse Then
                Me.RefreshPage(True, True)
            End If
        End Sub

        Private Shared Function SkinMsInput() As ValueTuple(Of String, String)
            Return New ValueTuple(Of String, String)(Settings.[Get](Of String)("CacheMsV2Name", Nothing), Settings.[Get](Of String)("CacheMsV2Uuid", Nothing))
        End Function

        Private Shared Sub SkinMsLoad(Data As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String))
            Dim action As Action
            If PageLaunchLeft._Closure$__.$I15-0 IsNot Nothing Then
                action = PageLaunchLeft._Closure$__.$I15-0
            Else
                Dim action2 As Action = Sub()
                    If ModMain.FrmLoginMsSkin IsNot Nothing AndAlso ModMain.FrmLoginMsSkin.Skin IsNot Nothing Then
                        ModMain.FrmLoginMsSkin.Skin.Clear()
                    End If
                End Sub
                action = action2
                PageLaunchLeft._Closure$__.$I15-0 = action2
            End If
            ModBase.RunInUi(action, False)
            Dim item As String = Data.Input.Item1
            Dim item2 As String = Data.Input.Item2
            If Operators.CompareString(item, "", False) = 0 Then
                Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(item))) + ".png"
                Logger.Info("获取微软正版皮肤失败，ID 为空", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
            Else
                Try
                    Dim text As String = ModMinecraft.McSkinGetAddress(item2, "Ms")
                    If Data.IsCanceled Then
                        Throw New OperationCanceledException("当前任务已取消：" + item)
                    End If
                    text = ModMinecraft.McSkinDownload(text)
                    If Data.IsCanceled Then
                        Throw New OperationCanceledException("当前任务已取消：" + item)
                    End If
                    Data.Output = text
                Catch ex As Exception
                    If ex.IsCanceled() Then
                        Data.Output = ""
                        Return
                    End If
                    If ex.GetDisplay(False).Contains("(429)") Then
                        Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(item))) + ".png"
                        Logger.[Error](String.Format("获取正版皮肤失败（{0}）：获取皮肤太过频繁，请 5 分钟后再试！", item), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                    ElseIf ex.GetDisplay(False).Contains("未设置自定义皮肤") Then
                        Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(item))) + ".png"
                        Logger.Info("用户未设置自定义皮肤，跳过皮肤加载", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                    Else
                        Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(item))) + ".png"
                        Logger.[Error](ex, String.Format("获取微软正版皮肤失败（{0}）", item), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                    End If
                End Try
            End If
            If ModMain.FrmLoginMsSkin IsNot Nothing Then
                ModBase.RunInUi(AddressOf ModMain.FrmLoginMsSkin.Skin.Load, False)
                Return
            End If
            If Not Data.IsCanceled Then
                Data.Input = Nothing
            End If
        End Sub

        Private Shared Function SkinLegacyInput() As ValueTuple(Of String, String)
            Dim num As Integer = Settings.[Get](Of Integer)("LaunchSkinType", Nothing)
            Dim valueTuple As ValueTuple(Of String, String)
            If num <> 0 Then
                If num <> 3 Then
                    valueTuple = New ValueTuple(Of String, String)(num.ToString(), "")
                Else
                    valueTuple = New ValueTuple(Of String, String)("3", Settings.[Get](Of String)("LaunchSkinID", Nothing))
                End If
            Else
                Dim frmLoginLegacy As PageLoginLegacy = ModMain.FrmLoginLegacy
                If frmLoginLegacy IsNot Nothing AndAlso frmLoginLegacy.IsReloaded Then
                    valueTuple = New ValueTuple(Of String, String)("0", If(ModMain.FrmLoginLegacy.ComboName.Text.Trim(), ""))
                ElseIf Operators.CompareString(Settings.[Get](Of String)("LoginLegacyName", Nothing), "", False) = 0 Then
                    valueTuple = New ValueTuple(Of String, String)("0", "")
                Else
                    valueTuple = New ValueTuple(Of String, String)("0", If(Settings.[Get](Of String)("LoginLegacyName", Nothing).ToString().BeforeFirst("¨", False), ""))
                End If
            End If
            Return valueTuple
        End Function

        Private Shared Sub SkinLegacyLoad(Data As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String))
            Dim action As Action
            If PageLaunchLeft._Closure$__.$I18-0 IsNot Nothing Then
                action = PageLaunchLeft._Closure$__.$I18-0
            Else
                Dim action2 As Action = Sub()
                    If ModMain.FrmLoginLegacy IsNot Nothing AndAlso ModMain.FrmLoginLegacy.Skin IsNot Nothing Then
                        ModMain.FrmLoginLegacy.Skin.Clear()
                    End If
                End Sub
                action = action2
                PageLaunchLeft._Closure$__.$I18-0 = action2
            End If
            ModBase.RunInUi(action, False)
            Select Case Conversions.ToInteger(Data.Input.Item1)
                Case 0
                    Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(Data.Input.Item2))) + ".png"
                    GoTo IL_0278
                Case 1
                Case 2
                    Data.Output = ModBase.PathImage + "Skins/Alex.png"
                    GoTo IL_0278
                Case 3
                    Dim item As String = Data.Input.Item2
                    Try
                        If item.Count() < 2 Then
                            Data.Output = ModBase.PathImage + "Skins/Steve.png"
                        Else
                            Dim text As String = Conversions.ToString(ModLaunch.McLoginMojangUuid(item, True))
                            If Data.IsCanceled Then
                                Throw New OperationCanceledException("当前任务已取消：" + item)
                            End If
                            text = ModMinecraft.McSkinGetAddress(text, "Mojang")
                            If Data.IsCanceled Then
                                Throw New OperationCanceledException("当前任务已取消：" + item)
                            End If
                            text = ModMinecraft.McSkinDownload(text)
                            If Data.IsCanceled Then
                                Throw New OperationCanceledException("当前任务已取消：" + item)
                            End If
                            Data.Output = text
                        End If
                        GoTo IL_0278
                    Catch ex As Exception
                        If ex.IsCanceled() Then
                            Data.Output = ""
                            Return
                        End If
                        If ex.GetDisplay(False).Contains("(429)") Then
                            Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(item))) + ".png"
                            Logger.Info(String.Format("获取离线登录使用的正版皮肤失败（{0}）：获取皮肤太过频繁，请 5 分钟后再试！", item), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                        Else
                            Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(item))) + ".png"
                            Logger.Warn(ex, String.Format("获取离线登录使用的正版皮肤失败（{0}）", item), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                        End If
                        GoTo IL_0278
                    End Try
                    GoTo IL_022B
                Case 4
                    GoTo IL_022B
                Case Else
                    GoTo IL_0278
            End Select
            IL_0093:
            Data.Output = ModBase.PathImage + "Skins/Steve.png"
            GoTo IL_0278
            IL_022B:
            If Not FileUtils.Exists(Paths.AppDataThenName + "CustomSkin.png") Then
                ModMain.Hint("未找到离线皮肤自定义文件，可能它已被删除。PCL 将使用默认的 Steve 皮肤！", ModMain.HintType.Blue, True)
                Settings.[Set]("LaunchSkinType", 1, Nothing)
                GoTo IL_0093
            End If
            Data.Output = Paths.AppDataThenName + "CustomSkin.png"
            IL_0278:
            If ModMain.FrmLoginLegacy IsNot Nothing Then
                ModBase.RunInUi(AddressOf ModMain.FrmLoginLegacy.Skin.Load, False)
                Return
            End If
            If Not Data.IsCanceled Then
                Data.Input = Nothing
            End If
        End Sub

        Private Shared Function SkinNideInput() As ValueTuple(Of String, String)
            Return New ValueTuple(Of String, String)(Settings.[Get](Of String)("CacheNideName", Nothing), Settings.[Get](Of String)("CacheNideUuid", Nothing))
        End Function

        Private Shared Sub SkinNideLoad(Data As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String))
            Dim action As Action
            If PageLaunchLeft._Closure$__.$I21-0 IsNot Nothing Then
                action = PageLaunchLeft._Closure$__.$I21-0
            Else
                Dim action2 As Action = Sub()
                    If ModMain.FrmLoginNideSkin IsNot Nothing AndAlso ModMain.FrmLoginNideSkin.Skin IsNot Nothing Then
                        ModMain.FrmLoginNideSkin.Skin.Clear()
                    End If
                End Sub
                action = action2
                PageLaunchLeft._Closure$__.$I21-0 = action2
            End If
            ModBase.RunInUi(action, False)
            Dim item As String = Data.Input.Item1
            Dim item2 As String = Data.Input.Item2
            If Operators.CompareString(item, "", False) = 0 Then
                Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(item))) + ".png"
                Logger.Info("获取统一通行证皮肤失败，ID 为空", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
            Else
                Try
                    Dim text As String = ModMinecraft.McSkinGetAddress(item2, "Nide")
                    If Data.IsCanceled Then
                        Throw New OperationCanceledException("当前任务已取消：" + item)
                    End If
                    text = ModMinecraft.McSkinDownload(text)
                    If Data.IsCanceled Then
                        Throw New OperationCanceledException("当前任务已取消：" + item)
                    End If
                    Data.Output = text
                Catch ex As Exception
                    If ex.IsCanceled() Then
                        Data.Output = ""
                        Return
                    End If
                    If ex.GetDisplay(False).Contains("(429)") Then
                        Data.Output = ModBase.PathImage + "Skins/Steve.png"
                        Logger.[Error](String.Format("获取统一通行证皮肤失败（{0}）：获取皮肤太过频繁，请 5 分钟后再试！", item), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                    ElseIf ex.GetDisplay(False).Contains("未设置自定义皮肤") Then
                        Data.Output = ModBase.PathImage + "Skins/Steve.png"
                        Logger.Info("用户未设置自定义皮肤，跳过皮肤加载", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                    Else
                        Data.Output = ModBase.PathImage + "Skins/Steve.png"
                        Logger.[Error](ex, String.Format("获取统一通行证皮肤失败（{0}）", item), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                    End If
                End Try
            End If
            If ModMain.FrmLoginNideSkin IsNot Nothing Then
                ModBase.RunInUi(AddressOf ModMain.FrmLoginNideSkin.Skin.Load, False)
                Return
            End If
            If Not Data.IsCanceled Then
                Data.Input = Nothing
            End If
        End Sub

        Private Shared Function SkinAuthInput() As ValueTuple(Of String, String)
            Return New ValueTuple(Of String, String)(Settings.[Get](Of String)("CacheAuthName", Nothing), Settings.[Get](Of String)("CacheAuthUuid", Nothing))
        End Function

        Private Shared Sub SkinAuthLoad(Data As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String))
            Dim action As Action
            If PageLaunchLeft._Closure$__.$I24-0 IsNot Nothing Then
                action = PageLaunchLeft._Closure$__.$I24-0
            Else
                Dim action2 As Action = Sub()
                    If ModMain.FrmLoginAuthSkin IsNot Nothing AndAlso ModMain.FrmLoginAuthSkin.Skin IsNot Nothing Then
                        ModMain.FrmLoginAuthSkin.Skin.Clear()
                    End If
                End Sub
                action = action2
                PageLaunchLeft._Closure$__.$I24-0 = action2
            End If
            ModBase.RunInUi(action, False)
            Dim item As String = Data.Input.Item1
            Dim item2 As String = Data.Input.Item2
            If Operators.CompareString(item, "", False) = 0 Then
                Data.Output = ModBase.PathImage + "Skins/Steve.png"
                Logger.Info("获取 Authlib-Injector 皮肤失败，ID 为空", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
            Else
                Try
                    Dim text As String = ModMinecraft.McSkinGetAddress(item2, "Auth")
                    If Data.IsCanceled Then
                        Throw New OperationCanceledException("当前任务已取消：" + item)
                    End If
                    text = ModMinecraft.McSkinDownload(text)
                    If Data.IsCanceled Then
                        Throw New OperationCanceledException("当前任务已取消：" + item)
                    End If
                    Data.Output = text
                Catch ex As Exception
                    If ex.IsCanceled() Then
                        Data.Output = ""
                        Return
                    End If
                    If ex.GetDisplay(False).Contains("(429)") Then
                        Data.Output = ModBase.PathImage + "Skins/Steve.png"
                        Logger.[Error](String.Format("获取 Authlib-Injector 皮肤失败（{0}）：获取皮肤太过频繁，请 5 分钟后再试！", item), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                    ElseIf ex.GetDisplay(False).Contains("未设置自定义皮肤") Then
                        Data.Output = ModBase.PathImage + "Skins/Steve.png"
                        Logger.Info("用户未设置自定义皮肤，跳过皮肤加载", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                    Else
                        Data.Output = ModBase.PathImage + "Skins/Steve.png"
                        Logger.[Error](ex, String.Format("获取 Authlib-Injector 皮肤失败（{0}）", item), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                    End If
                End Try
            End If
            If ModMain.FrmLoginAuthSkin IsNot Nothing Then
                ModBase.RunInUi(AddressOf ModMain.FrmLoginAuthSkin.Skin.Load, False)
                Return
            End If
            If Not Data.IsCanceled Then
                Data.Input = Nothing
            End If
        End Sub

        Private Sub BtnVersion_Click()
            If ModLaunch.McLaunchLoader.State <> ModBase.LoadState.Loading Then
                ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSelect, FormMain.PageSubType.[Default])
            End If
        End Sub

        Public Sub LaunchButtonClick()
            If ModLaunch.McLaunchLoader.State <> ModBase.LoadState.Loading AndAlso Me.BtnLaunch.IsEnabled AndAlso (ModMain.FrmMain.PageRight Is Nothing OrElse ModMain.FrmMain.PageRight.PageState = OurPageRight.PageStates.ContentStay OrElse ModMain.FrmMain.PageRight.PageState = OurPageRight.PageStates.ContentEnter) Then
                If ModMain.IsAprilEnabled AndAlso Not ModMain.IsAprilGiveup Then
                    ModSecret.ThemeUnlock(12, False, "隐藏主题 滑稽彩 已解锁！")
                    ModMain.IsAprilGiveup = True
                    Settings.[Set]("AprilYear", DateTime.Now.Year, Nothing)
                    ModMain.FrmLaunchLeft.AprilScaleTrans.ScaleX = 1.0
                    ModMain.FrmLaunchLeft.AprilScaleTrans.ScaleY = 1.0
                    ModMain.FrmLaunchLeft.AprilPosTrans.X = 0.0
                    ModMain.FrmLaunchLeft.AprilPosTrans.Y = 0.0
                    ModMain.FrmMain.BtnExtraApril.ShowRefresh()
                End If
                If Operators.CompareString(Me.BtnLaunch.Text, "启动游戏", False) = 0 Then
                    ModLaunch.McLaunchStart(Nothing)
                    Return
                End If
                If Operators.CompareString(Me.BtnLaunch.Text, "下载游戏", False) = 0 Then
                    ModMain.FrmMain.PageChange(FormMain.PageType.Download, FormMain.PageSubType.[Default])
                End If
            End If
        End Sub

        Public Sub RefreshButtonsUI()
            If Me.BtnLaunch.IsLoaded Then
                Dim num As Integer
                If Not Me.IsLoadFinished OrElse ModMinecraft.McInstanceListLoader.State = ModBase.LoadState.Loading OrElse ModMinecraft.McFolderListLoader.State = ModBase.LoadState.Loading Then
                    num = 0
                ElseIf ModMinecraft.McInstanceSelected Is Nothing Then
                    If Settings.[Get](Of Boolean)("UiHiddenPageDownload", Nothing) AndAlso Not PageSetupUI.HiddenForceShow Then
                        num = 1
                    Else
                        num = 2
                    End If
                Else
                    num = 3
                End If
                If num <> Me.BtnLaunchState OrElse Operators.CompareString(If((ModMinecraft.McInstanceSelected Is Nothing), "", ModMinecraft.McInstanceSelected.PathVersion), If((Me.BtnLaunchInstance Is Nothing), "", Me.BtnLaunchInstance.PathVersion), False) <> 0 Then
                    Me.BtnLaunchInstance = ModMinecraft.McInstanceSelected
                    Me.BtnLaunchState = num
                    Select Case num
                        Case 0
                            Logger.Info("启动按钮：正在加载 Minecraft 版本", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                            ModMain.FrmLaunchLeft.BtnLaunch.Text = "正在加载"
                            ModMain.FrmLaunchLeft.BtnLaunch.IsEnabled = False
                            ModMain.FrmLaunchLeft.LabVersion.Text = "正在加载中，请稍候"
                            ModMain.FrmLaunchLeft.BtnVersion.IsEnabled = False
                            ModMain.FrmLaunchLeft.BtnMore.Visibility = Visibility.Collapsed
                        Case 1
                            Logger.Info("启动按钮：无 Minecraft 版本，下载已禁用", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                            ModMain.FrmLaunchLeft.BtnLaunch.Text = "启动游戏"
                            ModMain.FrmLaunchLeft.BtnLaunch.IsEnabled = False
                            ModMain.FrmLaunchLeft.LabVersion.Text = "未找到可用的游戏版本"
                            ModMain.FrmLaunchLeft.BtnVersion.IsEnabled = True
                            ModMain.FrmLaunchLeft.BtnMore.Visibility = Visibility.Collapsed
                        Case 2
                            Logger.Info("启动按钮：无 Minecraft 版本，要求下载", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                            ModMain.FrmLaunchLeft.BtnLaunch.Text = "下载游戏"
                            ModMain.FrmLaunchLeft.BtnLaunch.IsEnabled = True
                            ModMain.FrmLaunchLeft.LabVersion.Text = "未找到可用的游戏版本"
                            ModMain.FrmLaunchLeft.BtnVersion.IsEnabled = True
                            ModMain.FrmLaunchLeft.BtnMore.Visibility = Visibility.Collapsed
                        Case 3
                            Logger.Info(String.Format("启动按钮：Minecraft 版本：{0}", ModMinecraft.McInstanceSelected.PathVersion), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                            ModMain.FrmLaunchLeft.BtnLaunch.Text = "启动游戏"
                            ModMain.FrmLaunchLeft.BtnVersion.IsEnabled = True
                            ModMain.FrmLaunchLeft.BtnLaunch.IsEnabled = True
                            ModMain.FrmLaunchLeft.LabVersion.Text = ModMinecraft.McInstanceSelected.Name
                    End Select
                End If
                ModMain.FrmLaunchLeft.BtnVersion.Visibility = If((Not PageSetupUI.HiddenForceShow AndAlso Settings.[Get](Of Boolean)("UiHiddenFunctionSelect", Nothing)), Visibility.Collapsed, Visibility.Visible)
                If num = 3 Then
                    ModMain.FrmLaunchLeft.BtnMore.Visibility = ModMain.FrmLaunchLeft.BtnVersion.Visibility
                End If
            End If
        End Sub

        Public Sub BtnCancel_Click()
            If ModLaunch.McLaunchLoaderReal IsNot Nothing Then
                ModLaunch.McLaunchLoaderReal.Cancel()
                ModLaunch.McLaunchLog("已取消启动", LogLevel.Info)
                Try
                    If ModLaunch.McLaunchWatcher IsNot Nothing Then
                        ModLaunch.McLaunchWatcher.Kill()
                    ElseIf ModLaunch.McLaunchProcess IsNot Nothing AndAlso Not ModLaunch.McLaunchProcess.HasExited Then
                        ModLaunch.McLaunchProcess.Kill()
                    End If
                Catch ex As Exception
                    Logger.[Error](ex, "取消启动结束进程失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                End Try
            End If
        End Sub

        Private Sub BtnMore_Click()
            If ModLaunch.McLaunchLoader.State <> ModBase.LoadState.Loading Then
                If ModMinecraft.McInstanceLock.IsLocked(ModMinecraft.McInstanceSelected) Then
                    ModMain.Hint("该版本正在被修改，暂时无法进入版本设置！", ModMain.HintType.Red, True)
                    Return
                End If
                ModMinecraft.McInstanceSelected.Load()
                PageInstanceLeft.Instance = ModMinecraft.McInstanceSelected
                ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSetup, FormMain.PageSubType.[Default])
            End If
        End Sub

        Public Sub LaunchingRefresh()
            Try
                Dim CS$<>8__locals1 As PageLaunchLeft._Closure$__33-0 = New PageLaunchLeft._Closure$__33-0(CS$<>8__locals1)
                CS$<>8__locals1.$VB$Me = Me
                If ModLaunch.McLaunchLoaderReal.State <> ModBase.LoadState.Canceled Then
                    Dim flag As Boolean = False
                    Try
                        Try
                            For Each loaderBase As ModLoader.LoaderBase In ModLaunch.McLaunchLoaderReal.GetLoaderList(False)
                                If loaderBase.State = ModBase.LoadState.Loading OrElse loaderBase.State = ModBase.LoadState.Waiting Then
                                    Me.LabLaunchingStage.Text = loaderBase.Name
                                    flag = Operators.CompareString(loaderBase.Name, "等待游戏窗口出现", False) = 0 OrElse Operators.CompareString(loaderBase.Name, "结束处理", False) = 0
                                    GoTo IL_00E2
                                End If
                            Next
                        Finally
                            Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                            CType(enumerator, IDisposable).Dispose()
                        End Try
                        Me.LabLaunchingStage.Text = "已完成"
                    Catch ex As Exception
                        Logger.Warn(ex, "获取是否启动完成失败，可能是由于启动状态改变导致集合已修改", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                        Return
                    End Try
                    IL_00E2:
                    Me.LabLaunchingTitle.Text = If((flag OrElse ModLaunch.McLaunchLoaderReal.State = ModBase.LoadState.Finished), "已启动游戏", If((ModLaunch.CurrentLaunchOptions.SaveBatch Is Nothing), "正在启动游戏", "正在导出启动脚本"))
                    If ModAnimation.AniIsRun("Launch State Page") Then
                        flag = False
                    End If
                    Dim progress As Double = ModLaunch.McLaunchLoaderReal.Progress
                    If progress >= Me.ShowProgress Then
                        Dim ptr As __ByRef(Of Double) = Me.ShowProgress
                        Me.ShowProgress = ptr + ((progress - Me.ShowProgress) * 0.1 + 0.0025)
                    End If
                    If progress <= Me.ShowProgress Then
                        Me.ShowProgress = progress
                    End If
                    If flag Then
                        Me.ShowProgress = 1.0
                    End If
                    Me.LabLaunchingProgress.Text = (Me.ShowProgress * 100.0).ToString("0.00") + " %"
                    CS$<>8__locals1.$VB$Local_HasLaunchDownloader = False
                    Try
                        If ModNet.NetManager.Speed = 0L AndAlso Me.LabLaunchingDownload.Visibility = Visibility.Collapsed Then
                            CS$<>8__locals1.$VB$Local_HasLaunchDownloader = False
                        Else
                            Try
                                For Each loaderDownload As ModNet.LoaderDownload In ModNet.NetManager.Tasks
                                    If loaderDownload.RealParent IsNot Nothing AndAlso Operators.CompareString(loaderDownload.RealParent.Name, "Minecraft 启动", False) = 0 AndAlso loaderDownload.State = ModBase.LoadState.Loading Then
                                        CS$<>8__locals1.$VB$Local_HasLaunchDownloader = True
                                    End If
                                Next
                            Finally
                                Dim enumerator2 As IEnumerator(Of ModNet.LoaderDownload)
                                If enumerator2 IsNot Nothing Then
                                    enumerator2.Dispose()
                                End If
                            End Try
                        End If
                    Catch ex2 As Exception
                        Logger.Warn(ex2, "获取 Minecraft 启动下载器失败，可能是因为启动被取消", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
                        CS$<>8__locals1.$VB$Local_HasLaunchDownloader = False
                    End Try
                    Me.LabLaunchingDownload.Text = StringUtils.FormatByteSize(ModNet.NetManager.Speed) + "/s"
                    Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)() From { ModAnimation.AaGridLengthWidth(Me.ProgressLaunchingFinished, Me.ShowProgress - Me.ProgressLaunchingFinished.Width.Value, 130, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaGridLengthWidth(Me.ProgressLaunchingUnfinished, 1.0 - Me.ShowProgress - Me.ProgressLaunchingUnfinished.Width.Value, 130, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) }
                    If CS$<>8__locals1.$VB$Local_HasLaunchDownloader = (Me.LabLaunchingDownload.Visibility = Visibility.Collapsed) Then
                        Me.LabLaunchingDownload.Visibility = Visibility.Visible
                        Me.LabLaunchingDownloadLeft.Visibility = Visibility.Visible
                        list.AddRange(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.LabLaunchingDownload, (CS$<>8__locals1.$VB$Local_HasLaunchDownloader > False) - Me.LabLaunchingDownload.Opacity, 90, 0, Nothing, False), ModAnimation.AaOpacity(Me.LabLaunchingDownloadLeft, If(CS$<>8__locals1.$VB$Local_HasLaunchDownloader, 0.5, 0.0) - Me.LabLaunchingDownloadLeft.Opacity, 90, 0, Nothing, False), ModAnimation.AaCode(Sub()
                            If Not CS$<>8__locals1.$VB$Local_HasLaunchDownloader Then
                                CS$<>8__locals1.$VB$Me.LabLaunchingDownload.Visibility = Visibility.Collapsed
                                CS$<>8__locals1.$VB$Me.LabLaunchingDownloadLeft.Visibility = Visibility.Collapsed
                            End If
                        End Sub, 110, False) })
                    End If
                    If Not flag = (Me.LabLaunchingProgress.Visibility = Visibility.Collapsed) Then
                        Me.LabLaunchingProgress.Visibility = Visibility.Visible
                        Me.LabLaunchingProgressLeft.Visibility = Visibility.Visible
                        If flag Then
                            Me.PanLaunchingHint.Visibility = Visibility.Visible
                        End If
                        list.AddRange(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.LabLaunchingProgress, Not flag - Me.LabLaunchingProgress.Opacity, 90, 0, Nothing, False), ModAnimation.AaOpacity(Me.LabLaunchingProgressLeft, If((Not flag), 0.5, 0.0) - Me.LabLaunchingProgressLeft.Opacity, 90, 0, Nothing, False), ModAnimation.AaOpacity(Me.PanLaunchingHint, (flag > False) - Me.PanLaunchingHint.Opacity, 90, 0, Nothing, False) })
                    End If
                    ModAnimation.AniStart(list, "Launching Progress", False)
                End If
            Catch ex3 As Exception
                Logger.[Error](ex3, "刷新启动信息失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchLeft.xaml.vb")
            End Try
        End Sub

        Private Sub PanLaunchingInfo_SizeChangedW(sender As Object, e As SizeChangedEventArgs)
            Dim num As Double = e.NewSize.Width - e.PreviousSize.Width
            If e.PreviousSize.Width <> 0.0 AndAlso Not Me.IsWidthAnimating AndAlso Math.Abs(num) >= 1.0 AndAlso Me.PanLaunchingInfo.ActualWidth <> 0.0 Then
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaWidth(Me.PanLaunchingInfo, num, 180, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaCode(Sub()
                    Me.IsWidthAnimating = False
                    Me.PanLaunchingInfo.Width = Me.ActualUsedWidth
                End Sub, 0, True) }, "Launching Info Width", False)
                Me.IsWidthAnimating = True
                Me.ActualUsedWidth = Me.PanLaunchingInfo.Width
                Me.PanLaunchingInfo.Width = e.PreviousSize.Width
            End If
        End Sub

        Private Sub PanLaunchingInfo_SizeChangedH(sender As Object, e As SizeChangedEventArgs)
            Dim num As Double = e.NewSize.Height - e.PreviousSize.Height
            If e.PreviousSize.Height <> 0.0 AndAlso Not Me.IsHeightAnimating AndAlso Math.Abs(num) >= 1.0 AndAlso Me.PanLaunchingInfo.ActualHeight <> 0.0 Then
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaHeight(Me.PanLaunchingInfo, num, 180, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaCode(Sub()
                    Me.IsHeightAnimating = False
                    Me.PanLaunchingInfo.Height = Me.ActualUsedHeight
                End Sub, 0, True) }, "Launching Info Height", False)
                Me.IsHeightAnimating = True
                Me.ActualUsedHeight = Me.PanLaunchingInfo.Height
                Me.PanLaunchingInfo.Height = e.PreviousSize.Height
            End If
        End Sub

        Friend Overridable Property PanBack As PageLaunchLeft

        Friend Overridable Property PanInput As Grid

        Friend Overridable Property BtnVersion As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnVersion
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.BtnVersion_Click()
                End Sub
                Dim myButton As OurButton = Me._BtnVersion
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnVersion = value
                myButton = Me._BtnVersion
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnMore As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnMore
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.BtnMore_Click()
                End Sub
                Dim myButton As OurButton = Me._BtnMore
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnMore = value
                myButton = Me._BtnMore
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanLogin As Grid

        Friend Overridable Property PanTypeOne As Grid

        Friend Overridable Property PathTypeOne As Path

        Friend Overridable Property LabTypeOne As TextBlock

        Friend Overridable Property PanType As Grid

        Friend Overridable Property RadioLoginType5 As OurRadioButton
            <CompilerGenerated()>
            Get
                Return Me._RadioLoginType5
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurRadioButton)
                Dim checkEventHandler As OurRadioButton.CheckEventHandler = AddressOf Me.RadioLoginType_Change
                Dim myRadioButton As OurRadioButton = Me._RadioLoginType5
                If myRadioButton IsNot Nothing Then
                    RemoveHandler myRadioButton.Check, checkEventHandler
                End If
                Me._RadioLoginType5 = value
                myRadioButton = Me._RadioLoginType5
                If myRadioButton IsNot Nothing Then
                    AddHandler myRadioButton.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property RadioLoginType0 As OurRadioButton
            <CompilerGenerated()>
            Get
                Return Me._RadioLoginType0
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurRadioButton)
                Dim checkEventHandler As OurRadioButton.CheckEventHandler = AddressOf Me.RadioLoginType_Change
                Dim myRadioButton As OurRadioButton = Me._RadioLoginType0
                If myRadioButton IsNot Nothing Then
                    RemoveHandler myRadioButton.Check, checkEventHandler
                End If
                Me._RadioLoginType0 = value
                myRadioButton = Me._RadioLoginType0
                If myRadioButton IsNot Nothing Then
                    AddHandler myRadioButton.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property AprilScaleTrans As ScaleTransform

        Friend Overridable Property AprilPosTrans As TranslateTransform

        Friend Overridable Property BtnLaunch As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnLaunch
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.LaunchButtonClick()
                End Sub
                Dim routedEventHandler As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    Me.RefreshButtonsUI()
                End Sub
                Dim myButton As OurButton = Me._BtnLaunch
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                    RemoveHandler myButton.Loaded, routedEventHandler
                End If
                Me._BtnLaunch = value
                myButton = Me._BtnLaunch
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                    AddHandler myButton.Loaded, routedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabVersion As TextBlock

        Friend Overridable Property PanLaunching As Grid

        Friend Overridable Property LoadLaunching As OurLoading

        Friend Overridable Property LabLaunchingTitle As TextBlock

        Friend Overridable Property LabLaunchingName As TextBlock

        Friend Overridable Property ProgressLaunchingFinished As ColumnDefinition

        Friend Overridable Property ProgressLaunchingUnfinished As ColumnDefinition

        Friend Overridable Property PanLaunchingInfo As Grid
            <CompilerGenerated()>
            Get
                Return Me._PanLaunchingInfo
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Grid)
                Dim sizeChangedEventHandler As SizeChangedEventHandler = AddressOf Me.PanLaunchingInfo_SizeChangedW
                Dim sizeChangedEventHandler2 As SizeChangedEventHandler = AddressOf Me.PanLaunchingInfo_SizeChangedH
                Dim grid As Grid = Me._PanLaunchingInfo
                If grid IsNot Nothing Then
                    RemoveHandler grid.SizeChanged, sizeChangedEventHandler
                    RemoveHandler grid.SizeChanged, sizeChangedEventHandler2
                End If
                Me._PanLaunchingInfo = value
                grid = Me._PanLaunchingInfo
                If grid IsNot Nothing Then
                    AddHandler grid.SizeChanged, sizeChangedEventHandler
                    AddHandler grid.SizeChanged, sizeChangedEventHandler2
                End If
            End Set
        End Property

        Friend Overridable Property LabLaunchingStage As TextBlock

        Friend Overridable Property LabLaunchingMethod As TextBlock

        Friend Overridable Property LabLaunchingProgressLeft As TextBlock

        Friend Overridable Property LabLaunchingProgress As TextBlock

        Friend Overridable Property LabLaunchingDownloadLeft As TextBlock

        Friend Overridable Property LabLaunchingDownload As TextBlock

        Friend Overridable Property PanLaunchingHint As Grid

        Friend Overridable Property LabLaunchingHint As TextBlock

        Friend Overridable Property BtnCancel As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnCancel
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.BtnCancel_Click()
                End Sub
                Dim myButton As OurButton = Me._BtnCancel
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnCancel = value
                myButton = Me._BtnCancel
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Private IsLoad As Boolean

        Private IsLoadFinished As Boolean

        Private PageCurrent As PageLaunchLeft.PageType

        Public Shared SkinMs As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String) = New ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)("Loader Skin Ms", AddressOf PageLaunchLeft.SkinMsLoad, AddressOf PageLaunchLeft.SkinMsInput, ThreadPriority.AboveNormal)

        Public Shared SkinLegacy As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String) = New ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)("Loader Skin Legacy", AddressOf PageLaunchLeft.SkinLegacyLoad, AddressOf PageLaunchLeft.SkinLegacyInput, ThreadPriority.AboveNormal)

        Public Shared SkinNide As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String) = New ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)("Loader Skin Nide", AddressOf PageLaunchLeft.SkinNideLoad, AddressOf PageLaunchLeft.SkinNideInput, ThreadPriority.AboveNormal)

        Public Shared SkinAuth As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String) = New ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)("Loader Skin Auth", AddressOf PageLaunchLeft.SkinAuthLoad, AddressOf PageLaunchLeft.SkinAuthInput, ThreadPriority.AboveNormal)

        Public Shared SkinLoaders As List(Of ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)) = New List(Of ModLoader.LoaderTask(Of ValueTuple(Of String, String), String))() From { PageLaunchLeft.SkinMs, PageLaunchLeft.SkinLegacy, PageLaunchLeft.SkinNide, PageLaunchLeft.SkinAuth }

        Private BtnLaunchState As Integer

        Private BtnLaunchInstance As ModMinecraft.McInstance

        Private ShowProgress As Double

        Private IsWidthAnimating As Boolean

        Private ActualUsedWidth As Double

        Private IsHeightAnimating As Boolean

        Private ActualUsedHeight As Double

        Private Enum PageType
            None
            Legacy
            Nide
            NideSkin
            Auth
            AuthSkin
            Ms
            MsSkin
        End Enum
    End Class
End Namespace
