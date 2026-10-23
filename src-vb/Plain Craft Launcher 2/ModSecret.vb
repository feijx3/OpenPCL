Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading
Imports System.Windows
Imports System.Windows.Media
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq
Imports PCL.My
Imports PCLCS

Namespace PCL
    Friend NotInheritable Module ModSecret
        Friend Function SecretCdnSign(UrlWithMark As String) As String
            Dim text As String
            If ModBase.ApplicationStartTick < 1L Then
                text = UrlWithMark
            ElseIf Not UrlWithMark.EndsWithF("{CDN}", False) Then
                text = UrlWithMark
            Else
                Dim text2 As String = UrlWithMark.Replace("{CDN}", "").Replace(" ", "%20")
                Dim text3 As String = ModBase.RandomInteger(0, 2147483645).ToString("x").EnsureLength("0"c, 8)
                Dim text4 As String = Conversions.ToString(ModBase.GetUnixTimestampUtc())
                Dim text5 As String = text2.Substring(text2.IndexOfF("://", False) + 3)
                text5 = text5.Substring(text5.IndexOfF("/", False))
                Dim text6 As String = CryptographyUtils.ComputeHash(New String() { text5, text4, text3, "0", "PCL2Server" }.Join("-"), CryptographyUtils.HashMethod.Md5)
                text = text2 + If(text2.Contains("?"), "&", "?") + "sign=" + New String() { text4, text3, "0", text6 }.Join("-")
            End If
            Return text
        End Function

        Friend Sub SecretHeadersSign(Url As String, ByRef Req As HttpRequestMessage, Optional SimulateBrowserHeaders As Boolean = False)
            If ModBase.ApplicationStartTick >= 1L Then
                If Not Req.Headers.UserAgent.Any() Then
                    If Url.Contains("baidupcs.com") OrElse Url.Contains("baidu.com") Then
                        Req.Headers.Add("User-Agent", "LogStatistic")
                    ElseIf SimulateBrowserHeaders Then
                        Req.Headers.Add("User-Agent", String.Format("PCL2/{0}.{1} Mozilla/5.0 AppleWebKit/537.36 Chrome/63.0.3239.132 Safari/537.36", "2.99.0.0", 0))
                    Else
                        Req.Headers.Add("User-Agent", String.Format("PCL2/{0}.{1}", "2.99.0.0", 0))
                    End If
                End If
                If Not SimulateBrowserHeaders Then
                    Req.Headers.Referrer = New Uri(String.Format("http://{0}.pcl2.server/", 414))
                End If
                If Url.Contains("api.curseforge.com") OrElse Url.Contains("forgecdn.net") Then
                    ' The original CurseForge API key was removed before publishing. Supply your own (base64-encoded) here.
                    Req.Headers.Add("x-api-key", Encoding.[Default].GetString(Convert.FromBase64String("")))
                End If
            End If
        End Sub

        Public Sub ThemeRefresh(Optional NewTheme As Integer = -1)
            Try
                If ModSecret.ThemeNow <> NewTheme OrElse NewTheme < 0 Then
                    If ModSecret.ThemeCheckOne(If((NewTheme >= 0), NewTheme, ModSecret.ThemeNow)) Then
                        If NewTheme >= 0 Then
                            ModSecret.ThemeNow = NewTheme
                        End If
                        Dim themeNow As Integer = ModSecret.ThemeNow
                        Select Case themeNow
                            Case 0
                                ModSecret.ColorHue = 210
                                ModSecret.ColorSat = 85
                                ModSecret.ColorLightAdjust = 0
                                ModSecret.ColorHueTopbarDelta = 0
                            Case 1
                                ModSecret.ColorHue = 180
                                ModSecret.ColorSat = 50
                                ModSecret.ColorLightAdjust = 0
                                ModSecret.ColorHueTopbarDelta = 0
                            Case 2
                                ModSecret.ColorHue = 130
                                ModSecret.ColorSat = 40
                                ModSecret.ColorLightAdjust = 0
                                ModSecret.ColorHueTopbarDelta = New Integer() { 5, -5, 5 }
                            Case 3
                                ModSecret.ColorHue = 75
                                ModSecret.ColorSat = 50
                                ModSecret.ColorLightAdjust = 0
                                ModSecret.ColorHueTopbarDelta = 0
                            Case 4
                                ModSecret.ColorHue = 35
                                ModSecret.ColorSat = 30
                                ModSecret.ColorLightAdjust = 0
                                ModSecret.ColorHueTopbarDelta = 0
                            Case 5
                                ModSecret.ColorHue = 225
                                ModSecret.ColorSat = 10
                                ModSecret.ColorLightAdjust = -20
                                ModSecret.ColorHueTopbarDelta = 0
                            Case 6
                                ModSecret.ColorHue = 345
                                ModSecret.ColorSat = 70
                                ModSecret.ColorLightAdjust = 15
                                ModSecret.ColorHueTopbarDelta = 0
                            Case 7
                                ModSecret.ColorHue = 285
                                ModSecret.ColorSat = 50
                                ModSecret.ColorLightAdjust = 0
                                ModSecret.ColorHueTopbarDelta = 0
                            Case 8
                                ModSecret.ColorHue = 50
                                ModSecret.ColorSat = 65
                                ModSecret.ColorLightAdjust = 10
                                ModSecret.ColorHueTopbarDelta = 0
                            Case 9
                                ModSecret.ColorHue = 25
                                ModSecret.ColorSat = 75
                                ModSecret.ColorLightAdjust = 5
                                ModSecret.ColorHueTopbarDelta = 0
                            Case 10
                                ModSecret.ColorHue = 5
                                ModSecret.ColorSat = 70
                                ModSecret.ColorLightAdjust = -5
                                ModSecret.ColorHueTopbarDelta = 0
                            Case 11
                                ModSecret.ColorHue = 230
                                ModSecret.ColorSat = 60
                                ModSecret.ColorLightAdjust = -5
                                ModSecret.ColorHueTopbarDelta = 0
                            Case 12
                                If Not ModSecret.IsThemeColorfulLoaded Then
                                    ModSecret.IsThemeColorfulLoaded = True
                                    ModSecret.ColorHue = ModBase.RandomInteger(0, 359)
                                    ModSecret.ColorHueDelta = (ModBase.RandomInteger(0, 1) * 2 - 1) * 4
                                End If
                                ModSecret.ColorHue = (ModSecret.ColorHue + ModSecret.ColorHueDelta) Mod 360
                                ModSecret.ColorSat = 45
                                ModSecret.ColorLightAdjust = 0
                            Case 13
                                ModSecret.ColorHue = 290
                                ModSecret.ColorSat = 50
                                ModSecret.ColorLightAdjust = 0
                                ModSecret.ColorHueTopbarDelta = New Integer() { -30, 10, 50 }
                            Case 14
                                ModSecret.ColorHue = Settings.[Get](Of Integer)("UiLauncherHue", Nothing)
                                ModSecret.ColorSat = Settings.[Get](Of Integer)("UiLauncherSat", Nothing)
                                ModSecret.ColorLightAdjust = Settings.[Get](Of Integer)("UiLauncherLight", Nothing) - 20
                                ModSecret.ColorHueTopbarDelta = Settings.[Get](Of Integer)("UiLauncherDelta", Nothing) - 90
                            Case Else
                                If themeNow <> 42 Then
                                    ModSecret.ColorHue = 210
                                    ModSecret.ColorSat = 85
                                    ModSecret.ColorLightAdjust = 0
                                Else
                                    ModSecret.ColorHue = 225
                                    ModSecret.ColorSat = 60
                                    ModSecret.ColorLightAdjust = -15
                                    ModSecret.ColorHueTopbarDelta = 0
                                End If
                        End Select
                        Select Case ModSecret.ThemeDontClick
                            Case 1
                                ModSecret.ColorLightAdjust = 999
                            Case 2
                                ModSecret.ColorHue = ModBase.RandomInteger(0, 359)
                                ModSecret.ColorSat = ModBase.RandomInteger(40, 70)
                                ModSecret.ColorLightAdjust = ModBase.RandomInteger(-20, 20)
                        End Select
                        ModSecret.Color1 = New ModBase.MyColor().FromHSL2(CDbl(ModSecret.ColorHue), CDbl(ModSecret.ColorSat) * 0.2, 25.0 + CDbl(ModSecret.ColorLightAdjust) * 0.3)
                        ModSecret.Color2 = New ModBase.MyColor().FromHSL2(CDbl(ModSecret.ColorHue), CDbl(ModSecret.ColorSat), CDbl((45 + ModSecret.ColorLightAdjust)))
                        ModSecret.Color3 = New ModBase.MyColor().FromHSL2(CDbl(ModSecret.ColorHue), CDbl(ModSecret.ColorSat), CDbl((55 + ModSecret.ColorLightAdjust)))
                        ModSecret.Color4 = New ModBase.MyColor().FromHSL2(CDbl(ModSecret.ColorHue), CDbl(ModSecret.ColorSat), CDbl((65 + ModSecret.ColorLightAdjust)))
                        ModSecret.Color5 = New ModBase.MyColor().FromHSL2(CDbl(ModSecret.ColorHue), CDbl(ModSecret.ColorSat), 80.0 + CDbl(ModSecret.ColorLightAdjust) * 0.4)
                        ModSecret.Color6 = New ModBase.MyColor().FromHSL2(CDbl(ModSecret.ColorHue), CDbl(ModSecret.ColorSat), 91.0 + CDbl(ModSecret.ColorLightAdjust) * 0.1)
                        ModSecret.Color7 = New ModBase.MyColor().FromHSL2(CDbl(ModSecret.ColorHue), CDbl(ModSecret.ColorSat), 95.0)
                        ModSecret.Color8 = New ModBase.MyColor().FromHSL2(CDbl(ModSecret.ColorHue), CDbl(ModSecret.ColorSat), 97.0)
                        ModSecret.ColorBg0 = ModSecret.Color4 * 0.4 + ModSecret.Color5 * 0.4 + ModSecret.ColorGray4 * 0.2
                        ModSecret.ColorBg1 = New ModBase.MyColor(190.0, ModSecret.Color7)
                        ModSecret.ColorSemiTransparent = New ModBase.MyColor(1.0, ModSecret.Color8)
                        Application.Current.Resources("ColorBrush1") = New SolidColorBrush(ModSecret.Color1)
                        Application.Current.Resources("ColorBrush2") = New SolidColorBrush(ModSecret.Color2)
                        Application.Current.Resources("ColorBrush3") = New SolidColorBrush(ModSecret.Color3)
                        Application.Current.Resources("ColorBrush4") = New SolidColorBrush(ModSecret.Color4)
                        Application.Current.Resources("ColorBrush5") = New SolidColorBrush(ModSecret.Color5)
                        Application.Current.Resources("ColorBrush6") = New SolidColorBrush(ModSecret.Color6)
                        Application.Current.Resources("ColorBrush7") = New SolidColorBrush(ModSecret.Color7)
                        Application.Current.Resources("ColorBrush8") = New SolidColorBrush(ModSecret.Color8)
                        Application.Current.Resources("ColorBrushBg0") = New SolidColorBrush(ModSecret.ColorBg0)
                        Application.Current.Resources("ColorBrushBg1") = New SolidColorBrush(ModSecret.ColorBg1)
                        Application.Current.Resources("ColorObject1") = ModSecret.Color1
                        Application.Current.Resources("ColorObject2") = ModSecret.Color2
                        Application.Current.Resources("ColorObject3") = ModSecret.Color3
                        Application.Current.Resources("ColorObject4") = ModSecret.Color4
                        Application.Current.Resources("ColorObject5") = ModSecret.Color5
                        Application.Current.Resources("ColorObject6") = ModSecret.Color6
                        Application.Current.Resources("ColorObject7") = ModSecret.Color7
                        Application.Current.Resources("ColorObject8") = ModSecret.Color8
                        Application.Current.Resources("ColorObjectBg0") = ModSecret.ColorBg0
                        Application.Current.Resources("ColorObjectBg1") = ModSecret.ColorBg1
                        ModSecret.ThemeRefreshMain()
                        If ModSecret.ThemeNow <> 12 AndAlso ModSecret.ThemeNow <> 14 AndAlso ModSecret.ThemeDontClick <> 2 Then
                            Logger.Info(String.Format("刷新主题：{0}", ModSecret.ThemeNow), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                        End If
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, "刷新主题颜色失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
            End Try
        End Sub

        Public Sub ThemeRefreshMain()
            Dim action As Action
            If ModSecret._Closure$__.$I36-0 IsNot Nothing Then
                action = ModSecret._Closure$__.$I36-0
            Else
                Dim action2 As Action = Sub()
                    If ModMain.FrmMain.IsLoaded Then
                        Dim linearGradientBrush As LinearGradientBrush = New LinearGradientBrush() With { .EndPoint = New Point(1.0, 0.0), .StartPoint = New Point(0.0, 0.0) }
                        If ModSecret.ThemeNow = 5 Then
                            linearGradientBrush.GradientStops.Add(New GradientStop() With { .Offset = 0.0, .Color = New ModBase.MyColor().FromHSL2(CDbl(ModSecret.ColorHue), CDbl(ModSecret.ColorSat), 25.0) })
                            linearGradientBrush.GradientStops.Add(New GradientStop() With { .Offset = 0.5, .Color = New ModBase.MyColor().FromHSL2(CDbl(ModSecret.ColorHue), CDbl(ModSecret.ColorSat), 15.0) })
                            linearGradientBrush.GradientStops.Add(New GradientStop() With { .Offset = 1.0, .Color = New ModBase.MyColor().FromHSL2(CDbl(ModSecret.ColorHue), CDbl(ModSecret.ColorSat), 25.0) })
                            ModMain.FrmMain.PanTitle.Background = linearGradientBrush
                            ModMain.FrmMain.PanTitle.Background.Freeze()
                        ElseIf ModSecret.ThemeNow <> 12 AndAlso ModSecret.ThemeDontClick <> 2 Then
                            Dim func As Func(Of Integer, Integer())
                            If ModSecret._Closure$__.$I36-1 IsNot Nothing Then
                                func = ModSecret._Closure$__.$I36-1
                            Else
                                Dim func2 As Func(Of Integer, Integer()) = Function(d As Integer) New Integer() { 0 - d, 0, d }
                                func = func2
                                ModSecret._Closure$__.$I36-1 = func2
                            End If
                            Dim func3 As Func(Of Integer(), Integer())
                            If ModSecret._Closure$__.$I36-2 IsNot Nothing Then
                                func3 = ModSecret._Closure$__.$I36-2
                            Else
                                Dim func4 As Func(Of Integer(), Integer()) = Function(d As Integer()) d
                                func3 = func4
                                ModSecret._Closure$__.$I36-2 = func4
                            End If
                            Dim array As Integer() = ModSecret.ColorHueTopbarDelta.Switch(Of Integer())(func, func3)
                            linearGradientBrush.GradientStops.Add(New GradientStop() With { .Offset = 0.0, .Color = New ModBase.MyColor().FromHSL2(CDbl((ModSecret.ColorHue + array(0))), CDbl(ModSecret.ColorSat), CDbl((48 + ModSecret.ColorLightAdjust))) })
                            linearGradientBrush.GradientStops.Add(New GradientStop() With { .Offset = 0.5, .Color = New ModBase.MyColor().FromHSL2(CDbl((ModSecret.ColorHue + array(1))), CDbl(ModSecret.ColorSat), CDbl((54 + ModSecret.ColorLightAdjust))) })
                            linearGradientBrush.GradientStops.Add(New GradientStop() With { .Offset = 1.0, .Color = New ModBase.MyColor().FromHSL2(CDbl((ModSecret.ColorHue + array(2))), CDbl(ModSecret.ColorSat), CDbl((48 + ModSecret.ColorLightAdjust))) })
                            ModMain.FrmMain.PanTitle.Background = linearGradientBrush
                            ModMain.FrmMain.PanTitle.Background.Freeze()
                        Else
                            linearGradientBrush.GradientStops.Add(New GradientStop() With { .Offset = 0.0, .Color = New ModBase.MyColor().FromHSL2(CDbl((ModSecret.ColorHue - 21)), CDbl(ModSecret.ColorSat), CDbl((53 + ModSecret.ColorLightAdjust))) })
                            linearGradientBrush.GradientStops.Add(New GradientStop() With { .Offset = 0.33, .Color = New ModBase.MyColor().FromHSL2(CDbl((ModSecret.ColorHue - 7)), CDbl(ModSecret.ColorSat), CDbl((47 + ModSecret.ColorLightAdjust))) })
                            linearGradientBrush.GradientStops.Add(New GradientStop() With { .Offset = 0.67, .Color = New ModBase.MyColor().FromHSL2(CDbl((ModSecret.ColorHue + 7)), CDbl(ModSecret.ColorSat), CDbl((47 + ModSecret.ColorLightAdjust))) })
                            linearGradientBrush.GradientStops.Add(New GradientStop() With { .Offset = 1.0, .Color = New ModBase.MyColor().FromHSL2(CDbl((ModSecret.ColorHue + 21)), CDbl(ModSecret.ColorSat), CDbl((53 + ModSecret.ColorLightAdjust))) })
                            ModMain.FrmMain.PanTitle.Background = linearGradientBrush
                        End If
                        If ModSecret.ThemeNow >= 5 AndAlso ModSecret.ThemeNow <> 14 Then
                            ModMain.FrmMain.ImgTitle.Source = String.Format("{0}Themes/{1}.png", ModBase.PathImage, ModSecret.ThemeNow)
                        Else
                            ModMain.FrmMain.ImgTitle.Source = Nothing
                        End If
                        ModMain.FrmMain.ImgTitle.Opacity = If((ModSecret.ThemeNow = 13), 0.25, 0.5)
                        If Settings.[Get](Of Boolean)("UiBackgroundColorful", Nothing) Then
                            linearGradientBrush = New LinearGradientBrush() With { .EndPoint = New Point(0.1, 1.0), .StartPoint = New Point(0.9, 0.0) }
                            linearGradientBrush.GradientStops.Add(New GradientStop() With { .Offset = -0.1, .Color = New ModBase.MyColor().FromHSL2(CDbl((ModSecret.ColorHue - 15)), CDbl(ModSecret.ColorSat) * 0.8, 91.0) })
                            linearGradientBrush.GradientStops.Add(New GradientStop() With { .Offset = 0.4, .Color = New ModBase.MyColor().FromHSL2(CDbl(ModSecret.ColorHue), CDbl(ModSecret.ColorSat) * 0.8, 91.0) })
                            linearGradientBrush.GradientStops.Add(New GradientStop() With { .Offset = 1.1, .Color = New ModBase.MyColor().FromHSL2(CDbl((ModSecret.ColorHue + 15)), CDbl(ModSecret.ColorSat) * 0.8, 91.0) })
                            ModMain.FrmMain.PanForm.Background = linearGradientBrush
                        Else
                            ModMain.FrmMain.PanForm.Background = New ModBase.MyColor(245.0, 245.0, 245.0)
                        End If
                        ModMain.FrmMain.PanForm.Background.Freeze()
                    End If
                End Sub
                action = action2
                ModSecret._Closure$__.$I36-0 = action2
            End If
            ModBase.RunInUi(action, False)
        End Sub

        Friend Sub ThemeCheckAll(EffectSetup As Boolean)
            Dim CS$<>8__locals1 As ModSecret._Closure$__37-0 = New ModSecret._Closure$__37-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_EffectSetup = EffectSetup
            ModBase.RunInUi(Sub()
                Try
                    Dim num As Integer = Settings.[Get](Of Integer)("UiLauncherTheme", Nothing)
                    If Not ModSecret.ThemeCheckOne(num) Then
                        Logger.Warn(String.Format("检测到尚未解锁的主题：{0}，已重置为默认主题", num), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                        Settings.[Set]("UiLauncherTheme", 0, Nothing)
                    End If
                    If CS$<>8__locals1.$VB$Local_EffectSetup AndAlso ModMain.FrmSetupUI IsNot Nothing Then
                        Dim list As List(Of Integer) = New List(Of Integer)()
                        Dim num2 As Integer = 0
                        Do
                            If ModSecret.ThemeCheckOne(num2) Then
                                list.Add(num2)
                            End If
                            num2 += 1
                        Loop While num2 <= 14
                        list.Add(42)
                        ModAnimation.AniControlEnabled += 1
                        If list.Contains(5) Then
                            ModMain.FrmSetupUI.RadioLauncherTheme5.Opacity = 1.0
                            ModMain.FrmSetupUI.LabLauncherTheme5Unlock.Visibility = Visibility.Collapsed
                            ModMain.FrmSetupUI.RadioLauncherTheme5Gray.Visibility = Visibility.Collapsed
                        End If
                        If list.Contains(6) Then
                            ModMain.FrmSetupUI.RadioLauncherTheme6.IsEnabled = True
                        End If
                        If list.Contains(7) Then
                            ModMain.FrmSetupUI.RadioLauncherTheme7.IsEnabled = True
                        End If
                        If list.Contains(8) Then
                            ModMain.FrmSetupUI.LabLauncherTheme8Copy.Visibility = Visibility.Collapsed
                            ModMain.FrmSetupUI.RadioLauncherTheme8.IsEnabled = True
                        End If
                        If list.Contains(9) Then
                            ModMain.FrmSetupUI.LabLauncherTheme9Copy.Visibility = Visibility.Collapsed
                            ModMain.FrmSetupUI.RadioLauncherTheme9.IsEnabled = True
                        End If
                        If list.Contains(10) Then
                            ModMain.FrmSetupUI.RadioLauncherTheme10.IsEnabled = True
                        End If
                        If list.Contains(11) Then
                            ModMain.FrmSetupUI.LabLauncherTheme11Click.Visibility = Visibility.Collapsed
                            ModMain.FrmSetupUI.RadioLauncherTheme11.IsEnabled = True
                        ElseIf Not Information.IsNothing(ModMain.FrmSetupUI) AndAlso Not Information.IsNothing(ModMain.FrmSetupUI.RadioLauncherTheme11) AndAlso ModMain.FrmSetupUI.RadioLauncherTheme11.IsLoaded Then
                            Dim num3 As Integer = 0
                            Try
                                For Each num4 As Integer In list
                                    If num4 >= 5 AndAlso num4 <= 14 Then
                                        num3 += 1
                                    End If
                                Next
                            Finally
                                Dim enumerator As List(Of Integer).Enumerator
                                CType(enumerator, IDisposable).Dispose()
                            End Try
                            If num3 < 3 Then
                                ModMain.FrmSetupUI.LabLauncherTheme11Click.ToolTip = "需要解锁三个隐藏主题"
                            Else
                                ModMain.FrmSetupUI.LabLauncherTheme11Click.ToolTip = "点击打开解密游戏入口"
                            End If
                        End If
                        If list.Contains(12) Then
                            ModMain.FrmSetupUI.RadioLauncherTheme12.IsEnabled = True
                        End If
                        If list.Contains(13) Then
                            ModMain.FrmSetupUI.RadioLauncherTheme13.IsEnabled = True
                        End If
                        If list.Contains(14) Then
                            ModMain.FrmSetupUI.RadioLauncherTheme14.IsEnabled = True
                        End If
                        ModAnimation.AniControlEnabled -= 1
                    End If
                Catch ex As Exception
                    If TypeOf(If(ex.InnerException, ex))Is FormatException Then
                        Logger.[Error](ex, "解锁的主题列表存档已损坏，主题解锁已被重置", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                        Settings.[Set]("UiLauncherThemeHide2", "", Nothing)
                    Else
                        Logger.[Error](ex, "检查主题失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                    End If
                End Try
            End Sub, False)
        End Sub

        Friend Function ThemeCheckOne(Id As Integer) As Boolean
            Dim flag As Boolean
            If New Integer() { 0, 1, 2, 3, 4, 42 }.Contains(Id) Then
                flag = True
            ElseIf ModSecret.PotatoFeatures.Contains("Theme" + Conversions.ToString(Id)) Then
                flag = True
            Else
                Dim num As Integer = Id
                If num <> 8 Then
                    If num <> 14 Then
                        If Settings.[Get](Of String)("UiLauncherThemeHide", Nothing).Contains("7") Then
                            ModSecret.ThemeUnlock(7, False, Nothing)
                        End If
                        flag = Settings.[Get](Of String)("UiLauncherThemeHide2", Nothing).ToString().Split("|", False).Contains(Id.ToString())
                    Else
                        flag = Enumerable.Range(5, 9).Count(AddressOf ModSecret.ThemeCheckOne) >= 5
                    End If
                Else
                    flag = ModSecret.CurrentRank >= DonationRank.Rank23
                End If
            End If
            Return flag
        End Function

        Friend Function ThemeUnlock(Id As Integer, Optional ShowDoubleHint As Boolean = True, Optional UnlockHint As String = Nothing) As Boolean
            Dim flag As Boolean
            If ModMain.FrmMain Is Nothing Then
                flag = False
            Else
                If Id = 8 Then
                    Throw New Exception("不可使用此方式解锁秋仪金！")
                End If
                Dim list As List(Of String) = New List(Of String)(Settings.[Get](Of String)("UiLauncherThemeHide2", Nothing).ToString().Split("|", False))
                If list.Contains(Id.ToString()) Then
                    If ShowDoubleHint Then
                        ModMain.Hint("该隐藏主题已被解锁！", ModMain.HintType.Blue, True)
                    End If
                    flag = False
                Else
                    Logger.Info(String.Format("解锁隐藏主题：{0}", Id), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                    If UnlockHint IsNot Nothing Then
                        ModMain.Hint(UnlockHint, ModMain.HintType.Green, False)
                    End If
                    list.Add(Id.ToString())
                    Settings.[Set]("UiLauncherThemeHide2", list.Join("|"), Nothing)
                    ModSecret.ThemeCheckAll(True)
                    flag = True
                End If
            End If
            Return flag
        End Function

        Friend Sub UpdateCheckByButton()
            If ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init Is Nothing Then
                Interlocked.CompareExchange(Of StaticLocalInitFlag)(ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init, New StaticLocalInitFlag(), Nothing)
            End If
            Dim flag As Boolean = False
            Try
                Monitor.Enter(ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init, flag)
                If ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init.State = 0S Then
                    ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init.State = 2S
                    ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate = False
                ElseIf ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init.State = 2S Then
                    Throw New IncompleteInitialization()
                End If
            Finally
                ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init.State = 1S
                If flag Then
                    Monitor.[Exit](ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init)
                End If
            End Try
            If ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate Then
                ModMain.Hint("正在检查更新，请稍候！", ModMain.HintType.Blue, True)
                Return
            End If
            ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate = True
            Dim action As Action
            If ModSecret._Closure$__.$I40-0 IsNot Nothing Then
                action = ModSecret._Closure$__.$I40-0
            Else
                Dim action2 As Action = Sub()
                    Try
                        ModMain.Hint("正在检查更新，请稍候！", ModMain.HintType.Blue, True)
                        ModSecret.ServerLoader.WaitForExit(Nothing, Nothing, True, Nothing, Nothing)
                        Dim flag2 As Boolean? = PageSetupSystem.IsLauncherNewest()
                        If flag2 Is Nothing Then
                            ModMain.Hint("连接 PCL 服务器失败，请确认系统时间是否准确，尝试将 PCL 加入杀毒软件或防火墙白名单，然后重启 PCL！", ModMain.HintType.Red, True)
                        ElseIf flag2.GetValueOrDefault() Then
                            ModMain.Hint(String.Format("已经是最新{0} {1}，不需要更新啦，可以直接使用！", "快照版", "2.99.0.0"), ModMain.HintType.Green, True)
                        Else
                            If Not ModSecret.HasUpdatePermission Then
                                ModSecret.InputPotatoCode(True)
                                If Not ModSecret.HasUpdatePermission Then
                                    Return
                                End If
                            ElseIf ModMain.MyMsgBox("发现启动器更新，是否立即下载？", "发现更新！", "更新！冲！", "取消", "", False, True, False, Nothing, Nothing, Nothing) = 2 Then
                                Return
                            End If
                            ModSecret.UpdateStart(False, Nothing)
                        End If
                    Catch ex As Exception
                        Logger.[Error](ex, "尝试手动开始更新失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                    Finally
                        ModSecret.$STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate = False
                    End Try
                End Sub
                action = action2
                ModSecret._Closure$__.$I40-0 = action2
            End If
            ModBase.RunInThread(action)
        End Sub

        Private Sub UpdateStart(Silent As Boolean, Optional UrlsOverride As IEnumerable(Of String) = Nothing)
            Dim CS$<>8__locals1 As ModSecret._Closure$__43-0 = New ModSecret._Closure$__43-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_UrlsOverride = UrlsOverride
            If ModSecret.IsUpdating Then
                If Not Silent Then
                    ModMain.Hint("PCL 正在下载更新，更新结束时将自动重启，请稍候！", ModMain.HintType.Blue, True)
                    ModSecret.$STATIC$UpdateStart$0212151280E91E$IsSilentUpdate = False
                End If
                If ModSecret.IsUpdateWaitingRestart Then
                    ModSecret.UpdateRestart(True)
                    Return
                End If
            Else
                ModSecret.IsUpdating = True
                ModSecret.$STATIC$UpdateStart$0212151280E91E$IsSilentUpdate = Silent
                If Not ModSecret.$STATIC$UpdateStart$0212151280E91E$IsSilentUpdate Then
                    ModMain.Hint("PCL 正在下载更新，更新结束时将自动重启，请稍候！", ModMain.HintType.Blue, True)
                End If
                Try
                    Dim CS$<>8__locals2 As ModSecret._Closure$__43-1 = New ModSecret._Closure$__43-1(CS$<>8__locals2)
                    CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1
                    CS$<>8__locals2.$VB$Local_Urls = Nothing
                    CS$<>8__locals2.$VB$Local_Extension = Nothing
                    CS$<>8__locals2.$VB$Local_LocalPath = Nothing
                    CS$<>8__locals2.$VB$Local_Loader = New ModLoader.LoaderCombo(Of String)("启动器更新", New ModLoader.LoaderBase() { New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))("获取更新信息", Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                        If ModSecret.ServerLoader.State = ModBase.LoadState.Loading Then
                            ModSecret.ServerLoader.WaitForExit(Nothing, Nothing, False, Nothing, Nothing)
                        End If
                        If CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_UrlsOverride IsNot Nothing Then
                            CS$<>8__locals2.$VB$Local_Urls = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_UrlsOverride.ToList()
                        Else
                            Dim serverConfig As JObject = ModSecret.ServerConfig
                            Dim flag As Boolean
                            If serverConfig Is Nothing Then
                                flag = Nothing IsNot Nothing
                            Else
                                Dim jtoken As JToken = serverConfig("Update")
                                flag = If((jtoken IsNot Nothing), jtoken(ModBase.BuildTypes.Snapshot.ToString()), Nothing) IsNot Nothing
                            End If
                            If flag Then
                                Dim jobject As JObject = CType(ModSecret.ServerConfig("Update")(ModBase.BuildTypes.Snapshot.ToString()), JObject)
                                If jobject.ContainsKey("PatchUrls") AndAlso ModSecret.UpdateMark.StartsWithF("Empty", False) Then
                                    CS$<>8__locals2.$VB$Local_Urls = jobject("PatchUrls").ToObject(Of List(Of String))()
                                ElseIf jobject.ContainsKey("DownloadUrls") Then
                                    CS$<>8__locals2.$VB$Local_Urls = jobject("DownloadUrls").ToObject(Of List(Of String))()
                                End If
                            End If
                        End If
                        If CS$<>8__locals2.$VB$Local_Urls Is Nothing OrElse Not CS$<>8__locals2.$VB$Local_Urls.Any() Then
                            Logger.[Error]("更新失败：服务器没有提供有效的更新包下载链接，或无法连接到服务器！", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                            ModSecret.IsUpdating = False
                            Return
                        End If
                        Dim $VB$Local_Urls As IEnumerable(Of String) = CS$<>8__locals2.$VB$Local_Urls
                        Dim func As Func(Of String, String)
                        If ModSecret._Closure$__.$I43-1 IsNot Nothing Then
                            func = ModSecret._Closure$__.$I43-1
                        Else
                            Dim func2 As Func(Of String, String) = Function(u As String) ModMain.ArgumentReplace(u, New Func(Of String, String)(WebUtility.HtmlEncode), True)
                            func = func2
                            ModSecret._Closure$__.$I43-1 = func2
                        End If
                        CS$<>8__locals2.$VB$Local_Urls = $VB$Local_Urls.[Select](func).ToList()
                        CS$<>8__locals2.$VB$Local_Extension = "." + PathUtils.GetExtension(CS$<>8__locals2.$VB$Local_Urls.First().BeforeLast("{CDN}", False))
                        CS$<>8__locals2.$VB$Local_LocalPath = String.Format("{0}{1}.{2}{3}", New Object() { ModMain.RequestTaskTempFolder(False), "2.99.0.0", 0, CS$<>8__locals2.$VB$Local_Extension })
                        Logger.Info(String.Format("更新开始，静默：{0}，扩展名：{1}，下载目标：{2}，URL：{3}", New Object() { ModSecret.$STATIC$UpdateStart$0212151280E91E$IsSilentUpdate, CS$<>8__locals2.$VB$Local_Extension, CS$<>8__locals2.$VB$Local_LocalPath, CS$<>8__locals2.$VB$Local_Urls.Join("，") }), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                        Task.Output = New ModNet.NetFile() { New ModNet.NetFile(CS$<>8__locals2.$VB$Local_Urls, CS$<>8__locals2.$VB$Local_LocalPath, Nothing, False) }.ToList()
                    End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.1 }, New ModNet.LoaderDownload("下载更新文件", New List(Of ModNet.NetFile)()) With { .ProgressWeight = 1.0 }, New ModLoader.LoaderTask(Of String, Integer)("安装更新", Sub(a0 As ModLoader.LoaderTask(Of String, Integer))
                        MyBase._Lambda$__2()
                    End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.1 } })
                    CS$<>8__locals2.$VB$Local_Loader.OnStateChanged = Sub(a0 As ModLoader.LoaderBase)
                        MyBase._Lambda$__3()
                    End Sub
                    CS$<>8__locals2.$VB$Local_Loader.Start(Nothing, False)
                    ModBase.RunInUi(Sub()
                        If Not ModSecret.$STATIC$UpdateStart$0212151280E91E$IsSilentUpdate Then
                            ModLoader.LoaderTaskbarAdd(Of String)(CS$<>8__locals2.$VB$Local_Loader)
                            ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                        End If
                    End Sub, False)
                Catch ex As Exception
                    Logger.[Error](ex, "开始启动器更新失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                    ModSecret.IsUpdating = False
                End Try
            End If
        End Sub

        Public Sub UpdateRestart(TriggerRestartAndByEnd As Boolean)
            Try
                ModSecret.IsUpdateWaitingRestart = False
                Dim text As String = Paths.Base + "PCL\Plain Craft Launcher 2.exe"
                Dim text2 As String = String.Format("--update {0} ""{1}"" ""{2}"" {3}", New Object() { Process.GetCurrentProcess().Id, AppDomain.CurrentDomain.SetupInformation.ApplicationName, AppDomain.CurrentDomain.SetupInformation.ApplicationName, TriggerRestartAndByEnd })
                Logger.Info(String.Format("更新程序启动，参数：{0}", text2), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                ModBase.StartProcess(New ProcessStartInfo(text) With { .WindowStyle = ProcessWindowStyle.Hidden, .CreateNoWindow = True, .Arguments = text2 })
                If TriggerRestartAndByEnd Then
                    ModMain.FrmMain.EndProgram(False)
                    Logger.Info("已由于更新强制结束程序", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                End If
            Catch ex As Win32Exception
                Logger.Warn(ex, "自动更新时触发 Win32 错误，疑似被拦截", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                If ModMain.MyMsgBox(String.Format("由于被 Windows 安全中心拦截，或者存在权限问题，导致 PCL 无法更新。{0}请将 PCL 所在文件夹加入白名单，或者手动用 {1}PCL\Plain Craft Launcher 2.exe 替换当前文件！", vbCrLf, Paths.Base), "更新失败", "查看帮助", "确定", "", True, True, False, Nothing, Nothing, Nothing) = 1 Then
                    CustomEvent.Raise(CustomEvent.EventType.打开帮助, "启动器/Microsoft Defender 添加排除项.json")
                End If
            End Try
        End Sub

        Public Sub UpdateReplace(ProcessId As Integer, OldFileName As String, NewFileName As String, TriggerRestart As Boolean)
            Dim CS$<>8__locals1 As ModSecret._Closure$__46-0 = New ModSecret._Closure$__46-0(CS$<>8__locals1)
            Try
                Process.GetProcessById(ProcessId).Kill()
            Catch ex As Exception
            End Try
            Dim text As String = Path.Combine(PathUtils.AddSlashSuffix(PathUtils.RemoveLastPart(Paths.Base)), PathUtils.GetLastPart(OldFileName))
            CS$<>8__locals1.$VB$Local_NewFilePath = Path.Combine(PathUtils.AddSlashSuffix(PathUtils.RemoveLastPart(Paths.Base)), PathUtils.GetLastPart(NewFileName))
            Dim ex2 As Exception = Nothing
            Dim num As Integer = 0
            Do
                Try
                    FileUtils.Delete(text, False)
                    FileUtils.Delete(CS$<>8__locals1.$VB$Local_NewFilePath, False)
                    If Not FileUtils.Exists(text) AndAlso Not FileUtils.Exists(CS$<>8__locals1.$VB$Local_NewFilePath) Then
                        Exit Try
                    End If
                Catch ex3 As Exception
                    ex2 = ex3
                End Try
                Thread.Sleep(num * 500 + 500)
                num += 1
            Loop While num <= 4
            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_11(Of String, String) = Function(Reason As String)
                Dim array As String() = New String(4) {}
                array(0) = String.Format("由于{0}，PCL 无法完成更新。{1}请依次尝试：{2}", Reason, vbCrLf, vbCrLf)
                array(1) = If((PathUtils.IsParentOf(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Paths.Base) OrElse PathUtils.IsParentOf(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Paths.Base)), " - 将 PCL 文件移动到桌面、文档、C 盘以外的文件夹（这或许可以一劳永逸地解决权限问题）" & vbCrLf, "")
                Dim num2 As Integer = 2
                Dim diskName As Char? = PathUtils.GetDiskName(Paths.Base)
                array(num2) = If(If((diskName IsNot Nothing), New Boolean?(diskName.GetValueOrDefault() = "C"c), Nothing).GetValueOrDefault(), " - 将 PCL 文件移动到 C 盘以外的文件夹（这或许可以一劳永逸地解决权限问题）" & vbCrLf, "")
                array(3) = If((Not WindowsUtils.HasAdminRole()), " - 右键 PCL，选择以管理员身份运行" & vbCrLf, "")
                array(4) = String.Format(" - 手动剪切 PCL 文件夹中的新版本程序，覆盖原程序：{0}   将位于 {1}{2}   的程序{3}   剪切到 {4} 并覆盖即可。", New Object() { vbCrLf, ModBase.PathExe, vbCrLf, vbCrLf, CS$<>8__locals1.$VB$Local_NewFilePath })
                Return String.Concat(array)
            End Function
            If Not FileUtils.Exists(text) AndAlso Not FileUtils.Exists(CS$<>8__locals1.$VB$Local_NewFilePath) Then
                Try
                    FileUtils.Copy(ModBase.PathExe, CS$<>8__locals1.$VB$Local_NewFilePath)
                Catch ex4 As UnauthorizedAccessException
                    Interaction.MsgBox(String.Format("{0}{1}{2}详细错误信息：{3}{4}", New Object() { vb$AnonymousDelegate_("复制新版本文件时权限不足"), vbCrLf, vbCrLf, vbCrLf, ex4.GetDisplay(True) }), MsgBoxStyle.Critical, "更新失败")
                Catch ex5 As Exception
                    Interaction.MsgBox(String.Format("{0}{1}{2}详细错误信息：{3}{4}", New Object() { vb$AnonymousDelegate_("复制新版本文件时出错"), vbCrLf, vbCrLf, vbCrLf, ex5.GetDisplay(True) }), MsgBoxStyle.Critical, "更新失败")
                    Return
                End Try
                If TriggerRestart Then
                    Try
                        ModBase.StartProcess(CS$<>8__locals1.$VB$Local_NewFilePath, "")
                    Catch ex6 As Exception
                        Interaction.MsgBox(String.Format("PCL 更新已完成，但在自动重启 PCL 时遇到了意外，请手动启动 PCL。{0}{1}详细错误信息：{2}{3}", New Object() { vbCrLf, vbCrLf, vbCrLf, ex6.GetDisplay(True) }), MsgBoxStyle.Information, "更新完成")
                    End Try
                End If
                Return
            End If
            If TypeOf ex2 Is UnauthorizedAccessException Then
                Interaction.MsgBox(String.Format("{0}{1}{2}详细错误信息：{3}{4}", New Object() { vb$AnonymousDelegate_("删除老版本文件时权限不足"), vbCrLf, vbCrLf, vbCrLf, ex2.GetDisplay(True) }), MsgBoxStyle.Critical, "更新失败")
                Return
            End If
            Interaction.MsgBox(String.Format("{0}{1}{2}详细错误信息：{3}{4}", New Object() { vb$AnonymousDelegate_("删除老版本文件时出错"), vbCrLf, vbCrLf, vbCrLf, ex2.GetDisplay(True) }), MsgBoxStyle.Critical, "更新失败")
        End Sub

        Friend Sub DownloadLatestPCL(Optional LoaderToSyncProgress As ModLoader.LoaderBase = Nothing)
            Dim num As Integer = If(FileUtils.Exists(ModBase.PathTemp + "Latest.exe"), Settings.[Get](Of Integer)("SystemHighestSavedBetaVersionReg", Nothing), 0)
            Dim num2 As Integer = 0
            Dim text As String = If(FileUtils.TryReadAsString(ModBase.PathTemp + "Cache\Notice.cfg", Nothing, Nothing), "")
            If text.Split("|", False).Count() >= 3 Then
                num2 = Conversions.ToInteger(text.Split("|", False)(2))
            End If
            If num < num2 Then
                Logger.Info(String.Format("需要下载最新正式版：{0} -> {1}", num, num2), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                If ModSecret.ServerConfig Is Nothing Then
                    ModSecret.ServerLoader.WaitForExit(Nothing, Nothing, ModSecret.ServerLoader.State = ModBase.LoadState.Finished, Nothing, Nothing)
                End If
                If ModSecret.ServerConfig Is Nothing Then
                    Throw New Exception("无法从服务器获取正式版 PCL 下载链接！")
                End If
                Dim enumerable As IEnumerable(Of JToken) = ModSecret.ServerConfig("Update")("Release")("DownloadUrls")
                Dim func As Func(Of JToken, String)
                If ModSecret._Closure$__.$I47-0 IsNot Nothing Then
                    func = ModSecret._Closure$__.$I47-0
                Else
                    Dim func2 As Func(Of JToken, String) = Function(t As JToken) t.ToString()
                    func = func2
                    ModSecret._Closure$__.$I47-0 = func2
                End If
                ModNet.NetDownloadByLoader(enumerable.[Select](func).ToList(), ModBase.PathTemp + "Latest.zip", LoaderToSyncProgress, New FileChecker() With { .MinSize = 1048576L }, False)
                FileUtils.ExtractToDirectory(ModBase.PathTemp + "Latest.zip", ModBase.PathTemp, Nothing, Nothing)
                FileUtils.Delete(ModBase.PathTemp + "Latest.zip", False)
                FileUtils.Move(ModBase.PathTemp + "Plain Craft Launcher 2.exe", ModBase.PathTemp + "Latest.exe", False)
                Settings.[Set]("SystemHighestSavedBetaVersionReg", num2, Nothing)
            End If
        End Sub

        Private Sub ServerSub(Loader As ModLoader.LoaderTask(Of Integer, Integer))
            Try
                If FileUtils.Exists(Paths.Base + "PCL\update.exe") Then
                    FileUtils.Delete(Paths.Base + "PCL\update.exe", False)
                    Logger.Info("已清理更新缓存", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                End If
            Catch ex As Exception
                Logger.Warn(ex, "清理更新缓存失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
            End Try
            Dim num As Integer = 0
            Dim text As String = ModBase.PathTemp + "Cache\ServerConfig.json"
            Try
                If FileUtils.Exists(text) Then
                    ModSecret.ServerConfig = CType(FileUtils.ReadAsJson(text, Nothing, Nothing), JObject)
                    num = CInt(Math.Round(ModBase.Val(ModSecret.ServerConfig("Version"))))
                    Logger.Info(String.Format("已读取本地缓存的配置：{0}", num), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                End If
            Catch ex2 As Exception
                Logger.Warn(ex2, "读取配置信息失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                ModSecret.ServerConfig = Nothing
                FileUtils.Delete(text, False)
            End Try
            Loader.Progress = 0.2
            Dim num2 As Integer = Settings.[Get](Of Integer)("HintNotice", Nothing)
            Dim text2 As String = ModBase.PathTemp + "Cache\Notice.cfg"
            Dim text3 As String
            Dim num3 As Integer
            Try
                text3 = ModNet.NetRequestByClientRetry("https://pcl2-server-1253424809.file.myqcloud.com/notice.cfg{CDN}", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, False, Nothing, False)
                num3 = CInt(Math.Round(ModBase.Val(text3.Split("|", False)(0))))
                If num3 = 0 Then
                    Throw New Exception("获取到的内容有误！（" + text3 + "）")
                End If
                If num3 > num2 Then
                    Logger.Info(String.Format("本地公告编号：{0}，服务器公告：{1}，需更新", num2, text3), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                Else
                    Logger.Info(String.Format("本地公告编号：{0}，服务器公告：{1}，无需更新", num2, text3), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                End If
                FileUtils.Write(text2, text3, Nothing)
            Catch ex3 As Exception
                If TypeOf ex3 Is InvalidOperationException AndAlso ex3.Message.Contains("FIPS") Then
                    If ModMain.MyMsgBox("由于系统未启用 FIPS 兼容算法，PCL 可能无法正常运行。" & vbCrLf & "请按照教程启用该功能，然后重启 PCL。", "兼容性警告", "打开教程", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 1 Then
                        ModBase.OpenWebsite("https://blog.csdn.net/qq_37608398/article/details/81209922")
                    End If
                Else
                    Logger.Warn(ex3, "获取 PCL 服务器状态失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                    FileUtils.Delete(text2, False)
                End If
                Return
            End Try
            Loader.Progress = 0.4
            Try
                Dim num4 As Double = ModBase.Val(text3.Split("|", False)(4))
                If CDbl(num) < num4 Then
                    Logger.Info(String.Format("本地配置版本号：{0}，服务器配置版本号：{1}，需要更新", num, num4), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                    ModNet.NetDownloadByClient("https://pcl2-server-1253424809.file.myqcloud.com/ServerConfig.json{CDN}", text, False)
                    ModSecret.ServerConfig = CType(FileUtils.ReadAsJson(text, Nothing, Nothing), JObject)
                End If
            Catch ex4 As Exception
                Logger.Warn(ex4, "下载配置信息失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
            End Try
            Loader.Progress = 0.6
            Dim text4 As String
            Try
                If num3 <= num2 AndAlso FileUtils.Exists(ModBase.PathTemp + "Cache\Notice.json") Then
                    text4 = FileUtils.ReadAsString(ModBase.PathTemp + "Cache\Notice.json", Nothing, Nothing)
                Else
                    text4 = ModNet.NetRequestByClientRetry("https://pcl2-server-1253424809.file.myqcloud.com/notice.json{CDN}", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False)
                    FileUtils.Write(ModBase.PathTemp + "Cache\Notice.json", text4, Nothing)
                End If
                Settings.[Set]("HintNotice", num3, Nothing)
            Catch ex5 As Exception
                Logger.Warn(ex5, "下载 PCL 服务器公告失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                FileUtils.Delete(ModBase.PathTemp + "Cache\Notice.json", False)
                Return
            End Try
            Loader.Progress = 0.9
            Try
                Try
                    For Each jtoken As JToken In text4.Replace("{UNIQUE}", ModBase.Identify).DeserializeJson()
                        Dim jobject As JObject = CType(jtoken, JObject)
                        Dim num5 As Integer = CInt(Math.Round(ModBase.Val(If(jobject("id"), 0))))
                        If num5 > num2 Then
                            Dim flag As Boolean = True
                            Try
                                For Each jtoken2 As JToken In (If(jobject("requirements"), New Byte(-1) {}))
                                    Dim jproperty As JProperty = CType(jtoken2, JProperty)
                                    If Not ModSecret.ServerRequirement(jproperty.Name, jproperty.Value) Then
                                        flag = False
                                    End If
                                Next
                            Finally
                                Dim enumerator2 As IEnumerator(Of JToken)
                                If enumerator2 IsNot Nothing Then
                                    enumerator2.Dispose()
                                End If
                            End Try
                            If flag Then
                                Dim num6 As Integer = CInt((If(jobject("importantLevel"), 2)))
                                Dim flag2 As Boolean = CBool((If(jobject("isUpdate"), False)))
                                Logger.Info(String.Format("重要等级 {0}，更新公告 {1}", num6, flag2), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                                If Settings.[Get](Of Integer)(If(flag2, "SystemSystemUpdate", "SystemSystemActivity"), Nothing) <= num6 AndAlso (Not flag2 OrElse Not ModSecret.IsUpdating) Then
                                    Dim text5 As String = (If(jobject("title"), "公告")).ToString()
                                    Dim text6 As String = (If(jobject("description"), "")).ToString().Replace("\n", vbCrLf)
                                    Dim jarray As JArray = CType((If(jobject("buttons"), New JArray())), JArray)
                                    Dim count As Integer = jarray.Count
                                    Dim obj As Object = New Object(-1) {}
                                    If count > 0 Then
                                        Logger.Info(String.Format("显示公告 {0}：{1}", num5, text6), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                                        Dim num7 As Integer
                                        Select Case count
                                            Case 1
                                                num7 = ModMain.MyMsgBox(text6, text5, (If(jarray(0)("text"), "确定")).ToString(), "", "", False, True, True, Nothing, Nothing, Nothing)
                                            Case 2
                                                num7 = ModMain.MyMsgBox(text6, text5, (If(jarray(0)("text"), "确定")).ToString(), (If(jarray(1)("text"), "确定")).ToString(), "", False, True, False, Nothing, Nothing, Nothing)
                                            Case 3
                                                num7 = ModMain.MyMsgBox(text6, text5, (If(jarray(0)("text"), "确定")).ToString(), (If(jarray(1)("text"), "确定")).ToString(), (If(jarray(2)("text"), "确定")).ToString(), False, True, False, Nothing, Nothing, Nothing)
                                            Case Else
                                                Logger.Warn(String.Format("公告 {0} 的弹窗有 {1} 个按钮，无法显示", num5, count), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                                                Continue For
                                        End Select
                                        num7 = num7.Clamp(1, count)
                                        obj = If(jarray(num7 - 1)("actions"), New Byte(-1) {})
                                    Else
                                        obj = If(jobject("actions"), New Byte(-1) {})
                                    End If
                                    Try
                                        For Each obj2 As Object In CType(obj, IEnumerable)
                                            Dim jproperty2 As JProperty = CType(obj2, JProperty)
                                            Try
                                                Dim name As String = jproperty2.Name
                                                If Operators.CompareString(name, "copy", False) <> 0 Then
                                                    If Operators.CompareString(name, "website", False) <> 0 Then
                                                        If Operators.CompareString(name, "stop", False) <> 0 Then
                                                            If Operators.CompareString(name, "setup", False) <> 0 Then
                                                                If Operators.CompareString(name, "update", False) <> 0 AndAlso Operators.CompareString(name, "slientupdate", False) <> 0 Then
                                                                    Throw New Exception("未知的行动支：" + jproperty2.Name + ", " + jproperty2.Value.ToString())
                                                                End If
                                                            Else
                                                                Dim jobject2 As JObject = CType(jproperty2.Value, JObject)
                                                                Try
                                                                    For Each keyValuePair As KeyValuePair(Of String, JToken) In jobject2
                                                                        Settings.[Set](keyValuePair.Key, keyValuePair.Value.ToString(), Nothing)
                                                                    Next
                                                                    Continue For
                                                                Finally
                                                                    Dim enumerator4 As IEnumerator(Of KeyValuePair(Of String, JToken))
                                                                    If enumerator4 IsNot Nothing Then
                                                                        enumerator4.Dispose()
                                                                    End If
                                                                End Try
                                                            End If
                                                            Dim jobject3 As JObject = CType(jproperty2.Value, JObject)
                                                            Dim flag3 As Boolean = Operators.CompareString(jproperty2.Name, "slientupdate", False) = 0
                                                            If Not ModSecret.HasUpdatePermission Then
                                                                If Not flag3 Then
                                                                    ModSecret.InputPotatoCode(True)
                                                                End If
                                                                If Not ModSecret.HasUpdatePermission Then
                                                                    Continue For
                                                                End If
                                                            End If
                                                            ModSecret.UpdateStart(flag3, Nothing)
                                                        Else
                                                            ModMain.FrmMain.EndProgram(False)
                                                        End If
                                                    Else
                                                        ModBase.OpenWebsite(jproperty2.Value.ToString())
                                                    End If
                                                Else
                                                    ModBase.ClipboardSet(jproperty2.Value.ToString(), True)
                                                End If
                                            Catch ex6 As Exception
                                                Logger.[Error](ex6, String.Format("执行 PCL 服务器公告动作失败（{0}, {1}）", jproperty2.Name, jproperty2.Value), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                                            End Try
                                        Next
                                    Finally
                                        Dim enumerator3 As IEnumerator
                                        If TypeOf enumerator3 Is IDisposable Then
                                            TryCast(enumerator3, IDisposable).Dispose()
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
            Catch ex7 As Exception
                Logger.Warn(text4, LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                Logger.[Error](ex7, String.Format("读取 PCL 服务器公告失败（{0}）", text4.Length), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                FileUtils.Delete(ModBase.PathTemp + "Cache\Notice.json", False)
            End Try
        End Sub

        Private Function ServerRequirement(Name As String, Value As JToken) As Boolean
            ' The following expression was wrapped in a checked-statement
            Dim flag As Boolean
            Try
                Dim num As UInteger = <PrivateImplementationDetails>.ComputeStringHash(Name)
                If num <= 2207725236UI Then
                    If num <= 1143669093UI Then
                        If num <> 127006290UI Then
                            If num <> 233440242UI Then
                                If num = 1143669093UI Then
                                    If Operators.CompareString(Name, "setupboolean =", False) = 0 Then
                                        Return Settings.[Get](Of Boolean)(CType(Value, JProperty).Name, Nothing) = CType(Value, JProperty).Value.ToObject(Of Boolean)()
                                    End If
                                End If
                            ElseIf Operators.CompareString(Name, "unlockedtheme =", False) = 0 Then
                                Return ModSecret.ThemeCheckOne(CInt(Math.Round(ModBase.Val(Value))))
                            End If
                        ElseIf Operators.CompareString(Name, "versionbranch <=", False) = 0 Then
                            Return ModBase.Val(0) <= ModBase.Val(Value)
                        End If
                    ElseIf num <= 1834020325UI Then
                        If num <> 1699755989UI Then
                            If num = 1834020325UI Then
                                If Operators.CompareString(Name, "setupinteger >=", False) = 0 Then
                                    Return CDbl(Settings.[Get](Of Integer)(CType(Value, JProperty).Name, Nothing)) >= ModBase.Val(CType(Value, JProperty).Value)
                                End If
                            End If
                        ElseIf Operators.CompareString(Name, "opencount >=", False) = 0 Then
                            Return CDbl(Settings.[Get](Of Integer)("SystemCount", Nothing)) >= ModBase.Val(Value)
                        End If
                    ElseIf num <> 1931416207UI Then
                        If num = 2207725236UI Then
                            If Operators.CompareString(Name, "versionbranch >=", False) = 0 Then
                                Return ModBase.Val(0) >= ModBase.Val(Value)
                            End If
                        End If
                    ElseIf Operators.CompareString(Name, "debug =", False) = 0 Then
                        Return Operators.CompareString(ModBase.ModeDebug.ToString().Lower(), Value.ToString().Lower(), False) = 0
                    End If
                ElseIf num <= 3063258025UI Then
                    If num <> 2410018677UI Then
                        If num <> 2561636181UI Then
                            If num = 3063258025UI Then
                                If Operators.CompareString(Name, "unlockedtheme !=", False) = 0 Then
                                    Return Not ModSecret.ThemeCheckOne(CInt(Math.Round(ModBase.Val(Value))))
                                End If
                            End If
                        ElseIf Operators.CompareString(Name, "uniqueaddress =", False) = 0 Then
                            Return Operators.CompareString(ModBase.Identify, Value.ToString(), False) = 0
                        End If
                    ElseIf Operators.CompareString(Name, "d10000 <=", False) = 0 Then
                        Return CDbl(ModBase.RandomInteger(1, 10000)) <= ModBase.Val(Value)
                    End If
                ElseIf num <= 3317911237UI Then
                    If num <> 3250506571UI Then
                        If num = 3317911237UI Then
                            If Operators.CompareString(Name, "versioncode <=", False) = 0 Then
                                Return 414.0 <= ModBase.Val(Value)
                            End If
                        End If
                    ElseIf Operators.CompareString(Name, "versioncode >=", False) = 0 Then
                        Return 414.0 >= ModBase.Val(Value)
                    End If
                ElseIf num <> 3779783387UI Then
                    If num = 3914047723UI Then
                        If Operators.CompareString(Name, "setupinteger <=", False) = 0 Then
                            Return CDbl(Settings.[Get](Of Integer)(CType(Value, JProperty).Name, Nothing)) <= ModBase.Val(CType(Value, JProperty).Value)
                        End If
                    End If
                ElseIf Operators.CompareString(Name, "opencount <=", False) = 0 Then
                    Return CDbl(Settings.[Get](Of Integer)("SystemCount", Nothing)) <= ModBase.Val(Value)
                End If
                Logger.Warn(String.Format("未知的条件支：{0}, {1}", Name, Value), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                flag = False
            Catch ex As Exception
                Logger.Warn(String.Format("判断分支条件失败：{0}, {1}", Name, Value), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                flag = False
            End Try
            Return flag
        End Function

        Public ReadOnly Property CurrentRank As DonationRank
            Get
                For Each donationRank As DonationRank In New DonationRank() { DonationRank.Rank98, DonationRank.Rank54, DonationRank.Rank23, DonationRank.Rank12, DonationRank.Rank6 }
                    If ModSecret.PotatoFeatures.Contains(donationRank.ToString()) Then
                        Return donationRank
                    End If
                Next
                Return DonationRank.None
            End Get
        End Property

        Private ReadOnly Property HasUpdatePermission As Boolean
            Get
                Return ModSecret.CurrentRank >= DonationRank.Rank12 OrElse ModSecret.PotatoFeatures.Contains("Update")
            End Get
        End Property

        Private ReadOnly Property PotatoFeatures As List(Of String)
            Get
                ' The following expression was wrapped in a checked-statement
                If ModSecret._PotatoFeatures Is Nothing Then
                    ModSecret._PotatoFeatures = New List(Of String)()
                    Dim array As String() = Settings.[Get](Of String)("Potatoes", Nothing).ToString().Split("|"c, True)
                    For i As Integer = 0 To array.Length - 1
                        Dim oneOf As OneOf(Of List(Of String), String) = ModSecret.CheckPotatoCode(array(i))
                        If oneOf.[Is](Of List(Of String))() Then
                            ModSecret._PotatoFeatures.AddRange(oneOf.[As](Of List(Of String))())
                        End If
                    Next
                    ModSecret._PotatoFeatures = ModSecret._PotatoFeatures.Distinct().ToList()
                End If
                Return ModSecret._PotatoFeatures
            End Get
        End Property

        Private Function CheckPotatoCode(Key As String) As OneOf(Of List(Of String), String)
            Dim oneOf As OneOf(Of List(Of String), String)
            If String.IsNullOrEmpty(Key) Then
                oneOf = "输入的土豆码为空。"
            ElseIf Key.Length < 30 AndAlso Key.ContainsIgnoreCase(ModBase.Identify) Then
                oneOf = String.Format("你输入的是识别码，不是土豆码。{0}要获取土豆码，请在爱发电私信发送【土豆 {1}】。", vbCrLf, ModBase.Identify)
            ElseIf Key.Length > 85 AndAlso Key.Length < 90 AndAlso Key.EndsWithF("=", False) Then
                oneOf = String.Format("你输入的是解锁码，不是土豆码。{0}要获取土豆码，请在爱发电私信发送【土豆】。", vbCrLf)
            ElseIf Key.Length > 150 AndAlso Key.Length < 165 AndAlso Key.EndsWithF("11", False) Then
                oneOf = String.Format("你输入的是更新密钥，不是土豆码。{0}要获取土豆码，请在爱发电私信发送【土豆】。", vbCrLf)
            ElseIf Not Key.StartsWithF("CD", True) OrElse Not Key.Contains("#") Then
                oneOf = String.Format("你输入的不是土豆码……有一说一，这是啥玩意儿？{0}要获取土豆码，请在爱发电私信发送【土豆 {1}】。", vbCrLf, ModBase.Identify)
            ElseIf Key(2) > "1"c Then
                oneOf = String.Format("这个土豆码得在新版 PCL 上才能使用。{0}请更新 PCL，或是在爱发电私信发送【下载】，然后在新下载的 PCL 上输入土豆码。", vbCrLf)
            ElseIf "1"c > Key(3) Then
                oneOf = String.Format("这个土豆码不太新鲜，已经过期了。{0}请在爱发电私信重新发送【土豆 {1}】，获取更新鲜的土豆码。", vbCrLf, ModBase.Identify)
            ElseIf Not Key.StartsWithF("CD", False) Then
                oneOf = String.Format("土豆码区分大小写，但你把一些大写的东西输成小写的了。{0}你可以选中文本，然后右键复制粘贴，不必手动输入。", vbCrLf)
            Else
                Try
                    Dim array As String() = Key.Substring(4).Replace("p", "=").Replace("%", "p").Split(New Char() { "#"c })
                    Dim [string] As String = Encoding.UTF8.GetString(Convert.FromBase64String(array(1)))
                    CryptographyUtils.EcdsaVerify(ModBase.Identify + Conversions.ToString(Key(2)) + Conversions.ToString(Key(3)) + [string], array(0), "RUNTMSAAAAC4QTUNAewh23Q4Q6koHkyIrDIIZUSbua23sf2DiZmIRwSzadISDRyTVTbuWniH3KR7rKj8XBsabms1be6i3c+S")
                    oneOf = [string].Split(","c, True).ToList()
                Catch ex As Exception
                    Logger.Warn(ex, String.Format("土豆码校验失败（{0}）", Key), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                    oneOf = String.Format("土豆码有误，请检查：{0}- 私信显示的识别码应当为 {1}，如果不是，请在私信发送【土豆 {2}】{3}- 你输入的以下土豆码是否与私信中的一致：{4}{5}{6}{7}错误原因：{8}", New Object() { vbCrLf, ModBase.Identify, ModBase.Identify, vbCrLf, vbCrLf, Key, vbCrLf, vbCrLf, ex.Message })
                End Try
            End If
            Return oneOf
        End Function

        Public Sub InputPotatoCode(IsUpdating As Boolean)
            Dim text6 As String
            If IsUpdating Then
                Dim text As String = String.Format("请点击【复制命令】，然后将复制的命令粘贴到爱发电私信，获取土豆码。{0}输入土豆码后，就能继续更新啦！", vbCrLf)
                Dim text2 As String = "验证爱发电赞助"
                Dim text3 As String = "输入土豆码"
                Dim text4 As String = "复制命令"
                Dim text5 As String = "取消"
                Dim flag As Boolean = False
                Dim flag2 As Boolean = True
                Dim flag3 As Boolean = False
                Dim action As Action = Nothing
                Dim action2 As Action
                If ModSecret._Closure$__.$I62-0 IsNot Nothing Then
                    action2 = ModSecret._Closure$__.$I62-0
                Else
                    Dim action3 As Action = Sub()
                        ModBase.ClipboardSet("土豆 " + ModBase.Identify, False)
                        ModBase.OpenWebsite("https://meloong.com/afd/message/df2dfad2960911e89c5252540025c377?potato")
                    End Sub
                    action2 = action3
                    ModSecret._Closure$__.$I62-0 = action3
                End If
                If ModMain.MyMsgBox(text, text2, text3, text4, text5, flag, flag2, flag3, action, action2, Nothing) = 3 Then
                    Return
                End If
                text6 = ModMain.MyMsgBoxInput("输入土豆码", String.Format("请在【爱发电】向龙腾猫跃发送【土豆 {0}】，然后输入自动回复的土豆码以继续更新。{1}自动回复可能比较慢，发送后等一会儿刷新一下网页应该就有了。", ModBase.Identify, vbCrLf), Nothing, New Collection(Of Validate)() From { New ValidateNullOrWhiteSpace() }, "", "确定", "取消", False)
            Else
                text6 = ModMain.MyMsgBoxInput("输入土豆码", String.Format("请输入你获得的土豆码。{0}你的识别码：{1}", vbCrLf, ModBase.Identify), Nothing, New Collection(Of Validate)() From { New ValidateNullOrWhiteSpace() }, "", "确定", "取消", False)
            End If
            If Not String.IsNullOrWhiteSpace(text6) Then
                text6 = text6.ReplaceLineEndings("", False).Between("：", "在", False).Trim(New Char() { " "c, "【"c, "】"c })
                If Not String.IsNullOrWhiteSpace(text6) Then
                    Logger.Info(String.Format("输入的土豆码：{0}", text6), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                    If Not IsUpdating AndAlso Operators.CompareString(text6, "PuzzleUnlock" + ModBase.Identify.Substring(ModBase.Identify.Count() - 4), False) = 0 OrElse Operators.CompareString(text6, "PuzzleUnlock" + ModBase.Identify.Substring(ModBase.Identify.Count() - 5), False) = 0 OrElse Operators.CompareString(text6, "PuzzleUnlock" + ModBase.Identify.Substring(ModBase.Identify.Count() - 6), False) = 0 Then
                        ModMain.MyMsgBox(String.Format("隐藏主题 极客蓝 已解锁！{0}难道，这就是终点了？还是说……？", vbCrLf), "解锁新隐藏主题！", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
                        ModSecret.ThemeUnlock(11, True, Nothing)
                        Return
                    End If
                    Try
                        Dim oneOf As OneOf(Of List(Of String), String) = ModSecret.CheckPotatoCode(text6)
                        If oneOf.[Is](Of String)() Then
                            Throw New Exception(oneOf.[As](Of String)())
                        End If
                        Dim list As List(Of String) = oneOf.[As](Of List(Of String))()
                        Logger.Info(String.Format("土豆码包含的特性：{0}", list.Join(", ")), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                        Dim enumerable As IEnumerable(Of String) = list
                        Dim func As Func(Of String, Boolean)
                        If ModSecret._Closure$__.$I62-1 IsNot Nothing Then
                            func = ModSecret._Closure$__.$I62-1
                        Else
                            Dim func2 As Func(Of String, Boolean) = Function(f As String) Not ModSecret.PotatoFeatures.Contains(f)
                            func = func2
                            ModSecret._Closure$__.$I62-1 = func2
                        End If
                        If enumerable.Any(func) Then
                            Dim list2 As List(Of String) = Settings.[Get](Of String)("Potatoes", Nothing).ToString().Split("|"c, True).ToList()
                            list2.Add(text6)
                            Settings.[Set]("Potatoes", list2.Join("|"), Nothing)
                            ModSecret._PotatoFeatures = Nothing
                        End If
                        If IsUpdating Then
                            If Not ModSecret.HasUpdatePermission Then
                                Throw New Exception("这个土豆码里不包含更新权限……")
                            End If
                        Else
                            ModMain.Hint("已成功解锁！", ModMain.HintType.Green, True)
                        End If
                    Catch ex As Exception
                        Logger.Info(String.Format("土豆码无效：{0}", ex.Message), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                        If ModMain.MyMsgBox(ex.Message, "错误", "重新输入", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 1 Then
                            ModSecret.InputPotatoCode(IsUpdating)
                        End If
                    End Try
                End If
            End If
        End Sub

        Friend Sub GeneratePotatoCode()
        End Sub

        Friend Function GetIdentify() As String
            Dim text As String
            Try
                If ModBase.ApplicationStartTick < 1L Then
                    text = "0000-0000-0000-0000"
                Else
                    Dim text2 As String
                    Try
                        text2 = RegistryUtils.TryRead(String.Format("HKEY_LOCAL_MACHINE\SYSTEM\Har{0}wareConfig", "D".Lower()), "LastConfig", "Unknown").ToString().Upper().Trim(New Char() { "{"c, "}"c })
                    Catch ex As Exception
                        Logger.Warn(ex, "获取主板标识码失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                        text2 = "Unknown"
                    End Try
                    Dim text3 As String
                    Try
                        text3 = Settings.[Get](Of String)("Identify", Nothing)
                    Catch ex2 As Exception
                        text3 = ""
                    End Try
                    If text3.Length < 3 Then
                        text3 = Conversions.ToString(ModBase.GetTimeMs()) + Conversions.ToString(MyWpfExtension.Computer.Info.AvailablePhysicalMemory)
                        Settings.[Set]("Identify", text3, Nothing)
                    End If
                    Dim text4 As String = (text2 + text3).GetStableHashCode().ToString("X").EnsureLength("7"c, 16)
                    text = String.Format("{0}-{1}-{2}-{3}", New Object() { text4.Substring(4, 4), text4.Substring(12, 4), text4.Substring(0, 4), text4.Substring(8, 4) })
                End If
            Catch ex3 As Exception
                Logger.[Error](ex3, String.Format("PCL 无法获取设备识别码，这可能会导致部分设置无法正常存储。{0}{1}详细的错误信息", vbCrLf, vbCrLf), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\ModSecret.vb")
                text = "0000-0000-0000-0000"
            End Try
            Return text
        End Function

        Public Const VersionBranchMain As String = "Official"

        Public Const RegFolder As String = "PCL"

        Public Const OAuthClientId As String = "fe72edc2-3a6f-4280-90e8-e2beb64ce7e1"

        Public Const ClsBaseUrl As String = "https://ap-shanghai.cls.tencentcs.com/track?topic_id=pcl-remote-1253424809"

        Public Const RestrictMcDownload As Boolean = True

        Public Color1 As ModBase.MyColor = New ModBase.MyColor(52.0, 61.0, 74.0)

        Public Color2 As ModBase.MyColor = New ModBase.MyColor(11.0, 91.0, 203.0)

        Public Color3 As ModBase.MyColor = New ModBase.MyColor(19.0, 112.0, 243.0)

        Public Color4 As ModBase.MyColor = New ModBase.MyColor(72.0, 144.0, 245.0)

        Public Color5 As ModBase.MyColor = New ModBase.MyColor(150.0, 192.0, 249.0)

        Public Color6 As ModBase.MyColor = New ModBase.MyColor(213.0, 230.0, 253.0)

        Public Color7 As ModBase.MyColor = New ModBase.MyColor(222.0, 236.0, 253.0)

        Public Color8 As ModBase.MyColor = New ModBase.MyColor(234.0, 242.0, 254.0)

        Public ColorBg0 As ModBase.MyColor = New ModBase.MyColor(150.0, 192.0, 249.0)

        Public ColorBg1 As ModBase.MyColor = New ModBase.MyColor(190.0, ModSecret.Color7)

        Public ColorGray1 As ModBase.MyColor = New ModBase.MyColor(64.0, 64.0, 64.0)

        Public ColorGray2 As ModBase.MyColor = New ModBase.MyColor(115.0, 115.0, 115.0)

        Public ColorGray3 As ModBase.MyColor = New ModBase.MyColor(140.0, 140.0, 140.0)

        Public ColorGray4 As ModBase.MyColor = New ModBase.MyColor(166.0, 166.0, 166.0)

        Public ColorGray5 As ModBase.MyColor = New ModBase.MyColor(204.0, 204.0, 204.0)

        Public ColorGray6 As ModBase.MyColor = New ModBase.MyColor(235.0, 235.0, 235.0)

        Public ColorGray7 As ModBase.MyColor = New ModBase.MyColor(240.0, 240.0, 240.0)

        Public ColorGray8 As ModBase.MyColor = New ModBase.MyColor(245.0, 245.0, 245.0)

        Public ColorSemiTransparent As ModBase.MyColor = New ModBase.MyColor(1.0, ModSecret.Color8)

        Public ThemeNow As Integer = -1

        Public ColorHue As Integer = 210

        Public ColorSat As Integer = 85

        Public ColorLightAdjust As Integer = 0

        Public ColorHueTopbarDelta As OneOf(Of Integer, Integer()) = 0

        Public ThemeDontClick As Integer = 0

        Private IsThemeColorfulLoaded As Boolean = False

        Private ColorHueDelta As Integer = 1

        Private IsUpdating As Boolean = False

        Friend IsUpdateWaitingRestart As Boolean = False

        Private IsUpdateRestartWaitingTasks As Boolean = False

        Private UpdateMark As String = "EmptyMark********************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************************"

        Public ServerConfig As JObject

        Public ServerLoader As ModLoader.LoaderTask(Of Integer, Integer) = New ModLoader.LoaderTask(Of Integer, Integer)("PCL 配置更新", AddressOf ModSecret.ServerSub, Nothing, ThreadPriority.BelowNormal) With { .ReloadTimeout = 3600000 }

        Private _PotatoFeatures As List(Of String) = Nothing

        Private Const EcdsaPublicKey As String = "RUNTMSAAAAC4QTUNAewh23Q4Q6koHkyIrDIIZUSbua23sf2DiZmIRwSzadISDRyTVTbuWniH3KR7rKj8XBsabms1be6i3c+S"

        Private $STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate As Boolean

        Private $STATIC$UpdateCheckByButton$001$IsUiCheckingUpdate$Init As StaticLocalInitFlag

        Private $STATIC$UpdateStart$0212151280E91E$IsSilentUpdate As Boolean
    End Module
End Namespace
