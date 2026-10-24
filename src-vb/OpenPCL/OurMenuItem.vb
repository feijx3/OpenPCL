Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class OurMenuItem
        Inherits MenuItem

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.OurMenuItem_Loaded
            AddHandler MyBase.MouseEnter, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.IsEnabledChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.Click, AddressOf Me.OurMenuItem_Click
            Me.Uuid = ModBase.GetUuid()
        End Sub

        Private Sub OurMenuItem_Loaded(sender As Object, e As RoutedEventArgs)
            If MyBase.Icon IsNot Nothing Then
                Dim path As Path = CType(MyBase.GetTemplateChild("Icon"), Path)
                If path IsNot Nothing Then
                    path.Data = CType(New GeometryConverter().ConvertFromString(Conversions.ToString(MyBase.Icon)), Geometry)
                End If
            End If
            If TypeOf MyBase.Parent Is ContextMenu Then
                CType(MyBase.Parent, ContextMenu).Opacity = CDbl(Settings.[Get](Of Integer)("UiLauncherTransparent", Nothing)) / 1000.0 + 0.4
            End If
        End Sub

        Private Sub RefreshColor()
            Dim text As String
            Dim text2 As String
            Dim num As Integer
            If Not MyBase.IsEnabled Then
                text = "ColorBrushTransparent"
                text2 = "ColorBrushGray5"
                num = 200
            ElseIf MyBase.IsMouseOver Then
                text = "ColorBrush6"
                text2 = "ColorBrush2"
                num = 100
            Else
                text = "ColorBrushTransparent"
                text2 = "ColorBrush1"
                num = 200
            End If
            If Operators.CompareString(Me.ColorName, text, False) <> 0 Then
                Me.ColorName = text
                If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me, Control.BackgroundProperty, text, num, 0, Nothing, False), ModAnimation.AaColor(Me, Control.ForegroundProperty, text2, num, 0, Nothing, False) }, "OurMenuItem Color " + Conversions.ToString(Me.Uuid), False)
                    Return
                End If
                ModAnimation.AniStop("OurMenuItem Color " + Conversions.ToString(Me.Uuid))
                MyBase.SetResourceReference(Control.BackgroundProperty, text)
                MyBase.SetResourceReference(Control.ForegroundProperty, text2)
            End If
        End Sub

        Private Sub OurMenuItem_Click(sender As Object, e As RoutedEventArgs)
            Me.RaiseCustomEvent()
        End Sub

        Public Uuid As Integer

        Private Const AnimationTimeIn As Integer = 100

        Private Const AnimationTimeOut As Integer = 200

        Private ColorName As String
    End Class
End Namespace
