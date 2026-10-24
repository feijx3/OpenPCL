Imports System
Imports System.CodeDom.Compiler
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports MeloongCore
Imports MeloongCore.Extensions
Imports MeloongCore.Wpf
Imports Microsoft.VisualBasic.CompilerServices
Imports PCL.My
Imports PCLCS

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageInstanceMod
        Inherits OurPageRight
        Implements IRefreshable

        Public Sub New()
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.PageOther_Loaded()
            End Sub
            AddHandler MyBase.Initialized, Sub(a0 As Object, a1 As EventArgs)
                Me.LoaderInit()
            End Sub
            AddHandler MyBase.PageExit, AddressOf Me.UnselectedAllWithAnimation
            Me.IsLoad = False
            Me.ModItems = New ConcurrentDictionary(Of String, OurLocalModItem)()
            Me.BottomBarShownCount = 0
            Me.SelectedMods = New List(Of String)()
            Me._Filter = PageInstanceMod.FilterType.All
            Me.InitializeComponent()
        End Sub

        Public Sub PageOther_Loaded()
            If ModMain.FrmMain.PageLast.Page <> FormMain.PageType.ResourceDetail Then
                Me.PanBack.ScrollToHome()
            End If
            ModAnimation.AniControlEnabled += 1
            Me.SelectedMods.Clear()
            Me.ReloadModList(False)
            Me.ChangeAllSelected(False)
            ModAnimation.AniControlEnabled -= 1
            If Not Me.IsLoad Then
                Me.IsLoad = True
                AddHandler ModMain.FrmMain.KeyDown, AddressOf Me.FrmMain_KeyDown
                Try
                    For Each obj As Object In Me.PanFilter.Children
                        CType(obj, OurRadioButton).LabText.Margin = New Thickness(-2.0, 0.0, 8.0, 0.0)
                    Next
                Finally
                    Dim enumerator As IEnumerator
                    If TypeOf enumerator Is IDisposable Then
                        TryCast(enumerator, IDisposable).Dispose()
                    End If
                End Try
            End If
        End Sub

        Public Sub ReloadModList(Optional ForceReload As Boolean = False)
            If Me.LoaderRun(If(ForceReload, ModLoader.LoaderFolderRunType.ForceRun, ModLoader.LoaderFolderRunType.RunOnUpdated)) Then
                Logger.Info("已刷新 Mod 列表", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
                Me.Filter = PageInstanceMod.FilterType.All
                Me.PanBack.ScrollToHome()
                Me.SearchBox.Text = ""
            End If
        End Sub

        Sub RefreshSelf() Implements PCL.IRefreshable.Refresh
            PageInstanceMod.Refresh()
        End Sub

        Public Shared Sub Refresh()
            Try
                ResourceProject.Cache.Clear()
                ResourceVersion.ProjectFilesCache.Clear()
                FileUtils.Delete(ModBase.PathTemp + "Cache\LocalMod.json", False)
                Logger.Info("由于点击刷新按钮，清理本地 Mod 信息缓存", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
            Catch ex As Exception
                Logger.Warn(ex, "强制刷新时清理本地 Mod 信息缓存失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
            End Try
            If ModMain.FrmInstanceMod IsNot Nothing Then
                ModMain.FrmInstanceMod.ReloadModList(True)
            End If
            ModMain.FrmInstanceLeft.ItemMod.Checked = True
            ModMain.Hint("正在刷新……", ModMain.HintType.Blue, False)
        End Sub

        Private Sub LoaderInit()
            MyBase.PageLoaderInit(Of String, List(Of LocalResourceFile))(Me.Load, Me.PanLoad, Me.PanAllBack, Nothing, LocalResourceLoaders.LocalResourceLoader, Sub(a0 As ModLoader.LoaderTask(Of String, List(Of LocalResourceFile)))
                Me.LoadUIFromLoaderOutput()
            End Sub, Nothing, False)
        End Sub

        Private Sub Load_Click(sender As Object, e As MouseButtonEventArgs)
            If LocalResourceLoaders.LocalResourceLoader.State = ModBase.LoadState.Failed Then
                Me.LoaderRun(ModLoader.LoaderFolderRunType.ForceRun)
            End If
        End Sub

        Public Function LoaderRun(Type As ModLoader.LoaderFolderRunType) As Boolean
            Return ModLoader.LoaderFolderRun(LocalResourceLoaders.LocalResourceLoader, PageInstanceLeft.Instance.PathIndie + "mods\", Type, 0, "", False)
        End Function

        Private Sub LoadUIFromLoaderOutput()
            Try
                If LocalResourceLoaders.LocalResourceLoader.Output.Any() Then
                    Me.PanBack.Visibility = Visibility.Visible
                    Me.PanEmpty.Visibility = Visibility.Collapsed
                    Me.ModItems.Clear()
                    Try
                        For Each localResourceFile As LocalResourceFile In LocalResourceLoaders.LocalResourceLoader.Output
                            Me.ModItems(localResourceFile.EnabledName) = Me.McModListItem(localResourceFile)
                        Next
                    Finally
                        Dim enumerator As List(Of LocalResourceFile).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                    Me.Filter = PageInstanceMod.FilterType.All
                    Me.SearchBox.Text = ""
                    Me.RefreshUI()
                Else
                    Me.PanEmpty.Visibility = Visibility.Visible
                    Me.PanBack.Visibility = Visibility.Collapsed
                End If
            Catch ex As Exception
                Logger.[Error](ex, "加载 Mod 列表 UI 失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
            End Try
        End Sub

        Private Function McModListItem(Entry As LocalResourceFile) As OurLocalModItem
            ' The following expression was wrapped in a checked-statement
            ModAnimation.AniControlEnabled += 1
            Dim NewItem As OurLocalModItem = New OurLocalModItem() With { .SnapsToDevicePixels = True, .Entry = Entry, .ButtonHandler = AddressOf Me.McModContent, .Checked = Me.SelectedMods.Contains(Entry.EnabledName) }
            AddHandler Entry.OnUpdate, Sub(a0 As LocalResourceFile)
                MyBase._Lambda$__0()
            End Sub
            ModAnimation.AniControlEnabled -= 1
            Return NewItem
        End Function

        Private Sub McModContent(sender As OurLocalModItem, e As EventArgs)
            AddHandler sender.Changed, Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                Me.CheckChanged(CType(a0, OurLocalModItem), a1)
            End Sub
            Dim clickEventHandler As OurLocalModItem.ClickEventHandler
            If PageInstanceMod._Closure$__.$IR13-6 IsNot Nothing Then
                clickEventHandler = PageInstanceMod._Closure$__.$IR13-6
            Else
                Dim clickEventHandler2 As OurLocalModItem.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_16(Of OurLocalModItem, EventArgs)
                    If PageInstanceMod._Closure$__.$I13-0 IsNot Nothing Then
                        vb$AnonymousDelegate_ = PageInstanceMod._Closure$__.$I13-0
                    Else
                        Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_16(Of OurLocalModItem, EventArgs) = Sub(ss As OurLocalModItem, ee As EventArgs)
                            ss.Checked = Not ss.Checked
                        End Sub
                        vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                        PageInstanceMod._Closure$__.$I13-0 = vb$AnonymousDelegate_2
                    End If
                    vb$AnonymousDelegate_(CType(a0, OurLocalModItem), a1)
                End Sub
                clickEventHandler = clickEventHandler2
                PageInstanceMod._Closure$__.$IR13-6 = clickEventHandler2
            End If
            AddHandler sender.Click, clickEventHandler
            Dim myIconButton As OurIconButton = New OurIconButton() With { .LogoScale = 1.05, .Logo = "M889.018182 418.909091H884.363636V316.509091a93.090909 93.090909 0 0 0-99.607272-89.832727h-302.545455l-93.090909-76.334546A46.545455 46.545455 0 0 0 358.865455 139.636364H146.152727A93.090909 93.090909 0 0 0 46.545455 229.469091V837.818182a46.545455 46.545455 0 0 0 46.545454 46.545454 46.545455 46.545455 0 0 0 16.756364-3.258181 109.381818 109.381818 0 0 0 25.134545 3.258181h586.472727a85.178182 85.178182 0 0 0 87.04-63.301818l163.374546-302.545454a46.545455 46.545455 0 0 0 5.585454-21.876364A82.385455 82.385455 0 0 0 889.018182 418.909091z m-744.727273-186.181818h198.283636l93.09091 76.334545a46.545455 46.545455 0 0 0 29.323636 10.705455h319.301818a12.101818 12.101818 0 0 1 6.516364 0V418.909091H302.545455a85.178182 85.178182 0 0 0-87.04 63.301818L139.636364 622.778182V232.727273a19.549091 19.549091 0 0 1 6.516363 0z m578.094546 552.029091a27.461818 27.461818 0 0 0-2.792728 6.516363H154.530909l147.083636-272.290909a27.461818 27.461818 0 0 0 2.792728-6.981818h565.061818z", .Tag = sender }
            myIconButton.ToolTip = "打开文件位置"
            AddHandler myIconButton.Click, Sub(a0 As Object, a1 As EventArgs)
                Me.Open_Click(CType(a0, OurIconButton), a1)
            End Sub
            Dim myIconButton2 As OurIconButton = New OurIconButton() With { .LogoScale = 1.0, .Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z", .Tag = sender }
            myIconButton2.ToolTip = "详情"
            AddHandler myIconButton2.Click, AddressOf Me.Info_Click
            AddHandler sender.MouseRightButtonUp, AddressOf Me.Info_Click
            Dim myIconButton3 As OurIconButton = New OurIconButton() With { .LogoScale = 1.0, .Logo = "M520.192 0C408.43 0 317.44 82.87 313.563 186.734H52.736c-29.038 0-52.663 21.943-52.663 49.079s23.625 49.152 52.663 49.152h58.075v550.473c0 103.35 75.118 187.757 167.717 187.757h472.43c92.599 0 167.716-83.894 167.716-187.757V285.477h52.59c29.038 0 52.59-21.943 52.663-49.08-0.073-27.135-23.625-49.151-52.663-49.151H726.235C723.237 83.017 631.955 0 520.192 0zM404.846 177.957c3.803-50.03 50.176-89.015 107.447-89.015 57.197 0 103.57 38.985 106.788 89.015H404.92zM284.379 933.669c-33.353 0-69.997-39.351-69.997-95.525v-549.01H833.39v549.522c0 56.247-36.645 95.525-69.998 95.525H284.379v-0.512z M357.23 800.695a48.274 48.274 0 0 0 47.616-49.006V471.7a48.274 48.274 0 0 0-47.543-49.08 48.274 48.274 0 0 0-47.69 49.006V751.69c0 27.282 20.846 49.006 47.617 49.006z m166.62 0a48.274 48.274 0 0 0 47.688-49.006V471.7a48.274 48.274 0 0 0-47.689-49.08 48.274 48.274 0 0 0-47.543 49.006V751.69c0 27.282 21.431 49.006 47.543 49.006z m142.92 0a48.274 48.274 0 0 0 47.543-49.006V471.7a48.274 48.274 0 0 0-47.543-49.08 48.274 48.274 0 0 0-47.616 49.006V751.69c0 27.282 20.773 49.006 47.543 49.006z", .Tag = sender }
            myIconButton3.ToolTip = "删除"
            AddHandler myIconButton3.Click, Sub(a0 As Object, a1 As EventArgs)
                Me.Delete_Click(CType(a0, OurIconButton), a1)
            End Sub
            Dim myIconButton4 As OurIconButton = New OurIconButton() With { .LogoScale = 1.0, .Logo = If(sender.Entry.IsEnabled, "M508 990.4c-261.6 0-474.4-212-474.4-474.4S246.4 41.6 508 41.6s474.4 212 474.4 474.4S769.6 990.4 508 990.4zM508 136.8c-209.6 0-379.2 169.6-379.2 379.2 0 209.6 169.6 379.2 379.2 379.2s379.2-169.6 379.2-379.2C887.2 306.4 717.6 136.8 508 136.8zM697.6 563.2 318.4 563.2c-26.4 0-47.2-21.6-47.2-47.2 0-26.4 21.6-47.2 47.2-47.2l379.2 0c26.4 0 47.2 21.6 47.2 47.2C744.8 542.4 724 563.2 697.6 563.2z", "M512 0a512 512 0 1 0 512 512A512 512 0 0 0 512 0z m0 921.6a409.6 409.6 0 1 1 409.6-409.6 409.6 409.6 0 0 1-409.6 409.6z M716.8 339.968l-256 253.44L328.192 460.8A51.2 51.2 0 0 0 256 532.992l168.448 168.96a51.2 51.2 0 0 0 72.704 0l289.28-289.792A51.2 51.2 0 0 0 716.8 339.968z"), .Tag = sender }
            myIconButton4.ToolTip = If(sender.Entry.IsEnabled, "禁用", "启用")
            AddHandler myIconButton4.Click, Sub(a0 As Object, a1 As EventArgs)
                Me.ED_Click(CType(a0, OurIconButton), a1)
            End Sub
            sender.Buttons = New OurIconButton() { myIconButton2, myIconButton, myIconButton4, myIconButton3 }
        End Sub

        Public Sub RefreshUI()
            Dim CS$<>8__locals1 As PageInstanceMod._Closure$__14-0 = New PageInstanceMod._Closure$__14-0(CS$<>8__locals1)
            If Me.PanList IsNot Nothing Then
                CS$<>8__locals1.$VB$Local_ShowingMods = If(Me.IsSearching, Me.SearchResult, (If(LocalResourceLoaders.LocalResourceLoader.Output, New List(Of LocalResourceFile)()))).Where(Function(m As LocalResourceFile) Me.CanPassFilter(m)).ToList()
                ModAnimation.AniControlEnabled += 1
                If CS$<>8__locals1.$VB$Local_ShowingMods.Any() Then
                    Me.PanList.Visibility = Visibility.Visible
                    Me.PanList.Children.Clear()
                    Try
                        For Each localResourceFile As LocalResourceFile In CS$<>8__locals1.$VB$Local_ShowingMods
                            Dim myLocalModItem As OurLocalModItem = Nothing
                            If Me.ModItems.TryGetValue(localResourceFile.EnabledName, myLocalModItem) Then
                                myLocalModItem.Checked = Me.SelectedMods.Contains(localResourceFile.EnabledName)
                                Me.PanList.Children.Add(myLocalModItem)
                            End If
                        Next
                        GoTo IL_0104
                    Finally
                        Dim enumerator As List(Of LocalResourceFile).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                End If
                Me.PanList.Visibility = Visibility.Collapsed
                IL_0104:
                ModAnimation.AniControlEnabled -= 1
                Me.SelectedMods = Me.SelectedMods.Where(Function(m As String)
                    Dim CS$<>8__locals2 As PageInstanceMod._Closure$__14-1 = New PageInstanceMod._Closure$__14-1(CS$<>8__locals2)
                    CS$<>8__locals2.$VB$Local_m = m
                    Return CS$<>8__locals1.$VB$Local_ShowingMods.Any(Function(s As LocalResourceFile) Operators.CompareString(s.EnabledName, CS$<>8__locals2.$VB$Local_m, False) = 0)
                End Function).ToList()
                Me.RefreshBars()
            End If
        End Sub

        Public Sub RefreshBars()
            Dim num As Integer = 0
            Dim num2 As Integer = 0
            Dim num3 As Integer = 0
            Dim count As Integer
            Dim flag As Boolean
            Try
                For Each localResourceFile As LocalResourceFile In If(Me.IsSearching, Me.SearchResult, (If(LocalResourceLoaders.LocalResourceLoader.Output, New List(Of LocalResourceFile)())))
                    num += 1
                    If localResourceFile.HasUpdate Then
                        num3 += 1
                    End If
                    If localResourceFile.IsEnabled Then
                        num2 += 1
                    End If
                Next
            Finally
                Dim enumerator As List(Of LocalResourceFile).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            Dim num4 As Integer = num - num2
            Me.BtnFilterAll.Text = If(Me.IsSearching, "搜索结果", "全部") + String.Format(" ({0})", num)
            Me.BtnFilterCanUpdate.Text = String.Format("可更新 ({0})", num3)
            Me.BtnFilterCanUpdate.Visibility = If((Me.Filter = PageInstanceMod.FilterType.CanUpdate OrElse num3 > 0), Visibility.Visible, Visibility.Collapsed)
            Me.BtnFilterEnabled.Text = String.Format("启用 ({0})", num2)
            Me.BtnFilterEnabled.Visibility = If((Me.Filter = PageInstanceMod.FilterType.Enabled OrElse (num2 > 0 AndAlso num2 < num)), Visibility.Visible, Visibility.Collapsed)
            Me.BtnFilterDisabled.Text = String.Format("禁用 ({0})", num4)
            Me.BtnFilterDisabled.Visibility = If((Me.Filter = PageInstanceMod.FilterType.Disabled OrElse num4 > 0), Visibility.Visible, Visibility.Collapsed)
            count = Me.SelectedMods.Count
            flag = count > 0
            If flag Then
                Me.LabSelect.Text = String.Format("已选择 {0} 个文件", count)
            End If
            If flag Then
                Dim flag2 As Boolean = False
                Dim flag3 As Boolean = False
                Dim flag4 As Boolean = False
                Try
                    For Each localResourceFile2 As LocalResourceFile In LocalResourceLoaders.LocalResourceLoader.Output
                        If Me.SelectedMods.Contains(localResourceFile2.EnabledName) Then
                            If localResourceFile2.HasUpdate Then
                                flag2 = True
                            End If
                            If localResourceFile2.IsEnabled Then
                                flag3 = True
                            End If
                            If Not localResourceFile2.IsEnabled Then
                                flag4 = True
                            End If
                        End If
                    Next
                Finally
                    Dim enumerator2 As List(Of LocalResourceFile).Enumerator
                    CType(enumerator2, IDisposable).Dispose()
                End Try
                Me.BtnSelectDisable.IsEnabled = flag3
                Me.BtnSelectEnable.IsEnabled = flag4
                Me.BtnSelectUpdate.IsEnabled = flag2
            End If
            If ModAnimation.AniControlEnabled = 0 Then
                Me.PanListBack.Margin = New Thickness(0.0, 0.0, 0.0, CDbl(If(flag, 95, 15)))
                If flag Then
                    If Me.BottomBarShownCount >= count Then
                        Me.BottomBarShownCount = count
                        Return
                    End If
                    Me.BottomBarShownCount = count
                    Me.CardSelect.Visibility = Visibility.Visible
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.CardSelect, 1.0 - Me.CardSelect.Opacity, 60, 0, Nothing, False), ModAnimation.AaTranslateY(Me.CardSelect, -27.0 - Me.TransSelect.Y, 120, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaTranslateY(Me.CardSelect, 3.0, 150, 120, New ModAnimation.AniEaseInoutFluent(ModAnimation.AniEasePower.Weak, 0.5), False), ModAnimation.AaTranslateY(Me.CardSelect, -1.0, 90, 270, New ModAnimation.AniEaseInoutFluent(ModAnimation.AniEasePower.Weak, 0.5), False) }, "Mod Sidebar", False)
                    Return
                ElseIf Me.BottomBarShownCount <> 0 Then
                    Me.BottomBarShownCount = 0
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.CardSelect, -Me.CardSelect.Opacity, 90, 0, Nothing, False), ModAnimation.AaTranslateY(Me.CardSelect, -10.0 - Me.TransSelect.Y, 90, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaCode(Sub()
                        Me.CardSelect.Visibility = Visibility.Collapsed
                    End Sub, 0, True) }, "Mod Sidebar", False)
                    Return
                End If
            Else
                ModAnimation.AniStop("Mod Sidebar")
                Me.BottomBarShownCount = count
                If flag Then
                    Me.CardSelect.Visibility = Visibility.Visible
                    Me.CardSelect.Opacity = 1.0
                    Me.TransSelect.Y = -25.0
                    Return
                End If
                Me.CardSelect.Visibility = Visibility.Collapsed
                Me.CardSelect.Opacity = 0.0
                Me.TransSelect.Y = -10.0
            End If
        End Sub

        Private Sub BtnManageOpen_Click(sender As Object, e As EventArgs)
            Try
                DirectoryUtils.Create(PageInstanceLeft.Instance.PathIndie + "mods\")
                ModBase.OpenExplorer(PageInstanceLeft.Instance.PathIndie + "mods\")
            Catch ex As Exception
                Logger.[Error](ex, "打开 Mods 文件夹失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
            End Try
        End Sub

        Private Sub BtnManageSelectAll_Click(sender As Object, e As MouseButtonEventArgs)
            Me.ChangeAllSelected(Me.SelectedMods.Count < Me.PanList.Children.Count)
        End Sub

        Private Sub BtnManageInstall_Click(sender As Object, e As MouseButtonEventArgs)
            Dim list As List(Of String) = Dialogs.SelectFile("选择要安装的 Mod", True, Nothing, New OneOf(Of IEnumerable(Of ValueTuple(Of String(), String)), String)?(New ValueTuple(Of String(), String)() { New ValueTuple(Of String(), String)(New String() { "jar", "litemod", "disabled", "old" }, "Mod 文件") }))
            If list.Any() Then
                PageInstanceMod.InstallMods(list)
            End If
        End Sub

        Public Shared Function InstallMods(FilePathList As IEnumerable(Of String)) As Boolean
            Dim CS$<>8__locals1 As PageInstanceMod._Closure$__20-0 = New PageInstanceMod._Closure$__20-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_Extension = PathUtils.GetExtension(FilePathList.First())
            Dim flag As Boolean
            If Not New String() { "jar", "litemod", "disabled", "old" }.Any(Function(t As String) Operators.CompareString(t, CS$<>8__locals1.$VB$Local_Extension, False) = 0) Then
                flag = False
            Else
                Logger.Info("文件为 jar/litemod 格式，尝试作为 Mod 安装", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
                If FilePathList.First().Contains(":\$RECYCLE.BIN\") Then
                    ModMain.Hint("请先将文件从回收站还原，再尝试安装！", ModMain.HintType.Red, True)
                    flag = True
                Else
                    Dim mcInstance As ModMinecraft.McInstance = ModMinecraft.McInstanceSelected
                    If ModMain.FrmMain.PageCurrent Is FormMain.PageType.InstanceSetup Then
                        mcInstance = PageInstanceLeft.Instance
                    End If
                    If ModMain.FrmMain.PageCurrent Is FormMain.PageType.InstanceSelect OrElse mcInstance Is Nothing OrElse Not mcInstance.Modable Then
                        ModMain.Hint("若要安装 Mod，请先选择一个可以安装 Mod 的版本！", ModMain.HintType.Blue, True)
                    ElseIf(ModMain.FrmMain.PageCurrent Is FormMain.PageType.InstanceSetup AndAlso ModMain.FrmMain.PageCurrentSub = FormMain.PageSubType.DownloadPack) OrElse ModMain.OurMsgBox(String.Format("是否要将这{0}文件作为 Mod 安装到 {1}？", If(FilePathList.IsSingle(), "个", "些"), mcInstance.Name), "Mod 安装确认", "确定", "取消", "", False, True, False, Nothing, Nothing, Nothing) = 1 Then
                        Try
                            Try
                                For Each text As String In FilePathList
                                    Dim text2 As String = New LocalResourceFile(text).EnabledName
                                    If Not text2.Contains(".") Then
                                        text2 += ".jar"
                                    End If
                                    text2 = mcInstance.PathIndie + "mods\" + text2
                                    If Not PathUtils.IsSame(text, text2) Then
                                        FileUtils.Delete(text2, True)
                                        FileUtils.Copy(text, text2)
                                    End If
                                Next
                            Finally
                                Dim enumerator As IEnumerator(Of String)
                                If enumerator IsNot Nothing Then
                                    enumerator.Dispose()
                                End If
                            End Try
                            If FilePathList.IsSingle() Then
                                ModMain.Hint(String.Format("已安装 {0}！", New LocalResourceFile(FilePathList.First()).EnabledName), ModMain.HintType.Green, True)
                            Else
                                ModMain.Hint(String.Format("已安装 {0} 个 Mod！", FilePathList.Count()), ModMain.HintType.Green, True)
                            End If
                            If ModMain.FrmMain.PageCurrent Is FormMain.PageType.InstanceSetup AndAlso ModMain.FrmMain.PageCurrentSub = FormMain.PageSubType.DownloadPack Then
                                ModLoader.LoaderFolderRun(LocalResourceLoaders.LocalResourceLoader, mcInstance.PathIndie + "mods\", ModLoader.LoaderFolderRunType.ForceRun, 0, "", False)
                            End If
                        Catch ex As Exception
                            Logger.[Error](ex, "复制 Mod 文件失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
                        End Try
                    End If
                    flag = True
                End If
            End If
            Return flag
        End Function

        Private Sub BtnManageDownload_Click(sender As Object, e As MouseButtonEventArgs)
            PageResource.TargetInstance = PageInstanceLeft.Instance
            PageResource.TargetName = Me.SearchBox.Text
            ModMain.FrmMain.PageChange(FormMain.PageType.Download, FormMain.PageSubType.DownloadMod)
        End Sub

        Public Sub CheckChanged(sender As OurLocalModItem, e As ModBase.RouteEventArgs)
            If ModAnimation.AniControlEnabled = 0 Then
                Dim enabledName As String = sender.Entry.EnabledName
                If sender.Checked Then
                    If Not Me.SelectedMods.Contains(enabledName) Then
                        Me.SelectedMods.Add(enabledName)
                    End If
                Else
                    Me.SelectedMods.Remove(enabledName)
                End If
                Me.RefreshBars()
            End If
        End Sub

        Private Sub ChangeAllSelected(Value As Boolean)
            ' The following expression was wrapped in a checked-statement
            ModAnimation.AniControlEnabled += 1
            Me.SelectedMods.Clear()
            Try
                For Each myLocalModItem As OurLocalModItem In Me.ModItems.Values
                    Dim flag As Boolean = Value AndAlso Me.PanList.Children.Contains(myLocalModItem)
                    myLocalModItem.Checked = flag
                    If flag Then
                        Me.SelectedMods.Add(myLocalModItem.Entry.EnabledName)
                    End If
                Next
            Finally
                Dim enumerator As IEnumerator(Of OurLocalModItem)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            ModAnimation.AniControlEnabled -= 1
            Me.RefreshBars()
        End Sub

        Private Sub UnselectedAllWithAnimation()
            Dim aniControlEnabled As Integer = ModAnimation.AniControlEnabled
            ModAnimation.AniControlEnabled = 0
            Me.ChangeAllSelected(False)
            ModAnimation.AniControlEnabled += aniControlEnabled
        End Sub

        Private Sub FrmMain_KeyDown(sender As Object, e As KeyEventArgs)
            If ModMain.FrmMain.PageRight Is Me AndAlso MyWpfExtension.Computer.Keyboard.CtrlKeyDown AndAlso e.Key = Key.A Then
                Me.ChangeAllSelected(True)
            End If
        End Sub

        Private Sub SearchBox_PreviewKeyDown(sender As Object, e As KeyEventArgs)
            If Not Me.SearchBox.Text.Any() AndAlso MyWpfExtension.Computer.Keyboard.CtrlKeyDown AndAlso e.Key = Key.A Then
                Me.PanBack.Focus()
            End If
        End Sub

        Public Property Filter As PageInstanceMod.FilterType
            Get
                Return Me._Filter
            End Get
            Set(value As PageInstanceMod.FilterType)
                If Me._Filter <> value Then
                    Me._Filter = value
                    Select Case value
                        Case PageInstanceMod.FilterType.All
                            Me.BtnFilterAll.Checked = True
                        Case PageInstanceMod.FilterType.Enabled
                            Me.BtnFilterEnabled.Checked = True
                        Case PageInstanceMod.FilterType.Disabled
                            Me.BtnFilterDisabled.Checked = True
                        Case Else
                            Me.BtnFilterCanUpdate.Checked = True
                    End Select
                    Me.RefreshUI()
                End If
            End Set
        End Property

        Private Function CanPassFilter(CheckingMod As LocalResourceFile) As Boolean
            Dim flag As Boolean
            Select Case Me.Filter
                Case PageInstanceMod.FilterType.All
                    flag = True
                Case PageInstanceMod.FilterType.Enabled
                    flag = CheckingMod.IsEnabled
                Case PageInstanceMod.FilterType.Disabled
                    flag = Not CheckingMod.IsEnabled
                Case PageInstanceMod.FilterType.CanUpdate
                    flag = CheckingMod.HasUpdate
                Case Else
                    flag = False
            End Select
            Return flag
        End Function

        Private Sub ChangeFilter(sender As OurRadioButton, raiseByMouse As Boolean)
            Me.Filter = CType(Conversions.ToInteger(sender.Tag), PageInstanceMod.FilterType)
            Me.RefreshUI()
        End Sub

        Private Sub BtnSelectED_Click(sender As OurIconTextButton, e As ModBase.RouteEventArgs)
            Me.EDMods(LocalResourceLoaders.LocalResourceLoader.Output.Where(Function(m As LocalResourceFile) Me.SelectedMods.Contains(m.EnabledName)), Not sender.Equals(Me.BtnSelectDisable))
            Me.ChangeAllSelected(False)
        End Sub

        Private Sub EDMods(ModList As IEnumerable(Of LocalResourceFile), IsEnable As Boolean)
            Dim flag As Boolean = True
            Try
                For Each localResourceFile As LocalResourceFile In ModList.ToList()
                    Dim CS$<>8__locals1 As PageInstanceMod._Closure$__36-0 = New PageInstanceMod._Closure$__36-0(CS$<>8__locals1)
                    CS$<>8__locals1.$VB$Local_ModEntity = localResourceFile
                    Dim text As String = Nothing
                    If CS$<>8__locals1.$VB$Local_ModEntity.IsEnabled AndAlso Not IsEnable Then
                        text = PathUtils.RemoveExtendedPrefix(CS$<>8__locals1.$VB$Local_ModEntity.File.FullName) + If(FileUtils.Exists(CS$<>8__locals1.$VB$Local_ModEntity.File.FullName + ".old"), ".old", ".disabled")
                    Else
                        If CS$<>8__locals1.$VB$Local_ModEntity.IsEnabled OrElse Not IsEnable Then
                            Continue For
                        End If
                        text = CS$<>8__locals1.$VB$Local_ModEntity.EnabledFullName
                    End If
                    Try
                        If FileUtils.Exists(text) Then
                            If Not FileUtils.Exists(CS$<>8__locals1.$VB$Local_ModEntity.File.FullName) Then
                                Logger.Warn("Mod 的状态已被切换", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
                                Continue For
                            End If
                            If Operators.CompareString(CryptographyUtils.ComputeFileHash(CS$<>8__locals1.$VB$Local_ModEntity.File.FullName, CryptographyUtils.HashMethod.Md5), CryptographyUtils.ComputeFileHash(text, CryptographyUtils.HashMethod.Md5), False) <> 0 Then
                                ModMain.OurMsgBox(String.Format("目前同时存在启用和禁用的两个 Mod 文件：{0} - {1}{2} - {3}{4}{5}注意，这两个文件的内容并不相同。{6}在手动删除或重命名其中一个文件后，才能继续操作。", New Object() { vbCrLf, PathUtils.GetLastPart(text), vbCrLf, CS$<>8__locals1.$VB$Local_ModEntity.File.Name, vbCrLf, vbCrLf, vbCrLf }), "存在文件冲突", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
                                Continue For
                            End If
                        End If
                        FileUtils.Move(CS$<>8__locals1.$VB$Local_ModEntity.File.FullName, text, False)
                    Catch ex As FileNotFoundException
                        Logger.[Error](ex, String.Format("未找到需要重命名的 Mod（{0}）", If(CS$<>8__locals1.$VB$Local_ModEntity.File.FullName, "null")), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
                        Me.ReloadModList(True)
                        Return
                    Catch ex2 As Exception
                        Logger.Warn(ex2, String.Format("重命名 Mod 失败（{0}）", If(CS$<>8__locals1.$VB$Local_ModEntity.File.FullName, "null")), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
                        flag = False
                    End Try
                    Dim localResourceFile2 As LocalResourceFile = New LocalResourceFile(text)
                    localResourceFile2.FromJson(CS$<>8__locals1.$VB$Local_ModEntity.ToJson())
                    If LocalResourceLoaders.LocalResourceLoader.Output.Contains(CS$<>8__locals1.$VB$Local_ModEntity) Then
                        Dim num As Integer = LocalResourceLoaders.LocalResourceLoader.Output.IndexOf(CS$<>8__locals1.$VB$Local_ModEntity)
                        LocalResourceLoaders.LocalResourceLoader.Output.RemoveAt(num)
                        LocalResourceLoaders.LocalResourceLoader.Output.Insert(num, localResourceFile2)
                    End If
                    If Me.SearchResult IsNot Nothing AndAlso Me.SearchResult.Contains(CS$<>8__locals1.$VB$Local_ModEntity) Then
                        Dim num2 As Integer = Me.SearchResult.IndexOf(CS$<>8__locals1.$VB$Local_ModEntity)
                        Me.SearchResult.Remove(CS$<>8__locals1.$VB$Local_ModEntity)
                        Me.SearchResult.Insert(num2, localResourceFile2)
                    End If
                    Dim myLocalModItem As OurLocalModItem = Me.McModListItem(localResourceFile2)
                    Me.ModItems(CS$<>8__locals1.$VB$Local_ModEntity.EnabledName) = myLocalModItem
                    Dim num3 As Integer = Me.PanList.Children.IndexOf(Me.PanList.Children.OfType().FirstOrDefault(Function(i As OurLocalModItem) i.Entry Is CS$<>8__locals1.$VB$Local_ModEntity))
                    If num3 <> -1 Then
                        Me.PanList.Children.RemoveAt(num3)
                        Me.PanList.Children.Insert(num3, myLocalModItem)
                        myLocalModItem.UpdateLayout()
                    End If
                Next
            Finally
                Dim enumerator As List(Of LocalResourceFile).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            If flag Then
                Me.RefreshBars()
            Else
                ModMain.Hint("由于文件被占用，Mod 的状态切换失败，请尝试关闭正在运行的游戏后再试！", ModMain.HintType.Red, True)
                Me.ReloadModList(True)
            End If
            Me.LoaderRun(ModLoader.LoaderFolderRunType.UpdateOnly)
        End Sub

        Private Sub BtnSelectUpdate_Click()
            Dim list As List(Of LocalResourceFile) = LocalResourceLoaders.LocalResourceLoader.Output.Where(Function(m As LocalResourceFile) Me.SelectedMods.Contains(m.EnabledName) AndAlso m.HasUpdate).ToList()
            If list.Any() Then
                Me.UpdateMods(list)
                Me.ChangeAllSelected(False)
            End If
        End Sub

        Public Sub UpdateMods(ModList As IEnumerable(Of LocalResourceFile))
            Dim CS$<>8__locals1 As PageInstanceMod._Closure$__39-0 = New PageInstanceMod._Closure$__39-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_ModList = ModList
            If Not Settings.[Get](Of Boolean)("HintUpdateMod", Nothing) OrElse CS$<>8__locals1.$VB$Local_ModList.Count() >= 15 Then
                If ModMain.OurMsgBox(String.Format("新版本 Mod 可能不兼容旧存档或者其他 Mod，这可能导致游戏崩溃，甚至永久损坏存档！{0}如果你在游玩整合包，请千万不要自行更新 Mod！{1}{2}在更新前，请先备份存档，并检查 Mod 的更新日志。{3}如果更新后出现问题，你也可以在回收站找回更新前的 Mod。", New Object() { vbCrLf, vbCrLf, vbCrLf, vbCrLf }), "Mod 更新警告", "我已了解风险，继续更新", "取消", "", True, True, False, Nothing, Nothing, Nothing) <> 1 Then
                    Return
                End If
                Settings.[Set]("HintUpdateMod", True, Nothing)
            End If
            Try
                Dim CS$<>8__locals2 As PageInstanceMod._Closure$__39-1 = New PageInstanceMod._Closure$__39-1(CS$<>8__locals2)
                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1
                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ModList = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ModList.ToList()
                Dim list As List(Of ModNet.NetFile) = New List(Of ModNet.NetFile)()
                CS$<>8__locals2.$VB$Local_FileCopyList = New Dictionary(Of String, String)()
                Dim list4 As List(Of ModLoader.LoaderBase)
                Try
                    For Each localResourceFile As LocalResourceFile In CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ModList
                        Dim updateFile As ResourceVersion = localResourceFile.UpdateFile
                        If updateFile.DownloadAvailable Then
                            Dim text As String = PathUtils.GetFileNameWithoutExtension(localResourceFile.ProjectVersion.FileName)
                            Dim text2 As String = PathUtils.GetFileNameWithoutExtension(localResourceFile.UpdateFile.FileName)
                            Dim list2 As List(Of String) = text.Split(New Char() { "-"c }).ToList()
                            Dim list3 As List(Of String) = text2.Split(New Char() { "-"c }).ToList()
                            Dim flag As Boolean = False
                            While list2.Any() AndAlso list3.Any() AndAlso Operators.CompareString(list2.First(), list3.First(), False) = 0
                                list2.RemoveAt(0)
                                list3.RemoveAt(0)
                                flag = True
                            End While
                            While list2.Any() AndAlso list3.Any() AndAlso Operators.CompareString(list2.Last(), list3.Last(), False) = 0
                                list2.RemoveAt(list2.Count - 1)
                                list3.RemoveAt(list3.Count - 1)
                                flag = True
                            End While
                            If flag AndAlso list2.Any() AndAlso list3.Any() Then
                                text = list2.Join("-")
                                text2 = list3.Join("-")
                            End If
                            Dim text3 As String = ModBase.PathTemp + "mods\" + localResourceFile.File.Name.Replace(text, text2)
                            Dim text4 As String = Path.Combine(PathUtils.RemoveLastPart(localResourceFile.File.FullName), localResourceFile.File.Name.Replace(text, text2))
                            list.Add(updateFile.ToNetFile(text3, New ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.Update), PageInstanceLeft.Instance.Version.VanillaName, PageInstanceLeft.Instance.Version.ModLoaders))
                            CS$<>8__locals2.$VB$Local_FileCopyList(text3) = text4
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of LocalResourceFile)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                list4 = New List(Of ModLoader.LoaderBase)()
                CS$<>8__locals2.$VB$Local_FinishedFileNames = New List(Of String)()
                list4.Add(New ModNet.LoaderDownload("下载新版 Mod 文件", list) With { .ProgressWeight = CDbl(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ModList.Count()) * 1.5 })
                list4.Add(New ModLoader.LoaderTask(Of Integer, Integer)("替换旧版 Mod 文件", Sub(a0 As ModLoader.LoaderTask(Of Integer, Integer))
                    MyBase._Lambda$__0()
                End Sub, Nothing, ThreadPriority.Normal))
                CS$<>8__locals2.$VB$Local_Loader = New ModLoader.LoaderCombo(Of IEnumerable(Of LocalResourceFile))("Mod 更新：" + PageInstanceLeft.Instance.Name, list4)
                CS$<>8__locals2.$VB$Local_PathMods = PageInstanceLeft.Instance.PathIndie + "mods\"
                CS$<>8__locals2.$VB$Local_Loader.OnStateChanged = Sub(a0 As ModLoader.LoaderBase)
                    MyBase._Lambda$__1()
                End Sub
                Dim text5 As String = "开始更新 {0} 个 Mod：{1}"
                Dim obj As Object = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ModList.Count()
                Dim $VB$Local_ModList As IEnumerable(Of LocalResourceFile) = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ModList
                Dim func As Func(Of LocalResourceFile, String)
                If PageInstanceMod._Closure$__.$I39-3 IsNot Nothing Then
                    func = PageInstanceMod._Closure$__.$I39-3
                Else
                    Dim func2 As Func(Of LocalResourceFile, String) = Function(m As LocalResourceFile) m.File.Name
                    func = func2
                    PageInstanceMod._Closure$__.$I39-3 = func2
                End If
                Logger.Info(String.Format(text5, obj, $VB$Local_ModList.[Select](func).Join("，")), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
                PageInstanceMod.UpdatingInstanceModFolders.Add(CS$<>8__locals2.$VB$Local_PathMods)
                CS$<>8__locals2.$VB$Local_Loader.Start(Nothing, False)
                ModLoader.LoaderTaskbarAdd(Of IEnumerable(Of LocalResourceFile))(CS$<>8__locals2.$VB$Local_Loader)
                ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                ModMain.FrmMain.BtnExtraDownload.Ribble()
                Me.ReloadModList(True)
            Catch ex As Exception
                Logger.Warn(ex, "初始化 Mod 更新失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
            End Try
        End Sub

        Private Sub BtnSelectDelete_Click()
            Me.DeleteMods(LocalResourceLoaders.LocalResourceLoader.Output.Where(Function(m As LocalResourceFile) Me.SelectedMods.Contains(m.EnabledName)))
            Me.ChangeAllSelected(False)
        End Sub

        Private Sub DeleteMods(ModsToDelete As IEnumerable(Of LocalResourceFile))
            Try
                Dim flag As Boolean = True
                Dim shiftKeyDown As Boolean = MyWpfExtension.Computer.Keyboard.ShiftKeyDown
                Dim func As Func(Of LocalResourceFile, IEnumerable(Of String))
                If PageInstanceMod._Closure$__.$I41-0 IsNot Nothing Then
                    func = PageInstanceMod._Closure$__.$I41-0
                Else
                    Dim func2 As Func(Of LocalResourceFile, IEnumerable(Of String)) = Function(Target As LocalResourceFile)
                        Dim enumerable3 As IEnumerable(Of String)
                        If Target.IsEnabled Then
                            enumerable3 = New String() { Target.File.FullName, Target.File.FullName + If(FileUtils.Exists(Target.File.FullName + ".old"), ".old", ".disabled") }
                        Else
                            enumerable3 = New String() { Target.File.FullName, Target.EnabledFullName }
                        End If
                        Return enumerable3
                    End Function
                    func = func2
                    PageInstanceMod._Closure$__.$I41-0 = func2
                End If
                Dim enumerable As IEnumerable(Of String) = ModsToDelete.SelectMany(func).Distinct()
                Dim func3 As Func(Of String, Boolean)
                If PageInstanceMod._Closure$__.$I41-1 IsNot Nothing Then
                    func3 = PageInstanceMod._Closure$__.$I41-1
                Else
                    Dim func4 As Func(Of String, Boolean) = Function(m As String) FileUtils.Exists(m)
                    func3 = func4
                    PageInstanceMod._Closure$__.$I41-1 = func4
                End If
                Dim enumerable2 As IEnumerable(Of String) = enumerable.Where(func3)
                Dim func5 As Func(Of String, LocalResourceFile)
                If PageInstanceMod._Closure$__.$I41-2 IsNot Nothing Then
                    func5 = PageInstanceMod._Closure$__.$I41-2
                Else
                    Dim func6 As Func(Of String, LocalResourceFile) = Function(m As String) New LocalResourceFile(m)
                    func5 = func6
                    PageInstanceMod._Closure$__.$I41-2 = func6
                End If
                Dim list As List(Of LocalResourceFile) = enumerable2.[Select](func5).ToList()
                Try
                    Dim enumerator As List(Of LocalResourceFile).Enumerator = list.GetEnumerator()
                    While enumerator.MoveNext()
                        Dim CS$<>8__locals1 As PageInstanceMod._Closure$__41-0 = New PageInstanceMod._Closure$__41-0(CS$<>8__locals1)
                        CS$<>8__locals1.$VB$Local_ModEntity = enumerator.Current
                        Try
                            FileUtils.Delete(CS$<>8__locals1.$VB$Local_ModEntity.File.FullName, Not shiftKeyDown)
                        Catch ex As OperationCanceledException
                            Logger.Warn(ex, "删除 Mod 被主动取消", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
                            Me.ReloadModList(True)
                            Return
                        Catch ex2 As Exception
                            Logger.[Error](ex2, String.Format("删除 Mod 失败（{0}）", CS$<>8__locals1.$VB$Local_ModEntity.File.FullName), LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
                            flag = False
                        End Try
                        Me.SelectedMods.Remove(CS$<>8__locals1.$VB$Local_ModEntity.EnabledName)
                        LocalResourceLoaders.LocalResourceLoader.Output.Remove(CS$<>8__locals1.$VB$Local_ModEntity)
                        Dim searchResult As List(Of LocalResourceFile) = Me.SearchResult
                        If searchResult IsNot Nothing Then
                            searchResult.Remove(CS$<>8__locals1.$VB$Local_ModEntity)
                        End If
                        Me.ModItems.Remove(CS$<>8__locals1.$VB$Local_ModEntity.EnabledName)
                        Dim num As Integer = Me.PanList.Children.IndexOf(Me.PanList.Children.OfType().FirstOrDefault(Function(i As OurLocalModItem) i.Entry.Equals(CS$<>8__locals1.$VB$Local_ModEntity)))
                        If num >= 0 Then
                            Me.PanList.Children.RemoveAt(num)
                        End If
                    End While
                Finally
                    Dim enumerator As List(Of LocalResourceFile).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                Me.RefreshBars()
                If Not flag Then
                    ModMain.Hint("由于文件被占用，Mod 删除失败，请尝试关闭正在运行的游戏后再试！", ModMain.HintType.Red, True)
                    Me.ReloadModList(True)
                ElseIf Me.PanList.Children.Count = 0 Then
                    Me.ReloadModList(True)
                Else
                    Me.RefreshBars()
                End If
                If Not flag Then
                    Return
                End If
                If list.IsSingle() Then
                    ModMain.Hint(String.Format("已删除 {0}！", list.[Single]().File.Name), ModMain.HintType.Green, True)
                Else
                    ModMain.Hint(String.Format("已删除 {0} 个文件！", list.Count), ModMain.HintType.Green, True)
                End If
            Catch ex3 As OperationCanceledException
                Logger.Warn(ex3, "删除 Mod 被主动取消", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
                Me.ReloadModList(True)
            Catch ex4 As Exception
                Logger.[Error](ex4, "删除 Mod 出现未知错误", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
                Me.ReloadModList(True)
            End Try
            Me.LoaderRun(ModLoader.LoaderFolderRunType.UpdateOnly)
        End Sub

        Private Sub BtnSelectCancel_Click()
            Me.ChangeAllSelected(False)
        End Sub

        Public Sub Info_Click(sender As Object, e As EventArgs)
            ' The following expression was wrapped in a checked-statement
            Try
                Dim entry As LocalResourceFile = CType(If((TypeOf sender Is OurIconButton), NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), sender), OurLocalModItem).Entry
                If entry.Project IsNot Nothing Then
                    ModMain.FrmMain.PageChange(New FormMain.PageStackData() With { .Page = FormMain.PageType.ResourceDetail, .Additional = New Object() { entry.Project, New List(Of String)(), PageInstanceLeft.Instance.Version.VanillaName, PageInstanceLeft.Instance.Version.ModLoaders, ResourceTypes.[Mod] } }, FormMain.PageSubType.[Default])
                Else
                    Dim list As List(Of String) = New List(Of String)()
                    If entry.Description IsNot Nothing Then
                        list.Add(entry.Description + vbCrLf)
                    End If
                    list.Add(String.Concat(New String() { "文件：", entry.File.Name, "（", StringUtils.FormatByteSize(entry.File.Length), "）" }))
                    If entry.Version IsNot Nothing Then
                        list.Add("版本：" + entry.Version)
                    End If
                    Dim list2 As List(Of String) = New List(Of String)()
                    If list2.Any() Then
                        list.Add("")
                        list.AddRange(list2)
                    End If
                    Dim text As String = entry.Display.Replace(" ", "+")
                    Dim text2 As String = text.Substring(0, 1)
                    Dim num As Integer = text.Count() - 1
                    For i As Integer = 1 To num
                        Dim flag As Boolean = text(i - 1).ToString().Lower().Equals(text(i - 1).ToString())
                        Dim flag2 As Boolean = text(i).ToString().Lower().Equals(text(i).ToString())
                        If flag AndAlso Not flag2 Then
                            text2 += "+"
                        End If
                        text2 += Conversions.ToString(text(i))
                    Next
                    text2 = text2.Replace("++", "+").Replace("pti+Fine", "ptiFine")
                    If ModMain.OurMsgBox(list.Join(vbCrLf), entry.Display, "百科搜索", "返回", "", False, True, False, Nothing, Nothing, Nothing) = 1 Then
                        ModBase.OpenWebsite("https://www.mcmod.cn/s?key=" + text2 + "&site=all&filter=0")
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, "获取 Mod 详情失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
            End Try
        End Sub

        Public Sub Open_Click(sender As OurIconButton, e As EventArgs)
            Try
                ModBase.OpenExplorer(CType(sender.Tag, OurLocalModItem).Entry.File.FullName)
            Catch ex As Exception
                Logger.[Error](ex, "打开 Mod 文件位置失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceMod.xaml.vb")
            End Try
        End Sub

        Public Sub Delete_Click(sender As OurIconButton, e As EventArgs)
            Dim myLocalModItem As OurLocalModItem = CType(sender.Tag, OurLocalModItem)
            Me.DeleteMods(New LocalResourceFile() { myLocalModItem.Entry })
        End Sub

        Public Sub ED_Click(sender As OurIconButton, e As EventArgs)
            Dim myLocalModItem As OurLocalModItem = CType(sender.Tag, OurLocalModItem)
            Me.EDMods(New LocalResourceFile() { myLocalModItem.Entry }, Not myLocalModItem.Entry.IsEnabled)
        End Sub

        Public ReadOnly Property IsSearching As Boolean
            Get
                Return Not String.IsNullOrWhiteSpace(Me.SearchBox.Text)
            End Get
        End Property

        Public Sub SearchRun()
            If Me.IsSearching Then
                Dim list As List(Of ModBase.SearchEntry(Of LocalResourceFile)) = New List(Of ModBase.SearchEntry(Of LocalResourceFile))()
                Try
                    For Each localResourceFile As LocalResourceFile In LocalResourceLoaders.LocalResourceLoader.Output
                        Dim list2 As List(Of ModBase.SearchSource) = New List(Of ModBase.SearchSource)()
                        list2.Add(New ModBase.SearchSource(localResourceFile.Display, 1.0))
                        list2.Add(New ModBase.SearchSource(localResourceFile.File.Name, 1.0))
                        If localResourceFile.Version IsNot Nothing Then
                            list2.Add(New ModBase.SearchSource(localResourceFile.Version, 0.2))
                        End If
                        If localResourceFile.Description IsNot Nothing AndAlso Operators.CompareString(localResourceFile.Description, "", False) <> 0 Then
                            list2.Add(New ModBase.SearchSource(localResourceFile.Description, 0.4))
                        End If
                        If localResourceFile.Project IsNot Nothing Then
                            If Operators.CompareString(localResourceFile.Project.RawName, localResourceFile.Display, False) <> 0 Then
                                list2.Add(New ModBase.SearchSource(localResourceFile.Project.RawName, 1.0))
                            End If
                            If Operators.CompareString(localResourceFile.Project.TranslatedName, localResourceFile.Project.RawName, False) <> 0 Then
                                list2.Add(New ModBase.SearchSource(localResourceFile.Project.TranslatedName, 1.0))
                            End If
                            If Operators.CompareString(localResourceFile.Project.Description, localResourceFile.Description, False) <> 0 Then
                                list2.Add(New ModBase.SearchSource(localResourceFile.Project.Description, 0.4))
                            End If
                            list2.Add(New ModBase.SearchSource(String.Join("", localResourceFile.Project.Tags), 0.2))
                        End If
                        list.Add(New ModBase.SearchEntry(Of LocalResourceFile)() With { .Item = localResourceFile, .SearchSource = list2 })
                    Next
                Finally
                    Dim enumerator As List(Of LocalResourceFile).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                Dim enumerable As IEnumerable(Of ModBase.SearchEntry(Of LocalResourceFile)) = ModBase.Search(Of LocalResourceFile)(list, Me.SearchBox.Text, 6, 0.35)
                Dim func As Func(Of ModBase.SearchEntry(Of LocalResourceFile), LocalResourceFile)
                If PageInstanceMod._Closure$__.$I50-0 IsNot Nothing Then
                    func = PageInstanceMod._Closure$__.$I50-0
                Else
                    Dim func2 As Func(Of ModBase.SearchEntry(Of LocalResourceFile), LocalResourceFile) = Function(r As ModBase.SearchEntry(Of LocalResourceFile)) r.Item
                    func = func2
                    PageInstanceMod._Closure$__.$I50-0 = func2
                End If
                Me.SearchResult = enumerable.[Select](func).ToList()
            End If
            Me.RefreshUI()
        End Sub

        Friend Overridable Property PanAllBack As Grid

        Friend Overridable Property PanBack As OurScrollViewer

        Friend Overridable Property PanMain As StackPanel

        Friend Overridable Property SearchBox As OurSearchBox
            <CompilerGenerated()>
            Get
                Return Me._SearchBox
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurSearchBox)
                Dim keyEventHandler As KeyEventHandler = AddressOf Me.SearchBox_PreviewKeyDown
                Dim textChangedEventHandler As OurSearchBox.TextChangedEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.SearchRun()
                End Sub
                Dim mySearchBox As OurSearchBox = Me._SearchBox
                If mySearchBox IsNot Nothing Then
                    RemoveHandler mySearchBox.PreviewKeyDown, keyEventHandler
                    RemoveHandler mySearchBox.TextChanged, textChangedEventHandler
                End If
                Me._SearchBox = value
                mySearchBox = Me._SearchBox
                If mySearchBox IsNot Nothing Then
                    AddHandler mySearchBox.PreviewKeyDown, keyEventHandler
                    AddHandler mySearchBox.TextChanged, textChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanManage As OurCard

        Friend Overridable Property BtnManageOpen As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnManageOpen
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnManageOpen_Click
                Dim myButton As OurButton = Me._BtnManageOpen
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnManageOpen = value
                myButton = Me._BtnManageOpen
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnManageInstall As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnManageInstall
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnManageInstall_Click
                Dim myButton As OurButton = Me._BtnManageInstall
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnManageInstall = value
                myButton = Me._BtnManageInstall
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnManageDownload As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnManageDownload
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnManageDownload_Click
                Dim myButton As OurButton = Me._BtnManageDownload
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnManageDownload = value
                myButton = Me._BtnManageDownload
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnManageSelectAll As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnManageSelectAll
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnManageSelectAll_Click
                Dim myButton As OurButton = Me._BtnManageSelectAll
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnManageSelectAll = value
                myButton = Me._BtnManageSelectAll
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanListBack As OurCard

        Friend Overridable Property PanFilter As StackPanel

        Friend Overridable Property BtnFilterAll As OurRadioButton
            <CompilerGenerated()>
            Get
                Return Me._BtnFilterAll
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurRadioButton)
                Dim checkEventHandler As OurRadioButton.CheckEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.ChangeFilter(CType(a0, OurRadioButton), a1)
                End Sub
                Dim myRadioButton As OurRadioButton = Me._BtnFilterAll
                If myRadioButton IsNot Nothing Then
                    RemoveHandler myRadioButton.Check, checkEventHandler
                End If
                Me._BtnFilterAll = value
                myRadioButton = Me._BtnFilterAll
                If myRadioButton IsNot Nothing Then
                    AddHandler myRadioButton.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnFilterEnabled As OurRadioButton
            <CompilerGenerated()>
            Get
                Return Me._BtnFilterEnabled
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurRadioButton)
                Dim checkEventHandler As OurRadioButton.CheckEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.ChangeFilter(CType(a0, OurRadioButton), a1)
                End Sub
                Dim myRadioButton As OurRadioButton = Me._BtnFilterEnabled
                If myRadioButton IsNot Nothing Then
                    RemoveHandler myRadioButton.Check, checkEventHandler
                End If
                Me._BtnFilterEnabled = value
                myRadioButton = Me._BtnFilterEnabled
                If myRadioButton IsNot Nothing Then
                    AddHandler myRadioButton.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnFilterDisabled As OurRadioButton
            <CompilerGenerated()>
            Get
                Return Me._BtnFilterDisabled
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurRadioButton)
                Dim checkEventHandler As OurRadioButton.CheckEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.ChangeFilter(CType(a0, OurRadioButton), a1)
                End Sub
                Dim myRadioButton As OurRadioButton = Me._BtnFilterDisabled
                If myRadioButton IsNot Nothing Then
                    RemoveHandler myRadioButton.Check, checkEventHandler
                End If
                Me._BtnFilterDisabled = value
                myRadioButton = Me._BtnFilterDisabled
                If myRadioButton IsNot Nothing Then
                    AddHandler myRadioButton.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnFilterCanUpdate As OurRadioButton
            <CompilerGenerated()>
            Get
                Return Me._BtnFilterCanUpdate
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurRadioButton)
                Dim checkEventHandler As OurRadioButton.CheckEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.ChangeFilter(CType(a0, OurRadioButton), a1)
                End Sub
                Dim myRadioButton As OurRadioButton = Me._BtnFilterCanUpdate
                If myRadioButton IsNot Nothing Then
                    RemoveHandler myRadioButton.Check, checkEventHandler
                End If
                Me._BtnFilterCanUpdate = value
                myRadioButton = Me._BtnFilterCanUpdate
                If myRadioButton IsNot Nothing Then
                    AddHandler myRadioButton.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanList As StackPanel

        Friend Overridable Property PanEmpty As OurCard

        Friend Overridable Property BtnHintDownload As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnHintDownload
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnManageDownload_Click
                Dim myButton As OurButton = Me._BtnHintDownload
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnHintDownload = value
                myButton = Me._BtnHintDownload
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnHintInstall As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnHintInstall
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnManageInstall_Click
                Dim myButton As OurButton = Me._BtnHintInstall
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnHintInstall = value
                myButton = Me._BtnHintInstall
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnHintOpen As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnHintOpen
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnManageOpen_Click
                Dim myButton As OurButton = Me._BtnHintOpen
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnHintOpen = value
                myButton = Me._BtnHintOpen
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanLoad As OurCard

        Friend Overridable Property Load As OurLoading
            <CompilerGenerated()>
            Get
                Return Me._Load
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurLoading)
                Dim clickEventHandler As OurLoading.ClickEventHandler = AddressOf Me.Load_Click
                Dim stateChangedEventHandler As OurLoading.StateChangedEventHandler = Sub(a0 As Object, a1 As OurLoading.OurLoadingState, a2 As OurLoading.OurLoadingState)
                    Me.UnselectedAllWithAnimation()
                End Sub
                Dim myLoading As OurLoading = Me._Load
                If myLoading IsNot Nothing Then
                    RemoveHandler myLoading.Click, clickEventHandler
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler
                End If
                Me._Load = value
                myLoading = Me._Load
                If myLoading IsNot Nothing Then
                    AddHandler myLoading.Click, clickEventHandler
                    AddHandler myLoading.StateChanged, stateChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CardSelect As OurCard

        Friend Overridable Property TransSelect As TranslateTransform

        Friend Overridable Property LabSelect As TextBlock

        Friend Overridable Property BtnSelectUpdate As OurIconTextButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSelectUpdate
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconTextButton)
                Dim clickEventHandler As OurIconTextButton.ClickEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.BtnSelectUpdate_Click()
                End Sub
                Dim myIconTextButton As OurIconTextButton = Me._BtnSelectUpdate
                If myIconTextButton IsNot Nothing Then
                    RemoveHandler myIconTextButton.Click, clickEventHandler
                End If
                Me._BtnSelectUpdate = value
                myIconTextButton = Me._BtnSelectUpdate
                If myIconTextButton IsNot Nothing Then
                    AddHandler myIconTextButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnSelectEnable As OurIconTextButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSelectEnable
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconTextButton)
                Dim clickEventHandler As OurIconTextButton.ClickEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.BtnSelectED_Click(CType(a0, OurIconTextButton), a1)
                End Sub
                Dim myIconTextButton As OurIconTextButton = Me._BtnSelectEnable
                If myIconTextButton IsNot Nothing Then
                    RemoveHandler myIconTextButton.Click, clickEventHandler
                End If
                Me._BtnSelectEnable = value
                myIconTextButton = Me._BtnSelectEnable
                If myIconTextButton IsNot Nothing Then
                    AddHandler myIconTextButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnSelectDisable As OurIconTextButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSelectDisable
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconTextButton)
                Dim clickEventHandler As OurIconTextButton.ClickEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.BtnSelectED_Click(CType(a0, OurIconTextButton), a1)
                End Sub
                Dim myIconTextButton As OurIconTextButton = Me._BtnSelectDisable
                If myIconTextButton IsNot Nothing Then
                    RemoveHandler myIconTextButton.Click, clickEventHandler
                End If
                Me._BtnSelectDisable = value
                myIconTextButton = Me._BtnSelectDisable
                If myIconTextButton IsNot Nothing Then
                    AddHandler myIconTextButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnSelectDelete As OurIconTextButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSelectDelete
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconTextButton)
                Dim clickEventHandler As OurIconTextButton.ClickEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.BtnSelectDelete_Click()
                End Sub
                Dim myIconTextButton As OurIconTextButton = Me._BtnSelectDelete
                If myIconTextButton IsNot Nothing Then
                    RemoveHandler myIconTextButton.Click, clickEventHandler
                End If
                Me._BtnSelectDelete = value
                myIconTextButton = Me._BtnSelectDelete
                If myIconTextButton IsNot Nothing Then
                    AddHandler myIconTextButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnSelectCancel As OurIconTextButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSelectCancel
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconTextButton)
                Dim clickEventHandler As OurIconTextButton.ClickEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.BtnSelectCancel_Click()
                End Sub
                Dim myIconTextButton As OurIconTextButton = Me._BtnSelectCancel
                If myIconTextButton IsNot Nothing Then
                    RemoveHandler myIconTextButton.Click, clickEventHandler
                End If
                Me._BtnSelectCancel = value
                myIconTextButton = Me._BtnSelectCancel
                If myIconTextButton IsNot Nothing Then
                    AddHandler myIconTextButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Private IsLoad As Boolean

        Public ModItems As ConcurrentDictionary(Of String, OurLocalModItem)

        Private BottomBarShownCount As Integer

        Public SelectedMods As List(Of String)

        Private _Filter As PageInstanceMod.FilterType

        Public Shared UpdatingInstanceModFolders As List(Of String) = New List(Of String)()

        Private SearchResult As List(Of LocalResourceFile)

        Public Enum FilterType
            All
            Enabled
            Disabled
            CanUpdate
        End Enum
    End Class
End Namespace
