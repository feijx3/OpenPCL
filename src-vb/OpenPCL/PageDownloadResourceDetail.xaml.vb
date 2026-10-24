Imports System
Imports System.CodeDom.Compiler
Imports System.Collections
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
Imports System.Windows.Media
Imports MeloongCore
Imports MeloongCore.Extensions
Imports MeloongCore.Wpf
Imports Microsoft.VisualBasic.CompilerServices
Imports PCLCS

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageDownloadResourceDetail
        Inherits OurPageRight

        Public Sub New()
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.LoadPageArguments()
            End Sub
            AddHandler MyBase.Initialized, AddressOf Me.PageDownloadResourceDetail_Inited
            AddHandler MyBase.PageEnter, AddressOf Me.Init
            Me.ResourceItem = Nothing
            Me.ResourceFileLoader = New ModLoader.LoaderTask(Of Integer, List(Of ResourceVersion))("Resource File", Sub(Task As ModLoader.LoaderTask(Of Integer, List(Of ResourceVersion)))
                Me.LoadPageArguments()
                Dim list As List(Of ResourceVersion) = ResourceVersion.FromProjectId(Me.Project.Id, Me.Project.Platform, True)
                If Not Task.IsCanceled Then
                    Task.Output = list
                End If
            End Sub, Nothing, ThreadPriority.Normal)
            Me.IsFirstInit = True
            Me.InitializeComponent()
        End Sub

        Public Sub LoadPageArguments()
            Me.Project = CType(NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, New Object() { 0 }, Nothing), ResourceProject)
            Me.TargetVersion = Conversions.ToString(NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, New Object() { 2 }, Nothing))
            Me.TargetLoaders = CType(Conversions.ToInteger(NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, New Object() { 3 }, Nothing)), ModLoaders)
            If Me.TargetLoaders <> ModLoaders.None AndAlso Not Me.TargetLoaders.Flags().IsSingle() Then
                Me.TargetLoaders = Me.TargetLoaders.Flags().First()
            End If
            Me.TargetResourceType = CType(Conversions.ToInteger(NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, New Object() { 4 }, Nothing)), ResourceTypes)
        End Sub

        Private Sub PageDownloadResourceDetail_Inited(sender As Object, e As EventArgs)
            Me.LoadPageArguments()
            MyBase.PageLoaderInit(Of Integer, List(Of ResourceVersion))(Me.Load, Me.PanLoad, Me.PanMain, Me.CardIntro, Me.ResourceFileLoader, Sub(a0 As ModLoader.LoaderTask(Of Integer, List(Of ResourceVersion)))
                Me.Load_OnFinish()
            End Sub, Nothing, True)
        End Sub

        Private Sub Load_State(sender As Object, state As OurLoading.OurLoadingState, oldState As OurLoading.OurLoadingState)
            Dim state2 As ModBase.LoadState = Me.ResourceFileLoader.State
            If state2 = ModBase.LoadState.Failed Then
                Dim text As String = ""
                If Me.ResourceFileLoader.[Error] IsNot Nothing Then
                    text = Me.ResourceFileLoader.[Error].Message
                End If
                If text.Contains("不是有效的 json 文件") Then
                    Logger.Warn("下载的文件 json 列表损坏，已自动重试", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\Resource\PageDownloadCompDetail.xaml.vb")
                    MyBase.PageLoaderRestart(Nothing, True)
                End If
            End If
        End Sub

        Private Function GetResults() As List(Of ResourceVersion)
            Dim list As List(Of ResourceVersion) = Me.ResourceFileLoader.Output
            Dim targetResourceType As ResourceTypes = Me.TargetResourceType
            If targetResourceType <> ResourceTypes.ResourcePack AndAlso targetResourceType <> ResourceTypes.Shader Then
                If targetResourceType = ResourceTypes.Any Then
                    Dim enumerable As IEnumerable(Of ResourceVersion) = list
                    Dim func As Func(Of ResourceVersion, Boolean)
                    If PageDownloadResourceDetail._Closure$__.$I15-0 IsNot Nothing Then
                        func = PageDownloadResourceDetail._Closure$__.$I15-0
                    Else
                        Dim func2 As Func(Of ResourceVersion, Boolean) = Function(r As ResourceVersion) r.ResourceType <> ResourceTypes.Plugin
                        func = func2
                        PageDownloadResourceDetail._Closure$__.$I15-0 = func2
                    End If
                    list = enumerable.Where(func).ToList()
                Else
                    list = list.Where(Function(r As ResourceVersion) r.ResourceType = Me.TargetResourceType).ToList()
                End If
            End If
            Return list
        End Function

        Private Sub Load_OnFinish()
            Dim CS$<>8__locals1 As PageDownloadResourceDetail._Closure$__16-0 = New PageDownloadResourceDetail._Closure$__16-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Me = Me
            CS$<>8__locals1.$VB$Local_Results = Me.GetResults()
            CS$<>8__locals1.$VB$Local_Filters = Nothing
            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_10 = Sub()
                Dim $VB$Local_Results As IEnumerable(Of ResourceVersion) = CS$<>8__locals1.$VB$Local_Results
                Dim func As Func(Of ResourceVersion, IEnumerable(Of String))
                If PageDownloadResourceDetail._Closure$__.$I16-1 IsNot Nothing Then
                    func = PageDownloadResourceDetail._Closure$__.$I16-1
                Else
                    Dim func2 As Func(Of ResourceVersion, IEnumerable(Of String)) = Function(v As ResourceVersion) v.GameVersions
                    func = func2
                    PageDownloadResourceDetail._Closure$__.$I16-1 = func2
                End If
                Dim enumerable As IEnumerable(Of String) = $VB$Local_Results.SelectMany(func).[Select](Function(v As String) CS$<>8__locals1.$VB$Me.GetGroupedVersionName(v, CS$<>8__locals1.$VB$Me.GroupedDrop, CS$<>8__locals1.$VB$Me.GroupedOld)).Distinct()
                Dim func3 As Func(Of String, String)
                If PageDownloadResourceDetail._Closure$__.$I16-3 IsNot Nothing Then
                    func3 = PageDownloadResourceDetail._Closure$__.$I16-3
                Else
                    Dim func4 As Func(Of String, String) = Function(s As String) s
                    func3 = func4
                    PageDownloadResourceDetail._Closure$__.$I16-3 = func4
                End If
                Dim comparison As Comparison(Of String)
                If PageDownloadResourceDetail._Closure$__.$I16-4 IsNot Nothing Then
                    comparison = PageDownloadResourceDetail._Closure$__.$I16-4
                Else
                    Dim comparison2 As Comparison(Of String) = Function(x As String, y As String) McUtils.CompareVersion(x, y)
                    comparison = comparison2
                    PageDownloadResourceDetail._Closure$__.$I16-4 = comparison2
                End If
                CS$<>8__locals1.$VB$Local_Filters = enumerable.OrderByDescending(func3, Comparer(Of String).Create(comparison)).ToList()
            End Sub
            Me.GroupedDrop = False
            Me.GroupedOld = False
            vb$AnonymousDelegate_()
            If CS$<>8__locals1.$VB$Local_Filters.Count >= 9 Then
                Me.GroupedDrop = True
                Me.GroupedOld = False
                vb$AnonymousDelegate_()
                If CS$<>8__locals1.$VB$Local_Filters.Count >= 9 Then
                    Me.GroupedDrop = False
                    Me.GroupedOld = True
                    vb$AnonymousDelegate_()
                    If CS$<>8__locals1.$VB$Local_Filters.Count >= 9 Then
                        Me.GroupedDrop = True
                        Me.GroupedOld = True
                        vb$AnonymousDelegate_()
                    End If
                End If
            End If
            Me.PanFilter.Children.Clear()
            If CS$<>8__locals1.$VB$Local_Filters.Count < 2 Then
                Me.CardFilter.Visibility = Visibility.Collapsed
                Me.VersionFilter = Nothing
            Else
                Me.CardFilter.Visibility = Visibility.Visible
                CS$<>8__locals1.$VB$Local_Filters.Insert(0, "全部")
                Try
                    For Each text As String In CS$<>8__locals1.$VB$Local_Filters
                        Dim myRadioButton As OurRadioButton = New OurRadioButton() With { .Text = text, .Margin = New Thickness(2.0, 0.0, 2.0, 0.0), .ColorType = OurRadioButton.ColorState.Highlight }
                        myRadioButton.LabText.Margin = New Thickness(-2.0, 0.0, 10.0, 0.0)
                        AddHandler myRadioButton.Check, Sub(a0 As Object, a1 As Boolean)
                            Me._Lambda$__16-5(CType(a0, OurRadioButton), a1)
                        End Sub
                        Me.PanFilter.Children.Add(myRadioButton)
                    Next
                Finally
                    Dim enumerator As List(Of String).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                Dim myRadioButton2 As OurRadioButton = Nothing
                If Operators.CompareString(Me.TargetVersion, "", False) <> 0 AndAlso CS$<>8__locals1.$VB$Local_Results.FirstOrDefault(Function(v As ResourceVersion) v.GameVersions.Contains(Me.TargetVersion)) IsNot Nothing Then
                    Dim groupedVersionName As String = Me.GetGroupedVersionName(Me.TargetVersion, Me.GroupedDrop, Me.GroupedOld)
                    Try
                        For Each obj As Object In Me.PanFilter.Children
                            Dim myRadioButton3 As OurRadioButton = CType(obj, OurRadioButton)
                            If Operators.CompareString(myRadioButton3.Text, groupedVersionName, False) = 0 Then
                                myRadioButton2 = myRadioButton3
                                Exit For
                            End If
                        Next
                    Finally
                        Dim enumerator2 As IEnumerator
                        If TypeOf enumerator2 Is IDisposable Then
                            TryCast(enumerator2, IDisposable).Dispose()
                        End If
                    End Try
                End If
                If myRadioButton2 Is Nothing Then
                    myRadioButton2 = CType(Me.PanFilter.Children(0), OurRadioButton)
                End If
                myRadioButton2.Checked = True
            End If
            Me.UpdateFilterResult()
        End Sub

        Private Sub UpdateFilterResult()
            Dim results As List(Of ResourceVersion) = Me.GetResults()
            Dim text As String = If((Operators.CompareString(Me.TargetVersion, "", False) <> 0 OrElse Me.TargetLoaders <> ModLoaders.None), String.Format("{0}{1}（所选版本）", If((Me.TargetLoaders <> ModLoaders.None), (Me.TargetLoaders.ToString() + " "), ""), Me.TargetVersion), "")
            Dim sortedDictionary As SortedDictionary(Of String, List(Of ResourceVersion)) = New SortedDictionary(Of String, List(Of ResourceVersion))(New PageDownloadResourceDetail.CardSorter(text))
            sortedDictionary.Add("其他", New List(Of ResourceVersion)())
            Try
                For Each resourceVersion As ResourceVersion In results
                    Try
                        For Each text2 As String In resourceVersion.GameVersions
                            If Me.VersionFilter Is Nothing OrElse Operators.CompareString(Me.GetGroupedVersionName(text2, Me.GroupedDrop, Me.GroupedOld), Me.VersionFilter, False) = 0 Then
                                Dim groupedVersionName As String = Me.GetGroupedVersionName(text2, False, False)
                                Dim list As List(Of String) = New List(Of String)()
                                If Me.Project.ModLoaders.Flags().Count() > 1 AndAlso resourceVersion.ResourceType = ResourceTypes.[Mod] AndAlso McVersion.IsStandardFormat(groupedVersionName) Then
                                    Dim list2 As List(Of String) = list
                                    Dim enumerable As IEnumerable(Of ModLoaders) = resourceVersion.ModLoaders.Flags()
                                    Dim func As Func(Of ModLoaders, String)
                                    If PageDownloadResourceDetail._Closure$__.$I17-0 IsNot Nothing Then
                                        func = PageDownloadResourceDetail._Closure$__.$I17-0
                                    Else
                                        Dim func2 As Func(Of ModLoaders, String) = Function(f As ModLoaders) f.ToString() + " "
                                        func = func2
                                        PageDownloadResourceDetail._Closure$__.$I17-0 = func2
                                    End If
                                    list2.AddRange(enumerable.[Select](func))
                                End If
                                If Not list.Any() Then
                                    list.Add("")
                                End If
                                Try
                                    For Each text3 As String In list
                                        Dim text4 As String = text3 + groupedVersionName
                                        If Not sortedDictionary.ContainsKey(text4) Then
                                            sortedDictionary.Add(text4, New List(Of ResourceVersion)())
                                        End If
                                        If Not sortedDictionary(text4).Contains(resourceVersion) Then
                                            sortedDictionary(text4).Add(resourceVersion)
                                        End If
                                    Next
                                Finally
                                    Dim enumerator3 As List(Of String).Enumerator
                                    CType(enumerator3, IDisposable).Dispose()
                                End Try
                            End If
                        Next
                    Finally
                        Dim enumerator2 As List(Of String).Enumerator
                        CType(enumerator2, IDisposable).Dispose()
                    End Try
                Next
            Finally
                Dim enumerator As List(Of ResourceVersion).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            If Operators.CompareString(text, "", False) <> 0 AndAlso (Me.VersionFilter Is Nothing OrElse Me.GetGroupedVersionName(Me.TargetVersion, Me.GroupedDrop, Me.GroupedOld).StartsWithF(Me.VersionFilter, False)) Then
                sortedDictionary.Add(text, New List(Of ResourceVersion)())
                Try
                    For Each resourceVersion2 As ResourceVersion In results
                        If resourceVersion2.GameVersions.Contains(Me.TargetVersion) AndAlso (Me.TargetLoaders = ModLoaders.None OrElse Me.TargetLoaders.Flags().Intersect(resourceVersion2.ModLoaders.Flags()).Any()) AndAlso (Me.VersionFilter Is Nothing OrElse resourceVersion2.GameVersions.Any(Function(v As String) Operators.CompareString(Me.GetGroupedVersionName(v, Me.GroupedDrop, Me.GroupedOld), Me.VersionFilter, False) = 0)) AndAlso Not sortedDictionary(text).Contains(resourceVersion2) Then
                            sortedDictionary(text).Add(resourceVersion2)
                        End If
                    Next
                Finally
                    Dim enumerator4 As List(Of ResourceVersion).Enumerator
                    CType(enumerator4, IDisposable).Dispose()
                End Try
            End If
            Try
                Me.PanResults.Children.Clear()
                Try
                    For Each keyValuePair As KeyValuePair(Of String, List(Of ResourceVersion)) In sortedDictionary
                        If keyValuePair.Value.Any() AndAlso Operators.CompareString(keyValuePair.Key, text.Replace("（所选版本）", ""), False) <> 0 Then
                            Dim myCard As OurCard = New OurCard() With { .Title = keyValuePair.Key, .Margin = New Thickness(0.0, 0.0, 0.0, 15.0) }
                            Dim stackPanel As StackPanel = New StackPanel() With { .Margin = New Thickness(20.0, 40.0, 18.0, 0.0), .VerticalAlignment = VerticalAlignment.Top, .RenderTransform = New TranslateTransform(0.0, 0.0), .Tag = keyValuePair.Value }
                            myCard.Children.Add(stackPanel)
                            myCard.SwapControl = stackPanel
                            myCard.SetContentBuilder(Sub(a0 As Panel)
                                Me.InitializeStackResourceVersions(CType(a0, StackPanel))
                            End Sub)
                            Me.PanResults.Children.Add(myCard)
                            If Operators.CompareString(keyValuePair.Key, text, False) = 0 OrElse (ModMain.FrmMain.PageCurrent.Additional IsNot Nothing AndAlso CType(NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, New Object() { 1 }, Nothing), List(Of String)).Contains(myCard.Title)) Then
                                myCard.BuildContent()
                            Else
                                myCard.IsSwapped = True
                            End If
                            If Operators.CompareString(keyValuePair.Key, "其他", False) = 0 Then
                                stackPanel.Children.Add(New OurHint() With { .Text = "由于版本信息更新缓慢，可能无法识别刚更新的 MC 版本。几天后即可正常识别。", .Theme = OurHint.Themes.Yellow, .Margin = New Thickness(5.0, 0.0, 0.0, 8.0) })
                            End If
                        End If
                    Next
                Finally
                    Dim enumerator5 As SortedDictionary(Of String, List(Of ResourceVersion)).Enumerator
                    CType(enumerator5, IDisposable).Dispose()
                End Try
                If Me.PanResults.Children.Count = 1 Then
                    CType(Me.PanResults.Children(0), OurCard).IsSwapped = False
                End If
                If Me.Project.Types = ResourceTypes.ModOrDataPack AndAlso (Me.TargetResourceType = ResourceTypes.[Mod] OrElse Me.TargetResourceType = ResourceTypes.DataPack) Then
                    Me.HintAlternative.Visibility = Visibility.Visible
                    Me.HintAlternative.Text = If((Me.TargetResourceType = ResourceTypes.[Mod]), "以下是该项目的 Mod 版本。点击这里查看其数据包版本。", "以下是该项目的数据包版本。点击这里查看其 Mod 版本。")
                Else
                    Me.HintAlternative.Visibility = Visibility.Collapsed
                End If
            Catch ex As Exception
                Logger.[Error](ex, "可视化工程下载列表出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\Resource\PageDownloadCompDetail.xaml.vb")
            End Try
        End Sub

        Private Sub InitializeStackResourceVersions(Stack As StackPanel)
            Dim list As IList(Of ResourceVersion) = CType(Stack.Tag, List(Of ResourceVersion))
            Dim func As Func(Of ResourceVersion, ResourceVersion, Boolean)
            If PageDownloadResourceDetail._Closure$__.$I18-0 IsNot Nothing Then
                func = PageDownloadResourceDetail._Closure$__.$I18-0
            Else
                Dim func2 As Func(Of ResourceVersion, ResourceVersion, Boolean) = Function(a As ResourceVersion, b As ResourceVersion) DateTime.Compare(a.ReleaseDate, b.ReleaseDate) > 0
                func = func2
                PageDownloadResourceDetail._Closure$__.$I18-0 = func2
            End If
            Dim list2 As List(Of ResourceVersion) = list.SortByComparison(func)
            Dim enumerable As IEnumerable(Of ResourceVersion) = list2
            Dim func3 As Func(Of ResourceVersion, String)
            If PageDownloadResourceDetail._Closure$__.$I18-1 IsNot Nothing Then
                func3 = PageDownloadResourceDetail._Closure$__.$I18-1
            Else
                Dim func4 As Func(Of ResourceVersion, String) = Function(a As ResourceVersion) a.Display
                func3 = func4
                PageDownloadResourceDetail._Closure$__.$I18-1 = func4
            End If
            Dim flag As Boolean = enumerable.DistinctBy(func3).Count() <> list2.Count
            Dim list3 As List(Of String)
            If Me.TargetResourceType = ResourceTypes.ModPack Then
                list3 = New List(Of String)()
            Else
                Dim enumerable2 As IEnumerable(Of ResourceVersion) = list2
                Dim func5 As Func(Of ResourceVersion, IEnumerable(Of String))
                If PageDownloadResourceDetail._Closure$__.$I18-2 IsNot Nothing Then
                    func5 = PageDownloadResourceDetail._Closure$__.$I18-2
                Else
                    Dim func6 As Func(Of ResourceVersion, IEnumerable(Of String)) = Function(f As ResourceVersion) f.Dependencies
                    func5 = func6
                    PageDownloadResourceDetail._Closure$__.$I18-2 = func6
                End If
                list3 = enumerable2.SelectMany(func5).Distinct().Ordered().ToList()
            End If
            If list3.Any() Then
                Dim enumerable3 As IEnumerable(Of String) = list3
                Dim func7 As Func(Of String, Boolean)
                If PageDownloadResourceDetail._Closure$__.$I18-3 IsNot Nothing Then
                    func7 = PageDownloadResourceDetail._Closure$__.$I18-3
                Else
                    Dim func8 As Func(Of String, Boolean) = Function(d As String)
                        If Not ResourceProject.Cache.ContainsKey(d) Then
                            Logger.Warn(String.Format("未找到 ID {0} 的前置信息", d), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\Resource\PageDownloadCompDetail.xaml.vb")
                        End If
                        Return ResourceProject.Cache.ContainsKey(d)
                    End Function
                    func7 = func8
                    PageDownloadResourceDetail._Closure$__.$I18-3 = func8
                End If
                list3 = enumerable3.Where(func7).ToList()
                Stack.Children.Add(New TextBlock() With { .Text = "前置资源", .FontSize = 14.0, .HorizontalAlignment = HorizontalAlignment.Left, .Margin = New Thickness(6.0, 2.0, 0.0, 5.0) })
                Try
                    For Each text As String In list3
                        Dim myVirtualizingElement As OurVirtualizingElement(Of OurResourceItem) = ResourceProject.Cache(text).ToResourceItem(False, False)
                        Stack.Children.Add(myVirtualizingElement)
                    Next
                Finally
                    Dim enumerator As List(Of String).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                Stack.Children.Add(New TextBlock() With { .Text = "版本列表", .FontSize = 14.0, .HorizontalAlignment = HorizontalAlignment.Left, .Margin = New Thickness(6.0, 12.0, 0.0, 5.0) })
            End If
            Try
                For Each resourceVersion As ResourceVersion In list2
                    Stack.Children.Add(resourceVersion.ToListItem(AddressOf Me.Save_Click, Nothing, flag))
                Next
            Finally
                Dim enumerator2 As List(Of ResourceVersion).Enumerator
                CType(enumerator2, IDisposable).Dispose()
            End Try
            Stack.Children.Add(New FrameworkElement() With { .Height = 18.0 })
            Stack.Tag = Nothing
        End Sub

        Private Function GetGroupedVersionName(Name As String, GroupedByDrop As Boolean, FoldOld As Boolean) As String
            Dim num As Integer = McVersion.VersionToDrop(Name)
            Dim text As String
            If Name Is Nothing Then
                text = "其他"
            ElseIf Name.Contains("w") OrElse num = 209 Then
                text = "快照版"
            ElseIf Not McVersion.IsStandardFormat(Name) OrElse (FoldOld AndAlso num < 120) Then
                text = "远古版"
            ElseIf GroupedByDrop Then
                text = McVersion.DropToVersion(num)
            Else
                text = Name
            End If
            Return text
        End Function

        Public Sub Init()
            ' The following expression was wrapped in a checked-statement
            ModAnimation.AniControlEnabled += 1
            Me.Project = CType(NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, New Object() { 0 }, Nothing), ResourceProject)
            Me.PanBack.ScrollToHome()
            If Me.IsFirstInit Then
                Me.IsFirstInit = False
            Else
                MyBase.PageLoaderRestart(Nothing, True)
            End If
            If Me.ResourceItem IsNot Nothing Then
                Me.PanIntro.Children.Remove(Me.ResourceItem)
            End If
            Me.ResourceItem = Me.Project.ToResourceItem(True, True).Init()
            Me.ResourceItem.CanInteraction = False
            Me.ResourceItem.Margin = New Thickness(-7.0, -7.0, 0.0, 8.0)
            Me.PanIntro.Children.Insert(0, Me.ResourceItem)
            Me.BtnIntroWeb.Text = String.Format("转到 {0}", Me.Project.Platform)
            Me.BtnIntroWiki.Visibility = If((Me.Project.WikiId = 0), Visibility.Collapsed, Visibility.Visible)
            ModAnimation.AniControlEnabled -= 1
        End Sub

        Public Sub Install_Click(sender As OurListItem, e As EventArgs)
            Try
                Dim resourceVersion As ResourceVersion = CType(sender.Tag, ResourceVersion)
                Dim text As String = String.Format("{0} 整合包下载：{1} ", Me.Project.Platform, Me.Project.TranslatedName)
                Dim text2 As String = Me.Project.TranslatedName.Replace(".zip", "").Replace(".rar", "").Replace(".mrpack", "").Replace("\", "＼").Replace("/", "／").Replace("|", "｜").Replace(":", "：").Replace("<", "＜").Replace(">", "＞").Replace("*", "＊").Replace("?", "？").Replace("""", "").Replace("： ", "：")
                Dim validateFolderName As ValidateFolderName = New ValidateFolderName(ModMinecraft.McFolderSelected + "versions", True, Nothing)
                If Operators.CompareString(validateFolderName.Validate(text2), "", False) <> 0 Then
                    text2 = ""
                End If
                Dim InstanceName As String = ModMain.OurMsgBoxInput("输入版本名称", "", text2, New Collection(Of Validate)() From { validateFolderName }, "", "确定", "取消", False)
                If Not String.IsNullOrEmpty(InstanceName) Then
                    Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
                    Dim Target As String = String.Format("{0}versions\{1}\原始整合包.{2}", ModMinecraft.McFolderSelected, InstanceName, If((Me.Project.Platform = ResourcePlatforms.CurseForge), "zip", "mrpack"))
                    Dim LogoFileAddress As String = Me.ResourceItem.PathLogo.ActualSource
                    list.Add(New ModNet.LoaderDownload("下载整合包文件", New ModNet.NetFile() { resourceVersion.ToNetFile(Target, New ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.Standalone), resourceVersion.GameVersions.FirstOrDefault(), resourceVersion.ModLoaders) }) With { .ProgressWeight = 10.0, .Block = True })
                    list.Add(New ModLoader.LoaderTask(Of Integer, Integer)("准备安装整合包", Sub(a0 As ModLoader.LoaderTask(Of Integer, Integer))
                        MyBase._Lambda$__0()
                    End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.1 })
                    Dim loaderCombo As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)(text, list)
                    Dim action As Action(Of ModLoader.LoaderBase)
                    If PageDownloadResourceDetail._Closure$__.$I22-1 IsNot Nothing Then
                        action = PageDownloadResourceDetail._Closure$__.$I22-1
                    Else
                        Dim action2 As Action(Of ModLoader.LoaderBase) = Sub(OurLoader As ModLoader.LoaderBase)
                            Select Case OurLoader.State
                                Case ModBase.LoadState.Loading
                                    Return
                                Case ModBase.LoadState.Failed
                                    ModMain.Hint(OurLoader.Name + "失败：" + OurLoader.[Error].GetDisplay(False), ModMain.HintType.Red, True)
                                Case ModBase.LoadState.Canceled
                                    ModMain.Hint(OurLoader.Name + "已取消！", ModMain.HintType.Blue, True)
                            End Select
                            ModDownloadLib.McInstallFailedClearFolder(OurLoader)
                        End Sub
                        action = action2
                        PageDownloadResourceDetail._Closure$__.$I22-1 = action2
                    End If
                    loaderCombo.OnStateChanged = action
                    loaderCombo.Start(ModMinecraft.McFolderSelected + "versions\" + InstanceName + "\", False)
                    ModLoader.LoaderTaskbarAdd(Of String)(loaderCombo)
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                    ModMain.FrmMain.PageChange(FormMain.PageType.TaskManager, FormMain.PageSubType.[Default])
                End If
            Catch ex As Exception
                Logger.[Error](ex, "下载资源整合包失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\Resource\PageDownloadCompDetail.xaml.vb")
            End Try
        End Sub

        Public Sub Save_Click(sender As Object, e As EventArgs)
            Dim File As ResourceVersion = CType(NewLateBinding.LateGet(If((TypeOf sender Is OurListItem), sender, NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing)), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ResourceVersion)
            ModBase.RunInNewThread(Function()
                Try
                    Dim Desc As String = Nothing
                    Dim resourceType As ResourceTypes = File.ResourceType
                    Select Case resourceType
                        Case ResourceTypes.[Mod]
                            Desc = "Mod "
                        Case ResourceTypes.ModPack
                            Desc = "整合包"
                        Case ResourceTypes.[Mod] Or ResourceTypes.ModPack
                        Case ResourceTypes.ResourcePack
                            Desc = "资源包"
                        Case Else
                            If resourceType <> ResourceTypes.Shader Then
                                If resourceType = ResourceTypes.DataPack Then
                                    Desc = "数据包"
                                End If
                            Else
                                Desc = "光影包"
                            End If
                    End Select
                    Dim DefaultFolder As String = Nothing
                    If File.ResourceType <> ResourceTypes.ModPack Then
                        Dim SubFolder As String = Nothing
                        Dim resourceType2 As ResourceTypes = File.ResourceType
                        If resourceType2 <= ResourceTypes.ResourcePack Then
                            If resourceType2 <> ResourceTypes.[Mod] Then
                                If resourceType2 = ResourceTypes.ResourcePack Then
                                    SubFolder = "resourcepacks\"
                                End If
                            Else
                                SubFolder = "mods\"
                            End If
                        ElseIf resourceType2 <> ResourceTypes.Shader Then
                            If resourceType2 = ResourceTypes.DataPack Then
                                SubFolder = ""
                            End If
                        Else
                            SubFolder = "shaderpacks\"
                        End If
                        Dim IsInstanceSuitable As Func(Of ModMinecraft.McInstance, Boolean) = Nothing
                        Dim AllowedLoaders As ModLoaders = ModLoaders.None
                        If File.ModLoaders <> ModLoaders.None Then
                            AllowedLoaders = File.ModLoaders
                        ElseIf Me.Project.ModLoaders <> ModLoaders.None Then
                            AllowedLoaders = Me.Project.ModLoaders
                        End If
                        Logger.Info(String.Format("{0}要求的加载器种类：{1}", Desc, AllowedLoaders), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\Resource\PageDownloadCompDetail.xaml.vb")
                        IsInstanceSuitable = Function(Instance As ModMinecraft.McInstance)
                            If Not Instance.IsLoaded Then
                                Instance.Load()
                            End If
                            Return((File.ResourceType <> ResourceTypes.[Mod] AndAlso File.ResourceType <> ResourceTypes.DataPack) OrElse File.GameVersions.Any(Function(FileVersion As String)
                                Dim flag2 As Boolean = FileVersion.Contains("预览版") OrElse Not FileVersion.Contains(".")
                                Dim flag3 As Boolean = Instance.Version.VanillaName.Contains("snapshot") OrElse Not Instance.Version.VanillaName.Contains(".")
                                Dim flag4 As Boolean
                                If flag2 <> flag3 Then
                                    flag4 = False
                                ElseIf Not flag2 AndAlso Not flag3 Then
                                    flag4 = Operators.CompareString(FileVersion, Instance.Version.VanillaName, False) = 0
                                Else
                                    Dim flag5 As Boolean = FileVersion.Contains(".") AndAlso ModBase.Val(FileVersion.BeforeFirst(".", False)) > 1.0
                                    Dim flag6 As Boolean = Instance.Version.Drop > 250
                                    If flag5 <> flag6 Then
                                        flag4 = False
                                    ElseIf flag5 AndAlso flag6 Then
                                        flag4 = Operators.CompareString(FileVersion.BeforeFirst(" ", False), Instance.Version.VanillaName.BeforeFirst("-", False), False) = 0
                                    Else
                                        flag4 = Not FileVersion.Contains("w") OrElse Not Instance.Version.VanillaName.Contains("w") OrElse Operators.CompareString(FileVersion, Instance.Version.VanillaName, False) = 0
                                    End If
                                End If
                                Return flag4
                            End Function)) AndAlso (AllowedLoaders = ModLoaders.None OrElse AllowedLoaders.Flags().Intersect(Instance.Version.ModLoaders.Flags()).Any())
                        End Function
                        If PageDownloadResourceDetail.CachedFolder.ContainsKey(File.ResourceType) Then
                            DefaultFolder = PageDownloadResourceDetail.CachedFolder(File.ResourceType)
                            Logger.Info(String.Format("使用上次下载时的文件夹作为默认下载位置：{0}", DefaultFolder), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\Resource\PageDownloadCompDetail.xaml.vb")
                        ElseIf ModMinecraft.McInstanceSelected IsNot Nothing AndAlso IsInstanceSuitable(ModMinecraft.McInstanceSelected) Then
                            DefaultFolder = String.Format("{0}{1}", ModMinecraft.McInstanceSelected.PathIndie, SubFolder)
                            DirectoryUtils.Create(DefaultFolder)
                            Logger.Info(String.Format("使用当前版本作为默认下载位置：{0}", DefaultFolder), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\Resource\PageDownloadCompDetail.xaml.vb")
                        Else
                            Dim flag As Boolean = ModMinecraft.McInstanceListLoader.State <> ModBase.LoadState.Finished
                            If flag Then
                                ModMain.Hint("正在查找适合的游戏版本……", ModMain.HintType.Blue, True)
                                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", True)
                            End If
                            Dim values As IEnumerable(Of List(Of ModMinecraft.McInstance)) = ModMinecraft.McInstanceList.Values
                            Dim func As Func(Of List(Of ModMinecraft.McInstance), IEnumerable(Of ModMinecraft.McInstance))
                            If PageDownloadResourceDetail._Closure$__.$I24-3 IsNot Nothing Then
                                func = PageDownloadResourceDetail._Closure$__.$I24-3
                            Else
                                Dim func2 As Func(Of List(Of ModMinecraft.McInstance), IEnumerable(Of ModMinecraft.McInstance)) = Function(l As List(Of ModMinecraft.McInstance)) l
                                func = func2
                                PageDownloadResourceDetail._Closure$__.$I24-3 = func2
                            End If
                            Dim enumerable As IEnumerable(Of DirectoryInfo) = values.SelectMany(func).Where(Function(v As ModMinecraft.McInstance) IsInstanceSuitable(v)).[Select](Function(v As ModMinecraft.McInstance) DirectoryUtils.GetInfo(String.Format("{0}{1}", v.PathIndie, SubFolder)))
                            If enumerable.Any() Then
                                Dim enumerable2 As IEnumerable(Of DirectoryInfo) = enumerable
                                Dim func3 As Func(Of DirectoryInfo, DateTime)
                                If PageDownloadResourceDetail._Closure$__.$I24-6 IsNot Nothing Then
                                    func3 = PageDownloadResourceDetail._Closure$__.$I24-6
                                Else
                                    Dim func4 As Func(Of DirectoryInfo, DateTime) = Function(Dir As DirectoryInfo)
                                        If Not Dir.Exists Then
                                            Return DateTime.MinValue
                                        End If
                                        Return Dir.LastWriteTimeUtc
                                    End Function
                                    func3 = func4
                                    PageDownloadResourceDetail._Closure$__.$I24-6 = func4
                                End If
                                Dim orderedEnumerable As IOrderedEnumerable(Of DirectoryInfo) = enumerable2.OrderByDescending(func3)
                                Dim func5 As Func(Of DirectoryInfo, Integer)
                                If PageDownloadResourceDetail._Closure$__.$I24-7 IsNot Nothing Then
                                    func5 = PageDownloadResourceDetail._Closure$__.$I24-7
                                Else
                                    Dim func6 As Func(Of DirectoryInfo, Integer) = Function(Dir As DirectoryInfo)
                                        If Not Dir.Exists Then
                                            Return -1
                                        End If
                                        Return Dir.GetFiles().Length
                                    End Function
                                    func5 = func6
                                    PageDownloadResourceDetail._Closure$__.$I24-7 = func6
                                End If
                                Dim directoryInfo As DirectoryInfo = orderedEnumerable.ThenByDescending(func5).First()
                                DefaultFolder = PathUtils.RemoveExtendedPrefix(directoryInfo.FullName)
                                DirectoryUtils.Create(DefaultFolder)
                                Logger.Info(String.Format("使用适合的游戏版本作为默认下载位置：{0}", DefaultFolder), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\Resource\PageDownloadCompDetail.xaml.vb")
                            Else
                                DefaultFolder = ModMinecraft.McFolderSelected
                                If flag Then
                                    ModMain.Hint("当前 MC 文件夹中没有找到合适的版本！", ModMain.HintType.Blue, True)
                                Else
                                    Logger.Info("由于当前版本不兼容，使用当前的 MC 文件夹作为默认下载位置", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\Resource\PageDownloadCompDetail.xaml.vb")
                                End If
                            End If
                        End If
                    End If
                    Dim FileName As String
                    If Operators.CompareString(Me.Project.TranslatedName, Me.Project.RawName, False) = 0 Then
                        FileName = File.FileName
                    Else
                        Dim text As String = Me.Project.TranslatedName.BeforeFirst(" (", False).BeforeFirst(" - ", False).Replace("\", "＼").Replace("/", "／").Replace("|", "｜").Replace(":", "：").Replace("<", "＜").Replace(">", "＞").Replace("*", "＊").Replace("?", "？").Replace("""", "").Replace("： ", "：")
                        Select Case Settings.[Get](Of Integer)("ToolDownloadTranslateV2", Nothing)
                            Case 0
                                FileName = String.Format("【{0}】{1}", text, File.FileName)
                            Case 1
                                FileName = String.Format("[{0}] {1}", text, File.FileName)
                            Case 2
                                FileName = String.Format("{0}-{1}", text, File.FileName)
                            Case 3
                                FileName = String.Format("{0}-{1}", File.FileName, text)
                            Case Else
                                FileName = File.FileName
                        End Select
                    End If
                    If File.ResourceType = ResourceTypes.[Mod] Then
                        FileName = FileName.Replace("~", "-")
                    End If
                    ModBase.RunInUi(Sub()
                        Dim text2 As String = If((File.ResourceType = ResourceTypes.[Mod]), If((Operators.CompareString(PathUtils.GetExtension(File.FileName), "litemod", False) = 0), "litemod", "jar"), If((Operators.CompareString(PathUtils.GetExtension(File.FileName), "mrpack", False) = 0), "mrpack", "zip"))
                        Dim Target As String = Dialogs.SaveFile("选择保存位置", FileName, DefaultFolder, New ValueTuple(Of String, String)() { New ValueTuple(Of String, String)(text2, Desc + "文件") })
                        If Target IsNot Nothing Then
                            Dim text3 As String = Desc + "下载：" + PathUtils.GetFileNameWithoutExtension(Target) + " "
                            If Not PathUtils.IsSame(PathUtils.RemoveLastPart(Target), DefaultFolder) Then
                                PageDownloadResourceDetail.CachedFolder(File.ResourceType) = PathUtils.RemoveLastPart(Target)
                            End If
                            Dim loaderCombo As ModLoader.LoaderCombo(Of Integer) = New ModLoader.LoaderCombo(Of Integer)(text3, New List(Of ModLoader.LoaderBase)() From { New ModNet.LoaderDownload("下载文件", New ModNet.NetFile() { File.ToNetFile(Target, New ResourceVersion.DownloadReason?(If((Me.TargetResourceType = ResourceTypes.Any), ResourceVersion.DownloadReason.Dependency, ResourceVersion.DownloadReason.Standalone)), Nothing, ModLoaders.None) }) With { .ProgressWeight = 6.0, .Block = True } })
                            loaderCombo.OnStateChanged = Sub(OurLoader As ModLoader.LoaderBase)
                                ModDownloadLib.LoaderStateChangedHintOnly(OurLoader)
                                If OurLoader.State = ModBase.LoadState.Finished AndAlso File.ResourceType = ResourceTypes.ModPack Then
                                    ModBase.OpenExplorer(Target)
                                End If
                            End Sub
                            loaderCombo.Start(1, False)
                            ModLoader.LoaderTaskbarAdd(Of Integer)(loaderCombo)
                            ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                            ModMain.FrmMain.BtnExtraDownload.Ribble()
                        End If
                    End Sub, False)
                Catch ex As Exception
                    Logger.[Error](ex, "保存资源文件失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\Resource\PageDownloadCompDetail.xaml.vb")
                End Try
            End Function, "Download Resource Detail Save", ThreadPriority.Normal)
        End Sub

        Private Sub BtnIntroWeb_Click(sender As Object, e As EventArgs)
            ModBase.OpenWebsite(Me.Project.Website)
        End Sub

        Private Sub BtnIntroWiki_Click(sender As Object, e As EventArgs)
            ModBase.OpenWebsite("https://www.mcmod.cn/class/" + Conversions.ToString(Me.Project.WikiId) + ".html")
        End Sub

        Private Sub BtnIntroCopy_Click(sender As Object, e As EventArgs)
            ModBase.ClipboardSet(Me.ResourceItem.LabTitle.Text + Me.ResourceItem.LabTitleRaw.Text, True)
        End Sub

        Private Sub HintAlternative_Click()
            Me.TargetResourceType = If((Me.TargetResourceType = ResourceTypes.[Mod]), ResourceTypes.DataPack, ResourceTypes.[Mod])
            NewLateBinding.LateIndexSet(ModMain.FrmMain.PageCurrent.Additional, New Object() { 4, Me.TargetResourceType }, Nothing)
            MyBase.PageLoaderRestart(Nothing, True)
        End Sub

        Friend Overridable Property PanBack As OurScrollViewer

        Friend Overridable Property CardIntro As OurCard

        Friend Overridable Property PanIntro As StackPanel

        Friend Overridable Property BtnIntroWeb As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnIntroWeb
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnIntroWeb_Click
                Dim myButton As OurButton = Me._BtnIntroWeb
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnIntroWeb = value
                myButton = Me._BtnIntroWeb
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnIntroWiki As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnIntroWiki
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnIntroWiki_Click
                Dim myButton As OurButton = Me._BtnIntroWiki
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnIntroWiki = value
                myButton = Me._BtnIntroWiki
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnIntroCopy As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnIntroCopy
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnIntroCopy_Click
                Dim myButton As OurButton = Me._BtnIntroCopy
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnIntroCopy = value
                myButton = Me._BtnIntroCopy
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanMain As StackPanel

        Friend Overridable Property HintAlternative As OurHint
            <CompilerGenerated()>
            Get
                Return Me._HintAlternative
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurHint)
                Dim clickEventHandler As OurHint.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.HintAlternative_Click()
                End Sub
                Dim myHint As OurHint = Me._HintAlternative
                If myHint IsNot Nothing Then
                    RemoveHandler myHint.Click, clickEventHandler
                End If
                Me._HintAlternative = value
                myHint = Me._HintAlternative
                If myHint IsNot Nothing Then
                    AddHandler myHint.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CardFilter As OurCard

        Friend Overridable Property PanFilter As StackPanel

        Friend Overridable Property PanResults As StackPanel

        Friend Overridable Property PanLoad As OurCard

        Friend Overridable Property Load As OurLoading
            <CompilerGenerated()>
            Get
                Return Me._Load
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurLoading)
                Dim stateChangedEventHandler As OurLoading.StateChangedEventHandler = AddressOf Me.Load_State
                Dim myLoading As OurLoading = Me._Load
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

        Private ResourceItem As OurResourceItem

        Private TargetResourceType As ResourceTypes

        Private Project As ResourceProject

        Private TargetVersion As String

        Private TargetLoaders As ModLoaders

        Private ResourceFileLoader As ModLoader.LoaderTask(Of Integer, List(Of ResourceVersion))

        Private VersionFilter As String

        Private GroupedDrop As Boolean

        Private GroupedOld As Boolean

        Private IsFirstInit As Boolean

        Public Shared CachedFolder As Dictionary(Of ResourceTypes, String) = New Dictionary(Of ResourceTypes, String)()

        Private Class CardSorter
            Implements IComparer(Of String)

            Public Function Compare(x As String, y As String) As Integer Implements System.Collections.Generic.IComparer(Of String).Compare
                ' The following expression was wrapped in a checked-statement
                Dim num As Integer
                If Operators.CompareString(x, y, False) = 0 Then
                    num = 0
                ElseIf Operators.CompareString(x, Me.Topmost, False) = 0 Then
                    num = -1
                ElseIf Operators.CompareString(y, Me.Topmost, False) = 0 Then
                    num = 1
                Else
                    Dim flag As Boolean = Not x.Contains(".")
                    Dim flag2 As Boolean = Not y.Contains(".")
                    If flag AndAlso flag2 Then
                        num = x.CompareTo(y)
                    ElseIf flag Then
                        num = 1
                    ElseIf flag2 Then
                        num = -1
                    Else
                        Dim num2 As Integer = 0 - McUtils.CompareVersion(x.Replace(x.BeforeFirst(" ", False) + " ", ""), y.Replace(y.BeforeFirst(" ", False) + " ", ""))
                        If num2 <> 0 Then
                            num = num2
                        Else
                            num = 0 - McUtils.CompareVersion(x, y)
                        End If
                    End If
                End If
                Return num
            End Function

            Public Sub New(Optional Topmost As String = "")
                Me.Topmost = ""
                Me.Topmost = If(Topmost, "")
            End Sub

            Public Topmost As String
        End Class
    End Class
End Namespace
