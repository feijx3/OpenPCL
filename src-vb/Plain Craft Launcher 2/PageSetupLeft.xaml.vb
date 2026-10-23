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
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageSetupLeft
        Inherits MyPageLeft

        Private Sub PageSetupLeft_Loaded(sender As Object, e As RoutedEventArgs)
            Dim flag As Boolean = False
            If Me.ItemLaunch.Checked AndAlso Settings.[Get](Of Boolean)("UiHiddenSetupLaunch", Nothing) Then
                flag = True
            End If
            If Me.ItemUI.Checked AndAlso Settings.[Get](Of Boolean)("UiHiddenSetupUi", Nothing) Then
                flag = True
            End If
            If Me.ItemSystem.Checked AndAlso Settings.[Get](Of Boolean)("UiHiddenSetupSystem", Nothing) Then
                flag = True
            End If
            If Me.ItemLink.Checked AndAlso Settings.[Get](Of Boolean)("UiHiddenSetupLink", Nothing) Then
                flag = True
            End If
            If PageSetupUI.HiddenForceShow Then
                flag = False
            End If
            If Not Me.IsLoad OrElse flag Then
                Me.IsLoad = True
                PageSetupUI.HiddenRefresh()
                If Not Me.IsPageSwitched Then
                    If Not Settings.[Get](Of Boolean)("UiHiddenSetupLaunch", Nothing) Then
                        Me.ItemLaunch.SetChecked(True, False, False)
                        Return
                    End If
                    If Not Settings.[Get](Of Boolean)("UiHiddenSetupUi", Nothing) Then
                        Me.ItemUI.SetChecked(True, False, False)
                        Return
                    End If
                    If Not Settings.[Get](Of Boolean)("UiHiddenSetupSystem", Nothing) Then
                        Me.ItemSystem.SetChecked(True, False, False)
                        Return
                    End If
                    If Not Settings.[Get](Of Boolean)("UiHiddenSetupLink", Nothing) Then
                        Me.ItemLink.SetChecked(True, False, False)
                        Return
                    End If
                    Me.ItemLaunch.SetChecked(True, False, False)
                End If
            End If
        End Sub

        Private Sub PageOtherLeft_Unloaded(sender As Object, e As RoutedEventArgs)
            Me.IsPageSwitched = False
        End Sub

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageSetupLeft_Loaded
            AddHandler MyBase.Unloaded, AddressOf Me.PageOtherLeft_Unloaded
            Me.IsLoad = False
            Me.IsPageSwitched = False
            Me.InitializeComponent()
            If Not Settings.[Get](Of Boolean)("UiHiddenSetupLaunch", Nothing) Then
                Me.PageID = FormMain.PageSubType.[Default]
                Return
            End If
            If Not Settings.[Get](Of Boolean)("UiHiddenSetupUi", Nothing) Then
                Me.PageID = FormMain.PageSubType.DownloadMod
                Return
            End If
            If Not Settings.[Get](Of Boolean)("UiHiddenSetupSystem", Nothing) Then
                Me.PageID = FormMain.PageSubType.DownloadPack
                Return
            End If
            If Not Settings.[Get](Of Boolean)("UiHiddenSetupLink", Nothing) Then
                Me.PageID = FormMain.PageSubType.SetupLink
                Return
            End If
            Me.PageID = FormMain.PageSubType.[Default]
        End Sub

        Private Sub PageCheck(sender As MyListItem, e As EventArgs)
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
                    If ModMain.FrmSetupLaunch Is Nothing Then
                        ModMain.FrmSetupLaunch = New PageSetupLaunch()
                    End If
                    obj = ModMain.FrmSetupLaunch
                Case FormMain.PageSubType.SetupLink
                    If ModMain.FrmSetupLink Is Nothing Then
                        ModMain.FrmSetupLink = New PageSetupLink()
                    End If
                    obj = ModMain.FrmSetupLink
                Case FormMain.PageSubType.DownloadMod
                    If ModMain.FrmSetupUI Is Nothing Then
                        ModMain.FrmSetupUI = New PageSetupUI()
                    End If
                    obj = ModMain.FrmSetupUI
                Case FormMain.PageSubType.DownloadPack
                    If ModMain.FrmSetupSystem Is Nothing Then
                        ModMain.FrmSetupSystem = New PageSetupSystem()
                    End If
                    obj = ModMain.FrmSetupSystem
                Case Else
                    Throw New Exception("未知的设置子页面种类：" + Conversions.ToString(CInt(ID)))
            End Select
            Return obj
        End Function

        Public Sub PageChange(ID As FormMain.PageSubType)
            ' The following expression was wrapped in a checked-statement
            If Me.PageID <> ID Then
                ModAnimation.AniControlEnabled += 1
                Me.IsPageSwitched = True
                Try
                    Select Case ID
                        Case FormMain.PageSubType.[Default]
                            If Information.IsNothing(ModMain.FrmSetupLaunch) Then
                                ModMain.FrmSetupLaunch = New PageSetupLaunch()
                            End If
                            PageSetupLeft.PageChangeRun(ModMain.FrmSetupLaunch)
                        Case FormMain.PageSubType.SetupLink
                            If Information.IsNothing(ModMain.FrmSetupLink) Then
                                ModMain.FrmSetupLink = New PageSetupLink()
                            End If
                            PageSetupLeft.PageChangeRun(ModMain.FrmSetupLink)
                        Case FormMain.PageSubType.DownloadMod
                            If Information.IsNothing(ModMain.FrmSetupUI) Then
                                ModMain.FrmSetupUI = New PageSetupUI()
                            End If
                            PageSetupLeft.PageChangeRun(ModMain.FrmSetupUI)
                        Case FormMain.PageSubType.DownloadPack
                            If Information.IsNothing(ModMain.FrmSetupSystem) Then
                                ModMain.FrmSetupSystem = New PageSetupSystem()
                            End If
                            PageSetupLeft.PageChangeRun(ModMain.FrmSetupSystem)
                        Case Else
                            Throw New Exception("未知的设置子页面种类：" + Conversions.ToString(CInt(ID)))
                    End Select
                    Me.PageID = ID
                Catch ex As Exception
                    Logger.[Error](ex, String.Format("切换分页面失败（ID {0}）", ID), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupLeft.xaml.vb")
                Finally
                    ModAnimation.AniControlEnabled -= 1
                End Try
            End If
        End Sub

        Private Shared Sub PageChangeRun(Target As MyPageRight)
            ModAnimation.AniStop("FrmMain PageChangeRight")
            If Target.Parent IsNot Nothing Then
                Target.SetValue(ContentPresenter.ContentProperty, Nothing)
            End If
            ModMain.FrmMain.PageRight = Target
            CType(ModMain.FrmMain.PanMainRight.Child, MyPageRight).PageOnExit()
            Dim array As ModAnimation.AniData() = New ModAnimation.AniData(1) {}
            Dim num As Integer = 0
            Dim threadStart As ThreadStart
            If PageSetupLeft._Closure$__.$I9-0 IsNot Nothing Then
                threadStart = PageSetupLeft._Closure$__.$I9-0
            Else
                Dim threadStart2 As ThreadStart = Sub()
                    CType(ModMain.FrmMain.PanMainRight.Child, MyPageRight).PageOnForceExit()
                    ModMain.FrmMain.PanMainRight.Child = ModMain.FrmMain.PageRight
                    ModMain.FrmMain.PageRight.Opacity = 0.0
                End Sub
                threadStart = threadStart2
                PageSetupLeft._Closure$__.$I9-0 = threadStart2
            End If
            array(num) = ModAnimation.AaCode(threadStart, 130, False)
            Dim num2 As Integer = 1
            Dim threadStart3 As ThreadStart
            If PageSetupLeft._Closure$__.$I9-1 IsNot Nothing Then
                threadStart3 = PageSetupLeft._Closure$__.$I9-1
            Else
                Dim threadStart4 As ThreadStart = Sub()
                    ModMain.FrmMain.PageRight.Opacity = 1.0
                    ModMain.FrmMain.PageRight.PageOnEnter()
                End Sub
                threadStart3 = threadStart4
                PageSetupLeft._Closure$__.$I9-1 = threadStart4
            End If
            array(num2) = ModAnimation.AaCode(threadStart3, 30, True)
            ModAnimation.AniStart(array, "PageLeft PageChange", False)
        End Sub

        Public Sub Reset(sender As Object, e As EventArgs)
            Dim num As Double = ModBase.Val(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing)))
            If num = 0.0 Then
                If ModMain.MyMsgBox("是否要初始化 启动 页面的所有设置？该操作不可撤销。", "初始化确认", "确定", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 1 Then
                    If Information.IsNothing(ModMain.FrmSetupLaunch) Then
                        ModMain.FrmSetupLaunch = New PageSetupLaunch()
                    End If
                    ModMain.FrmSetupLaunch.Reset()
                    Me.ItemLaunch.Checked = True
                    Return
                End If
            ElseIf num = 2.0 Then
                If ModMain.MyMsgBox("是否要初始化 个性化 页面的所有设置？该操作不可撤销。" & vbCrLf & "（背景图片与音乐、主页等外部文件不会被删除）", "初始化确认", "确定", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 1 Then
                    If Information.IsNothing(ModMain.FrmSetupUI) Then
                        ModMain.FrmSetupUI = New PageSetupUI()
                    End If
                    ModMain.FrmSetupUI.Reset()
                    Me.ItemUI.Checked = True
                    Return
                End If
            ElseIf num = 3.0 Then
                If ModMain.MyMsgBox("是否要初始化 其他 页面的所有设置？该操作不可撤销。", "初始化确认", "确定", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 1 Then
                    If Information.IsNothing(ModMain.FrmSetupSystem) Then
                        ModMain.FrmSetupSystem = New PageSetupSystem()
                    End If
                    ModMain.FrmSetupSystem.Reset()
                    Me.ItemSystem.Checked = True
                    Return
                End If
            ElseIf num = 1.0 AndAlso ModMain.MyMsgBox("是否要初始化 联机 页面的所有设置？该操作不可撤销。", "初始化确认", "确定", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 1 Then
                If Information.IsNothing(ModMain.FrmSetupLink) Then
                    ModMain.FrmSetupLink = New PageSetupLink()
                End If
                ModMain.FrmSetupLink.Reset()
                Me.ItemLink.Checked = True
            End If
        End Sub

        Friend Overridable Property PanItem As StackPanel

        Friend Overridable Property ItemLaunch As MyListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemLaunch
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyListItem)
                Dim checkEventHandler As IMyRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, MyListItem), a1)
                End Sub
                Dim myListItem As MyListItem = Me._ItemLaunch
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemLaunch = value
                myListItem = Me._ItemLaunch
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemLink As MyListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemLink
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyListItem)
                Dim checkEventHandler As IMyRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, MyListItem), a1)
                End Sub
                Dim myListItem As MyListItem = Me._ItemLink
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemLink = value
                myListItem = Me._ItemLink
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemUI As MyListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemUI
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyListItem)
                Dim checkEventHandler As IMyRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, MyListItem), a1)
                End Sub
                Dim myListItem As MyListItem = Me._ItemUI
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemUI = value
                myListItem = Me._ItemUI
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemSystem As MyListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemSystem
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyListItem)
                Dim checkEventHandler As IMyRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, MyListItem), a1)
                End Sub
                Dim myListItem As MyListItem = Me._ItemSystem
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemSystem = value
                myListItem = Me._ItemSystem
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Private IsLoad As Boolean

        Private IsPageSwitched As Boolean

        Public PageID As FormMain.PageSubType
    End Class
End Namespace
