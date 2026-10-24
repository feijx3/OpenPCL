Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports MeloongCore
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class OurComboBoxItem
        Inherits ComboBoxItem

        Public Sub New()
            AddHandler MyBase.Unselected, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseMove, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.Selected, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.IsEnabledChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseLeftButtonUp, AddressOf Me.OurComboBoxItem_MouseLeftButtonUp
            Me.Uuid = ModBase.GetUuid()
            MyBase.Style = CType(MyBase.FindResource("OurComboBoxItem"), Style)
        End Sub

        Private Sub RefreshColor()
            Dim text As String
            Dim num As Double
            Dim num2 As Integer
            If MyBase.IsSelected Then
                text = "ColorBrush6"
                num = 1.0
                num2 = 100
            ElseIf MyBase.IsMouseOver Then
                text = "ColorBrush8"
                num = 1.0
                num2 = 100
            ElseIf MyBase.IsEnabled Then
                text = "ColorBrushTransparent"
                num = 1.0
                num2 = 300
            Else
                text = "ColorBrushTransparent"
                num = 0.4
                num2 = 300
            End If
            If Operators.CompareString(Me.BackColorName, text, False) <> 0 OrElse Me.FontOpacity <> num Then
                Me.BackColorName = text
                Me.FontOpacity = num
                If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me, Control.BackgroundProperty, Me.BackColorName, num2, 0, Nothing, False), ModAnimation.AaOpacity(Me, Me.FontOpacity - MyBase.Opacity, num2, 0, Nothing, False) }, "ComboBoxItem Color " + Conversions.ToString(Me.Uuid), False)
                    Return
                End If
                ModAnimation.AniStop("ComboBoxItem Color " + Conversions.ToString(Me.Uuid))
                MyBase.SetResourceReference(Control.BackgroundProperty, Me.BackColorName)
                MyBase.Opacity = Me.FontOpacity
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return MyBase.Content.ToString()
        End Function

        Public Shared Widening Operator CType(Value As OurComboBoxItem) As String
            Return Value.Content.ToString()
        End Operator

        Private Sub OurComboBoxItem_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
            Logger.Info(String.Format("选择下拉列表项：{0}", Me.ToString()), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurComboBoxItem.vb")
        End Sub

        Public Uuid As Integer

        Private Const AnimationTimeIn As Integer = 100

        Private Const AnimationTimeOut As Integer = 300

        Private BackColorName As String

        Private FontOpacity As Double
    End Class
End Namespace
