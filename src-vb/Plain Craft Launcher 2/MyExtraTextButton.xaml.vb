Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports MeloongCore
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <ContentProperty("Inlines")>
    <DesignerGenerated()>
    Public Partial Class MyExtraTextButton
        Inherits Grid

        Public Sub New()
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.IsEnabledChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.RefreshColor()
            End Sub
            Me.Uuid = ModBase.GetUuid()
            Me._Logo = ""
            Me._LogoScale = 1.0
            Me._Show = False
            Me.IsLeftMouseHeld = False
            Me.InitializeComponent()
        End Sub

        Public Event Click As MyExtraTextButton.ClickEventHandler

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
                If Me.Path IsNot Nothing Then
                    Me.Path.RenderTransform = New ScaleTransform() With { .ScaleX = Me.LogoScale, .ScaleY = Me.LogoScale }
                End If
            End Set
        End Property

        Public ReadOnly Property Inlines As InlineCollection
            Get
                Return Me.LabText.Inlines
            End Get
        End Property

        Public Property Text As String
            Get
                Return Conversions.ToString(MyBase.GetValue(MyExtraTextButton.TextProperty))
            End Get
            Set(value As String)
                MyBase.SetValue(MyExtraTextButton.TextProperty, value)
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
                            Me.Opacity = 0.0
                            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me, 1.0 - Me.Opacity, 80, 50, Nothing, False), ModAnimation.AaScaleTransform(Me, 0.15 - CType(Me.RenderTransform, ScaleTransform).ScaleX, 400, 50, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaScaleTransform(Me, 0.85, 160, 50, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) }, "MyExtraTextButton MainScale " + Conversions.ToString(Me.Uuid), False)
                        Else
                            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me, -Me.Opacity, 50, 50, Nothing, False), ModAnimation.AaScaleTransform(Me, -CType(Me.RenderTransform, ScaleTransform).ScaleX, 100, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), False) }, "MyExtraTextButton MainScale " + Conversions.ToString(Me.Uuid), False)
                        End If
                        Me.IsHitTestVisible = value
                    End Sub, False)
                End If
            End Set
        End Property

        Private Sub Button_LeftMouseUp(sender As Object, e As MouseButtonEventArgs)
            If Me.IsLeftMouseHeld Then
                Logger.Info(String.Format("按下附加图标按钮：{0}", Me.Text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyExtraTextButton.xaml.vb")
                Dim clickEvent As MyExtraTextButton.ClickEventHandler = Me.ClickEvent
                If clickEvent IsNot Nothing Then
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e)
                End If
                e.Handled = True
                Me.RaiseCustomEvent()
                Me.Button_LeftMouseUp()
            End If
        End Sub

        Private Sub Button_LeftMouseDown(sender As Object, e As MouseButtonEventArgs)
            If Not Me.IsLeftMouseHeld Then
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me.PanScale, 0.85 - CType(Me.PanScale.RenderTransform, ScaleTransform).ScaleX, 800, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False), ModAnimation.AaScaleTransform(Me.PanScale, -0.05, 60, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) }, "MyExtraTextButton Scale " + Conversions.ToString(Me.Uuid), False)
            End If
            Me.IsLeftMouseHeld = True
            MyBase.Focus()
        End Sub

        Private Sub Button_LeftMouseUp()
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me.PanScale, 1.0 - CType(Me.PanScale.RenderTransform, ScaleTransform).ScaleX, 300, 0, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), False) }, "MyExtraTextButton Scale " + Conversions.ToString(Me.Uuid), False)
            Me.IsLeftMouseHeld = False
            Me.RefreshColor()
        End Sub

        Private Sub Button_RightMouseUp()
            If Not Me.IsLeftMouseHeld Then
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me.PanScale, 1.0 - CType(Me.PanScale.RenderTransform, ScaleTransform).ScaleX, 300, 0, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), False) }, "MyExtraTextButton Scale " + Conversions.ToString(Me.Uuid), False)
            End If
            Me.RefreshColor()
        End Sub

        Private Sub Button_MouseLeave()
            Me.IsLeftMouseHeld = False
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me.PanScale, 1.0 - CType(Me.PanScale.RenderTransform, ScaleTransform).ScaleX, 500, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) }, "MyExtraTextButton Scale " + Conversions.ToString(Me.Uuid), False)
            Me.RefreshColor()
        End Sub

        Public Sub RefreshColor()
            Try
                If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                    If Not MyBase.IsEnabled Then
                        ModAnimation.AniStart(ModAnimation.AaColor(Me.PanColor, Panel.BackgroundProperty, "ColorBrushGray4", 120, 0, Nothing, False), "MyExtraTextButton Color " + Conversions.ToString(Me.Uuid), False)
                    ElseIf MyBase.IsMouseOver Then
                        ModAnimation.AniStart(ModAnimation.AaColor(Me.PanColor, Panel.BackgroundProperty, "ColorBrush4", 120, 0, Nothing, False), "MyExtraTextButton Color " + Conversions.ToString(Me.Uuid), False)
                    Else
                        ModAnimation.AniStart(ModAnimation.AaColor(Me.PanColor, Panel.BackgroundProperty, "ColorBrush3", 150, 0, Nothing, False), "MyExtraTextButton Color " + Conversions.ToString(Me.Uuid), False)
                    End If
                Else
                    ModAnimation.AniStop("MyExtraTextButton Color " + Conversions.ToString(Me.Uuid))
                    If Not MyBase.IsEnabled Then
                        Me.PanColor.SetResourceReference(Panel.BackgroundProperty, "ColorBrushGray4")
                    ElseIf MyBase.IsMouseOver Then
                        Me.PanColor.SetResourceReference(Panel.BackgroundProperty, "ColorBrush4")
                    Else
                        Me.PanColor.SetResourceReference(Panel.BackgroundProperty, "ColorBrush3")
                    End If
                End If
            Catch ex As Exception
                Logger.Warn(ex, "刷新附加图标按钮颜色出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyExtraTextButton.xaml.vb")
            End Try
        End Sub

        Friend Overridable Property PanBack As MyExtraTextButton

        Friend Overridable Property PanClick As Border
            <CompilerGenerated()>
            Get
                Return Me._PanClick
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Border)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = AddressOf Me.Button_LeftMouseUp
                Dim mouseButtonEventHandler2 As MouseButtonEventHandler = AddressOf Me.Button_LeftMouseDown
                Dim mouseButtonEventHandler3 As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.Button_LeftMouseUp()
                End Sub
                Dim mouseButtonEventHandler4 As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
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
                    RemoveHandler border.MouseLeftButtonDown, mouseButtonEventHandler2
                    RemoveHandler border.MouseLeftButtonUp, mouseButtonEventHandler3
                    RemoveHandler border.MouseRightButtonUp, mouseButtonEventHandler4
                    RemoveHandler border.MouseLeave, mouseEventHandler
                    RemoveHandler border.MouseEnter, mouseEventHandler2
                    RemoveHandler border.MouseLeave, mouseEventHandler3
                End If
                Me._PanClick = value
                border = Me._PanClick
                If border IsNot Nothing Then
                    AddHandler border.MouseLeftButtonUp, mouseButtonEventHandler
                    AddHandler border.MouseLeftButtonDown, mouseButtonEventHandler2
                    AddHandler border.MouseLeftButtonUp, mouseButtonEventHandler3
                    AddHandler border.MouseRightButtonUp, mouseButtonEventHandler4
                    AddHandler border.MouseLeave, mouseEventHandler
                    AddHandler border.MouseEnter, mouseEventHandler2
                    AddHandler border.MouseLeave, mouseEventHandler3
                End If
            End Set
        End Property

        Friend Overridable Property PanScale As Grid

        Friend Overridable Property PanColor As Border

        Friend Overridable Property Path As Path

        Friend Overridable Property LabText As TextBlock

        Public Uuid As Integer

        Private _Logo As String

        Private _LogoScale As Double

        Public Shared TextProperty As DependencyProperty = DependencyProperty.Register("Text", GetType(String), GetType(MyExtraTextButton), New PropertyMetadata(Sub(sender As DependencyObject, e As DependencyPropertyChangedEventArgs)
            If sender IsNot Nothing Then
                CType(sender, MyExtraTextButton).LabText.Text = Conversions.ToString(e.NewValue)
            End If
        End Sub))

        Private _Show As Boolean

        Private IsLeftMouseHeld As Boolean

        Private Const AnimationColorIn As Integer = 120

        Private Const AnimationColorOut As Integer = 150

        Public Delegate Sub ClickEventHandler(sender As Object, e As MouseButtonEventArgs)
    End Class
End Namespace
