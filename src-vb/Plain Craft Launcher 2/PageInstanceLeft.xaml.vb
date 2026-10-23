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
Imports PCLCS

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageInstanceLeft
        Inherits MyPageLeft
        Implements IRefreshable

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageInstanceLeft_Loaded
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.RefreshModDisabled()
            End Sub
            Dim routedEventHandler As RoutedEventHandler
            If PageInstanceLeft._Closure$__.$IR1-2 IsNot Nothing Then
                routedEventHandler = PageInstanceLeft._Closure$__.$IR1-2
            Else
                Dim routedEventHandler2 As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    PageInstanceLeft.ReloadCurrentJava()
                End Sub
                routedEventHandler = routedEventHandler2
                PageInstanceLeft._Closure$__.$IR1-2 = routedEventHandler2
            End If
            AddHandler MyBase.Loaded, routedEventHandler
            Me.PageID = FormMain.PageSubType.[Default]
            Me.InitializeComponent()
        End Sub

        Private Sub PageInstanceLeft_Loaded(sender As Object, e As RoutedEventArgs)
            PageSetupUI.HiddenRefresh()
        End Sub

        Public Sub RefreshModDisabled()
            If PageInstanceLeft.Instance IsNot Nothing AndAlso PageInstanceLeft.Instance.Modable Then
                Me.ItemMod.Visibility = Visibility.Visible
                Me.ItemModDisabled.Visibility = Visibility.Collapsed
                Return
            End If
            Me.ItemMod.Visibility = Visibility.Collapsed
            Me.ItemModDisabled.Visibility = Visibility.Visible
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
                    If ModMain.FrmInstanceOverall Is Nothing Then
                        ModMain.FrmInstanceOverall = New PageInstanceOverall()
                    End If
                    obj = ModMain.FrmInstanceOverall
                Case FormMain.PageSubType.SetupLink
                    If ModMain.FrmInstanceSetup Is Nothing Then
                        ModMain.FrmInstanceSetup = New PageInstanceSetup()
                    End If
                    obj = ModMain.FrmInstanceSetup
                Case FormMain.PageSubType.DownloadMod
                    If ModMain.FrmDownloadModify Is Nothing Then
                        ModMain.FrmDownloadModify = New PageDownloadModify()
                    End If
                    obj = ModMain.FrmDownloadModify
                Case FormMain.PageSubType.DownloadPack
                    If ModMain.FrmInstanceMod Is Nothing Then
                        ModMain.FrmInstanceMod = New PageInstanceMod()
                    End If
                    obj = ModMain.FrmInstanceMod
                Case FormMain.PageSubType.DownloadDataPack
                    If ModMain.FrmInstanceModDisabled Is Nothing Then
                        ModMain.FrmInstanceModDisabled = New PageInstanceModDisabled()
                    End If
                    obj = ModMain.FrmInstanceModDisabled
                Case FormMain.PageSubType.DownloadResourcePack
                    If ModMain.FrmInstanceExport Is Nothing Then
                        ModMain.FrmInstanceExport = New PageInstanceExport()
                    End If
                    obj = ModMain.FrmInstanceExport
                Case Else
                    Throw New Exception("未知的版本设置子页面种类：" + Conversions.ToString(CInt(ID)))
            End Select
            Return obj
        End Function

        Public Sub PageChange(ID As FormMain.PageSubType)
            ' The following expression was wrapped in a checked-statement
            If Me.PageID <> ID Then
                If PageInstanceLeft.Instance IsNot Nothing AndAlso ModMinecraft.McInstanceLock.IsLocked(PageInstanceLeft.Instance) Then
                    ModMain.Hint("该版本正在被修改，请稍后再切换页面！", ModMain.HintType.Red, True)
                    Return
                End If
                ModAnimation.AniControlEnabled += 1
                Try
                    PageInstanceLeft.PageChangeRun(CType(Me.PageGet(ID), MyPageRight))
                    Me.PageID = ID
                Catch ex As Exception
                    Logger.[Error](ex, String.Format("切换分页面失败（ID {0}）", ID), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageInstance\PageInstanceLeft.xaml.vb")
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
            If PageInstanceLeft._Closure$__.$I9-0 IsNot Nothing Then
                threadStart = PageInstanceLeft._Closure$__.$I9-0
            Else
                Dim threadStart2 As ThreadStart = Sub()
                    CType(ModMain.FrmMain.PanMainRight.Child, MyPageRight).PageOnForceExit()
                    ModMain.FrmMain.PanMainRight.Child = ModMain.FrmMain.PageRight
                    ModMain.FrmMain.PageRight.Opacity = 0.0
                End Sub
                threadStart = threadStart2
                PageInstanceLeft._Closure$__.$I9-0 = threadStart2
            End If
            array(num) = ModAnimation.AaCode(threadStart, 130, False)
            Dim num2 As Integer = 1
            Dim threadStart3 As ThreadStart
            If PageInstanceLeft._Closure$__.$I9-1 IsNot Nothing Then
                threadStart3 = PageInstanceLeft._Closure$__.$I9-1
            Else
                Dim threadStart4 As ThreadStart = Sub()
                    ModMain.FrmMain.PageRight.Opacity = 1.0
                    ModMain.FrmMain.PageRight.PageOnEnter()
                End Sub
                threadStart3 = threadStart4
                PageInstanceLeft._Closure$__.$I9-1 = threadStart4
            End If
            array(num2) = ModAnimation.AaCode(threadStart3, 30, True)
            ModAnimation.AniStart(array, "PageLeft PageChange", False)
        End Sub

        Public Sub Refresh_Click(sender As Object, e As EventArgs)
            ' The following expression was wrapped in a checked-expression
            Me.Refresh(CType(Math.Round(ModBase.Val(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing)))), FormMain.PageSubType))
        End Sub

        Public Sub Refresh() Implements PCL.IRefreshable.Refresh
            Me.Refresh(ModMain.FrmMain.PageCurrentSub)
        End Sub

        Public Sub Refresh(SubType As FormMain.PageSubType)
            If PageInstanceLeft.Instance IsNot Nothing AndAlso ModMinecraft.McInstanceLock.IsLocked(PageInstanceLeft.Instance) Then
                ModMain.Hint("该版本正在被修改，请稍后再刷新！", ModMain.HintType.Red, True)
                Return
            End If
            Select Case SubType
                Case FormMain.PageSubType.DownloadMod
                    Dim frmDownloadModify As PageDownloadModify = ModMain.FrmDownloadModify
                    If frmDownloadModify IsNot Nothing Then
                        frmDownloadModify.PageLoaderRestart(Nothing, True)
                    End If
                    Me.ItemModify.Checked = True
                Case FormMain.PageSubType.DownloadPack
                    PageInstanceMod.Refresh()
                    Me.ItemMod.Checked = True
                    Return
                Case FormMain.PageSubType.DownloadDataPack
                Case FormMain.PageSubType.DownloadResourcePack
                    Dim frmInstanceExport As PageInstanceExport = ModMain.FrmInstanceExport
                    If frmInstanceExport IsNot Nothing Then
                        frmInstanceExport.RefreshAll()
                    End If
                    Me.ItemExport.Checked = True
                    Return
                Case Else
                    Return
            End Select
        End Sub

        Public Sub Reset(sender As Object, e As EventArgs)
            If ModMain.MyMsgBox("是否要初始化该版本的版本独立设置？该操作不可撤销。", "初始化确认", "确定", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 1 Then
                If Information.IsNothing(ModMain.FrmInstanceSetup) Then
                    ModMain.FrmInstanceSetup = New PageInstanceSetup()
                End If
                ModMain.FrmInstanceSetup.Reset()
                Me.ItemSetup.Checked = True
            End If
        End Sub

        Public Shared Sub ReloadCurrentJava()
            If ModAnimation.AniControlEnabled = 0 AndAlso PageInstanceLeft.Instance IsNot Nothing Then
                PageInstanceLeft.CurrentJavaWorker.Start(Nothing, Nothing)
            End If
        End Sub

        Friend Overridable Property PanItem As StackPanel

        Friend Overridable Property ItemOverall As MyListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemOverall
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyListItem)
                Dim checkEventHandler As IMyRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, FrameworkElement), a1)
                End Sub
                Dim myListItem As MyListItem = Me._ItemOverall
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemOverall = value
                myListItem = Me._ItemOverall
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemSetup As MyListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemSetup
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyListItem)
                Dim checkEventHandler As IMyRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, FrameworkElement), a1)
                End Sub
                Dim myListItem As MyListItem = Me._ItemSetup
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemSetup = value
                myListItem = Me._ItemSetup
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemModify As MyListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemModify
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyListItem)
                Dim checkEventHandler As IMyRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, FrameworkElement), a1)
                End Sub
                Dim myListItem As MyListItem = Me._ItemModify
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemModify = value
                myListItem = Me._ItemModify
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemMod As MyListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemMod
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyListItem)
                Dim checkEventHandler As IMyRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, FrameworkElement), a1)
                End Sub
                Dim myListItem As MyListItem = Me._ItemMod
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemMod = value
                myListItem = Me._ItemMod
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemModDisabled As MyListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemModDisabled
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyListItem)
                Dim checkEventHandler As IMyRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, FrameworkElement), a1)
                End Sub
                Dim myListItem As MyListItem = Me._ItemModDisabled
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemModDisabled = value
                myListItem = Me._ItemModDisabled
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemExport As MyListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemExport
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyListItem)
                Dim checkEventHandler As IMyRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, FrameworkElement), a1)
                End Sub
                Dim myListItem As MyListItem = Me._ItemExport
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemExport = value
                myListItem = Me._ItemExport
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Public Shared Instance As ModMinecraft.McInstance = Nothing

        Public PageID As FormMain.PageSubType

        Public Shared CurrentJavaWorker As RedoableWorker(Of Java) = New RedoableWorker(Of Java)(Function(c As CancellationToken?, p As ProgressProvider) ModJava.SelectOrDownloadJava(PageInstanceLeft.Instance, False, c.Value, p), "CurrentJavaWorker")
    End Class
End Namespace
