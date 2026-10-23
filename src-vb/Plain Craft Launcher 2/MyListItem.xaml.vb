Imports System
Imports System.CodeDom.Compiler
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <ContentProperty("Inlines")>
    <DesignerGenerated()>
    Public Partial Class MyListItem
        Inherits Grid
        Implements IMyRadio

        Public Sub New()
            AddHandler MyBase.SizeChanged, Sub(a0 As Object, a1 As SizeChangedEventArgs)
                Me.OnSizeChanged()
            End Sub
            AddHandler MyBase.PreviewMouseLeftButtonUp, AddressOf Me.Button_MouseUp
            AddHandler MyBase.PreviewMouseLeftButtonDown, AddressOf Me.Button_MouseDown
            AddHandler MyBase.MouseLeave, AddressOf Me.Button_MouseLeave
            AddHandler MyBase.PreviewMouseLeftButtonUp, AddressOf Me.Button_MouseLeave
            AddHandler MyBase.MouseEnter, AddressOf Me.RefreshColor
            AddHandler MyBase.MouseLeave, AddressOf Me.RefreshColor
            AddHandler MyBase.PreviewMouseLeftButtonDown, AddressOf Me.RefreshColor
            AddHandler MyBase.PreviewMouseLeftButtonUp, AddressOf Me.RefreshColor
            AddHandler MyBase.Loaded, AddressOf Me.MyListItem_Loaded
            Me._RectBack = Nothing
            Me._LabInfo = Nothing
            Me.Uuid = ModBase.GetUuid()
            Me._IsScaleAnimationEnabled = True
            Me.MinPaddingRight = 4
            Me._Info = ""
            Me._Logo = ""
            Me._LogoScale = 1.0
            Me.LogoClickable = False
            Me.IsLogoDown = False
            Me._Type = MyListItem.CheckType.None
            Me._Checked = False
            Me.IsMouseDown = False
            Me.IsMouseOverAnimationEnabled = True
            Me.InitializeComponent()
        End Sub

        Public Event Click As MyListItem.ClickEventHandler

        Public Event LogoClick As MyListItem.LogoClickEventHandler

        Public Event Check As IMyRadio.CheckEventHandler Implements PCL.IMyRadio.Check

        Public Event Changed As IMyRadio.ChangedEventHandler Implements PCL.IMyRadio.Changed

        Public ReadOnly Property RectBack As Border
            Get
                If Me._RectBack Is Nothing Then
                    Dim border As Border = New Border() With { .Name = "RectBack", .CornerRadius = New CornerRadius(CDbl(If((Me.IsScaleAnimationEnabled OrElse MyBase.Height > 40.0), 6, 0))), .RenderTransform = If(Me.IsScaleAnimationEnabled, New ScaleTransform(0.8, 0.8), Nothing), .RenderTransformOrigin = New Point(0.5, 0.5), .BorderThickness = New Thickness(ModBase.GetWPFSize(1.0)), .SnapsToDevicePixels = True, .IsHitTestVisible = False, .Opacity = 0.0 }
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

        Public ReadOnly Property LabInfo As TextBlock
            Get
                If Me._LabInfo Is Nothing Then
                    Dim textBlock As TextBlock = New TextBlock() With { .Name = "LabInfo", .SnapsToDevicePixels = False, .UseLayoutRounding = False, .HorizontalAlignment = HorizontalAlignment.Left, .IsHitTestVisible = False, .TextTrimming = TextTrimming.CharacterEllipsis, .Visibility = Visibility.Collapsed, .FontSize = 12.0, .Margin = New Thickness(4.0, 0.0, 0.0, 0.0), .Opacity = 0.6 }
                    Grid.SetColumn(textBlock, 3)
                    Grid.SetRow(textBlock, 2)
                    MyBase.Children.Add(textBlock)
                    Me._LabInfo = textBlock
                End If
                Return Me._LabInfo
            End Get
        End Property

        Public Property IsScaleAnimationEnabled As Boolean
            Get
                Return Me._IsScaleAnimationEnabled
            End Get
            Set(value As Boolean)
                Me._IsScaleAnimationEnabled = value
                If Me._RectBack IsNot Nothing Then
                    Me.RectBack.CornerRadius = New CornerRadius(CDbl(If(value, 6, 0)))
                End If
            End Set
        End Property

        Public Property PaddingLeft As Integer
            Get
                ' The following expression was wrapped in a checked-expression
                Return CInt(Math.Round(Me.ColumnPaddingLeft.Width.Value))
            End Get
            Set(value As Integer)
                Me.ColumnPaddingLeft.Width = New GridLength(CDbl(value))
            End Set
        End Property

        Public Property MinPaddingRight As Integer

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
                Dim num As Integer = value.Count()
                If num <> 0 Then
                    If num = 1 Then
                        Try
                            For Each myIconButton As MyIconButton In value
                                If myIconButton.Height.Equals(Double.NaN) Then
                                    myIconButton.Height = 25.0
                                End If
                                If myIconButton.Width.Equals(Double.NaN) Then
                                    myIconButton.Width = 25.0
                                End If
                                Dim myIconButton2 As MyIconButton = myIconButton
                                myIconButton2.Opacity = 0.0
                                myIconButton2.Margin = New Thickness(0.0, 0.0, 5.0, 0.0)
                                myIconButton2.SnapsToDevicePixels = False
                                myIconButton2.HorizontalAlignment = HorizontalAlignment.Right
                                myIconButton2.VerticalAlignment = VerticalAlignment.Center
                                myIconButton2.SnapsToDevicePixels = False
                                myIconButton2.UseLayoutRounding = False
                                Grid.SetColumnSpan(myIconButton, 10)
                                Grid.SetRowSpan(myIconButton, 10)
                                MyBase.Children.Add(myIconButton)
                                Me.ButtonStack = myIconButton
                            Next
                            Return
                        Finally
                            Dim enumerator As IEnumerator(Of MyIconButton)
                            If enumerator IsNot Nothing Then
                                enumerator.Dispose()
                            End If
                        End Try
                    End If
                    Me.ButtonStack = New StackPanel() With { .Opacity = 0.0, .Margin = New Thickness(0.0, 0.0, 5.0, 0.0), .SnapsToDevicePixels = False, .Orientation = Orientation.Horizontal, .HorizontalAlignment = HorizontalAlignment.Right, .VerticalAlignment = VerticalAlignment.Center, .UseLayoutRounding = False }
                    Grid.SetColumnSpan(Me.ButtonStack, 10)
                    Grid.SetRowSpan(Me.ButtonStack, 10)
                    Try
                        For Each myIconButton3 As MyIconButton In value
                            If myIconButton3.Height.Equals(Double.NaN) Then
                                myIconButton3.Height = 25.0
                            End If
                            If myIconButton3.Width.Equals(Double.NaN) Then
                                myIconButton3.Width = 25.0
                            End If
                            CType(Me.ButtonStack, StackPanel).Children.Add(myIconButton3)
                        Next
                    Finally
                        Dim enumerator2 As IEnumerator(Of MyIconButton)
                        If enumerator2 IsNot Nothing Then
                            enumerator2.Dispose()
                        End If
                    End Try
                    MyBase.Children.Add(Me.ButtonStack)
                End If
            End Set
        End Property

        Public ReadOnly Property Inlines As InlineCollection
            Get
                Return Me.LabTitle.Inlines
            End Get
        End Property

        Public Property Title As String
            Get
                Return Conversions.ToString(MyBase.GetValue(MyListItem.TitleProperty))
            End Get
            Set(value As String)
                MyBase.SetValue(MyListItem.TitleProperty, value.ReplaceLineEndings("", False))
            End Set
        End Property

        Public Property FontSize As Double
            Get
                Return Conversions.ToDouble(MyBase.GetValue(MyListItem.FontSizeProperty))
            End Get
            Set(value As Double)
                MyBase.SetValue(MyListItem.FontSizeProperty, value)
            End Set
        End Property

        Public Property Info As String
            Get
                Return Me._Info
            End Get
            Set(value As String)
                If Operators.CompareString(Me._Info, value, False) <> 0 Then
                    value = value.ReplaceLineEndings("", False)
                    Me._Info = value
                    Me.LabInfo.Text = value
                    Me.LabInfo.Visibility = If((Operators.CompareString(value, "", False) = 0), Visibility.Collapsed, Visibility.Visible)
                End If
            End Set
        End Property

        Public Property Logo As String
            Get
                Return Me._Logo
            End Get
            Set(value As String)
                If Operators.CompareString(Me._Logo, value, False) <> 0 Then
                    Me._Logo = value
                    If Not Information.IsNothing(Me.PathLogo) Then
                        MyBase.Children.Remove(Me.PathLogo)
                    End If
                    If Operators.CompareString(Me._Logo, "", False) <> 0 Then
                        If Me._Logo.StartsWithF("http", True) Then
                            Me.PathLogo = New MyImage() With { .Tag = Me, .IsHitTestVisible = Me.LogoClickable, .Source = Me._Logo, .RenderTransformOrigin = New Point(0.5, 0.5), .RenderTransform = New ScaleTransform() With { .ScaleX = Me.LogoScale, .ScaleY = Me.LogoScale }, .SnapsToDevicePixels = True, .UseLayoutRounding = False }
                            RenderOptions.SetBitmapScalingMode(Me.PathLogo, BitmapScalingMode.LowQuality)
                        ElseIf Me._Logo.EndsWithF(".png", True) OrElse Me._Logo.EndsWithF(".jpg", True) OrElse Me._Logo.EndsWithF(".webp", True) Then
                            Me.PathLogo = New Canvas() With { .Tag = Me, .IsHitTestVisible = Me.LogoClickable, .Background = New MyBitmap(Me._Logo), .RenderTransformOrigin = New Point(0.5, 0.5), .RenderTransform = New ScaleTransform() With { .ScaleX = Me.LogoScale, .ScaleY = Me.LogoScale }, .SnapsToDevicePixels = True, .UseLayoutRounding = False, .HorizontalAlignment = HorizontalAlignment.Stretch, .VerticalAlignment = VerticalAlignment.Stretch }
                            RenderOptions.SetBitmapScalingMode(Me.PathLogo, BitmapScalingMode.LowQuality)
                        Else
                            Me.PathLogo = New Path() With { .Tag = Me, .IsHitTestVisible = Me.LogoClickable, .HorizontalAlignment = HorizontalAlignment.Center, .VerticalAlignment = VerticalAlignment.Center, .Stretch = Stretch.Uniform, .Data = CType(New GeometryConverter().ConvertFromString(Me._Logo), Geometry), .RenderTransformOrigin = New Point(0.5, 0.5), .RenderTransform = New ScaleTransform() With { .ScaleX = Me.LogoScale, .ScaleY = Me.LogoScale }, .SnapsToDevicePixels = False, .UseLayoutRounding = False }
                            Me.PathLogo.SetBinding(Shape.FillProperty, New Binding("Foreground") With { .Source = Me })
                        End If
                        Grid.SetColumn(Me.PathLogo, 2)
                        Grid.SetRowSpan(Me.PathLogo, 4)
                        Me.OnSizeChanged()
                        MyBase.Children.Add(Me.PathLogo)
                        If Me.LogoClickable Then
                            AddHandler Me.PathLogo.MouseLeave, Sub(sender As Object, e As MouseEventArgs)
                                Me.IsLogoDown = False
                            End Sub
                            AddHandler Me.PathLogo.MouseLeftButtonDown, Sub(sender As Object, e As MouseButtonEventArgs)
                                Me.IsLogoDown = True
                            End Sub
                            AddHandler Me.PathLogo.MouseLeftButtonUp, Sub(sender As Object, e As MouseButtonEventArgs)
                                If Me.IsLogoDown Then
                                    Me.IsLogoDown = False
                                    Dim logoClickEvent As MyListItem.LogoClickEventHandler = Me.LogoClickEvent
                                    If logoClickEvent IsNot Nothing Then
                                        logoClickEvent(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing)), e)
                                    End If
                                End If
                            End Sub
                        End If
                    End If
                    Me.ColumnLogo.Width = New GridLength(CDbl((If((Operators.CompareString(Me._Logo, "", False) = 0), 0, 34) + If((MyBase.Height < 40.0), 0, 4))))
                End If
            End Set
        End Property

        Public Property LogoScale As Double
            Get
                Return Me._LogoScale
            End Get
            Set(value As Double)
                Me._LogoScale = value
                If Not Information.IsNothing(Me.PathLogo) Then
                    Me.PathLogo.RenderTransform = New ScaleTransform() With { .ScaleX = Me.LogoScale, .ScaleY = Me.LogoScale }
                End If
            End Set
        End Property

        Public Property LogoClickable As Boolean

        Public Property Type As MyListItem.CheckType
            Get
                Return Me._Type
            End Get
            Set(value As MyListItem.CheckType)
                If Me._Type <> value Then
                    Me._Type = value
                    Me.ColumnCheck.Width = New GridLength(CDbl(If((Me._Type = MyListItem.CheckType.None OrElse Me._Type = MyListItem.CheckType.Clickable), If((MyBase.Height < 40.0), 4, 2), 6)))
                    If Me._Type = MyListItem.CheckType.None OrElse Me._Type = MyListItem.CheckType.Clickable Then
                        If Not Information.IsNothing(Me.RectCheck) Then
                            MyBase.Children.Remove(Me.RectCheck)
                            Me.RectCheck = Nothing
                        End If
                        Me.SetChecked(False, False, False)
                        Return
                    End If
                    If Information.IsNothing(Me.RectCheck) Then
                        Me.RectCheck = New Border() With { .Width = 5.0, .Height = If(Me.Checked, Double.NaN, 0.0), .CornerRadius = New CornerRadius(2.0, 2.0, 2.0, 2.0), .VerticalAlignment = If(Me.Checked, VerticalAlignment.Stretch, VerticalAlignment.Center), .HorizontalAlignment = HorizontalAlignment.Left, .UseLayoutRounding = False, .SnapsToDevicePixels = False, .Margin = If(Me.Checked, New Thickness(-1.0, 6.0, 0.0, 6.0), New Thickness(-1.0, 0.0, 0.0, 0.0)) }
                        Me.RectCheck.SetResourceReference(Border.BackgroundProperty, "ColorBrush3")
                        Grid.SetRowSpan(Me.RectCheck, 4)
                        MyBase.Children.Add(Me.RectCheck)
                    End If
                End If
            End Set
        End Property

        Private Sub OnSizeChanged()
            Me.ColumnCheck.Width = New GridLength(CDbl(If((Me._Type = MyListItem.CheckType.None OrElse Me._Type = MyListItem.CheckType.Clickable), If((MyBase.Height < 40.0), 4, 2), 6)))
            Me.ColumnLogo.Width = New GridLength(CDbl((If((Operators.CompareString(Me._Logo, "", False) = 0), 0, 34) + If((MyBase.Height < 40.0), 0, 4))))
            If Me.PathLogo IsNot Nothing Then
                If Me._Logo.EndsWithF(".png", True) OrElse Me._Logo.EndsWithF(".jpg", True) OrElse Me._Logo.EndsWithF(".webp", True) Then
                    Me.PathLogo.Margin = New Thickness(4.0, 5.0, 3.0, 5.0)
                Else
                    Me.PathLogo.Margin = New Thickness(CDbl(If((MyBase.Height < 40.0), 6, 8)), 8.0, CDbl(If((MyBase.Height < 40.0), 4, 6)), 8.0)
                End If
            End If
            Me.LabTitle.Margin = New Thickness(4.0, 0.0, 0.0, CDbl(If((MyBase.Height < 40.0), 0, 2)))
        End Sub

        Public Property Checked As Boolean
            Get
                Return Me._Checked
            End Get
            Set(value As Boolean)
                Me.SetChecked(value, False, value <> Me._Checked)
            End Set
        End Property

        Public Sub SetChecked(value As Boolean, user As Boolean, anime As Boolean)
            Try
                Dim e As ModBase.RouteEventArgs = New ModBase.RouteEventArgs(user)
                Dim checked As Boolean = Me._Checked
                If Me.Type = MyListItem.CheckType.RadioBox Then
                    If MyBase.IsInitialized AndAlso value <> Me._Checked Then
                        Me._Checked = value
                        Dim changedEvent As IMyRadio.ChangedEventHandler = Me.ChangedEvent
                        If changedEvent IsNot Nothing Then
                            changedEvent(Me, e)
                        End If
                        If e.Handled Then
                            Me._Checked = checked
                            Return
                        End If
                    End If
                    Me._Checked = value
                Else
                    If value = Me._Checked Then
                        Return
                    End If
                    Me._Checked = value
                    If MyBase.IsInitialized Then
                        Dim changedEvent2 As IMyRadio.ChangedEventHandler = Me.ChangedEvent
                        If changedEvent2 IsNot Nothing Then
                            changedEvent2(Me, e)
                        End If
                        If e.Handled Then
                            Me._Checked = checked
                            Return
                        End If
                    End If
                End If
                If value Then
                    Dim e2 As ModBase.RouteEventArgs = New ModBase.RouteEventArgs(user)
                    Dim checkEvent As IMyRadio.CheckEventHandler = Me.CheckEvent
                    If checkEvent IsNot Nothing Then
                        checkEvent(Me, e2)
                    End If
                    If e2.Handled Then
                        Return
                    End If
                End If
                If Me.Type = MyListItem.CheckType.RadioBox Then
                    If Information.IsNothing(MyBase.Parent) Then
                        Return
                    End If
                    Dim list As List(Of MyListItem) = New List(Of MyListItem)()
                    Dim num As Integer = 0
                    Try
                        For Each obj As Object In CType(NewLateBinding.LateGet(MyBase.Parent, Nothing, "Children", New Object(-1) {}, Nothing, Nothing, Nothing), IEnumerable)
                            Dim obj2 As Object = RuntimeHelpers.GetObjectValue(obj)
                            obj2 = MyVirtualizingElement.TryInit(CType(obj2, FrameworkElement))
                            If TypeOf obj2 Is MyListItem AndAlso CType(obj2, MyListItem).Type = MyListItem.CheckType.RadioBox Then
                                list.Add(CType(obj2, MyListItem))
                                If Conversions.ToBoolean(NewLateBinding.LateGet(obj2, Nothing, "Checked", New Object(-1) {}, Nothing, Nothing, Nothing)) Then
                                    num += 1
                                End If
                            End If
                        Next
                    Finally
                        Dim enumerator As IEnumerator
                        If TypeOf enumerator Is IDisposable Then
                            TryCast(enumerator, IDisposable).Dispose()
                        End If
                    End Try
                    Dim num2 As Integer = num
                    If num2 = 0 Then
                        list(0).Checked = True
                    ElseIf num2 > 1 Then
                        If Me.Checked Then
                            Try
                                For Each myListItem As MyListItem In list
                                    If myListItem.Checked AndAlso Not myListItem.Equals(Me) Then
                                        myListItem.Checked = False
                                    End If
                                Next
                                GoTo IL_0263
                            Finally
                                Dim enumerator2 As List(Of MyListItem).Enumerator
                                CType(enumerator2, IDisposable).Dispose()
                            End Try
                        End If
                        Dim flag As Boolean = False
                        Try
                            For Each myListItem2 As MyListItem In list
                                If myListItem2.Checked Then
                                    If flag Then
                                        myListItem2.Checked = False
                                    Else
                                        flag = True
                                    End If
                                End If
                            Next
                        Finally
                            Dim enumerator3 As List(Of MyListItem).Enumerator
                            CType(enumerator3, IDisposable).Dispose()
                        End Try
                    End If
                End If
                IL_0263:
                If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 AndAlso anime Then
                    Dim list2 As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
                    If Me.Checked Then
                        If Not Information.IsNothing(Me.RectCheck) Then
                            Dim num3 As Double = MyBase.ActualHeight - Me.RectCheck.ActualHeight - 12.0
                            list2.Add(ModAnimation.AaHeight(Me.RectCheck, num3 * 0.4, 200, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False))
                            list2.Add(ModAnimation.AaHeight(Me.RectCheck, num3 * 0.6, 300, 0, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), False))
                            list2.Add(ModAnimation.AaOpacity(Me.RectCheck, 1.0 - Me.RectCheck.Opacity, 30, 0, Nothing, False))
                            Me.RectCheck.VerticalAlignment = VerticalAlignment.Center
                            Me.RectCheck.Margin = New Thickness(-1.0, 0.0, 0.0, 0.0)
                        End If
                        list2.Add(ModAnimation.AaColor(Me, MyListItem.ForegroundProperty, If((MyBase.Height < 40.0), "ColorBrush3", "ColorBrush2"), 200, 0, Nothing, False))
                    Else
                        If Not Information.IsNothing(Me.RectCheck) Then
                            list2.Add(ModAnimation.AaHeight(Me.RectCheck, -Me.RectCheck.ActualHeight, 120, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), False))
                            list2.Add(ModAnimation.AaOpacity(Me.RectCheck, -Me.RectCheck.Opacity, 70, 40, Nothing, False))
                            Me.RectCheck.VerticalAlignment = VerticalAlignment.Center
                        End If
                        list2.Add(ModAnimation.AaColor(Me, MyListItem.ForegroundProperty, "ColorBrush1", 120, 0, Nothing, False))
                    End If
                    ModAnimation.AniStart(list2, "MyListItem Checked " + Conversions.ToString(Me.Uuid), False)
                Else
                    ModAnimation.AniStop("MyListItem Checked " + Conversions.ToString(Me.Uuid))
                    If Me.Checked Then
                        If Not Information.IsNothing(Me.RectCheck) Then
                            Me.RectCheck.Height = Double.NaN
                            Me.RectCheck.Margin = New Thickness(-1.0, 6.0, 0.0, 6.0)
                            Me.RectCheck.Opacity = 1.0
                            Me.RectCheck.VerticalAlignment = VerticalAlignment.Stretch
                        End If
                        MyBase.SetResourceReference(MyListItem.ForegroundProperty, If((MyBase.Height < 40.0), "ColorBrush3", "ColorBrush2"))
                    Else
                        If Not Information.IsNothing(Me.RectCheck) Then
                            Me.RectCheck.Height = 0.0
                            Me.RectCheck.Margin = New Thickness(-1.0, 0.0, 0.0, 0.0)
                            Me.RectCheck.Opacity = 0.0
                            Me.RectCheck.VerticalAlignment = VerticalAlignment.Center
                        End If
                        MyBase.SetResourceReference(MyListItem.ForegroundProperty, "ColorBrush1")
                    End If
                End If
            Catch ex As Exception
                Logger.Warn(ex, "设置 Checked 失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyListItem.xaml.vb")
            End Try
        End Sub

        Public Property Foreground As Brush
            Get
                Return CType(MyBase.GetValue(MyListItem.ForegroundProperty), Brush)
            End Get
            Set(value As Brush)
                MyBase.SetValue(MyListItem.ForegroundProperty, value)
            End Set
        End Property

        Private Sub Button_MouseUp(sender As Object, e As MouseButtonEventArgs)
            If Me.IsMouseDown Then
                Dim clickEvent As MyListItem.ClickEventHandler = Me.ClickEvent
                If clickEvent IsNot Nothing Then
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e)
                End If
                If Not e.Handled Then
                    If CustomEventService.GetEventType(CType(sender, DependencyObject)) <> CustomEvent.EventType.None OrElse CustomEventService.GetEvents(CType(sender, DependencyObject)).Events.Count > 0 Then
                        Me.RaiseCustomEvent()
                        e.Handled = True
                    End If
                    If Not e.Handled Then
                        Select Case Me.Type
                            Case MyListItem.CheckType.Clickable
                                Logger.Info(String.Format("按下单击列表项：{0}", Me.Title), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyListItem.xaml.vb")
                                Return
                            Case MyListItem.CheckType.RadioBox
                                Logger.Info(String.Format("按下单选列表项：{0}", Me.Title), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyListItem.xaml.vb")
                                If Not Me.Checked Then
                                    Me.SetChecked(True, True, True)
                                    Return
                                End If
                            Case MyListItem.CheckType.CheckBox
                                Logger.Info(String.Format("按下复选列表项（{0}）：{1}", Not Me.Checked, Me.Title), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyListItem.xaml.vb")
                                Me.SetChecked(Not Me.Checked, True, True)
                            Case Else
                                Return
                        End Select
                    End If
                End If
            End If
        End Sub

        Private Sub Button_MouseDown(sender As Object, e As MouseButtonEventArgs)
            If MyBase.IsMouseDirectlyOver AndAlso Me.Type <> MyListItem.CheckType.None Then
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

        Public Sub RefreshColor(sender As Object, e As EventArgs)
            If Me.ContentHandler IsNot Nothing Then
                Me.ContentHandler(Me, e)
                Me.ContentHandler = Nothing
            End If
            Dim text As String
            Dim num As Integer
            If Me.IsMouseDown AndAlso (Me.Type <> MyListItem.CheckType.RadioBox OrElse Not Me.Checked) Then
                text = "MouseDown"
                num = 120
            ElseIf MyBase.IsMouseOver AndAlso Me.IsMouseOverAnimationEnabled Then
                text = "MouseOver"
                num = 120
            Else
                text = "Idle"
                num = 180
            End If
            If Operators.CompareString(Me.StateLast, text, False) <> 0 Then
                Me.StateLast = text
                If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                    Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
                    If MyBase.IsMouseOver AndAlso Me.IsMouseOverAnimationEnabled Then
                        If Me.ButtonStack IsNot Nothing Then
                            ' The following expression was wrapped in a unchecked-expression
                            ' The following expression was wrapped in a unchecked-expression
                            ' The following expression was wrapped in a unchecked-expression
                            list.Add(ModAnimation.AaOpacity(Me.ButtonStack, 1.0 - Me.ButtonStack.Opacity, CInt(Math.Round(CDbl(num) * 0.7)), CInt(Math.Round(CDbl(num) * 0.3)), Nothing, False))
                            list.Add(ModAnimation.AaDouble(Sub(i As Object)
                                Me.ColumnPaddingRight.Width = New GridLength(Conversions.ToDouble(NewLateBinding.LateGet(Nothing, GetType(Math), "Max", New Object() { 0, Operators.AddObject(Me.ColumnPaddingRight.Width.Value, i) }, Nothing, Nothing, Nothing)))
                            End Sub, CDbl(Math.Max(Me.MinPaddingRight, 5 + Me.Buttons.Count() * 25)) - Me.ColumnPaddingRight.Width.Value, CInt(Math.Round(CDbl(num) * 0.3)), CInt(Math.Round(CDbl(num) * 0.7)), Nothing, False))
                        End If
                        list.AddRange(New ModAnimation.AniData() { ModAnimation.AaColor(Me.RectBack, Border.BackgroundProperty, If(Me.IsMouseDown, "ColorBrush6", "ColorBrushBg1"), num, 0, Nothing, False), ModAnimation.AaOpacity(Me.RectBack, 1.0 - Me.RectBack.Opacity, num, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) })
                        If Me.IsScaleAnimationEnabled Then
                            ' The following expression was wrapped in a unchecked-expression
                            ' The following expression was wrapped in a unchecked-expression
                            list.Add(ModAnimation.AaScaleTransform(Me.RectBack, 1.0 - CType(Me.RectBack.RenderTransform, ScaleTransform).ScaleX, CInt(Math.Round(CDbl(num) * 1.6)), 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False))
                            If Me.IsMouseDown Then
                                ' The following expression was wrapped in a unchecked-expression
                                ' The following expression was wrapped in a unchecked-expression
                                list.Add(ModAnimation.AaScaleTransform(Me, 0.98 - CType(MyBase.RenderTransform, ScaleTransform).ScaleX, CInt(Math.Round(CDbl(num) * 0.9)), 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False))
                            Else
                                ' The following expression was wrapped in a unchecked-expression
                                ' The following expression was wrapped in a unchecked-expression
                                list.Add(ModAnimation.AaScaleTransform(Me, 1.0 - CType(MyBase.RenderTransform, ScaleTransform).ScaleX, CInt(Math.Round(CDbl(num) * 1.2)), 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False))
                            End If
                        End If
                    Else
                        If Me.ButtonStack IsNot Nothing Then
                            ' The following expression was wrapped in a unchecked-expression
                            ' The following expression was wrapped in a unchecked-expression
                            list.Add(ModAnimation.AaOpacity(Me.ButtonStack, -Me.ButtonStack.Opacity, CInt(Math.Round(CDbl(num) * 0.4)), 0, Nothing, False))
                            list.Add(ModAnimation.AaDouble(Sub(i As Object)
                                Me.ColumnPaddingRight.Width = New GridLength(Conversions.ToDouble(NewLateBinding.LateGet(Nothing, GetType(Math), "Max", New Object() { 0, Operators.AddObject(Me.ColumnPaddingRight.Width.Value, i) }, Nothing, Nothing, Nothing)))
                            End Sub, CDbl(Me.MinPaddingRight) - Me.ColumnPaddingRight.Width.Value, CInt(Math.Round(CDbl(num) * 0.4)), 0, Nothing, False))
                        End If

                            list.Add(ModAnimation.AaOpacity(Me.RectBack, -Me.RectBack.Opacity, num, 0, Nothing, False))
                            If Me.IsScaleAnimationEnabled Then
                                ' The following expression was wrapped in a checked-expression
                                list.AddRange(New ModAnimation.AniData() { ModAnimation.AaColor(Me.RectBack, Border.BackgroundProperty, If(Me.IsMouseDown, "ColorBrush6", "ColorBrush7"), num, 0, Nothing, False), ModAnimation.AaScaleTransform(Me, 1.0 - CType(MyBase.RenderTransform, ScaleTransform).ScaleX, num * 3, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaScaleTransform(Me.RectBack, 0.996 - CType(Me.RectBack.RenderTransform, ScaleTransform).ScaleX, num, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaScaleTransform(Me.RectBack, -0.246, 1, 0, Nothing, True) })
                            End If

                    End If
                    ModAnimation.AniStart(list, "ListItem Color " + Conversions.ToString(Me.Uuid), False)
                    Return
                End If
                If MyBase.IsMouseOver AndAlso Me.IsMouseOverAnimationEnabled Then
                    If Me.ButtonStack IsNot Nothing Then
                        Me.ButtonStack.Opacity = 1.0
                        Me.ColumnPaddingRight.Width = New GridLength(CDbl(Math.Max(Me.MinPaddingRight, 5 + Me.Buttons.Count() * 25)))
                    End If
                    Me.RectBack.Background = ModSecret.ColorBg1
                    Me.RectBack.Opacity = 1.0
                    Me.RectBack.RenderTransform = New ScaleTransform(1.0, 1.0)
                    MyBase.RenderTransform = New ScaleTransform(1.0, 1.0)
                Else
                    If Me.ButtonStack IsNot Nothing Then
                        Me.ButtonStack.Opacity = 0.0
                        Me.ColumnPaddingRight.Width = New GridLength(CDbl(Me.MinPaddingRight))
                    End If
                    MyBase.RenderTransform = New ScaleTransform(1.0, 1.0)
                    If Me._RectBack IsNot Nothing Then
                        If Me.IsScaleAnimationEnabled Then
                            Me.RectBack.RenderTransform = New ScaleTransform(0.75, 0.75)
                        End If
                        Me.RectBack.Background = ModSecret.Color7
                        Me.RectBack.Opacity = 0.0
                    End If
                End If
                ModAnimation.AniStop("ListItem Color " + Conversions.ToString(Me.Uuid))
            End If
        End Sub

        Private Sub MyListItem_Loaded(sender As Object, e As RoutedEventArgs)
            If Me.Checked Then
                MyBase.SetResourceReference(MyListItem.ForegroundProperty, If((MyBase.Height < 40.0), "ColorBrush3", "ColorBrush2"))
            Else
                MyBase.SetResourceReference(MyListItem.ForegroundProperty, "ColorBrush1")
            End If
            Me.ColumnPaddingRight.Width = New GridLength(CDbl(Me.MinPaddingRight))
            If CustomEventService.GetEventType(Me) = CustomEvent.EventType.打开帮助 AndAlso (Operators.CompareString(Me.Title, "", False) = 0 OrElse Operators.CompareString(Me.Info, "", False) = 0) Then
                Try
                    New ModMain.HelpEntry(CustomEvent.GetAbsoluteUrls(CustomEventService.GetEventData(Me), CustomEventService.GetEventType(Me))(0)).SetToListItem(Me)
                Catch ex As Exception
                    Logger.[Error](ex, "设置帮助 MyListItem 失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyListItem.xaml.vb")
                    CustomEventService.SetEventType(Me, CustomEvent.EventType.None)
                    CustomEventService.SetEventData(Me, "")
                End Try
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return Me.Title
        End Function

        Friend Overridable Property PanBack As MyListItem

        Friend Overridable Property ColumnCheck As ColumnDefinition

        Friend Overridable Property ColumnPaddingLeft As ColumnDefinition

        Friend Overridable Property ColumnLogo As ColumnDefinition

        Friend Overridable Property ColumnPaddingRight As ColumnDefinition

        Friend Overridable Property LabTitle As TextBlock

        Private _RectBack As Border

        Public ButtonStack As FrameworkElement

        Public PathLogo As FrameworkElement

        Public RectCheck As Border

        Private _LabInfo As TextBlock

        Public Uuid As Integer

        Private _IsScaleAnimationEnabled As Boolean

        Private _Buttons As IEnumerable(Of MyIconButton)

        Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register("Title", GetType(String), GetType(MyListItem))

        Public Shared FontSizeProperty As DependencyProperty = DependencyProperty.Register("FontSize", GetType(Double), GetType(MyListItem), New PropertyMetadata(14.0))

        Private _Info As String

        Private _Logo As String

        Private _LogoScale As Double

        Private IsLogoDown As Boolean

        Private _Type As MyListItem.CheckType

        Private _Checked As Boolean

        Public Shared ForegroundProperty As DependencyProperty = DependencyProperty.Register("Foreground", GetType(Brush), GetType(MyListItem), New PropertyMetadata(ModSecret.Color1))

        Public ContentHandler As Action(Of MyListItem, EventArgs)

        Private IsMouseDown As Boolean

        Private StateLast As String

        Public IsMouseOverAnimationEnabled As Boolean

        Public Delegate Sub ClickEventHandler(sender As Object, e As MouseButtonEventArgs)

        Public Delegate Sub LogoClickEventHandler(sender As Object, e As MouseButtonEventArgs)

        Public Enum CheckType
            None
            Clickable
            RadioBox
            CheckBox
        End Enum
    End Class
End Namespace
