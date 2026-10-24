Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class OurPageLeft
        Inherits Grid

        Public Sub New()
            Me.Uuid = ModBase.GetUuid()
        End Sub

        Public Property AnimatedControl As String
            Get
                Return Conversions.ToString(MyBase.GetValue(OurPageLeft.AnimatedControlProperty))
            End Get
            Set(value As String)
                MyBase.SetValue(OurPageLeft.AnimatedControlProperty, value)
            End Set
        End Property

        Public Sub TriggerShowAnimation()
            If Me.AnimatedControl Is Nothing Then
                If Not(TypeOf MyBase.RenderTransform Is ScaleTransform) Then
                    MyBase.RenderTransform = New ScaleTransform(0.96, 0.96)
                    MyBase.RenderTransformOrigin = New Point(0.5, 0.5)
                End If
                MyBase.Opacity = 0.0
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me, 1.0 - CType(MyBase.RenderTransform, ScaleTransform).ScaleX, 400, 0, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaOpacity(Me, 1.0, 100, 0, Nothing, False) }, "PageLeft PageChange " + Conversions.ToString(Me.Uuid), False)
                Return
            End If
            Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
            Dim num As Integer = 0
            Dim num2 As Integer = 0
            Try
                Dim enumerator As List(Of FrameworkElement).Enumerator = Me.GetAllAnimControls(True).GetEnumerator()
                While enumerator.MoveNext()
                    Dim CS$<>8__locals1 As OurPageLeft._Closure$__7-0 = New OurPageLeft._Closure$__7-0(CS$<>8__locals1)
                    CS$<>8__locals1.$VB$Me = Me
                    CS$<>8__locals1.$VB$Local_Element = enumerator.Current
                    CS$<>8__locals1.$VB$Local_Element = OurVirtualizingElement.TryInit(CS$<>8__locals1.$VB$Local_Element)
                    If CS$<>8__locals1.$VB$Local_Element.Visibility = Visibility.Collapsed Then
                        CS$<>8__locals1.$VB$Local_Element.Opacity = 1.0
                        CS$<>8__locals1.$VB$Local_Element.RenderTransform = New TranslateTransform(0.0, 0.0)
                        If TypeOf CS$<>8__locals1.$VB$Local_Element Is OurListItem Then
                            CType(CS$<>8__locals1.$VB$Local_Element, OurListItem).IsMouseOverAnimationEnabled = True
                        End If
                    Else
                        CS$<>8__locals1.$VB$Local_Element.Opacity = 0.0
                        CS$<>8__locals1.$VB$Local_Element.RenderTransform = New TranslateTransform(-25.0, 0.0)
                        If TypeOf CS$<>8__locals1.$VB$Local_Element Is OurListItem Then
                            CType(CS$<>8__locals1.$VB$Local_Element, OurListItem).IsMouseOverAnimationEnabled = False
                        End If
                        list.Add(ModAnimation.AaOpacity(CS$<>8__locals1.$VB$Local_Element, If((TypeOf CS$<>8__locals1.$VB$Local_Element Is TextBlock), 0.6, 1.0), 100, num2, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False))
                        list.Add(ModAnimation.AaTranslateX(CS$<>8__locals1.$VB$Local_Element, 5.0, 200, num2, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False))
                        list.Add(ModAnimation.AaTranslateX(CS$<>8__locals1.$VB$Local_Element, 20.0, 300, num2, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), False))
                        If TypeOf CS$<>8__locals1.$VB$Local_Element Is OurListItem Then
                            list.Add(ModAnimation.AaCode(Sub()
                                CType(CS$<>8__locals1.$VB$Local_Element, OurListItem).IsMouseOverAnimationEnabled = True
                                CType(CS$<>8__locals1.$VB$Local_Element, OurListItem).RefreshColor(CS$<>8__locals1.$VB$Me, New EventArgs())
                            End Sub, num2 + 280, False))
                        End If
                        num2 += Math.Max(15 - num, 7) * 2
                        num += 1
                    End If
                End While
            Finally
                Dim enumerator As List(Of FrameworkElement).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            ModAnimation.AniStart(list, "PageLeft PageChange " + Conversions.ToString(Me.Uuid), False)
        End Sub

        Public Sub TriggerHideAnimation()
            If Me.AnimatedControl Is Nothing Then
                If Not(TypeOf MyBase.RenderTransform Is ScaleTransform) Then
                    MyBase.RenderTransform = New ScaleTransform(1.0, 1.0)
                    MyBase.RenderTransformOrigin = New Point(0.5, 0.5)
                End If
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Me, 0.95 - CType(MyBase.RenderTransform, ScaleTransform).ScaleX, 110, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaOpacity(Me, -MyBase.Opacity, 80, 30, Nothing, False) }, "PageLeft PageChange " + Conversions.ToString(Me.Uuid), False)
                Return
            End If
            Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
            Dim num As Integer = 0
            Dim allAnimControls As List(Of FrameworkElement) = Me.GetAllAnimControls(False)
            Try
                For Each frameworkElement As FrameworkElement In allAnimControls
                    ' The following expression was wrapped in a unchecked-expression
                    ' The following expression was wrapped in a unchecked-expression
                    list.Add(ModAnimation.AaOpacity(frameworkElement, -frameworkElement.Opacity, 50, CInt(Math.Round(70.0 / CDbl(allAnimControls.Count) * CDbl(num))), Nothing, False))
                    list.Add(ModAnimation.AaTranslateX(frameworkElement, -6.0, 50, CInt(Math.Round(70.0 / CDbl(allAnimControls.Count) * CDbl(num))), Nothing, False))
                    num += 1
                Next
            Finally
                Dim enumerator As List(Of FrameworkElement).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            ModAnimation.AniStart(list, "PageLeft PageChange " + Conversions.ToString(Me.Uuid), False)
        End Sub

        Private Function GetAllAnimControls(Optional IgnoreInvisibility As Boolean = False) As List(Of FrameworkElement)
            Dim list As List(Of FrameworkElement) = New List(Of FrameworkElement)()
            Me.GetAllAnimControls(CType(MyBase.FindName(Me.AnimatedControl), FrameworkElement), list, IgnoreInvisibility)
            Return list
        End Function

        Private Sub GetAllAnimControls(Element As FrameworkElement, ByRef AllControls As List(Of FrameworkElement), IgnoreInvisibility As Boolean)
            If IgnoreInvisibility OrElse Element.Visibility <> Visibility.Collapsed Then
                If TypeOf Element Is OurTextButton Then
                    AllControls.Add(Element)
                    Return
                End If
                If TypeOf Element Is OurListItem Then
                    AllControls.Add(Element)
                    Return
                End If
                If TypeOf Element Is ContentControl Then
                    Me.GetAllAnimControls(CType(CType(Element, ContentControl).Content, FrameworkElement), AllControls, IgnoreInvisibility)
                    Return
                End If
                If TypeOf Element Is Panel Then
                    Try
                        For Each obj As Object In CType(Element, Panel).Children
                            Dim frameworkElement As FrameworkElement = CType(obj, FrameworkElement)
                            Me.GetAllAnimControls(frameworkElement, AllControls, IgnoreInvisibility)
                        Next
                        Return
                    Finally
                        Dim enumerator As IEnumerator
                        If TypeOf enumerator Is IDisposable Then
                            TryCast(enumerator, IDisposable).Dispose()
                        End If
                    End Try
                End If
                AllControls.Add(Element)
            End If
        End Sub

        Private Uuid As Integer

        Public Shared AnimatedControlProperty As DependencyProperty = DependencyProperty.Register("AnimatedControl", GetType(String), GetType(OurPageLeft), New PropertyMetadata(Nothing))
    End Class
End Namespace
