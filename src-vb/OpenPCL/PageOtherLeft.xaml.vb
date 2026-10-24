Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Markup
Imports MeloongCore
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageOtherLeft
        Inherits OurPageLeft

        Private Sub PageOtherLeft_Loaded(sender As Object, e As RoutedEventArgs)
            Dim flag As Boolean = False
            If Me.ItemHelp.Checked AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherHelp", Nothing) Then
                flag = True
            End If
            If Me.ItemAbout.Checked AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherAbout", Nothing) Then
                flag = True
            End If
            If Me.ItemTest.Checked AndAlso Settings.[Get](Of Boolean)("UiHiddenOtherTest", Nothing) Then
                flag = True
            End If
            If PageSetupUI.HiddenForceShow Then
                flag = False
            End If
            If Not Me.IsLoad OrElse flag Then
                Me.IsLoad = True
                PageSetupUI.HiddenRefresh()
                If Not Me.IsPageSwitched Then
                    If Not Settings.[Get](Of Boolean)("UiHiddenOtherHelp", Nothing) Then
                        Me.ItemHelp.SetChecked(True, False, False)
                        Return
                    End If
                    If Not Settings.[Get](Of Boolean)("UiHiddenOtherAbout", Nothing) Then
                        Me.ItemAbout.SetChecked(True, False, False)
                        Return
                    End If
                    Me.ItemTest.SetChecked(True, False, False)
                End If
            End If
        End Sub

        Private Sub PageOtherLeft_Unloaded(sender As Object, e As RoutedEventArgs)
            Me.IsPageSwitched = False
        End Sub

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageOtherLeft_Loaded
            AddHandler MyBase.Unloaded, AddressOf Me.PageOtherLeft_Unloaded
            Me.IsLoad = False
            Me.IsPageSwitched = False
            Me.InitializeComponent()
            If Not Settings.[Get](Of Boolean)("UiHiddenOtherHelp", Nothing) Then
                Me.PageID = FormMain.PageSubType.[Default]
                Return
            End If
            If Not Settings.[Get](Of Boolean)("UiHiddenOtherAbout", Nothing) Then
                Me.PageID = FormMain.PageSubType.SetupLink
                Return
            End If
            Me.PageID = FormMain.PageSubType.DownloadMod
        End Sub

        Private Sub PageCheck(sender As FrameworkElement, e As ModBase.RouteEventArgs)
            If sender.Tag IsNot Nothing Then
                ' The following expression was wrapped in a checked-expression
                Me.PageChange(CType(Math.Round(ModBase.Val(RuntimeHelpers.GetObjectValue(sender.Tag))), FormMain.PageSubType))
            End If
        End Sub

        Public Function PageGet(Optional ID As FormMain.PageSubType = CType((-1), FormMain.PageSubType)) As Object
            If ID = CType((-1), FormMain.PageSubType) Then
                ID = Me.PageID
            End If
            Dim obj As Object
            Select Case ID
                Case FormMain.PageSubType.[Default]
                    If ModMain.FrmOtherHelp Is Nothing Then
                        ModMain.FrmOtherHelp = New PageOtherHelp()
                    End If
                    obj = ModMain.FrmOtherHelp
                Case FormMain.PageSubType.SetupLink
                    If ModMain.FrmOtherAbout Is Nothing Then
                        ModMain.FrmOtherAbout = New PageOtherAbout()
                    End If
                    obj = ModMain.FrmOtherAbout
                Case FormMain.PageSubType.DownloadMod
                    If ModMain.FrmOtherTest Is Nothing Then
                        ModMain.FrmOtherTest = New PageOtherTest()
                    End If
                    obj = ModMain.FrmOtherTest
                Case Else
                    Throw New Exception("未知的更多子页面种类：" + Conversions.ToString(CInt(ID)))
            End Select
            Return obj
        End Function

        Public Sub PageChange(ID As FormMain.PageSubType)
            ' The following expression was wrapped in a checked-statement
            If Me.PageID <> ID Then
                ModAnimation.AniControlEnabled += 1
                Me.IsPageSwitched = True
                Try
                    PageOtherLeft.PageChangeRun(CType(Me.PageGet(ID), OurPageRight))
                    Me.PageID = ID
                Catch ex As Exception
                    Logger.[Error](ex, String.Format("切换分页面失败（ID {0}）", ID), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageOther\PageOtherLeft.xaml.vb")
                Finally
                    ModAnimation.AniControlEnabled -= 1
                End Try
            End If
        End Sub

        Private Shared Sub PageChangeRun(Target As OurPageRight)
            ModAnimation.AniStop("FrmMain PageChangeRight")
            If Target.Parent IsNot Nothing Then
                Target.SetValue(ContentPresenter.ContentProperty, Nothing)
            End If
            ModMain.FrmMain.PageRight = Target
            CType(ModMain.FrmMain.PanMainRight.Child, OurPageRight).PageOnExit()
            Dim array As ModAnimation.AniData() = New ModAnimation.AniData(1) {}
            Dim num As Integer = 0
            Dim threadStart As ThreadStart
            If PageOtherLeft._Closure$__.$I9-0 IsNot Nothing Then
                threadStart = PageOtherLeft._Closure$__.$I9-0
            Else
                Dim threadStart2 As ThreadStart = Sub()
                    CType(ModMain.FrmMain.PanMainRight.Child, OurPageRight).PageOnForceExit()
                    ModMain.FrmMain.PanMainRight.Child = ModMain.FrmMain.PageRight
                    ModMain.FrmMain.PageRight.Opacity = 0.0
                End Sub
                threadStart = threadStart2
                PageOtherLeft._Closure$__.$I9-0 = threadStart2
            End If
            array(num) = ModAnimation.AaCode(threadStart, 130, False)
            Dim num2 As Integer = 1
            Dim threadStart3 As ThreadStart
            If PageOtherLeft._Closure$__.$I9-1 IsNot Nothing Then
                threadStart3 = PageOtherLeft._Closure$__.$I9-1
            Else
                Dim threadStart4 As ThreadStart = Sub()
                    ModMain.FrmMain.PageRight.Opacity = 1.0
                    ModMain.FrmMain.PageRight.PageOnEnter()
                End Sub
                threadStart3 = threadStart4
                PageOtherLeft._Closure$__.$I9-1 = threadStart4
            End If
            array(num2) = ModAnimation.AaCode(threadStart3, 30, True)
            ModAnimation.AniStart(array, "PageLeft PageChange", False)
        End Sub

        Public Sub Refresh(sender As Object, e As EventArgs)
            If ModBase.Val(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing))) = 0.0 Then
                PageOtherLeft.RefreshHelp()
                Me.ItemHelp.Checked = True
            End If
            ModMain.Hint("正在刷新……", ModMain.HintType.Blue, False)
        End Sub

        Public Shared Sub RefreshHelp()
            Settings.[Set]("SystemHelpVersion", 0, Nothing)
            ModMain.FrmOtherHelp.PageLoaderRestart(Nothing, True)
            ModMain.FrmOtherHelp.SearchBox.Text = ""
        End Sub

        Private Sub TryFeedback(sender As Object, e As ModBase.RouteEventArgs)
            If Me.ItemFeedback.Checked Then
                PageOtherLeft.TryFeedback()
                e.Handled = True
            End If
        End Sub

        Public Shared Sub TryFeedback()
            If False.Equals(PageSetupSystem.IsLauncherNewest()) Then
                If ModMain.OurMsgBox(String.Format("你的 PCL 不是最新版，因此无法提交反馈。{0}请在更新后，确认该问题在最新版中依然存在，然后再提交反馈。", vbCrLf), "无法提交反馈", "更新", "取消", "", False, True, False, Nothing, Nothing, Nothing) = 1 Then
                    ModSecret.UpdateCheckByButton()
                    Return
                End If
            Else
                Dim num As Integer = ModMain.OurMsgBox("在提交新反馈前，建议先搜索反馈列表，以避免重复提交。" & vbCrLf & "如果无法打开该网页，请使用 VPN 改善网络环境。", "反馈", "提交新反馈", "查看反馈列表", "取消", False, True, False, Nothing, Nothing, Nothing)
                If num = 1 Then
                    ModMain.Feedback(True, False)
                    Return
                End If
                If num <> 2 Then
                    Return
                End If
                ModBase.OpenWebsite("https://github.com/Meloong-Git/PCL/issues/")
            End If
        End Sub

        Private Sub TryVote(sender As Object, e As ModBase.RouteEventArgs)
            If Me.ItemVote.Checked Then
                PageOtherLeft.TryVote()
                e.Handled = True
            End If
        End Sub

        Public Shared Sub TryVote()
            If ModMain.OurMsgBox("是否要打开新功能投票网页？" & vbCrLf & "如果无法打开该网页，请使用 VPN 改善网络环境。", "新功能投票", "打开", "取消", "", False, True, False, Nothing, Nothing, Nothing) <> 2 Then
                ModBase.OpenWebsite("https://github.com/Meloong-Git/PCL/discussions/categories/%E5%8A%9F%E8%83%BD%E6%8A%95%E7%A5%A8?discussions_q=category%3A%E5%8A%9F%E8%83%BD%E6%8A%95%E7%A5%A8+sort%3Adate_created")
            End If
        End Sub

        Friend Overridable Property PanItem As StackPanel

        Friend Overridable Property ItemHelp As OurListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemHelp
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurListItem)
                Dim checkEventHandler As IOurRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, FrameworkElement), a1)
                End Sub
                Dim myListItem As OurListItem = Me._ItemHelp
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemHelp = value
                myListItem = Me._ItemHelp
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemAbout As OurListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemAbout
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurListItem)
                Dim checkEventHandler As IOurRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, FrameworkElement), a1)
                End Sub
                Dim myListItem As OurListItem = Me._ItemAbout
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemAbout = value
                myListItem = Me._ItemAbout
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemTest As OurListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemTest
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurListItem)
                Dim checkEventHandler As IOurRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, FrameworkElement), a1)
                End Sub
                Dim myListItem As OurListItem = Me._ItemTest
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemTest = value
                myListItem = Me._ItemTest
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemFeedback As OurListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemFeedback
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurListItem)
                Dim changedEventHandler As IOurRadio.ChangedEventHandler = AddressOf Me.TryFeedback
                Dim myListItem As OurListItem = Me._ItemFeedback
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Changed, changedEventHandler
                End If
                Me._ItemFeedback = value
                myListItem = Me._ItemFeedback
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Changed, changedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemVote As OurListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemVote
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurListItem)
                Dim changedEventHandler As IOurRadio.ChangedEventHandler = AddressOf Me.TryVote
                Dim myListItem As OurListItem = Me._ItemVote
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Changed, changedEventHandler
                End If
                Me._ItemVote = value
                myListItem = Me._ItemVote
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Changed, changedEventHandler
                End If
            End Set
        End Property

        Private IsLoad As Boolean

        Private IsPageSwitched As Boolean

        Public PageID As FormMain.PageSubType
    End Class
End Namespace
