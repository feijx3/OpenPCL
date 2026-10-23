Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports MeloongCore
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class MyVirtualizingElement
        Inherits FrameworkElement

        Public Sub New(Initializer As Func(Of FrameworkElement))
            Me.Initializer = Initializer
            LazyLoadBehavior.OnFirstEnterScrollViewerViewport(Me, Sub()
                Me.Init()
            End Sub)
        End Sub

        Public Function Init() As FrameworkElement
            Dim frameworkElement As FrameworkElement = Me.Initializer()
            If MyBase.Parent IsNot Nothing Then
                If Not(TypeOf MyBase.Parent Is Panel) Then
                    Throw New Exception("MyVirtualizingElement 的父级必须是一个 Panel")
                End If
                Dim panel As Panel = CType(MyBase.Parent, Panel)
                Dim num As Integer = panel.Children.IndexOf(Me)
                panel.Children.RemoveAt(num)
                panel.Children.Insert(num, frameworkElement)
            End If
            Return frameworkElement
        End Function

        Public Shared Function TryInit(Element As FrameworkElement) As FrameworkElement
            Dim frameworkElement As FrameworkElement
            If GeneralUtils.IsGenericInstanceOf(Element, GetType(MyVirtualizingElement(Of))) Then
                frameworkElement = CType(NewLateBinding.LateGet(Element, Nothing, "Init", New Object(-1) {}, Nothing, Nothing, Nothing), FrameworkElement)
            ElseIf TypeOf Element Is MyVirtualizingElement Then
                frameworkElement = CType(Element, MyVirtualizingElement).Init()
            Else
                frameworkElement = Element
            End If
            Return frameworkElement
        End Function

        Private Initializer As Func(Of FrameworkElement)
    End Class
End Namespace
