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
Imports MeloongCore
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <ContentProperty("Inlines")>
    <DesignerGenerated()>
    Public Partial Class MyButton
        Inherits Border

        Public Sub New()
            AddHandler MyBase.MouseEnter, AddressOf Me.RefreshColor
            AddHandler MyBase.MouseLeave, AddressOf Me.RefreshColor
            AddHandler MyBase.Loaded, AddressOf Me.RefreshColor
            AddHandler MyBase.IsEnabledChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.RefreshColor(RuntimeHelpers.GetObjectValue(a0), a1)
            End Sub
            AddHandler MyBase.MouseLeftButtonUp, AddressOf Me.Button_MouseUp
            AddHandler MyBase.MouseLeftButtonDown, AddressOf Me.Button_MouseDown
            AddHandler MyBase.MouseEnter, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.Button_MouseEnter()
            End Sub
            AddHandler MyBase.MouseLeftButtonUp, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.Button_MouseUp()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.Button_MouseLeave()
            End Sub
            Me.Uuid = ModBase.GetUuid()
            Me._ColorType = MyButton.ColorState.Normal
            Me.IsMouseDown = False
            Me.InitializeComponent()
        End Sub

        Public Event Click As MyButton.ClickEventHandler

        Public ReadOnly Property Inlines As InlineCollection
            Get
                Return Me.LabText.Inlines
            End Get
        End Property

        Public Property Text As String
            Get
                Return Conversions.ToString(MyBase.GetValue(MyButton.TextProperty))
            End Get
            Set(value As String)
                MyBase.SetValue(MyButton.TextProperty, value)
            End Set
        End Property

        Public Property TextPadding As Thickness
            Get
                Return Me.LabText.Padding
            End Get
            Set(value As Thickness)
                Me.LabText.Padding = value
            End Set
        End Property

        Public Property ColorType As MyButton.ColorState
            Get
                Return Me._ColorType
            End Get
            Set(value As MyButton.ColorState)
                Me._ColorType = value
                Me.RefreshColor(Nothing, Nothing)
            End Set
        End Property

        Public Property Padding As Thickness
            Get
                Return Me.PanFore.Padding
            End Get
            Set(value As Thickness)
                Me.PanFore.Padding = value
            End Set
        End Property

        Public Property RealRenderTransform As Transform
            Get
                Return Me.PanFore.RenderTransform
            End Get
            Set(value As Transform)
                Me.PanFore.RenderTransform = value
            End Set
        End Property

        Private Sub RefreshColor(Optional obj As Object = Nothing, Optional e As Object = Nothing)
            Try
                If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                    If MyBase.IsEnabled Then
                        Select Case Me.ColorType
                            Case MyButton.ColorState.Normal
                                If MyBase.IsMouseOver Then
                                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.PanFore, Border.BorderBrushProperty, "ColorBrush3", 100, 0, Nothing, False) }, "MyButton Color " + Conversions.ToString(Me.Uuid), False)
                                Else
                                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.PanFore, Border.BorderBrushProperty, "ColorBrush1", 200, 0, Nothing, False) }, "MyButton Color " + Conversions.ToString(Me.Uuid), False)
                                End If
                            Case MyButton.ColorState.Highlight
                                If MyBase.IsMouseOver Then
                                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.PanFore, Border.BorderBrushProperty, "ColorBrush3", 100, 0, Nothing, False) }, "MyButton Color " + Conversions.ToString(Me.Uuid), False)
                                Else
                                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.PanFore, Border.BorderBrushProperty, "ColorBrush2", 200, 0, Nothing, False) }, "MyButton Color " + Conversions.ToString(Me.Uuid), False)
                                End If
                            Case MyButton.ColorState.Red
                                If MyBase.IsMouseOver Then
                                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.PanFore, Border.BorderBrushProperty, "ColorBrushRedLight", 100, 0, Nothing, False) }, "MyButton Color " + Conversions.ToString(Me.Uuid), False)
                                Else
                                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.PanFore, Border.BorderBrushProperty, "ColorBrushRedDark", 200, 0, Nothing, False) }, "MyButton Color " + Conversions.ToString(Me.Uuid), False)
                                End If
                        End Select
                    Else
                        ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.PanFore, Border.BorderBrushProperty, ModSecret.ColorGray4 - Me.PanFore.BorderBrush, 200, 0, Nothing, False) }, "MyButton Color " + Conversions.ToString(Me.Uuid), False)
                    End If
                Else
                    ModAnimation.AniStop("MyButton Color " + Conversions.ToString(Me.Uuid))
                    If MyBase.IsEnabled Then
                        Select Case Me.ColorType
                            Case MyButton.ColorState.Normal
                                If MyBase.IsMouseOver Then
                                    Me.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrush3")
                                Else
                                    Me.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrush1")
                                End If
                            Case MyButton.ColorState.Highlight
                                If MyBase.IsMouseOver Then
                                    Me.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrush3")
                                Else
                                    Me.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrush2")
                                End If
                            Case MyButton.ColorState.Red
                                If MyBase.IsMouseOver Then
                                    Me.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrushRedLight")
                                Else
                                    Me.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrushRedDark")
                                End If
                        End Select
                    Else
                        Me.PanFore.BorderBrush = ModSecret.ColorGray4
                    End If
                End If
            Catch ex As Exception
                Logger.Warn(ex, "刷新按钮颜色出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyButton.xaml.vb")
            End Try
        End Sub

        Private Sub Button_MouseUp(sender As Object, e As MouseButtonEventArgs)
            If Me.IsMouseDown Then
                Logger.Info(String.Format("按下按钮：{0}", Me.Text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyButton.xaml.vb")
                Dim clickEvent As MyButton.ClickEventHandler = Me.ClickEvent
                If clickEvent IsNot Nothing Then
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e)
                End If
                Me.RaiseCustomEvent()
            End If
        End Sub

        Private Sub Button_MouseDown(sender As Object, e As MouseButtonEventArgs)
            Me.IsMouseDown = True
            MyBase.Focus()
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me.PanFore, 0.955 - CType(Me.PanFore.RenderTransform, ScaleTransform).ScaleX, 80, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.ExtraStrong), False), ModAnimation.AaScaleTransform(Me.PanFore, -0.01, 700, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) }, "MyButton Scale " + Conversions.ToString(Me.Uuid), False)
        End Sub

        Private Sub Button_MouseEnter()
            ModAnimation.AniStart(ModAnimation.AaColor(Me.PanFore, Border.BackgroundProperty, If((Me._ColorType = MyButton.ColorState.Red), "ColorBrushRedBack", "ColorBrush7"), 100, 0, Nothing, False), "MyButton Background " + Conversions.ToString(Me.Uuid), False)
        End Sub

        Private Sub Button_MouseUp()
            If Me.IsMouseDown Then
                Me.IsMouseDown = False
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me.PanFore, 1.0 - CType(Me.PanFore.RenderTransform, ScaleTransform).ScaleX, 300, 10, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) }, "MyButton Scale " + Conversions.ToString(Me.Uuid), False)
            End If
        End Sub

        Private Sub Button_MouseLeave()
            ModAnimation.AniStart(ModAnimation.AaColor(Me.PanFore, Border.BackgroundProperty, "ColorBrushHalfWhite", 200, 0, Nothing, False), "MyButton Background " + Conversions.ToString(Me.Uuid), False)
            If Me.IsMouseDown Then
                Me.IsMouseDown = False
                ModAnimation.AniStart(ModAnimation.AaScaleTransform(Me.PanFore, 1.0 - CType(Me.PanFore.RenderTransform, ScaleTransform).ScaleX, 800, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False), "MyButton Scale " + Conversions.ToString(Me.Uuid), False)
            End If
        End Sub

        Friend Overridable Property PanBack As MyButton

        Friend Overridable Property PanFore As Border

        Friend Overridable Property LabText As TextBlock

        Public Uuid As Integer

        Public Shared TextProperty As DependencyProperty = DependencyProperty.Register("Text", GetType(String), GetType(MyButton), New PropertyMetadata(Sub(sender As DependencyObject, e As DependencyPropertyChangedEventArgs)
            If sender IsNot Nothing Then
                CType(sender, MyButton).LabText.Text = Conversions.ToString(e.NewValue)
            End If
        End Sub))

        Private _ColorType As MyButton.ColorState

        Public Shared PaddingProperty As DependencyProperty = DependencyProperty.Register("Padding", GetType(Thickness), GetType(MyButton), New PropertyMetadata(Sub(a0 As DependencyObject, a1 As DependencyPropertyChangedEventArgs)
            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_8(Of MyButton, DependencyPropertyChangedEventArgs)
            If MyButton._Closure$__.$I0-1 IsNot Nothing Then
                vb$AnonymousDelegate_ = MyButton._Closure$__.$I0-1
            Else
                Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_8(Of MyButton, DependencyPropertyChangedEventArgs) = Sub(sender As MyButton, e As DependencyPropertyChangedEventArgs)
                    If sender IsNot Nothing Then
                        Dim panFore As Border = sender.PanFore
                        Dim newValue As Object = e.NewValue
                        panFore.Padding = If((newValue IsNot Nothing), CType(newValue, Thickness), Nothing)
                    End If
                End Sub
                vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                MyButton._Closure$__.$I0-1 = vb$AnonymousDelegate_2
            End If
            vb$AnonymousDelegate_(CType(a0, MyButton), a1)
        End Sub))

        Private Const AnimationColorIn As Integer = 100

        Private Const AnimationColorOut As Integer = 200

        Private IsMouseDown As Boolean

        Public Delegate Sub ClickEventHandler(sender As Object, e As MouseButtonEventArgs)

        Public Enum ColorState
            Normal
            Highlight
            Red
        End Enum
    End Class
End Namespace
