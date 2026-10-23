Imports System
Imports System.CodeDom.Compiler
Imports System.Collections
Imports System.Collections.Generic
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
    Public Partial Class MyRadioButton
        Inherits Border

        Public Sub New()
            AddHandler MyBase.MouseLeftButtonUp, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.Radiobox_MouseUp()
            End Sub
            AddHandler MyBase.MouseLeftButtonDown, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.Radiobox_MouseDown()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.Radiobox_MouseLeave()
            End Sub
            AddHandler MyBase.MouseEnter, AddressOf Me.RefreshColor
            AddHandler MyBase.MouseLeave, AddressOf Me.RefreshColor
            AddHandler MyBase.Loaded, AddressOf Me.RefreshColor
            Me.Uuid = ModBase.GetUuid()
            Me._LogoScale = 1.0
            Me._Checked = False
            Me._ColorType = MyRadioButton.ColorState.White
            Me.IsMouseDown = False
            Me.InitializeComponent()
        End Sub

        Public Event Check As MyRadioButton.CheckEventHandler

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

        Public Property Checked As Boolean
            Get
                Return Me._Checked
            End Get
            Set(value As Boolean)
                Me.SetChecked(value, False, True)
            End Set
        End Property

        Public Sub SetChecked(value As Boolean, raiseByMouse As Boolean, anime As Boolean)
            ' The following expression was wrapped in a checked-statement
            Try
                Dim flag As Boolean = False
                If Me._Checked <> value Then
                    Me._Checked = value
                    flag = True
                End If
                If MyBase.Parent IsNot Nothing Then
                    Dim list As List(Of MyRadioButton) = New List(Of MyRadioButton)()
                    Dim num As Integer = 0
                    Try
                        For Each obj As Object In CType(NewLateBinding.LateGet(MyBase.Parent, Nothing, "Children", New Object(-1) {}, Nothing, Nothing, Nothing), IEnumerable)
                            Dim objectValue As Object = RuntimeHelpers.GetObjectValue(obj)
                            If TypeOf objectValue Is MyRadioButton Then
                                list.Add(CType(objectValue, MyRadioButton))
                                If Conversions.ToBoolean(NewLateBinding.LateGet(objectValue, Nothing, "Checked", New Object(-1) {}, Nothing, Nothing, Nothing)) Then
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
                                For Each myRadioButton As MyRadioButton In list
                                    If myRadioButton.Checked AndAlso Not myRadioButton.Equals(Me) Then
                                        myRadioButton.Checked = False
                                    End If
                                Next
                                GoTo IL_016B
                            Finally
                                Dim enumerator2 As List(Of MyRadioButton).Enumerator
                                CType(enumerator2, IDisposable).Dispose()
                            End Try
                        End If
                        Dim flag2 As Boolean = False
                        Try
                            For Each myRadioButton2 As MyRadioButton In list
                                If myRadioButton2.Checked Then
                                    If flag2 Then
                                        myRadioButton2.Checked = False
                                    Else
                                        flag2 = True
                                    End If
                                End If
                            Next
                        Finally
                            Dim enumerator3 As List(Of MyRadioButton).Enumerator
                            CType(enumerator3, IDisposable).Dispose()
                        End Try
                    End If
                    IL_016B:
                    If flag Then
                        Me.RefreshColor(Nothing, anime)
                        If Me.Checked Then
                            Dim checkEvent As MyRadioButton.CheckEventHandler = Me.CheckEvent
                            If checkEvent IsNot Nothing Then
                                checkEvent(Me, raiseByMouse)
                            End If
                        End If
                        Me.RaiseCustomEvent()
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, "单选按钮勾选改变错误", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyRadioButton.xaml.vb")
            End Try
        End Sub

        Public ReadOnly Property Inlines As InlineCollection
            Get
                Return Me.LabText.Inlines
            End Get
        End Property

        Public Property Text As String
            Get
                Return Conversions.ToString(MyBase.GetValue(MyRadioButton.TextProperty))
            End Get
            Set(value As String)
                MyBase.SetValue(MyRadioButton.TextProperty, value)
            End Set
        End Property

        Public Property ColorType As MyRadioButton.ColorState
            Get
                Return Me._ColorType
            End Get
            Set(value As MyRadioButton.ColorState)
                Me._ColorType = value
                Me.RefreshColor(Nothing, Nothing)
            End Set
        End Property

        Public Event PreviewClick As MyRadioButton.PreviewClickEventHandler

        Private Sub Radiobox_MouseUp()
            If Not Me.Checked AndAlso Me.IsMouseDown Then
                Logger.Info(String.Format("按下单选按钮：{0}", Me.Text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyRadioButton.xaml.vb")
                Me.IsMouseDown = False
                Dim e As ModBase.RouteEventArgs = New ModBase.RouteEventArgs(True)
                Dim previewClickEvent As MyRadioButton.PreviewClickEventHandler = Me.PreviewClickEvent
                If previewClickEvent IsNot Nothing Then
                    previewClickEvent(Me, e)
                End If
                If Not e.Handled Then
                    Me.SetChecked(True, True, True)
                End If
            End If
        End Sub

        Private Sub Radiobox_MouseDown()
            If Not Me.Checked Then
                Me.IsMouseDown = True
                Me.RefreshColor(Nothing, Nothing)
            End If
        End Sub

        Private Sub Radiobox_MouseLeave()
            Me.IsMouseDown = False
        End Sub

        Private Sub RefreshColor(Optional obj As Object = Nothing, Optional e As Object = Nothing)
            Try
                If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 AndAlso Not False.Equals(RuntimeHelpers.GetObjectValue(e)) Then
                    Dim colorType As MyRadioButton.ColorState = Me.ColorType
                    If colorType <> MyRadioButton.ColorState.White Then
                        If colorType = MyRadioButton.ColorState.Highlight Then
                            If Me.Checked Then
                                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeLogo, Shape.FillProperty, New ModBase.MyColor(255.0, 255.0, 255.0) - Me.ShapeLogo.Fill, 120, 0, Nothing, False), ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, New ModBase.MyColor(255.0, 255.0, 255.0) - Me.LabText.Foreground, 120, 0, Nothing, False) }, "MyRadioButton Checked " + Conversions.ToString(Me.Uuid), False)
                                ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, "ColorBrush3", 120, 0, Nothing, False), "MyRadioButton Color " + Conversions.ToString(Me.Uuid), False)
                            ElseIf Me.IsMouseDown Then
                                ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, "ColorBrush6", 90, 0, Nothing, False), "MyRadioButton Color " + Conversions.ToString(Me.Uuid), False)
                            ElseIf MyBase.IsMouseOver Then
                                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeLogo, Shape.FillProperty, "ColorBrush3", 90, 0, Nothing, False), ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 90, 0, Nothing, False) }, "MyRadioButton Checked " + Conversions.ToString(Me.Uuid), False)
                                ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, "ColorBrush7", 90, 0, Nothing, False), "MyRadioButton Color " + Conversions.ToString(Me.Uuid), False)
                            Else
                                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeLogo, Shape.FillProperty, "ColorBrush3", 150, 0, Nothing, False), ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 150, 0, Nothing, False) }, "MyRadioButton Checked " + Conversions.ToString(Me.Uuid), False)
                                ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - MyBase.Background, 150, 0, Nothing, False), "MyRadioButton Color " + Conversions.ToString(Me.Uuid), False)
                            End If
                        End If
                    ElseIf Me.Checked Then
                        ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeLogo, Shape.FillProperty, "ColorBrush3", 120, 0, Nothing, False), ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 120, 0, Nothing, False) }, "MyRadioButton Checked " + Conversions.ToString(Me.Uuid), False)
                        ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, New ModBase.MyColor(255.0, 255.0, 255.0) - MyBase.Background, 120, 0, Nothing, False), "MyRadioButton Color " + Conversions.ToString(Me.Uuid), False)
                    ElseIf Me.IsMouseDown Then
                        ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, New ModBase.MyColor(120.0, ModSecret.Color8) - MyBase.Background, 60, 0, Nothing, False), "MyRadioButton Color " + Conversions.ToString(Me.Uuid), False)
                    ElseIf MyBase.IsMouseOver Then
                        ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeLogo, Shape.FillProperty, New ModBase.MyColor(255.0, 255.0, 255.0) - Me.ShapeLogo.Fill, 90, 0, Nothing, False), ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, New ModBase.MyColor(255.0, 255.0, 255.0) - Me.LabText.Foreground, 90, 0, Nothing, False) }, "MyRadioButton Checked " + Conversions.ToString(Me.Uuid), False)
                        ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, New ModBase.MyColor(50.0, ModSecret.Color8) - MyBase.Background, 90, 0, Nothing, False), "MyRadioButton Color " + Conversions.ToString(Me.Uuid), False)
                    Else
                        ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeLogo, Shape.FillProperty, New ModBase.MyColor(255.0, 255.0, 255.0) - Me.ShapeLogo.Fill, 150, 0, Nothing, False), ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, New ModBase.MyColor(255.0, 255.0, 255.0) - Me.LabText.Foreground, 150, 0, Nothing, False) }, "MyRadioButton Checked " + Conversions.ToString(Me.Uuid), False)
                        ModAnimation.AniStart(ModAnimation.AaColor(Me, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - MyBase.Background, 150, 0, Nothing, False), "MyRadioButton Color " + Conversions.ToString(Me.Uuid), False)
                    End If
                Else
                    ModAnimation.AniStop("MyRadioButton Checked " + Conversions.ToString(Me.Uuid))
                    ModAnimation.AniStop("MyRadioButton Color " + Conversions.ToString(Me.Uuid))
                    Dim colorType2 As MyRadioButton.ColorState = Me.ColorType
                    If colorType2 <> MyRadioButton.ColorState.White Then
                        If colorType2 = MyRadioButton.ColorState.Highlight Then
                            If Me.Checked Then
                                MyBase.SetResourceReference(Border.BackgroundProperty, "ColorBrush3")
                                Me.ShapeLogo.Fill = New ModBase.MyColor(255.0, 255.0, 255.0)
                                Me.LabText.Foreground = New ModBase.MyColor(255.0, 255.0, 255.0)
                            Else
                                MyBase.Background = ModSecret.ColorSemiTransparent
                                Me.ShapeLogo.SetResourceReference(Shape.FillProperty, "ColorBrush3")
                                Me.LabText.SetResourceReference(TextBlock.ForegroundProperty, "ColorBrush3")
                            End If
                        End If
                    ElseIf Me.Checked Then
                        MyBase.Background = New ModBase.MyColor(255.0, 255.0, 255.0)
                        Me.ShapeLogo.SetResourceReference(Shape.FillProperty, "ColorBrush3")
                        Me.LabText.SetResourceReference(TextBlock.ForegroundProperty, "ColorBrush3")
                    Else
                        MyBase.Background = ModSecret.ColorSemiTransparent
                        Me.ShapeLogo.Fill = New ModBase.MyColor(255.0, 255.0, 255.0)
                        Me.LabText.Foreground = New ModBase.MyColor(255.0, 255.0, 255.0)
                    End If
                End If
            Catch ex As Exception
                Logger.Warn(ex, "刷新单选按钮颜色出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyRadioButton.xaml.vb")
            End Try
        End Sub

        Friend Overridable Property PanBack As MyRadioButton

        Friend Overridable Property ShapeLogo As Path

        Friend Overridable Property LabText As TextBlock

        Public Uuid As Integer

        Private _LogoScale As Double

        Private _Checked As Boolean

        Public Shared TextProperty As DependencyProperty = DependencyProperty.Register("Text", GetType(String), GetType(MyRadioButton), New PropertyMetadata(Sub(sender As DependencyObject, e As DependencyPropertyChangedEventArgs)
            If sender IsNot Nothing Then
                CType(sender, MyRadioButton).LabText.Text = Conversions.ToString(e.NewValue)
            End If
        End Sub))

        Private _ColorType As MyRadioButton.ColorState

        Private IsMouseDown As Boolean

        Private Const AnimationTimeOfMouseIn As Integer = 90

        Private Const AnimationTimeOfMouseOut As Integer = 150

        Private Const AnimationTimeOfCheck As Integer = 120

        Public Delegate Sub CheckEventHandler(sender As Object, raiseByMouse As Boolean)

        Public Enum ColorState
            White
            Highlight
        End Enum

        Public Delegate Sub PreviewClickEventHandler(sender As Object, e As ModBase.RouteEventArgs)
    End Class
End Namespace
