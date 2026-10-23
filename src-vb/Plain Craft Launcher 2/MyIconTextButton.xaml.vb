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
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <ContentProperty("Inlines")>
    <DesignerGenerated()>
    Public Partial Class MyIconTextButton
        Inherits Border

        Public Sub New()
            AddHandler MyBase.MouseLeftButtonUp, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.MyIconTextButton_MouseUp()
            End Sub
            AddHandler MyBase.MouseLeftButtonDown, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.MyIconTextButton_MouseDown()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.MyIconTextButton_MouseLeave()
            End Sub
            AddHandler MyBase.MouseEnter, AddressOf Me.RefreshColor
            AddHandler MyBase.Loaded, AddressOf Me.RefreshColor
            AddHandler MyBase.IsEnabledChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.RefreshColor(RuntimeHelpers.GetObjectValue(a0), a1)
            End Sub
            Me.Uuid = ModBase.GetUuid()
            Me._LogoScale = 1.0
            Me.IsMouseDown = False
            Me.InitializeComponent()
        End Sub

        Public Event Check As MyIconTextButton.CheckEventHandler

        Public Event Change As MyIconTextButton.ChangeEventHandler

        Public Property Logo As String
            Get
                Return Me.ShapeLogo.Data.ToString()
            End Get
            Set(value As String)
                Me.ShapeLogo.Data = CType(New GeometryConverter().ConvertFromString(value), Geometry)
            End Set
        End Property

        Public Property LogoScale As Double
            Get
                Return Me._LogoScale
            End Get
            Set(value As Double)
                Me._LogoScale = value
                If Not Information.IsNothing(Me.ShapeLogo) Then
                    Me.ShapeLogo.RenderTransform = New ScaleTransform() With { .ScaleX = Me.LogoScale, .ScaleY = Me.LogoScale }
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
                Return Conversions.ToString(MyBase.GetValue(MyIconTextButton.TextProperty))
            End Get
            Set(value As String)
                MyBase.SetValue(MyIconTextButton.TextProperty, value)
            End Set
        End Property

        Public Property ColorType As MyIconTextButton.ColorState
            Get
                Return CType(Conversions.ToInteger(MyBase.GetValue(MyIconTextButton.ColorTypeProperty)), MyIconTextButton.ColorState)
            End Get
            Set(value As MyIconTextButton.ColorState)
                If Me.ColorType <> value Then
                    MyBase.SetValue(MyIconTextButton.ColorTypeProperty, value)
                    Me.RefreshColor(Nothing, Nothing)
                End If
            End Set
        End Property

        Public Event Click As MyIconTextButton.ClickEventHandler

        Private Sub MyIconTextButton_MouseUp()
            If Me.IsMouseDown Then
                Logger.Info(String.Format("按下带图标按钮：{0}", Me.Text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyIconTextButton.xaml.vb")
                Me.IsMouseDown = False
                Dim clickEvent As MyIconTextButton.ClickEventHandler = Me.ClickEvent
                If clickEvent IsNot Nothing Then
                    clickEvent(Me, New ModBase.RouteEventArgs(True))
                End If
                Me.RaiseCustomEvent()
                Me.RefreshColor(Nothing, Nothing)
            End If
        End Sub

        Private Sub MyIconTextButton_MouseDown()
            Me.IsMouseDown = True
            Me.RefreshColor(Nothing, Nothing)
        End Sub

        Private Sub MyIconTextButton_MouseLeave()
            Me.IsMouseDown = False
            Me.RefreshColor(Nothing, Nothing)
        End Sub

        Private Sub RefreshColor(Optional obj As Object = Nothing, Optional e As Object = Nothing)
            Try
                If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 AndAlso Not False.Equals(RuntimeHelpers.GetObjectValue(e)) Then
                    Dim colorType As MyIconTextButton.ColorState = Me.ColorType
                    If colorType <> MyIconTextButton.ColorState.Black Then
                        If colorType = MyIconTextButton.ColorState.Highlight Then
                            If Me.IsMouseDown Then
                                ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, "ColorBrush6", 70, 0, Nothing, False), "MyIconTextButton Color " + Conversions.ToString(Me.Uuid), False)
                            ElseIf MyBase.IsMouseOver Then
                                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeLogo, Shape.FillProperty, "ColorBrush3", 100, 0, Nothing, False), ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 100, 0, Nothing, False) }, "MyIconTextButton Checked " + Conversions.ToString(Me.Uuid), False)
                                ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, "ColorBrushBg1", 100, 0, Nothing, False), "MyIconTextButton Color " + Conversions.ToString(Me.Uuid), False)
                            ElseIf MyBase.IsEnabled Then
                                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeLogo, Shape.FillProperty, "ColorBrush3", 150, 0, Nothing, False), ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 150, 0, Nothing, False) }, "MyIconTextButton Checked " + Conversions.ToString(Me.Uuid), False)
                                ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - MyBase.Background, 150, 0, Nothing, False), "MyIconTextButton Color " + Conversions.ToString(Me.Uuid), False)
                            Else
                                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeLogo, Shape.FillProperty, "ColorBrushGray5", 100, 0, Nothing, False), ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, "ColorBrushGray5", 100, 0, Nothing, False) }, "MyIconTextButton Checked " + Conversions.ToString(Me.Uuid), False)
                                ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - MyBase.Background, 150, 0, Nothing, False), "MyIconTextButton Color " + Conversions.ToString(Me.Uuid), False)
                            End If
                        End If
                    ElseIf Me.IsMouseDown Then
                        ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, "ColorBrush6", 70, 0, Nothing, False), "MyIconTextButton Color " + Conversions.ToString(Me.Uuid), False)
                    ElseIf MyBase.IsMouseOver Then
                        ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeLogo, Shape.FillProperty, "ColorBrush3", 100, 0, Nothing, False), ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 100, 0, Nothing, False) }, "MyIconTextButton Checked " + Conversions.ToString(Me.Uuid), False)
                        ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, "ColorBrushBg1", 100, 0, Nothing, False), "MyIconTextButton Color " + Conversions.ToString(Me.Uuid), False)
                    ElseIf MyBase.IsEnabled Then
                        ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeLogo, Shape.FillProperty, "ColorBrush1", 150, 0, Nothing, False), ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, "ColorBrush1", 150, 0, Nothing, False) }, "MyIconTextButton Checked " + Conversions.ToString(Me.Uuid), False)
                        ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - MyBase.Background, 150, 0, Nothing, False), "MyIconTextButton Color " + Conversions.ToString(Me.Uuid), False)
                    Else
                        ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeLogo, Shape.FillProperty, "ColorBrushGray5", 100, 0, Nothing, False), ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, "ColorBrushGray5", 100, 0, Nothing, False) }, "MyIconTextButton Checked " + Conversions.ToString(Me.Uuid), False)
                        ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - MyBase.Background, 150, 0, Nothing, False), "MyIconTextButton Color " + Conversions.ToString(Me.Uuid), False)
                    End If
                Else
                    ModAnimation.AniStop("MyIconTextButton Checked " + Conversions.ToString(Me.Uuid))
                    ModAnimation.AniStop("MyIconTextButton Color " + Conversions.ToString(Me.Uuid))
                    Dim colorType2 As MyIconTextButton.ColorState = Me.ColorType
                    If colorType2 <> MyIconTextButton.ColorState.Black Then
                        If colorType2 = MyIconTextButton.ColorState.Highlight Then
                            MyBase.Background = ModSecret.ColorSemiTransparent
                            Me.ShapeLogo.SetResourceReference(Shape.FillProperty, If(MyBase.IsEnabled, "ColorBrush3", "ColorBrushGray5"))
                            Me.LabText.SetResourceReference(TextBlock.ForegroundProperty, If(MyBase.IsEnabled, "ColorBrush3", "ColorBrushGray5"))
                        End If
                    Else
                        MyBase.Background = ModSecret.ColorSemiTransparent
                        Me.ShapeLogo.SetResourceReference(Shape.FillProperty, If(MyBase.IsEnabled, "ColorBrush1", "ColorBrushGray5"))
                        Me.LabText.SetResourceReference(TextBlock.ForegroundProperty, If(MyBase.IsEnabled, "ColorBrush1", "ColorBrushGray5"))
                    End If
                End If
            Catch ex As Exception
                Logger.Warn(ex, "刷新带图标按钮颜色出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyIconTextButton.xaml.vb")
            End Try
        End Sub

        Friend Overridable Property PanBack As MyIconTextButton

        Friend Overridable Property ShapeLogo As Path

        Friend Overridable Property LabText As TextBlock

        Public Uuid As Integer

        Private _LogoScale As Double

        Public Shared TextProperty As DependencyProperty = DependencyProperty.Register("Text", GetType(String), GetType(MyIconTextButton), New PropertyMetadata(Sub(sender As DependencyObject, e As DependencyPropertyChangedEventArgs)
            If Not Information.IsNothing(sender) Then
                CType(sender, MyIconTextButton).LabText.Text = Conversions.ToString(e.NewValue)
            End If
        End Sub))

        Public Shared ColorTypeProperty As DependencyProperty = DependencyProperty.Register("ColorType", GetType(MyIconTextButton.ColorState), GetType(MyIconTextButton), New PropertyMetadata(MyIconTextButton.ColorState.Black))

        Private IsMouseDown As Boolean

        Private Const AnimationTimeOfMouseIn As Integer = 100

        Private Const AnimationTimeOfMouseOut As Integer = 150

        Public Delegate Sub CheckEventHandler(sender As Object, raiseByMouse As Boolean)

        Public Delegate Sub ChangeEventHandler(sender As Object, raiseByMouse As Boolean)

        Public Enum ColorState
            Black
            Highlight
        End Enum

        Public Delegate Sub ClickEventHandler(sender As Object, e As ModBase.RouteEventArgs)
    End Class
End Namespace
