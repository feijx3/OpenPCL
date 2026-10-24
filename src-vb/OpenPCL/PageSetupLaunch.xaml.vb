Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Shapes
Imports System.Windows.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports PCL.My
Imports PCLCS

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageSetupLaunch
        Inherits OurPageRight

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageSetupLaunch_Loaded
            Me.RamTextLeft = 2
            Me.RamTextRight = 1
            Me.InitializeComponent()
        End Sub

        Private Sub PageSetupLaunch_Loaded(sender As Object, e As RoutedEventArgs)
            Me.PanBack.ScrollToHome()
            Me.RefreshRam(False)
            Me.BtnSwitch.Visibility = If((ModMinecraft.McInstanceSelected Is Nothing), Visibility.Collapsed, Visibility.Visible)
            ModAnimation.AniControlEnabled += 1
            Me.Refresh()
            ModAnimation.AniControlEnabled -= 1
            If Me.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init Is Nothing Then
                Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init, New StaticLocalInitFlag(), Nothing)
            End If
            Dim flag As Boolean = False
            Try
                Monitor.Enter(Me.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init, flag)
                If Me.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init.State = 0S Then
                    Me.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init.State = 2S
                    Me.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded = False
                ElseIf Me.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init.State = 2S Then
                    Throw New IncompleteInitialization()
                End If
            Finally
                Me.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init.State = 1S
                If flag Then
                    Monitor.[Exit](Me.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init)
                End If
            End Try
            If Not Me.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded Then
                Me.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded = True
                Dim dispatcherTimer As DispatcherTimer = New DispatcherTimer()
                dispatcherTimer.Interval = New TimeSpan(0, 0, 0, 1)
                AddHandler dispatcherTimer.Tick, Sub(a0 As Object, a1 As EventArgs)
                    Me.RefreshRam()
                End Sub
                dispatcherTimer.Start()
            End If
        End Sub

        Public Sub Refresh()
            Try
                SettingService.RefreshSettings(Me)
                PageSetupLaunch.UpdateSkinType()
                PageSetupLaunch.UpdateRamType()
                Me.UpdateJavaList()
            Catch ex As NullReferenceException
                Logger.[Error](ex, "启动设置项存在异常，已被自动重置", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\PageSetupLaunch.xaml.vb")
                Me.Reset()
            Catch ex2 As Exception
                Logger.[Error](ex2, "重载启动设置时出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\PageSetupLaunch.xaml.vb")
            End Try
        End Sub

        Public Sub Reset()
            Try
                SettingService.ResetSettings(Me)
                Settings.[Set]("LaunchArgumentIndieV2", Settings.GetDefault("LaunchArgumentIndieV2"), Nothing)
                Configs.JavaList.Reset()
                Configs.JavaRemovedList.Reset()
                ModJava.JavaListRefreshWorker.Start(Nothing, Nothing)
                ModMain.Hint("已初始化启动设置！", ModMain.HintType.Green, True)
            Catch ex As Exception
                Logger.[Error](ex, "初始化启动设置失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\PageSetupLaunch.xaml.vb")
            End Try
            Me.Refresh()
        End Sub

        Private Sub BtnSkinChange_Click(sender As Object, e As EventArgs)
            Dim mcSkinInfo As ModMinecraft.McSkinInfo = ModMinecraft.McSkinSelect()
            If mcSkinInfo.IsVaild Then
                Me.ChangeSkin(mcSkinInfo)
            End If
        End Sub

        Private Sub RadioSkinType3_Check(sender As Object, e As ModBase.RouteEventArgs)
            If ModAnimation.AniControlEnabled = 0 AndAlso e.RaiseByMouse AndAlso Not FileUtils.Exists(Paths.AppDataThenName + "CustomSkin.png") Then
                Dim mcSkinInfo As ModMinecraft.McSkinInfo = ModMinecraft.McSkinSelect()
                If Not mcSkinInfo.IsVaild Then
                    e.Handled = True
                    Return
                End If
                If Not Me.ChangeSkin(mcSkinInfo) Then
                    e.Handled = True
                End If
            End If
        End Sub

        Private Function ChangeSkin(SkinInfo As ModMinecraft.McSkinInfo) As Boolean
            Dim flag As Boolean
            Try
                FileUtils.Copy(SkinInfo.LocalFile, Paths.AppDataThenName + "CustomSkin.png")
                Dim myBitmap As OurBitmap = New OurBitmap(Paths.AppDataThenName + "CustomSkin.png")
                If myBitmap.Pic.Width = 64 AndAlso myBitmap.Pic.Height = 32 Then
                    Dim image As Global.System.Drawing.Image = myBitmap
                    Dim bitmap As Bitmap = New Bitmap(64, 64)
                    Using graphics As Graphics = Graphics.FromImage(bitmap)
                        graphics.DrawImageUnscaled(image, New Global.System.Drawing.Point(0, 0))
                    End Using
                    FileUtils.Delete(Paths.AppDataThenName + "CustomSkin.png", False)
                    bitmap.Save(Paths.AppDataThenName + "CustomSkin.png")
                End If
                Settings.[Set]("LaunchSkinSlim", SkinInfo.IsSlim, Nothing)
                flag = True
            Catch ex As Exception
                Logger.[Error](ex, "改变离线皮肤失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\PageSetupLaunch.xaml.vb")
                flag = False
            Finally
                PageLaunchLeft.SkinLegacy.Start(Nothing, True)
            End Try
            Return flag
        End Function

        Private Sub BtnSkinDelete_Click(sender As Object, e As EventArgs)
            Try
                FileUtils.Delete(Paths.AppDataThenName + "CustomSkin.png", False)
                Me.RadioSkinType0.SetChecked(True, True)
                ModMain.Hint("离线皮肤已清空！", ModMain.HintType.Green, True)
            Catch ex As Exception
                Logger.[Error](ex, "清空离线皮肤失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\PageSetupLaunch.xaml.vb")
            End Try
        End Sub

        Private Sub BtnSkinSave_Click(sender As Object, e As EventArgs)
            OurSkin.Save(PageLaunchLeft.SkinLegacy)
        End Sub

        Private Sub BtnSkinCache_Click(sender As Object, e As EventArgs)
            OurSkin.RefreshCache(Nothing)
        End Sub

        Public Shared Sub UpdateSkinType()
            PageLaunchLeft.SkinLegacy.Start(Nothing, False)
            If ModMain.FrmSetupLaunch IsNot Nothing Then
                Select Case Settings.[Get](Of Integer)("LaunchSkinType", Nothing)
                    Case 0, 1, 2
                        ModMain.FrmSetupLaunch.PanSkinID.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupLaunch.PanSkinChange.Visibility = Visibility.Collapsed
                    Case 3
                        ModMain.FrmSetupLaunch.PanSkinID.Visibility = Visibility.Visible
                        ModMain.FrmSetupLaunch.PanSkinChange.Visibility = Visibility.Collapsed
                    Case 4
                        ModMain.FrmSetupLaunch.PanSkinID.Visibility = Visibility.Collapsed
                        ModMain.FrmSetupLaunch.PanSkinChange.Visibility = Visibility.Visible
                End Select
                ModMain.FrmSetupLaunch.CardSkin.TriggerForceResize()
            End If
        End Sub

        Public Shared Sub UpdateRamType()
            Dim frmSetupLaunch As PageSetupLaunch = ModMain.FrmSetupLaunch
            If If((frmSetupLaunch IsNot Nothing), frmSetupLaunch.SliderRamCustom, Nothing) IsNot Nothing Then
                ModMain.FrmSetupLaunch.SliderRamCustom.IsEnabled = Settings.[Get](Of Integer)("LaunchRamType", Nothing) = 1
            End If
        End Sub

        Public Sub RefreshRam(ShowAnim As Boolean)
            If Me.LabRamGame IsNot Nothing AndAlso Me.LabRamUsed IsNot Nothing AndAlso Not(ModMain.FrmMain.PageCurrent IsNot FormMain.PageType.Setup) AndAlso ModMain.FrmSetupLeft.PageID = FormMain.PageSubType.[Default] Then
                Dim num As Double = Math.Round(PageSetupLaunch.GetRam(ModMinecraft.McInstanceSelected, False), 5)
                Dim num2 As Double = Math.Round(MyWpfExtension.Computer.Info.TotalPhysicalMemory / 1024.0 / 1024.0 / 1024.0, 1)
                Dim num3 As Double = Math.Round(MyWpfExtension.Computer.Info.AvailablePhysicalMemory / 1024.0 / 1024.0 / 1024.0, 1)
                Dim num4 As Double = Math.Round(Math.Min(num, num3), 5)
                Dim num5 As Double = Math.Round(num2 - num3, 5)
                Dim num6 As Double = Math.Round((num2 - num5 - num).Clamp(0.0, 1000.0), 1)
                If num2 <= 1.5 Then
                    ' The following expression was wrapped in a unchecked-expression
                    Me.SliderRamCustom.MaxValue = CInt(Math.Round(Math.Max(Math.Floor(num2 - 0.3 / 0.1), 1.0)))
                ElseIf num2 <= 8.0 Then
                    ' The following expression was wrapped in a unchecked-expression
                    Me.SliderRamCustom.MaxValue = CInt(Math.Round(Math.Floor((num2 - 1.5) / 0.5) + 12.0))
                ElseIf num2 <= 16.0 Then
                    ' The following expression was wrapped in a unchecked-expression
                    Me.SliderRamCustom.MaxValue = CInt(Math.Round(Math.Floor((num2 - 8.0) / 1.0) + 25.0))
                Else
                    ' The following expression was wrapped in a unchecked-expression
                    Me.SliderRamCustom.MaxValue = CInt(Math.Round(Math.Floor((num2 - 16.0) / 2.0) + 33.0))
                End If
                Me.LabRamGame.Text = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(If((num = Math.Floor(num)), (Conversions.ToString(num) + ".0"), num), " GB"), If((num <> num4), Operators.ConcatenateObject(Operators.ConcatenateObject(" (可用 ", If((num4 = Math.Floor(num4)), (Conversions.ToString(num4) + ".0"), num4)), " GB)"), "")))
                Me.LabRamUsed.Text = Conversions.ToString(Operators.ConcatenateObject(If((num5 = Math.Floor(num5)), (Conversions.ToString(num5) + ".0"), num5), " GB"))
                Me.LabRamTotal.Text = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(" / ", If((num2 = Math.Floor(num2)), (Conversions.ToString(num2) + ".0"), num2)), " GB"))
                If ShowAnim Then
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaGridLengthWidth(Me.ColumnRamUsed, num5 - Me.ColumnRamUsed.Width.Value, 800, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False), ModAnimation.AaGridLengthWidth(Me.ColumnRamGame, num4 - Me.ColumnRamGame.Width.Value, 800, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False), ModAnimation.AaGridLengthWidth(Me.ColumnRamEmpty, num6 - Me.ColumnRamEmpty.Width.Value, 800, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False) }, "SetupLaunch Ram Grid", False)
                    Return
                End If
                Me.ColumnRamUsed.Width = New GridLength(num5, GridUnitType.Star)
                Me.ColumnRamGame.Width = New GridLength(num4, GridUnitType.Star)
                Me.ColumnRamEmpty.Width = New GridLength(num6, GridUnitType.Star)
            End If
        End Sub

        Private Sub RefreshRam()
            Me.RefreshRam(True)
        End Sub

        Private Sub RefreshRamText()
            Dim actualWidth As Double = Me.RectRamUsed.ActualWidth
            Dim actualWidth2 As Double = Me.PanRamDisplay.ActualWidth
            Dim actualWidth3 As Double = Me.LabRamGame.ActualWidth
            Dim actualWidth4 As Double = Me.LabRamUsed.ActualWidth
            Dim actualWidth5 As Double = Me.LabRamTotal.ActualWidth
            Dim actualWidth6 As Double = Me.LabRamGameTitle.ActualWidth
            Dim actualWidth7 As Double = Me.LabRamUsedTitle.ActualWidth
            Dim num As Integer
            If actualWidth - 30.0 < actualWidth4 OrElse actualWidth - 30.0 < actualWidth7 Then
                num = 0
            ElseIf actualWidth - 25.0 < actualWidth4 + actualWidth5 Then
                num = 1
            Else
                num = 2
            End If
            If Me.RamTextLeft <> num Then
                Me.RamTextLeft = num
                Select Case num
                    Case 0
                        ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.LabRamUsed, -Me.LabRamUsed.Opacity, 100, 0, Nothing, False), ModAnimation.AaOpacity(Me.LabRamTotal, -Me.LabRamTotal.Opacity, 100, 0, Nothing, False), ModAnimation.AaOpacity(Me.LabRamUsedTitle, -Me.LabRamUsedTitle.Opacity, 100, 0, Nothing, False) }, "SetupLaunch Ram TextLeft", False)
                    Case 1
                        ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.LabRamUsed, 1.0 - Me.LabRamUsed.Opacity, 100, 0, Nothing, False), ModAnimation.AaOpacity(Me.LabRamTotal, -Me.LabRamTotal.Opacity, 100, 0, Nothing, False), ModAnimation.AaOpacity(Me.LabRamUsedTitle, 0.7 - Me.LabRamUsedTitle.Opacity, 100, 0, Nothing, False) }, "SetupLaunch Ram TextLeft", False)
                    Case 2
                        ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.LabRamUsed, 1.0 - Me.LabRamUsed.Opacity, 100, 0, Nothing, False), ModAnimation.AaOpacity(Me.LabRamTotal, 1.0 - Me.LabRamTotal.Opacity, 100, 0, Nothing, False), ModAnimation.AaOpacity(Me.LabRamUsedTitle, 0.7 - Me.LabRamUsedTitle.Opacity, 100, 0, Nothing, False) }, "SetupLaunch Ram TextLeft", False)
                End Select
            End If
            Dim num2 As Integer
            If actualWidth2 < actualWidth3 + 2.0 + actualWidth OrElse actualWidth2 < actualWidth6 + 2.0 + actualWidth Then
                num2 = 0
            Else
                num2 = 1
            End If
            If num2 = 0 Then
                If ModAnimation.AniControlEnabled = 0 AndAlso (Me.RamTextRight <> num2 OrElse ModAnimation.AniIsRun("SetupLaunch Ram TextRight")) Then
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaX(Me.LabRamGame, actualWidth2 - actualWidth3 - Me.LabRamGame.Margin.Left, 100, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaX(Me.LabRamGameTitle, actualWidth2 - actualWidth6 - Me.LabRamGameTitle.Margin.Left, 100, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False) }, "SetupLaunch Ram TextRight", False)
                Else
                    ModAnimation.AniStop("SetupLaunch Ram TextRight")
                    Me.LabRamGame.Margin = New Thickness(actualWidth2 - actualWidth3, 3.0, 0.0, 0.0)
                    Me.LabRamGameTitle.Margin = New Thickness(actualWidth2 - actualWidth6, 0.0, 0.0, 5.0)
                End If
            ElseIf ModAnimation.AniControlEnabled = 0 AndAlso (Me.RamTextRight <> num2 OrElse ModAnimation.AniIsRun("SetupLaunch Ram TextRight")) Then
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaX(Me.LabRamGame, 2.0 + actualWidth - Me.LabRamGame.Margin.Left, 100, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaX(Me.LabRamGameTitle, 2.0 + actualWidth - Me.LabRamGameTitle.Margin.Left, 100, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False) }, "SetupLaunch Ram TextRight", False)
            Else
                ModAnimation.AniStop("SetupLaunch Ram TextRight")
                Me.LabRamGame.Margin = New Thickness(2.0 + actualWidth, 3.0, 0.0, 0.0)
                Me.LabRamGameTitle.Margin = New Thickness(2.0 + actualWidth, 0.0, 0.0, 5.0)
            End If
            Me.RamTextRight = num2
        End Sub

        Public Shared Function GetRam(Instance As ModMinecraft.McInstance, UseVersionJavaSetup As Boolean) As Double
            Dim num9 As Double
            If Settings.[Get](Of Integer)("LaunchRamType", Nothing) = 0 Then
                Dim num As Double = Math.Round(MyWpfExtension.Computer.Info.AvailablePhysicalMemory / 1024.0 / 1024.0 / 1024.0 * 10.0) / 10.0
                If Instance IsNot Nothing AndAlso Not Instance.IsLoaded Then
                    Instance.Load()
                End If
                Dim num4 As Double
                Dim num5 As Double
                Dim num6 As Double
                Dim num7 As Double
                If Instance IsNot Nothing AndAlso Instance.Modable Then
                    Dim info As DirectoryInfo = DirectoryUtils.GetInfo(Instance.PathIndie + "mods\")
                    Dim num2 As Integer
                    If Not info.Exists Then
                        num2 = 0
                    Else
                        Dim files As IEnumerable(Of FileInfo) = info.GetFiles()
                        Dim func As Func(Of FileInfo, Boolean)
                        If PageSetupLaunch._Closure$__.$I17-0 IsNot Nothing Then
                            func = PageSetupLaunch._Closure$__.$I17-0
                        Else
                            Dim func2 As Func(Of FileInfo, Boolean) = Function(f As FileInfo) New String() { ".jar", ".zip", ".litemod" }.Contains(f.Extension.Lower())
                            func = func2
                            PageSetupLaunch._Closure$__.$I17-0 = func2
                        End If
                        num2 = files.Count(func)
                    End If
                    Dim num3 As Integer = num2
                    num4 = 0.5 + CDbl(num3) / 150.0
                    num5 = 1.5 + CDbl(num3) / 90.0
                    num6 = 2.7 + CDbl(num3) / 50.0
                    num7 = 4.5 + CDbl(num3) / 25.0
                ElseIf Instance IsNot Nothing AndAlso Instance.Version.HasOptiFine Then
                    num4 = 0.5
                    num5 = 1.5
                    num6 = 3.0
                    num7 = 5.0
                Else
                    num4 = 0.5
                    num5 = 1.5
                    num6 = 2.5
                    num7 = 4.0
                End If
                Dim num8 As Double = num5
                num9 += Math.Min(num, num8)
                num -= num8
                If num >= 0.1 Then
                    num8 = num6 - num5
                    num9 += Math.Min(num * 0.7, num8)
                    num -= num8 / 0.7
                    If num >= 0.1 Then
                        num8 = num7 - num6
                        num9 += Math.Min(num * 0.4, num8)
                        num -= num8 / 0.4
                        If num >= 0.1 Then
                            num8 = num7
                            num9 += Math.Min(num * 0.15, num8)
                            num -= num8 / 0.15
                        End If
                    End If
                End If
                num9 = Math.Round(Math.Max(num9, num4), 1)
            Else
                Dim num10 As Integer = Settings.[Get](Of Integer)("LaunchRamCustom", Nothing)
                If num10 <= 12 Then
                    num9 = CDbl(num10) * 0.1 + 0.3
                ElseIf num10 <= 25 Then
                    ' The following expression was wrapped in a checked-expression
                    num9 = CDbl((num10 - 12)) * 0.5 + 1.5
                ElseIf num10 <= 33 Then
                    ' The following expression was wrapped in a checked-expression
                    num9 = CDbl(((num10 - 25) * 1 + 8))
                Else
                    ' The following expression was wrapped in a checked-expression
                    num9 = CDbl(((num10 - 33) * 2 + 16))
                End If
            End If
            Return num9
        End Function

        Public Sub UpdateJavaList()
            ' The following expression was wrapped in a checked-statement
            If Me.ComboAdvanceJava IsNot Nothing AndAlso Me.LabAdvanceJava IsNot Nothing AndAlso Me.ComboAdvanceJava.IsLoaded Then
                Me.ComboAdvanceJava.Items.Clear()
                If ModJava.JavaListRefreshWorker.Running Then
                    Me.BtnAdvanceJavaSearch.IsEnabled = False
                    Me.ComboAdvanceJava.IsEnabled = False
                    Me.LabAdvanceJava.Text = "搜索中 …"
                    Return
                End If
                Me.BtnAdvanceJavaSearch.IsEnabled = True
                Me.ComboAdvanceJava.IsEnabled = True
                Dim count As Integer = Configs.JavaList.[Get]().Count
                Me.LabAdvanceJava.Text = If((count > 0), String.Format("共有 {0} 个 Java …", count), "未找到 Java，点击以导入已有的 Java")
                Try
                    Dim concurrentList As ConcurrentList(Of Java) = Configs.JavaList.[Get]()
                    Dim num As Integer = concurrentList.Count - 1
                    For i As Integer = 0 To num
                        Dim CS$<>8__locals1 As PageSetupLaunch._Closure$__18-0 = New PageSetupLaunch._Closure$__18-0(CS$<>8__locals1)
                        CS$<>8__locals1.$VB$Me = Me
                        CS$<>8__locals1.$VB$Local_JavaEntry = concurrentList(i)
                        Dim myListItem As OurListItem = New OurListItem() With { .FontSize = 13.0, .Height = 24.0, .IsScaleAnimationEnabled = False, .Type = OurListItem.CheckType.Clickable, .Tag = CS$<>8__locals1.$VB$Local_JavaEntry, .Title = CS$<>8__locals1.$VB$Local_JavaEntry.ToString() }
                        Dim uielement As UIElement = myListItem
                        Dim mouseButtonEventHandler As MouseButtonEventHandler
                        If PageSetupLaunch._Closure$__.$I18-0 IsNot Nothing Then
                            mouseButtonEventHandler = PageSetupLaunch._Closure$__.$I18-0
                        Else
                            Dim mouseButtonEventHandler2 As MouseButtonEventHandler = Sub(sender As Object, e As MouseButtonEventArgs)
                                e.Handled = True
                            End Sub
                            mouseButtonEventHandler = mouseButtonEventHandler2
                            PageSetupLaunch._Closure$__.$I18-0 = mouseButtonEventHandler2
                        End If
                        AddHandler uielement.MouseLeftButtonUp, mouseButtonEventHandler
                        Dim myListItem2 As OurListItem = myListItem
                        Dim clickEventHandler As OurListItem.ClickEventHandler
                        If PageSetupLaunch._Closure$__.$IR18-2 IsNot Nothing Then
                            clickEventHandler = PageSetupLaunch._Closure$__.$IR18-2
                        Else
                            Dim clickEventHandler2 As OurListItem.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                                Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_10
                                If PageSetupLaunch._Closure$__.$I18-1 IsNot Nothing Then
                                    vb$AnonymousDelegate_ = PageSetupLaunch._Closure$__.$I18-1
                                Else
                                    Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_10 = Sub()
                                        ModMain.Hint("点击选项右侧的箭头可以进行排序，以控制 PCL 优先选择哪个 Java！", ModMain.HintType.Blue, True)
                                    End Sub
                                    vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                                    PageSetupLaunch._Closure$__.$I18-1 = vb$AnonymousDelegate_2
                                End If
                                vb$AnonymousDelegate_()
                            End Sub
                            clickEventHandler = clickEventHandler2
                            PageSetupLaunch._Closure$__.$IR18-2 = clickEventHandler2
                        End If
                        AddHandler myListItem2.Click, clickEventHandler
                        Me.ComboAdvanceJava.Items.Add(myListItem)
                        Dim list As List(Of OurIconButton) = New List(Of OurIconButton)()
                        Dim myIconButton As OurIconButton = New OurIconButton() With { .Logo = "M554 333V853h-85V333l-228 228-60-60L512 170l331 331-60 60L554 333z", .LogoScale = 0.95, .Height = 24.0, .Width = 24.0 }
                        myIconButton.IsEnabled = i > 0
                        myIconButton.ToolTip = "提高优先级"
                        ToolTipService.SetShowOnDisabled(myIconButton, True)
                        AddHandler myIconButton.Click, Sub(a0 As Object, a1 As EventArgs)
                            MyBase._Lambda$__2()
                        End Sub
                        list.Add(myIconButton)
                        Dim myIconButton2 As OurIconButton = New OurIconButton() With { .Logo = "M554 691V171h-85V691L241 463l-60 60L512 854l331-331-60-60L554 691z", .LogoScale = 0.95, .Height = 24.0, .Width = 24.0 }
                        myIconButton2.IsEnabled = i < concurrentList.Count - 1
                        myIconButton2.ToolTip = "降低优先级"
                        ToolTipService.SetShowOnDisabled(myIconButton2, True)
                        AddHandler myIconButton2.Click, Sub(a0 As Object, a1 As EventArgs)
                            MyBase._Lambda$__3()
                        End Sub
                        list.Add(myIconButton2)
                        Dim flag As Boolean = PathUtils.IsParentOf(String.Format("{0}.minecraft\runtime\", Paths.AppData), CS$<>8__locals1.$VB$Local_JavaEntry.Folder)
                        Dim myIconButton3 As OurIconButton = New OurIconButton() With { .Logo = "M508 990.4c-261.6 0-474.4-212-474.4-474.4S246.4 41.6 508 41.6s474.4 212 474.4 474.4S769.6 990.4 508 990.4zM508 136.8c-209.6 0-379.2 169.6-379.2 379.2 0 209.6 169.6 379.2 379.2 379.2s379.2-169.6 379.2-379.2C887.2 306.4 717.6 136.8 508 136.8zM697.6 563.2 318.4 563.2c-26.4 0-47.2-21.6-47.2-47.2 0-26.4 21.6-47.2 47.2-47.2l379.2 0c26.4 0 47.2 21.6 47.2 47.2C744.8 542.4 724 563.2 697.6 563.2z", .Height = 24.0, .Width = 24.0 }
                        myIconButton3.IsEnabled = Not flag
                        myIconButton3.ToolTip = If(myIconButton3.IsEnabled, "从列表中移除", "无法移除官方 Java")
                        AddHandler myIconButton3.Click, Sub(a0 As Object, a1 As EventArgs)
                            MyBase._Lambda$__4()
                        End Sub
                        ToolTipService.SetShowOnDisabled(myIconButton3, True)
                        list.Add(myIconButton3)
                        Dim myIconButton4 As OurIconButton = New OurIconButton() With { .Logo = "M889.018182 418.909091H884.363636V316.509091a93.090909 93.090909 0 0 0-99.607272-89.832727h-302.545455l-93.090909-76.334546A46.545455 46.545455 0 0 0 358.865455 139.636364H146.152727A93.090909 93.090909 0 0 0 46.545455 229.469091V837.818182a46.545455 46.545455 0 0 0 46.545454 46.545454 46.545455 46.545455 0 0 0 16.756364-3.258181 109.381818 109.381818 0 0 0 25.134545 3.258181h586.472727a85.178182 85.178182 0 0 0 87.04-63.301818l163.374546-302.545454a46.545455 46.545455 0 0 0 5.585454-21.876364A82.385455 82.385455 0 0 0 889.018182 418.909091z m-744.727273-186.181818h198.283636l93.09091 76.334545a46.545455 46.545455 0 0 0 29.323636 10.705455h319.301818a12.101818 12.101818 0 0 1 6.516364 0V418.909091H302.545455a85.178182 85.178182 0 0 0-87.04 63.301818L139.636364 622.778182V232.727273a19.549091 19.549091 0 0 1 6.516363 0z m578.094546 552.029091a27.461818 27.461818 0 0 0-2.792728 6.516363H154.530909l147.083636-272.290909a27.461818 27.461818 0 0 0 2.792728-6.981818h565.061818z", .LogoScale = 1.1, .Height = 24.0, .Width = 24.0 }
                        myIconButton4.ToolTip = "打开文件夹"
                        AddHandler myIconButton4.Click, Sub(a0 As Object, a1 As EventArgs)
                            MyBase._Lambda$__5()
                        End Sub
                        list.Add(myIconButton4)
                        myListItem.Buttons = list
                    Next
                    Dim myListItem3 As OurListItem = New OurListItem() With { .FontSize = 13.0, .Height = 24.0, .IsScaleAnimationEnabled = False, .Title = "导入电脑中已有的 Java…", .Type = OurListItem.CheckType.Clickable }
                    Dim uielement2 As UIElement = myListItem3
                    Dim mouseButtonEventHandler3 As MouseButtonEventHandler
                    If PageSetupLaunch._Closure$__.$I18-6 IsNot Nothing Then
                        mouseButtonEventHandler3 = PageSetupLaunch._Closure$__.$I18-6
                    Else
                        Dim mouseButtonEventHandler4 As MouseButtonEventHandler = Sub(sender As Object, e As MouseButtonEventArgs)
                            e.Handled = True
                            ModJava.ManuallyImportJava()
                        End Sub
                        mouseButtonEventHandler3 = mouseButtonEventHandler4
                        PageSetupLaunch._Closure$__.$I18-6 = mouseButtonEventHandler4
                    End If
                    AddHandler uielement2.MouseLeftButtonUp, mouseButtonEventHandler3
                    Me.ComboAdvanceJava.Items.Add(myListItem3)
                Catch ex As Exception
                    Logger.[Error](ex, "更新 Java 列表下拉框失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\PageSetupLaunch.xaml.vb")
                End Try
            End If
        End Sub

        Private Sub MoveJavaInList(JavaEntry As Java, Offset As Integer)
            Configs.JavaList.Edit(Sub(ByRef List As ConcurrentList(Of Java))
                Dim num As Integer = List.IndexOf(JavaEntry)
                Dim num2 As Integer = num + Offset
                If num >= 0 AndAlso num2 >= 0 AndAlso num2 < List.Count Then
                    List.RemoveAt(num)
                    List.Insert(num2, JavaEntry)
                End If
            End Sub)
            ModJava.UpdateJavaLists()
        End Sub

        Private Sub BtnAdvanceJavaSearch_Click()
            ModJava.ManuallySearchJava(Nothing)
        End Sub

        Private Sub WindowTypeUIRefresh()
            Dim visibility As Visibility = (Me.ComboArgumentWindowType.SelectedIndex = 3).ToVisibility()
            Me.TextArgumentWindowHeight.Visibility = visibility
            Me.LabArgumentWindowMiddle.Visibility = visibility
            Me.TextArgumentWindowWidth.Visibility = visibility
        End Sub

        Private Sub ComboArgumentVisibie_SizeChanged(sender As Object, e As SelectionChangedEventArgs)
            If ModAnimation.AniControlEnabled = 0 AndAlso Me.ComboArgumentVisibie.SelectedIndex = 0 AndAlso ModMain.OurMsgBox("若在游戏启动后立即关闭启动器，崩溃检测、更改游戏标题等功能将失效。" & vbCrLf & "如果想保留这些功能，可以选择让启动器在游戏启动后隐藏，游戏退出后自动关闭。", "提醒", "继续", "取消", "", False, True, False, Nothing, Nothing, Nothing) = 2 AndAlso e.RemovedItems.Count > 0 Then
                Me.ComboArgumentVisibie.SelectedItem = RuntimeHelpers.GetObjectValue(e.RemovedItems(0))
            End If
        End Sub

        Private Sub CheckArgumentRam_Change()
            If ModAnimation.AniControlEnabled = 0 AndAlso Me.CheckArgumentRam.Checked AndAlso ModMain.OurMsgBox("内存优化会显著延长启动耗时，建议仅在内存不足时开启。" & vbCrLf & "如果你在使用机械硬盘，这还可能导致一小段时间的严重卡顿。" + If(WindowsUtils.HasAdminRole(), "", String.Format("{0}{1}每次启动游戏，PCL 都需要申请管理员权限以进行内存优化。{2}若想自动授予权限，可以右键 PCL，打开 属性 → 兼容性 → 以管理员身份运行此程序。", vbCrLf, vbCrLf, vbCrLf)), "提醒", "确定", "取消", "", False, True, False, Nothing, Nothing, Nothing) = 2 Then
                Me.CheckArgumentRam.Checked = False
            End If
        End Sub

        Private Sub ComboArgumentIndie_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If ModAnimation.AniControlEnabled = 0 Then
                ModMain.OurMsgBox("本设置仅会对之后新安装的版本生效。" & vbCrLf & "如果要修改已安装的版本的隔离方式，请在它的版本独立设置中调整。", "提示", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
            End If
        End Sub

        Private Sub TextAdvanceRun_TextChanged(sender As Object, e As TextChangedEventArgs)
            Me.CheckAdvanceRunWait.Visibility = If((Operators.CompareString(Me.TextAdvanceRun.Text, "", False) = 0), Visibility.Collapsed, Visibility.Visible)
        End Sub

        Private Sub TextAdvanceJvm_TextChanged()
            Me.BtnAdvanceJvmReset.Visibility = If((Operators.CompareString(Me.TextAdvanceJvm.Text, Settings.GetDefault("LaunchAdvanceJvm"), False) = 0), Visibility.Hidden, Visibility.Visible)
        End Sub

        Private Sub BtnAdvanceJvmReset_Click(sender As Object, e As EventArgs)
            Settings.Reset("LaunchAdvanceJvm", Nothing)
            Me.Refresh()
        End Sub

        Private Sub ReplaceEnter(sender As OurTextBox, e As TextChangedEventArgs)
            Dim text As String = sender.Text.ReplaceLineEndings(" ", True)
            If Operators.CompareString(text, sender.Text, False) <> 0 Then
                Dim caretIndex As Integer = sender.CaretIndex
                sender.Text = text
                sender.CaretIndex = Math.Max(0, caretIndex - 1)
            End If
        End Sub

        Private Sub BtnSwitch_Click(sender As Object, e As MouseButtonEventArgs)
            ModMinecraft.McInstanceSelected.Load()
            PageInstanceLeft.Instance = ModMinecraft.McInstanceSelected
            ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSetup, FormMain.PageSubType.SetupLink)
        End Sub

        Friend Overridable Property PanBack As OurScrollViewer

        Friend Overridable Property PanMain As StackPanel

        Friend Overridable Property CardArgument As OurCard

        Friend Overridable Property ComboArgumentIndieV2 As OurComboBox
            <CompilerGenerated()>
            Get
                Return Me._ComboArgumentIndieV2
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurComboBox)
                Dim selectionChangedEventHandler As SelectionChangedEventHandler = AddressOf Me.ComboArgumentIndie_SelectionChanged
                Dim myComboBox As OurComboBox = Me._ComboArgumentIndieV2
                If myComboBox IsNot Nothing Then
                    RemoveHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
                Me._ComboArgumentIndieV2 = value
                myComboBox = Me._ComboArgumentIndieV2
                If myComboBox IsNot Nothing Then
                    AddHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ComboArgumentVisibie As OurComboBox
            <CompilerGenerated()>
            Get
                Return Me._ComboArgumentVisibie
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurComboBox)
                Dim selectionChangedEventHandler As SelectionChangedEventHandler = AddressOf Me.ComboArgumentVisibie_SizeChanged
                Dim myComboBox As OurComboBox = Me._ComboArgumentVisibie
                If myComboBox IsNot Nothing Then
                    RemoveHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
                Me._ComboArgumentVisibie = value
                myComboBox = Me._ComboArgumentVisibie
                If myComboBox IsNot Nothing Then
                    AddHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanArgumentWindow As Grid

        Friend Overridable Property ComboArgumentWindowType As OurComboBox
            <CompilerGenerated()>
            Get
                Return Me._ComboArgumentWindowType
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurComboBox)
                Dim selectionChangedEventHandler As SelectionChangedEventHandler = Sub(a0 As Object, a1 As SelectionChangedEventArgs)
                    Me.WindowTypeUIRefresh()
                End Sub
                Dim myComboBox As OurComboBox = Me._ComboArgumentWindowType
                If myComboBox IsNot Nothing Then
                    RemoveHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
                Me._ComboArgumentWindowType = value
                myComboBox = Me._ComboArgumentWindowType
                If myComboBox IsNot Nothing Then
                    AddHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property TextArgumentWindowWidth As OurTextBox

        Friend Overridable Property LabArgumentWindowMiddle As TextBlock

        Friend Overridable Property TextArgumentWindowHeight As OurTextBox

        Friend Overridable Property RadioRamType0 As OurRadioBox
            <CompilerGenerated()>
            Get
                Return Me._RadioRamType0
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurRadioBox)
                Dim checkEventHandler As IOurRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.RefreshRam()
                End Sub
                Dim myRadioBox As OurRadioBox = Me._RadioRamType0
                If myRadioBox IsNot Nothing Then
                    RemoveHandler myRadioBox.Check, checkEventHandler
                End If
                Me._RadioRamType0 = value
                myRadioBox = Me._RadioRamType0
                If myRadioBox IsNot Nothing Then
                    AddHandler myRadioBox.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property RadioRamType1 As OurRadioBox
            <CompilerGenerated()>
            Get
                Return Me._RadioRamType1
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurRadioBox)
                Dim checkEventHandler As IOurRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.RefreshRam()
                End Sub
                Dim myRadioBox As OurRadioBox = Me._RadioRamType1
                If myRadioBox IsNot Nothing Then
                    RemoveHandler myRadioBox.Check, checkEventHandler
                End If
                Me._RadioRamType1 = value
                myRadioBox = Me._RadioRamType1
                If myRadioBox IsNot Nothing Then
                    AddHandler myRadioBox.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property SliderRamCustom As OurSlider
            <CompilerGenerated()>
            Get
                Return Me._SliderRamCustom
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurSlider)
                Dim changeEventHandler As OurSlider.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.RefreshRam()
                End Sub
                Dim mySlider As OurSlider = Me._SliderRamCustom
                If mySlider IsNot Nothing Then
                    RemoveHandler mySlider.Change, changeEventHandler
                End If
                Me._SliderRamCustom = value
                mySlider = Me._SliderRamCustom
                If mySlider IsNot Nothing Then
                    AddHandler mySlider.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckArgumentRam As OurCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckArgumentRam
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurCheckBox)
                Dim changeEventHandler As OurCheckBox.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.CheckArgumentRam_Change()
                End Sub
                Dim myCheckBox As OurCheckBox = Me._CheckArgumentRam
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckArgumentRam = value
                myCheckBox = Me._CheckArgumentRam
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanRamDisplay As Grid

        Friend Overridable Property ColumnRamUsed As ColumnDefinition

        Friend Overridable Property ColumnRamGame As ColumnDefinition

        Friend Overridable Property ColumnRamEmpty As ColumnDefinition

        Friend Overridable Property RectRamUsed As Global.System.Windows.Shapes.Rectangle

        Friend Overridable Property RectRamGame As Global.System.Windows.Shapes.Rectangle
            <CompilerGenerated()>
            Get
                Return Me._RectRamGame
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Global.System.Windows.Shapes.Rectangle)
                Dim sizeChangedEventHandler As SizeChangedEventHandler = Sub(a0 As Object, a1 As SizeChangedEventArgs)
                    Me.RefreshRamText()
                End Sub
                Dim rectangle As Global.System.Windows.Shapes.Rectangle = Me._RectRamGame
                If rectangle IsNot Nothing Then
                    RemoveHandler rectangle.SizeChanged, sizeChangedEventHandler
                End If
                Me._RectRamGame = value
                rectangle = Me._RectRamGame
                If rectangle IsNot Nothing Then
                    AddHandler rectangle.SizeChanged, sizeChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property RectRamEmpty As Global.System.Windows.Shapes.Rectangle
            <CompilerGenerated()>
            Get
                Return Me._RectRamEmpty
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Global.System.Windows.Shapes.Rectangle)
                Dim sizeChangedEventHandler As SizeChangedEventHandler = Sub(a0 As Object, a1 As SizeChangedEventArgs)
                    Me.RefreshRamText()
                End Sub
                Dim rectangle As Global.System.Windows.Shapes.Rectangle = Me._RectRamEmpty
                If rectangle IsNot Nothing Then
                    RemoveHandler rectangle.SizeChanged, sizeChangedEventHandler
                End If
                Me._RectRamEmpty = value
                rectangle = Me._RectRamEmpty
                If rectangle IsNot Nothing Then
                    AddHandler rectangle.SizeChanged, sizeChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabRamUsedTitle As TextBlock

        Friend Overridable Property LabRamGameTitle As TextBlock

        Friend Overridable Property LabRamUsed As TextBlock

        Friend Overridable Property LabRamTotal As TextBlock

        Friend Overridable Property LabRamGame As TextBlock
            <CompilerGenerated()>
            Get
                Return Me._LabRamGame
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As TextBlock)
                Dim sizeChangedEventHandler As SizeChangedEventHandler = Sub(a0 As Object, a1 As SizeChangedEventArgs)
                    Me.RefreshRamText()
                End Sub
                Dim textBlock As TextBlock = Me._LabRamGame
                If textBlock IsNot Nothing Then
                    RemoveHandler textBlock.SizeChanged, sizeChangedEventHandler
                End If
                Me._LabRamGame = value
                textBlock = Me._LabRamGame
                If textBlock IsNot Nothing Then
                    AddHandler textBlock.SizeChanged, sizeChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CardSkin As OurCard

        Friend Overridable Property RadioSkinType0 As OurRadioBox

        Friend Overridable Property RadioSkinType1 As OurRadioBox

        Friend Overridable Property RadioSkinType2 As OurRadioBox

        Friend Overridable Property RadioSkinType3 As OurRadioBox

        Friend Overridable Property RadioSkinType4 As OurRadioBox
            <CompilerGenerated()>
            Get
                Return Me._RadioSkinType4
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurRadioBox)
                Dim previewCheckEventHandler As OurRadioBox.PreviewCheckEventHandler = AddressOf Me.RadioSkinType3_Check
                Dim myRadioBox As OurRadioBox = Me._RadioSkinType4
                If myRadioBox IsNot Nothing Then
                    RemoveHandler myRadioBox.PreviewCheck, previewCheckEventHandler
                End If
                Me._RadioSkinType4 = value
                myRadioBox = Me._RadioSkinType4
                If myRadioBox IsNot Nothing Then
                    AddHandler myRadioBox.PreviewCheck, previewCheckEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanSkinID As Grid

        Friend Overridable Property BtnSkinSave As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSkinSave
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnSkinSave_Click
                Dim myButton As OurButton = Me._BtnSkinSave
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnSkinSave = value
                myButton = Me._BtnSkinSave
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnSkinCache As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSkinCache
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnSkinCache_Click
                Dim myButton As OurButton = Me._BtnSkinCache
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnSkinCache = value
                myButton = Me._BtnSkinCache
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanSkinChange As Grid

        Friend Overridable Property BtnSkinChange As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSkinChange
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnSkinChange_Click
                Dim myButton As OurButton = Me._BtnSkinChange
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnSkinChange = value
                myButton = Me._BtnSkinChange
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnSkinDelete As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSkinDelete
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnSkinDelete_Click
                Dim myButton As OurButton = Me._BtnSkinDelete
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnSkinDelete = value
                myButton = Me._BtnSkinDelete
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnAdvanceJvmReset As OurIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnAdvanceJvmReset
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconButton)
                Dim clickEventHandler As OurIconButton.ClickEventHandler = AddressOf Me.BtnAdvanceJvmReset_Click
                Dim myIconButton As OurIconButton = Me._BtnAdvanceJvmReset
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnAdvanceJvmReset = value
                myIconButton = Me._BtnAdvanceJvmReset
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property TextAdvanceJvm As OurTextBox
            <CompilerGenerated()>
            Get
                Return Me._TextAdvanceJvm
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurTextBox)
                Dim routedEventHandler As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    Me.TextAdvanceJvm_TextChanged()
                End Sub
                Dim textChangedEventHandler As TextChangedEventHandler = Sub(a0 As Object, a1 As TextChangedEventArgs)
                    Me.ReplaceEnter(CType(a0, OurTextBox), a1)
                End Sub
                Dim myTextBox As OurTextBox = Me._TextAdvanceJvm
                If myTextBox IsNot Nothing Then
                    RemoveHandler myTextBox.ValidatedTextChanged, routedEventHandler
                    RemoveHandler myTextBox.TextChanged, textChangedEventHandler
                End If
                Me._TextAdvanceJvm = value
                myTextBox = Me._TextAdvanceJvm
                If myTextBox IsNot Nothing Then
                    AddHandler myTextBox.ValidatedTextChanged, routedEventHandler
                    AddHandler myTextBox.TextChanged, textChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property TextAdvanceGame As OurTextBox
            <CompilerGenerated()>
            Get
                Return Me._TextAdvanceGame
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurTextBox)
                Dim textChangedEventHandler As TextChangedEventHandler = Sub(a0 As Object, a1 As TextChangedEventArgs)
                    Me.ReplaceEnter(CType(a0, OurTextBox), a1)
                End Sub
                Dim myTextBox As OurTextBox = Me._TextAdvanceGame
                If myTextBox IsNot Nothing Then
                    RemoveHandler myTextBox.TextChanged, textChangedEventHandler
                End If
                Me._TextAdvanceGame = value
                myTextBox = Me._TextAdvanceGame
                If myTextBox IsNot Nothing Then
                    AddHandler myTextBox.TextChanged, textChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property TextAdvanceRun As OurTextBox
            <CompilerGenerated()>
            Get
                Return Me._TextAdvanceRun
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurTextBox)
                Dim textChangedEventHandler As TextChangedEventHandler = AddressOf Me.TextAdvanceRun_TextChanged
                Dim myTextBox As OurTextBox = Me._TextAdvanceRun
                If myTextBox IsNot Nothing Then
                    RemoveHandler myTextBox.TextChanged, textChangedEventHandler
                End If
                Me._TextAdvanceRun = value
                myTextBox = Me._TextAdvanceRun
                If myTextBox IsNot Nothing Then
                    AddHandler myTextBox.TextChanged, textChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckAdvanceRunWait As OurCheckBox

        Friend Overridable Property PanAdvanceJava As Grid

        Friend Overridable Property ComboAdvanceJava As OurComboBox

        Friend Overridable Property LabAdvanceJava As TextBlock

        Friend Overridable Property BtnAdvanceJavaSearch As OurIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnAdvanceJavaSearch
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconButton)
                Dim clickEventHandler As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.BtnAdvanceJavaSearch_Click()
                End Sub
                Dim myIconButton As OurIconButton = Me._BtnAdvanceJavaSearch
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnAdvanceJavaSearch = value
                myIconButton = Me._BtnAdvanceJavaSearch
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnSwitch As OurExtraTextButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSwitch
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurExtraTextButton)
                Dim clickEventHandler As OurExtraTextButton.ClickEventHandler = AddressOf Me.BtnSwitch_Click
                Dim myExtraTextButton As OurExtraTextButton = Me._BtnSwitch
                If myExtraTextButton IsNot Nothing Then
                    RemoveHandler myExtraTextButton.Click, clickEventHandler
                End If
                Me._BtnSwitch = value
                myExtraTextButton = Me._BtnSwitch
                If myExtraTextButton IsNot Nothing Then
                    AddHandler myExtraTextButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Private RamTextLeft As Integer

        Private RamTextRight As Integer

        Private $STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded As Boolean

        Private $STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init As StaticLocalInitFlag
    End Class
End Namespace
