Imports System
Imports System.CodeDom.Compiler
Imports System.Collections
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
Imports MeloongCore
Imports MeloongCore.Extensions
Imports MeloongCore.Wpf
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageSetupUI
        Inherits MyPageRight

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageSetupUI_Loaded
            Dim routedEventHandler As RoutedEventHandler
            If PageSetupUI._Closure$__.$IR1-1 IsNot Nothing Then
                routedEventHandler = PageSetupUI._Closure$__.$IR1-1
            Else
                Dim routedEventHandler2 As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    PageSetupUI.HiddenRefresh()
                End Sub
                routedEventHandler = routedEventHandler2
                PageSetupUI._Closure$__.$IR1-1 = routedEventHandler2
            End If
            AddHandler MyBase.Loaded, routedEventHandler
            Me.InitializeComponent()
        End Sub

        Private Sub PageSetupUI_Loaded(sender As Object, e As RoutedEventArgs)
            Me.PanBack.ScrollToHome()
            ModSecret.ThemeCheckAll(True)
            If ModSecret.ThemeDontClick <> 0 Then
                Dim text As String = Nothing
                Dim themeDontClick As Integer = ModSecret.ThemeDontClick
                If themeDontClick <> 1 Then
                    If themeDontClick = 2 Then
                        text = "真·滑稽彩"
                    End If
                Else
                    text = "眼瞎白"
                End If
                Try
                    For Each obj As Object In Me.PanLauncherTheme.Children
                        Dim objectValue As Object = RuntimeHelpers.GetObjectValue(obj)
                        If TypeOf objectValue Is MyRadioBox AndAlso CType(objectValue, MyRadioBox).IsEnabled Then
                            CType(objectValue, MyRadioBox).Text = text
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator
                    If TypeOf enumerator Is IDisposable Then
                        TryCast(enumerator, IDisposable).Dispose()
                    End If
                End Try
            End If
            ModAnimation.AniControlEnabled += 1
            Me.Refresh()
            ModAnimation.AniControlEnabled -= 1
            If Me.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init Is Nothing Then
                Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init, New StaticLocalInitFlag(), Nothing)
            End If
            Dim flag As Boolean = False
            Try
                Monitor.Enter(Me.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init, flag)
                If Me.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init.State = 0S Then
                    Me.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init.State = 2S
                    Me.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded = False
                ElseIf Me.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init.State = 2S Then
                    Throw New IncompleteInitialization()
                End If
            Finally
                Me.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init.State = 1S
                If flag Then
                    Monitor.[Exit](Me.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init)
                End If
            End Try
            If Not Me.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded Then
                Me.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded = True
                Me.SliderLoad()
                If Not Me.RadioLauncherTheme8.IsEnabled Then
                    Me.LabLauncherTheme8Copy.ToolTip = String.Format("累积赞助达到 ¥23.33 后，在爱发电私信发送【土豆 {0}】以解锁。", ModBase.Identify) + vbCrLf & "右键打开赞助页面，如果觉得 PCL 做得还不错就支持一下吧 =w=！"
                End If
                Me.RadioLauncherTheme8.ToolTip = String.Format("累积赞助达到 ¥23.33 后，在爱发电私信发送【土豆 {0}】以解锁", ModBase.Identify)
                If Not Me.RadioLauncherTheme9.IsEnabled Then
                    Me.LabLauncherTheme9Copy.ToolTip = "· 反馈一个 Bug，在标记为 [完成] 后回复识别码要求解锁（右键打开反馈页面）" & vbCrLf & "· 提交一个 Pull Request 或主页预设，在标记为 [完成] 后回复识别码要求解锁"
                End If
                Me.RadioLauncherTheme9.ToolTip = "· 反馈一个 Bug，在标记为 [完成] 后回复识别码要求解锁" & vbCrLf & "· 提交一个 Pull Request 或主页预设，在标记为 [完成] 后回复识别码要求解锁"
            End If
        End Sub

        Public Sub Refresh()
            Try
                SettingService.RefreshSettings(Me)
                PageSetupUI.BackgroundRefresh(False, False)
                Me.CheckLogoLeft.Visibility = If(Me.RadioLogoType0.Checked, Visibility.Visible, Visibility.Collapsed)
                Me.PanLogoText.Visibility = If(Me.RadioLogoType2.Checked, Visibility.Visible, Visibility.Collapsed)
                Me.PanLogoChange.Visibility = If(Me.RadioLogoType3.Checked, Visibility.Visible, Visibility.Collapsed)
                Me.MusicRefreshUI()
                PageSetupUI.OnMainPageTypeChanged()
            Catch ex As NullReferenceException
                Logger.[Error](ex, "个性化设置项存在异常，已被自动重置", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
                Me.Reset()
            Catch ex2 As Exception
                Logger.[Error](ex2, "重载个性化设置时出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
            End Try
        End Sub

        Public Sub Reset()
            Try
                SettingService.ResetSettings(Me)
                Logger.Info("已初始化个性化设置！", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
                ModMain.Hint("已初始化个性化设置", ModMain.HintType.Green, False)
            Catch ex As Exception
                Logger.[Error](ex, "初始化个性化设置失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
            End Try
            Me.Refresh()
        End Sub

        Private Sub BtnUIBgOpen_Click(sender As Object, e As EventArgs)
            ModBase.OpenExplorer(Paths.Base + "PCL\Pictures\")
        End Sub

        Private Sub BtnBackgroundRefresh_Click(sender As Object, e As EventArgs)
            PageSetupUI.BackgroundRefresh(True, True)
        End Sub

        Public Sub BackgroundRefreshUI(Show As Boolean, Count As Integer)
            If Not Information.IsNothing(Me.PanBackgroundOpacity) Then
                If Show Then
                    Me.PanBackgroundOpacity.Visibility = Visibility.Visible
                    Me.PanBackgroundBlur.Visibility = Visibility.Visible
                    Me.PanBackgroundSuit.Visibility = Visibility.Visible
                    Me.BtnBackgroundClear.Visibility = Visibility.Visible
                    Me.CardBackground.Title = "背景图片（" + Conversions.ToString(Count) + " 张）"
                Else
                    Me.PanBackgroundOpacity.Visibility = Visibility.Collapsed
                    Me.PanBackgroundBlur.Visibility = Visibility.Collapsed
                    Me.PanBackgroundSuit.Visibility = Visibility.Collapsed
                    Me.BtnBackgroundClear.Visibility = Visibility.Collapsed
                    Me.CardBackground.Title = "背景图片"
                End If
                Me.CardBackground.TriggerForceResize()
            End If
        End Sub

        Private Sub BtnBackgroundClear_Click(sender As Object, e As EventArgs)
            If ModMain.MyMsgBox("即将删除背景图片文件夹中的所有文件。" & vbCrLf & "此操作不可撤销，是否确定？", "警告", "确定", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 1 Then
                DirectoryUtils.Delete(Paths.Base + "PCL\Pictures", False)
                PageSetupUI.BackgroundRefresh(False, True)
                ModMain.Hint("背景图片已清空！", ModMain.HintType.Green, True)
            End If
        End Sub

        Public Shared Sub BackgroundRefresh(IsHint As Boolean, Refresh As Boolean)
            Try
                DirectoryUtils.Create(Paths.Base + "PCL\Pictures\")
                Dim list As List(Of String) = New List(Of String)()
                Try
                    For Each text As String In DirectoryUtils.EnumerateFiles(Paths.Base + "PCL\Pictures\", True, "*")
                        Dim extension As String = PathUtils.GetExtension(text)
                        If Operators.CompareString(extension, "ini", False) <> 0 AndAlso Operators.CompareString(extension, "db", False) <> 0 Then
                            list.Add(text)
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of String)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                If Not list.Any() Then
                    If Refresh Then
                        If ModMain.FrmMain.ImgBack.Visibility = Visibility.Collapsed Then
                            If IsHint Then
                                ModMain.Hint("未检测到可用背景图片！", ModMain.HintType.Red, True)
                            End If
                        Else
                            ModMain.FrmMain.ImgBack.Visibility = Visibility.Collapsed
                            If IsHint Then
                                ModMain.Hint("背景图片已清除！", ModMain.HintType.Green, True)
                            End If
                        End If
                    End If
                    If Not Information.IsNothing(ModMain.FrmSetupUI) Then
                        ModMain.FrmSetupUI.BackgroundRefreshUI(False, 0)
                    End If
                Else
                    If Refresh Then
                        Dim text2 As String = ModBase.RandomOne(Of String)(list)
                        Try
                            Logger.Info(String.Format("加载背景图片：{0}", text2), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
                            ModMain.FrmMain.ImgBack.Background = New MyBitmap(text2)
                            ModMain.FrmMain.ImgBack.Visibility = Visibility.Visible
                            ModMain.FrmMain.UpdateBackgroundAndTitleBar()
                            If IsHint Then
                                ModMain.Hint("背景图片已刷新：" + PathUtils.GetLastPart(text2), ModMain.HintType.Green, False)
                            End If
                        Catch ex As Exception
                            If ex.Message.Contains("参数无效") Then
                                Logger.[Error](String.Format("刷新背景图片失败，该图片文件可能并非标准格式。{0}你可以尝试使用画图打开该文件并重新保存，这会让图片变为标准格式。{1}文件：{2}", vbCrLf, vbCrLf, text2), LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
                            Else
                                Logger.[Error](ex, String.Format("刷新背景图片失败（{0}）", text2), LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
                            End If
                        End Try
                    End If
                    If Not Information.IsNothing(ModMain.FrmSetupUI) Then
                        ModMain.FrmSetupUI.BackgroundRefreshUI(True, list.Count)
                    End If
                End If
            Catch ex2 As Exception
                Logger.[Error](ex2, "刷新背景图片时出现未知错误", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
            End Try
        End Sub

        Private Sub BtnLogoChange_Click(sender As Object, e As EventArgs)
            Dim text As String = Dialogs.SelectFile("选择图片", False, Nothing, New OneOf(Of IEnumerable(Of ValueTuple(Of String(), String)), String)?(New ValueTuple(Of String(), String)() { New ValueTuple(Of String(), String)(New String() { "png", "jpg", "jpeg", "gif", "webp" }, "常用图片文件") })).FirstOrDefault()
            If Not String.IsNullOrEmpty(text) Then
                Dim text2 As String = Paths.Base + "PCL\Logo.png"
                Try
                    FileUtils.Copy(text, text2)
                    ModMain.FrmMain.ImageTitleLogo.Source = Nothing
                    ModMain.FrmMain.ImageTitleLogo.Source = text2
                Catch ex As Exception
                    If ex.Message.Contains("参数无效") Then
                        Logger.[Error](String.Format("改变标题栏图片失败，该图片文件可能并非标准格式。{0}你可以尝试使用画图打开该文件并重新保存，这会让图片变为标准格式。{1}文件：{2}", vbCrLf, vbCrLf, text2), LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
                    Else
                        Logger.[Error](ex, "设置标题栏图片失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
                    End If
                    ModMain.FrmMain.ImageTitleLogo.Source = Nothing
                End Try
            End If
        End Sub

        Private Sub RadioLogoType3_Check(sender As Object, e As ModBase.RouteEventArgs)
            If ModAnimation.AniControlEnabled = 0 AndAlso e.RaiseByMouse Then
                While True
                    Dim text As String = Paths.Base + "PCL\Logo.png"
                    If FileUtils.Exists(text) Then
                        Try
                            ModMain.FrmMain.ImageTitleLogo.Source = Nothing
                            ModMain.FrmMain.ImageTitleLogo.Source = text
                            Return
                        Catch ex As Exception
                            If ex.Message.Contains("参数无效") Then
                                Logger.[Error](String.Format("改变标题栏图片失败，该图片文件可能并非标准格式。{0}你可以尝试使用画图打开该文件并重新保存，这会让图片变为标准格式。{1}文件：{2}", vbCrLf, vbCrLf, text), LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
                            Else
                                Logger.[Error](ex, "调整标题栏图片失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
                            End If
                            ModMain.FrmMain.ImageTitleLogo.Source = Nothing
                            e.Handled = True
                            Try
                                FileUtils.Delete(text, False)
                            Catch ex2 As Exception
                                Logger.[Error](ex2, "清理错误的标题栏图片失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
                            End Try
                            Return
                        End Try
                    End If
                    Dim text2 As String = Dialogs.SelectFile("选择图片", False, Nothing, New OneOf(Of IEnumerable(Of ValueTuple(Of String(), String)), String)?(New ValueTuple(Of String(), String)() { New ValueTuple(Of String(), String)(New String() { "png", "jpeg", "jpg", "gif", "webp" }, "常用图片文件") })).FirstOrDefault()
                    If String.IsNullOrEmpty(text2) Then
                        Exit For
                    End If
                    Try
                        FileUtils.Copy(text2, text)
                        Continue For
                    Catch ex3 As Exception
                        Logger.[Error](ex3, "复制标题栏图片失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
                    End Try
                    Return
                End While
                ModMain.FrmMain.ImageTitleLogo.Source = Nothing
                e.Handled = True
                Return
            End If
        End Sub

        Private Sub BtnLogoDelete_Click(sender As Object, e As EventArgs)
            Try
                FileUtils.Delete(Paths.Base + "PCL\Logo.png", False)
                Me.RadioLogoType1.SetChecked(True, True)
                ModMain.Hint("标题栏图片已清空！", ModMain.HintType.Green, True)
            Catch ex As Exception
                Logger.[Error](ex, "清空标题栏图片失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
            End Try
        End Sub

        Private Sub BtnMusicOpen_Click(sender As Object, e As EventArgs)
            ModBase.OpenExplorer(Paths.Base + "PCL\Musics\")
        End Sub

        Private Sub BtnMusicRefresh_Click(sender As Object, e As EventArgs)
            ModMusic.MusicRefreshPlay(True, False)
        End Sub

        Public Sub MusicRefreshUI()
            If Me.PanBackgroundOpacity IsNot Nothing Then
                If ModMusic.MusicAllList.Any() Then
                    Me.PanMusicVolume.Visibility = Visibility.Visible
                    Me.PanMusicDetail.Visibility = Visibility.Visible
                    Me.BtnMusicClear.Visibility = Visibility.Visible
                    Dim cardMusic As MyCard = Me.CardMusic
                    Dim text As String = "背景音乐（"
                    Dim enumerable As IEnumerable(Of String) = DirectoryUtils.EnumerateFiles(Paths.Base + "PCL\Musics\", True, "*")
                    Dim func As Func(Of String, Boolean)
                    If PageSetupUI._Closure$__.$I15-0 IsNot Nothing Then
                        func = PageSetupUI._Closure$__.$I15-0
                    Else
                        Dim func2 As Func(Of String, Boolean) = Function(f As String) Not New String() { "ini", "jpg", "txt", "cfg", "lrc", "db", "png" }.Contains(PathUtils.GetExtension(f))
                        func = func2
                        PageSetupUI._Closure$__.$I15-0 = func2
                    End If
                    cardMusic.Title = text + Conversions.ToString(enumerable.Count(func)) + " 首）"
                Else
                    Me.PanMusicVolume.Visibility = Visibility.Collapsed
                    Me.PanMusicDetail.Visibility = Visibility.Collapsed
                    Me.BtnMusicClear.Visibility = Visibility.Collapsed
                    Me.CardMusic.Title = "背景音乐"
                End If
                Me.CardMusic.TriggerForceResize()
            End If
        End Sub

        Private Sub BtnMusicClear_Click(sender As Object, e As EventArgs)
            If ModMain.MyMsgBox("即将删除背景音乐文件夹中的所有文件。" & vbCrLf & "此操作不可撤销，是否确定？", "警告", "确定", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 1 Then
                Dim action As Action
                If PageSetupUI._Closure$__.$I16-0 IsNot Nothing Then
                    action = PageSetupUI._Closure$__.$I16-0
                Else
                    Dim action2 As Action = Sub()
                        ModMain.Hint("正在删除背景音乐……", ModMain.HintType.Blue, True)
                        ModMusic.MusicNAudio = Nothing
                        ModMusic.MusicWaitingList = New List(Of String)()
                        ModMusic.MusicAllList = New List(Of String)()
                        Thread.Sleep(200)
                        Try
                            DirectoryUtils.Delete(Paths.Base + "PCL\Musics", False)
                            ModMain.Hint("背景音乐已删除！", ModMain.HintType.Green, True)
                        Catch ex As Exception
                            Logger.[Error](ex, "删除背景音乐失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
                        End Try
                        Try
                            DirectoryUtils.Create(Paths.Base + "PCL\Musics\")
                            Dim action3 As Action
                            If PageSetupUI._Closure$__.$I16-1 IsNot Nothing Then
                                action3 = PageSetupUI._Closure$__.$I16-1
                            Else
                                Dim action4 As Action = Sub()
                                    ModMusic.MusicRefreshPlay(False, False)
                                End Sub
                                action3 = action4
                                PageSetupUI._Closure$__.$I16-1 = action4
                            End If
                            ModBase.RunInUi(action3, False)
                        Catch ex2 As Exception
                            Logger.[Error](ex2, "重建背景音乐文件夹失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
                        End Try
                    End Sub
                    action = action2
                    PageSetupUI._Closure$__.$I16-0 = action2
                End If
                ModBase.RunInThread(action)
            End If
        End Sub

        Private Sub CheckMusicStart_Change()
            If ModAnimation.AniControlEnabled = 0 AndAlso Me.CheckMusicStart.Checked Then
                Me.CheckMusicStop.Checked = False
            End If
        End Sub

        Private Sub CheckMusicStop_Change()
            If ModAnimation.AniControlEnabled = 0 AndAlso Me.CheckMusicStop.Checked Then
                Me.CheckMusicStart.Checked = False
            End If
        End Sub

        Private Sub BtnCustomFile_Click(sender As Object, e As EventArgs)
            Try
                If Not FileUtils.Exists(Paths.Base + "PCL\Custom.xaml") OrElse ModMain.MyMsgBox("当前已存在布局文件，继续生成教学文件将会覆盖现有布局文件！", "覆盖确认", "继续", "取消", "", True, True, False, Nothing, Nothing, Nothing) <> 2 Then
                    ModBase.ExtractResources(Paths.Base + "PCL\Custom.xaml", "Custom")
                    ModMain.Hint("教学文件已生成！", ModMain.HintType.Green, True)
                    ModBase.OpenExplorer(Paths.Base + "PCL\Custom.xaml")
                End If
            Catch ex As Exception
                Logger.[Error](ex, "生成教学文件失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
            End Try
        End Sub

        Private Sub BtnCustomRefresh_Click()
            ModMain.FrmLaunchRight.ForceRefresh()
            ModMain.Hint("已刷新主页！", ModMain.HintType.Green, True)
        End Sub

        Private Sub BtnCustomTutorial_Click(sender As Object, e As EventArgs)
            ModMain.MyMsgBox("1. 点击 生成教学文件 按钮，这会在 PCL 文件夹下生成 Custom.xaml 布局文件。" & vbCrLf & "2. 使用记事本等工具打开这个文件并进行修改，修改完记得保存。" & vbCrLf & "3. 点击 刷新主页 按钮，查看主页现在长啥样了。" & vbCrLf & vbCrLf & "你可以在生成教学文件后直接刷新主页，对照着进行修改，更有助于理解。" & vbCrLf & "直接将主页文件拖进 PCL 窗口也可以快捷加载。", "主页自定义教程", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
        End Sub

        Public Shared Sub OnMainPageTypeChanged()
            If ModMain.FrmSetupUI IsNot Nothing Then
                Select Case Settings.[Get](Of Integer)("UiCustomType", Nothing)
                    Case 0
                        ModMain.FrmSetupUI.PanCustomPreset.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.PanCustomLocal.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.PanCustomNet.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.HintCustom.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.HintCustomWarn.Visibility = Visibility.Collapsed
                    Case 1
                        ModMain.FrmSetupUI.PanCustomPreset.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.PanCustomLocal.Visibility = Visibility.Visible
                        ModMain.FrmSetupUI.PanCustomNet.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.HintCustom.Visibility = Visibility.Visible
                        ModMain.FrmSetupUI.HintCustomWarn.Visibility = If(Settings.[Get](Of Boolean)("HintCustomWarn", Nothing), Visibility.Collapsed, Visibility.Visible)
                        ModMain.FrmSetupUI.HintCustom.Text = String.Format("从 PCL 文件夹下的 Custom.xaml 读取主页内容。{0}你可以手动编辑该文件，向主页添加文本、图片、常用网站、快捷启动等功能。", vbCrLf)
                        CustomEventService.SetEventType(ModMain.FrmSetupUI.HintCustom, CustomEvent.EventType.None)
                    Case 2
                        ModMain.FrmSetupUI.PanCustomPreset.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.PanCustomLocal.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.PanCustomNet.Visibility = Visibility.Visible
                        ModMain.FrmSetupUI.HintCustom.Visibility = Visibility.Visible
                        ModMain.FrmSetupUI.HintCustomWarn.Visibility = If(Settings.[Get](Of Boolean)("HintCustomWarn", Nothing), Visibility.Collapsed, Visibility.Visible)
                        ModMain.FrmSetupUI.HintCustom.Text = String.Format("从指定网址联网获取主页内容。服主也可以用于动态更新服务器公告。{0}如果你制作了稳定运行的联网主页，可以点击这条提示投稿，若合格即可加入预设！", vbCrLf)
                        CustomEventService.SetEventType(ModMain.FrmSetupUI.HintCustom, CustomEvent.EventType.打开网页)
                        CustomEventService.SetEventData(ModMain.FrmSetupUI.HintCustom, "https://github.com/Meloong-Git/PCL/discussions/2528")
                    Case 3
                        ModMain.FrmSetupUI.PanCustomPreset.Visibility = Visibility.Visible
                        ModMain.FrmSetupUI.PanCustomLocal.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.PanCustomNet.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.HintCustom.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupUI.HintCustomWarn.Visibility = Visibility.Collapsed
                End Select
                ModMain.FrmSetupUI.CardCustom.TriggerForceResize()
            End If
        End Sub

        Private Sub BtnCustomFolder_Click(sender As Object, e As MouseButtonEventArgs)
            Dim text As String = Paths.Base + "PCL\Custom.xaml"
            If FileUtils.Exists(text) Then
                ModBase.OpenExplorer(text)
                Return
            End If
            ModBase.OpenExplorer(Paths.Base + "PCL\")
        End Sub

        Private Sub LabLauncherTheme5Unlock_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
            Dim radioLauncherTheme5Gray As MyRadioBox = Me.RadioLauncherTheme5Gray
            Dim myRadioBox As MyRadioBox = radioLauncherTheme5Gray
            radioLauncherTheme5Gray.Opacity = myRadioBox.Opacity - 0.7
            Dim radioLauncherTheme As MyRadioBox = Me.RadioLauncherTheme5
            myRadioBox = radioLauncherTheme
            radioLauncherTheme.Opacity = myRadioBox.Opacity + 0.7
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.RadioLauncherTheme5Gray, 1.0, CInt(Math.Round(1000.0 * ModAnimation.AniSpeed)), CInt(Math.Round(500.0 * ModAnimation.AniSpeed)), New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaOpacity(Me.RadioLauncherTheme5, -1.0, CInt(Math.Round(1000.0 * ModAnimation.AniSpeed)), CInt(Math.Round(500.0 * ModAnimation.AniSpeed)), New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False) }, "ThemeUnlock", False)
            If Me.RadioLauncherTheme5Gray.Opacity < 0.02 Then
                ModSecret.ThemeUnlock(5, True, "隐藏主题 玄素黑 已解锁！")
                ModAnimation.AniStop("ThemeUnlock")
                Me.RadioLauncherTheme5.Checked = True
            End If
        End Sub

        Private Sub LabLauncherTheme11Click_MouseLeftButtonUp()
            If(Me.LabLauncherTheme11Click.Visibility = Visibility.Collapsed OrElse (If(Me.LabLauncherTheme11Click.ToolTip, "")).ToString().Contains("点击")) AndAlso ModMain.MyMsgBox("1. 不爬取或攻击相关服务或网站，不盗取相关账号，没有谜题可以或需要以此来解决。" & vbCrLf & "2. 不得篡改或损毁相关公开信息，请尽量让它们保持原状。" & vbCrLf & "3. 在你感到迷茫的时候，看看回声洞可能会给你带来惊喜。" & vbCrLf & vbCrLf & "若违规，可能会被从任意相关群中踢出！", "解密游戏的基本规则", "我知道了", "恕我拒绝", "", False, True, False, Nothing, Nothing, Nothing) = 1 Then
                ModMain.MyMsgBox("你需要用自己的智慧来找到下一步的线索……" & vbCrLf & "初始线索：gnp.dorC61\60\20\0202\moc.x1xa.2s\\:sp" + "T".Lower() + "th", "解密游戏", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
            End If
        End Sub

        Private Sub LabLauncherTheme8Copy_MouseRightButtonUp()
            ModBase.OpenWebsite("https://meloong.com/afd/a/LTCat")
        End Sub

        Private Sub LabLauncherTheme9Copy_MouseRightButtonUp()
            PageOtherLeft.TryFeedback()
        End Sub

        Private Sub RadioLauncherTheme14_Change(sender As Object, e As ModBase.RouteEventArgs)
            If Me.RadioLauncherTheme14.Checked Then
                If Me.LabLauncherHue.Visibility = Visibility.Visible Then
                    Return
                End If
                Me.LabLauncherHue.Visibility = Visibility.Visible
                Me.SliderLauncherHue.Visibility = Visibility.Visible
                Me.LabLauncherSat.Visibility = Visibility.Visible
                Me.SliderLauncherSat.Visibility = Visibility.Visible
                Me.LabLauncherDelta.Visibility = Visibility.Visible
                Me.SliderLauncherDelta.Visibility = Visibility.Visible
                Me.LabLauncherLight.Visibility = Visibility.Visible
                Me.SliderLauncherLight.Visibility = Visibility.Visible
            Else
                If Me.LabLauncherHue.Visibility = Visibility.Collapsed Then
                    Return
                End If
                Me.LabLauncherHue.Visibility = Visibility.Collapsed
                Me.SliderLauncherHue.Visibility = Visibility.Collapsed
                Me.LabLauncherSat.Visibility = Visibility.Collapsed
                Me.SliderLauncherSat.Visibility = Visibility.Collapsed
                Me.LabLauncherDelta.Visibility = Visibility.Collapsed
                Me.SliderLauncherDelta.Visibility = Visibility.Collapsed
                Me.LabLauncherLight.Visibility = Visibility.Collapsed
                Me.SliderLauncherLight.Visibility = Visibility.Collapsed
            End If
            Me.CardLauncher.TriggerForceResize()
        End Sub

        Private Sub HSL_Change()
            If ModAnimation.AniControlEnabled = 0 AndAlso Me.SliderLauncherSat IsNot Nothing AndAlso Me.SliderLauncherSat.IsLoaded Then
                ModSecret.ThemeRefresh(-1)
            End If
        End Sub

        Public Shared Property HiddenForceShow As Boolean
            Get
                Return PageSetupUI._HiddenForceShow
            End Get
            Set(value As Boolean)
                PageSetupUI._HiddenForceShow = value
                PageSetupUI.HiddenRefresh()
            End Set
        End Property

        Public Shared Sub HiddenRefresh()
            ' The following expression was wrapped in a checked-statement
            If ModMain.FrmMain.PanTitleSelect IsNot Nothing AndAlso ModMain.FrmMain.PanTitleSelect.IsLoaded Then
                Try
                    If Not PageSetupUI.HiddenForceShow AndAlso Settings.[Get](Of Boolean)("UiHiddenPageDownload", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenPageLink", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenPageSetup", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenPageOther", Nothing) Then
                        ModMain.FrmMain.PanTitleSelect.Visibility = Visibility.Collapsed
                    Else
                        ModMain.FrmMain.PanTitleSelect.Visibility = Visibility.Visible
                        ModMain.FrmMain.BtnTitleSelect1.Visibility = If((Not PageSetupUI.HiddenForceShow AndAlso Settings.[Get](Of Boolean)("UiHiddenPageDownload", Nothing)), Visibility.Collapsed, Visibility.Visible)
                        ModMain.FrmMain.BtnTitleSelect2.Visibility = Visibility.Collapsed
                        ModMain.FrmMain.BtnTitleSelect3.Visibility = If((Not PageSetupUI.HiddenForceShow AndAlso Settings.[Get](Of Boolean)("UiHiddenPageSetup", Nothing)), Visibility.Collapsed, Visibility.Visible)
                        ModMain.FrmMain.BtnTitleSelect4.Visibility = If((Not PageSetupUI.HiddenForceShow AndAlso Settings.[Get](Of Boolean)("UiHiddenPageOther", Nothing)), Visibility.Collapsed, Visibility.Visible)
                    End If
                    ModMain.FrmLaunchLeft.RefreshButtonsUI()
                    If ModMain.FrmSetupUI IsNot Nothing Then
                        ModMain.FrmSetupUI.CardSwitch.Visibility = If((Not PageSetupUI.HiddenForceShow AndAlso Settings.[Get](Of Boolean)("UiHiddenFunctionHidden", Nothing)), Visibility.Collapsed, Visibility.Visible)
                    End If
                    If ModMain.FrmInstanceLeft IsNot Nothing Then
                        ModMain.FrmInstanceLeft.ItemModify.Visibility = If((Not PageSetupUI.HiddenForceShow AndAlso Settings.[Get](Of Boolean)("UiHiddenFunctionInstanceModify", Nothing)), Visibility.Collapsed, Visibility.Visible)
                    End If
                    If ModMain.FrmSetupLeft IsNot Nothing Then
                        ModMain.FrmSetupLeft.ItemLaunch.Visibility = If((Not PageSetupUI.HiddenForceShow AndAlso Settings.[Get](Of Boolean)("UiHiddenSetupLaunch", Nothing)), Visibility.Collapsed, Visibility.Visible)
                        ModMain.FrmSetupLeft.ItemUI.Visibility = If((Not PageSetupUI.HiddenForceShow AndAlso Settings.[Get](Of Boolean)("UiHiddenSetupUi", Nothing)), Visibility.Collapsed, Visibility.Visible)
                        ModMain.FrmSetupLeft.ItemLink.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupLeft.ItemSystem.Visibility = If((Not PageSetupUI.HiddenForceShow AndAlso Settings.[Get](Of Boolean)("UiHiddenSetupSystem", Nothing)), Visibility.Collapsed, Visibility.Visible)
                        Dim num As Integer = 0
                        If Not Settings.[Get](Of Boolean)("UiHiddenSetupLaunch", Nothing) Then
                            num += 1
                        End If
                        If Not Settings.[Get](Of Boolean)("UiHiddenSetupUi", Nothing) Then
                            num += 1
                        End If
                        If Not Settings.[Get](Of Boolean)("UiHiddenSetupSystem", Nothing) Then
                            num += 1
                        End If
                        ModMain.FrmSetupLeft.PanItem.Visibility = If((num < 2 AndAlso Not PageSetupUI.HiddenForceShow), Visibility.Collapsed, Visibility.Visible)
                    End If
                    Dim num2 As Integer = 0
                    If Not Settings.[Get](Of Boolean)("UiHiddenOtherHelp", Nothing) Then
                        num2 += 1
                    End If
                    If Not Settings.[Get](Of Boolean)("UiHiddenOtherAbout", Nothing) Then
                        num2 += 1
                    End If
                    If Not Settings.[Get](Of Boolean)("UiHiddenOtherTest", Nothing) Then
                        num2 += 1
                    End If
                    If Not Settings.[Get](Of Boolean)("UiHiddenOtherFeedback", Nothing) Then
                        num2 += 1
                    End If
                    If Not Settings.[Get](Of Boolean)("UiHiddenOtherVote", Nothing) Then
                        num2 += 1
                    End If
                    If ModMain.FrmOtherLeft IsNot Nothing Then
                        ModMain.FrmOtherLeft.ItemHelp.Visibility = If((Not PageSetupUI.HiddenForceShow AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherHelp", Nothing)), Visibility.Collapsed, Visibility.Visible)
                        ModMain.FrmOtherLeft.ItemFeedback.Visibility = If((Not PageSetupUI.HiddenForceShow AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherFeedback", Nothing)), Visibility.Collapsed, Visibility.Visible)
                        ModMain.FrmOtherLeft.ItemVote.Visibility = If((Not PageSetupUI.HiddenForceShow AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherVote", Nothing)), Visibility.Collapsed, Visibility.Visible)
                        ModMain.FrmOtherLeft.ItemAbout.Visibility = If((Not PageSetupUI.HiddenForceShow AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherAbout", Nothing)), Visibility.Collapsed, Visibility.Visible)
                        ModMain.FrmOtherLeft.ItemTest.Visibility = If((Not PageSetupUI.HiddenForceShow AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherTest", Nothing)), Visibility.Collapsed, Visibility.Visible)
                        ModMain.FrmOtherLeft.PanItem.Visibility = If((num2 < 2 AndAlso Not PageSetupUI.HiddenForceShow), Visibility.Collapsed, Visibility.Visible)
                    End If
                    If num2 = 1 AndAlso Not PageSetupUI.HiddenForceShow Then
                        If Not Settings.[Get](Of Boolean)("UiHiddenOtherHelp", Nothing) Then
                            ModMain.FrmMain.BtnTitleSelect4.Text = "帮助"
                        ElseIf Not Settings.[Get](Of Boolean)("UiHiddenOtherAbout", Nothing) Then
                            ModMain.FrmMain.BtnTitleSelect4.Text = "关于"
                        Else
                            ModMain.FrmMain.BtnTitleSelect4.Text = "百宝箱"
                        End If
                    Else
                        ModMain.FrmMain.BtnTitleSelect4.Text = "更多"
                    End If
                    If ModMain.FrmMain.PageCurrent Is FormMain.PageType.InstanceSelect Then
                        ModMain.FrmSelectRight.BtnEmptyDownload_Loaded()
                    End If
                    If ModMain.FrmMain.PageCurrent Is FormMain.PageType.Launch Then
                        ModMain.FrmLaunchLeft.RefreshButtonsUI()
                    End If
                    If ModMain.FrmMain.PageCurrent Is FormMain.PageType.InstanceSetup AndAlso ModMain.FrmInstanceModDisabled IsNot Nothing Then
                        ModMain.FrmInstanceModDisabled.BtnDownload_Loaded()
                    End If
                    If ModMain.FrmSetupUI IsNot Nothing Then
                        ModMain.FrmSetupUI.CardSwitch.Title = If(PageSetupUI.HiddenForceShow, "功能隐藏（已暂时关闭，按 F12 以重新启用）", "功能隐藏")
                    End If
                Catch ex As Exception
                    Logger.[Error](ex, "刷新功能隐藏项目失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupUI.xaml.vb")
                End Try
            End If
        End Sub

        Private Sub HiddenSetupMain()
            If Me.CheckHiddenPageSetup.Checked Then
                Me.CheckHiddenSetupLaunch.Checked = True
                Me.CheckHiddenSetupSystem.Checked = True
                Me.CheckHiddenSetupLink.Checked = True
                Me.CheckHiddenSetupUI.Checked = True
                Return
            End If
            If Settings.[Get](Of Boolean)("UiHiddenSetupLaunch", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenSetupUi", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenSetupSystem", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenSetupLink", Nothing) Then
                Me.CheckHiddenSetupLaunch.Checked = False
                Me.CheckHiddenSetupSystem.Checked = False
                Me.CheckHiddenSetupLink.Checked = False
                Me.CheckHiddenSetupUI.Checked = False
            End If
        End Sub

        Private Sub HiddenSetupSub()
            If Settings.[Get](Of Boolean)("UiHiddenSetupLaunch", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenSetupUi", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenSetupSystem", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenSetupLink", Nothing) Then
                Me.CheckHiddenPageSetup.Checked = True
                Return
            End If
            Me.CheckHiddenPageSetup.Checked = False
        End Sub

        Private Sub HiddenOtherMain()
            If Me.CheckHiddenPageOther.Checked Then
                Me.CheckHiddenOtherAbout.Checked = True
                Me.CheckHiddenOtherTest.Checked = True
                Me.CheckHiddenOtherFeedback.Checked = True
                Me.CheckHiddenOtherVote.Checked = True
                Me.CheckHiddenOtherHelp.Checked = True
                Return
            End If
            If Settings.[Get](Of Boolean)("UiHiddenOtherHelp", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherAbout", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherTest", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherVote", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherFeedback", Nothing) Then
                Me.CheckHiddenOtherAbout.Checked = False
                Me.CheckHiddenOtherTest.Checked = False
                Me.CheckHiddenOtherFeedback.Checked = False
                Me.CheckHiddenOtherVote.Checked = False
                Me.CheckHiddenOtherHelp.Checked = False
            End If
        End Sub

        Private Sub HiddenOtherSub(sender As Object, user As Boolean)
            If Settings.[Get](Of Boolean)("UiHiddenOtherHelp", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherAbout", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherTest", Nothing) Then
                Me.CheckHiddenPageOther.Checked = True
            Else
                Me.CheckHiddenPageOther.Checked = False
            End If
            If user AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherHelp", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherAbout", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherTest", Nothing) Then
                Me.CheckHiddenOtherFeedback.Checked = True
                Me.CheckHiddenOtherVote.Checked = True
            End If
        End Sub

        Private Sub HiddenOtherNet(sender As Object, user As Boolean)
            If user AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherHelp", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherAbout", Nothing) AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherTest", Nothing) AndAlso (Not Settings.[Get](Of Boolean)("UiHiddenOtherFeedback", Nothing) OrElse Not Settings.[Get](Of Boolean)("UiHiddenOtherVote", Nothing)) Then
                Me.CheckHiddenOtherAbout.Checked = False
                Me.CheckHiddenOtherTest.Checked = False
                Me.CheckHiddenOtherHelp.Checked = False
            End If
        End Sub

        Private Sub HiddenHint(sender As Object, user As Boolean)
            If Conversions.ToBoolean(ModAnimation.AniControlEnabled = 0 AndAlso Conversions.ToBoolean(NewLateBinding.LateGet(sender, Nothing, "Checked", New Object(-1) {}, Nothing, Nothing, Nothing))) Then
                ModMain.Hint("按 F12 即可暂时关闭功能隐藏设置。千万别忘了，要不然设置就改不回来了……", ModMain.HintType.Blue, True)
            End If
        End Sub

        Private Sub SliderLoad()
            Dim sliderMusicVolume As MySlider = Me.SliderMusicVolume
            Dim [delegate] As [Delegate]
            If PageSetupUI._Closure$__.$I41-0 IsNot Nothing Then
                [delegate] = PageSetupUI._Closure$__.$I41-0
            Else
                Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_25(Of Object, Object) = Function(v As Object) Operators.ConcatenateObject(NewLateBinding.LateGet(Nothing, GetType(Math), "Ceiling", New Object() { Operators.MultiplyObject(v, 0.1) }, Nothing, Nothing, Nothing), "%")
                [delegate] = vb$AnonymousDelegate_
                PageSetupUI._Closure$__.$I41-0 = vb$AnonymousDelegate_
            End If
            sliderMusicVolume.GetHintText = [delegate]
            Dim sliderLauncherTransparent As MySlider = Me.SliderLauncherTransparent
            Dim delegate2 As [Delegate]
            If PageSetupUI._Closure$__.$I41-1 IsNot Nothing Then
                delegate2 = PageSetupUI._Closure$__.$I41-1
            Else
                Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_25(Of Object, Object) = Function(v As Object) Operators.ConcatenateObject(NewLateBinding.LateGet(Nothing, GetType(Math), "Round", New Object() { Operators.AddObject(40, Operators.MultiplyObject(v, 0.1)) }, Nothing, Nothing, Nothing), "%")
                delegate2 = vb$AnonymousDelegate_2
                PageSetupUI._Closure$__.$I41-1 = vb$AnonymousDelegate_2
            End If
            sliderLauncherTransparent.GetHintText = delegate2
            Dim sliderLauncherHue As MySlider = Me.SliderLauncherHue
            Dim delegate3 As [Delegate]
            If PageSetupUI._Closure$__.$I41-2 IsNot Nothing Then
                delegate3 = PageSetupUI._Closure$__.$I41-2
            Else
                Dim vb$AnonymousDelegate_3 As VB$AnonymousDelegate_25(Of Object, Object) = Function(v As Object) Operators.ConcatenateObject(v, "°")
                delegate3 = vb$AnonymousDelegate_3
                PageSetupUI._Closure$__.$I41-2 = vb$AnonymousDelegate_3
            End If
            sliderLauncherHue.GetHintText = delegate3
            Dim sliderLauncherSat As MySlider = Me.SliderLauncherSat
            Dim delegate4 As [Delegate]
            If PageSetupUI._Closure$__.$I41-3 IsNot Nothing Then
                delegate4 = PageSetupUI._Closure$__.$I41-3
            Else
                Dim vb$AnonymousDelegate_4 As VB$AnonymousDelegate_25(Of Object, Object) = Function(v As Object) Operators.ConcatenateObject(v, "%")
                delegate4 = vb$AnonymousDelegate_4
                PageSetupUI._Closure$__.$I41-3 = vb$AnonymousDelegate_4
            End If
            sliderLauncherSat.GetHintText = delegate4
            Dim sliderLauncherDelta As MySlider = Me.SliderLauncherDelta
            Dim delegate5 As [Delegate]
            If PageSetupUI._Closure$__.$I41-4 IsNot Nothing Then
                delegate5 = PageSetupUI._Closure$__.$I41-4
            Else
                Dim vb$AnonymousDelegate_5 As VB$AnonymousDelegate_26(Of Integer, String) = Function(Value As Integer)
                    Dim text As String
                    If Value > 90 Then
                        text = "+" + Conversions.ToString(Value - 90)
                    ElseIf Value = 90 Then
                        text = Conversions.ToString(0)
                    Else
                        text = Conversions.ToString(Value - 90)
                    End If
                    Return text
                End Function
                delegate5 = vb$AnonymousDelegate_5
                PageSetupUI._Closure$__.$I41-4 = vb$AnonymousDelegate_5
            End If
            sliderLauncherDelta.GetHintText = delegate5
            Dim sliderLauncherLight As MySlider = Me.SliderLauncherLight
            Dim delegate6 As [Delegate]
            If PageSetupUI._Closure$__.$I41-5 IsNot Nothing Then
                delegate6 = PageSetupUI._Closure$__.$I41-5
            Else
                Dim vb$AnonymousDelegate_6 As VB$AnonymousDelegate_26(Of Integer, String) = Function(Value As Integer)
                    Dim text2 As String
                    If Value > 20 Then
                        text2 = "+" + Conversions.ToString(Value - 20)
                    ElseIf Value = 20 Then
                        text2 = Conversions.ToString(0)
                    Else
                        text2 = Conversions.ToString(Value - 20)
                    End If
                    Return text2
                End Function
                delegate6 = vb$AnonymousDelegate_6
                PageSetupUI._Closure$__.$I41-5 = vb$AnonymousDelegate_6
            End If
            sliderLauncherLight.GetHintText = delegate6
            Dim sliderBackgroundOpacity As MySlider = Me.SliderBackgroundOpacity
            Dim delegate7 As [Delegate]
            If PageSetupUI._Closure$__.$I41-6 IsNot Nothing Then
                delegate7 = PageSetupUI._Closure$__.$I41-6
            Else
                Dim vb$AnonymousDelegate_7 As VB$AnonymousDelegate_25(Of Object, Object) = Function(v As Object) Operators.ConcatenateObject(NewLateBinding.LateGet(Nothing, GetType(Math), "Round", New Object() { Operators.MultiplyObject(v, 0.1) }, Nothing, Nothing, Nothing), "%")
                delegate7 = vb$AnonymousDelegate_7
                PageSetupUI._Closure$__.$I41-6 = vb$AnonymousDelegate_7
            End If
            sliderBackgroundOpacity.GetHintText = delegate7
            Dim sliderBackgroundBlur As MySlider = Me.SliderBackgroundBlur
            Dim delegate8 As [Delegate]
            If PageSetupUI._Closure$__.$I41-7 IsNot Nothing Then
                delegate8 = PageSetupUI._Closure$__.$I41-7
            Else
                Dim vb$AnonymousDelegate_8 As VB$AnonymousDelegate_25(Of Object, Object) = Function(v As Object) Operators.ConcatenateObject(v, " 像素")
                delegate8 = vb$AnonymousDelegate_8
                PageSetupUI._Closure$__.$I41-7 = vb$AnonymousDelegate_8
            End If
            sliderBackgroundBlur.GetHintText = delegate8
        End Sub

        Friend Overridable Property PanBack As MyScrollViewer

        Friend Overridable Property PanMain As StackPanel

        Friend Overridable Property CardLauncher As MyCard

        Friend Overridable Property SliderLauncherTransparent As MySlider

        Friend Overridable Property LabLauncherHue As TextBlock

        Friend Overridable Property SliderLauncherHue As MySlider
            <CompilerGenerated()>
            Get
                Return Me._SliderLauncherHue
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MySlider)
                Dim changeEventHandler As MySlider.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.HSL_Change()
                End Sub
                Dim mySlider As MySlider = Me._SliderLauncherHue
                If mySlider IsNot Nothing Then
                    RemoveHandler mySlider.Change, changeEventHandler
                End If
                Me._SliderLauncherHue = value
                mySlider = Me._SliderLauncherHue
                If mySlider IsNot Nothing Then
                    AddHandler mySlider.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabLauncherDelta As TextBlock

        Friend Overridable Property SliderLauncherDelta As MySlider
            <CompilerGenerated()>
            Get
                Return Me._SliderLauncherDelta
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MySlider)
                Dim changeEventHandler As MySlider.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.HSL_Change()
                End Sub
                Dim mySlider As MySlider = Me._SliderLauncherDelta
                If mySlider IsNot Nothing Then
                    RemoveHandler mySlider.Change, changeEventHandler
                End If
                Me._SliderLauncherDelta = value
                mySlider = Me._SliderLauncherDelta
                If mySlider IsNot Nothing Then
                    AddHandler mySlider.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabLauncherSat As TextBlock

        Friend Overridable Property SliderLauncherSat As MySlider
            <CompilerGenerated()>
            Get
                Return Me._SliderLauncherSat
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MySlider)
                Dim changeEventHandler As MySlider.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.HSL_Change()
                End Sub
                Dim mySlider As MySlider = Me._SliderLauncherSat
                If mySlider IsNot Nothing Then
                    RemoveHandler mySlider.Change, changeEventHandler
                End If
                Me._SliderLauncherSat = value
                mySlider = Me._SliderLauncherSat
                If mySlider IsNot Nothing Then
                    AddHandler mySlider.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabLauncherLight As TextBlock

        Friend Overridable Property SliderLauncherLight As MySlider
            <CompilerGenerated()>
            Get
                Return Me._SliderLauncherLight
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MySlider)
                Dim changeEventHandler As MySlider.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.HSL_Change()
                End Sub
                Dim mySlider As MySlider = Me._SliderLauncherLight
                If mySlider IsNot Nothing Then
                    RemoveHandler mySlider.Change, changeEventHandler
                End If
                Me._SliderLauncherLight = value
                mySlider = Me._SliderLauncherLight
                If mySlider IsNot Nothing Then
                    AddHandler mySlider.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanLauncherTheme As Grid

        Friend Overridable Property RadioLauncherTheme0 As MyRadioBox

        Friend Overridable Property RadioLauncherTheme1 As MyRadioBox

        Friend Overridable Property RadioLauncherTheme2 As MyRadioBox

        Friend Overridable Property RadioLauncherTheme3 As MyRadioBox

        Friend Overridable Property RadioLauncherTheme4 As MyRadioBox

        Friend Overridable Property RadioLauncherTheme5 As MyRadioBox

        Friend Overridable Property RadioLauncherTheme5Gray As MyRadioBox

        Friend Overridable Property LabLauncherTheme5Unlock As TextBlock
            <CompilerGenerated()>
            Get
                Return Me._LabLauncherTheme5Unlock
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As TextBlock)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = AddressOf Me.LabLauncherTheme5Unlock_MouseLeftButtonUp
                Dim textBlock As TextBlock = Me._LabLauncherTheme5Unlock
                If textBlock IsNot Nothing Then
                    RemoveHandler textBlock.MouseLeftButtonUp, mouseButtonEventHandler
                End If
                Me._LabLauncherTheme5Unlock = value
                textBlock = Me._LabLauncherTheme5Unlock
                If textBlock IsNot Nothing Then
                    AddHandler textBlock.MouseLeftButtonUp, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property RadioLauncherTheme12 As MyRadioBox

        Friend Overridable Property RadioLauncherTheme6 As MyRadioBox

        Friend Overridable Property RadioLauncherTheme7 As MyRadioBox

        Friend Overridable Property RadioLauncherTheme13 As MyRadioBox

        Friend Overridable Property RadioLauncherTheme8 As MyRadioBox
            <CompilerGenerated()>
            Get
                Return Me._RadioLauncherTheme8
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyRadioBox)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.LabLauncherTheme8Copy_MouseRightButtonUp()
                End Sub
                Dim myRadioBox As MyRadioBox = Me._RadioLauncherTheme8
                If myRadioBox IsNot Nothing Then
                    RemoveHandler myRadioBox.MouseRightButtonUp, mouseButtonEventHandler
                End If
                Me._RadioLauncherTheme8 = value
                myRadioBox = Me._RadioLauncherTheme8
                If myRadioBox IsNot Nothing Then
                    AddHandler myRadioBox.MouseRightButtonUp, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabLauncherTheme8Copy As TextBlock
            <CompilerGenerated()>
            Get
                Return Me._LabLauncherTheme8Copy
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As TextBlock)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.LabLauncherTheme8Copy_MouseRightButtonUp()
                End Sub
                Dim textBlock As TextBlock = Me._LabLauncherTheme8Copy
                If textBlock IsNot Nothing Then
                    RemoveHandler textBlock.MouseRightButtonUp, mouseButtonEventHandler
                End If
                Me._LabLauncherTheme8Copy = value
                textBlock = Me._LabLauncherTheme8Copy
                If textBlock IsNot Nothing Then
                    AddHandler textBlock.MouseRightButtonUp, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property RadioLauncherTheme9 As MyRadioBox
            <CompilerGenerated()>
            Get
                Return Me._RadioLauncherTheme9
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyRadioBox)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.LabLauncherTheme9Copy_MouseRightButtonUp()
                End Sub
                Dim myRadioBox As MyRadioBox = Me._RadioLauncherTheme9
                If myRadioBox IsNot Nothing Then
                    RemoveHandler myRadioBox.MouseRightButtonUp, mouseButtonEventHandler
                End If
                Me._RadioLauncherTheme9 = value
                myRadioBox = Me._RadioLauncherTheme9
                If myRadioBox IsNot Nothing Then
                    AddHandler myRadioBox.MouseRightButtonUp, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabLauncherTheme9Copy As TextBlock
            <CompilerGenerated()>
            Get
                Return Me._LabLauncherTheme9Copy
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As TextBlock)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.LabLauncherTheme9Copy_MouseRightButtonUp()
                End Sub
                Dim textBlock As TextBlock = Me._LabLauncherTheme9Copy
                If textBlock IsNot Nothing Then
                    RemoveHandler textBlock.MouseRightButtonUp, mouseButtonEventHandler
                End If
                Me._LabLauncherTheme9Copy = value
                textBlock = Me._LabLauncherTheme9Copy
                If textBlock IsNot Nothing Then
                    AddHandler textBlock.MouseRightButtonUp, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property RadioLauncherTheme10 As MyRadioBox

        Friend Overridable Property RadioLauncherTheme11 As MyRadioBox
            <CompilerGenerated()>
            Get
                Return Me._RadioLauncherTheme11
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyRadioBox)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.LabLauncherTheme11Click_MouseLeftButtonUp()
                End Sub
                Dim myRadioBox As MyRadioBox = Me._RadioLauncherTheme11
                If myRadioBox IsNot Nothing Then
                    RemoveHandler myRadioBox.MouseRightButtonUp, mouseButtonEventHandler
                End If
                Me._RadioLauncherTheme11 = value
                myRadioBox = Me._RadioLauncherTheme11
                If myRadioBox IsNot Nothing Then
                    AddHandler myRadioBox.MouseRightButtonUp, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabLauncherTheme11Click As TextBlock
            <CompilerGenerated()>
            Get
                Return Me._LabLauncherTheme11Click
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As TextBlock)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.LabLauncherTheme11Click_MouseLeftButtonUp()
                End Sub
                Dim textBlock As TextBlock = Me._LabLauncherTheme11Click
                If textBlock IsNot Nothing Then
                    RemoveHandler textBlock.MouseLeftButtonUp, mouseButtonEventHandler
                End If
                Me._LabLauncherTheme11Click = value
                textBlock = Me._LabLauncherTheme11Click
                If textBlock IsNot Nothing Then
                    AddHandler textBlock.MouseLeftButtonUp, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property RadioLauncherTheme14 As MyRadioBox
            <CompilerGenerated()>
            Get
                Return Me._RadioLauncherTheme14
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyRadioBox)
                Dim changedEventHandler As IMyRadio.ChangedEventHandler = AddressOf Me.RadioLauncherTheme14_Change
                Dim myRadioBox As MyRadioBox = Me._RadioLauncherTheme14
                If myRadioBox IsNot Nothing Then
                    RemoveHandler myRadioBox.Changed, changedEventHandler
                End If
                Me._RadioLauncherTheme14 = value
                myRadioBox = Me._RadioLauncherTheme14
                If myRadioBox IsNot Nothing Then
                    AddHandler myRadioBox.Changed, changedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanLauncherHide As Border

        Friend Overridable Property CardBackground As MyCard

        Friend Overridable Property PanBackgroundSuit As Grid

        Friend Overridable Property PanBackgroundOpacity As Grid

        Friend Overridable Property SliderBackgroundOpacity As MySlider

        Friend Overridable Property PanBackgroundBlur As Grid

        Friend Overridable Property SliderBackgroundBlur As MySlider

        Friend Overridable Property BtnBackgroundOpen As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnBackgroundOpen
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnUIBgOpen_Click
                Dim myButton As MyButton = Me._BtnBackgroundOpen
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnBackgroundOpen = value
                myButton = Me._BtnBackgroundOpen
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnBackgroundRefresh As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnBackgroundRefresh
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnBackgroundRefresh_Click
                Dim myButton As MyButton = Me._BtnBackgroundRefresh
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnBackgroundRefresh = value
                myButton = Me._BtnBackgroundRefresh
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnBackgroundClear As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnBackgroundClear
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnBackgroundClear_Click
                Dim myButton As MyButton = Me._BtnBackgroundClear
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnBackgroundClear = value
                myButton = Me._BtnBackgroundClear
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CardMusic As MyCard

        Friend Overridable Property PanMusicVolume As Grid

        Friend Overridable Property SliderMusicVolume As MySlider

        Friend Overridable Property PanMusicDetail As StackPanel

        Friend Overridable Property CheckMusicRandom As MyCheckBox

        Friend Overridable Property CheckMusicAuto As MyCheckBox

        Friend Overridable Property CheckMusicStart As MyCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckMusicStart
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCheckBox)
                Dim changeEventHandler As MyCheckBox.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.CheckMusicStart_Change()
                End Sub
                Dim myCheckBox As MyCheckBox = Me._CheckMusicStart
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckMusicStart = value
                myCheckBox = Me._CheckMusicStart
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckMusicStop As MyCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckMusicStop
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCheckBox)
                Dim changeEventHandler As MyCheckBox.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.CheckMusicStop_Change()
                End Sub
                Dim myCheckBox As MyCheckBox = Me._CheckMusicStop
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckMusicStop = value
                myCheckBox = Me._CheckMusicStop
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnMusicOpen As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnMusicOpen
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnMusicOpen_Click
                Dim myButton As MyButton = Me._BtnMusicOpen
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnMusicOpen = value
                myButton = Me._BtnMusicOpen
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnMusicRefresh As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnMusicRefresh
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnMusicRefresh_Click
                Dim myButton As MyButton = Me._BtnMusicRefresh
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnMusicRefresh = value
                myButton = Me._BtnMusicRefresh
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnMusicClear As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnMusicClear
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnMusicClear_Click
                Dim myButton As MyButton = Me._BtnMusicClear
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnMusicClear = value
                myButton = Me._BtnMusicClear
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CardLogo As MyCard

        Friend Overridable Property RadioLogoType0 As MyRadioBox

        Friend Overridable Property RadioLogoType1 As MyRadioBox

        Friend Overridable Property RadioLogoType2 As MyRadioBox

        Friend Overridable Property RadioLogoType3 As MyRadioBox
            <CompilerGenerated()>
            Get
                Return Me._RadioLogoType3
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyRadioBox)
                Dim previewCheckEventHandler As MyRadioBox.PreviewCheckEventHandler = AddressOf Me.RadioLogoType3_Check
                Dim myRadioBox As MyRadioBox = Me._RadioLogoType3
                If myRadioBox IsNot Nothing Then
                    RemoveHandler myRadioBox.PreviewCheck, previewCheckEventHandler
                End If
                Me._RadioLogoType3 = value
                myRadioBox = Me._RadioLogoType3
                If myRadioBox IsNot Nothing Then
                    AddHandler myRadioBox.PreviewCheck, previewCheckEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckLogoLeft As MyCheckBox

        Friend Overridable Property PanLogoText As Grid

        Friend Overridable Property TextLogoText As MyTextBox

        Friend Overridable Property PanLogoChange As Grid

        Friend Overridable Property BtnLogoChange As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnLogoChange
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnLogoChange_Click
                Dim myButton As MyButton = Me._BtnLogoChange
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnLogoChange = value
                myButton = Me._BtnLogoChange
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnLogoDelete As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnLogoDelete
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnLogoDelete_Click
                Dim myButton As MyButton = Me._BtnLogoDelete
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnLogoDelete = value
                myButton = Me._BtnLogoDelete
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CardCustom As MyCard

        Friend Overridable Property RadioCustomType0 As MyRadioBox

        Friend Overridable Property RadioCustomType3 As MyRadioBox

        Friend Overridable Property RadioCustomType1 As MyRadioBox

        Friend Overridable Property RadioCustomType2 As MyRadioBox

        Friend Overridable Property HintCustom As MyHint

        Friend Overridable Property HintCustomWarn As MyHint

        Friend Overridable Property PanCustomLocal As WrapPanel

        Friend Overridable Property BtnCustomRefresh As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnCustomRefresh
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.BtnCustomRefresh_Click()
                End Sub
                Dim myButton As MyButton = Me._BtnCustomRefresh
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnCustomRefresh = value
                myButton = Me._BtnCustomRefresh
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnCustomFile As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnCustomFile
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnCustomFile_Click
                Dim myButton As MyButton = Me._BtnCustomFile
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnCustomFile = value
                myButton = Me._BtnCustomFile
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnCustomTutorial As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnCustomTutorial
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnCustomTutorial_Click
                Dim myButton As MyButton = Me._BtnCustomTutorial
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnCustomTutorial = value
                myButton = Me._BtnCustomTutorial
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnCustomFolder As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnCustomFolder
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnCustomFolder_Click
                Dim myButton As MyButton = Me._BtnCustomFolder
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnCustomFolder = value
                myButton = Me._BtnCustomFolder
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanCustomNet As Grid

        Friend Overridable Property TextCustomNet As MyTextBox

        Friend Overridable Property PanCustomPreset As Grid

        Friend Overridable Property CardSwitch As MyCard

        Friend Overridable Property CheckHiddenPageDownload As MyCheckBox

        Friend Overridable Property CheckHiddenPageLink As MyCheckBox

        Friend Overridable Property CheckHiddenPageSetup As MyCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckHiddenPageSetup
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCheckBox)
                Dim changeEventHandler As MyCheckBox.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.HiddenSetupMain()
                End Sub
                Dim changeEventHandler2 As MyCheckBox.ChangeEventHandler = AddressOf Me.HiddenHint
                Dim myCheckBox As MyCheckBox = Me._CheckHiddenPageSetup
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                    RemoveHandler myCheckBox.Change, changeEventHandler2
                End If
                Me._CheckHiddenPageSetup = value
                myCheckBox = Me._CheckHiddenPageSetup
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                    AddHandler myCheckBox.Change, changeEventHandler2
                End If
            End Set
        End Property

        Friend Overridable Property CheckHiddenPageOther As MyCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckHiddenPageOther
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCheckBox)
                Dim changeEventHandler As MyCheckBox.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.HiddenOtherMain()
                End Sub
                Dim myCheckBox As MyCheckBox = Me._CheckHiddenPageOther
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckHiddenPageOther = value
                myCheckBox = Me._CheckHiddenPageOther
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckHiddenSetupLaunch As MyCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckHiddenSetupLaunch
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCheckBox)
                Dim changeEventHandler As MyCheckBox.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.HiddenSetupSub()
                End Sub
                Dim myCheckBox As MyCheckBox = Me._CheckHiddenSetupLaunch
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckHiddenSetupLaunch = value
                myCheckBox = Me._CheckHiddenSetupLaunch
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckHiddenSetupLink As MyCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckHiddenSetupLink
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCheckBox)
                Dim changeEventHandler As MyCheckBox.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.HiddenSetupSub()
                End Sub
                Dim myCheckBox As MyCheckBox = Me._CheckHiddenSetupLink
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckHiddenSetupLink = value
                myCheckBox = Me._CheckHiddenSetupLink
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckHiddenSetupUI As MyCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckHiddenSetupUI
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCheckBox)
                Dim changeEventHandler As MyCheckBox.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.HiddenSetupSub()
                End Sub
                Dim changeEventHandler2 As MyCheckBox.ChangeEventHandler = AddressOf Me.HiddenHint
                Dim myCheckBox As MyCheckBox = Me._CheckHiddenSetupUI
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                    RemoveHandler myCheckBox.Change, changeEventHandler2
                End If
                Me._CheckHiddenSetupUI = value
                myCheckBox = Me._CheckHiddenSetupUI
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                    AddHandler myCheckBox.Change, changeEventHandler2
                End If
            End Set
        End Property

        Friend Overridable Property CheckHiddenSetupSystem As MyCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckHiddenSetupSystem
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCheckBox)
                Dim changeEventHandler As MyCheckBox.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.HiddenSetupSub()
                End Sub
                Dim myCheckBox As MyCheckBox = Me._CheckHiddenSetupSystem
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckHiddenSetupSystem = value
                myCheckBox = Me._CheckHiddenSetupSystem
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckHiddenOtherHelp As MyCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckHiddenOtherHelp
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCheckBox)
                Dim changeEventHandler As MyCheckBox.ChangeEventHandler = AddressOf Me.HiddenOtherSub
                Dim myCheckBox As MyCheckBox = Me._CheckHiddenOtherHelp
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckHiddenOtherHelp = value
                myCheckBox = Me._CheckHiddenOtherHelp
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckHiddenOtherAbout As MyCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckHiddenOtherAbout
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCheckBox)
                Dim changeEventHandler As MyCheckBox.ChangeEventHandler = AddressOf Me.HiddenOtherSub
                Dim myCheckBox As MyCheckBox = Me._CheckHiddenOtherAbout
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckHiddenOtherAbout = value
                myCheckBox = Me._CheckHiddenOtherAbout
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckHiddenOtherTest As MyCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckHiddenOtherTest
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCheckBox)
                Dim changeEventHandler As MyCheckBox.ChangeEventHandler = AddressOf Me.HiddenOtherSub
                Dim myCheckBox As MyCheckBox = Me._CheckHiddenOtherTest
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckHiddenOtherTest = value
                myCheckBox = Me._CheckHiddenOtherTest
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckHiddenOtherFeedback As MyCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckHiddenOtherFeedback
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCheckBox)
                Dim changeEventHandler As MyCheckBox.ChangeEventHandler = AddressOf Me.HiddenOtherNet
                Dim myCheckBox As MyCheckBox = Me._CheckHiddenOtherFeedback
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckHiddenOtherFeedback = value
                myCheckBox = Me._CheckHiddenOtherFeedback
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckHiddenOtherVote As MyCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckHiddenOtherVote
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCheckBox)
                Dim changeEventHandler As MyCheckBox.ChangeEventHandler = AddressOf Me.HiddenOtherNet
                Dim myCheckBox As MyCheckBox = Me._CheckHiddenOtherVote
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckHiddenOtherVote = value
                myCheckBox = Me._CheckHiddenOtherVote
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckLauncherEmail As MyCheckBox

        Friend Overridable Property CheckHiddenFunctionSelect As MyCheckBox

        Friend Overridable Property CheckHiddenFunctionModUpdate As MyCheckBox

        Friend Overridable Property CheckHiddenFunctionHidden As MyCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckHiddenFunctionHidden
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCheckBox)
                Dim changeEventHandler As MyCheckBox.ChangeEventHandler = AddressOf Me.HiddenHint
                Dim myCheckBox As MyCheckBox = Me._CheckHiddenFunctionHidden
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckHiddenFunctionHidden = value
                myCheckBox = Me._CheckHiddenFunctionHidden
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckHiddenFunctionModify As MyCheckBox

        Private Shared _HiddenForceShow As Boolean = False

        Private $STATIC$PageSetupUI_Loaded$20211C128141$Reloaded As Boolean

        Private $STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init As StaticLocalInitFlag
    End Class
End Namespace
