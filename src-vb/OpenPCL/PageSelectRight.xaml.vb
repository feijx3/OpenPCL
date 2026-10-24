Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports PCL.My
Imports PCLCS

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageSelectRight
        Inherits OurPageRight

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageSelectRight_Loaded
            AddHandler MyBase.Initialized, Sub(a0 As Object, a1 As EventArgs)
                Me.LoaderInit()
            End Sub
            Me.ShowHidden = False
            Me.InitializeComponent()
        End Sub

        Private Sub PageSelectRight_Loaded(sender As Object, e As RoutedEventArgs)
            ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.RunOnUpdated, 1, "versions\", False)
            Me.PanBack.ScrollToHome()
        End Sub

        Private Sub LoaderInit()
            MyBase.PageLoaderInit(Of String, Integer)(Me.Load, Me.PanLoad, Me.PanAllBack, Nothing, ModMinecraft.McInstanceListLoader, AddressOf Me.McInstanceListUI, Nothing, False)
        End Sub

        Private Sub Load_Click(sender As Object, e As MouseButtonEventArgs)
            If ModMinecraft.McInstanceListLoader.State = ModBase.LoadState.Failed Then
                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
            End If
        End Sub

        Private Sub McInstanceListUI(Loader As ModLoader.LoaderTask(Of String, Integer))
            Try
                Me.PanMain.Children.Clear()
                For Each keyValuePair As KeyValuePair(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance)) In ModMinecraft.McInstanceList.ToArray()
                    If Not((keyValuePair.Key = ModMinecraft.McInstanceCardType.Hidden) Xor Me.ShowHidden) Then
                        Dim value As IEnumerable(Of ModMinecraft.McInstance) = keyValuePair.Value
                        Dim func As Func(Of ModMinecraft.McInstance, Boolean)
                        If PageSelectRight._Closure$__.$I5-0 IsNot Nothing Then
                            func = PageSelectRight._Closure$__.$I5-0
                        Else
                            Dim func2 As Func(Of ModMinecraft.McInstance, Boolean) = Function(Instance As ModMinecraft.McInstance) Not ModMinecraft.McInstanceLock.IsLocked(Instance)
                            func = func2
                            PageSelectRight._Closure$__.$I5-0 = func2
                        End If
                        Dim list As List(Of ModMinecraft.McInstance) = value.Where(func).ToList()
                        If list.Any() Then
                            Dim text As String
                            Select Case keyValuePair.Key
                                Case ModMinecraft.McInstanceCardType.Star
                                    text = "收藏夹"
                                Case ModMinecraft.McInstanceCardType.Auto
                                    GoTo IL_01D9
                                Case ModMinecraft.McInstanceCardType.Hidden
                                    text = "隐藏的版本"
                                Case ModMinecraft.McInstanceCardType.API
                                    Dim flag As Boolean = False
                                    Dim flag2 As Boolean = False
                                    Dim flag3 As Boolean = False
                                    Dim flag4 As Boolean = False
                                    Try
                                        For Each mcInstance As ModMinecraft.McInstance In list
                                            If mcInstance.Version.ModLoaders.HasFlagF(ModLoaders.Fabric) Then
                                                flag3 = True
                                            End If
                                            If mcInstance.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader) Then
                                                flag4 = True
                                            End If
                                            If mcInstance.Version.ModLoaders.HasFlagF(ModLoaders.Forge) Then
                                                flag = True
                                            End If
                                            If mcInstance.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge) Then
                                                flag2 = True
                                            End If
                                        Next
                                    Finally
                                        Dim enumerator As List(Of ModMinecraft.McInstance).Enumerator
                                        CType(enumerator, IDisposable).Dispose()
                                    End Try
                                    If(flag4 > False) + (flag > False) + (flag3 > False) + (flag2 > False) > True Then
                                        text = "可安装 Mod"
                                    ElseIf flag Then
                                        text = "Forge 版本"
                                    ElseIf flag2 Then
                                        text = "NeoForge 版本"
                                    ElseIf flag4 Then
                                        text = "LiteLoader 版本"
                                    Else
                                        text = "Fabric 版本"
                                    End If
                                Case ModMinecraft.McInstanceCardType.OriginalLike
                                    text = "常规版本"
                                Case ModMinecraft.McInstanceCardType.Rubbish
                                    text = "不常用版本"
                                Case ModMinecraft.McInstanceCardType.Fool
                                    text = "愚人节版本"
                                Case ModMinecraft.McInstanceCardType.[Error]
                                    text = "错误的版本"
                                Case Else
                                    GoTo IL_01D9
                            End Select
                            Dim text2 As String = text + If((Operators.CompareString(text, "收藏夹", False) = 0), "", (" (" + Conversions.ToString(list.Count) + ")"))
                            Dim myCard As OurCard = New OurCard() With { .Title = text2, .Margin = New Thickness(0.0, 0.0, 0.0, 15.0) }
                            Dim stackPanel As StackPanel = New StackPanel() With { .Margin = New Thickness(20.0, 40.0, 18.0, 0.0), .VerticalAlignment = VerticalAlignment.Top, .RenderTransform = New TranslateTransform(0.0, 0.0) }
                            myCard.Children.Add(stackPanel)
                            myCard.SwapControl = stackPanel
                            Dim myCard2 As OurCard = myCard
                            Dim enumerable As IEnumerable(Of ModMinecraft.McInstance) = list
                            Dim func3 As Func(Of ModMinecraft.McInstance, UIElement)
                            If PageSelectRight._Closure$__.$I5-1 IsNot Nothing Then
                                func3 = PageSelectRight._Closure$__.$I5-1
                            Else
                                Dim func4 As Func(Of ModMinecraft.McInstance, UIElement) = Function(Instance As ModMinecraft.McInstance) Instance.ToListItem(AddressOf PageSelectRight.McInstanceListContent)
                                func3 = func4
                                PageSelectRight._Closure$__.$I5-1 = func4
                            End If
                            myCard2.SetContentBuilder(Of ModMinecraft.McInstance)(enumerable, func3)
                            Me.PanMain.Children.Add(myCard)
                            If keyValuePair.Key = ModMinecraft.McInstanceCardType.Rubbish OrElse keyValuePair.Key = ModMinecraft.McInstanceCardType.[Error] OrElse keyValuePair.Key = ModMinecraft.McInstanceCardType.Fool Then
                                myCard.IsSwapped = True
                                GoTo IL_0351
                            End If
                            myCard.BuildContent()
                            GoTo IL_0351
                            IL_01D9:
                            Throw New ArgumentException("未知的卡片种类（" + Conversions.ToString(CInt(keyValuePair.Key)) + "）")
                        End If
                    End If
                    IL_0351:
                Next
                If Me.PanMain.Children.Count = 1 AndAlso CType(Me.PanMain.Children(0), OurCard).IsSwapped Then
                    CType(Me.PanMain.Children(0), OurCard).IsSwapped = False
                End If
                If Me.PanMain.Children.Count = 0 Then
                    Me.PanEmpty.Visibility = Visibility.Visible
                    Me.PanBack.Visibility = Visibility.Collapsed
                    If Me.ShowHidden Then
                        Me.LabEmptyTitle.Text = "无隐藏版本"
                        Me.LabEmptyContent.Text = "没有版本被隐藏，你可以在版本设置的版本分类选项中隐藏版本。" & vbCrLf & "再次按下 F11 即可退出隐藏版本查看模式。"
                        Me.BtnEmptyDownload.Visibility = Visibility.Collapsed
                    Else
                        Me.LabEmptyTitle.Text = "无可用版本"
                        Me.LabEmptyContent.Text = "未找到任何版本的游戏，请先下载任意版本的游戏。" & vbCrLf & "若有已存在的游戏，请在左边的列表中选择添加文件夹，选择 .minecraft 文件夹将其导入。"
                        Me.BtnEmptyDownload.Visibility = If((Settings.[Get](Of Boolean)("UiHiddenPageDownload", Nothing) AndAlso Not PageSetupUI.HiddenForceShow), Visibility.Collapsed, Visibility.Visible)
                    End If
                Else
                    Me.PanBack.Visibility = Visibility.Visible
                    Me.PanEmpty.Visibility = Visibility.Collapsed
                End If
            Catch ex As Exception
                Logger.[Error](ex, "将版本列表转换显示时失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSelectRight.xaml.vb")
            End Try
        End Sub

        Public Shared Sub McInstanceListContent(sender As OurListItem, e As EventArgs)
            Dim Instance As ModMinecraft.McInstance = CType(sender.Tag, ModMinecraft.McInstance)
            Dim $VB$Local_sender As OurListItem = sender
            Dim clickEventHandler As OurListItem.ClickEventHandler
            If PageSelectRight._Closure$__.$IR6-2 IsNot Nothing Then
                clickEventHandler = PageSelectRight._Closure$__.$IR6-2
            Else
                Dim clickEventHandler2 As OurListItem.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    PageSelectRight.Item_Click(CType(a0, OurListItem), a1)
                End Sub
                clickEventHandler = clickEventHandler2
                PageSelectRight._Closure$__.$IR6-2 = clickEventHandler2
            End If
            AddHandler $VB$Local_sender.Click, clickEventHandler
            Dim myIconButton As OurIconButton = New OurIconButton()
            If Instance.IsStar Then
                myIconButton.ToolTip = "取消收藏"
                myIconButton.LogoScale = 1.1
                myIconButton.Logo = "M700.856 155.543c-74.769 0-144.295 72.696-190.046 127.26-45.737-54.576-115.247-127.26-190.056-127.26-134.79 0-244.443 105.78-244.443 235.799 0 77.57 39.278 131.988 70.845 175.713C238.908 694.053 469.62 852.094 479.39 858.757c9.41 6.414 20.424 9.629 31.401 9.629 11.006 0 21.998-3.215 31.398-9.63 9.782-6.662 240.514-164.703 332.238-291.701 31.587-43.724 70.874-98.143 70.874-175.713-0.001-130.02-109.656-235.8-244.445-235.8z m0 0"
            Else
                myIconButton.ToolTip = "收藏"
                myIconButton.LogoScale = 1.1
                myIconButton.Logo = "M512 896a42.666667 42.666667 0 0 1-30.293333-12.373333l-331.52-331.946667a224.426667 224.426667 0 0 1 0-315.733333 223.573333 223.573333 0 0 1 315.733333 0L512 282.026667l46.08-46.08a223.573333 223.573333 0 0 1 315.733333 0 224.426667 224.426667 0 0 1 0 315.733333l-331.52 331.946667A42.666667 42.666667 0 0 1 512 896zM308.053333 256a136.533333 136.533333 0 0 0-97.28 40.106667 138.24 138.24 0 0 0 0 194.986666L512 792.746667l301.226667-301.653334a138.24 138.24 0 0 0 0-194.986666 141.653333 141.653333 0 0 0-194.56 0l-76.373334 76.8a42.666667 42.666667 0 0 1-60.586666 0L405.333333 296.106667A136.533333 136.533333 0 0 0 308.053333 256z"
            End If
            AddHandler myIconButton.Click, Sub(a0 As Object, a1 As EventArgs)
                MyBase._Lambda$__0()
            End Sub
            Dim myIconButton2 As OurIconButton = New OurIconButton() With { .LogoScale = 1.1, .Logo = "M520.192 0C408.43 0 317.44 82.87 313.563 186.734H52.736c-29.038 0-52.663 21.943-52.663 49.079s23.625 49.152 52.663 49.152h58.075v550.473c0 103.35 75.118 187.757 167.717 187.757h472.43c92.599 0 167.716-83.894 167.716-187.757V285.477h52.59c29.038 0 52.59-21.943 52.663-49.08-0.073-27.135-23.625-49.151-52.663-49.151H726.235C723.237 83.017 631.955 0 520.192 0zM404.846 177.957c3.803-50.03 50.176-89.015 107.447-89.015 57.197 0 103.57 38.985 106.788 89.015H404.92zM284.379 933.669c-33.353 0-69.997-39.351-69.997-95.525v-549.01H833.39v549.522c0 56.247-36.645 95.525-69.998 95.525H284.379v-0.512z M357.23 800.695a48.274 48.274 0 0 0 47.616-49.006V471.7a48.274 48.274 0 0 0-47.543-49.08 48.274 48.274 0 0 0-47.69 49.006V751.69c0 27.282 20.846 49.006 47.617 49.006z m166.62 0a48.274 48.274 0 0 0 47.688-49.006V471.7a48.274 48.274 0 0 0-47.689-49.08 48.274 48.274 0 0 0-47.543 49.006V751.69c0 27.282 21.431 49.006 47.543 49.006z m142.92 0a48.274 48.274 0 0 0 47.543-49.006V471.7a48.274 48.274 0 0 0-47.543-49.08 48.274 48.274 0 0 0-47.616 49.006V751.69c0 27.282 20.773 49.006 47.543 49.006z" }
            myIconButton2.ToolTip = "删除"
            AddHandler myIconButton2.Click, Sub(a0 As Object, a1 As EventArgs)
                MyBase._Lambda$__1()
            End Sub
            If Instance.State <> ModMinecraft.McInstance.McInstanceState.[Error] Then
                Dim myIconButton3 As OurIconButton = New OurIconButton() With { .LogoScale = 1.1, .Logo = "M651.946667 1001.813333c-22.186667 0-42.666667-10.24-61.44-27.306666-23.893333-23.893333-49.493333-35.84-75.093334-35.84-29.013333 0-56.32 11.946667-73.386666 30.72v3.413333c-17.066667 17.066667-42.666667 27.306667-66.56 27.306667h-6.826667c-6.826667 0-11.946667-1.706667-15.36-1.706667l-6.826667-1.706667c-64.853333-20.48-121.173333-54.613333-168.96-98.986666-29.013333-23.893333-37.546667-63.146667-25.6-95.573334 8.533333-23.893333 5.12-51.2-10.24-75.093333-15.36-27.306667-34.133333-40.96-59.733333-47.786667h-1.706667l-5.12-1.706666c-35.84-8.533333-61.44-34.133333-66.56-69.973334C1.706667 575.146667 0 537.6 0 512c0-32.426667 3.413333-63.146667 8.533333-93.866667v-6.826666l3.413334-8.533334c10.24-23.893333 23.893333-40.96 44.373333-51.2 5.12-3.413333 11.946667-6.826667 20.48-8.533333 27.306667-8.533333 51.2-25.6 63.146667-44.373333 13.653333-23.893333 17.066667-52.906667 10.24-81.92-11.946667-34.133333 0-71.68 30.72-93.866667 44.373333-37.546667 97.28-68.266667 158.72-93.866667l3.413333-1.706666c44.373333-13.653333 75.093333 3.413333 92.16 20.48 23.893333 23.893333 49.493333 35.84 75.093333 35.84 30.72 0 56.32-10.24 71.68-30.72l3.413334-3.413334c27.306667-27.306667 63.146667-35.84 93.866666-22.186666 63.146667 22.186667 117.76 54.613333 165.546667 97.28 29.013333 23.893333 37.546667 63.146667 25.6 95.573333-8.533333 23.893333-5.12 51.2 10.24 75.093333 15.36 27.306667 34.133333 40.96 59.733333 47.786667h1.706667l5.12 1.706667c35.84 8.533333 61.44 34.133333 66.56 71.68 6.826667 30.72 10.24 63.146667 11.946667 93.866666v3.413334c0 32.426667-3.413333 63.146667-8.533334 93.866666v6.826667l-3.413333 8.533333c-10.24 23.893333-23.893333 40.96-44.373333 51.2-5.12 3.413333-11.946667 6.826667-20.48 8.533334-27.306667 8.533333-51.2 25.6-63.146667 46.08-13.653333 23.893333-17.066667 52.906667-10.24 81.92 11.946667 35.84-1.706667 75.093333-30.72 95.573333-44.373333 35.84-95.573333 66.56-157.013333 92.16-15.36 3.413333-27.306667 3.413333-35.84 3.413333z m3.413333-83.626666z m1.706667 0zM517.12 853.333333c47.786667 0 93.866667 20.48 134.826667 59.733334 1.706667 1.706667 3.413333 1.706667 3.413333 3.413333 52.906667-22.186667 97.28-49.493333 136.533333-80.213333l1.706667-1.706667v-3.413333c-13.653333-52.906667-8.533333-104.106667 17.066667-148.48 23.893333-39.253333 64.853333-69.973333 114.346666-85.333334 1.706667 0 3.413333-1.706667 6.826667-6.826666 5.12-25.6 8.533333-51.2 8.533333-78.506667-1.706667-29.013333-3.413333-56.32-10.24-81.92v-5.12h-1.706666c-51.2-11.946667-90.453333-39.253333-119.466667-87.04-27.306667-44.373333-34.133333-100.693333-17.066667-148.48l-1.706666-1.706667h-3.413334c-39.253333-35.84-85.333333-63.146667-136.533333-80.213333H648.533333s-1.706667 1.706667-3.413333 1.706667c-32.426667 39.253333-80.213333 59.733333-136.533333 59.733333-47.786667 0-93.866667-20.48-134.826667-59.733333l-1.706667-1.706667h-1.706666c-54.613333 22.186667-98.986667 49.493333-136.533334 80.213333l-1.706666 1.706667v3.413333c13.653333 52.906667 8.533333 104.106667-17.066667 148.48-23.893333 39.253333-64.853333 69.973333-114.346667 85.333334-1.706667 0-3.413333 1.706667-6.826666 6.826666-6.826667 25.6-8.533333 51.2-8.533334 78.506667 0 30.72 3.413333 58.026667 6.826667 76.8l1.706667 5.12h1.706666c51.2 11.946667 90.453333 39.253333 119.466667 87.04 27.306667 44.373333 34.133333 100.693333 17.066667 148.48l1.706666 1.706667 1.706667 1.706666c37.546667 35.84 83.626667 63.146667 134.826667 80.213334 1.706667 0 3.413333 0 3.413333 1.706666h1.706667s1.706667 0 5.12-1.706666c34.133333-37.546667 81.92-59.733333 136.533333-59.733334z m-6.826667-146.773333c-110.933333 0-199.68-85.333333-199.68-196.266667 0-109.226667 87.04-196.266667 199.68-196.266666s199.68 85.333333 199.68 196.266666c-1.706667 109.226667-88.746667 196.266667-199.68 196.266667z m0-307.2c-63.146667 0-114.346667 49.493333-114.346666 110.933333 0 63.146667 49.493333 110.933333 114.346666 110.933334 30.72 0 59.733333-11.946667 80.213334-32.426667 20.48-20.48 32.426667-49.493333 32.426666-78.506667 0-63.146667-49.493333-110.933333-112.64-110.933333z" }
                myIconButton3.ToolTip = "设置"
                AddHandler myIconButton3.Click, Sub(a0 As Object, a1 As EventArgs)
                    MyBase._Lambda$__2()
                End Sub
                AddHandler sender.MouseRightButtonUp, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    MyBase._Lambda$__3()
                End Sub
                sender.Buttons = New OurIconButton() { myIconButton, myIconButton2, myIconButton3 }
                Return
            End If
            Dim myIconButton4 As OurIconButton = New OurIconButton() With { .LogoScale = 1.15, .Logo = "M889.018182 418.909091H884.363636V316.509091a93.090909 93.090909 0 0 0-99.607272-89.832727h-302.545455l-93.090909-76.334546A46.545455 46.545455 0 0 0 358.865455 139.636364H146.152727A93.090909 93.090909 0 0 0 46.545455 229.469091V837.818182a46.545455 46.545455 0 0 0 46.545454 46.545454 46.545455 46.545455 0 0 0 16.756364-3.258181 109.381818 109.381818 0 0 0 25.134545 3.258181h586.472727a85.178182 85.178182 0 0 0 87.04-63.301818l163.374546-302.545454a46.545455 46.545455 0 0 0 5.585454-21.876364A82.385455 82.385455 0 0 0 889.018182 418.909091z m-744.727273-186.181818h198.283636l93.09091 76.334545a46.545455 46.545455 0 0 0 29.323636 10.705455h319.301818a12.101818 12.101818 0 0 1 6.516364 0V418.909091H302.545455a85.178182 85.178182 0 0 0-87.04 63.301818L139.636364 622.778182V232.727273a19.549091 19.549091 0 0 1 6.516363 0z m578.094546 552.029091a27.461818 27.461818 0 0 0-2.792728 6.516363H154.530909l147.083636-272.290909a27.461818 27.461818 0 0 0 2.792728-6.981818h565.061818z" }
            myIconButton4.ToolTip = "打开文件夹"
            AddHandler myIconButton4.Click, Sub(a0 As Object, a1 As EventArgs)
                MyBase._Lambda$__4()
            End Sub
            AddHandler sender.MouseRightButtonUp, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                MyBase._Lambda$__5()
            End Sub
            sender.Buttons = New OurIconButton() { myIconButton, myIconButton2, myIconButton4 }
        End Sub

        Public Shared Sub Item_Click(sender As OurListItem, e As EventArgs)
            Dim mcInstance As ModMinecraft.McInstance = CType(sender.Tag, ModMinecraft.McInstance)
            If ModMinecraft.McInstanceLock.IsLocked(mcInstance) Then
                ModMain.Hint("该版本正在被修改，请稍后再选择！", ModMain.HintType.Red, True)
                Return
            End If
            If New ModMinecraft.McInstance(mcInstance.PathVersion).Check() Then
                ModMinecraft.McInstanceSelected = mcInstance
                ModMain.FrmMain.PageBack()
                Return
            End If
            PageInstanceOverall.OpenInstanceFolder(mcInstance)
        End Sub

        Private Sub BtnDownload_Click(sender As Object, e As EventArgs)
            ModMain.FrmMain.PageChange(FormMain.PageType.Download, FormMain.PageSubType.[Default])
        End Sub

        Public Shared Sub DeleteInstance(Item As OurListItem, Instance As ModMinecraft.McInstance)
            Try
                If ModMinecraft.McInstanceLock.IsLocked(Instance) Then
                    ModMain.Hint("该版本正在被修改，请稍后再删除！", ModMain.HintType.Red, True)
                Else
                    Dim shiftKeyDown As Boolean = MyWpfExtension.Computer.Keyboard.ShiftKeyDown
                    Dim flag As Boolean = Instance.State <> ModMinecraft.McInstance.McInstanceState.[Error] AndAlso Operators.CompareString(Instance.PathIndie, ModMinecraft.McFolderSelected, False) <> 0
                    Dim info As DirectoryInfo = DirectoryUtils.GetInfo(Instance.PathIndie + "saves\")
                    Dim list As List(Of String)
                    If Not flag OrElse Not info.Exists Then
                        list = New List(Of String)()
                    Else
                        Dim enumerable As IEnumerable(Of DirectoryInfo) = info.EnumerateDirectories()
                        Dim func As Func(Of DirectoryInfo, DateTime)
                        If PageSelectRight._Closure$__.$I9-0 IsNot Nothing Then
                            func = PageSelectRight._Closure$__.$I9-0
                        Else
                            Dim func2 As Func(Of DirectoryInfo, DateTime) = Function(Save As DirectoryInfo) Save.LastWriteTime
                            func = func2
                            PageSelectRight._Closure$__.$I9-0 = func2
                        End If
                        Dim enumerable2 As IEnumerable(Of DirectoryInfo) = enumerable.OrderByDescending(func)
                        Dim func3 As Func(Of DirectoryInfo, String)
                        If PageSelectRight._Closure$__.$I9-1 IsNot Nothing Then
                            func3 = PageSelectRight._Closure$__.$I9-1
                        Else
                            Dim func4 As Func(Of DirectoryInfo, String) = Function(Save As DirectoryInfo) String.Format("{0}（上次修改：{1}）", Save.Name, StringUtils.FormatTimeSpan(Save.LastWriteTime - DateTime.Now, False))
                            func3 = func4
                            PageSelectRight._Closure$__.$I9-1 = func4
                        End If
                        list = enumerable2.[Select](func3).ToList()
                    End If
                    Dim list2 As List(Of String) = list
                    Dim num As Integer = ModMain.OurMsgBox(String.Format("你确定要{0}删除版本 {1} 吗？", If(shiftKeyDown, "永久", ""), Instance.Name) + If(flag, vbCrLf & "该版本对应的存档、资源包、Mod 等文件也将被一并删除！", "") + If(list2.Any(), (vbCrLf & vbCrLf & "这会删除以下存档：" & vbCrLf & "· " + list2.Join(vbCrLf & "· ")), ""), "版本删除确认", "确定", "取消", "", True, True, False, Nothing, Nothing, Nothing)
                    If num <> 1 Then
                        If num = 2 Then
                            Return
                        End If
                    Else
                        Instance.ResetSettingsCache()
                        DirectoryUtils.Delete(Instance.PathVersion, Not shiftKeyDown)
                        ModMain.Hint("版本 " + Instance.Name + " 已删除！", ModMain.HintType.Green, True)
                    End If
                    If Instance.DisplayType = ModMinecraft.McInstanceCardType.Hidden OrElse Not Instance.IsStar Then
                        Dim stackPanel As StackPanel = CType(Item.Parent, StackPanel)
                        If stackPanel.Children.Count > 2 Then
                            Dim myCard As OurCard = CType(stackPanel.Parent, OurCard)
                            myCard.Title = myCard.Title.Replace(Conversions.ToString(stackPanel.Children.Count - 1), Conversions.ToString(stackPanel.Children.Count - 2))
                            stackPanel.Children.Remove(Item)
                            If ModMinecraft.McInstanceSelected IsNot Nothing AndAlso PathUtils.IsSame(Instance.PathVersion, ModMinecraft.McInstanceSelected.PathVersion) Then
                                ModMinecraft.McInstanceSelected = CType(OurVirtualizingElement.TryInit(CType(stackPanel.Children(0), FrameworkElement)).Tag, ModMinecraft.McInstance)
                            End If
                            ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.UpdateOnly, 1, "versions\", False)
                        Else
                            ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
                        End If
                    Else
                        ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
                    End If
                End If
            Catch ex As OperationCanceledException
                Logger.Warn(ex, String.Format("删除版本 {0} 被主动取消", Instance.Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSelectRight.xaml.vb")
            Catch ex2 As Exception
                Logger.[Error](ex2, String.Format("删除版本 {0} 失败", Instance.Name), LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSelectRight.xaml.vb")
            End Try
        End Sub

        Public Sub BtnEmptyDownload_Loaded()
            Dim visibility As Visibility = If(((Settings.[Get](Of Boolean)("UiHiddenPageDownload", Nothing) AndAlso Not PageSetupUI.HiddenForceShow) OrElse Me.ShowHidden), Visibility.Collapsed, Visibility.Visible)
            If Me.BtnEmptyDownload.Visibility <> visibility Then
                Me.BtnEmptyDownload.Visibility = visibility
                Me.PanLoad.TriggerForceResize()
            End If
        End Sub

        Friend Overridable Property PanAllBack As Grid

        Friend Overridable Property PanBack As OurScrollViewer

        Friend Overridable Property PanMain As StackPanel

        Friend Overridable Property PanEmpty As OurCard

        Friend Overridable Property LabEmptyTitle As TextBlock

        Friend Overridable Property LabEmptyContent As TextBlock

        Friend Overridable Property BtnEmptyDownload As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnEmptyDownload
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnDownload_Click
                Dim routedEventHandler As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    Me.BtnEmptyDownload_Loaded()
                End Sub
                Dim myButton As OurButton = Me._BtnEmptyDownload
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                    RemoveHandler myButton.Loaded, routedEventHandler
                End If
                Me._BtnEmptyDownload = value
                myButton = Me._BtnEmptyDownload
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                    AddHandler myButton.Loaded, routedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanLoad As OurCard

        Friend Overridable Property Load As OurLoading
            <CompilerGenerated()>
            Get
                Return Me._Load
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurLoading)
                Dim clickEventHandler As OurLoading.ClickEventHandler = AddressOf Me.Load_Click
                Dim myLoading As OurLoading = Me._Load
                If myLoading IsNot Nothing Then
                    RemoveHandler myLoading.Click, clickEventHandler
                End If
                Me._Load = value
                myLoading = Me._Load
                If myLoading IsNot Nothing Then
                    AddHandler myLoading.Click, clickEventHandler
                End If
            End Set
        End Property

        Public ShowHidden As Boolean
    End Class
End Namespace
