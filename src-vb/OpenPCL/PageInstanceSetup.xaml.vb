Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
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
Imports System.Windows.Shapes
Imports System.Windows.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports PCL.My
Imports PCLCS

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageInstanceSetup
        Inherits OurPageRight

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageSetupSystem_Loaded
            Me.LoadedFirst = False
            Me.RamTextLeft = 2
            Me.RamTextRight = 1
            Me.InitializeComponent()
        End Sub

        Private Sub PageSetupSystem_Loaded(sender As Object, e As RoutedEventArgs)
            Me.PanBack.ScrollToHome()
            Me.RefreshRam(False)
            ModAnimation.AniControlEnabled += 1
            Me.Reload()
            ModAnimation.AniControlEnabled -= 1
            If Not Me.LoadedFirst Then
                Me.LoadedFirst = True
                AddHandler PageInstanceLeft.CurrentJavaWorker.Started, AddressOf Me.UpdateJavaRelatedUi
                AddHandler PageInstanceLeft.CurrentJavaWorker.Stopped, AddressOf Me.UpdateJavaRelatedUi
                Dim textArgumentJavaRange As OurTextBox = Me.TextArgumentJavaRange
                Dim collection As Collection(Of Validate) = New Collection(Of Validate)()
                Dim func As Func(Of String, String)
                If PageInstanceSetup._Closure$__.$I2-0 IsNot Nothing Then
                    func = PageInstanceSetup._Closure$__.$I2-0
                Else
                    Dim func2 As Func(Of String, String) = Function(Str As String)
                        Dim text As String
                        Try
                            If String.IsNullOrWhiteSpace(Str) Then
                                text = "不能为空"
                            Else
                                Dim func3 As Func(Of String, Version)
                                If PageInstanceSetup._Closure$__.$I2-1 IsNot Nothing Then
                                    func3 = PageInstanceSetup._Closure$__.$I2-1
                                Else
                                    Dim func4 As Func(Of String, Version) = Function(s As String) StringUtils.ParseVersionWithDefaults(s)
                                    func3 = func4
                                    PageInstanceSetup._Closure$__.$I2-1 = func4
                                End If
                                Dim valueRange As ValueRange(Of Version) = ValueRange(Of Version).FromString(Str, func3)
                                If valueRange.IsEmpty() Then
                                    text = "范围下限的版本号比上限更高，导致该范围无法匹配到任何版本"
                                ElseIf valueRange.HasLower AndAlso valueRange.Lower.Major <= 1 AndAlso (valueRange.Lower.Minor > 0 OrElse valueRange.Lower.Build > 0 OrElse valueRange.Lower.Revision > 0) Then
                                    text = "不应使用 1.x 格式进行匹配（例如，若想匹配 Java 17，应填写 17 而非 1.17）"
                                ElseIf valueRange.HasUpper AndAlso valueRange.Upper.Major <= 1 AndAlso (valueRange.Upper.Minor > 0 OrElse valueRange.Upper.Build > 0 OrElse valueRange.Upper.Revision > 0) Then
                                    text = "不应使用 1.x 格式进行匹配（例如，若想匹配 Java 17，应填写 17 而非 1.17）"
                                ElseIf valueRange.HasUpper AndAlso valueRange.Upper.Major <= 4 Then
                                    text = "该范围所要求的 Java 版本过低"
                                ElseIf valueRange.HasUpper AndAlso valueRange.IsUpperInclusive AndAlso valueRange.Upper.Minor <= 0 AndAlso valueRange.Upper.Build <= 0 Then
                                    text = String.Format("右侧闭区间的意图并不明确。如果不想允许 Java {0}，请改为 {1}{2}){3}。如果想允许 Java {4}，请改为 {5}{6}){7}。", New Object() { valueRange.Upper.Major, ModBase.vbLQ, valueRange.Upper.Major, ModBase.vbRQ, valueRange.Upper.Major, ModBase.vbLQ, valueRange.Upper.Major + 1, ModBase.vbRQ })
                                ElseIf valueRange.Intersect(ValueRange(Of Version).OpenClosed(New Version(5, 0), New Version(99, 0))).IsEmpty() Then
                                    text = "该范围无法匹配到任何常见的 Java"
                                Else
                                    text = ""
                                End If
                            End If
                        Catch ex As Exception
                            text = ex.Message
                        End Try
                        Return text
                    End Function
                    func = func2
                    PageInstanceSetup._Closure$__.$I2-0 = func2
                End If
                collection.Add(New ValidateFunc(func))
                textArgumentJavaRange.ValidateRules = collection
                Dim dispatcherTimer As DispatcherTimer = New DispatcherTimer()
                dispatcherTimer.Interval = New TimeSpan(0, 0, 0, 1)
                AddHandler dispatcherTimer.Tick, Sub(a0 As Object, a1 As EventArgs)
                    Me.RefreshRam()
                End Sub
                dispatcherTimer.Start()
            End If
        End Sub

        Public Sub Reload()
            Try
                Dim pathIndie As String = PageInstanceLeft.Instance.PathIndie
                Me.ComboArgumentIndieV2.SelectedIndex = If((Not Settings.[Get](Of Boolean)("VersionArgumentIndieV2", PageInstanceLeft.Instance)), 1, 0)
                Me.UpdateJavaList()
                Me.ComboServerLogin.SelectedIndex = Settings.[Get](Of Integer)("VersionServerLogin", PageInstanceLeft.Instance)
                Me.ComboServerLoginLast = Me.ComboServerLogin.SelectedIndex
                Me.UpdateServerLoginUI()
                If Settings.[Get](Of Integer)("VersionAdvanceAssets", PageInstanceLeft.Instance) = 2 Then
                    Logger.Info("已迁移老版本的关闭文件校验设置", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceSetup.xaml.vb")
                    Settings.Reset("VersionAdvanceAssets", PageInstanceLeft.Instance)
                    Settings.[Set]("VersionAdvanceAssetsV2", True, PageInstanceLeft.Instance)
                End If
                SettingService.RefreshSettings(Me)
                PageInstanceSetup.OnVersionRamTypeChanged(Settings.[Get](Of Integer)("VersionRamType", PageInstanceLeft.Instance))
            Catch ex As Exception
                Logger.[Error](ex, "重载版本独立设置时出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceSetup.xaml.vb")
            End Try
        End Sub

        Public Sub Reset()
            Try
                SettingService.ResetSettings(Me)
                Settings.Reset("VersionServerLogin", PageInstanceLeft.Instance)
                Settings.Reset("VersionArgumentIndieV2", PageInstanceLeft.Instance)
                Settings.Reset("VersionAdvanceAssets", PageInstanceLeft.Instance)
                Configs.JavaForced.Reset(PageInstanceLeft.Instance.Config)
                ModJava.JavaListRefreshWorker.Start(Nothing, Nothing)
                Logger.Info("已初始化版本独立设置", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceSetup.xaml.vb")
                ModMain.Hint("已初始化版本独立设置！", ModMain.HintType.Green, False)
            Catch ex As Exception
                Logger.[Error](ex, "初始化版本独立设置失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceSetup.xaml.vb")
            End Try
            Me.Reload()
        End Sub

        Public Shared Sub OnVersionRamTypeChanged(Type As Integer)
            If ModMain.FrmInstanceSetup IsNot Nothing Then
                ModMain.FrmInstanceSetup.RamType(Type)
            End If
        End Sub

        Public Sub RamType(Type As Integer)
            If Me.SliderRamCustom IsNot Nothing Then
                Me.SliderRamCustom.IsEnabled = Type = 1
            End If
        End Sub

        Public Sub RefreshRam(ShowAnim As Boolean)
            If Me.LabRamGame IsNot Nothing AndAlso Me.LabRamUsed IsNot Nothing AndAlso Not(ModMain.FrmMain.PageCurrent IsNot FormMain.PageType.InstanceSetup) AndAlso ModMain.FrmInstanceLeft.PageID = FormMain.PageSubType.SetupLink Then
                Dim num As Double = Math.Round(PageInstanceSetup.GetRam(PageInstanceLeft.Instance), 5)
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
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaGridLengthWidth(Me.ColumnRamUsed, num5 - Me.ColumnRamUsed.Width.Value, 800, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False), ModAnimation.AaGridLengthWidth(Me.ColumnRamGame, num4 - Me.ColumnRamGame.Width.Value, 800, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False), ModAnimation.AaGridLengthWidth(Me.ColumnRamEmpty, num6 - Me.ColumnRamEmpty.Width.Value, 800, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False) }, "VersionSetup Ram Grid", False)
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
                        ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.LabRamUsed, -Me.LabRamUsed.Opacity, 100, 0, Nothing, False), ModAnimation.AaOpacity(Me.LabRamTotal, -Me.LabRamTotal.Opacity, 100, 0, Nothing, False), ModAnimation.AaOpacity(Me.LabRamUsedTitle, -Me.LabRamUsedTitle.Opacity, 100, 0, Nothing, False) }, "VersionSetup Ram TextLeft", False)
                    Case 1
                        ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.LabRamUsed, 1.0 - Me.LabRamUsed.Opacity, 100, 0, Nothing, False), ModAnimation.AaOpacity(Me.LabRamTotal, -Me.LabRamTotal.Opacity, 100, 0, Nothing, False), ModAnimation.AaOpacity(Me.LabRamUsedTitle, 0.7 - Me.LabRamUsedTitle.Opacity, 100, 0, Nothing, False) }, "VersionSetup Ram TextLeft", False)
                    Case 2
                        ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.LabRamUsed, 1.0 - Me.LabRamUsed.Opacity, 100, 0, Nothing, False), ModAnimation.AaOpacity(Me.LabRamTotal, 1.0 - Me.LabRamTotal.Opacity, 100, 0, Nothing, False), ModAnimation.AaOpacity(Me.LabRamUsedTitle, 0.7 - Me.LabRamUsedTitle.Opacity, 100, 0, Nothing, False) }, "VersionSetup Ram TextLeft", False)
                End Select
            End If
            Dim num2 As Integer
            If actualWidth2 < actualWidth3 + 2.0 + actualWidth OrElse actualWidth2 < actualWidth6 + 2.0 + actualWidth Then
                num2 = 0
            Else
                num2 = 1
            End If
            If num2 = 0 Then
                If ModAnimation.AniControlEnabled = 0 AndAlso (Me.RamTextRight <> num2 OrElse ModAnimation.AniIsRun("VersionSetup Ram TextRight")) Then
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaX(Me.LabRamGame, actualWidth2 - actualWidth3 - Me.LabRamGame.Margin.Left, 100, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaX(Me.LabRamGameTitle, actualWidth2 - actualWidth6 - Me.LabRamGameTitle.Margin.Left, 100, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False) }, "VersionSetup Ram TextRight", False)
                Else
                    Me.LabRamGame.Margin = New Thickness(actualWidth2 - actualWidth3, 3.0, 0.0, 0.0)
                    Me.LabRamGameTitle.Margin = New Thickness(actualWidth2 - actualWidth6, 0.0, 0.0, 5.0)
                End If
            ElseIf ModAnimation.AniControlEnabled = 0 AndAlso (Me.RamTextRight <> num2 OrElse ModAnimation.AniIsRun("VersionSetup Ram TextRight")) Then
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaX(Me.LabRamGame, 2.0 + actualWidth - Me.LabRamGame.Margin.Left, 100, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaX(Me.LabRamGameTitle, 2.0 + actualWidth - Me.LabRamGameTitle.Margin.Left, 100, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False) }, "VersionSetup Ram TextRight", False)
            Else
                Me.LabRamGame.Margin = New Thickness(2.0 + actualWidth, 3.0, 0.0, 0.0)
                Me.LabRamGameTitle.Margin = New Thickness(2.0 + actualWidth, 0.0, 0.0, 5.0)
            End If
            Me.RamTextRight = num2
        End Sub

        Public Shared Function GetRam(Instance As ModMinecraft.McInstance) As Double
            Dim num As Double
            If Settings.[Get](Of Integer)("VersionRamType", Instance) = 2 Then
                num = PageSetupLaunch.GetRam(Instance, True)
            Else
                Dim num10 As Double
                If Settings.[Get](Of Integer)("VersionRamType", Instance) = 0 Then
                    Dim num2 As Double = Math.Round(MyWpfExtension.Computer.Info.AvailablePhysicalMemory / 1024.0 / 1024.0 / 1024.0 * 10.0) / 10.0
                    If Instance IsNot Nothing AndAlso Not Instance.IsLoaded Then
                        Instance.Load()
                    End If
                    Dim num5 As Double
                    Dim num6 As Double
                    Dim num7 As Double
                    Dim num8 As Double
                    If Instance IsNot Nothing AndAlso Instance.Modable Then
                        Dim info As DirectoryInfo = DirectoryUtils.GetInfo(Instance.PathIndie + "mods\")
                        Dim num3 As Integer
                        If Not info.Exists Then
                            num3 = 0
                        Else
                            Dim files As IEnumerable(Of FileInfo) = info.GetFiles()
                            Dim func As Func(Of FileInfo, Boolean)
                            If PageInstanceSetup._Closure$__.$I12-0 IsNot Nothing Then
                                func = PageInstanceSetup._Closure$__.$I12-0
                            Else
                                Dim func2 As Func(Of FileInfo, Boolean) = Function(f As FileInfo) New String() { ".jar", ".zip", ".litemod" }.Contains(f.Extension.Lower())
                                func = func2
                                PageInstanceSetup._Closure$__.$I12-0 = func2
                            End If
                            num3 = files.Count(func)
                        End If
                        Dim num4 As Integer = num3
                        num5 = 0.5 + CDbl(num4) / 150.0
                        num6 = 1.5 + CDbl(num4) / 90.0
                        num7 = 2.7 + CDbl(num4) / 50.0
                        num8 = 4.5 + CDbl(num4) / 25.0
                    ElseIf Instance IsNot Nothing AndAlso Instance.Version.HasOptiFine Then
                        num5 = 0.5
                        num6 = 1.5
                        num7 = 3.0
                        num8 = 5.0
                    Else
                        num5 = 0.5
                        num6 = 1.5
                        num7 = 2.5
                        num8 = 4.0
                    End If
                    Dim num9 As Double = num6
                    num10 += Math.Min(num2, num9)
                    num2 -= num9
                    If num2 >= 0.1 Then
                        num9 = num7 - num6
                        num10 += Math.Min(num2 * 0.7, num9)
                        num2 -= num9 / 0.7
                        If num2 >= 0.1 Then
                            num9 = num8 - num7
                            num10 += Math.Min(num2 * 0.4, num9)
                            num2 -= num9 / 0.4
                            If num2 >= 0.1 Then
                                num9 = num8
                                num10 += Math.Min(num2 * 0.15, num9)
                                num2 -= num9 / 0.15
                            End If
                        End If
                    End If
                    num10 = Math.Round(Math.Max(num10, num5), 1)
                Else
                    Dim num11 As Integer = Settings.[Get](Of Integer)("VersionRamCustom", Instance)
                    If num11 <= 12 Then
                        num10 = CDbl(num11) * 0.1 + 0.3
                    ElseIf num11 <= 25 Then
                        ' The following expression was wrapped in a checked-expression
                        num10 = CDbl((num11 - 12)) * 0.5 + 1.5
                    ElseIf num11 <= 33 Then
                        ' The following expression was wrapped in a checked-expression
                        num10 = CDbl(((num11 - 25) * 1 + 8))
                    Else
                        ' The following expression was wrapped in a checked-expression
                        num10 = CDbl(((num11 - 33) * 2 + 16))
                    End If
                End If
                num = num10
            End If
            Return num
        End Function

        Public Shared Sub OnVersionServerLoginChanged(Type As Integer)
            If ModMain.FrmInstanceSetup IsNot Nothing Then
                ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "InstanceCache", "")
                If PageInstanceLeft.Instance IsNot Nothing Then
                    PageInstanceLeft.Instance = New ModMinecraft.McInstance(PageInstanceLeft.Instance.Name).Load()
                    ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
                End If
            End If
        End Sub

        Private Sub TextServerEnter_Change()
            Dim text As String = Me.TextServerEnter.Text.Replace("：", ":").Replace("。", ".")
            If Operators.CompareString(text, Me.TextServerEnter.Text, False) <> 0 Then
                Dim selectionStart As Integer = Me.TextServerEnter.SelectionStart
                Me.TextServerEnter.Text = text
                Me.TextServerEnter.SelectionStart = selectionStart
            End If
        End Sub

        Private Sub ComboServerLogin_Changed()
            If ModAnimation.AniControlEnabled = 0 Then
                Me.UpdateServerLoginUI()
                If(Me.ComboServerLogin.SelectedIndex <> 3 OrElse Me.TextServerNide.IsValidated) AndAlso (Me.ComboServerLogin.SelectedIndex <> 4 OrElse Me.TextServerAuthServer.IsValidated) AndAlso Me.ComboServerLoginLast <> Me.ComboServerLogin.SelectedIndex Then
                    Me.ComboServerLoginLast = Me.ComboServerLogin.SelectedIndex
                    Settings.[Set]("VersionServerLogin", Me.ComboServerLogin.SelectedIndex, PageInstanceLeft.Instance)
                End If
            End If
        End Sub

        Private Sub UpdateServerLoginUI()
            If Me.LabServerNide IsNot Nothing Then
                Dim selectedIndex As Integer = Me.ComboServerLogin.SelectedIndex
                Me.LabServerNide.Visibility = If((selectedIndex = 3), Visibility.Visible, Visibility.Collapsed)
                Me.TextServerNide.Visibility = If((selectedIndex = 3), Visibility.Visible, Visibility.Collapsed)
                Me.PanServerNide.Visibility = If((selectedIndex = 3), Visibility.Visible, Visibility.Collapsed)
                Me.LabServerAuthName.Visibility = If((selectedIndex = 4), Visibility.Visible, Visibility.Collapsed)
                Me.TextServerAuthName.Visibility = If((selectedIndex = 4), Visibility.Visible, Visibility.Collapsed)
                Me.LabServerAuthRegister.Visibility = If((selectedIndex = 4), Visibility.Visible, Visibility.Collapsed)
                Me.TextServerAuthRegister.Visibility = If((selectedIndex = 4), Visibility.Visible, Visibility.Collapsed)
                Me.LabServerAuthServer.Visibility = If((selectedIndex = 4), Visibility.Visible, Visibility.Collapsed)
                Me.TextServerAuthServer.Visibility = If((selectedIndex = 4), Visibility.Visible, Visibility.Collapsed)
                Me.BtnServerAuthLittle.Visibility = If((selectedIndex = 4), Visibility.Visible, Visibility.Collapsed)
                Me.CardServer.TriggerForceResize()
            End If
        End Sub

        Private Sub BtnServerAuthLittle_Click(sender As Object, e As EventArgs)
            If Operators.CompareString(Me.TextServerAuthServer.Text, "", False) = 0 OrElse Operators.CompareString(Me.TextServerAuthServer.Text, "https://littleskin.cn/api/yggdrasil", False) = 0 OrElse ModMain.OurMsgBox("即将把第三方登录设置覆盖为 LittleSkin 登录。" & vbCrLf & "除非你是服主，或者服主要求你这样做，否则请不要继续。" & vbCrLf & vbCrLf & "是否确实需要覆盖当前设置？", "设置覆盖确认", "继续", "取消", "", False, True, False, Nothing, Nothing, Nothing) <> 2 Then
                Me.TextServerAuthServer.Text = "https://littleskin.cn/api/yggdrasil"
                Me.TextServerAuthRegister.Text = "https://littleskin.cn/auth/register"
                Me.TextServerAuthName.Text = "LittleSkin 登录"
            End If
        End Sub

        Private Sub ReloadCurrentJava()
            PageInstanceLeft.ReloadCurrentJava()
        End Sub

        Private Sub UpdateJavaRelatedUi()
            ModBase.RunInUi(AddressOf Me._UpdateJavaRelatedUi, False)
        End Sub

        Private Sub _UpdateJavaRelatedUi()
            Dim currentJavaWorker As RedoableWorker(Of Java) = PageInstanceLeft.CurrentJavaWorker
            Select Case Me.ComboArgumentJava.SelectedIndex
                Case 0, 2
                    Me.HintArgumentJava.Visibility = Visibility.Visible
                    Me.TextArgumentJavaRange.Visibility = Visibility.Collapsed
                    Me.PanArgumentJavaSelect.Visibility = Visibility.Collapsed
                    If Not currentJavaWorker.LastSucceeded OrElse currentJavaWorker.Running Then
                        If Not currentJavaWorker.Running Then
                            currentJavaWorker.Start(Nothing, Nothing)
                        End If
                        Me.HintArgumentJava.Text = "正在查找 Java……"
                        Me.HintArgumentJava.Theme = OurHint.Themes.Blue
                        Return
                    End If
                    If currentJavaWorker.LastResult IsNot Nothing Then
                        Me.HintArgumentJava.Text = String.Format("将会使用：{0}", currentJavaWorker.LastResult)
                        Me.HintArgumentJava.Theme = OurHint.Themes.Blue
                        Return
                    End If
                    If Me.ComboArgumentJava.SelectedIndex = 0 Then
                        Me.HintArgumentJava.Text = "你的电脑上没有可供该版本使用的 Java，PCL 会在启动游戏时自动下载。"
                        Me.HintArgumentJava.Theme = OurHint.Themes.Yellow
                        Return
                    End If
                    Me.HintArgumentJava.Text = String.Format("该版本的版本文件夹中没有发现任何 Java！{0}请点击此处打开版本文件夹，然后将 Java 文件夹复制进去。", vbCrLf)
                    Me.HintArgumentJava.Theme = OurHint.Themes.Red
                    Return
                Case 1
                    Me.HintArgumentJava.Visibility = Visibility.Visible
                    Me.TextArgumentJavaRange.Visibility = Visibility.Visible
                    Me.PanArgumentJavaSelect.Visibility = Visibility.Collapsed
                    If Not Settings.HasSaved("VersionArgumentJavaRange", PageInstanceLeft.Instance) Then
                        Dim text As String = ModJava.GetJavaRequirement(PageInstanceLeft.Instance).Item1.ToString().Replace("-∞", "").Replace("+∞", "")
                        Settings.[Set]("VersionArgumentJavaRange", text, PageInstanceLeft.Instance)
                        Me.TextArgumentJavaRange.Text = text
                    End If
                    If Not String.IsNullOrEmpty(Me.TextArgumentJavaRange.ValidateResult) Then
                        Me.HintArgumentJava.Text = String.Format("Java 版本范围错误：{0}", Me.TextArgumentJavaRange.ValidateResult)
                        Me.HintArgumentJava.Theme = OurHint.Themes.Red
                        Return
                    End If
                    If Not currentJavaWorker.LastSucceeded OrElse currentJavaWorker.Running Then
                        If Not currentJavaWorker.Running Then
                            currentJavaWorker.Start(Nothing, Nothing)
                        End If
                        Me.HintArgumentJava.Text = "正在查找 Java……"
                        Me.HintArgumentJava.Theme = OurHint.Themes.Blue
                        Return
                    End If
                    If currentJavaWorker.LastResult Is Nothing Then
                        Me.HintArgumentJava.Text = String.Format("你的电脑上没有任何 Java 符合该版本范围！{0}如果 Mojang 提供了对应版本的 Java，PCL 会在启动游戏时自动下载。", vbCrLf)
                        Me.HintArgumentJava.Theme = OurHint.Themes.Yellow
                        Return
                    End If
                    Me.HintArgumentJava.Text = String.Format("将会使用：{0}", currentJavaWorker.LastResult)
                    Me.HintArgumentJava.Theme = OurHint.Themes.Blue
                    Return
                Case 3
                    Me.HintArgumentJava.Visibility = Visibility.Collapsed
                    Me.TextArgumentJavaRange.Visibility = Visibility.Collapsed
                    Me.PanArgumentJavaSelect.Visibility = Visibility.Visible
                    Return
                Case Else
                    Return
            End Select
        End Sub

        Private Sub HintArgumentJava_Click(sender As Object, e As MouseButtonEventArgs)
            If Me.ComboArgumentJava.SelectedIndex = 2 Then
                PageInstanceOverall.OpenInstanceFolder(PageInstanceLeft.Instance)
            End If
        End Sub

        Public Sub UpdateJavaList()
            If Me.ComboArgumentJavaSelect IsNot Nothing AndAlso Me.LabArgumentJava IsNot Nothing AndAlso Me.ComboArgumentJavaSelect.IsLoaded Then
                Me.ComboArgumentJavaSelect.Items.Clear()
                If ModJava.JavaListRefreshWorker.Running Then
                    Me.BtnArgumentJavaSearch.IsEnabled = False
                    Me.ComboArgumentJavaSelect.IsEnabled = False
                    Me.LabArgumentJava.Text = "搜索中 …"
                    Return
                End If
                Me.BtnArgumentJavaSearch.IsEnabled = True
                Me.ComboArgumentJavaSelect.IsEnabled = True
                Dim java As Java = Configs.JavaForced.[Get](PageInstanceLeft.Instance.Config)
                If Not Configs.JavaList.[Get]().Any() Then
                    Me.LabArgumentJava.Text = "未找到 Java，点击以导入已有的 Java"
                ElseIf java Is Nothing Then
                    Me.LabArgumentJava.Text = "点击选择 Java …"
                Else
                    Me.LabArgumentJava.Text = java.ToString()
                End If
                Try
                    Try
                        Dim enumerator As IEnumerator(Of Java) = Configs.JavaList.[Get]().GetEnumerator()
                        While enumerator.MoveNext()
                            Dim CS$<>8__locals1 As PageInstanceSetup._Closure$__23-0 = New PageInstanceSetup._Closure$__23-0(CS$<>8__locals1)
                            CS$<>8__locals1.$VB$Me = Me
                            CS$<>8__locals1.$VB$Local_JavaEntry = enumerator.Current
                            Dim myListItem As OurListItem = New OurListItem() With { .FontSize = 13.0, .Height = 24.0, .IsScaleAnimationEnabled = False, .Type = OurListItem.CheckType.Clickable, .Tag = CS$<>8__locals1.$VB$Local_JavaEntry, .Title = CS$<>8__locals1.$VB$Local_JavaEntry.ToString() }
                            Dim uielement As UIElement = myListItem
                            Dim mouseButtonEventHandler As MouseButtonEventHandler
                            If PageInstanceSetup._Closure$__.$I23-0 IsNot Nothing Then
                                mouseButtonEventHandler = PageInstanceSetup._Closure$__.$I23-0
                            Else
                                Dim mouseButtonEventHandler2 As MouseButtonEventHandler = Sub(sender As Object, e As MouseButtonEventArgs)
                                    e.Handled = True
                                End Sub
                                mouseButtonEventHandler = mouseButtonEventHandler2
                                PageInstanceSetup._Closure$__.$I23-0 = mouseButtonEventHandler2
                            End If
                            AddHandler uielement.MouseLeftButtonUp, mouseButtonEventHandler
                            AddHandler myListItem.Click, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                                MyBase._Lambda$__1()
                            End Sub
                            Me.ComboArgumentJavaSelect.Items.Add(myListItem)
                            Dim list As List(Of OurIconButton) = New List(Of OurIconButton)()
                            Dim flag As Boolean = PathUtils.IsParentOf(String.Format("{0}.minecraft\runtime\", Paths.AppData), CS$<>8__locals1.$VB$Local_JavaEntry.Folder)
                            Dim myIconButton As OurIconButton = New OurIconButton() With { .Logo = "M508 990.4c-261.6 0-474.4-212-474.4-474.4S246.4 41.6 508 41.6s474.4 212 474.4 474.4S769.6 990.4 508 990.4zM508 136.8c-209.6 0-379.2 169.6-379.2 379.2 0 209.6 169.6 379.2 379.2 379.2s379.2-169.6 379.2-379.2C887.2 306.4 717.6 136.8 508 136.8zM697.6 563.2 318.4 563.2c-26.4 0-47.2-21.6-47.2-47.2 0-26.4 21.6-47.2 47.2-47.2l379.2 0c26.4 0 47.2 21.6 47.2 47.2C744.8 542.4 724 563.2 697.6 563.2z", .Height = 24.0, .Width = 24.0 }
                            myIconButton.IsEnabled = Not flag
                            myIconButton.ToolTip = If(myIconButton.IsEnabled, "从列表中移除", "无法移除官方 Java")
                            AddHandler myIconButton.Click, Sub(a0 As Object, a1 As EventArgs)
                                MyBase._Lambda$__2()
                            End Sub
                            ToolTipService.SetShowOnDisabled(myIconButton, True)
                            list.Add(myIconButton)
                            Dim myIconButton2 As OurIconButton = New OurIconButton() With { .Logo = "M889.018182 418.909091H884.363636V316.509091a93.090909 93.090909 0 0 0-99.607272-89.832727h-302.545455l-93.090909-76.334546A46.545455 46.545455 0 0 0 358.865455 139.636364H146.152727A93.090909 93.090909 0 0 0 46.545455 229.469091V837.818182a46.545455 46.545455 0 0 0 46.545454 46.545454 46.545455 46.545455 0 0 0 16.756364-3.258181 109.381818 109.381818 0 0 0 25.134545 3.258181h586.472727a85.178182 85.178182 0 0 0 87.04-63.301818l163.374546-302.545454a46.545455 46.545455 0 0 0 5.585454-21.876364A82.385455 82.385455 0 0 0 889.018182 418.909091z m-744.727273-186.181818h198.283636l93.09091 76.334545a46.545455 46.545455 0 0 0 29.323636 10.705455h319.301818a12.101818 12.101818 0 0 1 6.516364 0V418.909091H302.545455a85.178182 85.178182 0 0 0-87.04 63.301818L139.636364 622.778182V232.727273a19.549091 19.549091 0 0 1 6.516363 0z m578.094546 552.029091a27.461818 27.461818 0 0 0-2.792728 6.516363H154.530909l147.083636-272.290909a27.461818 27.461818 0 0 0 2.792728-6.981818h565.061818z", .LogoScale = 1.1, .Height = 24.0, .Width = 24.0 }
                            myIconButton2.ToolTip = "打开文件夹"
                            AddHandler myIconButton2.Click, Sub(a0 As Object, a1 As EventArgs)
                                MyBase._Lambda$__3()
                            End Sub
                            list.Add(myIconButton2)
                            myListItem.Buttons = list
                        End While
                    Finally
                        Dim enumerator As IEnumerator(Of Java)
                        If enumerator IsNot Nothing Then
                            enumerator.Dispose()
                        End If
                    End Try
                    Dim myListItem2 As OurListItem = New OurListItem() With { .FontSize = 13.0, .Height = 24.0, .IsScaleAnimationEnabled = False, .Title = " 导入电脑中已有的 Java…", .Type = OurListItem.CheckType.Clickable }
                    Dim uielement2 As UIElement = myListItem2
                    Dim mouseButtonEventHandler3 As MouseButtonEventHandler
                    If PageInstanceSetup._Closure$__.$I23-4 IsNot Nothing Then
                        mouseButtonEventHandler3 = PageInstanceSetup._Closure$__.$I23-4
                    Else
                        Dim mouseButtonEventHandler4 As MouseButtonEventHandler = Sub(sender As Object, e As MouseButtonEventArgs)
                            e.Handled = True
                            ModJava.ManuallyImportJava()
                        End Sub
                        mouseButtonEventHandler3 = mouseButtonEventHandler4
                        PageInstanceSetup._Closure$__.$I23-4 = mouseButtonEventHandler4
                    End If
                    AddHandler uielement2.MouseLeftButtonUp, mouseButtonEventHandler3
                    Me.ComboArgumentJavaSelect.Items.Add(myListItem2)
                Catch ex As Exception
                    Logger.[Error](ex, "更新 Java 下拉框失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceSetup.xaml.vb")
                End Try
            End If
        End Sub

        Private Sub BtnAdvanceJavaSearch_Click()
            ModJava.ManuallySearchJava(Nothing)
        End Sub

        Private Sub ComboArgumentIndieV2_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If ModAnimation.AniControlEnabled = 0 Then
                If Me.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init Is Nothing Then
                    Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init, New StaticLocalInitFlag(), Nothing)
                End If
                Dim flag As Boolean = False
                Try
                    Monitor.Enter(Me.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init, flag)
                    If Me.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init.State = 0S Then
                        Me.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init.State = 2S
                        Me.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting = False
                    ElseIf Me.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init.State = 2S Then
                        Throw New IncompleteInitialization()
                    End If
                Finally
                    Me.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init.State = 1S
                    If flag Then
                        Monitor.[Exit](Me.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init)
                    End If
                End Try
                If Not Me.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting Then
                    If ModMain.OurMsgBox("调整版本隔离设置后，你需要游戏存档、Mod 等文件手动迁移到新的游戏文件夹中。" & vbCrLf & "如果发现存档消失，把这项设置改回来就能恢复。" & vbCrLf & "如果你不会迁移存档，不建议修改这项设置！", "警告", "我知道我在做什么", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 2 Then
                        Me.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting = True
                        If e.RemovedItems.Count > 0 Then
                            Me.ComboArgumentIndieV2.SelectedItem = RuntimeHelpers.GetObjectValue(e.RemovedItems(0))
                        End If
                        Me.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting = False
                    End If
                    Settings.[Set]("VersionArgumentIndieV2", RuntimeHelpers.GetObjectValue(Operators.CompareObjectEqual(NewLateBinding.LateGet(sender, Nothing, "SelectedIndex", New Object(-1) {}, Nothing, Nothing, Nothing), 0, False)), PageInstanceLeft.Instance)
                End If
            End If
        End Sub

        Private Sub TextAdvanceRun_TextChanged(sender As Object, e As TextChangedEventArgs)
            Me.CheckAdvanceRunWait.Visibility = (Operators.CompareString(Me.TextAdvanceRun.Text, "", False) <> 0).ToVisibility()
        End Sub

        Private Sub BtnSwitch_Click(sender As Object, e As MouseButtonEventArgs)
            ModMain.FrmMain.PageChange(FormMain.PageType.Setup, FormMain.PageSubType.[Default])
        End Sub

        Private Sub ReplaceEnter(sender As OurTextBox, e As TextChangedEventArgs)
            Dim text As String = sender.Text.ReplaceLineEndings(" ", True)
            If Operators.CompareString(text, sender.Text, False) <> 0 Then
                Dim caretIndex As Integer = sender.CaretIndex
                sender.Text = text
                sender.CaretIndex = Math.Max(0, caretIndex - 1)
            End If
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
                Dim selectionChangedEventHandler As SelectionChangedEventHandler = AddressOf Me.ComboArgumentIndieV2_SelectionChanged
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

        Friend Overridable Property TextArgumentTitle As OurTextBox

        Friend Overridable Property TextArgumentInfo As OurTextBox

        Friend Overridable Property ComboArgumentJava As OurComboBox
            <CompilerGenerated()>
            Get
                Return Me._ComboArgumentJava
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurComboBox)
                Dim selectionChangedEventHandler As SelectionChangedEventHandler = Sub(a0 As Object, a1 As SelectionChangedEventArgs)
                    Me.ReloadCurrentJava()
                End Sub
                Dim selectionChangedEventHandler2 As SelectionChangedEventHandler = Sub(a0 As Object, a1 As SelectionChangedEventArgs)
                    Me.UpdateJavaRelatedUi()
                End Sub
                Dim myComboBox As OurComboBox = Me._ComboArgumentJava
                If myComboBox IsNot Nothing Then
                    RemoveHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                    RemoveHandler myComboBox.SelectionChanged, selectionChangedEventHandler2
                End If
                Me._ComboArgumentJava = value
                myComboBox = Me._ComboArgumentJava
                If myComboBox IsNot Nothing Then
                    AddHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                    AddHandler myComboBox.SelectionChanged, selectionChangedEventHandler2
                End If
            End Set
        End Property

        Friend Overridable Property TextArgumentJavaRange As OurTextBox
            <CompilerGenerated()>
            Get
                Return Me._TextArgumentJavaRange
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurTextBox)
                Dim routedEventHandler As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    Me.ReloadCurrentJava()
                End Sub
                Dim textChangedEventHandler As TextChangedEventHandler = Sub(a0 As Object, a1 As TextChangedEventArgs)
                    Me.UpdateJavaRelatedUi()
                End Sub
                Dim myTextBox As OurTextBox = Me._TextArgumentJavaRange
                If myTextBox IsNot Nothing Then
                    RemoveHandler myTextBox.ValidatedTextChanged, routedEventHandler
                    RemoveHandler myTextBox.TextChanged, textChangedEventHandler
                End If
                Me._TextArgumentJavaRange = value
                myTextBox = Me._TextArgumentJavaRange
                If myTextBox IsNot Nothing Then
                    AddHandler myTextBox.ValidatedTextChanged, routedEventHandler
                    AddHandler myTextBox.TextChanged, textChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property HintArgumentJava As OurHint
            <CompilerGenerated()>
            Get
                Return Me._HintArgumentJava
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurHint)
                Dim clickEventHandler As OurHint.ClickEventHandler = AddressOf Me.HintArgumentJava_Click
                Dim myHint As OurHint = Me._HintArgumentJava
                If myHint IsNot Nothing Then
                    RemoveHandler myHint.Click, clickEventHandler
                End If
                Me._HintArgumentJava = value
                myHint = Me._HintArgumentJava
                If myHint IsNot Nothing Then
                    AddHandler myHint.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanArgumentJavaSelect As Grid

        Friend Overridable Property ComboArgumentJavaSelect As OurComboBox

        Friend Overridable Property LabArgumentJava As TextBlock

        Friend Overridable Property BtnArgumentJavaSearch As OurIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnArgumentJavaSearch
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconButton)
                Dim clickEventHandler As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.BtnAdvanceJavaSearch_Click()
                End Sub
                Dim myIconButton As OurIconButton = Me._BtnArgumentJavaSearch
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnArgumentJavaSearch = value
                myIconButton = Me._BtnArgumentJavaSearch
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property RadioRamType2 As OurRadioBox
            <CompilerGenerated()>
            Get
                Return Me._RadioRamType2
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurRadioBox)
                Dim checkEventHandler As IOurRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.RefreshRam()
                End Sub
                Dim myRadioBox As OurRadioBox = Me._RadioRamType2
                If myRadioBox IsNot Nothing Then
                    RemoveHandler myRadioBox.Check, checkEventHandler
                End If
                Me._RadioRamType2 = value
                myRadioBox = Me._RadioRamType2
                If myRadioBox IsNot Nothing Then
                    AddHandler myRadioBox.Check, checkEventHandler
                End If
            End Set
        End Property

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

        Friend Overridable Property ComboRamOptimize As OurComboBox

        Friend Overridable Property PanRamDisplay As Grid

        Friend Overridable Property ColumnRamUsed As ColumnDefinition

        Friend Overridable Property ColumnRamGame As ColumnDefinition

        Friend Overridable Property ColumnRamEmpty As ColumnDefinition

        Friend Overridable Property RectRamUsed As Rectangle

        Friend Overridable Property RectRamGame As Rectangle
            <CompilerGenerated()>
            Get
                Return Me._RectRamGame
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Rectangle)
                Dim sizeChangedEventHandler As SizeChangedEventHandler = Sub(a0 As Object, a1 As SizeChangedEventArgs)
                    Me.RefreshRamText()
                End Sub
                Dim rectangle As Rectangle = Me._RectRamGame
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

        Friend Overridable Property RectRamEmpty As Rectangle
            <CompilerGenerated()>
            Get
                Return Me._RectRamEmpty
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Rectangle)
                Dim sizeChangedEventHandler As SizeChangedEventHandler = Sub(a0 As Object, a1 As SizeChangedEventArgs)
                    Me.RefreshRamText()
                End Sub
                Dim rectangle As Rectangle = Me._RectRamEmpty
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

        Friend Overridable Property CardServer As OurCard

        Friend Overridable Property ComboServerLogin As OurComboBox
            <CompilerGenerated()>
            Get
                Return Me._ComboServerLogin
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurComboBox)
                Dim selectionChangedEventHandler As SelectionChangedEventHandler = Sub(a0 As Object, a1 As SelectionChangedEventArgs)
                    Me.ComboServerLogin_Changed()
                End Sub
                Dim myComboBox As OurComboBox = Me._ComboServerLogin
                If myComboBox IsNot Nothing Then
                    RemoveHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
                Me._ComboServerLogin = value
                myComboBox = Me._ComboServerLogin
                If myComboBox IsNot Nothing Then
                    AddHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabServerNide As TextBlock

        Friend Overridable Property TextServerNide As OurTextBox
            <CompilerGenerated()>
            Get
                Return Me._TextServerNide
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurTextBox)
                Dim routedEventHandler As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    Me.ComboServerLogin_Changed()
                End Sub
                Dim myTextBox As OurTextBox = Me._TextServerNide
                If myTextBox IsNot Nothing Then
                    RemoveHandler myTextBox.ValidatedTextChanged, routedEventHandler
                End If
                Me._TextServerNide = value
                myTextBox = Me._TextServerNide
                If myTextBox IsNot Nothing Then
                    AddHandler myTextBox.ValidatedTextChanged, routedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabServerAuthServer As TextBlock

        Friend Overridable Property TextServerAuthServer As OurTextBox
            <CompilerGenerated()>
            Get
                Return Me._TextServerAuthServer
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurTextBox)
                Dim routedEventHandler As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    Me.ComboServerLogin_Changed()
                End Sub
                Dim myTextBox As OurTextBox = Me._TextServerAuthServer
                If myTextBox IsNot Nothing Then
                    RemoveHandler myTextBox.ValidatedTextChanged, routedEventHandler
                End If
                Me._TextServerAuthServer = value
                myTextBox = Me._TextServerAuthServer
                If myTextBox IsNot Nothing Then
                    AddHandler myTextBox.ValidatedTextChanged, routedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabServerAuthRegister As TextBlock

        Friend Overridable Property TextServerAuthRegister As OurTextBox
            <CompilerGenerated()>
            Get
                Return Me._TextServerAuthRegister
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurTextBox)
                Dim routedEventHandler As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    Me.ComboServerLogin_Changed()
                End Sub
                Dim myTextBox As OurTextBox = Me._TextServerAuthRegister
                If myTextBox IsNot Nothing Then
                    RemoveHandler myTextBox.ValidatedTextChanged, routedEventHandler
                End If
                Me._TextServerAuthRegister = value
                myTextBox = Me._TextServerAuthRegister
                If myTextBox IsNot Nothing Then
                    AddHandler myTextBox.ValidatedTextChanged, routedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabServerAuthName As TextBlock

        Friend Overridable Property TextServerAuthName As OurTextBox

        Friend Overridable Property TextServerEnter As OurTextBox
            <CompilerGenerated()>
            Get
                Return Me._TextServerEnter
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurTextBox)
                Dim textChangedEventHandler As TextChangedEventHandler = Sub(a0 As Object, a1 As TextChangedEventArgs)
                    Me.TextServerEnter_Change()
                End Sub
                Dim myTextBox As OurTextBox = Me._TextServerEnter
                If myTextBox IsNot Nothing Then
                    RemoveHandler myTextBox.TextChanged, textChangedEventHandler
                End If
                Me._TextServerEnter = value
                myTextBox = Me._TextServerEnter
                If myTextBox IsNot Nothing Then
                    AddHandler myTextBox.TextChanged, textChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanServerNide As Grid

        Friend Overridable Property BtnServerAuthLittle As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnServerAuthLittle
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnServerAuthLittle_Click
                Dim myButton As OurButton = Me._BtnServerAuthLittle
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnServerAuthLittle = value
                myButton = Me._BtnServerAuthLittle
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CardAdvance As OurCard

        Friend Overridable Property TextAdvanceJvm As OurTextBox
            <CompilerGenerated()>
            Get
                Return Me._TextAdvanceJvm
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurTextBox)
                Dim textChangedEventHandler As TextChangedEventHandler = Sub(a0 As Object, a1 As TextChangedEventArgs)
                    Me.ReplaceEnter(CType(a0, OurTextBox), a1)
                End Sub
                Dim myTextBox As OurTextBox = Me._TextAdvanceJvm
                If myTextBox IsNot Nothing Then
                    RemoveHandler myTextBox.TextChanged, textChangedEventHandler
                End If
                Me._TextAdvanceJvm = value
                myTextBox = Me._TextAdvanceJvm
                If myTextBox IsNot Nothing Then
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

        Friend Overridable Property CheckAdvanceDisableModUpdate As OurCheckBox

        Friend Overridable Property CheckAdvanceAssetsV2 As OurCheckBox

        Friend Overridable Property CheckAdvanceDisableJLW As OurCheckBox

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

        Private LoadedFirst As Boolean

        Private RamTextLeft As Integer

        Private RamTextRight As Integer

        Private ComboServerLoginLast As Integer

        Private $STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting As Boolean

        Private $STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init As StaticLocalInitFlag
    End Class
End Namespace
