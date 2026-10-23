Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports MeloongCore
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class MySlider
        Inherits Border
        Implements ISettingControl

        Public Sub New()
            AddHandler MyBase.SizeChanged, AddressOf Me.RefreshWidth
            AddHandler MyBase.MouseLeftButtonDown, AddressOf Me.DragStart
            AddHandler MyBase.IsEnabledChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseEnter, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseEnter, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.MySlider_MouseEnter()
            End Sub
            AddHandler MyBase.KeyDown, AddressOf Me.MySlider_KeyDown
            AddHandler Me.Change, Sub(a0 As Object, a1 As Boolean)
                Me.SaveSetting()
            End Sub
            Me.Uuid = ModBase.GetUuid()
            Me._MaxValue = 100
            Me.ChangeByKey = False
            Me._Value = 0
            Me.ValueByKey = 1UI
            Me.InitializeComponent()
        End Sub

        Public Event Change As MySlider.ChangeEventHandler

        Public Event PreviewChange As MySlider.PreviewChangeEventHandler

        Public Property MaxValue As Integer
            Get
                Return Me._MaxValue
            End Get
            Set(value As Integer)
                If value <> Me._MaxValue Then
                    Me._MaxValue = value
                    Me.RefreshWidth(Nothing, Nothing)
                End If
            End Set
        End Property

        Public Property Value As Integer
            Get
                Return Me._Value
            End Get
            Set(value As Integer)
                Try
                    value = value.Clamp(0, Me.MaxValue)
                    If Me._Value <> value Then
                        Dim value2 As Integer = Me._Value
                        Me._Value = value
                        If ModAnimation.AniControlEnabled = 0 Then
                            Dim e As ModBase.RouteEventArgs = New ModBase.RouteEventArgs(False)
                            Dim previewChangeEvent As MySlider.PreviewChangeEventHandler = Me.PreviewChangeEvent
                            If previewChangeEvent IsNot Nothing Then
                                previewChangeEvent(Me, e)
                            End If
                            If e.Handled Then
                                Me._Value = value2
                                Me.DragStop()
                                Return
                            End If
                        End If
                        If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                            If MyBase.ActualWidth < Me.ShapeDot.Width Then
                                Return
                            End If
                            Dim num As Double = CDbl(Me._Value) / CDbl(Me.MaxValue) * (MyBase.ActualWidth - Me.ShapeDot.Width)
                            Dim num2 As Double = Math.Abs(Me.LineFore.Width / (MyBase.ActualWidth - Me.ShapeDot.Width) - CDbl(Me._Value) / CDbl(Me.MaxValue))
                            Dim num3 As Double = (1.0 - Math.Pow(1.0 - num2, 3.0)) * 300.0 + CDbl(If(Me.ChangeByKey, 100, 0))
                            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaWidth(Me.LineFore, Math.Max(0.0, num + If((num < 0.5), 0.0, 0.5)) - Me.LineFore.Width, CInt(Math.Round(num3)), 0, CType(If((num3 > 50.0), New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), New ModAnimation.AniEaseLinear()), ModAnimation.AniEase), False), ModAnimation.AaWidth(Me.LineBack, Math.Max(0.0, MyBase.ActualWidth - Me.ShapeDot.Width - num + If((MyBase.ActualWidth - Me.ShapeDot.Width - num < 0.5), 0.0, 0.5)) - Me.LineBack.Width, CInt(Math.Round(num3)), 0, CType(If((num3 > 50.0), New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), New ModAnimation.AniEaseLinear()), ModAnimation.AniEase), False), ModAnimation.AaX(Me.ShapeDot, num - Me.ShapeDot.Margin.Left, CInt(Math.Round(num3)), 0, CType(If((num3 > 50.0), New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), New ModAnimation.AniEaseLinear()), ModAnimation.AniEase), False) }, "MySlider Progress " + Conversions.ToString(Me.Uuid), False)
                        Else
                            Me.RefreshWidth(Nothing, Nothing)
                        End If
                        If ModAnimation.AniControlEnabled = 0 Then
                            Dim changeEvent As MySlider.ChangeEventHandler = Me.ChangeEvent
                            If changeEvent IsNot Nothing Then
                                changeEvent(Me, False)
                            End If
                        End If
                    End If
                Catch ex As Exception
                    Logger.[Error](ex, "滑动条进度改变出错", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MySlider.xaml.vb")
                End Try
            End Set
        End Property

        Private Sub RefreshWidth(sender As Object, e As SizeChangedEventArgs)
            If Not Information.IsNothing(e) Then
                Me.PanMain.Width = e.NewSize.Width
            End If
            ModAnimation.AniStop("MySlider Progress " + Conversions.ToString(Me.Uuid))
            Dim num As Double = CDbl(Me._Value) / CDbl(Me.MaxValue) * (MyBase.ActualWidth - Me.ShapeDot.Width)
            Me.LineFore.Width = Math.Max(0.0, num + If((num < 0.5), 0.0, 0.5))
            Me.LineBack.Width = Math.Max(0.0, MyBase.ActualWidth - Me.ShapeDot.Width - num + If((MyBase.ActualWidth - Me.ShapeDot.Width - num < 0.5), 0.0, 0.5))
            Me.ShapeDot.Margin = New Thickness(num, Me.ShapeDot.Margin.Top, Me.ShapeDot.Margin.Right, Me.ShapeDot.Margin.Bottom)
        End Sub

        Private Sub DragStart(sender As Object, e As MouseButtonEventArgs)
            e.Handled = True
            ModMain.DragControl = Me
            Me.RefreshColor()
            ModMain.FrmMain.DragDoing()
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me.ShapeDot, 1.3 - CType(Me.ShapeDot.RenderTransform, ScaleTransform).ScaleX, 40, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) }, "MySlider Scale " + Conversions.ToString(Me.Uuid), False)
            Me.RefreshPopup()
            ModAnimation.AniStop("MySlider KeyPopup " + Conversions.ToString(Me.Uuid))
        End Sub

        Public Sub DragDoing()
            ' The following expression was wrapped in a checked-statement
            Dim num As Integer = CInt(Math.Round(((Mouse.GetPosition(Me.PanMain).X - Me.ShapeDot.Width / 2.0) / (MyBase.ActualWidth - Me.ShapeDot.Width)).Clamp(0.0, 1.0) * CDbl(Me.MaxValue)))
            If num <> Me.Value Then
                Me.Value = num
            End If
            Me.RefreshPopup()
        End Sub

        Public Sub DragStop()
            Me.RefreshColor()
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me.ShapeDot, 1.0 - CType(Me.ShapeDot.RenderTransform, ScaleTransform).ScaleX, 200, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) }, "MySlider Scale " + Conversions.ToString(Me.Uuid), False)
            Me.Popup.IsOpen = False
        End Sub

        Public Sub RefreshPopup()
            If Me.GetHintText IsNot Nothing Then
                Me.Popup.IsOpen = True
                Me.TextHint.Text = Conversions.ToString(Me.GetHintText.DynamicInvoke(New Object() { Me.Value }))
                Dim typeface As Typeface = New Typeface(Me.TextHint.FontFamily, Me.TextHint.FontStyle, Me.TextHint.FontWeight, Me.TextHint.FontStretch)
                Dim formattedText As FormattedText = New FormattedText(Me.TextHint.Text, Thread.CurrentThread.CurrentCulture, Me.TextHint.FlowDirection, typeface, Me.TextHint.FontSize, Me.TextHint.Foreground, CDbl(ModBase.DPI))
                Me.TextHint.Width = formattedText.Width
            End If
        End Sub

        Private Sub RefreshColor()
            Try
                Dim text As String
                Dim text2 As String
                Dim num As Integer
                If MyBase.IsEnabled Then
                    If Not Information.IsNothing(RuntimeHelpers.GetObjectValue(ModMain.DragControl)) AndAlso ModMain.DragControl.Equals(Me) Then
                        text = "ColorBrush3"
                        text2 = "ColorBrush3"
                        num = 40
                    ElseIf MyBase.IsMouseOver Then
                        text = "ColorBrush3"
                        text2 = "ColorBrush3"
                        num = 40
                    Else
                        text = "ColorBrushBg0"
                        text2 = "ColorBrushBg0"
                        num = 100
                    End If
                Else
                    text = "ColorBrushGray5"
                    text2 = "ColorBrushGray5"
                    num = 200
                End If
                If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me, Border.BorderBrushProperty, text, num, 0, Nothing, False), ModAnimation.AaColor(Me.ShapeDot, Shape.FillProperty, text2, num, 0, Nothing, False) }, "MySlider Color " + Conversions.ToString(Me.Uuid), False)
                Else
                    ModAnimation.AniStop("MySlider Color " + Conversions.ToString(Me.Uuid))
                    MyBase.SetResourceReference(Border.BorderBrushProperty, text)
                    Me.ShapeDot.SetResourceReference(Shape.FillProperty, text2)
                End If
            Catch ex As Exception
                Logger.Warn(ex, "滑动条颜色改变出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MySlider.xaml.vb")
            End Try
        End Sub

        Public Property ValueByKey As UInteger

        Private Sub MySlider_MouseEnter()
            MyBase.Focus()
        End Sub

        Private Sub MySlider_KeyDown(sender As Object, e As KeyEventArgs)
            ' The following expression was wrapped in a checked-statement
            If Not Object.ReferenceEquals(Me, RuntimeHelpers.GetObjectValue(ModMain.DragControl)) Then
                If e.Key = Key.Left Then
                    Me.ChangeByKey = True
                    Me.Value = CInt((CLng(Me.Value) - CLng((CULng(Me.ValueByKey)))))
                    Me.ChangeByKey = False
                    e.Handled = True
                Else
                    If e.Key <> Key.Right Then
                        Return
                    End If
                    Me.ChangeByKey = True
                    Me.Value = CInt((CLng(Me.Value) + CLng((CULng(Me.ValueByKey)))))
                    Me.ChangeByKey = False
                    e.Handled = True
                End If
                If Me.GetHintText IsNot Nothing Then
                    Me.RefreshPopup()
                    ModAnimation.AniStop("MySlider KeyPopup " + Conversions.ToString(Me.Uuid))
                    ModAnimation.AniStart(ModAnimation.AaCode(Sub()
                        Me.Popup.IsOpen = False
                    End Sub, CInt(Math.Round(700.0 * ModAnimation.AniSpeed)), False), "MySlider KeyPopup " + Conversions.ToString(Me.Uuid), False)
                End If
            End If
        End Sub

        Sub RefreshSetting(NewValue As String) Implements PCL.ISettingControl.RefreshSetting
            ' The following expression was wrapped in a checked-expression
            Me.Value = CInt(Math.Round(ModBase.Val(NewValue)))
        End Sub

        Function GetCurrentSetting() As String Implements PCL.ISettingControl.GetCurrentSetting
            Return Conversions.ToString(Me.Value)
        End Function

        Private Sub SaveSetting()
            SettingService.SaveSetting(Me)
        End Sub

        Friend Overridable Property PanBack As MySlider

        Friend Overridable Property PanMain As Grid

        Friend Overridable Property LineBack As Line

        Friend Overridable Property LineFore As Line

        Friend Overridable Property ShapeDot As Ellipse

        Friend Overridable Property Popup As Popup

        Friend Overridable Property TextHint As TextBlock

        Public Uuid As Integer

        Private _MaxValue As Integer

        Private ChangeByKey As Boolean

        Private _Value As Integer

        Public GetHintText As [Delegate]

        Public Delegate Sub ChangeEventHandler(sender As Object, user As Boolean)

        Public Delegate Sub PreviewChangeEventHandler(sender As Object, e As ModBase.RouteEventArgs)
    End Class
End Namespace
