Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports MeloongCore
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class OurExtraButton
        Inherits Grid

        Public Sub New()
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.IsEnabledChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.RefreshColor()
            End Sub
            Me._Progress = 0.0
            Me.Uuid = ModBase.GetUuid()
            Me._Logo = ""
            Me._LogoScale = 1.0
            Me._Show = False
            Me.ShowCheck = Nothing
            Me._CanRightClick = False
            Me.IsLeftMouseHeld = False
            Me.IsRightMouseHeld = False
            Me.InitializeComponent()
        End Sub

        Public Event Click As OurExtraButton.ClickEventHandler

        Public Event RightClick As OurExtraButton.RightClickEventHandler

        Public Property Progress As Double
            Get
                Return Me._Progress
            End Get
            Set(value As Double)
                If Me._Progress <> value Then
                    Me._Progress = value
                    If value < 0.0001 Then
                        Me.PanProgress.Visibility = Visibility.Collapsed
                        Return
                    End If
                    Me.PanProgress.Visibility = Visibility.Visible
                    Me.RectProgress.Rect = New Rect(0.0, 40.0 * (1.0 - value), 40.0, 40.0 * value)
                End If
            End Set
        End Property

        Public Property Logo As String
            Get
                Return Me._Logo
            End Get
            Set(value As String)
                If Operators.CompareString(value, Me._Logo, False) <> 0 Then
                    Me._Logo = value
                    Me.Path.Data = CType(New GeometryConverter().ConvertFromString(value), Geometry)
                End If
            End Set
        End Property

        Public Property LogoScale As Double
            Get
                Return Me._LogoScale
            End Get
            Set(value As Double)
                Me._LogoScale = value
                If Not Information.IsNothing(Me.Path) Then
                    Me.Path.RenderTransform = New ScaleTransform() With { .ScaleX = Me.LogoScale, .ScaleY = Me.LogoScale }
                End If
            End Set
        End Property

        Public Property Show As Boolean
            Get
                Return Me._Show
            End Get
            Set(value As Boolean)
                If Me._Show <> value Then
                    Me._Show = value
                    ModBase.RunInUi(Sub()
                        If value Then
                            Me.Visibility = Visibility.Visible
                            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me, 0.3 - CType(Me.RenderTransform, ScaleTransform).ScaleX, 500, 60, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaScaleTransform(Me, 0.7, 500, 60, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaHeight(Me, 50.0 - Me.Height, 200, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False) }, "OurExtraButton MainScale " + Conversions.ToString(Me.Uuid), False)
                        Else
                            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me, -CType(Me.RenderTransform, ScaleTransform).ScaleX, 100, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaHeight(Me, -Me.Height, 400, 100, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaCode(Sub()
                                MyBase.Visibility = Visibility.Collapsed
                            End Sub, 0, True) }, "OurExtraButton MainScale " + Conversions.ToString(Me.Uuid), False)
                        End If
                        Me.IsHitTestVisible = value
                    End Sub, False)
                End If
            End Set
        End Property

        Public Sub ShowRefresh()
            If Me.ShowCheck IsNot Nothing Then
                Me.Show = Me.ShowCheck()
            End If
        End Sub

        Private Sub Button_LeftMouseUp(sender As Object, e As MouseButtonEventArgs)
            If Me.IsLeftMouseHeld Then
                Logger.Info(String.Format("按下附加按钮{0}", RuntimeHelpers.GetObjectValue(If(Operators.ConditionalCompareObjectEqual(MyBase.ToolTip, "", False), "", Operators.ConcatenateObject("：", MyBase.ToolTip)))), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurExtraButton.xaml.vb")
                Dim clickEvent As OurExtraButton.ClickEventHandler = Me.ClickEvent
                If clickEvent IsNot Nothing Then
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e)
                End If
                e.Handled = True
                Me.Button_LeftMouseUp()
            End If
        End Sub

        Private Sub Button_RightMouseUp(sender As Object, e As MouseButtonEventArgs)
            If Me.IsRightMouseHeld Then
                Logger.Info(String.Format("右键按下附加按钮{0}", RuntimeHelpers.GetObjectValue(If(Operators.ConditionalCompareObjectEqual(MyBase.ToolTip, "", False), "", Operators.ConcatenateObject("：", MyBase.ToolTip)))), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurExtraButton.xaml.vb")
                Dim rightClickEvent As OurExtraButton.RightClickEventHandler = Me.RightClickEvent
                If rightClickEvent IsNot Nothing Then
                    rightClickEvent(RuntimeHelpers.GetObjectValue(sender), e)
                End If
                e.Handled = True
                Me.Button_RightMouseUp()
            End If
        End Sub

        Public Property CanRightClick As Boolean
            Get
                Return Me._CanRightClick
            End Get
            Set(value As Boolean)
                Me._CanRightClick = value
            End Set
        End Property

        Private Sub Button_LeftMouseDown(sender As Object, e As MouseButtonEventArgs)
            If Not Me.IsLeftMouseHeld AndAlso Not Me.IsRightMouseHeld Then
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me.PanScale, 0.85 - CType(Me.PanScale.RenderTransform, ScaleTransform).ScaleX, 800, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False), ModAnimation.AaScaleTransform(Me.PanScale, -0.05, 60, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) }, "OurExtraButton Scale " + Conversions.ToString(Me.Uuid), False)
            End If
            Me.IsLeftMouseHeld = True
            MyBase.Focus()
        End Sub

        Private Sub Button_RightMouseDown(sender As Object, e As MouseButtonEventArgs)
            If Me.CanRightClick Then
                If Not Me.IsLeftMouseHeld AndAlso Not Me.IsRightMouseHeld Then
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me.PanScale, 0.85 - CType(Me.PanScale.RenderTransform, ScaleTransform).ScaleX, 800, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False), ModAnimation.AaScaleTransform(Me.PanScale, -0.05, 60, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) }, "OurExtraButton Scale " + Conversions.ToString(Me.Uuid), False)
                End If
                Me.IsRightMouseHeld = True
                MyBase.Focus()
            End If
        End Sub

        Private Sub Button_LeftMouseUp()
            If Not Me.IsRightMouseHeld Then
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me.PanScale, 1.0 - CType(Me.PanScale.RenderTransform, ScaleTransform).ScaleX, 300, 0, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), False) }, "OurExtraButton Scale " + Conversions.ToString(Me.Uuid), False)
            End If
            If Me.IsLeftMouseHeld Then
                Me.RaiseCustomEvent()
            End If
            Me.IsLeftMouseHeld = False
            Me.RefreshColor()
        End Sub

        Private Sub Button_RightMouseUp()
            If Me.CanRightClick Then
                If Not Me.IsLeftMouseHeld Then
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me.PanScale, 1.0 - CType(Me.PanScale.RenderTransform, ScaleTransform).ScaleX, 300, 0, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), False) }, "OurExtraButton Scale " + Conversions.ToString(Me.Uuid), False)
                End If
                Me.IsRightMouseHeld = False
                Me.RefreshColor()
            End If
        End Sub

        Private Sub Button_MouseLeave()
            Me.IsLeftMouseHeld = False
            Me.IsRightMouseHeld = False
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me.PanScale, 1.0 - CType(Me.PanScale.RenderTransform, ScaleTransform).ScaleX, 500, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) }, "OurExtraButton Scale " + Conversions.ToString(Me.Uuid), False)
            Me.RefreshColor()
        End Sub

        Public Sub RefreshColor()
            Try
                If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                    If Not MyBase.IsEnabled Then
                        ModAnimation.AniStart(ModAnimation.AaColor(Me.PanColor, Panel.BackgroundProperty, "ColorBrushGray4", 120, 0, Nothing, False), "OurExtraButton Color " + Conversions.ToString(Me.Uuid), False)
                    ElseIf MyBase.IsMouseOver Then
                        ModAnimation.AniStart(ModAnimation.AaColor(Me.PanColor, Panel.BackgroundProperty, "ColorBrush4", 120, 0, Nothing, False), "OurExtraButton Color " + Conversions.ToString(Me.Uuid), False)
                    Else
                        ModAnimation.AniStart(ModAnimation.AaColor(Me.PanColor, Panel.BackgroundProperty, "ColorBrush3", 150, 0, Nothing, False), "OurExtraButton Color " + Conversions.ToString(Me.Uuid), False)
                    End If
                Else
                    ModAnimation.AniStop("OurExtraButton Color " + Conversions.ToString(Me.Uuid))
                    If Not MyBase.IsEnabled Then
                        Me.PanColor.SetResourceReference(Panel.BackgroundProperty, "ColorBrushGray4")
                    ElseIf MyBase.IsMouseOver Then
                        Me.PanColor.SetResourceReference(Panel.BackgroundProperty, "ColorBrush4")
                    Else
                        Me.PanColor.SetResourceReference(Panel.BackgroundProperty, "ColorBrush3")
                    End If
                End If
            Catch ex As Exception
                Logger.Warn(ex, "刷新图标按钮颜色出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurExtraButton.xaml.vb")
            End Try
        End Sub

        Public Sub Ribble()
            ModBase.RunInUi(Sub()
                Dim Shape As Border = New Border() With { .CornerRadius = New CornerRadius(1000.0), .BorderThickness = New Thickness(0.001), .Opacity = 0.5, .RenderTransformOrigin = New Point(0.5, 0.5), .RenderTransform = New ScaleTransform() }
                Shape.SetResourceReference(Border.BackgroundProperty, "ColorBrush5")
                Me.PanScale.Children.Insert(0, Shape)
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Shape, 13.0, 1000, 0, New ModAnimation.AniEaseInoutFluent(ModAnimation.AniEasePower.Strong, 0.3), False), ModAnimation.AaOpacity(Shape, -Shape.Opacity, 1000, 0, Nothing, False), ModAnimation.AaCode(Sub()
                    Me.PanScale.Children.Remove(Shape)
                End Sub, 0, True) }, "ExtraButton Ribble " + Conversions.ToString(ModBase.GetUuid()), False)
            End Sub, False)
        End Sub

        Friend Overridable Property PanBack As OurExtraButton

        Friend Overridable Property PanClick As Border
            <CompilerGenerated()>
            Get
                Return Me._PanClick
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Border)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = AddressOf Me.Button_LeftMouseUp
                Dim mouseButtonEventHandler2 As MouseButtonEventHandler = AddressOf Me.Button_RightMouseUp
                Dim mouseButtonEventHandler3 As MouseButtonEventHandler = AddressOf Me.Button_LeftMouseDown
                Dim mouseButtonEventHandler4 As MouseButtonEventHandler = AddressOf Me.Button_RightMouseDown
                Dim mouseButtonEventHandler5 As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.Button_LeftMouseUp()
                End Sub
                Dim mouseButtonEventHandler6 As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.Button_RightMouseUp()
                End Sub
                Dim mouseEventHandler As MouseEventHandler = Sub(a0 As Object, a1 As MouseEventArgs)
                    Me.Button_MouseLeave()
                End Sub
                Dim mouseEventHandler2 As MouseEventHandler = Sub(a0 As Object, a1 As MouseEventArgs)
                    Me.RefreshColor()
                End Sub
                Dim mouseEventHandler3 As MouseEventHandler = Sub(a0 As Object, a1 As MouseEventArgs)
                    Me.RefreshColor()
                End Sub
                Dim border As Border = Me._PanClick
                If border IsNot Nothing Then
                    RemoveHandler border.MouseLeftButtonUp, mouseButtonEventHandler
                    RemoveHandler border.MouseRightButtonUp, mouseButtonEventHandler2
                    RemoveHandler border.MouseLeftButtonDown, mouseButtonEventHandler3
                    RemoveHandler border.MouseRightButtonDown, mouseButtonEventHandler4
                    RemoveHandler border.MouseLeftButtonUp, mouseButtonEventHandler5
                    RemoveHandler border.MouseRightButtonUp, mouseButtonEventHandler6
                    RemoveHandler border.MouseLeave, mouseEventHandler
                    RemoveHandler border.MouseEnter, mouseEventHandler2
                    RemoveHandler border.MouseLeave, mouseEventHandler3
                End If
                Me._PanClick = value
                border = Me._PanClick
                If border IsNot Nothing Then
                    AddHandler border.MouseLeftButtonUp, mouseButtonEventHandler
                    AddHandler border.MouseRightButtonUp, mouseButtonEventHandler2
                    AddHandler border.MouseLeftButtonDown, mouseButtonEventHandler3
                    AddHandler border.MouseRightButtonDown, mouseButtonEventHandler4
                    AddHandler border.MouseLeftButtonUp, mouseButtonEventHandler5
                    AddHandler border.MouseRightButtonUp, mouseButtonEventHandler6
                    AddHandler border.MouseLeave, mouseEventHandler
                    AddHandler border.MouseEnter, mouseEventHandler2
                    AddHandler border.MouseLeave, mouseEventHandler3
                End If
            End Set
        End Property

        Friend Overridable Property PanScale As Grid

        Friend Overridable Property PanColor As Border

        Friend Overridable Property PanProgress As Border

        Friend Overridable Property RectProgress As RectangleGeometry

        Friend Overridable Property Path As Path

        Private _Progress As Double

        Public Uuid As Integer

        Private _Logo As String

        Private _LogoScale As Double

        Private _Show As Boolean

        Public ShowCheck As OurExtraButton.ShowCheckDelegate

        Private _CanRightClick As Boolean

        Private IsLeftMouseHeld As Boolean

        Private IsRightMouseHeld As Boolean

        Private Const AnimationColorIn As Integer = 120

        Private Const AnimationColorOut As Integer = 150

        Public Delegate Sub ClickEventHandler(sender As Object, e As MouseButtonEventArgs)

        Public Delegate Sub RightClickEventHandler(sender As Object, e As MouseButtonEventArgs)

        Public Delegate Function ShowCheckDelegate() As Boolean
    End Class
End Namespace
