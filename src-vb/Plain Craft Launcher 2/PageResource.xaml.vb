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
Imports Microsoft.VisualBasic.CompilerServices
Imports PCLCS

Namespace PCL
    <ContentProperty("SearchTags")>
    <DesignerGenerated()>
    Public Partial Class PageResource
        Inherits MyScrollViewer

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageResource_Loaded
            Me._TypeName = ""
            Me._TypeNameSpaced = ""
            Me._Type = CType((-1), ResourceTypes)
            Me.Loader = ResourceSearcher.GetLoader(AddressOf Me.LoaderInput)
            Me.Storage = New ResourceSearcher.SearchResult()
            Me.Page = 0
            Me.InitializeComponent()
        End Sub

        Public ReadOnly Property SearchTags As ItemCollection
            Get
                Return Me.ComboSearchTag.Items
            End Get
        End Property

        Public Property TypeName As String
            Get
                Return Me._TypeName
            End Get
            Set(value As String)
                If Operators.CompareString(Me._TypeName, value, False) <> 0 Then
                    Me._TypeName = value
                    Me.Loader.Name = String.Format("社区资源获取：{0}", value)
                End If
            End Set
        End Property

        Public Property TypeNameSpaced As String
            Get
                Return Me._TypeNameSpaced
            End Get
            Set(value As String)
                If Operators.CompareString(Me._TypeNameSpaced, value, False) <> 0 Then
                    Me._TypeNameSpaced = value
                    Me.PanAlways.Title = String.Format("搜索{0}", value)
                    Me.Load.Text = String.Format("正在获取{0}列表", value)
                End If
            End Set
        End Property

        Public Property PageType As ResourceTypes
            Get
                Return Me._Type
            End Get
            Set(value As ResourceTypes)
                If Me._Type <> value Then
                    Me._Type = value
                    Me.BtnSearchInstallModPack.Visibility = If((value = ResourceTypes.ModPack), Visibility.Visible, Visibility.Collapsed)
                End If
            End Set
        End Property

        Private Sub PageResource_Loaded(sender As Object, e As EventArgs)
            If Me.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init Is Nothing Then
                Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init, New StaticLocalInitFlag(), Nothing)
            End If
            Dim flag As Boolean = False
            Try
                Monitor.Enter(Me.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init, flag)
                If Me.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init.State = 0S Then
                    Me.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init.State = 2S
                    Me.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded = False
                ElseIf Me.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init.State = 2S Then
                    Throw New IncompleteInitialization()
                End If
            Finally
                Me.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init.State = 1S
                If flag Then
                    Monitor.[Exit](Me.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init)
                End If
            End Try
            If PageResource.TargetInstance IsNot Nothing Then
                Me.ResetFilter()
                Me.TextSearchVersion.Text = PageResource.TargetInstance.Version.VanillaName
                Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_12(Of String, MyComboBoxItem) = Function(Name As String)
                    Try
                        For Each obj As Object In CType(Me.ComboSearchLoader.Items, IEnumerable)
                            Dim myComboBoxItem As MyComboBoxItem = CType(obj, MyComboBoxItem)
                            If Operators.ConditionalCompareObjectEqual(myComboBoxItem.Content, Name, False) Then
                                Return myComboBoxItem
                            End If
                        Next
                    Finally
                        Dim enumerator As IEnumerator
                        If TypeOf enumerator Is IDisposable Then
                            TryCast(enumerator, IDisposable).Dispose()
                        End If
                    End Try
                    Return CType(Me.ComboSearchLoader.Items(0), MyComboBoxItem)
                End Function
                If PageResource.TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.Forge) Then
                    Me.ComboSearchLoader.SelectedItem = vb$AnonymousDelegate_("Forge")
                ElseIf PageResource.TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.Fabric) Then
                    Me.ComboSearchLoader.SelectedItem = vb$AnonymousDelegate_("Fabric")
                ElseIf PageResource.TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge) Then
                    Me.ComboSearchLoader.SelectedItem = vb$AnonymousDelegate_("NeoForge")
                End If
                PageResource.TargetInstance = Nothing
                If PageResource.TargetName IsNot Nothing Then
                    Me.TextSearchName.Text = PageResource.TargetName
                    PageResource.TargetName = Nothing
                End If
                If Me.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded Then
                    Me.StartNewSearch()
                End If
                MyBase.ScrollToHome()
            End If
            If Not Me.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded Then
                Me.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded = True
                CType(MyBase.Parent, MyPageRight).PageLoaderInit(Of ResourceSearcher.SearchRequest, Integer)(Me.Load, Me.PanLoad, Me.PanContent, Me.PanAlways, Me.Loader, Sub(a0 As ModLoader.LoaderTask(Of ResourceSearcher.SearchRequest, Integer))
                    Me.Load_OnFinish()
                End Sub, AddressOf Me.LoaderInput, True)
                If ModDownload.AllDrops IsNot Nothing AndAlso ModDownload.AllDrops.First() > 250 Then
                    Dim text As String = McVersion.DropToVersion(ModDownload.AllDrops.First())
                    If Operators.CompareString(CType(Me.TextSearchVersion.Items(1), MyComboBoxItem).Content.ToString(), text, False) <> 0 Then
                        Me.TextSearchVersion.Items.Insert(1, New MyComboBoxItem() With { .Content = text })
                    End If
                End If
            End If
        End Sub

        Private Function LoaderInput() As ResourceSearcher.SearchRequest
            ' The following expression was wrapped in a checked-statement
            Dim searchRequest As ResourceSearcher.SearchRequest = New ResourceSearcher.SearchRequest(Me.PageType, Me.Storage, (Me.Page + 1) * 40)
            Dim text As String = If((Operators.CompareString(Me.TextSearchVersion.Text, "全部 (也可自行输入)", False) = 0), Nothing, If((Me.TextSearchVersion.Text.Contains(".") OrElse Me.TextSearchVersion.Text.Contains("w")), Me.TextSearchVersion.Text, Nothing))
            searchRequest.SearchText = Me.TextSearchName.Text
            searchRequest.GameVersion = text
            searchRequest.Tag = Conversions.ToString(NewLateBinding.LateGet(Me.ComboSearchTag.SelectedItem, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing))
            searchRequest.ModLoaders = CType(Math.Round(If((Me.PageType = ResourceTypes.[Mod]), ModBase.Val(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(Me.ComboSearchLoader.SelectedItem, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing))), 0.0)), ModLoaders)
            searchRequest.Sources = CType(Math.Round(ModBase.Val(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(Me.ComboSearchSource.SelectedItem, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing)))), ResourcePlatforms)
            Return searchRequest
        End Function

        Private Sub Load_OnFinish()
            ' The following expression was wrapped in a checked-statement
            Try
                Logger.Info(String.Format("开始可视化{0}列表，已储藏 {1} 个结果，当前在第 {2} 页", Me.TypeNameSpaced, Me.Storage.Results.Count, Me.Page + 1), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageDownload\Resource\PageResource.xaml.vb")
                Me.PanProjects.Children.Clear()
                Dim num As Integer = Math.Min(Me.Page * 40, Me.Storage.Results.Count - 1)
                Try
                    For Each resourceProject As ResourceProject In Me.Storage.Results.GetRange(num, Math.Min(Me.Storage.Results.Count - num, 40))
                        Me.PanProjects.Children.Add(resourceProject.ToResourceItem(Me.Loader.Input.GameVersion = Nothing, Me.Loader.Input.ModLoaders = ModLoaders.None AndAlso (Me.PageType = ResourceTypes.[Mod] OrElse Me.PageType = ResourceTypes.ModPack)))
                    Next
                Finally
                    Dim enumerator As List(Of ResourceProject).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                Me.CardPages.Visibility = If((Me.Storage.Results.Count > 40 OrElse Me.Storage.CurseForgeOffset < Me.Storage.CurseForgeTotal OrElse Me.Storage.ModrinthOffset < Me.Storage.ModrinthTotal), Visibility.Visible, Visibility.Collapsed)
                Me.LabPage.Text = Conversions.ToString(Me.Page + 1)
                Me.BtnPageFirst.IsEnabled = Me.Page > 1
                Me.BtnPageFirst.Opacity = If((Me.Page > 1), 1.0, 0.2)
                Me.BtnPageLeft.IsEnabled = Me.Page > 0
                Me.BtnPageLeft.Opacity = If((Me.Page > 0), 1.0, 0.2)
                Dim flag As Boolean = Me.Storage.Results.Count > 40 * (Me.Page + 1) OrElse Me.Storage.CurseForgeOffset < Me.Storage.CurseForgeTotal OrElse Me.Storage.ModrinthOffset < Me.Storage.ModrinthTotal
                Me.BtnPageRight.IsEnabled = flag
                Me.BtnPageRight.Opacity = If(flag, 1.0, 0.2)
                If Me.Storage.ErrorMessage Is Nothing Then
                    Me.HintError.Visibility = Visibility.Collapsed
                Else
                    Me.HintError.Visibility = Visibility.Visible
                    Me.HintError.Text = Me.Storage.ErrorMessage
                End If
                MyBase.ScrollToTop()
            Catch ex As Exception
                Logger.[Error](ex, String.Format("可视化{0}列表出错", Me.TypeNameSpaced), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageDownload\Resource\PageResource.xaml.vb")
            End Try
        End Sub

        Private Sub Load_State(sender As Object, state As MyLoading.MyLoadingState, oldState As MyLoading.MyLoadingState)
            Dim state2 As ModBase.LoadState = Me.Loader.State
            If state2 = ModBase.LoadState.Failed Then
                Dim text As String = ""
                If Me.Loader.[Error] IsNot Nothing Then
                    text = Me.Loader.[Error].Message
                End If
                If text.Contains("不是有效的 json 文件") Then
                    Logger.Warn(String.Format("下载的{0}列表 json 文件损坏，已自动重试", Me.TypeNameSpaced), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageDownload\Resource\PageResource.xaml.vb")
                    CType(MyBase.Parent, MyPageRight).PageLoaderRestart(Nothing, True)
                End If
            End If
        End Sub

        Private Sub BtnPageFirst_Click(sender As Object, e As RoutedEventArgs)
            Me.ChangePage(0)
        End Sub

        Private Sub BtnPageLeft_Click(sender As Object, e As RoutedEventArgs)
            ' The following expression was wrapped in a checked-expression
            Me.ChangePage(Me.Page - 1)
        End Sub

        Private Sub BtnPageRight_Click(sender As Object, e As RoutedEventArgs)
            ' The following expression was wrapped in a checked-expression
            Me.ChangePage(Me.Page + 1)
        End Sub

        Private Sub ChangePage(NewPage As Integer)
            Me.CardPages.IsEnabled = False
            Me.Page = NewPage
            ModMain.FrmMain.BackToTop()
            Logger.Info(String.Format("{0}：切换到第 {1} 页", Me.TypeName, Me.Page + 1), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageDownload\Resource\PageResource.xaml.vb")
            ModBase.RunInThread(Sub()
                Thread.Sleep(100)
                ModBase.RunInUi(Sub()
                    Me.CardPages.IsEnabled = True
                End Sub, False)
                Me.Loader.Start(Nothing, False)
            End Sub)
        End Sub

        Private Sub StartNewSearch()
            Me.Page = 0
            Dim loader As ModLoader.LoaderTask(Of ResourceSearcher.SearchRequest, Integer) = Me.Loader
            Dim searchRequest As ResourceSearcher.SearchRequest = Me.LoaderInput()
            If loader.ShouldStart(searchRequest, False, False) Then
                Me.Storage = New ResourceSearcher.SearchResult()
            End If
            Me.Loader.Start(Nothing, False)
        End Sub

        Private Sub EnterTrigger(sender As Object, e As KeyEventArgs)
            If e.Key = Key.[Return] Then
                Me.StartNewSearch()
            End If
        End Sub

        Private Sub ResetFilter()
            Me.TextSearchName.Text = ""
            Me.TextSearchVersion.Text = "全部 (也可自行输入)"
            Me.TextSearchVersion.SelectedIndex = 0
            Me.ComboSearchSource.SelectedIndex = 0
            Me.ComboSearchTag.SelectedIndex = 0
            Me.ComboSearchLoader.SelectedIndex = 0
            Me.Loader.LastFinishedTime = 0L
        End Sub

        Private Sub TextSearchVersion_TextChanged()
            If Not Me.TextSearchVersion.IsDropDownOpen Then
                Me.UpdateSearchLoaderVisibility()
            End If
        End Sub

        Private Sub UpdateSearchLoaderVisibility()
            If Me.PageType = ResourceTypes.[Mod] AndAlso (Me.TextSearchVersion.Text.Contains(".") OrElse Me.TextSearchVersion.Text.Contains("w")) Then
                Me.ComboSearchLoader.Visibility = Visibility.Visible
                Grid.SetColumnSpan(Me.TextSearchVersion, 1)
                Return
            End If
            Me.ComboSearchLoader.Visibility = Visibility.Collapsed
            Grid.SetColumnSpan(Me.TextSearchVersion, 2)
            Me.ComboSearchLoader.SelectedIndex = 0
        End Sub

        Private Sub BtnSearchInstallModPack_Click(sender As Object, e As EventArgs)
            ModModpack.ModpackInstall()
        End Sub

        Friend Overridable Property PanAlways As MyCard

        Friend Overridable Property TextSearchName As MyTextBox
            <CompilerGenerated()>
            Get
                Return Me._TextSearchName
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyTextBox)
                Dim keyEventHandler As KeyEventHandler = AddressOf Me.EnterTrigger
                Dim myTextBox As MyTextBox = Me._TextSearchName
                If myTextBox IsNot Nothing Then
                    RemoveHandler myTextBox.KeyDown, keyEventHandler
                End If
                Me._TextSearchName = value
                myTextBox = Me._TextSearchName
                If myTextBox IsNot Nothing Then
                    AddHandler myTextBox.KeyDown, keyEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ComboSearchSource As MyComboBox

        Friend Overridable Property TextSearchVersion As MyComboBox
            <CompilerGenerated()>
            Get
                Return Me._TextSearchVersion
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyComboBox)
                Dim keyEventHandler As KeyEventHandler = AddressOf Me.EnterTrigger
                Dim textChangedEventHandler As MyComboBox.TextChangedEventHandler = Sub(a0 As Object, a1 As TextChangedEventArgs)
                    Me.TextSearchVersion_TextChanged()
                End Sub
                Dim eventHandler As EventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.UpdateSearchLoaderVisibility()
                End Sub
                Dim myComboBox As MyComboBox = Me._TextSearchVersion
                If myComboBox IsNot Nothing Then
                    RemoveHandler myComboBox.KeyDown, keyEventHandler
                    RemoveHandler myComboBox.TextChanged, textChangedEventHandler
                    RemoveHandler myComboBox.DropDownClosed, eventHandler
                End If
                Me._TextSearchVersion = value
                myComboBox = Me._TextSearchVersion
                If myComboBox IsNot Nothing Then
                    AddHandler myComboBox.KeyDown, keyEventHandler
                    AddHandler myComboBox.TextChanged, textChangedEventHandler
                    AddHandler myComboBox.DropDownClosed, eventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ComboSearchLoader As MyComboBox

        Friend Overridable Property ComboSearchTag As MyComboBox

        Friend Overridable Property BtnSearchRun As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSearchRun
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.StartNewSearch()
                End Sub
                Dim myButton As MyButton = Me._BtnSearchRun
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnSearchRun = value
                myButton = Me._BtnSearchRun
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnSearchReset As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSearchReset
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.ResetFilter()
                End Sub
                Dim myButton As MyButton = Me._BtnSearchReset
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnSearchReset = value
                myButton = Me._BtnSearchReset
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnSearchInstallModPack As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSearchInstallModPack
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnSearchInstallModPack_Click
                Dim myButton As MyButton = Me._BtnSearchInstallModPack
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnSearchInstallModPack = value
                myButton = Me._BtnSearchInstallModPack
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanContent As StackPanel

        Friend Overridable Property HintError As MyHint

        Friend Overridable Property CardProjects As MyCard

        Friend Overridable Property PanProjects As StackPanel

        Friend Overridable Property CardPages As MyCard

        Friend Overridable Property BtnPageFirst As MyIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnPageFirst
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.BtnPageFirst_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                Dim myIconButton As MyIconButton = Me._BtnPageFirst
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnPageFirst = value
                myIconButton = Me._BtnPageFirst
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnPageLeft As MyIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnPageLeft
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.BtnPageLeft_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                Dim myIconButton As MyIconButton = Me._BtnPageLeft
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnPageLeft = value
                myIconButton = Me._BtnPageLeft
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property LabPage As TextBlock

        Friend Overridable Property BtnPageRight As MyIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnPageRight
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.BtnPageRight_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                Dim myIconButton As MyIconButton = Me._BtnPageRight
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnPageRight = value
                myIconButton = Me._BtnPageRight
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanLoad As MyCard

        Friend Overridable Property Load As MyLoading
            <CompilerGenerated()>
            Get
                Return Me._Load
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyLoading)
                Dim stateChangedEventHandler As MyLoading.StateChangedEventHandler = AddressOf Me.Load_State
                Dim myLoading As MyLoading = Me._Load
                If myLoading IsNot Nothing Then
                    RemoveHandler myLoading.StateChanged, stateChangedEventHandler
                End If
                Me._Load = value
                myLoading = Me._Load
                If myLoading IsNot Nothing Then
                    AddHandler myLoading.StateChanged, stateChangedEventHandler
                End If
            End Set
        End Property

        Private _TypeName As String

        Private _TypeNameSpaced As String

        Private _Type As ResourceTypes

        Public Shared TargetInstance As ModMinecraft.McInstance = Nothing

        Public Shared TargetName As String = Nothing

        Public Loader As ModLoader.LoaderTask(Of ResourceSearcher.SearchRequest, Integer)

        Public Storage As ResourceSearcher.SearchResult

        Public Const PAGE_SIZE As Integer = 40

        Public Page As Integer

        Private $STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded As Boolean

        Private $STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init As StaticLocalInitFlag
    End Class
End Namespace
