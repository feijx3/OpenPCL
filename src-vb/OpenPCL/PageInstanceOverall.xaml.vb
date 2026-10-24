Imports System
Imports System.CodeDom.Compiler
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports MeloongCore
Imports MeloongCore.Extensions
Imports MeloongCore.Wpf
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports PCL.My

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageInstanceOverall
        Inherits OurPageRight

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageSetupLaunch_Loaded
            Me.IsLoad = False
            Me.InitializeComponent()
        End Sub

        Private Sub PageSetupLaunch_Loaded(sender As Object, e As RoutedEventArgs)
            Me.PanBack.ScrollToHome()
            Me.Reload()
            If Not Me.IsLoad Then
                Me.IsLoad = True
                Me.PanDisplay.TriggerForceResize()
            End If
        End Sub

        Private Sub Reload()
            ' The following expression was wrapped in a checked-statement
            ModAnimation.AniControlEnabled += 1
            Me.ComboDisplayType.SelectedIndex = Conversions.ToInteger(ModBase.ReadIni(PageInstanceLeft.Instance.PathVersion + "PCL\Setup.ini", "DisplayType", Conversions.ToString(0)))
            Me.BtnDisplayStar.Text = If(PageInstanceLeft.Instance.IsStar, "从收藏夹中移除", "加入收藏夹")
            Me.BtnFolderMods.Visibility = If(PageInstanceLeft.Instance.Modable, Visibility.Visible, Visibility.Collapsed)
            Me.PanDisplayItem.Children.Clear()
            Me.InstanceHeaderItem = PageInstanceLeft.Instance.ToListItem(Nothing).Init()
            Me.InstanceHeaderItem.IsHitTestVisible = False
            Me.PanDisplayItem.Children.Add(Me.InstanceHeaderItem)
            ModMain.FrmMain.PageNameRefresh()
            Me.ComboDisplayLogo.SelectedIndex = 0
            Dim text As String = ModBase.ReadIni(PageInstanceLeft.Instance.PathVersion + "PCL\Setup.ini", "Logo", "")
            If Conversions.ToBoolean(ModBase.ReadIni(PageInstanceLeft.Instance.PathVersion + "PCL\Setup.ini", "LogoCustom", "False")) Then
                Try
                    For Each obj As Object In CType(Me.ComboDisplayLogo.Items, IEnumerable)
                        Dim myComboBoxItem As OurComboBoxItem = CType(obj, OurComboBoxItem)
                        If Operators.ConditionalCompareObjectEqual(myComboBoxItem.Tag, text, False) OrElse (Operators.ConditionalCompareObjectEqual(myComboBoxItem.Tag, "PCL\Logo.png", False) AndAlso text.EndsWithF("PCL\Logo.png", False)) Then
                            Me.ComboDisplayLogo.SelectedItem = myComboBoxItem
                            Exit For
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator
                    If TypeOf enumerator Is IDisposable Then
                        TryCast(enumerator, IDisposable).Dispose()
                    End If
                End Try
            End If
            ModAnimation.AniControlEnabled -= 1
        End Sub

        Private Sub ComboDisplayType_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If Me.IsLoad AndAlso ModAnimation.AniControlEnabled = 0 Then
                If Me.ComboDisplayType.SelectedIndex <> 1 Then
                    Try
                        ModBase.WriteIni(PageInstanceLeft.Instance.PathVersion + "PCL\Setup.ini", "DisplayType", Conversions.ToString(Me.ComboDisplayType.SelectedIndex))
                        PageInstanceLeft.Instance.DisplayType = CType(Conversions.ToInteger(ModBase.ReadIni(PageInstanceLeft.Instance.PathVersion + "PCL\Setup.ini", "DisplayType", Conversions.ToString(0))), ModMinecraft.McInstanceCardType)
                        ModMain.FrmInstanceLeft.RefreshModDisabled()
                        ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "InstanceCache", "")
                        ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
                    Catch ex As Exception
                        Logger.[Error](ex, String.Format("修改版本分类失败（{0}）", PageInstanceLeft.Instance.Name), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceOverall.xaml.vb")
                    End Try
                    Me.Reload()
                    Return
                End If
                Try
                    If Not Settings.[Get](Of Boolean)("HintHide", Nothing) Then
                        If ModMain.OurMsgBox("确认要从版本列表中隐藏该版本吗？隐藏该版本后，它将不再出现于 PCL 显示的版本列表中。" & vbCrLf & "此后，在版本列表页面按下 F11 才可以查看被隐藏的版本。", "隐藏版本提示", "确定", "取消", "", False, True, False, Nothing, Nothing, Nothing) <> 1 Then
                            Me.ComboDisplayType.SelectedIndex = 0
                            Return
                        End If
                        Settings.[Set]("HintHide", True, Nothing)
                    End If
                    ModBase.WriteIni(PageInstanceLeft.Instance.PathVersion + "PCL\Setup.ini", "DisplayType", Conversions.ToString(1))
                    ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "InstanceCache", "")
                    ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
                Catch ex2 As Exception
                    Logger.[Error](ex2, String.Format("隐藏版本 {0} 失败", PageInstanceLeft.Instance.Name), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceOverall.xaml.vb")
                End Try
            End If
        End Sub

        Private Sub BtnDisplayDesc_Click(sender As Object, e As EventArgs)
            Try
                Dim text As String = ModBase.ReadIni(PageInstanceLeft.Instance.PathVersion + "PCL\Setup.ini", "CustomInfo", "")
                Dim text2 As String = ModMain.OurMsgBoxInput("更改描述", "修改版本的描述文本，留空则使用 PCL 的默认描述。", text, New Collection(Of Validate)(), "默认描述", "确定", "取消", False)
                If text2 IsNot Nothing AndAlso Operators.CompareString(text, text2, False) <> 0 Then
                    ModBase.WriteIni(PageInstanceLeft.Instance.PathVersion + "PCL\Setup.ini", "CustomInfo", text2)
                End If
                PageInstanceLeft.Instance = New ModMinecraft.McInstance(PageInstanceLeft.Instance.Name).Load()
                Me.Reload()
                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
            Catch ex As Exception
                Logger.[Error](ex, String.Format("版本 {0} 描述更改失败", PageInstanceLeft.Instance.Name), LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceOverall.xaml.vb")
            End Try
        End Sub

        Private Sub BtnDisplayRename_Click(sender As Object, e As EventArgs)
            Try
                If ModMinecraft.McInstanceLock.IsLocked(PageInstanceLeft.Instance) Then
                    ModMain.Hint("该版本正在被修改，请稍后再重命名！", ModMain.HintType.Red, True)
                Else
                    Dim name As String = PageInstanceLeft.Instance.Name
                    Dim text As String = ModMain.OurMsgBoxInput("重命名版本", "", name, New Collection(Of Validate)() From { New ValidateFolderName(ModMinecraft.McFolderSelected + "versions", True, New List(Of String)() From { name }) }, "", "确定", "取消", False)
                    If Not String.IsNullOrWhiteSpace(text) Then
                        Dim mcInstance As ModMinecraft.McInstance = PageInstanceLeft.Instance.Rename(text)
                        ModMain.Hint("重命名成功！", ModMain.HintType.Green, True)
                        PageInstanceLeft.Instance = mcInstance
                        If Not Information.IsNothing(ModMinecraft.McInstanceSelected) AndAlso ModMinecraft.McInstanceSelected.Equals(PageInstanceLeft.Instance) Then
                            ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "Version", text)
                        End If
                        Me.Reload()
                        ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, "重命名版本失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceOverall.xaml.vb")
            End Try
        End Sub

        Private Sub ComboDisplayLogo_SelectionChanged()
            If Me.IsLoad AndAlso ModAnimation.AniControlEnabled = 0 Then
                Try
                    If Me.ComboDisplayLogo.SelectedItem Is Me.ItemDisplayLogoCustom Then
                        Dim text As String = Dialogs.SelectFile("选择图片", False, Nothing, New OneOf(Of IEnumerable(Of ValueTuple(Of String(), String)), String)?(New ValueTuple(Of String(), String)() { New ValueTuple(Of String(), String)(New String() { "png", "jpeg", "jpg", "gif", "webp" }, "常用图片文件") })).FirstOrDefault()
                        If String.IsNullOrEmpty(text) Then
                            Me.Reload()
                            Return
                        End If
                        FileUtils.Copy(text, PageInstanceLeft.Instance.PathVersion + "PCL\Logo.png")
                    Else
                        FileUtils.Delete(PageInstanceLeft.Instance.PathVersion + "PCL\Logo.png", False)
                    End If
                Catch ex As Exception
                    Logger.[Error](ex, String.Format("更改自定义版本图标失败（{0}）", PageInstanceLeft.Instance.Name), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceOverall.xaml.vb")
                End Try
                Try
                    Dim text2 As String = Conversions.ToString(NewLateBinding.LateGet(Me.ComboDisplayLogo.SelectedItem, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing))
                    ModBase.WriteIni(PageInstanceLeft.Instance.PathVersion + "PCL\Setup.ini", "Logo", text2)
                    ModBase.WriteIni(PageInstanceLeft.Instance.PathVersion + "PCL\Setup.ini", "LogoCustom", Conversions.ToString(Operators.CompareString(text2, "", False) <> 0))
                    ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "InstanceCache", "")
                    PageInstanceLeft.Instance = New ModMinecraft.McInstance(PageInstanceLeft.Instance.Name).Load()
                    Me.Reload()
                    ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
                Catch ex2 As Exception
                    Logger.[Error](ex2, String.Format("更改版本图标失败（{0}）", PageInstanceLeft.Instance.Name), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceOverall.xaml.vb")
                End Try
            End If
        End Sub

        Private Sub BtnDisplayStar_Click(sender As Object, e As EventArgs)
            Try
                ModBase.WriteIni(PageInstanceLeft.Instance.PathVersion + "PCL\Setup.ini", "IsStar", Conversions.ToString(Not PageInstanceLeft.Instance.IsStar))
                PageInstanceLeft.Instance = New ModMinecraft.McInstance(PageInstanceLeft.Instance.Name).Load()
                Me.Reload()
                ModMinecraft.McInstanceListForceRefresh = True
                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
            Catch ex As Exception
                Logger.[Error](ex, String.Format("版本 {0} 收藏状态更改失败", PageInstanceLeft.Instance.Name), LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceOverall.xaml.vb")
            End Try
        End Sub

        Private Sub BtnFolderVersion_Click()
            PageInstanceOverall.OpenInstanceFolder(PageInstanceLeft.Instance)
        End Sub

        Public Shared Sub OpenInstanceFolder(Instance As ModMinecraft.McInstance)
            ModBase.OpenExplorer(Instance.PathVersion)
        End Sub

        Private Sub BtnFolderSaves_Click()
            Dim text As String = PageInstanceLeft.Instance.PathIndie + "saves\"
            DirectoryUtils.Create(text)
            ModBase.OpenExplorer(text)
        End Sub

        Private Sub BtnFolderMods_Click()
            Dim text As String = PageInstanceLeft.Instance.PathIndie + "mods\"
            DirectoryUtils.Create(text)
            ModBase.OpenExplorer(text)
        End Sub

        Private Sub BtnFolderScreen_Click(sender As Object, e As MouseButtonEventArgs)
            Dim text As String = PageInstanceLeft.Instance.PathIndie + "screenshots\"
            DirectoryUtils.Create(text)
            ModBase.OpenExplorer(text)
        End Sub

        Private Sub BtnManageScript_Click()
            Try
                Dim text As String = Dialogs.SaveFile("选择脚本保存位置", "启动 " + PageInstanceLeft.Instance.Name + ".bat", Nothing, New ValueTuple(Of String, String)() { New ValueTuple(Of String, String)("bat", "批处理文件") })
                If text IsNot Nothing Then
                    If ModLaunch.McLaunchLoader.State = ModBase.LoadState.Loading Then
                        ModMain.Hint("请在当前启动任务结束后再试！", ModMain.HintType.Red, True)
                    ElseIf ModLaunch.McLaunchStart(New ModLaunch.McLaunchOptions() With { .SaveBatch = text, .Instance = PageInstanceLeft.Instance }) Then
                        If Settings.[Get](Of ModLaunch.McLoginType)("LoginType", Nothing) = ModLaunch.McLoginType.Legacy Then
                            ModMain.Hint("正在导出启动脚本……", ModMain.HintType.Blue, True)
                        Else
                            ModMain.Hint("正在导出启动脚本……（注意，使用脚本启动可能会导致登录失效！）", ModMain.HintType.Blue, True)
                        End If
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, String.Format("导出启动脚本失败（{0}）", PageInstanceLeft.Instance.Name), LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceOverall.xaml.vb")
            End Try
        End Sub

        Private Sub BtnManageCheck_Click(sender As Object, e As EventArgs)
            Try
                Dim CS$<>8__locals1 As PageInstanceOverall._Closure$__16-0 = New PageInstanceOverall._Closure$__16-0(CS$<>8__locals1)
                If ModMinecraft.McInstanceLock.IsLocked(PageInstanceLeft.Instance) Then
                    ModMain.Hint("该版本正在被修改，请稍后再补全文件！", ModMain.HintType.Red, True)
                ElseIf Conversions.ToBoolean(ModMinecraft.ShouldIgnoreFileCheck(PageInstanceLeft.Instance)) Then
                    ModMain.Hint("请先关闭 [版本设置 → 设置 → 高级启动选项 → 关闭文件校验]，然后再尝试补全文件！", ModMain.HintType.Blue, True)
                Else
                    Try
                        Dim enumerator As IEnumerator(Of ModLoader.LoaderBase) = ModLoader.LoaderTaskbar.GetEnumerator()
                        While enumerator.MoveNext()
                            If Operators.CompareString(enumerator.Current.Name, PageInstanceLeft.Instance.Name + " 文件补全", False) = 0 Then
                                ModMain.Hint("正在处理中，请稍候！", ModMain.HintType.Red, True)
                                Return
                            End If
                        End While
                    Finally
                        Dim enumerator As IEnumerator(Of ModLoader.LoaderBase)
                        If enumerator IsNot Nothing Then
                            enumerator.Dispose()
                        End If
                    End Try
                    CS$<>8__locals1.$VB$Local_Loader = New ModLoader.LoaderCombo(Of String)(PageInstanceLeft.Instance.Name + " 文件补全", ModDownload.DlClientFix(PageInstanceLeft.Instance, True, False))
                    CS$<>8__locals1.$VB$Local_Loader.OnStateChanged = Sub(a0 As ModLoader.LoaderBase)
                        MyBase._Lambda$__0()
                    End Sub
                    CS$<>8__locals1.$VB$Local_Loader.Start(PageInstanceLeft.Instance.Name, False)
                    ModLoader.LoaderTaskbarAdd(Of String)(CS$<>8__locals1.$VB$Local_Loader)
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                    ModMain.FrmMain.BtnExtraDownload.Ribble()
                End If
            Catch ex As Exception
                Logger.[Error](ex, String.Format("尝试补全文件失败（{0}）", PageInstanceLeft.Instance.Name), LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceOverall.xaml.vb")
            End Try
        End Sub

        Private Sub BtnManageDelete_Click(sender As Object, e As EventArgs)
            Try
                If ModMinecraft.McInstanceLock.IsLocked(PageInstanceLeft.Instance) Then
                    ModMain.Hint("该版本正在被修改，请稍后再删除！", ModMain.HintType.Red, True)
                Else
                    Dim shiftKeyDown As Boolean = MyWpfExtension.Computer.Keyboard.ShiftKeyDown
                    Dim flag As Boolean = PageInstanceLeft.Instance.State <> ModMinecraft.McInstance.McInstanceState.[Error] AndAlso Operators.CompareString(PageInstanceLeft.Instance.PathIndie, ModMinecraft.McFolderSelected, False) <> 0
                    Dim info As DirectoryInfo = DirectoryUtils.GetInfo(PageInstanceLeft.Instance.PathIndie + "saves\")
                    Dim list As List(Of String)
                    If Not flag OrElse Not info.Exists Then
                        list = New List(Of String)()
                    Else
                        Dim enumerable As IEnumerable(Of DirectoryInfo) = info.EnumerateDirectories()
                        Dim func As Func(Of DirectoryInfo, DateTime)
                        If PageInstanceOverall._Closure$__.$I17-0 IsNot Nothing Then
                            func = PageInstanceOverall._Closure$__.$I17-0
                        Else
                            Dim func2 As Func(Of DirectoryInfo, DateTime) = Function(Save As DirectoryInfo) Save.LastWriteTime
                            func = func2
                            PageInstanceOverall._Closure$__.$I17-0 = func2
                        End If
                        Dim enumerable2 As IEnumerable(Of DirectoryInfo) = enumerable.OrderByDescending(func)
                        Dim func3 As Func(Of DirectoryInfo, String)
                        If PageInstanceOverall._Closure$__.$I17-1 IsNot Nothing Then
                            func3 = PageInstanceOverall._Closure$__.$I17-1
                        Else
                            Dim func4 As Func(Of DirectoryInfo, String) = Function(Save As DirectoryInfo) String.Format("{0}（上次修改：{1}）", Save.Name, StringUtils.FormatTimeSpan(Save.LastWriteTime - DateTime.Now, False))
                            func3 = func4
                            PageInstanceOverall._Closure$__.$I17-1 = func4
                        End If
                        list = enumerable2.[Select](func3).ToList()
                    End If
                    Dim list2 As List(Of String) = list
                    Dim num As Integer = ModMain.OurMsgBox(String.Format("你确定要{0}删除版本 {1} 吗？", If(shiftKeyDown, "永久", ""), PageInstanceLeft.Instance.Name) + If(flag, (vbCrLf & "该版本对应的存档、资源包、Mod 等文件也将被一并删除！" + If(list2.Any(), (vbCrLf & vbCrLf & "这会删除以下存档：" & vbCrLf & "· " + list2.Join(vbCrLf & "· ")), "")), ""), "版本删除确认", "确定", "取消", "", flag OrElse shiftKeyDown, True, False, Nothing, Nothing, Nothing)
                    If num <> 1 Then
                        If num = 2 Then
                            Return
                        End If
                    Else
                        PageInstanceLeft.Instance.ResetSettingsCache()
                        DirectoryUtils.Delete(PageInstanceLeft.Instance.PathVersion, Not shiftKeyDown)
                        ModMain.Hint("已删除版本 " + PageInstanceLeft.Instance.Name + "！", ModMain.HintType.Green, True)
                    End If
                    ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
                    ModMain.FrmMain.PageBack()
                End If
            Catch ex As OperationCanceledException
                Logger.Warn(ex, String.Format("删除版本 {0} 被主动取消", PageInstanceLeft.Instance.Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceOverall.xaml.vb")
            Catch ex2 As Exception
                Logger.[Error](ex2, String.Format("删除版本 {0} 失败", PageInstanceLeft.Instance.Name), LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceOverall.xaml.vb")
            End Try
        End Sub

        Friend Overridable Property PanBack As OurScrollViewer

        Friend Overridable Property PanMain As StackPanel

        Friend Overridable Property PanDisplayItem As Grid

        Friend Overridable Property PanDisplay As OurCard

        Friend Overridable Property PanDisplayIcon As Grid

        Friend Overridable Property ComboDisplayLogo As OurComboBox
            <CompilerGenerated()>
            Get
                Return Me._ComboDisplayLogo
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurComboBox)
                Dim selectionChangedEventHandler As SelectionChangedEventHandler = Sub(a0 As Object, a1 As SelectionChangedEventArgs)
                    Me.ComboDisplayLogo_SelectionChanged()
                End Sub
                Dim myComboBox As OurComboBox = Me._ComboDisplayLogo
                If myComboBox IsNot Nothing Then
                    RemoveHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
                Me._ComboDisplayLogo = value
                myComboBox = Me._ComboDisplayLogo
                If myComboBox IsNot Nothing Then
                    AddHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemDisplayLogoCustom As OurComboBoxItem
            <CompilerGenerated()>
            Get
                Return Me._ItemDisplayLogoCustom
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurComboBoxItem)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.ComboDisplayLogo_SelectionChanged()
                End Sub
                Dim myComboBoxItem As OurComboBoxItem = Me._ItemDisplayLogoCustom
                If myComboBoxItem IsNot Nothing Then
                    RemoveHandler myComboBoxItem.PreviewMouseLeftButtonUp, mouseButtonEventHandler
                End If
                Me._ItemDisplayLogoCustom = value
                myComboBoxItem = Me._ItemDisplayLogoCustom
                If myComboBoxItem IsNot Nothing Then
                    AddHandler myComboBoxItem.PreviewMouseLeftButtonUp, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ComboDisplayType As OurComboBox
            <CompilerGenerated()>
            Get
                Return Me._ComboDisplayType
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurComboBox)
                Dim selectionChangedEventHandler As SelectionChangedEventHandler = AddressOf Me.ComboDisplayType_SelectionChanged
                Dim myComboBox As OurComboBox = Me._ComboDisplayType
                If myComboBox IsNot Nothing Then
                    RemoveHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
                Me._ComboDisplayType = value
                myComboBox = Me._ComboDisplayType
                If myComboBox IsNot Nothing Then
                    AddHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnDisplayRename As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnDisplayRename
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnDisplayRename_Click
                Dim myButton As OurButton = Me._BtnDisplayRename
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnDisplayRename = value
                myButton = Me._BtnDisplayRename
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnDisplayDesc As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnDisplayDesc
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnDisplayDesc_Click
                Dim myButton As OurButton = Me._BtnDisplayDesc
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnDisplayDesc = value
                myButton = Me._BtnDisplayDesc
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnDisplayStar As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnDisplayStar
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnDisplayStar_Click
                Dim myButton As OurButton = Me._BtnDisplayStar
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnDisplayStar = value
                myButton = Me._BtnDisplayStar
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanFolder As OurCard

        Friend Overridable Property BtnFolderVersion As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnFolderVersion
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.BtnFolderVersion_Click()
                End Sub
                Dim myButton As OurButton = Me._BtnFolderVersion
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnFolderVersion = value
                myButton = Me._BtnFolderVersion
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnFolderSaves As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnFolderSaves
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.BtnFolderSaves_Click()
                End Sub
                Dim myButton As OurButton = Me._BtnFolderSaves
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnFolderSaves = value
                myButton = Me._BtnFolderSaves
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnFolderMods As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnFolderMods
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.BtnFolderMods_Click()
                End Sub
                Dim myButton As OurButton = Me._BtnFolderMods
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnFolderMods = value
                myButton = Me._BtnFolderMods
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnFolderScreen As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnFolderScreen
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnFolderScreen_Click
                Dim myButton As OurButton = Me._BtnFolderScreen
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnFolderScreen = value
                myButton = Me._BtnFolderScreen
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanManage As OurCard

        Friend Overridable Property BtnManageScript As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnManageScript
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.BtnManageScript_Click()
                End Sub
                Dim myButton As OurButton = Me._BtnManageScript
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnManageScript = value
                myButton = Me._BtnManageScript
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnManageCheck As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnManageCheck
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnManageCheck_Click
                Dim myButton As OurButton = Me._BtnManageCheck
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnManageCheck = value
                myButton = Me._BtnManageCheck
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnManageDelete As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnManageDelete
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnManageDelete_Click
                Dim myButton As OurButton = Me._BtnManageDelete
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnManageDelete = value
                myButton = Me._BtnManageDelete
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Private IsLoad As Boolean

        Public InstanceHeaderItem As OurListItem
    End Class
End Namespace
