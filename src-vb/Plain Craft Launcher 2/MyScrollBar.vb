Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Input
Imports MeloongCore
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class MyScrollBar
        Inherits ScrollBar

        Public Sub New()
            AddHandler MyBase.IsEnabledChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.GotMouseCapture, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.LostMouseCapture, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseEnter, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.IsVisibleChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.RefreshColor()
            End Sub
            Me.Uuid = ModBase.GetUuid()
        End Sub

        Private Sub RefreshColor()
            Try
                Dim num As Double
                Dim num2 As Integer
                Dim text As String
                If Not MyBase.IsVisible Then
                    num = 0.0
                    num2 = 20
                    text = "ColorBrush4"
                ElseIf MyBase.IsMouseCaptureWithin Then
                    num = 1.0
                    text = "ColorBrush4"
                    num2 = 50
                ElseIf MyBase.IsMouseOver Then
                    num = 0.9
                    text = "ColorBrush3"
                    num2 = 130
                Else
                    num = 0.5
                    text = "ColorBrush4"
                    num2 = 180
                End If
                If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me, Control.ForegroundProperty, text, num2, 0, Nothing, False), ModAnimation.AaOpacity(Me, num - MyBase.Opacity, num2, 0, Nothing, False) }, "MyScrollBar Color " + Conversions.ToString(Me.Uuid), False)
                Else
                    ModAnimation.AniStop("MyScrollBar Color " + Conversions.ToString(Me.Uuid))
                    MyBase.SetResourceReference(Control.ForegroundProperty, text)
                    MyBase.Opacity = num
                End If
            Catch ex As Exception
                Logger.Warn(ex, "滚动条颜色改变出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyScrollBar.vb")
            End Try
        End Sub

        Public Uuid As Integer
    End Class
End Namespace
