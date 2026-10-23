Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Globalization
Imports System.Linq
Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageLinkMain
        Inherits MyPageRight

        Public Sub New()
            AddHandler MyBase.Initialized, Sub(a0 As Object, a1 As EventArgs)
                Me.LoaderInit()
            End Sub
            AddHandler MyBase.PageEnter, AddressOf Me.UpdatePanelVisibility
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.StartTimerThread()
            End Sub
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.Update()
            End Sub
            Me.FailReason = "准备初始化"
            Me.PathEasyTier = Paths.AppDataThenName + "EasyTier\"
            Me.ClientAddress = Nothing
            Me.DiscoverNodeID = -1
            Me.LinkLoader = New ModLoader.LoaderCombo(Of Integer)("联机", New ModLoader.LoaderBase() { New ModLoader.LoaderTask(Of Integer, Integer)("获取配置", AddressOf Me.InitConfig, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 8.0 }, New ModLoader.LoaderTask(Of Integer, List(Of ModNet.NetFile))("准备下载联机模块", AddressOf Me.InitPrepareDownload, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 2.0 }, New ModNet.LoaderDownload("下载联机模块", New List(Of ModNet.NetFile)()) With { .ProgressWeight = 40.0 }, New ModLoader.LoaderTask(Of Integer, Integer)("启动联机模块", AddressOf Me.InitLaunch, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 50.0 } })
            Me.LogHistory = New Queue(Of String)(10)
            Me.Peers = Nothing
            Me.NATType = PageLinkMain.NATTypes.Pending
            Me.RefreshPeerLoader = New ModLoader.LoaderTask(Of Integer, List(Of PageLinkMain.Peer))("EasyTier CLI", Sub(a0 As ModLoader.LoaderTask(Of Integer, List(Of PageLinkMain.Peer)))
                Me.RefreshPeer()
            End Sub, Nothing, ThreadPriority.Normal)
            Me.FailCount = 0
            Me.InitializeComponent()
        End Sub

        Private Sub ChangeState(NewState As PageLinkMain.LinkStates)
            If PageLinkMain.LinkState <> NewState Then
                Dim OldState As PageLinkMain.LinkStates = PageLinkMain.LinkState
                PageLinkMain.LinkState = NewState
                Logger.Info(String.Format("主状态由 {0} 变更为 {1}", OldState, NewState), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                ModBase.RunInUi(Sub()
                    Select Case NewState
                        Case PageLinkMain.LinkStates.Waiting
                            Me.SwitchToWaiting(OldState)
                            Return
                        Case PageLinkMain.LinkStates.Loading
                            Me.SwitchToLoading(OldState)
                            Return
                        Case PageLinkMain.LinkStates.Failed
                            Me.SwitchToFailed(OldState)
                            Return
                        Case PageLinkMain.LinkStates.Finished
                            Me.SwitchToFinished(OldState)
                            Return
                        Case Else
                            Return
                    End Select
                End Sub, False)
            End If
        End Sub

        Private Sub LoaderInit()
            MyBase.PageLoaderInit(Of Integer)(Me.Load, Me.PanLoad, Me.PanContent, Me.PanAlways, Me.LinkLoader, Nothing, Nothing, False)
            AddHandler Me.LinkLoader.OnStateChangedUi, Sub(Loader As ModLoader.LoaderBase, NewState As ModBase.LoadState, OldState As ModBase.LoadState)
                If NewState = ModBase.LoadState.Finished Then
                    Me.ChangeState(PageLinkMain.LinkStates.Finished)
                    Return
                End If
                If NewState <> ModBase.LoadState.Failed Then
                    Return
                End If
                ModMain.Telemetry("联机失败", New String() { "Exception", ModMinecraft.FilterUserName(Loader.[Error].GetDisplay(True), "*"c) })
                Me.ChangeState(PageLinkMain.LinkStates.Failed)
            End Sub
        End Sub

        Private Sub UpdatePanelVisibility()
            ModMain.FrmLinkMain.PanSelect.Visibility = If((PageLinkMain.LinkState = PageLinkMain.LinkStates.Waiting), Visibility.Visible, Visibility.Collapsed)
            ModMain.FrmLinkMain.PanFinish.Visibility = If((PageLinkMain.LinkState = PageLinkMain.LinkStates.Finished), Visibility.Visible, Visibility.Collapsed)
        End Sub

        Private Sub SwitchToWaiting(OldState As PageLinkMain.LinkStates)
            Me.LinkLoader.Cancel()
            Dim lockState As Object = Me.LinkLoader.LockState
            ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
            SyncLock lockState
                Me.LinkLoader.State = ModBase.LoadState.Waiting
            End SyncLock
            PageLinkMain.ProcessStop()
            If OldState <> PageLinkMain.LinkStates.Finished Then
                Me.UpdatePanelVisibility()
            End If
            MyBase.PageOnContentExit()
        End Sub

        Private Sub Create_MouseLeftButtonUp()
            Dim text As String = ModMain.MyMsgBoxInput("输入端口", String.Format("在单人游戏的暂停菜单选择 {0}对局域网开放{1}，然后输入端口数字。{2}甚至可以输入其他游戏的端口……嗯……", ModBase.vbLQ, ModBase.vbRQ, vbCrLf), Nothing, New Collection(Of Validate)() From { New ValidateInteger(1024, 65535) }, "端口号", "确定", "取消", False)
            If text IsNot Nothing Then
                Me.IsServerSide = True
                Me.ServerPort = Conversions.ToInteger(text)
                Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_0(Of String)
                If PageLinkMain._Closure$__.$I9-0 IsNot Nothing Then
                    vb$AnonymousDelegate_ = PageLinkMain._Closure$__.$I9-0
                Else
                    ' The following expression was wrapped in a checked-expression
                    Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_0(Of String) = Function()
                        Dim text2 As String = ""
                        Dim num As Integer = 1
                        Do
                            text2 += ModBase.RandomInteger(0, 35).ConvertRadix(36)
                            num += 1
                        Loop While num <= 5
                        Return text2.Replace("O", "0").Replace("I", "1")
                    End Function
                    vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                    PageLinkMain._Closure$__.$I9-0 = vb$AnonymousDelegate_2
                End If
                Dim vb$AnonymousDelegate_3 As VB$AnonymousDelegate_0(Of String) = vb$AnonymousDelegate_
                Me.NetworkName = String.Format("P{0}-{1}", Me.ServerPort.ConvertRadix(16).PadLeft(4, "0"c), vb$AnonymousDelegate_3())
                Me.NetworkSecret = vb$AnonymousDelegate_3()
                Me.DiscoverNodeID = -1
                Logger.Info(String.Format("尝试创建房间，网络名 {0}，网络密码 {1}，端口 {2}", Me.NetworkName, Me.NetworkSecret, Me.ServerPort), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                Me.ChangeState(PageLinkMain.LinkStates.Loading)
            End If
        End Sub

        Public Shared Sub Join()
            Dim text As String = ModMain.MyMsgBoxInput("输入邀请码", "输入房主发给你的邀请码。", Nothing, Nothing, If(String.IsNullOrEmpty(PageLinkMain.LastCode), "", "使用上一次的邀请码"), "确定", "取消", False)
            If Not String.IsNullOrEmpty(PageLinkMain.LastCode) AndAlso text IsNot Nothing AndAlso Operators.CompareString(text, "", False) = 0 Then
                text = PageLinkMain.LastCode
            End If
            If text IsNot Nothing Then
                PageLinkMain.Join(text)
            End If
        End Sub

        Public Sub JoinInternal(Code As String)
            If Code IsNot Nothing Then
                Dim text As String = PageLinkMain.ValidateCodeFormat(Code)
                If text IsNot Nothing Then
                    ModMain.Hint(text, ModMain.HintType.Red, True)
                    Return
                End If
                Code = PageLinkMain.FixCodeFormat(Code)
                Logger.Info(String.Format("实际使用的邀请码：{0}", Code), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                Me.IsServerSide = False
                Me.ServerPort = Conversions.ToInteger(Code.Substring(1, 4).Upper().ConvertRadix(16, 10))
                Me.NetworkName = Code.Substring(0, 11)
                Me.NetworkSecret = Code.Substring(12, 5)
                If Operators.CompareString(Code.Substring(20, 3), "000", False) = 0 Then
                    Me.DiscoverNodeID = -2
                Else
                    Me.DiscoverNodeID = Conversions.ToInteger(Code.Substring(20, 3).Upper().ConvertRadix(16, 10))
                End If
                Logger.Info(String.Format("尝试加入房间，网络名 {0}，网络密码 {1}，端口 {2}，发现节点 {3}", New Object() { Me.NetworkName, Me.NetworkSecret, Me.ServerPort, Me.DiscoverNodeID }), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                PageLinkMain.LastCode = Code
                Me.ChangeState(PageLinkMain.LinkStates.Loading)
            End If
        End Sub

        Public Shared Function ValidateCodeFormat(Code As String) As String
            Dim text As String
            If Code Is Nothing Then
                text = "邀请码为空！"
            Else
                Code = PageLinkMain.FixCodeFormat(Code)
                If Code.Length < 14 OrElse Code(0) <> "P"c OrElse Code(5) <> "-"c OrElse Code(11) <> "-"c Then
                    If Code.StartsWithF("U/", False) Then
                        text = "请让房主使用 PCL 创建房间！"
                    ElseIf Code.Length = 10 Then
                        text = "请让房主使用非社区版的 PCL 创建房间！"
                    Else
                        text = "邀请码有误，请让房主使用 PCL 创建房间！"
                    End If
                ElseIf Code.Length >= 23 AndAlso Code(17) = "-"c AndAlso ModBase.Val(Code.Substring(18, 2)) > 2.0 Then
                    text = "你的 PCL 版本太老了，请在更新 PCL 之后再联机！"
                Else
                    text = Nothing
                End If
            End If
            Return text
        End Function

        Private Shared Function FixCodeFormat(Code As String) As String
            Code = Code.Between("【", "】", False).Between("[", "]", False)
            Code = Code.Upper().Replace("O", "0").Replace("I", "1")
            If Code.Length >= 17 AndAlso (Code.Length < 23 OrElse (Code.Length >= 18 AndAlso Code(17) <> "-"c)) Then
                Code = Code.Substring(0, 17) + "-0105E"
            End If
            Return Code
        End Function

        Public Shared Sub Join(Code As String)
            If PageLinkMain.LinkState <> PageLinkMain.LinkStates.Waiting Then
                ModMain.Hint("你已经在联机房间中了！", ModMain.HintType.Red, True)
                Return
            End If
            If ModMain.FrmMain.PageCurrent Is FormMain.PageType.Link Then
                ModMain.FrmLinkMain.JoinInternal(Code)
                Return
            End If
            PageLinkMain.AutoJoinCode = Code
            ModMain.FrmMain.PageChange(FormMain.PageType.Link, FormMain.PageSubType.[Default])
        End Sub

        Private Sub PanSelectJoin_Loaded()
            If PageLinkMain.AutoJoinCode IsNot Nothing Then
                If PageLinkMain.LinkState = PageLinkMain.LinkStates.Waiting Then
                    Me.JoinInternal(PageLinkMain.AutoJoinCode)
                End If
                PageLinkMain.AutoJoinCode = Nothing
            End If
        End Sub

        Private Sub SwitchToLoading(OldState As PageLinkMain.LinkStates)
            Me.Peers = Nothing
            Me.FailCount = 0
            Me.LinkLoader.Start(Nothing, True)
            Me.UpdateProgressBar(0.0)
            Me.LabLoadTitle.Text = If(Me.IsServerSide, "创建房间中", "加入房间中")
            Me.UpdateLoadingPage("正在初始化……", "准备初始化")
        End Sub

        Private Sub SwitchToFailed(OldState As PageLinkMain.LinkStates)
            Me.LinkLoader.Cancel()
            PageLinkMain.ProcessStop()
            Me.UpdateProgressBar(1.0)
            Me.LabLoadTitle.Text = Me.FailReason
            Dim text As String = Me.LinkLoader.[Error].GetDisplay(False)
            Dim ex As Exception = Me.LinkLoader.[Error]
            If ex.Message.StartsWithF("$", False) Then
                text = ex.Message
            End If
            While ex.InnerException IsNot Nothing
                ex = ex.InnerException
                If ex.Message.StartsWithF("$", False) Then
                    text = ex.Message
                End If
            End While
            Me.LabLoadDesc.Text = If(text.StartsWithF("$", False), text.TrimStart(New Char() { "$"c }), Me.LinkLoader.[Error].GetDisplay(True))
            Logger.Warn(Me.LinkLoader.[Error], Me.LabLoadTitle.Text, LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
        End Sub

        Private Sub UpdateLoadingPage(Title As String, FailBrief As String)
            If Operators.CompareString(Me.FailReason, FailBrief + "失败", False) <> 0 Then
                Me.FailReason = FailBrief + "失败"
                Logger.Info(String.Format("开始步骤：{0}", Title), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                ModBase.RunInUiWait(Sub()
                    If ModMain.FrmLinkMain IsNot Nothing AndAlso ModMain.FrmLinkMain.LabLoadDesc.IsLoaded Then
                        ModMain.FrmLinkMain.LabLoadDesc.Text = Title
                    End If
                End Sub)
            End If
        End Sub

        Private Sub BtnLoadCancel_Click()
            Me.ChangeState(PageLinkMain.LinkStates.Waiting)
        End Sub

        Private Sub CardLoad_MouseLeftButtonUp()
            Me.ChangeState(PageLinkMain.LinkStates.Loading)
        End Sub

        Private Sub UpdateProgressBar(Optional Value As Double = -1.0)
            If Value = -1.0 Then
                Value = Me.LinkLoader.Progress
            End If
            Dim value As Double = Me.ColumnProgressA.Width.Value
            If Math.Round(Value - value, 3) <> 0.0 Then
                If value <= Value Then
                    Dim num As Double = If((Value = 1.0), 1.0, (value + Math.Pow(Value - value, 2.0) * 0.5))
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaGridLengthWidth(Me.ColumnProgressA, num - Me.ColumnProgressA.Width.Value, 200, 0, Nothing, False), ModAnimation.AaGridLengthWidth(Me.ColumnProgressB, 1.0 - num - Me.ColumnProgressB.Width.Value, 200, 0, Nothing, False) }, "Link Progress", False)
                    Return
                End If
                Me.ColumnProgressA.Width = New GridLength(Value, GridUnitType.Star)
                Me.ColumnProgressB.Width = New GridLength(1.0 - Value, GridUnitType.Star)
                ModAnimation.AniStop("Link Progress")
            End If
        End Sub

        Private Sub CardLoad_SizeChanged()
            Me.RectProgressClip.Rect = New Rect(0.0, 0.0, Me.CardLoad.ActualWidth, 12.0)
        End Sub

        Private Sub SwitchToFinished(OldState As PageLinkMain.LinkStates)
            Me.UpdatePanelVisibility()
            If Me.IsServerSide Then
                Me.LabFinishTitle.Text = "已创建房间"
                Me.LabFinishDesc.Text = String.Format("把邀请码发给朋友，让大家加入房间吧！{0}邀请码：{1}", vbCrLf, Me.GetInviteCode())
                Me.BtnFinishExit.Text = "关闭"
                Me.BtnFinishPing.ToolTip = "网络延迟"
                Me.BtnFinishCopy.Visibility = Visibility.Visible
                Me.Copy()
                Me.BtnFinishPort.Visibility = Visibility.Visible
                Me.LabFinishPort.Visibility = Visibility.Visible
                Me.BtnFinishIp.Visibility = Visibility.Collapsed
                Me.LabFinishIp.Visibility = Visibility.Collapsed
                Me.LabFinishPort.Text = Conversions.ToString(Me.ServerPort)
            Else
                Me.LabFinishTitle.Text = "已加入房间"
                Me.LabFinishDesc.Text = String.Format("在多人游戏页面的最下方就能找到联机房间！{0}注意：使用离线登录时不要手动输入 IP！", vbCrLf)
                Me.BtnFinishExit.Text = "离开"
                Me.BtnFinishPing.ToolTip = "与房主的延迟"
                Me.BtnFinishCopy.Visibility = Visibility.Collapsed
                Me.BtnFinishPort.Visibility = Visibility.Collapsed
                Me.LabFinishPort.Visibility = Visibility.Collapsed
                Me.LabFinishIp.Text = Me.ClientAddress
                Me.BtnFinishIp.Visibility = Visibility.Visible
                Me.LabFinishIp.Visibility = Visibility.Visible
            End If
            Me.Update()
        End Sub

        Private Sub BtnFinishExit_Click()
            Me.TryExit(False, False)
        End Sub

        Public Function TryExit(Silent As Boolean, Closing As Boolean) As Boolean
            Dim flag As Boolean
            If PageLinkMain.LinkState = PageLinkMain.LinkStates.Waiting OrElse PageLinkMain.LinkState = PageLinkMain.LinkStates.Failed Then
                flag = False
            Else
                If Not Silent Then
                    If Me.IsServerSide AndAlso Me.PeopleCount > 1 Then
                        If ModMain.MyMsgBox("你确定要关闭联机房间吗？" & vbCrLf & "所有玩家都需要重新输入邀请码才可加入游戏！", "退出联机", "确定", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 2 Then
                            Return True
                        End If
                    ElseIf Closing AndAlso ModMain.MyMsgBox(If(Me.IsServerSide, "你确定要关闭联机房间吗？", "你确定要离开联机房间吗？"), "退出联机", "确定", "取消", "", False, True, False, Nothing, Nothing, Nothing) = 2 Then
                        Return True
                    End If
                End If
                Me.ChangeState(PageLinkMain.LinkStates.Waiting)
                flag = False
            End If
            Return flag
        End Function

        Private Sub Copy()
            Dim text As String = String.Format("在 PCL 启动器中输入邀请码【{0}】，即可加入联机房间！", Me.GetInviteCode())
            ModBase.ClipboardSet(text, False)
            Settings.[Set]("LinkLastAutoJoinInviteCode", text, Nothing)
            ModMain.Hint("已复制邀请码！", ModMain.HintType.Green, True)
        End Sub

        Private Function GetInviteCode() As String
            Return String.Format("{0}-{1}-{2}{3}", New Object() { Me.NetworkName, Me.NetworkSecret, 2.ToString().PadLeft(2, "0"c), If((Me.DiscoverNodeID = -1), 0, Me.DiscoverNodeID).ConvertRadix(16).PadLeft(3, "0"c) })
        End Function

        Private Sub BtnFinishIp_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
            ModBase.ClipboardSet(Me.ClientAddress, False)
            ModMain.Hint("已复制服务器地址！", ModMain.HintType.Green, True)
        End Sub

        Private Overridable Property LinkLoader As ModLoader.LoaderCombo(Of Integer)

        Private Sub InitConfig(Task As ModLoader.LoaderTask(Of Integer, Integer))
            Me.UpdateLoadingPage("正在联网获取配置……", "联网获取配置")
            ModSecret.ServerLoader.WaitForExit(Nothing, Task, False, Nothing, Nothing)
            If ModSecret.ServerConfig Is Nothing Then
                Throw New Exception("无法从服务器获取配置")
            End If
            Dim jtoken As JToken = ModSecret.ServerConfig("Link")
            Dim text As String
            If jtoken Is Nothing Then
                text = Nothing
            Else
                Dim jtoken2 As JToken = jtoken("DisableReason2")
                text = If((jtoken2 IsNot Nothing), jtoken2.ToString(), Nothing)
            End If
            Dim text2 As String = text
            If Not String.IsNullOrEmpty(text2) Then
                Throw New Exception("$" + text2)
            End If
            If CType(ModSecret.ServerConfig("Link"), JObject).ContainsKey("MinVersionCode") AndAlso 414 < ModSecret.ServerConfig("Link")("MinVersionCode").ToObject(Of Integer)() Then
                Throw New Exception("$你的 PCL 版本太老了，请在更新 PCL 之后再联机！")
            End If
        End Sub

        Private Sub InitPrepareDownload(Task As ModLoader.LoaderTask(Of Integer, List(Of ModNet.NetFile)))
            Me.UpdateLoadingPage("正在获取 CPU 架构……", "获取 CPU 架构")
            Dim Architecture As String = Conversions.ToString(CInt(GetType(String).Assembly.GetName().ProcessorArchitecture))
            Dim $VB$Local_Architecture As String = Architecture
            If Operators.CompareString($VB$Local_Architecture, Conversions.ToString(2), False) = 0 Then
                Architecture = "i686"
            ElseIf Operators.CompareString($VB$Local_Architecture, Conversions.ToString(4), False) = 0 Then
                Architecture = "x86_64"
            ElseIf Operators.CompareString($VB$Local_Architecture, Conversions.ToString(5), False) = 0 Then
                Architecture = "arm64"
            Else
                Logger.Warn(String.Format("CPU 是不支持的 {0} 架构，这可能会导致联机模块无法启动！", Architecture), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                Architecture = "arm64"
            End If
            Logger.Info(String.Format("CPU 架构：{0}", Architecture), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
            ModMain.Telemetry("联机开始", New String(-1) {})
            Me.UpdateLoadingPage("正在检查联机模块版本……", "检查联机模块版本")
            If Not FileUtils.Exists(Me.PathEasyTier + "联机模块 CLI.exe") OrElse Not FileUtils.Exists(Me.PathEasyTier + "联机模块.exe") OrElse Not FileUtils.Exists(Me.PathEasyTier + "Packet.dll") Then
                Settings.[Set]("LinkEasyTierVersion", -1, Nothing)
            End If
            Dim num As Integer = Settings.[Get](Of Integer)("LinkEasyTierVersion", Nothing)
            Me.ServerVersion = CInt(ModSecret.ServerConfig("Link")("EasyTierVersion"))
            Dim list As List(Of ModNet.NetFile) = New List(Of ModNet.NetFile)()
            Logger.Info(String.Format("EasyTier 本地版本：{0}，需求版本：{1}", num, Me.ServerVersion), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
            If num < Me.ServerVersion Then
                list.Add(New ModNet.NetFile(ModSecret.ServerConfig("Link")("Downloads").[Select](Function(UrlEntry As JToken) UrlEntry.ToString().Replace("{arch}", Architecture)), Me.PathEasyTier + "EasyTier.zip", New FileChecker() With { .MinSize = 2097152L }, False))
            End If
            Me.UpdateLoadingPage("正在下载联机模块……", "下载联机模块")
            Task.Output = list
        End Sub

        Private Sub InitLaunch(Task As ModLoader.LoaderTask(Of Integer, Integer))
            Me.UpdateLoadingPage("正在解压联机模块……", "解压联机模块")
            If FileUtils.Exists(Me.PathEasyTier + "EasyTier.zip") Then
                DirectoryUtils.Copy(DirectoryUtils.EnumerateDirectories(ModMain.RequestTaskTempFolder(False), True, "*").First(), Me.PathEasyTier)
                FileUtils.Move(Me.PathEasyTier + "easytier-core.exe", Me.PathEasyTier + "联机模块.exe", False)
                FileUtils.Move(Me.PathEasyTier + "easytier-cli.exe", Me.PathEasyTier + "联机模块 CLI.exe", False)
                FileUtils.Delete(Me.PathEasyTier + "EasyTier.zip", False)
                Settings.[Set]("LinkEasyTierVersion", Me.ServerVersion, Nothing)
            End If
            Task.Progress = 0.07
            Me.UpdateLoadingPage("正在获取节点列表……", "获取节点列表")
            Dim text As String = Settings.[Get](Of String)("LinkCustomPeer", Nothing)
            Dim list As List(Of String)
            If String.IsNullOrWhiteSpace(text) Then
                If Me.DiscoverNodeID = -2 AndAlso Not Me.IsServerSide Then
                    Me.Panic("未填写自定义节点设置", String.Format("$你必须在 {0}自定义节点{1} 设置中填写与房主相同的内容，{2}才能进入该房间！", ModBase.vbLQ, ModBase.vbRQ, vbCrLf))
                    Return
                End If
                list = Me.GetTargetPeers()
            Else
                If Me.DiscoverNodeID <> -2 AndAlso Not Me.IsServerSide Then
                    ModMain.Hint("房主可能没有使用自定义节点设置，请确认你们的自定义节点设置是否一致！", ModMain.HintType.Blue, True)
                End If
                Dim enumerable As IEnumerable(Of String) = text.Split("，,".ToCharArray())
                Dim func As Func(Of String, String)
                If PageLinkMain._Closure$__.$I48-0 IsNot Nothing Then
                    func = PageLinkMain._Closure$__.$I48-0
                Else
                    Dim func2 As Func(Of String, String) = Function(p As String) p.Trim()
                    func = func2
                    PageLinkMain._Closure$__.$I48-0 = func2
                End If
                Dim enumerable2 As IEnumerable(Of String) = enumerable.[Select](func)
                Dim func3 As Func(Of String, Boolean)
                If PageLinkMain._Closure$__.$I48-1 IsNot Nothing Then
                    func3 = PageLinkMain._Closure$__.$I48-1
                Else
                    Dim func4 As Func(Of String, Boolean) = Function(p As String) Not String.IsNullOrEmpty(p)
                    func3 = func4
                    PageLinkMain._Closure$__.$I48-1 = func4
                End If
                list = enumerable2.Where(func3).ToList()
                Logger.Info(String.Format("使用自定义节点：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
            End If
            Task.Progress = 0.13
            Me.UpdateLoadingPage("正在启动联机模块……", "启动联机模块")
            Dim list2 As List(Of Integer) = ModNet.FindFreePorts(5, New Integer() { Me.ServerPort })
            Me.ClientPort = list2(0)
            Me.RPCPort = list2(1)
            Dim num As Integer = list2(2)
            Dim text2 As String = CStr(ModSecret.ServerConfig("Link")("Argument"))
            text2 += String.Format(" --network-name={0} --network-secret={1} --listeners {2} --rpc-portal {3} --private-mode true", New Object() { Me.NetworkName, Me.NetworkSecret, num, Me.RPCPort })
            Dim text3 As String = If(Me.IsServerSide, "Server-", "Client-") + ModBase.Identify.GetStableHashCode().ConvertRadix(36).TrimStart(New Char() { "-"c }).EnsureLength("0"c, 6)
            If Me.IsServerSide Then
                text2 += String.Format(" -i 10.114.114.114 --hostname={0} --tcp-whitelist={1} --udp-whitelist={2}", text3, Me.ServerPort, Me.ServerPort)
            Else
                text2 += String.Format(" -d --hostname={0} --tcp-whitelist=0 --udp-whitelist=0", text3)
                Me.ClientAddress = String.Format("localhost:{0}", Me.ClientPort)
                text2 += String.Format(" --port-forward tcp://[{0}]:{1}/10.114.114.114:{2}", IPAddress.IPv6Loopback, Me.ClientPort, Me.ServerPort)
                text2 += String.Format(" --port-forward udp://[{0}]:{1}/10.114.114.114:{2}", IPAddress.IPv6Loopback, Me.ClientPort, Me.ServerPort)
                text2 += String.Format(" --port-forward tcp://{0}:{1}/10.114.114.114:{2}", IPAddress.Loopback, Me.ClientPort, Me.ServerPort)
                text2 += String.Format(" --port-forward udp://{0}:{1}/10.114.114.114:{2}", IPAddress.Loopback, Me.ClientPort, Me.ServerPort)
            End If
            Try
                For Each text4 As String In list
                    text2 += String.Format(" -p=""{0}""", text4)
                Next
            Finally
                Dim enumerator As List(Of String).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            If Settings.[Get](Of Integer)("LinkLatencyMode", Nothing) = 1 Then
                text2 += " --latency-first"
            End If
            Me.ProcessStart(text2)
            Task.Progress = 0.15
            Dim num2 As Integer = 0
            Dim peerPing As Double
            While True
                Thread.Sleep(500)
                Me.CheckCrash()
                Me.RefreshPeerLoader.WaitForExit(Nothing, Nothing, True, Nothing, Nothing)
                peerPing = Me.GetPeerPing()
                If peerPing > 0.0 Then
                    Exit For
                End If
                Dim progress As Double = Task.Progress
                Dim num3 As Integer
                If Me.Peers IsNot Nothing Then
                    Dim peers As IEnumerable(Of PageLinkMain.Peer) = Me.Peers
                    Dim func5 As Func(Of PageLinkMain.Peer, Boolean)
                    If PageLinkMain._Closure$__.$I48-2 IsNot Nothing Then
                        func5 = PageLinkMain._Closure$__.$I48-2
                    Else
                        Dim func6 As Func(Of PageLinkMain.Peer, Boolean) = Function(p As PageLinkMain.Peer) p.Ping > 0.0
                        func5 = func6
                        PageLinkMain._Closure$__.$I48-2 = func6
                    End If
                    num3 = peers.Where(func5).Count()
                Else
                    num3 = -1
                End If
                Dim num4 As Integer = num3
                If num4 <> -1 Then
                    If num4 <> 0 Then
                        Me.UpdateLoadingPage("正在连接到房主……", "连接房主")
                        Task.Progress = (Task.Progress + 0.02).Clamp(Math.Min(0.45 + CDbl(num4) * 0.05, 0.65), 0.95)
                    Else
                        Me.UpdateLoadingPage("正在连接到节点……", "连接节点")
                        Task.Progress = (Task.Progress + 0.02).Clamp(If(Me.IsServerSide, 0.5, 0.3), If(Me.IsServerSide, 0.95, 0.5))
                    End If
                Else
                    Task.Progress = (Task.Progress + 0.02).Clamp(0.15, 0.25)
                End If
                If progress <> Task.Progress Then
                    num2 = 0
                ElseIf num2 < 60 Then
                    num2 += 1
                ElseIf num4 <> -1 Then
                    If num4 <> 0 Then
                        Me.Panic("无法连接到房主", String.Format("可能的原因：{0}- 你或者房主的网络环境不佳{1}- 房主已关闭房间{2}- 邀请码输错了", vbCrLf, vbCrLf, vbCrLf))
                    Else
                        Me.Panic("无法连接到节点", "请检查你的网络环境是否良好。")
                    End If
                Else
                    Me.Panic("无法启动联机模块", "近期日志：" & vbCrLf + Me.LogHistory.Join(vbCrLf))
                End If
                If Task.IsCanceled Then
                    GoTo IL_06E7
                End If
            End While
            Logger.Info(String.Format("已与目标建立连接，当前 Ping 为 {0:0.0}ms", peerPing), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
            ModMain.Telemetry("联机成功", New String() { "Server", Conversions.ToString(Me.IsServerSide), "NAT", Conversions.ToString(CInt(Me.NATType)) })
            IL_06E7:
            If Task.IsCanceled Then
                Throw New OperationCanceledException()
            End If
            If Not Me.IsServerSide Then
                Me.UpdateLoadingPage("连接优化中……", "优化连接")
                Task.Progress = 0.999
                Dim num5 As Integer = 1
                While True
                    Dim targetPeer As PageLinkMain.Peer = Me.GetTargetPeer()
                    If targetPeer IsNot Nothing AndAlso Not targetPeer.Relay AndAlso targetPeer.Ping < 100.0 Then
                        Return
                    End If
                    If Task.IsCanceled Then
                        Exit For
                    End If
                    Thread.Sleep(100)
                    num5 += 1
                    If num5 > 50 Then
                        Return
                    End If
                End While
                Throw New OperationCanceledException()
            End If
        End Sub

        Private Function GetTargetPeers() As List(Of String)
            Dim list As List(Of String) = New List(Of String)()
            Dim num As Integer = -1
            Dim num2 As Integer = ModSecret.ServerConfig("Link")("DiscoverPeerId").ToObject(Of Integer)()
            Dim text As String = ModSecret.ServerConfig("Link")("DiscoverPeer").ToString()
            Try
                Dim flag As Boolean = False
                Dim jobject As JObject
                Try
                    jobject = CType(ModNet.NetRequestByClient("https://uptime.easytier.cn/api/nodes?page=1&per_page=1000", Nothing, Nothing, Nothing, "*/*", 25000, Nothing, True, Nothing, False, True).DeserializeJson(), JObject)
                Catch ex As Exception
                    Logger.Warn(ex, "从源站获取节点列表失败，将使用 CDN 缓存", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                    jobject = CType(ModNet.NetRequestByClientRetry("https://easytier.meloong.com/?page=1&per_page=1000", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False).DeserializeJson(), JObject)
                    flag = True
                End Try
                Dim list2 As List(Of JObject) = New List(Of JObject)()
                Dim enumerable As IEnumerable(Of JToken) = ModSecret.ServerConfig("Link")("PeersBlackList")
                Dim func As Func(Of JToken, String)
                If PageLinkMain._Closure$__.$I49-0 IsNot Nothing Then
                    func = PageLinkMain._Closure$__.$I49-0
                Else
                    Dim func2 As Func(Of JToken, String) = Function(p As JToken) p.ToString()
                    func = func2
                    PageLinkMain._Closure$__.$I49-0 = func2
                End If
                Dim list3 As List(Of String) = enumerable.[Select](func).ToList()
                Try
                    For Each jtoken As JToken In jobject("data")("items")
                        Dim jobject2 As JObject = CType(jtoken, JObject)
                        Dim num3 As Integer = jobject2("id").ToObject(Of Integer)()
                        If jobject2("is_active").ToObject(Of Boolean)() AndAlso jobject2("is_approved").ToObject(Of Boolean)() Then
                            If num3 <> Me.DiscoverNodeID Then
                                If num3 = num2 OrElse list3.Contains(jobject2("address").ToString()) OrElse (jobject2("usage_percentage").ToObject(Of Double)() = 0.0 AndAlso ModBase.RandomInteger(1, 100) <> 1) Then
                                    Continue For
                                End If
                                Dim enumerable2 As IEnumerable(Of JToken) = jobject2("tags")
                                Dim func3 As Func(Of JToken, String)
                                If PageLinkMain._Closure$__.$I49-1 IsNot Nothing Then
                                    func3 = PageLinkMain._Closure$__.$I49-1
                                Else
                                    Dim func4 As Func(Of JToken, String) = Function(t As JToken) t.ToString()
                                    func3 = func4
                                    PageLinkMain._Closure$__.$I49-1 = func4
                                End If
                                Dim list4 As List(Of String) = enumerable2.[Select](func3).ToList()
                                If Not list4.Contains("国内") OrElse Not list4.Contains("MC中继") Then
                                    Continue For
                                End If
                            End If
                            Dim num4 As Double = jobject2("usage_percentage").ToObject(Of Double)()
                            num4 *= 110.0 - jobject2("health_percentage_24h").ToObject(Of Double)()
                            jobject2("load") = num4
                            list2.Add(jobject2)
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of JToken)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                If Not flag Then
                    Dim enumerable3 As IEnumerable(Of JObject) = list2
                    Dim func5 As Func(Of JObject, Double)
                    If PageLinkMain._Closure$__.$I49-2 IsNot Nothing Then
                        func5 = PageLinkMain._Closure$__.$I49-2
                    Else
                        Dim func6 As Func(Of JObject, Double) = Function(n As JObject) n("load").ToObject(Of Double)()
                        func5 = func6
                        PageLinkMain._Closure$__.$I49-2 = func6
                    End If
                    list2 = enumerable3.OrderBy(func5).ToList()
                Else
                    list2 = list2.Shuffle().ToList()
                End If
                Dim jobject3 As JObject = Nothing
                If Me.DiscoverNodeID = -1 Then
                    Dim enumerable4 As IEnumerable(Of JObject) = list2
                    Dim func7 As Func(Of JObject, Boolean)
                    If PageLinkMain._Closure$__.$I49-3 IsNot Nothing Then
                        func7 = PageLinkMain._Closure$__.$I49-3
                    Else
                        Dim func8 As Func(Of JObject, Boolean) = Function(n As JObject) Not n("allow_relay").ToObject(Of Boolean)()
                        func7 = func8
                        PageLinkMain._Closure$__.$I49-3 = func8
                    End If
                    jobject3 = enumerable4.FirstOrDefault(func7)
                ElseIf Me.DiscoverNodeID > 0 Then
                    jobject3 = list2.FirstOrDefault(Function(n As JObject) n("id").ToObject(Of Integer)() = Me.DiscoverNodeID)
                    If jobject3 Is Nothing Then
                        Logger.Warn(String.Format("未找到 ID {0} 的发现节点", Me.DiscoverNodeID), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                        Me.Panic("房间已过期", "请让房主重新创建房间！")
                        Throw New OperationCanceledException()
                    End If
                End If
                If jobject3 Is Nothing Then
                    Dim jobject4 As JObject = New JObject()
                    jobject4.Add("address", text)
                    jobject4.Add("id", num2)
                    jobject3 = jobject4
                    Logger.Warn("将使用回退发现节点", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                End If
                list.Add(jobject3("address").ToString())
                num = jobject3("id").ToObject(Of Integer)()
                Logger.Info(String.Format("发现节点：{0} (ID: {1})", jobject3("address"), num), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                If ModBase.ModeDebug OrElse Not Me.IsServerSide Then
                    Dim num5 As Integer = ModSecret.ServerConfig("Link")("RandomPeer").ToObject(Of Integer)()
                    Dim enumerable5 As IEnumerable(Of JObject) = list2
                    Dim func9 As Func(Of JObject, Boolean)
                    If PageLinkMain._Closure$__.$I49-5 IsNot Nothing Then
                        func9 = PageLinkMain._Closure$__.$I49-5
                    Else
                        Dim func10 As Func(Of JObject, Boolean) = Function(n As JObject) n("allow_relay").ToObject(Of Boolean)()
                        func9 = func10
                        PageLinkMain._Closure$__.$I49-5 = func10
                    End If
                    Dim list5 As List(Of JObject) = enumerable5.Where(func9).ToList()
                    If list5.Count < num5 Then
                        Throw New Exception(String.Format("可用的中继节点数量不足，需要 {0} 个，实际 {1} 个", num5, list5.Count))
                    End If
                    Dim list6 As List(Of String) = list
                    Dim enumerable6 As IEnumerable(Of JObject) = list5.Take(num5)
                    Dim func11 As Func(Of JObject, String)
                    If PageLinkMain._Closure$__.$I49-6 IsNot Nothing Then
                        func11 = PageLinkMain._Closure$__.$I49-6
                    Else
                        Dim func12 As Func(Of JObject, String) = Function(n As JObject) n("address").ToString()
                        func11 = func12
                        PageLinkMain._Closure$__.$I49-6 = func12
                    End If
                    list6.AddRange(enumerable6.[Select](func11))
                End If
            Catch ex2 As Exception
                ex2.ThrowIfCanceled()
                Logger.[Error](ex2, "获取节点列表失败，联机质量可能受到影响", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                Dim list7 As List(Of String) = list
                Dim enumerable7 As IEnumerable(Of JToken) = ModSecret.ServerConfig("Link")("Peers")
                Dim func13 As Func(Of JToken, String)
                If PageLinkMain._Closure$__.$I49-7 IsNot Nothing Then
                    func13 = PageLinkMain._Closure$__.$I49-7
                Else
                    Dim func14 As Func(Of JToken, String) = Function(p As JToken) p.ToString()
                    func13 = func14
                    PageLinkMain._Closure$__.$I49-7 = func14
                End If
                list7.AddRange(enumerable7.[Select](func13))
                If num <= 0 Then
                    list.Add(text)
                    num = num2
                End If
            End Try
            If CType(ModSecret.ServerConfig("Link"), JObject).ContainsKey("EnableForcedPeer") Then
                list.Add("tcp://mc1.easytier.cn:55558")
                If num <= 0 Then
                    num = 94
                End If
            End If
            Dim list8 As List(Of String) = list
            Dim enumerable8 As IEnumerable(Of JToken) = ModSecret.ServerConfig("Link")("MandatoryPeers")
            Dim func15 As Func(Of JToken, String)
            If PageLinkMain._Closure$__.$I49-8 IsNot Nothing Then
                func15 = PageLinkMain._Closure$__.$I49-8
            Else
                Dim func16 As Func(Of JToken, String) = Function(p As JToken) p.ToString()
                func15 = func16
                PageLinkMain._Closure$__.$I49-8 = func16
            End If
            list8.AddRange(enumerable8.[Select](func15))
            Me.DiscoverNodeID = num
            Return list.Distinct().ToList()
        End Function

        Public Shared Sub ProcessStop()
            Try
                For Each process As Process In Process.GetProcesses()
                    Dim id As Integer = process.Id
                    Dim processCore As Process = PageLinkMain.ProcessCore
                    Dim num As Integer? = If((processCore IsNot Nothing), New Integer?(processCore.Id), Nothing)
                    If Not If((num IsNot Nothing), New Boolean?(id = num.GetValueOrDefault()), Nothing).GetValueOrDefault() AndAlso (Operators.CompareString(process.ProcessName, "联机模块", False) = 0 OrElse Operators.CompareString(process.ProcessName, "联机模块 CLI", False) = 0) Then
                        Try
                            Logger.Info(String.Format("停止残留的联机模块，PID：{0}，进程名：{1}", process.Id, process.ProcessName), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                            process.Kill()
                            process.Close()
                        Catch ex As Exception
                            Logger.Warn(ex, String.Format("结束进程失败（{0}，PID {1}）", process.ProcessName, process.Id), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                        End Try
                    End If
                Next
                If PageLinkMain.ProcessCore IsNot Nothing AndAlso Not PageLinkMain.ProcessCore.HasExited Then
                    Logger.Info(String.Format("停止所启动的联机模块，PID：{0}", PageLinkMain.ProcessCore.Id), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                    PageLinkMain.ProcessCore.Kill()
                    PageLinkMain.ProcessCore.Close()
                    PageLinkMain.ProcessCore = Nothing
                End If
                Dim processOutputHandle As AutoResetEvent = PageLinkMain.ProcessOutputHandle
                If processOutputHandle IsNot Nothing Then
                    processOutputHandle.Dispose()
                End If
                Dim processErrorHandle As AutoResetEvent = PageLinkMain.ProcessErrorHandle
                If processErrorHandle IsNot Nothing Then
                    processErrorHandle.Dispose()
                End If
            Catch ex2 As Exception
                Logger.Warn(ex2, "停止联机模块失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
            End Try
        End Sub

        Private Sub Panic(Brief As String, Detail As String)
            If Detail.Contains("failed to listen on ") Then
                Detail = String.Format("监听端口失败。{0}请点击重试，如果还是出现此错误，可以重启电脑解决。{1}{2}{3}", New Object() { vbCrLf, vbCrLf, vbCrLf, Detail })
            End If
            If PageLinkMain.LinkState = PageLinkMain.LinkStates.Loading Then
                Me.FailReason = Brief
                Me.LinkLoader.Failed(New Exception(Detail))
                Return
            End If
            ModMain.MyMsgBox(Detail, "联机出错：" + Brief, "确定", "", "", True, True, False, Nothing, Nothing, Nothing)
            Me.ChangeState(PageLinkMain.LinkStates.Waiting)
            ModMain.Telemetry("联机失败", New String() { "Exception", ModMinecraft.FilterUserName(Brief + "：" + Detail, "*"c) })
        End Sub

        Private Sub ProcessStart(Arguments As String)
            Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo() With { .FileName = Me.PathEasyTier + "联机模块.exe", .Arguments = Arguments, .UseShellExecute = False, .CreateNoWindow = True, .RedirectStandardError = True, .RedirectStandardOutput = True, .StandardOutputEncoding = Encoding.UTF8, .StandardErrorEncoding = Encoding.UTF8 }
            Logger.Info(String.Format("正在启动 EasyTier：{0}", Arguments), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
            PageLinkMain.ProcessCore = New Process() With { .StartInfo = processStartInfo }
            Dim LogLineHandler As VB$AnonymousDelegate_13(Of Object, DataReceivedEventArgs, AutoResetEvent) = Sub(sender As Object, e As DataReceivedEventArgs, Handle As AutoResetEvent)
                Try
                    If e.Data Is Nothing Then
                        Handle.[Set]()
                    Else
                        Me.ProcessLogLine(e.Data)
                    End If
                Catch ex As ObjectDisposedException
                Catch ex2 As Exception
                    Logger.Warn(ex2, "读取 EasyTier 信息失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                End Try
            End Sub
            PageLinkMain.ProcessOutputHandle = New AutoResetEvent(False)
            AddHandler PageLinkMain.ProcessCore.OutputDataReceived, Sub(sender As Object, e As DataReceivedEventArgs)
                LogLineHandler(RuntimeHelpers.GetObjectValue(sender), e, PageLinkMain.ProcessOutputHandle)
            End Sub
            PageLinkMain.ProcessErrorHandle = New AutoResetEvent(False)
            AddHandler PageLinkMain.ProcessCore.ErrorDataReceived, Sub(sender As Object, e As DataReceivedEventArgs)
                LogLineHandler(RuntimeHelpers.GetObjectValue(sender), e, PageLinkMain.ProcessErrorHandle)
            End Sub
            PageLinkMain.ProcessCore.Start()
            PageLinkMain.ProcessCore.BeginOutputReadLine()
            PageLinkMain.ProcessCore.BeginErrorReadLine()
        End Sub

        Private Sub ProcessLogLine(Line As String)
            Logger.Info(Line, LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
            Me.LogHistory.Enqueue(Line)
            If Me.LogHistory.Count >= 10 Then
                Me.LogHistory.Dequeue()
            End If
            If Line.ContainsIgnoreCase("new peer connection added") Then
                Logger.Warn(String.Format("已建立连接：{0}", If(Line.RegexSeek("(?<=remote_addr.+?"")[^""}]{3,}", RegexOptions.None), Line)), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                Me.Update()
                Return
            End If
            If Line.ContainsIgnoreCase("peer connection removed") Then
                Logger.Warn(String.Format("已断开连接：{0}", If(Line.RegexSeek("(?<=remote_addr.+?"")[^""}]{3,}", RegexOptions.None), Line)), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                Me.Update()
            End If
        End Sub

        Private Sub RefreshPeer()
            Try
                Dim text As String = ""
                If Not text.Contains("lat_ms") Then
                    Throw New Exception("CLI 调用失败：" & vbCrLf + text)
                End If
                If ModBase.GetUuid() Mod If(ModBase.ModeDebug, 23, 103) = 0 Then
                    Logger.Info(String.Format("CLI 输出抽样：{0}{1}", vbCrLf, text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                End If
                Dim list As List(Of PageLinkMain.Peer) = New List(Of PageLinkMain.Peer)()
                Try
                    For Each jtoken As JToken In CType(text.DeserializeJson(), JArray)
                        Dim jobject As JObject = CType(jtoken, JObject)
                        Try
                            Dim peer As PageLinkMain.Peer = New PageLinkMain.Peer(jobject)
                            If peer.Type = PageLinkMain.Peer.Types.Self Then
                                Me.NATType = peer.NATType
                            Else
                                list.Add(peer)
                            End If
                        Catch ex As Exception
                            Logger.Warn(ex, String.Format("错误的信息（{0}）", jobject), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                        End Try
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of JToken)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                Me.Peers = list
                Me.FailCount = 0
            Catch ex2 As Exception
                Logger.Warn(ex2, "获取节点信息失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                If PageLinkMain.LinkState = PageLinkMain.LinkStates.Finished Then
                    Dim ptr As __ByRef(Of Integer) = Me.FailCount
                    Me.FailCount = ptr + 1
                    If Me.FailCount >= 4 Then
                        Me.Panic("获取节点信息失败", ex2.Message)
                    End If
                End If
            End Try
        End Sub

        Private Function GetPeerPing() As Double
            Dim targetPeer As PageLinkMain.Peer = Me.GetTargetPeer()
            Dim num As Double
            If targetPeer Is Nothing Then
                num = 0.0
            Else
                num = targetPeer.Ping
            End If
            Return num
        End Function

        Private Function GetTargetPeer() As PageLinkMain.Peer
            Dim CS$<>8__locals1 As PageLinkMain._Closure$__66-0 = New PageLinkMain._Closure$__66-0()
            Dim peer As PageLinkMain.Peer
            If Me.Peers Is Nothing Then
                peer = Nothing
            Else
                Dim enumerable As IEnumerable(Of PageLinkMain.Peer)
                If Me.IsServerSide Then
                    Dim peers As IEnumerable(Of PageLinkMain.Peer) = Me.Peers
                    Dim func As Func(Of PageLinkMain.Peer, Boolean)
                    If PageLinkMain._Closure$__.$I66-0 IsNot Nothing Then
                        func = PageLinkMain._Closure$__.$I66-0
                    Else
                        Dim func2 As Func(Of PageLinkMain.Peer, Boolean) = Function(p As PageLinkMain.Peer) p.Ping > 0.0
                        func = func2
                        PageLinkMain._Closure$__.$I66-0 = func2
                    End If
                    enumerable = peers.Where(func)
                Else
                    Dim peers2 As IEnumerable(Of PageLinkMain.Peer) = Me.Peers
                    Dim func3 As Func(Of PageLinkMain.Peer, Boolean)
                    If PageLinkMain._Closure$__.$I66-1 IsNot Nothing Then
                        func3 = PageLinkMain._Closure$__.$I66-1
                    Else
                        Dim func4 As Func(Of PageLinkMain.Peer, Boolean) = Function(p As PageLinkMain.Peer) p.Type = PageLinkMain.Peer.Types.Server AndAlso p.Ping > 0.0
                        func3 = func4
                        PageLinkMain._Closure$__.$I66-1 = func4
                    End If
                    enumerable = peers2.Where(func3)
                End If
                If Not enumerable.Any() Then
                    peer = Nothing
                Else
                    Dim CS$<>8__locals2 As PageLinkMain._Closure$__66-0 = CS$<>8__locals1
                    Dim enumerable2 As IEnumerable(Of PageLinkMain.Peer) = enumerable
                    Dim func5 As Func(Of PageLinkMain.Peer, Double)
                    If PageLinkMain._Closure$__.$I66-2 IsNot Nothing Then
                        func5 = PageLinkMain._Closure$__.$I66-2
                    Else
                        Dim func6 As Func(Of PageLinkMain.Peer, Double) = Function(p As PageLinkMain.Peer) p.Ping
                        func5 = func6
                        PageLinkMain._Closure$__.$I66-2 = func6
                    End If
                    CS$<>8__locals2.$VB$Local_MinPing = enumerable2.Min(func5)
                    peer = enumerable.First(Function(p As PageLinkMain.Peer) p.Ping = CS$<>8__locals1.$VB$Local_MinPing)
                End If
            End If
            Return peer
        End Function

        Private ReadOnly Property PeopleCount As Integer
            Get
                Dim num As Integer
                If Me.Peers Is Nothing Then
                    num = 0
                Else
                    Dim peers As IEnumerable(Of PageLinkMain.Peer) = Me.Peers
                    Dim func As Func(Of PageLinkMain.Peer, Boolean)
                    If PageLinkMain._Closure$__.$I68-0 IsNot Nothing Then
                        func = PageLinkMain._Closure$__.$I68-0
                    Else
                        Dim func2 As Func(Of PageLinkMain.Peer, Boolean) = Function(p As PageLinkMain.Peer) p.Type <> PageLinkMain.Peer.Types.Misc
                        func = func2
                        PageLinkMain._Closure$__.$I68-0 = func2
                    End If
                    num = peers.Where(func).Count() + 1
                End If
                Return num
            End Get
        End Property

        Private Sub StartTimerThread()
            If Me.$STATIC$StartTimerThread$2001$IsTimerStarted$Init Is Nothing Then
                Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$StartTimerThread$2001$IsTimerStarted$Init, New StaticLocalInitFlag(), Nothing)
            End If
            Dim flag As Boolean = False
            Try
                Monitor.Enter(Me.$STATIC$StartTimerThread$2001$IsTimerStarted$Init, flag)
                If Me.$STATIC$StartTimerThread$2001$IsTimerStarted$Init.State = 0S Then
                    Me.$STATIC$StartTimerThread$2001$IsTimerStarted$Init.State = 2S
                    Me.$STATIC$StartTimerThread$2001$IsTimerStarted = False
                ElseIf Me.$STATIC$StartTimerThread$2001$IsTimerStarted$Init.State = 2S Then
                    Throw New IncompleteInitialization()
                End If
            Finally
                Me.$STATIC$StartTimerThread$2001$IsTimerStarted$Init.State = 1S
                If flag Then
                    Monitor.[Exit](Me.$STATIC$StartTimerThread$2001$IsTimerStarted$Init)
                End If
            End Try
            If Not Me.$STATIC$StartTimerThread$2001$IsTimerStarted Then
                ' The following expression was wrapped in a checked-expression
                ModBase.RunInNewThread(Sub()
                    Dim num As Integer = 0
                    While True
                        Try
                            Thread.Sleep(200)
                            num += 200
                            If num >= 1000 Then
                                num = 0
                                Me.Update()
                            End If
                            If PageLinkMain.LinkState = PageLinkMain.LinkStates.Loading Then
                                ModBase.RunInUi(Sub()
                                    Me.UpdateProgressBar(-1.0)
                                End Sub, False)
                            End If
                        Catch ex As Exception
                            Logger.[Error](ex, "联机模块主时钟出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                            Thread.Sleep(10000)
                        End Try
                    End While
                End Sub, "Link Timer", ThreadPriority.Normal)
            End If
        End Sub

        Private Sub Update()
            If PageLinkMain.LinkState = PageLinkMain.LinkStates.Finished Then
                Dim lockState As Object = Me.RefreshPeerLoader.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    If Me.RefreshPeerLoader.State <> ModBase.LoadState.Loading Then
                        Me.RefreshPeerLoader.Start(Nothing, True)
                    End If
                End SyncLock
                If ModMain.FrmMain.PageCurrent Is FormMain.PageType.Link Then
                    ModBase.RunInUi(Sub()
                        ModAnimation.AniStart(ModAnimation.AaRotateTransform(Me.ImgFinishLogo, 500.0, 5000, 0, Nothing, False), "Link Logo Rotation", False)
                        Dim peerPing As Double = Me.GetPeerPing()
                        Dim flag3 As Boolean = peerPing Mod 500.0 = 0.0 OrElse Operators.CompareString(String.Format("{0:0.0}", peerPing), "1.0", False) = 0 OrElse Me.FailCount > 0
                        If flag3 Then
                            Me.LabFinishPing.Text = "连接优化中"
                        Else
                            Me.LabFinishPing.Text = If((peerPing >= 10.0), String.Format("{0:0} ms", peerPing), String.Format("{0:0.0} ms", peerPing))
                        End If
                        Me.LabFinishPlayer.Text = Conversions.ToString(Me.PeopleCount) + "  人"
                        Me.HintFinish.Visibility = Visibility.Collapsed
                        If Not Me.IsServerSide AndAlso Not flag3 Then
                            Dim targetPeer As PageLinkMain.Peer = Me.GetTargetPeer()
                            If targetPeer IsNot Nothing Then
                                If targetPeer.Relay Then
                                    If Me.NATType >= PageLinkMain.NATTypes.Symmetric AndAlso targetPeer.NATType >= PageLinkMain.NATTypes.Symmetric Then
                                        Me.HintFinish.Text = "你和房主的网络环境都不太好，"
                                    ElseIf Me.NATType >= PageLinkMain.NATTypes.Symmetric Then
                                        Me.HintFinish.Text = "你的网络环境不太好，"
                                    ElseIf targetPeer.NATType >= PageLinkMain.NATTypes.Symmetric Then
                                        Me.HintFinish.Text = "房主的网络环境不太好，"
                                    Else
                                        Me.HintFinish.Text = "你或者房主的网络环境不太好，"
                                    End If
                                    If String.IsNullOrWhiteSpace(Settings.[Get](Of String)("LinkCustomPeer", Nothing)) Then
                                        Dim hintFinish As MyHint = Me.HintFinish
                                        Dim myHint As MyHint = hintFinish
                                        hintFinish.Text = myHint.Text + "正使用社区节点进行中继。"
                                    Else
                                        Dim hintFinish2 As MyHint = Me.HintFinish
                                        Dim myHint As MyHint = hintFinish2
                                        hintFinish2.Text = myHint.Text + "正通过自定义节点进行中继。"
                                    End If
                                    Me.HintFinish.Visibility = Visibility.Visible
                                    Me.HintFinish.Theme = MyHint.Themes.Yellow
                                    Return
                                End If
                                If Me.NATType < PageLinkMain.NATTypes.PortRestricted AndAlso targetPeer.NATType >= PageLinkMain.NATTypes.PortRestricted Then
                                    Dim peers2 As IEnumerable(Of PageLinkMain.Peer) = Me.Peers
                                    Dim func3 As Func(Of PageLinkMain.Peer, Boolean)
                                    If PageLinkMain._Closure$__.$I70-1 IsNot Nothing Then
                                        func3 = PageLinkMain._Closure$__.$I70-1
                                    Else
                                        Dim func4 As Func(Of PageLinkMain.Peer, Boolean) = Function(p As PageLinkMain.Peer) p.Type = PageLinkMain.Peer.Types.Client
                                        func3 = func4
                                        PageLinkMain._Closure$__.$I70-1 = func4
                                    End If
                                    Dim list As List(Of PageLinkMain.Peer) = peers2.Where(func3).ToList()
                                    If list.Any() Then
                                        Dim enumerable As IEnumerable(Of PageLinkMain.Peer) = list
                                        Dim func5 As Func(Of PageLinkMain.Peer, Boolean)
                                        If PageLinkMain._Closure$__.$I70-2 IsNot Nothing Then
                                            func5 = PageLinkMain._Closure$__.$I70-2
                                        Else
                                            Dim func6 As Func(Of PageLinkMain.Peer, Boolean) = Function(p As PageLinkMain.Peer) p.Relay
                                            func5 = func6
                                            PageLinkMain._Closure$__.$I70-2 = func6
                                        End If
                                        If Not enumerable.Any(func5) Then
                                            Dim enumerable2 As IEnumerable(Of PageLinkMain.Peer) = list
                                            Dim func7 As Func(Of PageLinkMain.Peer, Boolean)
                                            If PageLinkMain._Closure$__.$I70-3 IsNot Nothing Then
                                                func7 = PageLinkMain._Closure$__.$I70-3
                                            Else
                                                Dim func8 As Func(Of PageLinkMain.Peer, Boolean) = Function(p As PageLinkMain.Peer) p.NATType < PageLinkMain.NATTypes.PortRestricted
                                                func7 = func8
                                                PageLinkMain._Closure$__.$I70-3 = func8
                                            End If
                                            If Not enumerable2.All(func7) Then
                                                Me.HintFinish.Visibility = Visibility.Visible
                                                Me.HintFinish.Theme = MyHint.Themes.Blue
                                                Me.HintFinish.Text = "你的网络环境比房主更好！如果你来当房主，其他玩家或许能更加流畅！"
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End Sub, False)
                End If
                Me.CheckCrash()
                If Not Me.Peers.Any() Then
                    Me.Panic("网络连接已断开", "请检查你的网络环境是否正常。")
                    Return
                End If
                If Not Me.IsServerSide Then
                    Dim peers As IEnumerable(Of PageLinkMain.Peer) = Me.Peers
                    Dim func As Func(Of PageLinkMain.Peer, Boolean)
                    If PageLinkMain._Closure$__.$I70-4 IsNot Nothing Then
                        func = PageLinkMain._Closure$__.$I70-4
                    Else
                        Dim func2 As Func(Of PageLinkMain.Peer, Boolean) = Function(p As PageLinkMain.Peer) p.Type = PageLinkMain.Peer.Types.Server
                        func = func2
                        PageLinkMain._Closure$__.$I70-4 = func2
                    End If
                    If Not peers.Any(func) Then
                        ModMain.MyMsgBox("房主已离开房间！", "联机结束", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
                        Me.ChangeState(PageLinkMain.LinkStates.Waiting)
                        Return
                    End If
                End If
                If Not Me.IsServerSide Then
                    Try
                        If Me.$STATIC$Update$2001$BroadcastSocket$Init Is Nothing Then
                            Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$Update$2001$BroadcastSocket$Init, New StaticLocalInitFlag(), Nothing)
                        End If
                        Dim flag2 As Boolean = False
                        Try
                            Monitor.Enter(Me.$STATIC$Update$2001$BroadcastSocket$Init, flag2)
                            If Me.$STATIC$Update$2001$BroadcastSocket$Init.State = 0S Then
                                Me.$STATIC$Update$2001$BroadcastSocket$Init.State = 2S
                                Me.$STATIC$Update$2001$BroadcastSocket = New Socket(SocketType.Dgram, ProtocolType.Udp)
                            ElseIf Me.$STATIC$Update$2001$BroadcastSocket$Init.State = 2S Then
                                Throw New IncompleteInitialization()
                            End If
                        Finally
                            Me.$STATIC$Update$2001$BroadcastSocket$Init.State = 1S
                            If flag2 Then
                                Monitor.[Exit](Me.$STATIC$Update$2001$BroadcastSocket$Init)
                            End If
                        End Try
                        Me.$STATIC$Update$2001$BroadcastSocket.SendTo(Encoding.UTF8.GetBytes(String.Format("[MOTD]PCL 联机房间[/MOTD][AD]{0}[/AD]", Me.ClientPort)), SocketFlags.None, New IPEndPoint(IPAddress.Loopback, 4445))
                    Catch ex As Exception
                        Logger.Warn(ex, "广播联机房间端口失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLink\PageLinkMain.xaml.vb")
                    End Try
                End If
            End If
        End Sub

        Private Sub CheckCrash()
            If PageLinkMain.ProcessCore Is Nothing OrElse PageLinkMain.ProcessCore.HasExited Then
                Me.Panic("联机模块已崩溃", "近期日志：" & vbCrLf + Me.LogHistory.Join(vbCrLf))
            End If
        End Sub

        Friend Overridable Property PanLoad As Grid

        Friend Overridable Property CardLoad As MyCard
            <CompilerGenerated()>
            Get
                Return Me._CardLoad
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCard)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.CardLoad_MouseLeftButtonUp()
                End Sub
                Dim sizeChangedEventHandler As SizeChangedEventHandler = Sub(a0 As Object, a1 As SizeChangedEventArgs)
                    Me.CardLoad_SizeChanged()
                End Sub
                Dim myCard As MyCard = Me._CardLoad
                If myCard IsNot Nothing Then
                    RemoveHandler myCard.MouseLeftButtonUp, mouseButtonEventHandler
                    RemoveHandler myCard.SizeChanged, sizeChangedEventHandler
                End If
                Me._CardLoad = value
                myCard = Me._CardLoad
                If myCard IsNot Nothing Then
                    AddHandler myCard.MouseLeftButtonUp, mouseButtonEventHandler
                    AddHandler myCard.SizeChanged, sizeChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property Load As MyLoading

        Friend Overridable Property LabLoadTitle As TextBlock

        Friend Overridable Property LabLoadDesc As TextBlock

        Friend Overridable Property BtnLoadCancel As MyIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnLoadCancel
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.BtnLoadCancel_Click()
                End Sub
                Dim myIconButton As MyIconButton = Me._BtnLoadCancel
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnLoadCancel = value
                myIconButton = Me._BtnLoadCancel
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ColumnProgressA As ColumnDefinition

        Friend Overridable Property ColumnProgressB As ColumnDefinition

        Friend Overridable Property RectProgressClip As RectangleGeometry

        Friend Overridable Property PanContent As Grid

        Friend Overridable Property PanSelect As Grid

        Friend Overridable Property PanSelectCreate As MyCard
            <CompilerGenerated()>
            Get
                Return Me._PanSelectCreate
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCard)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.Create_MouseLeftButtonUp()
                End Sub
                Dim myCard As MyCard = Me._PanSelectCreate
                If myCard IsNot Nothing Then
                    RemoveHandler myCard.MouseLeftButtonUp, mouseButtonEventHandler
                End If
                Me._PanSelectCreate = value
                myCard = Me._PanSelectCreate
                If myCard IsNot Nothing Then
                    AddHandler myCard.MouseLeftButtonUp, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanSelectJoin As MyCard
            <CompilerGenerated()>
            Get
                Return Me._PanSelectJoin
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCard)
                Dim mouseButtonEventHandler As MouseButtonEventHandler
                If PageLinkMain._Closure$__.$IR122-10 IsNot Nothing Then
                    mouseButtonEventHandler = PageLinkMain._Closure$__.$IR122-10
                Else
                    Dim mouseButtonEventHandler2 As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                        PageLinkMain.Join()
                    End Sub
                    mouseButtonEventHandler = mouseButtonEventHandler2
                    PageLinkMain._Closure$__.$IR122-10 = mouseButtonEventHandler2
                End If
                Dim mouseButtonEventHandler3 As MouseButtonEventHandler = mouseButtonEventHandler
                Dim routedEventHandler As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    Me.PanSelectJoin_Loaded()
                End Sub
                Dim myCard As MyCard = Me._PanSelectJoin
                If myCard IsNot Nothing Then
                    RemoveHandler myCard.MouseLeftButtonUp, mouseButtonEventHandler3
                    RemoveHandler myCard.Loaded, routedEventHandler
                End If
                Me._PanSelectJoin = value
                myCard = Me._PanSelectJoin
                If myCard IsNot Nothing Then
                    AddHandler myCard.MouseLeftButtonUp, mouseButtonEventHandler3
                    AddHandler myCard.Loaded, routedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanFinish As Grid

        Friend Overridable Property HintFinish As MyHint

        Friend Overridable Property ImgFinishLogo As Path

        Friend Overridable Property LabFinishTitle As TextBlock

        Friend Overridable Property LabFinishDesc As TextBlock

        Friend Overridable Property BtnFinishCopy As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnFinishCopy
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.Copy()
                End Sub
                Dim myButton As MyButton = Me._BtnFinishCopy
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnFinishCopy = value
                myButton = Me._BtnFinishCopy
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnFinishExit As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnFinishExit
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.BtnFinishExit_Click()
                End Sub
                Dim myButton As MyButton = Me._BtnFinishExit
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnFinishExit = value
                myButton = Me._BtnFinishExit
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnFinishPing As StackPanel

        Friend Overridable Property LabFinishPing As TextBlock

        Friend Overridable Property BtnFinishPlayer As StackPanel

        Friend Overridable Property LabFinishPlayer As TextBlock

        Friend Overridable Property LineFinishPort As TextBlock

        Friend Overridable Property BtnFinishPort As StackPanel

        Friend Overridable Property LabFinishPort As TextBlock

        Friend Overridable Property BtnFinishIp As StackPanel
            <CompilerGenerated()>
            Get
                Return Me._BtnFinishIp
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As StackPanel)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = AddressOf Me.BtnFinishIp_MouseLeftButtonUp
                Dim stackPanel As StackPanel = Me._BtnFinishIp
                If stackPanel IsNot Nothing Then
                    RemoveHandler stackPanel.MouseLeftButtonUp, mouseButtonEventHandler
                End If
                Me._BtnFinishIp = value
                stackPanel = Me._BtnFinishIp
                If stackPanel IsNot Nothing Then
                    AddHandler stackPanel.MouseLeftButtonUp, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabFinishIp As TextBlock

        Friend Overridable Property PanAlways As Grid

        Private Const INVITE_CODE_VERSION As Integer = 2

        Public Shared LinkState As PageLinkMain.LinkStates = PageLinkMain.LinkStates.Waiting

        Private Shared LastCode As String = Nothing

        Private Shared AutoJoinCode As String = Nothing

        Private FailReason As String

        Private PathEasyTier As String

        Private IsServerSide As Boolean

        Private ServerPort As Integer

        Private ClientPort As Integer

        Private RPCPort As Integer

        Private ClientAddress As String

        Private DiscoverNodeID As Integer

        Private NetworkName As String

        Private NetworkSecret As String

        Private ServerVersion As Integer

        Private Shared ProcessCore As Process

        Private Shared ProcessOutputHandle As AutoResetEvent

        Private Shared ProcessErrorHandle As AutoResetEvent

        Private LogHistory As Queue(Of String)

        Private Peers As List(Of PageLinkMain.Peer)

        Private NATType As PageLinkMain.NATTypes

        Private RefreshPeerLoader As ModLoader.LoaderTask(Of Integer, List(Of PageLinkMain.Peer))

        Private FailCount As Integer

        Private $STATIC$StartTimerThread$2001$IsTimerStarted As Boolean

        Private $STATIC$StartTimerThread$2001$IsTimerStarted$Init As StaticLocalInitFlag

        Private $STATIC$Update$2001$BroadcastSocket As Socket

        Private $STATIC$Update$2001$BroadcastSocket$Init As StaticLocalInitFlag

        Public Enum LinkStates
            Waiting
            Loading
            Failed
            Finished
        End Enum

        Private Class Peer
            Public Sub New(Info As JObject)
                Dim text As String = Info("hostname").ToString()
                If Operators.CompareString(Info("cost").ToString(), "Local", False) = 0 Then
                    Me.Type = PageLinkMain.Peer.Types.Self
                ElseIf text.StartsWithF("Client", False) Then
                    Me.Type = PageLinkMain.Peer.Types.Client
                ElseIf CStr(Info("ipv4")) = "10.114.114.114" Then
                    Me.Type = PageLinkMain.Peer.Types.Server
                Else
                    Me.Type = PageLinkMain.Peer.Types.Misc
                End If
                Double.TryParse(CStr(Info("lat_ms")), NumberStyles.Any, CultureInfo.InvariantCulture, Me.Ping)
                Me.Name = text
                Me.Relay = Info("cost").ToString().ContainsIgnoreCase("relay")
                Me.NATType = EnumUtils.FromString(Of PageLinkMain.NATTypes)(CStr(Info("nat_type")))
            End Sub

            Public Overrides Function ToString() As String
                Return String.Format("{0} - {1} - Ping {2:0.0}ms [中继? {3}] - NAT {4}", New Object() { Me.Type, Me.Name, Me.Ping, Me.Relay, Me.NATType })
            End Function

            Public Type As PageLinkMain.Peer.Types

            Public Ping As Double

            Public Name As String

            Public Relay As Boolean

            Public NATType As PageLinkMain.NATTypes

            Public Enum Types
                Client
                Server
                Misc
                Self
            End Enum
        End Class

        Private Enum NATTypes
            Unknown
            OpenInternet
            NoPAT
            FullCone
            Restricted
            PortRestricted
            Symmetric
            SymUdpFirewall
            SymmetricEasyInc
            SymmetricEasyDec
            Pending
        End Enum
    End Class
End Namespace
