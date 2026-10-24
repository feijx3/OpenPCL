Imports System
Imports System.Windows
Imports System.Windows.Controls

Namespace PCL
    Public Class OurVirtualizingElement(Of T As FrameworkElement)
        Inherits FrameworkElement

        Public Sub New(Initializer As Func(Of T))
            Me.Initializer = Initializer
            LazyLoadBehavior.OnFirstEnterScrollViewerViewport(Me, Sub()
                Me.Init()
            End Sub)
        End Sub

        Public Function Init() As T
            Dim t As T = Me.Initializer()
            If MyBase.Parent IsNot Nothing Then
                If Not(TypeOf MyBase.Parent Is Panel) Then
                    Throw New Exception("OurVirtualizingElement 的父级必须是一个 Panel")
                End If
                Dim panel As Panel = CType(MyBase.Parent, Panel)
                Dim num As Integer = panel.Children.IndexOf(Me)
                panel.Children.RemoveAt(num)
                panel.Children.Insert(num, CType(CObj(t), UIElement))
            End If
            Return t
        End Function

        Public Shared Widening Operator CType(Virtualized As OurVirtualizingElement(Of T)) As T
            Return Virtualized.Init()
        End Operator

        Private Initializer As Func(Of T)
    End Class
End Namespace
