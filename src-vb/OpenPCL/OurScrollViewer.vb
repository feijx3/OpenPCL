Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class OurScrollViewer
        Inherits ScrollViewer

        Public Sub New()
            AddHandler MyBase.PreviewMouseWheel, AddressOf Me.OurScrollViewer_PreviewMouseWheel
            AddHandler MyBase.ScrollChanged, AddressOf Me.OurScrollViewer_ScrollChanged
            AddHandler MyBase.IsVisibleChanged, AddressOf Me.OurScrollViewer_IsVisibleChanged
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.Load()
            End Sub
            AddHandler MyBase.PreviewGotKeyboardFocus, AddressOf Me.OurScrollViewer_PreviewGotKeyboardFocus
            Me.DeltaMult = 1.0
        End Sub

        Public Property DeltaMult As Double

        Private Sub OurScrollViewer_PreviewMouseWheel(sender As Object, e As MouseWheelEventArgs)
            If e.Delta <> 0 AndAlso MyBase.ActualHeight <> 0.0 AndAlso MyBase.ScrollableHeight <> 0.0 Then
                Dim type As Type = e.Source.[GetType]()
                If NewLateBinding.LateGet(MyBase.Content, Nothing, "TemplatedParent", New Object(-1) {}, Nothing, Nothing, Nothing) Is Nothing Then
                    If(GetType(ComboBox).IsAssignableFrom(type) AndAlso CType(e.Source, ComboBox).IsDropDownOpen) OrElse (GetType(TextBox).IsAssignableFrom(type) AndAlso CType(e.Source, TextBox).AcceptsReturn) OrElse GetType(ComboBoxItem).IsAssignableFrom(type) Then
                        Return
                    End If
                    If TypeOf e.Source Is Visual Then
                        Dim presentationSource As PresentationSource = PresentationSource.FromVisual(CType(e.Source, Visual))
                        Dim text As String
                        If presentationSource Is Nothing Then
                            text = Nothing
                        Else
                            Dim rootVisual As Visual = presentationSource.RootVisual
                            text = If((rootVisual IsNot Nothing), rootVisual.[GetType]().FullName, Nothing)
                        End If
                        If Operators.CompareString(text, "System.Windows.Controls.Primitives.PopupRoot", False) = 0 Then
                            Return
                        End If
                    End If
                End If
                e.Handled = True
                Me.PerformVerticalOffsetDelta(CDbl((0 - e.Delta)))
            End If
        End Sub

        Public Sub PerformVerticalOffsetDelta(Delta As Double)
            ModAnimation.AniStart(ModAnimation.AaDouble(Sub(a0 As Object)
                Me._Lambda$__7-0(Conversions.ToDouble(a0))
            End Sub, Delta * Me.DeltaMult, 300, 0, New ModAnimation.AniEaseOutFluent(CType(6, ModAnimation.AniEasePower)), False), "", False)
        End Sub

        Private Sub OurScrollViewer_ScrollChanged(sender As Object, e As ScrollChangedEventArgs)
            Me.RealOffset = MyBase.VerticalOffset
            If ModMain.FrmMain IsNot Nothing AndAlso (e.VerticalChange <> 0.0 OrElse e.ViewportHeightChange <> 0.0) Then
                ModMain.FrmMain.BtnExtraBack.ShowRefresh()
            End If
        End Sub

        Private Sub OurScrollViewer_IsVisibleChanged(sender As Object, e As DependencyPropertyChangedEventArgs)
            ModMain.FrmMain.BtnExtraBack.ShowRefresh()
        End Sub

        Private Sub Load()
            Me.ScrollBar = CType(MyBase.GetTemplateChild("PART_VerticalScrollBar"), OurScrollBar)
        End Sub

        Private Sub OurScrollViewer_PreviewGotKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
            If e.NewFocus IsNot Nothing AndAlso TypeOf e.NewFocus Is OurSlider Then
                e.Handled = True
            End If
        End Sub

        Private RealOffset As Double

        Public ScrollBar As OurScrollBar
    End Class
End Namespace
