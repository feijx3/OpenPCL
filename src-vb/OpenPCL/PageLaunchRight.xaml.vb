Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Linq
Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Markup
Imports System.Windows.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports PCLCS

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageLaunchRight
        Inherits OurPageRight
        Implements IRefreshable, IDispatcherUnhandledException

        Public Sub New()
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.Init()
            End Sub
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.Refresh()
            End Sub
            Me.RefreshLock = RuntimeHelpers.GetObjectValue(New Object())
            Me.OnlineLoader = New ModLoader.LoaderTask(Of ValueTuple(Of String, Boolean), Integer)("下载主页", AddressOf Me.OnlineLoaderSub, Nothing, ThreadPriority.Normal) With { .ReloadTimeout = 600000 }
            Me.LoadedContentHash = Nothing
            Me.LoadContentLock = RuntimeHelpers.GetObjectValue(New Object())
            Me.InitializeComponent()
        End Sub

        Private Sub Init()
            Me.PanBack.ScrollToHome()
            Me.PanLog.Visibility = ModBase.ModeDebug.ToVisibility()
            Me.PanHint.Visibility = (ModSecret.CurrentRank < DonationRank.Rank23 AndAlso Not Settings.[Get](Of Boolean)("HintSnapshot", Nothing)).ToVisibility()
        End Sub

        Private Sub Refresh()
            ModBase.RunInThread(Sub()
                Try
                    Dim refreshLock As Object = Me.RefreshLock
                    ObjectFlowControl.CheckForSyncLockOnValueType(refreshLock)
                    SyncLock refreshLock
                        Me.RefreshReal()
                    End SyncLock
                Catch ex As Exception
                    Logger.[Error](ex, "加载 PCL 主页自定义信息失败", If(ModBase.ModeDebug, LogBehavior.Alert, LogBehavior.Toast), "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                End Try
            End Sub)
        End Sub

        Private Sub RefreshReal()
            Dim Content As String = Nothing
            Dim text As String = Nothing
            Select Case Settings.[Get](Of Integer)("UiCustomType", Nothing)
                Case 1
                    Logger.Info("主页自定义数据来源：本地文件", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                    Content = FileUtils.TryReadAsString(Paths.Base + "PCL\Custom.xaml", Nothing, Nothing)
                Case 2
                    text = Settings.[Get](Of String)("UiCustomNet", Nothing)
                Case 3
                    Select Case Settings.[Get](Of Integer)("UiCustomPreset", Nothing)
                        Case 0
                            Logger.Info("主页预设：你知道吗", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            Content = vbCrLf & "                            <local:OurCard Title=""你知道吗？"" Margin=""0,0,0,15"">" & vbCrLf & "                                <TextBlock Margin=""25,38,23,15"" FontSize=""13.5"" IsHitTestVisible=""False"" Text=""{hint}"" TextWrapping=""Wrap"" Foreground=""{DynamicResource ColorBrush1}"" />" & vbCrLf & "                                <local:OurIconButton Height=""22"" Width=""22"" Margin=""9"" VerticalAlignment=""Top"" HorizontalAlignment=""Right"" " & vbCrLf & "                                    EventType=""刷新主页"" EventData=""/""" & vbCrLf & "                                    Logo=""M512.0 838.3c-80.2 0-153.4-29.3-210.2-77.4l75.5-75.5c11.5-11.5 25.8-22.0 25.8-37.0a27.2 27.2 0 0 0-27.1-27.1H104.0c-27.1 0-27.1 23.9-27.1 27.1v271.9a27.1 27.1 0 0 0 27.1 27.1c15.0 0 27.8-16.6 42.5-31.2l77.9-77.9c76.6 67.7 177.1 108.9 287.4 108.9 221.7 0 404.5-166.0 431.2-380.6h-109.8c-25.9 154.2-159.7 271.9-321.3 271.9zM919.9 76.6c-15.0 0-27.8 16.6-42.5 31.3L799.5 185.8c-76.5-67.7-177.1-108.9-287.4-108.9-221.8 0-404.5 166.1-431.3 380.6H190.6c25.9-154.2 159.7-271.9 321.4-271.9 80.2 0 153.4 29.3 210.1 77.4l-75.5 75.5c-11.6 11.5-25.8 22.0-25.8 37.1a27.2 27.2 0 0 0 27.1 27.1h271.9c27.1 0 27.1-23.9 27.1-27.1V103.8a27.1 27.1 0 0 0-27.1-27.1z"" />" & vbCrLf & "                            </local:OurCard>"
                        Case 1
                            Logger.Info("主页预设：回声洞", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            Content = vbCrLf & "                            <local:OurCard Title=""回声洞"" Margin=""0,0,0,15"">" & vbCrLf & "                                <TextBlock Margin=""25,38,23,15"" FontSize=""13.5"" IsHitTestVisible=""False"" Text=""{cave}"" TextWrapping=""Wrap"" Foreground=""{DynamicResource ColorBrush1}"" />" & vbCrLf & "                                <local:OurIconButton Height=""22"" Width=""22"" Margin=""9"" VerticalAlignment=""Top"" HorizontalAlignment=""Right"" " & vbCrLf & "                                    EventType=""刷新主页"" EventData=""/""" & vbCrLf & "                                    Logo=""M512.0 838.3c-80.2 0-153.4-29.3-210.2-77.4l75.5-75.5c11.5-11.5 25.8-22.0 25.8-37.0a27.2 27.2 0 0 0-27.1-27.1H104.0c-27.1 0-27.1 23.9-27.1 27.1v271.9a27.1 27.1 0 0 0 27.1 27.1c15.0 0 27.8-16.6 42.5-31.2l77.9-77.9c76.6 67.7 177.1 108.9 287.4 108.9 221.7 0 404.5-166.0 431.2-380.6h-109.8c-25.9 154.2-159.7 271.9-321.3 271.9zM919.9 76.6c-15.0 0-27.8 16.6-42.5 31.3L799.5 185.8c-76.5-67.7-177.1-108.9-287.4-108.9-221.8 0-404.5 166.1-431.3 380.6H190.6c25.9-154.2 159.7-271.9 321.4-271.9 80.2 0 153.4 29.3 210.1 77.4l-75.5 75.5c-11.6 11.5-25.8 22.0-25.8 37.1a27.2 27.2 0 0 0 27.1 27.1h271.9c27.1 0 27.1-23.9 27.1-27.1V103.8a27.1 27.1 0 0 0-27.1-27.1z"" />" & vbCrLf & "                            </local:OurCard>"
                        Case 2
                            Logger.Info("主页预设：Minecraft 新闻", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://mcnews.meloong.com"
                        Case 3
                            Logger.Info("主页预设：简单主页", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://pclhomeplazaoss.lingyunawa.top:26994/d/Homepages/MFn233/Custom.xaml"
                        Case 4
                            Logger.Info("主页预设：每日整合包推荐", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://pclsub.sodamc.com/"
                        Case 5
                            Logger.Info("主页预设：Minecraft 皮肤推荐", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://forgepixel.com/pcl_sub_file"
                        Case 6
                            Logger.Info("主页预设：OpenBMCLAPI 仪表盘 Lite", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://pcl-bmcl.milu.ink/"
                        Case 7
                            Logger.Info("主页预设：主页市场", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://pclhomeplazaoss.lingyunawa.top:26994/p/Homepages/Homepage.Market/Custom.xaml"
                        Case 8
                            Logger.Info("主页预设：更新日志", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://pclhomeplazaoss.lingyunawa.top:26994/d/Homepages/Joker2184/UpdateHomepage.xaml"
                        Case 9
                            Logger.Info("主页预设：PCL 新功能说明书", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://raw.gitcode.com/WForst-Breeze/WhatsNewPCL/raw/main/Custom.xaml"
                        Case 10
                            Logger.Info("主页预设：OpenMCIM Dashboard", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://files.mcimirror.top/PCL"
                        Case 11
                            Logger.Info("主页预设：杂志主页", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://gh-proxy.com/https://github.com/Neclyon/Magazine-Homepage-PCL/raw/main/output/Custom.xaml"
                        Case 12
                            Logger.Info("主页预设：PCL GitHub 仪表盘", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://ddf.pcl-community.top/Custom.xaml"
                        Case 13
                            Logger.Info("主页预设：PCL 更新摘要", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://raw.gitcode.com/ENC_Euphony/PCL-AI-Summary-HomePage/raw/master/Custom.xaml"
                        Case 14
                            Logger.Info("主页预设：今日新闻热点", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://pcl.wyc-w.top/index.xaml"
                        Case 15
                            Logger.Info("主页预设：Minecraft 芝士站", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://mkss-services.awaeric.xyz"
                        Case 16
                            Logger.Info("主页预设：整合包推荐引擎", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://qawsedrftgyhujiko.fun/pcl2/Custom.xaml"
                        Case 17
                            Logger.Info("主页预设：Bangumi 番剧主页", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://bangumi.p.kaphia.top"
                        Case 18
                            Logger.Info("主页预设：Bilibili 热门", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://bilibili.p.kaphia.top"
                        Case 19
                            Logger.Info("主页预设：Music 云音乐热门", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://cloudmusic.p.kaphia.top"
                        Case 20
                            Logger.Info("主页预设：模组推荐", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://g-fish.dpdns.org/download/ModPage.xaml"
                        Case 21
                            Logger.Info("主页预设：每日游戏推荐", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://pcl.xn--9iq209asnop94b.top/download/GamePage.xaml"
                        Case 22
                            Logger.Info("主页预设：Wallpaper 每日壁纸", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://wallpaper.p.kaphia.top"
                        Case 23
                            Logger.Info("主页预设：Weather 天气预报", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://weather-p.kaphia.top/{variable:PCLDailyWeatherPage:choose}.xaml"
                        Case 24
                            Logger.Info("主页预设：新一代狗屁王大杂烩主页", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            text = "https://end.g-fish.dpdns.org/Custom.xaml"
                    End Select
            End Select
            If Not String.IsNullOrWhiteSpace(text) Then
                text = ModMain.ArgumentReplace(text, New Func(Of String, String)(WebUtility.HtmlEncode), True)
                If Operators.CompareString(text, Settings.[Get](Of String)("CacheSavedPageUrl", Nothing), False) <> 0 OrElse Not FileUtils.Exists(ModBase.PathTemp + "Cache\Custom.xaml") Then
                    Logger.Info("主页自定义数据来源：联网全新下载", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                    ModMain.Hint("正在加载主页……", ModMain.HintType.Blue, True)
                    ModBase.RunInUiWait(Sub()
                        Me.LoadContent(Nothing)
                    End Sub)
                    Settings.[Set]("CacheSavedPageVersion", "", Nothing)
                    Me.OnlineLoader.Start(New ValueTuple(Of String, Boolean)(text, True), False)
                    Return
                End If
                Logger.Info("主页自定义数据来源：联网缓存文件", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                Content = FileUtils.TryReadAsString(ModBase.PathTemp + "Cache\Custom.xaml", Nothing, Nothing)
                Me.OnlineLoader.Start(New ValueTuple(Of String, Boolean)(text, False), False)
            End If
            ModBase.RunInUi(Sub()
                Me.LoadContent(Content)
            End Sub, False)
        End Sub

        Private Sub OnlineLoaderSub(<TupleElementNames(New String() { "Address", "ShouldRefresh" })> Task As ModLoader.LoaderTask(Of ValueTuple(Of String, Boolean), Integer))
            Dim item As String = Task.Input.Item1
            Dim item2 As Boolean = Task.Input.Item2
            Try
                Dim text As String
                If item.Contains(".xaml") Then
                    text = item.Replace(".xaml", ".xaml.ini")
                Else
                    text = item.BeforeFirst("?", False)
                    If Not text.EndsWithF("/", False) Then
                        text += "/"
                    End If
                    text += "version"
                    If item.Contains("?") Then
                        text = text + "?" + item.AfterFirst("?", False)
                    End If
                End If
                Dim text2 As String = ""
                Try
                    text2 = ModNet.NetRequestByClientRetry(text, Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, False, Nothing, False)
                    If text2.Length > 1000 Then
                        Throw New Exception(String.Format("获取的主页版本过长（{0} 字符）", text2.Length))
                    End If
                    Dim text3 As String = Settings.[Get](Of String)("CacheSavedPageVersion", Nothing)
                    If Operators.CompareString(text2, "", False) <> 0 AndAlso Operators.CompareString(text3, "", False) <> 0 AndAlso Operators.CompareString(text2, text3, False) = 0 Then
                        Logger.Info(String.Format("当前缓存的主页已为最新，当前版本：{0}，检查源：{1}", text2, text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                        Return
                    End If
                    Logger.Info(String.Format("需要下载联网主页，当前版本：{0}，检查源：{1}", text2, text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                Catch ex As Exception
                    Logger.Warn(ex, "联网获取主页版本失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                    Logger.Info(String.Format("无法检查联网主页版本，将直接下载，检查源：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                End Try
                Dim text4 As String = ModNet.NetRequestByClientRetry(item, Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, False, Nothing, False)
                Logger.Info(String.Format("已联网下载主页，内容长度：{0}，来源：{1}", text4.Length, item), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                Settings.[Set]("CacheSavedPageUrl", item, Nothing)
                Settings.[Set]("CacheSavedPageVersion", text2, Nothing)
                FileUtils.Write(ModBase.PathTemp + "Cache\Custom.xaml", text4, Nothing)
                Dim loadedContentHash As ULong? = Me.LoadedContentHash
                Dim stableHashCode As ULong = text4.GetStableHashCode()
                Dim flag As Boolean? = If((loadedContentHash IsNot Nothing), New Boolean?(loadedContentHash.GetValueOrDefault() <> stableHashCode), Nothing)
                If(flag Is Nothing OrElse flag.GetValueOrDefault()) AndAlso item2 AndAlso flag IsNot Nothing Then
                    Me.Refresh()
                End If
            Catch ex2 As Exception
                Logger.[Error](ex2, String.Format("下载主页失败（{0}）", item), If(ModBase.ModeDebug, LogBehavior.Alert, LogBehavior.Toast), "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
            End Try
        End Sub

        Public Sub ForceRefresh() Implements PCL.IRefreshable.Refresh
            Logger.Info("要求强制刷新主页", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
            Me.ClearCache()
            If ModMain.FrmMain.PageCurrent.Page = FormMain.PageType.Launch Then
                Me.PanBack.ScrollToHome()
                Me.Refresh()
                Return
            End If
            ModMain.FrmMain.PageChange(FormMain.PageType.Launch, FormMain.PageSubType.[Default])
        End Sub

        Private Sub ClearCache()
            Me.LoadedContentHash = Nothing
            Me.OnlineLoader.Input = New ValueTuple(Of String, Boolean)("", True)
            Settings.[Set]("CacheSavedPageUrl", "", Nothing)
            Settings.[Set]("CacheSavedPageVersion", "", Nothing)
            Logger.Info("已清空主页缓存", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
        End Sub

        Private Sub LoadContent(Content As String)
            Try
                Dim loadContentLock As Object = Me.LoadContentLock
                ObjectFlowControl.CheckForSyncLockOnValueType(loadContentLock)
                SyncLock loadContentLock
                    Dim stableHashCode As ULong = (If(Content, "")).GetStableHashCode()
                    Dim num As ULong = stableHashCode
                    Dim loadedContentHash As ULong? = Me.LoadedContentHash
                    If Not If((loadedContentHash IsNot Nothing), New Boolean?(num = loadedContentHash.GetValueOrDefault()), Nothing).GetValueOrDefault() Then
                        Me.LoadedContentHash = New ULong?(stableHashCode)
                        Me.PanCustom.Children.Clear()
                        If String.IsNullOrWhiteSpace(Content) Then
                            Logger.Info("实例化：清空主页 UI，来源为空", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                        Else
                            Dim now As DateTime = DateTime.Now
                            Content = ModMain.ArgumentReplace(Content, New Func(Of String, String)(StringUtils.XmlEscape), True)
                            While Content.Contains("xmlns")
                                Content = Content.RegexReplace("xmlns[^""']*(""|')[^""']*(""|')", "", RegexOptions.None).Replace("xmlns", "")
                            End While
                            Content = "<StackPanel xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:sys=""clr-namespace:System;assembly=mscorlib"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:local=""clr-namespace:PCL;assembly=OpenPCL"" xmlns:core=""clr-namespace:MeloongCore;assembly=MeloongCore"" xmlns:corewpf=""clr-namespace:MeloongCore.Wpf;assembly=MeloongCore.Wpf"">" + Content + "</StackPanel>"
                            Logger.Info(String.Format("实例化：加载主页 UI 开始，最终内容长度：{0}", Content.Count()), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            Me.PanCustom.Children.Add(CType(ModBase.GetObjectFromXML(Content), UIElement))
                            Dim milliseconds As Integer = (DateTime.Now - now).Milliseconds
                            Logger.Info(String.Format("实例化：加载主页 UI 完成，耗时 {0}ms", milliseconds), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                            If milliseconds > 3000 Then
                                ModMain.Hint(String.Format("主页加载过于缓慢（花费了 {0} 秒），请向主页作者反馈此问题，或暂时停止使用该主页", Math.Round(CDbl(milliseconds) / 1000.0, 1)), ModMain.HintType.Blue, True)
                            End If
                        End If
                    End If
                End SyncLock
            Catch ex As Exception
                Logger.Warn(ex, String.Format("加载失败的主页内容：{0}{1}", vbCrLf, Content), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                Me.OnLoadContentFailed(ex)
            End Try
        End Sub

        Private Sub OnLoadContentFailed(ex As Exception)
            If ModBase.ModeDebug OrElse Settings.[Get](Of Integer)("UiCustomType", Nothing) = 1 Then
                Logger.Warn(ex, "加载主页失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
                If ModMain.OurMsgBox(If((TypeOf ex Is UnauthorizedAccessException), ex.Message, String.Format("主页内容编写有误，请根据下列错误信息进行检查：{0}{1}", vbCrLf, ex.GetDisplay(False))), "加载主页失败", "重试", "取消", "", False, True, False, Nothing, Nothing, Nothing) = 1 Then
                    Me.ForceRefresh()
                    Return
                End If
            Else
                Logger.[Error](ex, "加载主页失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLaunchRight.xaml.vb")
            End If
        End Sub

        Sub DispatcherUnhandledException(sender As Object, e As DispatcherUnhandledExceptionEventArgs) Implements PCL.IDispatcherUnhandledException.DispatcherUnhandledException
            If TypeOf e.Exception Is XamlParseException Then
                e.Handled = True
                Me.LoadContent(Nothing)
                Me.OnLoadContentFailed(e.Exception)
            End If
        End Sub

        Friend Overridable Property PanBack As OurScrollViewer

        Friend Overridable Property PanMain As StackPanel

        Friend Overridable Property PanHint As OurHint

        Friend Overridable Property PanCustom As StackPanel

        Friend Overridable Property PanLog As OurCard

        Friend Overridable Property LabLog As OurTextBox

        Private RefreshLock As Object

        <TupleElementNames(New String() { "Address", "ShouldRefresh" })>
        Private OnlineLoader As ModLoader.LoaderTask(Of ValueTuple(Of String, Boolean), Integer)

        Private LoadedContentHash As ULong?

        Private LoadContentLock As Object
    End Class
End Namespace
