Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
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
    Public Partial Class MyIconButton
        Inherits Border

        Public Sub New()
            AddHandler MyBase.MouseLeftButtonUp, AddressOf Me.Button_MouseUp
            AddHandler MyBase.MouseLeftButtonDown, AddressOf Me.Button_MouseDown
            AddHandler MyBase.MouseLeftButtonUp, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.Button_MouseUp()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.Button_MouseLeave()
            End Sub
            AddHandler MyBase.MouseEnter, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshAnim()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshAnim()
            End Sub
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.RefreshAnim()
            End Sub
            AddHandler MyBase.IsEnabledChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.RefreshAnim()
            End Sub
            Me.Uuid = ModBase.GetUuid()
            Me._LogoScale = 1.0
            Me.Theme = MyIconButton.Themes.Color
            Me._Foreground = New SolidColorBrush(Color.FromRgb(128, 128, 128))
            Me.IsMouseDown = False
            Me.CanClick = True
            Me.InitializeComponent()
        End Sub

        Public Event Click As MyIconButton.ClickEventHandler

        Public Property Logo As String
            Get
                Return Me.Path.Data.ToString()
            End Get
            Set(value As String)
                Me.Path.Data = CType(New GeometryConverter().ConvertFromString(value), Geometry)
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

        Public Property Theme As MyIconButton.Themes

        Public Property Foreground As SolidColorBrush
            Get
                Return Me._Foreground
            End Get
            Set(value As SolidColorBrush)
                Me._Foreground = value
                ModAnimation.AniControlEnabled += 1
                Me.RefreshAnim()
                ModAnimation.AniControlEnabled -= 1
            End Set
        End Property

        Public Property CanClick As Boolean

        Private Sub Button_MouseUp(sender As Object, e As MouseButtonEventArgs)
            If Me.CanClick AndAlso Me.IsMouseDown Then
                Logger.Info(String.Format("按下图标按钮{0}", If(String.IsNullOrEmpty(MyBase.Name), "", ("：" + MyBase.Name))), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyIconButton.xaml.vb")
                Dim clickEvent As MyIconButton.ClickEventHandler = Me.ClickEvent
                If clickEvent IsNot Nothing Then
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e)
                End If
                e.Handled = True
                Me.Button_MouseUp()
                Me.RaiseCustomEvent()
            End If
        End Sub

        Private Sub Button_MouseDown(sender As Object, e As MouseButtonEventArgs)
            If Me.CanClick Then
                Me.IsMouseDown = True
                MyBase.Focus()
                ModAnimation.AniStart(ModAnimation.AaScaleTransform(Me.PanBack, 0.8 - CType(Me.PanBack.RenderTransform, ScaleTransform).ScaleX, 400, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False), "MyIconButton Scale " + Conversions.ToString(Me.Uuid), False)
            End If
        End Sub

        Private Sub Button_MouseUp()
            If Me.CanClick Then
                If Me.IsMouseDown Then
                    Me.IsMouseDown = False
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me.PanBack, 1.05 - CType(Me.PanBack.RenderTransform, ScaleTransform).ScaleX, 250, 0, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaScaleTransform(Me.PanBack, -0.05, 250, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False) }, "MyIconButton Scale " + Conversions.ToString(Me.Uuid), False)
                End If
                Me.RefreshAnim()
            End If
        End Sub

        Private Sub Button_MouseLeave()
            If Me.CanClick Then
                Me.IsMouseDown = False
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me.PanBack, 1.0 - CType(Me.PanBack.RenderTransform, ScaleTransform).ScaleX, 250, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) }, "MyIconButton Scale " + Conversions.ToString(Me.Uuid), False)
                Me.RefreshAnim()
            End If
        End Sub

        Public Sub RefreshAnim()
            Try
                If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                    If Me.PanBack.Background Is Nothing Then
                        Me.PanBack.Background = New ModBase.MyColor(0.0, 255.0, 255.0, 255.0)
                    End If
                    If Me.Path.Fill Is Nothing Then
                        Select Case Me.Theme
                            Case MyIconButton.Themes.Black
                                Me.Path.Fill = New ModBase.MyColor(160.0, 0.0, 0.0, 0.0)
                            Case MyIconButton.Themes.Red
                                Me.Path.Fill = New ModBase.MyColor(160.0, 255.0, 76.0, 76.0)
                            Case MyIconButton.Themes.Custom
                                Me.Path.Fill = New ModBase.MyColor(160.0, Me.Foreground)
                        End Select
                    End If
                    Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
                    If Not MyBase.IsEnabled Then
                        list.Add(ModAnimation.AaColor(Me.Path, Shape.FillProperty, "ColorBrushGray5", 120, 0, Nothing, False))
                    ElseIf MyBase.IsMouseOver Then
                        Select Case Me.Theme
                            Case MyIconButton.Themes.Color
                                list.Add(ModAnimation.AaColor(Me.Path, Shape.FillProperty, "ColorBrush2", 120, 0, Nothing, False))
                            Case MyIconButton.Themes.White
                                list.Add(ModAnimation.AaColor(Me.PanBack, Border.BackgroundProperty, New ModBase.MyColor(50.0, 255.0, 255.0, 255.0) - Me.PanBack.Background, 120, 0, Nothing, False))
                            Case MyIconButton.Themes.Black
                                list.Add(ModAnimation.AaColor(Me.Path, Shape.FillProperty, New ModBase.MyColor(230.0, 0.0, 0.0, 0.0) - Me.Path.Fill, 120, 0, Nothing, False))
                            Case MyIconButton.Themes.Red
                                list.Add(ModAnimation.AaColor(Me.Path, Shape.FillProperty, New ModBase.MyColor(255.0, 76.0, 76.0) - Me.Path.Fill, 120, 0, Nothing, False))
                            Case MyIconButton.Themes.Custom
                                list.Add(ModAnimation.AaColor(Me.Path, Shape.FillProperty, New ModBase.MyColor(255.0, Me.Foreground) - Me.Path.Fill, 120, 0, Nothing, False))
                        End Select
                    Else
                        Select Case Me.Theme
                            Case MyIconButton.Themes.Color
                                list.Add(ModAnimation.AaColor(Me.Path, Shape.FillProperty, "ColorBrush4", 150, 0, Nothing, False))
                                Me.PanBack.Background = New ModBase.MyColor(0.0, 255.0, 255.0, 255.0)
                            Case MyIconButton.Themes.White
                                list.Add(ModAnimation.AaColor(Me.Path, Shape.FillProperty, "ColorBrush8", 150, 0, Nothing, False))
                                list.Add(ModAnimation.AaColor(Me.PanBack, Border.BackgroundProperty, New ModBase.MyColor(0.0, 255.0, 255.0, 255.0) - Me.PanBack.Background, 150, 0, Nothing, False))
                            Case MyIconButton.Themes.Black
                                list.Add(ModAnimation.AaColor(Me.Path, Shape.FillProperty, New ModBase.MyColor(160.0, 0.0, 0.0, 0.0) - Me.Path.Fill, 150, 0, Nothing, False))
                                Me.PanBack.Background = New ModBase.MyColor(0.0, 255.0, 255.0, 255.0)
                            Case MyIconButton.Themes.Red
                                list.Add(ModAnimation.AaColor(Me.Path, Shape.FillProperty, New ModBase.MyColor(160.0, 255.0, 76.0, 76.0) - Me.Path.Fill, 150, 0, Nothing, False))
                                Me.PanBack.Background = New ModBase.MyColor(0.0, 255.0, 255.0, 255.0)
                            Case MyIconButton.Themes.Custom
                                list.Add(ModAnimation.AaColor(Me.Path, Shape.FillProperty, New ModBase.MyColor(160.0, Me.Foreground) - Me.Path.Fill, 150, 0, Nothing, False))
                                Me.PanBack.Background = New ModBase.MyColor(0.0, 255.0, 255.0, 255.0)
                        End Select
                    End If
                    ModAnimation.AniStart(list, "MyIconButton Color " + Conversions.ToString(Me.Uuid), False)
                Else
                    ModAnimation.AniStop("MyIconButton Color " + Conversions.ToString(Me.Uuid))
                    If MyBase.IsEnabled Then
                        Select Case Me.Theme
                            Case MyIconButton.Themes.Color
                                Me.Path.SetResourceReference(Shape.FillProperty, "ColorBrush5")
                            Case MyIconButton.Themes.White
                                Me.Path.SetResourceReference(Shape.FillProperty, "ColorBrush8")
                            Case MyIconButton.Themes.Black
                                Me.Path.Fill = New ModBase.MyColor(160.0, 0.0, 0.0, 0.0)
                            Case MyIconButton.Themes.Red
                                Me.Path.Fill = New ModBase.MyColor(160.0, 255.0, 76.0, 76.0)
                            Case MyIconButton.Themes.Custom
                                Me.Path.Fill = New ModBase.MyColor(160.0, Me.Foreground)
                        End Select
                    Else
                        Me.Path.SetResourceReference(Shape.FillProperty, "ColorBrushGray5")
                    End If
                    Me.PanBack.Background = New ModBase.MyColor(0.0, 255.0, 255.0, 255.0)
                End If
            Catch ex As Exception
                Logger.Warn(ex, "刷新图标按钮动画状态出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyIconButton.xaml.vb")
            End Try
        End Sub

        Friend Overridable Property PanBack As Border

        Friend Overridable Property Path As Path

        Public Uuid As Integer

        Private _LogoScale As Double

        Private _Foreground As SolidColorBrush

        Private IsMouseDown As Boolean

        Private Const AnimationColorIn As Integer = 120

        Private Const AnimationColorOut As Integer = 150

        Public Delegate Sub ClickEventHandler(sender As Object, e As EventArgs)

        Public Enum Themes
            Color
            White
            Black
            Red
            Custom
        End Enum
    End Class
End Namespace
