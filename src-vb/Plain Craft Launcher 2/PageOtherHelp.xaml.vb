Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Markup
Imports System.Windows.Media
Imports MeloongCore
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageOtherHelp
        Inherits MyPageRight
        Implements IRefreshable

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageOther_Loaded
            AddHandler MyBase.Initialized, AddressOf Me.PageOther_Inited
            Me.InitializeComponent()
        End Sub

        Private Sub PageOther_Loaded(sender As Object, e As RoutedEventArgs)
            Me.PanBack.ScrollToHome()
        End Sub

        Private Sub PageOther_Inited(sender As Object, e As EventArgs)
            MyBase.PageLoaderInit(Of Integer, List(Of ModMain.HelpEntry))(Me.Load, Me.PanLoad, Me.PanBack, Nothing, ModMain.HelpLoader, AddressOf Me.HelpListLoad, Nothing, True)
        End Sub

        Private Sub HelpListLoad(Loader As ModLoader.LoaderTask(Of Integer, List(Of ModMain.HelpEntry)))
            Try
                Me.PanList.Children.Clear()
                Me.PanBack.ScrollToHome()
                Dim output As List(Of ModMain.HelpEntry) = Loader.Output
                Dim list As List(Of String) = New List(Of String)()
                Try
                    For Each helpEntry As ModMain.HelpEntry In output
                        If helpEntry.ShowInSnapshot Then
                            Try
                                For Each text As String In helpEntry.Types
                                    If Not list.Contains(text) Then
                                        list.Add(text)
                                    End If
                                Next
                            Finally
                                Dim enumerator2 As List(Of String).Enumerator
                                CType(enumerator2, IDisposable).Dispose()
                            End Try
                        End If
                    Next
                Finally
                    Dim enumerator As List(Of ModMain.HelpEntry).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                If list.Contains("指南") Then
                    list.Remove("指南")
                    list.Insert(0, "指南")
                End If
                Try
                    For Each text2 As String In list
                        Dim list2 As List(Of ModMain.HelpEntry) = New List(Of ModMain.HelpEntry)()
                        Try
                            For Each helpEntry2 As ModMain.HelpEntry In output
                                If helpEntry2.ShowInSnapshot AndAlso helpEntry2.Types.Contains(text2) Then
                                    list2.Add(helpEntry2)
                                End If
                            Next
                        Finally
                            Dim enumerator4 As List(Of ModMain.HelpEntry).Enumerator
                            CType(enumerator4, IDisposable).Dispose()
                        End Try
                        Dim myCard As MyCard = New MyCard() With { .Title = text2, .Margin = New Thickness(0.0, 0.0, 0.0, 15.0) }
                        Dim stackPanel As StackPanel = New StackPanel() With { .Margin = New Thickness(20.0, 40.0, 18.0, 0.0), .VerticalAlignment = VerticalAlignment.Top, .RenderTransform = New TranslateTransform(0.0, 0.0) }
                        myCard.Children.Add(stackPanel)
                        myCard.SwapControl = stackPanel
                        Dim myCard2 As MyCard = myCard
                        Dim enumerable As IEnumerable(Of ModMain.HelpEntry) = list2
                        Dim func As Func(Of ModMain.HelpEntry, UIElement)
                        If PageOtherHelp._Closure$__.$I3-0 IsNot Nothing Then
                            func = PageOtherHelp._Closure$__.$I3-0
                        Else
                            Dim func2 As Func(Of ModMain.HelpEntry, UIElement) = Function(Entry As ModMain.HelpEntry) Entry.ToListItem()
                            func = func2
                            PageOtherHelp._Closure$__.$I3-0 = func2
                        End If
                        myCard2.SetContentBuilder(Of ModMain.HelpEntry)(enumerable, func)
                        If Operators.CompareString(text2, "指南", False) = 0 Then
                            myCard.BuildContent()
                        Else
                            myCard.IsSwapped = True
                        End If
                        Me.PanList.Children.Add(myCard)
                    Next
                Finally
                    Dim enumerator3 As List(Of String).Enumerator
                    CType(enumerator3, IDisposable).Dispose()
                End Try
            Catch ex As Exception
                Logger.[Error](ex, "加载帮助列表 UI 失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageOther\PageOtherHelp.xaml.vb")
            End Try
        End Sub

        Public Shared Sub OnItemClick(Entry As ModMain.HelpEntry)
            Try
                If Entry.IsEvent Then
                    CustomEvent.Raise(Entry.EventType, Entry.EventData)
                Else
                    PageOtherHelp.EnterHelpPage(Entry)
                End If
            Catch ex As Exception
                Logger.[Error](ex, "处理帮助项目点击时发生意外错误", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageOther\PageOtherHelp.xaml.vb")
            End Try
        End Sub

        Public Shared Sub EnterHelpPage(Location As String)
            ModBase.RunInThread(Sub()
                If ModMain.HelpLoader.State <> ModBase.LoadState.Finished Then
                    ModMain.HelpLoader.WaitForExit(ModBase.GetUuid(), Nothing, False, Nothing, Nothing)
                End If
                Dim Entry As ModMain.HelpEntry = New ModMain.HelpEntry(Location)
                ModBase.RunInUi(Sub()
                    Dim pageOtherHelpDetail As PageOtherHelpDetail = New PageOtherHelpDetail()
                    If pageOtherHelpDetail.Init(Entry) Then
                        ModMain.FrmMain.PageChange(New FormMain.PageStackData() With { .Page = FormMain.PageType.HelpDetail, .Additional = New Object() { Entry, pageOtherHelpDetail } }, FormMain.PageSubType.[Default])
                        Return
                    End If
                    Logger.Warn("已取消进入帮助项目，这一般是由于 xaml 初始化失败，且用户在弹窗中手动放弃", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageOther\PageOtherHelp.xaml.vb")
                End Sub, False)
            End Sub)
        End Sub

        Public Shared Sub EnterHelpPage(Entry As ModMain.HelpEntry)
            Dim $I1 As Action
            ModBase.RunInThread(Sub()
                If ModMain.HelpLoader.State <> ModBase.LoadState.Finished Then
                    ModMain.HelpLoader.WaitForExit(ModBase.GetUuid(), Nothing, False, Nothing, Nothing)
                End If
                Dim action As Action
                If $I1 IsNot Nothing Then
                    action = $I1
                Else
                    Dim action2 As Action = Sub()
                        Dim pageOtherHelpDetail As PageOtherHelpDetail = New PageOtherHelpDetail()
                        If pageOtherHelpDetail.Init(Entry) Then
                            ModMain.FrmMain.PageChange(New FormMain.PageStackData() With { .Page = FormMain.PageType.HelpDetail, .Additional = New Object() { Entry, pageOtherHelpDetail } }, FormMain.PageSubType.[Default])
                            Return
                        End If
                        Logger.Warn("已取消进入帮助项目，这一般是由于 xaml 初始化失败，且用户在弹窗中手动放弃", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageOther\PageOtherHelp.xaml.vb")
                    End Sub
                    Dim action3 As Action = action2
                    $I1 = action2
                    action = action3
                End If
                ModBase.RunInUi(action, False)
            End Sub)
        End Sub

        Public Sub SearchRun()
            If String.IsNullOrWhiteSpace(Me.SearchBox.Text) Then
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.PanSearch, -Me.PanSearch.Opacity, 100, 0, Nothing, False), ModAnimation.AaCode(Sub()
                    Me.PanSearch.Height = 0.0
                    Me.PanSearch.Visibility = Visibility.Collapsed
                    Me.PanList.Visibility = Visibility.Visible
                End Sub, 0, True), ModAnimation.AaOpacity(Me.PanList, 1.0 - Me.PanList.Opacity, 150, 30, Nothing, False) }, "FrmOtherHelp Search Switch", False)
                Return
            End If
            Dim list As List(Of ModBase.SearchEntry(Of ModMain.HelpEntry)) = New List(Of ModBase.SearchEntry(Of ModMain.HelpEntry))()
            Try
                For Each helpEntry As ModMain.HelpEntry In ModMain.HelpLoader.Output
                    If helpEntry.ShowInSearch AndAlso helpEntry.ShowInSearch AndAlso helpEntry.ShowInSnapshot Then
                        list.Add(New ModBase.SearchEntry(Of ModMain.HelpEntry)() With { .Item = helpEntry, .SearchSource = New List(Of ModBase.SearchSource)() From { New ModBase.SearchSource(helpEntry.Title, 1.0), New ModBase.SearchSource(helpEntry.Desc, 0.5), New ModBase.SearchSource(helpEntry.Search, 1.5) } })
                    End If
                Next
            Finally
                Dim enumerator As List(Of ModMain.HelpEntry).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            Dim list2 As List(Of ModBase.SearchEntry(Of ModMain.HelpEntry)) = ModBase.Search(Of ModMain.HelpEntry)(list, Me.SearchBox.Text, 5, 0.08)
            Me.PanSearchList.Children.Clear()
            If Not list2.Any() Then
                Me.PanSearch.Title = "无搜索结果"
                Me.PanSearchList.Visibility = Visibility.Collapsed
            Else
                Me.PanSearch.Title = "搜索结果"
                Try
                    For Each searchEntry As ModBase.SearchEntry(Of ModMain.HelpEntry) In list2
                        Dim myListItem As MyListItem = searchEntry.Item.ToListItem()
                        If ModBase.ModeDebug Then
                            myListItem.Info = String.Concat(New String() { If(searchEntry.AbsoluteRight, "完全匹配，", ""), "相似度：", Conversions.ToString(Math.Round(searchEntry.Similarity, 3)), "，", myListItem.Info })
                        End If
                        Me.PanSearchList.Children.Add(myListItem)
                    Next
                Finally
                    Dim enumerator2 As List(Of ModBase.SearchEntry(Of ModMain.HelpEntry)).Enumerator
                    CType(enumerator2, IDisposable).Dispose()
                End Try
                Me.PanSearchList.Visibility = Visibility.Visible
            End If
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.PanList, -Me.PanList.Opacity, 100, 0, Nothing, False), ModAnimation.AaCode(Sub()
                Me.PanList.Visibility = Visibility.Collapsed
                Me.PanSearch.Visibility = Visibility.Visible
                Me.PanSearch.TriggerForceResize()
            End Sub, 0, True), ModAnimation.AaOpacity(Me.PanSearch, 1.0 - Me.PanSearch.Opacity, 150, 30, Nothing, False) }, "FrmOtherHelp Search Switch", False)
        End Sub

        Public Sub Refresh() Implements PCL.IRefreshable.Refresh
            PageOtherLeft.RefreshHelp()
        End Sub

        Friend Overridable Property PanBack As MyScrollViewer

        Friend Overridable Property PanMain As StackPanel

        Friend Overridable Property SearchBox As MySearchBox
            <CompilerGenerated()>
            Get
                Return Me._SearchBox
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MySearchBox)
                Dim textChangedEventHandler As MySearchBox.TextChangedEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.SearchRun()
                End Sub
                Dim mySearchBox As MySearchBox = Me._SearchBox
                If mySearchBox IsNot Nothing Then
                    RemoveHandler mySearchBox.TextChanged, textChangedEventHandler
                End If
                Me._SearchBox = value
                mySearchBox = Me._SearchBox
                If mySearchBox IsNot Nothing Then
                    AddHandler mySearchBox.TextChanged, textChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanSearch As MyCard

        Friend Overridable Property PanSearchList As StackPanel

        Friend Overridable Property PanList As StackPanel

        Friend Overridable Property PanLoad As MyCard

        Friend Overridable Property Load As MyLoading
    End Class
End Namespace
