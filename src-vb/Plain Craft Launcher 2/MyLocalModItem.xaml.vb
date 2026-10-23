Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
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

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class MyLocalModItem
        Inherits Grid

        Public Property Title As String
            Get
                Return Me._Title
            End Get
            Set(value As String)
                Me.LabTitle.TextDecorations = If(Me.Entry.IsEnabled, Nothing, TextDecorations.Strikethrough)
                If Operators.CompareString(Me.LabTitle.Text, value, False) <> 0 Then
                    Me.LabTitle.Text = value
                    Me._Title = value
                End If
            End Set
        End Property

        Public Property SubTitle As String
            Get
                Dim labSubtitle As TextBlock = Me.LabSubtitle
                Return If(If((labSubtitle IsNot Nothing), labSubtitle.Text, Nothing), "")
            End Get
            Set(value As String)
                If Operators.CompareString(Me.LabSubtitle.Text, value, False) <> 0 Then
                    Me.LabSubtitle.Text = value
                    Me.LabSubtitle.Visibility = If((Operators.CompareString(value, "", False) = 0), Visibility.Collapsed, Visibility.Visible)
                End If
            End Set
        End Property

        Public Property Description As String
            Get
                Return Me.LabInfo.Text
            End Get
            Set(value As String)
                If Operators.CompareString(Me.LabInfo.Text, value, False) <> 0 Then
                    Me.LabInfo.Text = value
                End If
            End Set
        End Property

        Public WriteOnly Property Tags As List(Of String)
            Set(value As List(Of String))
                Me.PanTags.Children.Clear()
                Me.PanTags.Visibility = If(value.Any(), Visibility.Visible, Visibility.Collapsed)
                Try
                    For Each text As String In value
                        Dim border As Border = New Border() With { .Background = New ModBase.MyColor("#0A000000"), .Padding = New Thickness(3.0, 1.0, 3.0, 1.0), .CornerRadius = New CornerRadius(3.0), .Margin = New Thickness(0.0, 0.0, 3.0, 0.0), .SnapsToDevicePixels = True, .UseLayoutRounding = False }
                        Dim textBlock As TextBlock = New TextBlock() With { .Text = text, .Foreground = New ModBase.MyColor("#B0B0B0"), .FontSize = 11.0 }
                        border.Child = textBlock
                        Me.PanTags.Children.Add(border)
                    Next
                Finally
                    Dim enumerator As List(Of String).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
            End Set
        End Property

        Public Property Entry As LocalResourceFile
            Get
                Return CType(MyBase.Tag, LocalResourceFile)
            End Get
            Set(value As LocalResourceFile)
                MyBase.Tag = value
            End Set
        End Property

        Public Event Click As MyLocalModItem.ClickEventHandler

        Private Sub Button_MouseUp(sender As Object, e As MouseButtonEventArgs)
            If Me.IsMouseDown Then
                Dim clickEvent As MyLocalModItem.ClickEventHandler = Me.ClickEvent
                If clickEvent IsNot Nothing Then
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e)
                End If
                If Not e.Handled Then
                    Logger.Info(String.Format("按下本地 Mod 列表项：{0}", Me.LabTitle.Text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageInstance\MyLocalModItem.xaml.vb")
                End If
            End If
        End Sub

        Private Sub Button_MouseDown(sender As Object, e As MouseButtonEventArgs)
            If MyBase.IsMouseDirectlyOver Then
                Me.IsMouseDown = True
                If Me.ButtonStack IsNot Nothing Then
                    Me.ButtonStack.IsHitTestVisible = False
                End If
            End If
        End Sub

        Private Sub Button_MouseLeave(sender As Object, e As Object)
            Me.IsMouseDown = False
            If Me.ButtonStack IsNot Nothing Then
                Me.ButtonStack.IsHitTestVisible = True
            End If
        End Sub

        Private Shared Property Swiping As Boolean
            Get
                Return MyLocalModItem._Swiping
            End Get
            Set(value As Boolean)
                MyLocalModItem._Swiping = value
                ModMain.FrmInstanceMod.CardSelect.IsHitTestVisible = Not value
            End Set
        End Property

        Private Sub Button_MouseSwipeStart(sender As Object, e As Object)
            If MyBase.Parent IsNot Nothing Then
                Dim num As Integer = CType(MyBase.Parent, StackPanel).Children.IndexOf(Me)
                MyLocalModItem.SwipeStart = num
                MyLocalModItem.SwipeEnd = num
                MyLocalModItem.Swiping = True
                MyLocalModItem.SwipToState = Not Me.Checked
            End If
        End Sub

        Private Sub Button_MouseSwipe(sender As Object, e As Object)
            ' The following expression was wrapped in a checked-statement
            If MyBase.Parent IsNot Nothing Then
                If Mouse.LeftButton <> MouseButtonState.Pressed OrElse Not(TypeOf Mouse.DirectlyOver Is MyLocalModItem) Then
                    MyLocalModItem.Swiping = False
                    Return
                End If
                Dim children As UIElementCollection = CType(MyBase.Parent, StackPanel).Children
                Dim num As Integer = children.IndexOf(Me)
                MyLocalModItem.SwipeStart = Math.Min(MyLocalModItem.SwipeStart, num).Clamp(0, children.Count - 1)
                MyLocalModItem.SwipeEnd = Math.Max(MyLocalModItem.SwipeEnd, num).Clamp(0, children.Count - 1)
                If MyLocalModItem.SwipeStart <> MyLocalModItem.SwipeEnd Then
                    Dim swipeStart As Integer = MyLocalModItem.SwipeStart
                    Dim swipeEnd As Integer = MyLocalModItem.SwipeEnd
                    For i As Integer = swipeStart To swipeEnd
                        Dim myLocalModItem As MyLocalModItem = CType(children(i), MyLocalModItem)
                        myLocalModItem.InitLate(myLocalModItem, CType(e, EventArgs))
                        myLocalModItem.Checked = MyLocalModItem.SwipToState
                    Next
                End If
            End If
        End Sub

        Public Event Check As MyLocalModItem.CheckEventHandler

        Public Event Changed As MyLocalModItem.ChangedEventHandler

        Public Property Checked As Boolean
            Get
                Return Me._Checked
            End Get
            Set(value As Boolean)
                Try
                    Dim checked As Boolean = Me._Checked
                    If value <> Me._Checked Then
                        Me._Checked = value
                        Dim e As ModBase.RouteEventArgs = New ModBase.RouteEventArgs(False)
                        If MyBase.IsInitialized Then
                            Dim changedEvent As MyLocalModItem.ChangedEventHandler = Me.ChangedEvent
                            If changedEvent IsNot Nothing Then
                                changedEvent(Me, e)
                            End If
                            If e.Handled Then
                                Me._Checked = checked
                                Return
                            End If
                        End If
                        If value Then
                            Dim e2 As ModBase.RouteEventArgs = New ModBase.RouteEventArgs(False)
                            Dim checkEvent As MyLocalModItem.CheckEventHandler = Me.CheckEvent
                            If checkEvent IsNot Nothing Then
                                checkEvent(Me, e2)
                            End If
                            If e2.Handled Then
                                Return
                            End If
                        End If
                        If Me.IsVisibleInForm() Then
                            Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
                            If Me.Checked Then
                                Dim num As Double = 32.0 - Me.RectCheck.ActualHeight
                                list.Add(ModAnimation.AaHeight(Me.RectCheck, num * 0.4, 200, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False))
                                list.Add(ModAnimation.AaHeight(Me.RectCheck, num * 0.6, 300, 0, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), False))
                                list.Add(ModAnimation.AaOpacity(Me.RectCheck, 1.0 - Me.RectCheck.Opacity, 30, 0, Nothing, False))
                                Me.RectCheck.VerticalAlignment = VerticalAlignment.Center
                                Me.RectCheck.Margin = New Thickness(-3.0, 0.0, 0.0, 0.0)
                                list.Add(ModAnimation.AaColor(Me.LabTitle, TextBlock.ForegroundProperty, If(Me.Entry.IsEnabled, "ColorBrush2", "ColorBrush5"), 200, 0, Nothing, False))
                            Else
                                list.Add(ModAnimation.AaHeight(Me.RectCheck, -Me.RectCheck.ActualHeight, 120, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), False))
                                list.Add(ModAnimation.AaOpacity(Me.RectCheck, -Me.RectCheck.Opacity, 70, 40, Nothing, False))
                                Me.RectCheck.VerticalAlignment = VerticalAlignment.Center
                                list.Add(ModAnimation.AaColor(Me.LabTitle, TextBlock.ForegroundProperty, If((Me.LabTitle.TextDecorations Is Nothing), "ColorBrush1", "ColorBrushGray4"), 120, 0, Nothing, False))
                            End If
                            ModAnimation.AniStart(list, "MyLocalModItem Checked " + Conversions.ToString(Me.Uuid), False)
                        Else
                            Me.RectCheck.VerticalAlignment = VerticalAlignment.Center
                            Me.RectCheck.Margin = New Thickness(-3.0, 0.0, 0.0, 0.0)
                            If Me.Checked Then
                                Me.RectCheck.Height = 32.0
                                Me.RectCheck.Opacity = 1.0
                                Me.LabTitle.SetResourceReference(TextBlock.ForegroundProperty, If(Me.Entry.IsEnabled, "ColorBrush2", "ColorBrush5"))
                            Else
                                Me.RectCheck.Height = 0.0
                                Me.RectCheck.Opacity = 0.0
                                Me.LabTitle.SetResourceReference(TextBlock.ForegroundProperty, If(Me.Entry.IsEnabled, "ColorBrush1", "ColorBrushGray4"))
                            End If
                            ModAnimation.AniStop("MyLocalModItem Checked " + Conversions.ToString(Me.Uuid))
                        End If
                    End If
                Catch ex As Exception
                    Logger.Warn(ex, "设置 Checked 失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageInstance\MyLocalModItem.xaml.vb")
                End Try
            End Set
        End Property

        Public ReadOnly Property RectBack As Border
            Get
                If Me._RectBack Is Nothing Then
                    Dim border As Border = New Border() With { .Name = "RectBack", .CornerRadius = New CornerRadius(3.0), .RenderTransform = New ScaleTransform(0.8, 0.8), .RenderTransformOrigin = New Point(0.5, 0.5), .BorderThickness = New Thickness(ModBase.GetWPFSize(1.0)), .SnapsToDevicePixels = True, .IsHitTestVisible = False, .Opacity = 0.0 }
                    border.SetResourceReference(Border.BackgroundProperty, "ColorBrush7")
                    border.SetResourceReference(Border.BorderBrushProperty, "ColorBrush6")
                    Grid.SetColumnSpan(border, 999)
                    Grid.SetRowSpan(border, 999)
                    MyBase.Children.Insert(0, border)
                    Me._RectBack = border
                End If
                Return Me._RectBack
            End Get
        End Property

        Public Property Buttons As IEnumerable(Of MyIconButton)
            Get
                Return Me._Buttons
            End Get
            Set(value As IEnumerable(Of MyIconButton))
                Me._Buttons = value
                If Me.ButtonStack IsNot Nothing Then
                    MyBase.Children.Remove(Me.ButtonStack)
                    Me.ButtonStack = Nothing
                End If
                If value.Any() Then
                    Me.ButtonStack = New StackPanel() With { .Opacity = 0.0, .Margin = New Thickness(0.0, 0.0, 5.0, 0.0), .SnapsToDevicePixels = False, .Orientation = Orientation.Horizontal, .HorizontalAlignment = HorizontalAlignment.Right, .VerticalAlignment = VerticalAlignment.Center, .UseLayoutRounding = False }
                    Grid.SetColumnSpan(Me.ButtonStack, 10)
                    Grid.SetRowSpan(Me.ButtonStack, 10)
                    Try
                        For Each myIconButton As MyIconButton In value
                            If myIconButton.Height.Equals(Double.NaN) Then
                                myIconButton.Height = 25.0
                            End If
                            If myIconButton.Width.Equals(Double.NaN) Then
                                myIconButton.Width = 25.0
                            End If
                            CType(Me.ButtonStack, StackPanel).Children.Add(myIconButton)
                        Next
                    Finally
                        Dim enumerator As IEnumerator(Of MyIconButton)
                        If enumerator IsNot Nothing Then
                            enumerator.Dispose()
                        End If
                    End Try
                    MyBase.Children.Add(Me.ButtonStack)
                End If
            End Set
        End Property

        Public ReadOnly Property RectCheck As Border
            Get
                If Me._RectCheck Is Nothing Then
                    Me._RectCheck = New Border() With { .Width = 5.0, .Height = If(Me.Checked, Double.NaN, 0.0), .CornerRadius = New CornerRadius(2.0, 2.0, 2.0, 2.0), .VerticalAlignment = If(Me.Checked, VerticalAlignment.Stretch, VerticalAlignment.Center), .HorizontalAlignment = HorizontalAlignment.Left, .UseLayoutRounding = False, .SnapsToDevicePixels = False, .Margin = If(Me.Checked, New Thickness(-3.0, 6.0, 0.0, 6.0), New Thickness(-3.0, 0.0, 0.0, 0.0)) }
                    Me._RectCheck.SetResourceReference(Border.BackgroundProperty, "ColorBrush3")
                    Grid.SetRowSpan(Me._RectCheck, 10)
                    MyBase.Children.Add(Me._RectCheck)
                End If
                Return Me._RectCheck
            End Get
        End Property

        Private Function GetUpdateCompareDescription() As String
            Dim text As String = (If(Me.Entry.ProjectVersion.Version, Me.Entry.ProjectVersion.Display)).Replace(".jar", "")
            Dim text2 As String = (If(Me.Entry.UpdateFile.Version, Me.Entry.UpdateFile.Display)).Replace(".jar", "")
            Dim list As List(Of String) = text.Split("-_+ ".ToCharArray()).ToList()
            Dim list2 As List(Of String) = text2.Split("-_+ ".ToCharArray()).ToList()
            Dim flag As Boolean = False
            Try
                For Each text3 As String In list.ToList()
                    If list2.Contains(text3) Then
                        list.Remove(text3)
                        list2.Remove(text3)
                        flag = True
                    End If
                Next
            Finally
                Dim enumerator As List(Of String).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            If flag AndAlso list.Any() AndAlso list2.Any() Then
                text = list.Join("-")
                text2 = list2.Join("-")
                Me.Entry._Version = text
            End If
            Return String.Format("当前版本：{0}（{1}）{2}最新版本：{3}（{4}）", New Object() { text, StringUtils.FormatTimeSpan(Me.Entry.ProjectVersion.ReleaseDate - DateTime.Now, False), vbCrLf, text2, StringUtils.FormatTimeSpan(Me.Entry.UpdateFile.ReleaseDate - DateTime.Now, False) })
        End Function

        Public Sub New()
            AddHandler MyBase.PreviewMouseLeftButtonUp, AddressOf Me.Button_MouseUp
            AddHandler MyBase.PreviewMouseLeftButtonDown, AddressOf Me.Button_MouseDown
            AddHandler MyBase.MouseLeave, AddressOf Me.Button_MouseLeave
            AddHandler MyBase.PreviewMouseLeftButtonUp, AddressOf Me.Button_MouseLeave
            AddHandler MyBase.MouseLeftButtonDown, AddressOf Me.Button_MouseSwipeStart
            AddHandler MyBase.MouseEnter, AddressOf Me.Button_MouseSwipe
            AddHandler MyBase.MouseLeave, AddressOf Me.Button_MouseSwipe
            AddHandler MyBase.MouseLeftButtonUp, AddressOf Me.Button_MouseSwipe
            AddHandler MyBase.MouseEnter, AddressOf Me.RefreshColor
            AddHandler MyBase.MouseLeave, AddressOf Me.RefreshColor
            AddHandler MyBase.MouseLeftButtonDown, AddressOf Me.RefreshColor
            AddHandler MyBase.MouseLeftButtonUp, AddressOf Me.RefreshColor
            AddHandler Me.Changed, AddressOf Me.RefreshColor
            Me.Uuid = ModBase.GetUuid()
            Me.IsMouseDown = False
            Me._Checked = False
            Me._RectBack = Nothing
            Me.HasEnteredViewport = False
            Me.InitializeComponent()
            LazyLoadBehavior.OnFirstEnterScrollViewerViewport(Me, AddressOf Me.FirstEnterScrollViewerViewport)
        End Sub

        Private Sub FirstEnterScrollViewerViewport()
            Me.HasEnteredViewport = True
            Me.Refresh()
        End Sub

        Public Sub Refresh()
            If Me.HasEnteredViewport Then
                If Me.Entry.HasUpdate Then
                    Me.BtnUpdate.Visibility = Visibility.Visible
                    Me.BtnUpdate.ToolTip = String.Format("{0}{1}点击以更新，右键查看更新日志。", Me.GetUpdateCompareDescription(), vbCrLf)
                Else
                    Me.BtnUpdate.Visibility = Visibility.Collapsed
                End If
                Dim fileNameWithoutExtension As String = PathUtils.GetFileNameWithoutExtension(Me.Entry.EnabledName)
                Dim text As String
                If Settings.[Get](Of Integer)("ToolModLocalNameStyle", Nothing) = 1 Then
                    Me.Title = fileNameWithoutExtension
                    Me.SubTitle = ""
                    If Me.Entry.Project Is Nothing Then
                        text = Me.Entry.Display
                    Else
                        Dim controlTitle As ValueTuple(Of String, String) = Me.Entry.Project.GetControlTitle(False)
                        text = controlTitle.Item1 + controlTitle.Item2
                    End If
                    text = text.Replace("  |  ", " / ")
                    If Me.Entry.Version IsNot Nothing Then
                        text += String.Format(" ({0})", Me.Entry.Version)
                    End If
                Else
                    If Me.Entry.Project Is Nothing Then
                        Me.Title = Me.Entry.Display
                        Me.SubTitle = If((Me.Entry.Version Is Nothing), "", ("  |  " + Me.Entry.Version))
                    Else
                        Dim controlTitle2 As ValueTuple(Of String, String) = Me.Entry.Project.GetControlTitle(False)
                        Me.Title = controlTitle2.Item1
                        Me.SubTitle = controlTitle2.Item2 + If((Me.Entry.Version Is Nothing), "", ("  |  " + Me.Entry.Version))
                    End If
                    text = fileNameWithoutExtension
                End If
                If Me.Entry.Project IsNot Nothing Then
                    text = text + ": " + Me.Entry.Project.Description.ReplaceLineEndings("", False)
                ElseIf Me.Entry.Description IsNot Nothing Then
                    text = text + ": " + Me.Entry.Description.ReplaceLineEndings("", False)
                End If
                Me.Description = text
                If Me.Checked Then
                    Me.LabTitle.SetResourceReference(TextBlock.ForegroundProperty, If(Me.Entry.IsEnabled, "ColorBrush2", "ColorBrush5"))
                Else
                    Me.LabTitle.SetResourceReference(TextBlock.ForegroundProperty, If(Me.Entry.IsEnabled, "ColorBrush1", "ColorBrushGray4"))
                End If
                If Me.Entry.Project IsNot Nothing Then
                    Me.Entry.Project.ApplyLogoToMyImage(Me.PathLogo)
                Else
                    Me.PathLogo.Source = ModBase.PathImage + "Icons/NoIcon.png"
                End If
                If Me.Entry.IsEnabled Then
                    If Me.ImgState IsNot Nothing Then
                        MyBase.Children.Remove(Me.ImgState)
                        Me.ImgState = Nothing
                    End If
                Else
                    If Me.ImgState Is Nothing Then
                        Me.ImgState = New Image() With { .Width = 20.0, .Height = 20.0, .Margin = New Thickness(0.0, 0.0, -5.0, -3.0), .IsHitTestVisible = False, .HorizontalAlignment = HorizontalAlignment.Right, .VerticalAlignment = VerticalAlignment.Bottom }
                        RenderOptions.SetBitmapScalingMode(Me.ImgState, BitmapScalingMode.HighQuality)
                        Grid.SetColumn(Me.ImgState, 1)
                        Grid.SetRow(Me.ImgState, 1)
                        Grid.SetRowSpan(Me.ImgState, 2)
                        MyBase.Children.Add(Me.ImgState)
                    End If
                    Me.ImgState.Source = New MyBitmap(ModBase.PathImage + "Icons/Disabled.png")
                End If
                If Me.Entry.Project IsNot Nothing Then
                    Me.Tags = Me.Entry.Project.Tags
                End If
            End If
        End Sub

        Public Sub RefreshColor(sender As Object, e As EventArgs)
            Me.InitLate(RuntimeHelpers.GetObjectValue(sender), e)
            Dim num As Integer = If(MyBase.IsMouseOver, 120, 180)
            Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
            If Me.ButtonStack IsNot Nothing Then
                If MyBase.IsMouseOver Then
                    ' The following expression was wrapped in a unchecked-expression
                    ' The following expression was wrapped in a unchecked-expression
                    ' The following expression was wrapped in a unchecked-expression
                    list.Add(ModAnimation.AaOpacity(Me.ButtonStack, 1.0 - Me.ButtonStack.Opacity, CInt(Math.Round(CDbl(num) * 0.7)), CInt(Math.Round(CDbl(num) * 0.3)), Nothing, False))
                    list.Add(ModAnimation.AaDouble(Sub(i As Object)
                        Me.ColumnPaddingRight.Width = New GridLength(Conversions.ToDouble(NewLateBinding.LateGet(Nothing, GetType(Math), "Max", New Object() { 0, Operators.AddObject(Me.ColumnPaddingRight.Width.Value, i) }, Nothing, Nothing, Nothing)))
                    End Sub, CDbl((5 + Me.Buttons.Count() * 25)) - Me.ColumnPaddingRight.Width.Value, CInt(Math.Round(CDbl(num) * 0.3)), CInt(Math.Round(CDbl(num) * 0.7)), Nothing, False))
                Else
                    ' The following expression was wrapped in a unchecked-expression
                    ' The following expression was wrapped in a unchecked-expression
                    list.Add(ModAnimation.AaOpacity(Me.ButtonStack, -Me.ButtonStack.Opacity, CInt(Math.Round(CDbl(num) * 0.4)), 0, Nothing, False))
                    list.Add(ModAnimation.AaDouble(Sub(i As Object)
                        Me.ColumnPaddingRight.Width = New GridLength(Conversions.ToDouble(NewLateBinding.LateGet(Nothing, GetType(Math), "Max", New Object() { 0, Operators.AddObject(Me.ColumnPaddingRight.Width.Value, i) }, Nothing, Nothing, Nothing)))
                    End Sub, 4.0 - Me.ColumnPaddingRight.Width.Value, CInt(Math.Round(CDbl(num) * 0.4)), 0, Nothing, False))
                End If
            End If
            If MyBase.IsMouseOver OrElse Me.Checked Then
                list.AddRange(New ModAnimation.AniData() { ModAnimation.AaColor(Me.RectBack, Border.BackgroundProperty, If(Me.IsMouseDown, "ColorBrush6", "ColorBrushBg1"), num, 0, Nothing, False), ModAnimation.AaOpacity(Me.RectBack, 1.0 - Me.RectBack.Opacity, num, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) })
                If Me.IsMouseDown Then
                    ' The following expression was wrapped in a unchecked-expression
                    ' The following expression was wrapped in a unchecked-expression
                    list.Add(ModAnimation.AaScaleTransform(Me.RectBack, 0.996 - CType(Me.RectBack.RenderTransform, ScaleTransform).ScaleX, CInt(Math.Round(CDbl(num) * 1.2)), 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False))
                Else
                    ' The following expression was wrapped in a unchecked-expression
                    ' The following expression was wrapped in a unchecked-expression
                    list.Add(ModAnimation.AaScaleTransform(Me.RectBack, 1.0 - CType(Me.RectBack.RenderTransform, ScaleTransform).ScaleX, CInt(Math.Round(CDbl(num) * 1.2)), 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False))
                End If
            Else
                list.AddRange(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.RectBack, -Me.RectBack.Opacity, num, 0, Nothing, False), ModAnimation.AaScaleTransform(Me.RectBack, 0.996 - CType(Me.RectBack.RenderTransform, ScaleTransform).ScaleX, num, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaScaleTransform(Me.RectBack, -0.196, 1, 0, Nothing, True) })
            End If
            ModAnimation.AniStart(list, "LocalModItem Color " + Conversions.ToString(Me.Uuid), False)
        End Sub

        Private Sub InitLate(sender As Object, e As EventArgs)
            If Me.ButtonHandler IsNot Nothing Then
                Me.ButtonHandler(CType(sender, MyLocalModItem), e)
                Me.ButtonHandler = Nothing
            End If
        End Sub

        Private Sub BtnUpdate_PreviewMouseRightButtonUp(sender As Object, e As MouseButtonEventArgs)
            e.Handled = True
            Me.ShowUpdateLog()
        End Sub

        Private Sub ShowUpdateLog()
            Dim changelogUrls As IEnumerable(Of String) = Me.Entry.ChangelogUrls
            Dim func As Func(Of String, Boolean)
            If MyLocalModItem._Closure$__.$I70-0 IsNot Nothing Then
                func = MyLocalModItem._Closure$__.$I70-0
            Else
                Dim func2 As Func(Of String, Boolean) = Function(x As String) x.Contains("curseforge.com")
                func = func2
                MyLocalModItem._Closure$__.$I70-0 = func2
            End If
            Dim text As String = changelogUrls.FirstOrDefault(func)
            Dim changelogUrls2 As IEnumerable(Of String) = Me.Entry.ChangelogUrls
            Dim func3 As Func(Of String, Boolean)
            If MyLocalModItem._Closure$__.$I70-1 IsNot Nothing Then
                func3 = MyLocalModItem._Closure$__.$I70-1
            Else
                Dim func4 As Func(Of String, Boolean) = Function(x As String) x.Contains("modrinth.com")
                func3 = func4
                MyLocalModItem._Closure$__.$I70-1 = func4
            End If
            Dim text2 As String = changelogUrls2.FirstOrDefault(func3)
            If text Is Nothing OrElse text2 Is Nothing Then
                ModBase.OpenWebsite(Me.Entry.ChangelogUrls.First())
                Return
            End If
            Dim num As Integer = ModMain.MyMsgBox("要在哪个网站上查看更新日志？", "查看更新日志", "Modrinth", "CurseForge", "取消", False, True, False, Nothing, Nothing, Nothing)
            If num = 1 Then
                ModBase.OpenWebsite(text2)
                Return
            End If
            If num <> 2 Then
                Return
            End If
            ModBase.OpenWebsite(text)
        End Sub

        Private Sub BtnUpdate_Click(sender As Object, e As EventArgs)
            Select Case ModMain.MyMsgBox(String.Format("是否要更新 {0}？{1}{2}{3}", New Object() { Me.Entry.Display, vbCrLf, vbCrLf, Me.GetUpdateCompareDescription() }), "Mod 更新确认", "更新", "查看更新日志", "取消", False, True, False, Nothing, Nothing, Nothing)
                Case 1
                    ModMain.FrmInstanceMod.UpdateMods(New LocalResourceFile() { Me.Entry })
                    Return
                Case 2
                    Me.ShowUpdateLog()
                Case 3
                Case Else
                    Return
            End Select
        End Sub

        Private Sub PanTitle_SizeChanged()
            Dim num As Integer
            Select Case If(Me.ColumnExtend.Width.IsStar, 0, If(Me.ColumnTitle.Width.IsStar, 2, 1))
                Case 0
                    If Me.ColumnExtend.ActualWidth >= 0.5 Then
                        Return
                    End If
                    num = If((Me.LabSubtitle.Visibility = Visibility.Collapsed), 2, 1)
                Case 1
                    If Me.ColumnSubtitle.ActualWidth < 0.5 Then
                        num = 2
                    Else
                        If Me.LabSubtitle.IsTextTrimmed() Then
                            Return
                        End If
                        num = 0
                    End If
                Case 2
                    If Me.LabTitle.IsTextTrimmed() Then
                        Return
                    End If
                    num = If((Me.LabSubtitle.Visibility <> Visibility.Collapsed), 1, 0)
            End Select
            Select Case num
                Case 0
                    Me.ColumnTitle.Width = GridLength.Auto
                    Me.ColumnSubtitle.Width = GridLength.Auto
                    Me.ColumnExtend.Width = New GridLength(1.0, GridUnitType.Star)
                    Return
                Case 1
                    Me.ColumnTitle.Width = GridLength.Auto
                    Me.ColumnSubtitle.Width = New GridLength(1.0, GridUnitType.Star)
                    Me.ColumnExtend.Width = New GridLength(0.0, GridUnitType.Pixel)
                    Return
                Case 2
                    Me.ColumnTitle.Width = New GridLength(1.0, GridUnitType.Star)
                    Me.ColumnSubtitle.Width = New GridLength(0.0, GridUnitType.Pixel)
                    Me.ColumnExtend.Width = New GridLength(0.0, GridUnitType.Pixel)
                Case Else
                    Return
            End Select
        End Sub

        Friend Overridable Property PanBack As MyLocalModItem

        Friend Overridable Property ColumnPaddingRight As ColumnDefinition

        Friend Overridable Property PathLogo As MyImage

        Friend Overridable Property PanTitle As Grid
            <CompilerGenerated()>
            Get
                Return Me._PanTitle
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Grid)
                Dim sizeChangedEventHandler As SizeChangedEventHandler = Sub(a0 As Object, a1 As SizeChangedEventArgs)
                    Me.PanTitle_SizeChanged()
                End Sub
                Dim grid As Grid = Me._PanTitle
                If grid IsNot Nothing Then
                    RemoveHandler grid.SizeChanged, sizeChangedEventHandler
                End If
                Me._PanTitle = value
                grid = Me._PanTitle
                If grid IsNot Nothing Then
                    AddHandler grid.SizeChanged, sizeChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ColumnTitle As ColumnDefinition

        Friend Overridable Property ColumnSubtitle As ColumnDefinition

        Friend Overridable Property ColumnExtend As ColumnDefinition

        Friend Overridable Property LabTitle As TextBlock

        Friend Overridable Property LabSubtitle As TextBlock

        Friend Overridable Property BtnUpdate As MyIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnUpdate
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = AddressOf Me.BtnUpdate_PreviewMouseRightButtonUp
                Dim clickEventHandler As MyIconButton.ClickEventHandler = AddressOf Me.BtnUpdate_Click
                Dim myIconButton As MyIconButton = Me._BtnUpdate
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.PreviewMouseRightButtonUp, mouseButtonEventHandler
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnUpdate = value
                myIconButton = Me._BtnUpdate
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.PreviewMouseRightButtonUp, mouseButtonEventHandler
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanTags As StackPanel

        Friend Overridable Property LabInfo As TextBlock

        Public Uuid As Integer

        Private _Title As String

        Private IsMouseDown As Boolean

        Private Shared SwipeStart As Integer

        Private Shared SwipeEnd As Integer

        Private Shared _Swiping As Boolean = False

        Private Shared SwipToState As Boolean

        Private _Checked As Boolean

        Private ImgState As Image

        Private _RectBack As Border

        Public ButtonHandler As Action(Of MyLocalModItem, EventArgs)

        Public ButtonStack As FrameworkElement

        Private _Buttons As IEnumerable(Of MyIconButton)

        Private _RectCheck As Border

        Private HasEnteredViewport As Boolean

        Public Delegate Sub ClickEventHandler(sender As Object, e As MouseButtonEventArgs)

        Public Delegate Sub CheckEventHandler(sender As Object, e As ModBase.RouteEventArgs)

        Public Delegate Sub ChangedEventHandler(sender As Object, e As ModBase.RouteEventArgs)
    End Class
End Namespace
