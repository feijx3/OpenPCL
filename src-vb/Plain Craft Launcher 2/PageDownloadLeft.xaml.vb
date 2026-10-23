Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports MeloongCore
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageDownloadLeft
        Inherits MyPageLeft
        Implements IRefreshable

        Public Sub New()
            Me.PageID = FormMain.PageSubType.[Default]
            Me.InitializeComponent()
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
            Select Case ID
                Case FormMain.PageSubType.[Default]
                    If ModMain.FrmDownloadInstall Is Nothing Then
                        ModMain.FrmDownloadInstall = New PageDownloadInstall()
                    End If
                    Return ModMain.FrmDownloadInstall
                Case FormMain.PageSubType.DownloadMod
                    If ModMain.FrmDownloadMod Is Nothing Then
                        ModMain.FrmDownloadMod = New PageDownloadMod()
                    End If
                    Return ModMain.FrmDownloadMod
                Case FormMain.PageSubType.DownloadPack
                    If ModMain.FrmDownloadPack Is Nothing Then
                        ModMain.FrmDownloadPack = New PageDownloadPack()
                    End If
                    Return ModMain.FrmDownloadPack
                Case FormMain.PageSubType.DownloadDataPack
                    If ModMain.FrmDownloadDataPack Is Nothing Then
                        ModMain.FrmDownloadDataPack = New PageDownloadDataPack()
                    End If
                    Return ModMain.FrmDownloadDataPack
                Case FormMain.PageSubType.DownloadResourcePack
                    If ModMain.FrmDownloadResourcePack Is Nothing Then
                        ModMain.FrmDownloadResourcePack = New PageDownloadResourcePack()
                    End If
                    Return ModMain.FrmDownloadResourcePack
                Case FormMain.PageSubType.DownloadShader
                    If ModMain.FrmDownloadShader Is Nothing Then
                        ModMain.FrmDownloadShader = New PageDownloadShader()
                    End If
                    Return ModMain.FrmDownloadShader
            End Select
            Throw New Exception("未知的下载子页面种类：" + Conversions.ToString(CInt(ID)))
        End Function

        Public Sub PageChange(ID As FormMain.PageSubType)
            ' The following expression was wrapped in a checked-statement
            If Me.PageID <> ID Then
                ModAnimation.AniControlEnabled += 1
                Try
                    PageDownloadLeft.PageChangeRun(CType(Me.PageGet(ID), MyPageRight))
                    Me.PageID = ID
                Catch ex As Exception
                    Logger.[Error](ex, String.Format("切换分页面失败（ID {0}）", ID), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageDownload\PageDownloadLeft.xaml.vb")
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
            If PageDownloadLeft._Closure$__.$I5-0 IsNot Nothing Then
                threadStart = PageDownloadLeft._Closure$__.$I5-0
            Else
                Dim threadStart2 As ThreadStart = Sub()
                    CType(ModMain.FrmMain.PanMainRight.Child, MyPageRight).PageOnForceExit()
                    ModMain.FrmMain.PanMainRight.Child = ModMain.FrmMain.PageRight
                    ModMain.FrmMain.PageRight.Opacity = 0.0
                End Sub
                threadStart = threadStart2
                PageDownloadLeft._Closure$__.$I5-0 = threadStart2
            End If
            array(num) = ModAnimation.AaCode(threadStart, 130, False)
            Dim num2 As Integer = 1
            Dim threadStart3 As ThreadStart
            If PageDownloadLeft._Closure$__.$I5-1 IsNot Nothing Then
                threadStart3 = PageDownloadLeft._Closure$__.$I5-1
            Else
                Dim threadStart4 As ThreadStart = Sub()
                    ModMain.FrmMain.PageRight.Opacity = 1.0
                    ModMain.FrmMain.PageRight.PageOnEnter()
                End Sub
                threadStart3 = threadStart4
                PageDownloadLeft._Closure$__.$I5-1 = threadStart4
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
            ResourceProject.Cache.Clear()
            ResourceVersion.ProjectFilesCache.Clear()
            Select Case SubType
                Case FormMain.PageSubType.[Default]
                    ModDownload.DlClientListLoader.Start(Nothing, True)
                    ModDownload.DlOptiFineListLoader.Start(Nothing, True)
                    ModDownload.DlForgeListLoader.Start(Nothing, True)
                    ModDownload.DlNeoForgeListLoader.Start(Nothing, True)
                    ModDownload.DlLiteLoaderListLoader.Start(Nothing, True)
                    ModDownload.DlFabricListLoader.Start(Nothing, True)
                    ModDownload.DlFabricApiLoader.Start(Nothing, True)
                    ModDownload.DlOptiFabricLoader.Start(Nothing, True)
                    Me.ItemInstall.Checked = True
                Case FormMain.PageSubType.DownloadMod
                    If ModMain.FrmDownloadMod IsNot Nothing Then
                        ModMain.FrmDownloadMod.Content.Storage = New ResourceSearcher.SearchResult()
                        ModMain.FrmDownloadMod.Content.Page = 0
                        ModMain.FrmDownloadMod.PageLoaderRestart(Nothing, True)
                    End If
                    Me.ItemMod.Checked = True
                Case FormMain.PageSubType.DownloadPack
                    If ModMain.FrmDownloadPack IsNot Nothing Then
                        ModMain.FrmDownloadPack.Content.Storage = New ResourceSearcher.SearchResult()
                        ModMain.FrmDownloadPack.Content.Page = 0
                        ModMain.FrmDownloadPack.PageLoaderRestart(Nothing, True)
                    End If
                    Me.ItemPack.Checked = True
                Case FormMain.PageSubType.DownloadDataPack
                    If ModMain.FrmDownloadDataPack IsNot Nothing Then
                        ModMain.FrmDownloadDataPack.Content.Storage = New ResourceSearcher.SearchResult()
                        ModMain.FrmDownloadDataPack.Content.Page = 0
                        ModMain.FrmDownloadDataPack.PageLoaderRestart(Nothing, True)
                    End If
                    Me.ItemDataPack.Checked = True
                Case FormMain.PageSubType.DownloadResourcePack
                    If ModMain.FrmDownloadResourcePack IsNot Nothing Then
                        ModMain.FrmDownloadResourcePack.Content.Storage = New ResourceSearcher.SearchResult()
                        ModMain.FrmDownloadResourcePack.Content.Page = 0
                        ModMain.FrmDownloadResourcePack.PageLoaderRestart(Nothing, True)
                    End If
                    Me.ItemResourcePack.Checked = True
                Case FormMain.PageSubType.DownloadShader
                    If ModMain.FrmDownloadShader IsNot Nothing Then
                        ModMain.FrmDownloadShader.Content.Storage = New ResourceSearcher.SearchResult()
                        ModMain.FrmDownloadShader.Content.Page = 0
                        ModMain.FrmDownloadShader.PageLoaderRestart(Nothing, True)
                    End If
                    Me.ItemShader.Checked = True
            End Select
            ModMain.Hint("正在刷新……", ModMain.HintType.Blue, False)
        End Sub

        Private Sub ItemInstall_Click(sender As Object, e As MouseButtonEventArgs)
            If Me.ItemInstall.Checked Then
                ModMain.FrmDownloadInstall.ExitSelectPage()
            End If
        End Sub

        Friend Overridable Property PanItem As StackPanel

        Friend Overridable Property ItemInstall As MyListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemInstall
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyListItem)
                Dim checkEventHandler As IMyRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, FrameworkElement), a1)
                End Sub
                Dim clickEventHandler As MyListItem.ClickEventHandler = AddressOf Me.ItemInstall_Click
                Dim myListItem As MyListItem = Me._ItemInstall
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                    RemoveHandler myListItem.Click, clickEventHandler
                End If
                Me._ItemInstall = value
                myListItem = Me._ItemInstall
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                    AddHandler myListItem.Click, clickEventHandler
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

        Friend Overridable Property ItemPack As MyListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemPack
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyListItem)
                Dim checkEventHandler As IMyRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, FrameworkElement), a1)
                End Sub
                Dim myListItem As MyListItem = Me._ItemPack
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemPack = value
                myListItem = Me._ItemPack
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemDataPack As MyListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemDataPack
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyListItem)
                Dim checkEventHandler As IMyRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, FrameworkElement), a1)
                End Sub
                Dim myListItem As MyListItem = Me._ItemDataPack
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemDataPack = value
                myListItem = Me._ItemDataPack
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemResourcePack As MyListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemResourcePack
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyListItem)
                Dim checkEventHandler As IMyRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, FrameworkElement), a1)
                End Sub
                Dim myListItem As MyListItem = Me._ItemResourcePack
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemResourcePack = value
                myListItem = Me._ItemResourcePack
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemShader As MyListItem
            <CompilerGenerated()>
            Get
                Return Me._ItemShader
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyListItem)
                Dim checkEventHandler As IMyRadio.CheckEventHandler = Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                    Me.PageCheck(CType(a0, FrameworkElement), a1)
                End Sub
                Dim myListItem As MyListItem = Me._ItemShader
                If myListItem IsNot Nothing Then
                    RemoveHandler myListItem.Check, checkEventHandler
                End If
                Me._ItemShader = value
                myListItem = Me._ItemShader
                If myListItem IsNot Nothing Then
                    AddHandler myListItem.Check, checkEventHandler
                End If
            End Set
        End Property

        Public PageID As FormMain.PageSubType
    End Class
End Namespace
