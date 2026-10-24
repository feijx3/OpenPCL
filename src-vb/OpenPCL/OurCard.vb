Imports System
Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports MeloongCore
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class OurCard
        Inherits Grid

        Public ReadOnly Property MainChrome As OurDropShadow

        Public Property BorderChild As UIElement
            Get
                Return Me.MainBorder.Child
            End Get
            Set(value As UIElement)
                Me.MainBorder.Child = value
            End Set
        End Property

        Public Property MainTextBlock As TextBlock
            Get
                Me.Init()
                Return Me._MainTextBlock
            End Get
            Set(value As TextBlock)
                Me._MainTextBlock = value
            End Set
        End Property

        Public Property MainSwap As Path
            Get
                Me.Init()
                Return Me._MainSwap
            End Get
            Set(value As Path)
                Me._MainSwap = value
            End Set
        End Property

        Public ReadOnly Property Inlines As InlineCollection
            Get
                Return Me.MainTextBlock.Inlines
            End Get
        End Property

        Public Property CornerRadius As CornerRadius
            Get
                Return Me.MainChrome.CornerRadius
            End Get
            Set(value As CornerRadius)
                Me.MainChrome.CornerRadius = value
                Me.MainBorder.CornerRadius = value
            End Set
        End Property

        Public Property Title As String
            Get
                Return Conversions.ToString(MyBase.GetValue(OurCard.TitleProperty))
            End Get
            Set(value As String)
                MyBase.SetValue(OurCard.TitleProperty, value)
                If Me._MainTextBlock IsNot Nothing Then
                    Me.MainTextBlock.Text = value
                End If
            End Set
        End Property

        Public Sub New()
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.Init()
            End Sub
            AddHandler MyBase.MouseEnter, AddressOf Me.OurCard_MouseEnter
            AddHandler MyBase.MouseLeave, AddressOf Me.OurCard_MouseLeave
            AddHandler MyBase.SizeChanged, AddressOf Me.OurSizeChanged
            AddHandler MyBase.MouseLeftButtonDown, AddressOf Me.OurCard_MouseLeftButtonDown
            AddHandler MyBase.MouseLeftButtonUp, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.OurCard_MouseLeftButtonUp()
            End Sub
            AddHandler MyBase.MouseLeave, AddressOf Me.OurCard_MouseLeave_Swap
            AddHandler MyBase.MouseMove, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.UpdateCursorStyle()
            End Sub
            Me.Uuid = ModBase.GetUuid()
            Me.IsLoad = False
            Me.HasMouseAnimation = True
            Me.UseAnimation = True
            Me.IsHeightAnimating = False
            Me.CanSwap = False
            Me._IsSwapped = False
            Me.SwapLogoRight = False
            Me.IsSwapMouseDown = False
            Me.IsCustomMouseDown = False
            Me.MainChrome = New OurDropShadow() With { .Margin = New Thickness(-3.0, -3.0, -3.0, -3.0 - ModBase.GetWPFSize(1.0)), .ShadowRadius = 3.0, .Opacity = 0.07, .CornerRadius = New CornerRadius(5.0) }
            Me.MainChrome.SetResourceReference(OurDropShadow.ColorProperty, "ColorObject1")
            MyBase.Children.Insert(0, Me.MainChrome)
            Me.MainBorder = New Border() With { .Background = New SolidColorBrush(Color.FromArgb(245, Byte.MaxValue, Byte.MaxValue, Byte.MaxValue)), .CornerRadius = New CornerRadius(5.0), .IsHitTestVisible = False }
            MyBase.Children.Insert(1, Me.MainBorder)
            Me.MainGrid = New Grid()
            MyBase.Children.Add(Me.MainGrid)
        End Sub

        Private Sub Init()
            If Not Me.IsLoad Then
                Me.IsLoad = True
                If Me.MainTextBlock Is Nothing Then
                    Me.MainTextBlock = New TextBlock() With { .HorizontalAlignment = HorizontalAlignment.Left, .VerticalAlignment = VerticalAlignment.Top, .Margin = New Thickness(15.0, 12.0, 0.0, 0.0), .FontWeight = FontWeights.Bold, .FontSize = 13.0, .IsHitTestVisible = False }
                    Me.MainTextBlock.SetResourceReference(TextBlock.ForegroundProperty, "ColorBrush1")
                    Me.MainTextBlock.SetBinding(TextBlock.TextProperty, New Binding("Title") With { .Source = Me, .Mode = BindingMode.OneWay })
                    Me.MainGrid.Children.Add(Me.MainTextBlock)
                End If
                If Me.CanSwap OrElse Me.SwapControl IsNot Nothing Then
                    If Me.SwapControl Is Nothing AndAlso MyBase.Children.Count > 3 Then
                        Me.SwapControl = CType(MyBase.Children(3), FrameworkElement)
                    End If
                    Me.MainSwap = New Path() With { .HorizontalAlignment = HorizontalAlignment.Right, .Stretch = Stretch.Uniform, .Height = 6.0, .Width = 10.0, .VerticalAlignment = VerticalAlignment.Top, .Margin = New Thickness(0.0, 17.0, 16.0, 0.0), .Data = CType(New GeometryConverter().ConvertFromString("M2,4 l-2,2 10,10 10,-10 -2,-2 -8,8 -8,-8 z"), Geometry), .RenderTransform = New RotateTransform(180.0), .RenderTransformOrigin = New Point(0.5, 0.5) }
                    Me.MainSwap.SetResourceReference(Shape.FillProperty, "ColorBrush1")
                    Me.MainGrid.Children.Add(Me.MainSwap)
                End If
                If Me.IsSwapped AndAlso Me.SwapControl IsNot Nothing Then
                    Me.MainSwap.RenderTransform = New RotateTransform(CDbl(If(Me.SwapLogoRight, 270, 0)))
                    Me.SwapControl.Visibility = Visibility.Collapsed
                    Dim RawUseAnimation As Boolean = Me.UseAnimation
                    Me.UseAnimation = False
                    MyBase.Height = 40.0
                    ModAnimation.AniStop("OurCard Height " + Conversions.ToString(Me.Uuid))
                    Me.IsHeightAnimating = False
                    ModBase.RunInUi(Sub()
                        Me.UseAnimation = RawUseAnimation
                    End Sub, True)
                End If
            End If
        End Sub

        Public Sub SetContentBuilder(Builder As Action(Of Panel))
            Me.ContentBuilder = Builder
        End Sub

        Public Sub SetContentBuilder(Of T)(Items As IEnumerable(Of T), ItemBuilder As Func(Of T, UIElement))
            Dim CS$<>8__locals1 As OurCard._Closure$__32-0(Of T) = New OurCard._Closure$__32-0(Of T)(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_Items = Items
            CS$<>8__locals1.$VB$Local_ItemBuilder = ItemBuilder
            Me.SetContentBuilder(Sub(Stack As Panel)
                Try
                    For Each $CLS As $CLS0 In CS$<>8__locals1.$VB$Local_Items
                        Stack.Children.Add(CS$<>8__locals1.$VB$Local_ItemBuilder($CLS))
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of $CLS0)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                Stack.Children.Add(New FrameworkElement() With { .Height = 18.0 })
            End Sub)
        End Sub

        Public Sub BuildContent()
            If Me.ContentBuilder IsNot Nothing Then
                Dim panel As Panel = TryCast(Me.SwapControl, Panel)
                If panel IsNot Nothing Then
                    Try
                        Me.ContentBuilder(panel)
                        Me.ContentBuilder = Nothing
                    Catch ex As Exception
                        panel.Children.Clear()
                        Throw
                    End Try
                    Me.TriggerForceResize()
                End If
            End If
        End Sub

        Public Property HasMouseAnimation As Boolean

        Private Sub OurCard_MouseEnter(sender As Object, e As MouseEventArgs)
            If Me.HasMouseAnimation Then
                Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
                If Not Information.IsNothing(Me.MainTextBlock) Then
                    list.Add(ModAnimation.AaColor(Me.MainTextBlock, TextBlock.ForegroundProperty, "ColorBrush2", 90, 0, Nothing, False))
                End If
                If Not Information.IsNothing(Me.MainSwap) Then
                    list.Add(ModAnimation.AaColor(Me.MainSwap, Shape.FillProperty, "ColorBrush2", 90, 0, Nothing, False))
                End If
                list.AddRange(New ModAnimation.AniData() { ModAnimation.AaColor(Me.MainChrome, OurDropShadow.ColorProperty, "ColorObject4", 90, 0, Nothing, False), ModAnimation.AaOpacity(Me.MainChrome, 0.4 - Me.MainChrome.Opacity, 90, 0, Nothing, False) })
                ModAnimation.AniStart(list, "OurCard Mouse " + Conversions.ToString(Me.Uuid), False)
            End If
        End Sub

        Private Sub OurCard_MouseLeave(sender As Object, e As MouseEventArgs)
            If Me.HasMouseAnimation Then
                Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
                If Not Information.IsNothing(Me.MainTextBlock) Then
                    list.Add(ModAnimation.AaColor(Me.MainTextBlock, TextBlock.ForegroundProperty, "ColorBrush1", 90, 0, Nothing, False))
                End If
                If Not Information.IsNothing(Me.MainSwap) Then
                    list.Add(ModAnimation.AaColor(Me.MainSwap, Shape.FillProperty, "ColorBrush1", 90, 0, Nothing, False))
                End If
                list.AddRange(New ModAnimation.AniData() { ModAnimation.AaColor(Me.MainChrome, OurDropShadow.ColorProperty, "ColorObject1", 90, 0, Nothing, False), ModAnimation.AaOpacity(Me.MainChrome, 0.07 - Me.MainChrome.Opacity, 90, 0, Nothing, False) })
                ModAnimation.AniStart(list, "OurCard Mouse " + Conversions.ToString(Me.Uuid), False)
            End If
        End Sub

        Public Property UseAnimation As Boolean

        Private Sub OurSizeChanged(sender As Object, e As SizeChangedEventArgs)
            If Me.UseAnimation Then
                Dim num As Double = If(Me.IsSwapped, 40.0, e.NewSize.Height) - e.PreviousSize.Height
                If e.PreviousSize.Height <> 0.0 AndAlso Not Me.IsHeightAnimating AndAlso Math.Abs(num) >= 1.0 AndAlso MyBase.ActualHeight <> 0.0 Then
                    Me.StartHeightAnimation(num, e.PreviousSize.Height, False)
                End If
            End If
        End Sub

        Private Sub StartHeightAnimation(Delta As Double, PreviousHeight As Double, IsLoadAnimation As Boolean)
            ' The following expression was wrapped in a checked-statement
            If Not Me.IsHeightAnimating AndAlso ModMain.FrmMain IsNot Nothing Then
                Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
                Dim num As Double = Math.Abs(Delta)
                If num <= 800.0 Then
                    list.Add(ModAnimation.AaHeight(Me, Delta, 150, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.ExtraStrong), False))
                Else
                    Dim num2 As Integer
                    Dim num3 As Integer
                    Dim num4 As Integer
                    If Delta < 0.0 AndAlso num - CDbl(num2) > 500.0 Then
                        num2 = 200
                        num3 = 150
                        num4 = CInt(Math.Round(num - CDbl(num2) / 0.1))
                    ElseIf Delta > 0.0 AndAlso num - CDbl(num2) > 3000.0 Then
                        num4 = 5000
                        num2 = CInt(Math.Round(num - CDbl(num4) * 0.3))
                        num3 = 400
                    Else
                        num2 = 150
                        num3 = 200
                        num4 = 4000
                    End If
                    list.Add(ModAnimation.AaHeight(Me, (num - CDbl(num2)) * CDbl(Math.Sign(Delta)), CInt(Math.Round((num - CDbl(num2)) / CDbl(num4) * 1000.0)), 0, Nothing, False))
                    list.Add(ModAnimation.AaHeight(Me, CDbl((num2 * Math.Sign(Delta))), num3, 0, New ModAnimation.AniEaseOutFluentWithInitial(CDbl(num4), CDbl(num3) / 1000.0, CDbl(num2)), True))
                End If
                list.Add(ModAnimation.AaCode(Sub()
                    Me.IsHeightAnimating = False
                    MyBase.Height = Me.ActualUsedHeight
                    If Me.IsSwapped AndAlso Me.SwapControl IsNot Nothing Then
                        Me.SwapControl.Visibility = Visibility.Collapsed
                    End If
                End Sub, 0, True))
                ModAnimation.AniStart(list, "OurCard Height " + Conversions.ToString(Me.Uuid), False)
                Me.IsHeightAnimating = True
                Me.ActualUsedHeight = If(Me.IsSwapped, 40.0, MyBase.Height)
                MyBase.Height = PreviousHeight
            End If
        End Sub

        Public Sub TriggerForceResize()
            MyBase.Height = If(Me.IsSwapped, 40.0, Double.NaN)
            ModAnimation.AniStop("OurCard Height " + Conversions.ToString(Me.Uuid))
            Me.IsHeightAnimating = False
        End Sub

        Public Property CanSwap As Boolean

        Public Property IsSwapped As Boolean
            Get
                Return Me._IsSwapped
            End Get
            Set(value As Boolean)
                If Me._IsSwapped <> value Then
                    Me._IsSwapped = value
                    If Me.SwapControl IsNot Nothing Then
                        If Not Me.IsSwapped Then
                            Me.BuildContent()
                        End If
                        If MyBase.IsLoaded Then
                            Me.SwapControl.Visibility = Visibility.Visible
                            Me.TriggerForceResize()
                            Me.UpdateCursorStyle()
                            ModAnimation.AniStart(ModAnimation.AaRotateTransform(Me.MainSwap, CDbl(If(Me._IsSwapped, If(Me.SwapLogoRight, 270, 0), 180)) - CType(Me.MainSwap.RenderTransform, RotateTransform).Angle, 250, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.ExtraStrong), False), "OurCard Swap " + Conversions.ToString(Me.Uuid), True)
                        End If
                    End If
                End If
            End Set
        End Property

        <Obsolete("IsSwaped 存在拼写错误，请换用 IsSwapped 属性。")>
        Public Property IsSwaped As Boolean
            Get
                Return Conversions.ToBoolean(MyBase.GetValue(OurCard.IsSwapedProperty))
            End Get
            Set(value As Boolean)
                MyBase.SetValue(OurCard.IsSwapedProperty, value)
            End Set
        End Property

        Public Property SwapLogoRight As Boolean

        Public Event PreviewSwap As OurCard.PreviewSwapEventHandler

        Public Event Swap As OurCard.SwapEventHandler

        Private Sub OurCard_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
            Dim y As Double = Mouse.GetPosition(Me).Y
            If Me.IsSwapped OrElse (y <= CDbl(If(Me.IsSwapped, 40, 34)) AndAlso (y <> 0.0 OrElse MyBase.IsMouseDirectlyOver)) Then
                Me.IsCustomMouseDown = True
                If Me.IsSwapped OrElse (Me.SwapControl IsNot Nothing AndAlso y <= CDbl(If(Me.IsSwapped, 40, 34)) AndAlso (y <> 0.0 OrElse MyBase.IsMouseDirectlyOver)) Then
                    Me.IsSwapMouseDown = True
                End If
            End If
        End Sub

        Private Sub OurCard_MouseLeftButtonUp()
            If Me.IsCustomMouseDown Then
                Me.IsCustomMouseDown = False
                Me.RaiseCustomEvent()
                If Me.IsSwapMouseDown Then
                    Me.IsSwapMouseDown = False
                    Dim y As Double = Mouse.GetPosition(Me).Y
                    If Me.IsSwapped OrElse (Me.SwapControl IsNot Nothing AndAlso y <= CDbl(If(Me.IsSwapped, 40, 34)) AndAlso (y <> 0.0 OrElse MyBase.IsMouseDirectlyOver)) Then
                        Dim e As ModBase.RouteEventArgs = New ModBase.RouteEventArgs(True)
                        Dim previewSwapEvent As OurCard.PreviewSwapEventHandler = Me.PreviewSwapEvent
                        If previewSwapEvent IsNot Nothing Then
                            previewSwapEvent(Me, e)
                        End If
                        If e.Handled Then
                            Me.IsSwapMouseDown = False
                            Return
                        End If
                        Me.IsSwapped = Not Me.IsSwapped
                        Logger.Info(String.Format("{0}{1}", If(Me.IsSwapped, "折叠卡片", "展开卡片"), If((Me.Title Is Nothing), "", ("：" + Me.Title))), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurCard.vb")
                        Dim swapEvent As OurCard.SwapEventHandler = Me.SwapEvent
                        If swapEvent IsNot Nothing Then
                            swapEvent(Me, e)
                        End If
                    End If
                End If
            End If
        End Sub

        Private Sub OurCard_MouseLeave_Swap(sender As Object, e As MouseEventArgs)
            Me.IsSwapMouseDown = False
        End Sub

        Private Sub UpdateCursorStyle()
            If Me._MainSwap Is Nothing Then
                MyBase.Cursor = Nothing
                Return
            End If
            Dim y As Double = Mouse.GetPosition(Me).Y
            MyBase.Cursor = If((Me.MainSwap.Visibility = Visibility.Visible AndAlso (Me.IsSwapped OrElse (y <= 34.0 AndAlso (y <> 0.0 OrElse MyBase.IsMouseDirectlyOver)))), Cursors.Hand, Nothing)
        End Sub

        Private MainGrid As Grid

        Private MainBorder As Border

        Private _MainTextBlock As TextBlock

        Private _MainSwap As Path

        Public Uuid As Integer

        Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register("Title", GetType(String), GetType(OurCard), New PropertyMetadata(""))

        Private IsLoad As Boolean

        Private ContentBuilder As Action(Of Panel)

        Private Const DropShadowIdleOpacity As Double = 0.07

        Private Const DropShadowHoverOpacity As Double = 0.4

        Private IsHeightAnimating As Boolean

        Private ActualUsedHeight As Double

        Public SwapControl As FrameworkElement

        Private _IsSwapped As Boolean

        Public Shared IsSwapedProperty As DependencyProperty = DependencyProperty.Register("IsSwaped", GetType(Boolean), GetType(OurCard), New PropertyMetadata(False, Sub(sender As DependencyObject, e As DependencyPropertyChangedEventArgs)
            If sender IsNot Nothing AndAlso TypeOf sender Is OurCard Then
                CType(sender, OurCard).IsSwapped = Conversions.ToBoolean(e.NewValue)
            End If
        End Sub))

        Private IsSwapMouseDown As Boolean

        Private IsCustomMouseDown As Boolean

        Public Const SwapedHeight As Integer = 40

        Public Delegate Sub PreviewSwapEventHandler(sender As Object, e As ModBase.RouteEventArgs)

        Public Delegate Sub SwapEventHandler(sender As Object, e As ModBase.RouteEventArgs)
    End Class
End Namespace
