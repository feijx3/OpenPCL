Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media

Namespace PCL
    Public Class LazyLoadBehavior
        Public Shared Sub OnFirstEnterScrollViewerViewport(obj As DependencyObject, value As Action)
            obj.SetValue(LazyLoadBehavior.IsInViewportProperty, value)
        End Sub

        Private Shared Sub OnIsInViewportChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim Element As FrameworkElement = TryCast(d, FrameworkElement)
            If Element IsNot Nothing AndAlso e.NewValue IsNot Nothing Then
                Dim Handled As Boolean = False
                Dim Handler As EventHandler = Sub(a0 As Object, a1 As EventArgs)
                    MyBase._Lambda$__0()
                End Sub
                AddHandler Element.LayoutUpdated, Handler
            End If
        End Sub

        Private Shared Function FindParentScrollViewer(d As DependencyObject) As ScrollViewer
            While d IsNot Nothing
                If TypeOf d Is ScrollViewer Then
                    Return CType(d, ScrollViewer)
                End If
                d = VisualTreeHelper.GetParent(d)
            End While
            Return Nothing
        End Function

        Private Shared IsInViewportProperty As DependencyProperty = DependencyProperty.RegisterAttached("IsInViewport", GetType(Action), GetType(LazyLoadBehavior), New PropertyMetadata(Nothing, AddressOf LazyLoadBehavior.OnIsInViewportChanged))
    End Class
End Namespace
