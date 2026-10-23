Imports System
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Effects
Imports System.Windows.Media.Media3D
Imports System.Windows.Threading

Namespace MeloongCore.Wpf
    <NullableContext(1)>
    <Nullable(0)>
    Public Module ModernTooltipService
        Public Sub SetUseModernTooltip(element As DependencyObject, value As Boolean)
            element.SetValue(ModernTooltipService.UseModernTooltipProperty, value)
        End Sub

        Public Function GetUseModernTooltip(element As DependencyObject) As Boolean
            Return CBool(element.GetValue(ModernTooltipService.UseModernTooltipProperty))
        End Function

        Public Sub SetFollowMouse(element As DependencyObject, value As Boolean)
            element.SetValue(ModernTooltipService.FollowMouseProperty, value)
        End Sub

        Public Function GetFollowMouse(element As DependencyObject) As Boolean
            Return CBool(element.GetValue(ModernTooltipService.FollowMouseProperty))
        End Function

        Friend Sub Init()
            If ModernTooltipService.initialized Then
                Return
            End If
            ModernTooltipService.initialized = True
            EventManager.RegisterClassHandler(GetType(FrameworkElement), UIElement.MouseEnterEvent, AddressOf ModernTooltipService.OnMouseEnter, True)
            EventManager.RegisterClassHandler(GetType(FrameworkElement), UIElement.MouseMoveEvent, AddressOf ModernTooltipService.OnMouseMove, True)
            EventManager.RegisterClassHandler(GetType(FrameworkElement), UIElement.MouseLeaveEvent, AddressOf ModernTooltipService.OnMouseLeave, True)
            EventManager.RegisterClassHandler(GetType(FrameworkElement), UIElement.PreviewMouseUpEvent, AddressOf ModernTooltipService.OnPreviewMouseUp, True)
            EventManager.RegisterClassHandler(GetType(FrameworkElement), ToolTipService.ToolTipOpeningEvent, AddressOf ModernTooltipService.OnToolTipOpening, True)
            Dim typeFromHandle As Type = GetType(FrameworkElement)
            Dim unloadedEvent As RoutedEvent = FrameworkElement.UnloadedEvent
            Dim <>9__4_ As RoutedEventHandler = ModernTooltipService.<>c.<>9__4_0
            Dim [delegate] As [Delegate] = <>9__4_
            If <>9__4_ Is Nothing Then
                Dim routedEventHandler As RoutedEventHandler = Sub(sender As Object, _ As RoutedEventArgs)
                    Dim frameworkElement As FrameworkElement = TryCast(sender, FrameworkElement)
                    If frameworkElement IsNot Nothing AndAlso frameworkElement Is ModernTooltipService.currentOwner Then
                        ModernTooltipService.Close(False)
                    End If
                End Sub
                [delegate] = routedEventHandler
                ModernTooltipService.<>c.<>9__4_0 = routedEventHandler
            End If
            EventManager.RegisterClassHandler(typeFromHandle, unloadedEvent, [delegate], True)
            Dim typeFromHandle2 As Type = GetType(ComboBox)
            Dim loadedEvent As RoutedEvent = FrameworkElement.LoadedEvent
            Dim <>9__4_2 As RoutedEventHandler = ModernTooltipService.<>c.<>9__4_1
            Dim delegate2 As [Delegate] = <>9__4_2
            If <>9__4_2 Is Nothing Then
                Dim routedEventHandler2 As RoutedEventHandler = Sub(sender As Object, _ As RoutedEventArgs)
                    ModernTooltipService.HookComboBoxDropDown(TryCast(sender, ComboBox))
                End Sub
                delegate2 = routedEventHandler2
                ModernTooltipService.<>c.<>9__4_1 = routedEventHandler2
            End If
            EventManager.RegisterClassHandler(typeFromHandle2, loadedEvent, delegate2, True)
            Dim typeFromHandle3 As Type = GetType(ComboBox)
            Dim previewMouseDownEvent As RoutedEvent = UIElement.PreviewMouseDownEvent
            Dim <>9__4_3 As MouseButtonEventHandler = ModernTooltipService.<>c.<>9__4_2
            Dim delegate3 As [Delegate] = <>9__4_3
            If <>9__4_3 Is Nothing Then
                Dim mouseButtonEventHandler As MouseButtonEventHandler = Sub(sender As Object, _ As MouseButtonEventArgs)
                    ModernTooltipService.HookComboBoxDropDown(TryCast(sender, ComboBox))
                End Sub
                delegate3 = mouseButtonEventHandler
                ModernTooltipService.<>c.<>9__4_2 = mouseButtonEventHandler
            End If
            EventManager.RegisterClassHandler(typeFromHandle3, previewMouseDownEvent, delegate3, True)
        End Sub

        Shared Sub New()
            ModernTooltipService.backgroundBrush.Freeze()
            ModernTooltipService.borderBrush.Freeze()
            ModernTooltipService.foregroundBrush.Freeze()
            AddHandler ModernTooltipService.openTimer.Tick, Sub(_ As Object, _ As EventArgs)
                ModernTooltipService.openTimer.[Stop]()
                If ModernTooltipService.currentOwner IsNot Nothing Then
                    ModernTooltipService.Show(ModernTooltipService.currentOwner, ModernTooltipService.lastMousePoint)
                End If
            End Sub
        End Sub

        Private Sub OnMouseEnter(sender As Object, e As MouseEventArgs)
            Dim reference As FrameworkElement = TryCast(sender, FrameworkElement)
            If reference Is Nothing Then
                Return
            End If
            reference.Dispatcher.BeginInvoke(New Action(Sub()
                ModernTooltipService.RefreshCurrentOwner(reference)
            End Sub), DispatcherPriority.Input, Array.Empty(Of Object)())
        End Sub

        Private Sub OnMouseMove(sender As Object, e As MouseEventArgs)
            Dim frameworkElement As FrameworkElement = TryCast(sender, FrameworkElement)
            If frameworkElement Is Nothing Then
                Return
            End If
            Dim popup As Popup
            If Mouse.LeftButton = MouseButtonState.Pressed Then
                Dim frameworkElement2 As FrameworkElement = ModernTooltipService.currentOwner
                If frameworkElement2 IsNot Nothing Then
                    ModernTooltipService.lastMousePoint = Mouse.GetPosition(frameworkElement2)
                    If ModernTooltipService.GetFollowMouse(frameworkElement2) Then
                        popup = ModernTooltipService.popup
                        If popup IsNot Nothing AndAlso popup.IsOpen Then
                            ModernTooltipService.UpdatePosition(frameworkElement2, ModernTooltipService.lastMousePoint)
                            Return
                        End If
                    End If
                Else
                    popup = ModernTooltipService.popup
                    If popup IsNot Nothing AndAlso popup.IsOpen Then
                        Dim frameworkElement3 As FrameworkElement = TryCast(popup.PlacementTarget, FrameworkElement)
                        If frameworkElement3 IsNot Nothing AndAlso ModernTooltipService.GetFollowMouse(frameworkElement3) Then
                            ModernTooltipService.lastMousePoint = Mouse.GetPosition(frameworkElement3)
                            ModernTooltipService.UpdatePosition(frameworkElement3, ModernTooltipService.lastMousePoint)
                        End If
                    End If
                End If
                Return
            End If
            ModernTooltipService.RefreshCurrentOwner(frameworkElement)
            Dim frameworkElement4 As FrameworkElement = ModernTooltipService.currentOwner
            If frameworkElement4 IsNot Nothing Then
                ModernTooltipService.lastMousePoint = Mouse.GetPosition(frameworkElement4)
                If ModernTooltipService.GetFollowMouse(frameworkElement4) Then
                    popup = ModernTooltipService.popup
                    If popup IsNot Nothing AndAlso popup.IsOpen Then
                        ModernTooltipService.UpdatePosition(frameworkElement4, ModernTooltipService.lastMousePoint)
                    End If
                End If
                Return
            End If
            popup = ModernTooltipService.popup
            If popup IsNot Nothing AndAlso popup.IsOpen Then
                Dim frameworkElement5 As FrameworkElement = TryCast(popup.PlacementTarget, FrameworkElement)
                If frameworkElement5 IsNot Nothing AndAlso ModernTooltipService.GetFollowMouse(frameworkElement5) Then
                    ModernTooltipService.lastMousePoint = Mouse.GetPosition(frameworkElement5)
                    ModernTooltipService.UpdatePosition(frameworkElement5, ModernTooltipService.lastMousePoint)
                End If
            End If
        End Sub

        Private Sub OnMouseLeave(sender As Object, e As MouseEventArgs)
            Dim frameworkElement As FrameworkElement = TryCast(sender, FrameworkElement)
            If frameworkElement Is Nothing OrElse frameworkElement IsNot ModernTooltipService.currentOwner Then
                Return
            End If
            If Not frameworkElement.IsEnabled AndAlso ToolTipService.GetShowOnDisabled(frameworkElement) AndAlso ModernTooltipService.IsPointInside(frameworkElement, Mouse.GetPosition(frameworkElement)) Then
                Return
            End If
            Dim frameworkElement2 As FrameworkElement = ModernTooltipService.FindCurrentTooltipOwner(frameworkElement)
            If frameworkElement2 IsNot Nothing AndAlso frameworkElement2 IsNot frameworkElement Then
                ModernTooltipService.BeginShow(frameworkElement2, Mouse.GetPosition(frameworkElement2))
                Return
            End If
            ModernTooltipService.Close(True)
        End Sub

        Private Sub OnPreviewMouseUp(sender As Object, e As MouseButtonEventArgs)
            Dim reference As FrameworkElement = TryCast(sender, FrameworkElement)
            If reference Is Nothing Then
                Return
            End If
            reference.Dispatcher.BeginInvoke(New Action(Sub()
                ModernTooltipService.RefreshCurrentOwner(reference)
            End Sub), DispatcherPriority.Input, Array.Empty(Of Object)())
        End Sub

        Private Sub OnToolTipOpening(sender As Object, e As ToolTipEventArgs)
            Dim frameworkElement As FrameworkElement = TryCast(sender, FrameworkElement)
            Dim obj As Object
            Dim toolTip As ToolTip
            If frameworkElement Is Nothing OrElse Not ModernTooltipService.CanShowModernTooltip(frameworkElement) OrElse Not ModernTooltipService.TryGetToolTip(frameworkElement, obj, toolTip) Then
                Return
            End If
            e.Handled = True
            If ModernTooltipService.ShouldSuppressTooltip(frameworkElement) Then
                ModernTooltipService.Close(False)
                Return
            End If
            If frameworkElement.IsEnabled Then
                Return
            End If
            If ModernTooltipService.currentOwner IsNot frameworkElement Then
                ModernTooltipService.HideImmediately()
            End If
            ModernTooltipService.currentOwner = frameworkElement
            ModernTooltipService.openTimer.[Stop]()
            ModernTooltipService.lastMousePoint = Mouse.GetPosition(frameworkElement)
            ModernTooltipService.Show(frameworkElement, ModernTooltipService.lastMousePoint)
        End Sub

        Private Sub RefreshCurrentOwner(reference As FrameworkElement)
            Dim frameworkElement As FrameworkElement = ModernTooltipService.FindCurrentTooltipOwner(reference)
            Dim frameworkElement2 As FrameworkElement = ModernTooltipService.currentOwner
            If frameworkElement2 IsNot Nothing AndAlso Not frameworkElement2.IsEnabled AndAlso ToolTipService.GetShowOnDisabled(frameworkElement2) AndAlso ModernTooltipService.IsPointInside(frameworkElement2, Mouse.GetPosition(frameworkElement2)) Then
                Return
            End If
            If ModernTooltipService.ShouldSuppressTooltip(frameworkElement) Then
                If ModernTooltipService.currentOwner IsNot Nothing Then
                    Dim dependencyObject As DependencyObject = TryCast(Mouse.Captured, DependencyObject)
                    If dependencyObject IsNot Nothing AndAlso ModernTooltipService.IsSameTreeBranch(dependencyObject, ModernTooltipService.currentOwner) AndAlso ModernTooltipService.IsPointInside(ModernTooltipService.currentOwner, Mouse.GetPosition(ModernTooltipService.currentOwner)) Then
                        Return
                    End If
                End If
                ModernTooltipService.Close(False)
                Return
            End If
            If frameworkElement IsNot Nothing Then
                ModernTooltipService.BeginShow(frameworkElement, Mouse.GetPosition(frameworkElement))
                Return
            End If
            If ModernTooltipService.currentOwner IsNot Nothing Then
                ModernTooltipService.Close(True)
            End If
        End Sub

        Private Function FindCurrentTooltipOwner(reference As FrameworkElement) As <Nullable(2)> FrameworkElement
            Dim frameworkElement As FrameworkElement = ModernTooltipService.<FindCurrentTooltipOwner>g__FindOwnerInAncestors|34_0(TryCast(Mouse.DirectlyOver, DependencyObject))
            If frameworkElement IsNot Nothing Then
                Return frameworkElement
            End If
            Dim presentationSource As PresentationSource = PresentationSource.FromVisual(reference)
            Dim uielement As UIElement = TryCast(If((presentationSource IsNot Nothing), presentationSource.RootVisual, Nothing), UIElement)
            If uielement IsNot Nothing Then
                Dim dependencyObject As DependencyObject = TryCast(uielement.InputHitTest(Mouse.GetPosition(uielement)), DependencyObject)
                frameworkElement = ModernTooltipService.<FindCurrentTooltipOwner>g__FindOwnerInAncestors|34_0(dependencyObject)
                If frameworkElement IsNot Nothing Then
                    Return frameworkElement
                End If
                If dependencyObject IsNot Nothing AndAlso Not ModernTooltipService.IsSameTreeBranch(dependencyObject, reference) Then
                    Return Nothing
                End If
            End If
            Dim dependencyObject2 As DependencyObject = TryCast(reference.InputHitTest(Mouse.GetPosition(reference)), DependencyObject)
            If dependencyObject2 Is Nothing Then
                Return Nothing
            End If
            Return ModernTooltipService.<FindCurrentTooltipOwner>g__FindOwnerInAncestors|34_0(dependencyObject2)
        End Function

        Private Function CanShowModernTooltip(owner As FrameworkElement) As Boolean
            Return ModernTooltipService.GetUseModernTooltip(owner) AndAlso ToolTipService.GetIsEnabled(owner) AndAlso (owner.IsEnabled OrElse ToolTipService.GetShowOnDisabled(owner))
        End Function

        <NullableContext(2)>
        Private Function TryGetToolTip(<Nullable(1)> owner As FrameworkElement, <System.Runtime.InteropServices.OutAttribute()> ByRef content As Object, <System.Runtime.InteropServices.OutAttribute()> ByRef sourceToolTip As ToolTip) As Boolean
            Dim toolTip As Object = owner.ToolTip
            sourceToolTip = TryCast(toolTip, ToolTip)
            content = If((sourceToolTip Is Nothing), toolTip, (If(sourceToolTip.Content, If((sourceToolTip Is ModernTooltipService.borrowedToolTip), ModernTooltipService.borrowedContent, Nothing))))
            If content IsNot Nothing Then
                Dim text As String = TryCast(content, String)
                Return text Is Nothing OrElse text.Length > 0
            End If
            Return False
        End Function

        <NullableContext(2)>
        Private Function ShouldSuppressTooltip(owner As FrameworkElement) As Boolean
            If Mouse.LeftButton = MouseButtonState.Pressed OrElse Mouse.Captured Is Nothing Then
                Return False
            End If
            Dim dependencyObject As DependencyObject = TryCast(Mouse.Captured, DependencyObject)
            Return dependencyObject Is Nothing OrElse owner Is Nothing OrElse Not ModernTooltipService.IsSameTreeBranch(dependencyObject, owner)
        End Function

        Private Function IsPointInside(owner As FrameworkElement, point As Point) As Boolean
            Return point.X >= 0.0 AndAlso point.Y >= 0.0 AndAlso point.X <= owner.ActualWidth AndAlso point.Y <= owner.ActualHeight
        End Function

        Private Function IsSameTreeBranch(first As DependencyObject, second As DependencyObject) As Boolean
            If first Is second Then
                Return True
            End If
            Dim dependencyObject As DependencyObject = ModernTooltipService.GetParent(second)
            While dependencyObject IsNot Nothing
                If dependencyObject Is first Then
                    Return True
                End If
                dependencyObject = ModernTooltipService.GetParent(dependencyObject)
            End While
            Dim dependencyObject2 As DependencyObject = ModernTooltipService.GetParent(first)
            While dependencyObject2 IsNot Nothing
                If dependencyObject2 Is second Then
                    Return True
                End If
                dependencyObject2 = ModernTooltipService.GetParent(dependencyObject2)
            End While
            Return False
        End Function

        Private Function GetParent(current As DependencyObject) As <Nullable(2)> DependencyObject
            Dim flag As Boolean = TypeOf current Is Visual OrElse TypeOf current Is Visual3D
            If flag Then
                Dim parent As DependencyObject = VisualTreeHelper.GetParent(current)
                If parent IsNot Nothing Then
                    Return parent
                End If
            End If
            Return LogicalTreeHelper.GetParent(current)
        End Function

        Private Sub BeginShow(owner As FrameworkElement, point As Point)
            Dim obj As Object
            Dim toolTip As ToolTip
            If Not ModernTooltipService.CanShowModernTooltip(owner) OrElse Not ModernTooltipService.TryGetToolTip(owner, obj, toolTip) Then
                Return
            End If
            If ModernTooltipService.ShouldSuppressTooltip(owner) Then
                ModernTooltipService.Close(False)
                Return
            End If
            Dim comboBox As ComboBox = TryCast(owner, ComboBox)
            If comboBox IsNot Nothing Then
                ModernTooltipService.HookComboBoxDropDown(comboBox)
            End If
            If ModernTooltipService.currentOwner Is owner Then
                ModernTooltipService.lastMousePoint = point
                Dim popup As Popup = ModernTooltipService.popup
                If(popup Is Nothing OrElse Not popup.IsOpen) AndAlso Not ModernTooltipService.openTimer.IsEnabled Then
                    ModernTooltipService.StartOpenTimer(owner)
                End If
                Return
            End If
            ModernTooltipService.HideImmediately()
            ModernTooltipService.currentOwner = owner
            ModernTooltipService.lastMousePoint = point
            ModernTooltipService.StartOpenTimer(owner)
        End Sub

        Private Sub StartOpenTimer(owner As FrameworkElement)
            ModernTooltipService.openTimer.[Stop]()
            Dim num As Integer = Math.Max(0, ToolTipService.GetInitialShowDelay(owner))
            If num = 0 Then
                ModernTooltipService.Show(owner, ModernTooltipService.lastMousePoint)
                Return
            End If
            ModernTooltipService.openTimer.Interval = TimeSpan.FromMilliseconds(CDbl(num))
            ModernTooltipService.openTimer.Start()
        End Sub

        Private Sub Show(owner As FrameworkElement, point As Point)
            Dim obj As Object
            Dim toolTip As ToolTip
            If owner IsNot ModernTooltipService.currentOwner OrElse Not ModernTooltipService.CanShowModernTooltip(owner) OrElse Not ModernTooltipService.TryGetToolTip(owner, obj, toolTip) Then
                Return
            End If
            If owner.IsEnabled AndAlso owner IsNot ModernTooltipService.FindCurrentTooltipOwner(owner) Then
                ModernTooltipService.Close(False)
                Return
            End If
            If ModernTooltipService.popup Is Nothing Then
                ModernTooltipService.popupScale = New ScaleTransform(0.97, 0.97)
                ModernTooltipService.popupCard = New Border() With { .Background = ModernTooltipService.backgroundBrush, .BorderBrush = ModernTooltipService.borderBrush, .BorderThickness = New Thickness(1.0), .CornerRadius = New CornerRadius(8.0), .MaxWidth = 700.0, .SnapsToDevicePixels = True, .UseLayoutRounding = True, .RenderTransform = ModernTooltipService.popupScale, .RenderTransformOrigin = New Point(0.0, 0.0), .Effect = New DropShadowEffect() With { .Opacity = 0.15, .BlurRadius = 18.0, .ShadowDepth = 0.0, .Color = Colors.Black } }
                Dim grid As Grid = New Grid() With { .Margin = New Thickness(19.0), .SnapsToDevicePixels = True, .UseLayoutRounding = True }
                grid.Children.Add(ModernTooltipService.popupCard)
                ModernTooltipService.popup = New Popup() With { .AllowsTransparency = True, .IsHitTestVisible = False, .StaysOpen = True, .PopupAnimation = PopupAnimation.None, .Placement = PlacementMode.Relative, .Child = grid }
            End If
            ModernTooltipService.popup.PlacementTarget = owner
            ModernTooltipService.UpdatePosition(owner, point)
            ModernTooltipService.ObserveToolTip(owner)
            ModernTooltipService.UpdateContent(owner, obj, toolTip)
            ModernTooltipService.animationToken += 1
            ModernTooltipService.popupCard.Opacity = 0.0
            ModernTooltipService.popupScale.ScaleX = 0.97
            ModernTooltipService.popupScale.ScaleY = 0.97
            ModernTooltipService.popup.IsOpen = True
            ModernTooltipService.Animate(1.0, 1.0, Nothing)
        End Sub

        Private Sub UpdateContent(owner As FrameworkElement, content As Object, <Nullable(2)> sourceToolTip As ToolTip)
            ModernTooltipService.popupCard.DataContext = If(If((sourceToolTip IsNot Nothing), sourceToolTip.DataContext, Nothing), owner.DataContext)
            ModernTooltipService.popupCard.FlowDirection = owner.FlowDirection
            ModernTooltipService.popupCard.Child = Nothing
            ModernTooltipService.RestoreBorrowedContent()
            If sourceToolTip IsNot Nothing Then
                Dim dependencyObject As DependencyObject = TryCast(content, DependencyObject)
                If dependencyObject IsNot Nothing AndAlso LogicalTreeHelper.GetParent(dependencyObject) Is sourceToolTip Then
                    ModernTooltipService.borrowedToolTip = sourceToolTip
                    ModernTooltipService.borrowedContent = content
                    sourceToolTip.Content = Nothing
                End If
            End If
            Dim flag As Boolean = If((sourceToolTip IsNot Nothing), sourceToolTip.ContentTemplate, Nothing) IsNot Nothing OrElse If((sourceToolTip IsNot Nothing), sourceToolTip.ContentTemplateSelector, Nothing) IsNot Nothing
            Dim decorator As Decorator = ModernTooltipService.popupCard
            Dim text As String = TryCast(content, String)
            Dim uielement As UIElement
            If text Is Nothing OrElse flag Then
                Dim contentPresenter As ContentPresenter = New ContentPresenter()
                contentPresenter.Content = content
                contentPresenter.ContentTemplate = If((sourceToolTip IsNot Nothing), sourceToolTip.ContentTemplate, Nothing)
                contentPresenter.ContentTemplateSelector = If((sourceToolTip IsNot Nothing), sourceToolTip.ContentTemplateSelector, Nothing)
                contentPresenter.ContentStringFormat = If((sourceToolTip IsNot Nothing), sourceToolTip.ContentStringFormat, Nothing)
                contentPresenter.Margin = ModernTooltipService.contentMargin
                uielement = contentPresenter
                contentPresenter.MaxWidth = 676.0
            Else
                Dim textBlock As TextBlock = New TextBlock()
                textBlock.Text = text
                textBlock.TextWrapping = TextWrapping.Wrap
                textBlock.Foreground = ModernTooltipService.foregroundBrush
                textBlock.Margin = ModernTooltipService.contentMargin
                textBlock.FontSize = 12.5
                textBlock.LineHeight = 17.0
                uielement = textBlock
                textBlock.MaxWidth = 676.0
            End If
            decorator.Child = uielement
        End Sub

        Private Sub ObserveToolTip(owner As FrameworkElement)
            If ModernTooltipService.observedToolTipOwner Is owner Then
                Return
            End If
            ModernTooltipService.StopObservingToolTip()
            ModernTooltipService.observedToolTipOwner = owner
            Dim propertyDescriptor As PropertyDescriptor = ModernTooltipService.toolTipPropertyDescriptor
            Dim <0>__OnToolTipChanged As EventHandler = ModernTooltipService.<>O.<0>__OnToolTipChanged
            Dim eventHandler As EventHandler = <0>__OnToolTipChanged
            If <0>__OnToolTipChanged Is Nothing Then
                Dim eventHandler2 As EventHandler = AddressOf ModernTooltipService.OnToolTipChanged
                eventHandler = eventHandler2
                ModernTooltipService.<>O.<0>__OnToolTipChanged = eventHandler2
            End If
            propertyDescriptor.AddValueChanged(owner, eventHandler)
        End Sub

        Private Sub StopObservingToolTip()
            If ModernTooltipService.observedToolTipOwner Is Nothing Then
                Return
            End If
            Dim propertyDescriptor As PropertyDescriptor = ModernTooltipService.toolTipPropertyDescriptor
            Dim obj As Object = ModernTooltipService.observedToolTipOwner
            Dim <0>__OnToolTipChanged As EventHandler = ModernTooltipService.<>O.<0>__OnToolTipChanged
            Dim eventHandler As EventHandler = <0>__OnToolTipChanged
            If <0>__OnToolTipChanged Is Nothing Then
                Dim eventHandler2 As EventHandler = AddressOf ModernTooltipService.OnToolTipChanged
                eventHandler = eventHandler2
                ModernTooltipService.<>O.<0>__OnToolTipChanged = eventHandler2
            End If
            propertyDescriptor.RemoveValueChanged(obj, eventHandler)
            ModernTooltipService.observedToolTipOwner = Nothing
        End Sub

        Private Sub OnToolTipChanged(<Nullable(2)> sender As Object, e As EventArgs)
            Dim frameworkElement As FrameworkElement = TryCast(sender, FrameworkElement)
            If frameworkElement IsNot Nothing AndAlso frameworkElement Is ModernTooltipService.currentOwner Then
                Dim popup As Popup = ModernTooltipService.popup
                If popup IsNot Nothing AndAlso popup.IsOpen Then
                    Dim obj As Object
                    Dim toolTip As ToolTip
                    If Not ModernTooltipService.CanShowModernTooltip(frameworkElement) OrElse Not ModernTooltipService.TryGetToolTip(frameworkElement, obj, toolTip) Then
                        ModernTooltipService.Close(True)
                        Return
                    End If
                    ModernTooltipService.UpdateContent(frameworkElement, obj, toolTip)
                    Return
                End If
            End If
        End Sub

        Private Sub Close(animated As Boolean)
            Dim CS$<>8__locals1 As ModernTooltipService.<>c__DisplayClass48_0 = New ModernTooltipService.<>c__DisplayClass48_0()
            ModernTooltipService.openTimer.[Stop]()
            ModernTooltipService.currentOwner = Nothing
            ModernTooltipService.StopObservingToolTip()
            Dim popup As Popup = ModernTooltipService.popup
            If popup Is Nothing OrElse Not popup.IsOpen OrElse ModernTooltipService.popupCard Is Nothing OrElse Not animated Then
                ModernTooltipService.HideImmediately()
                Return
            End If
            Dim CS$<>8__locals2 As ModernTooltipService.<>c__DisplayClass48_0 = CS$<>8__locals1
            Dim num As Integer = ModernTooltipService.animationToken + 1
            ModernTooltipService.animationToken = num
            CS$<>8__locals2.token = num
            ModernTooltipService.Animate(0.0, 0.97, Sub(_ As Object, _ As EventArgs)
                If CS$<>8__locals1.token <> ModernTooltipService.animationToken Then
                    Return
                End If
                ModernTooltipService.HideImmediately()
            End Sub)
        End Sub

        Private Sub HideImmediately()
            ModernTooltipService.openTimer.[Stop]()
            ModernTooltipService.animationToken += 1
            ModernTooltipService.StopObservingToolTip()
            If ModernTooltipService.popup IsNot Nothing Then
                ModernTooltipService.popup.IsOpen = False
            End If
            If ModernTooltipService.popupCard IsNot Nothing Then
                ModernTooltipService.popupCard.BeginAnimation(UIElement.OpacityProperty, Nothing)
                ModernTooltipService.popupCard.Child = Nothing
            End If
            If ModernTooltipService.popupScale IsNot Nothing Then
                ModernTooltipService.popupScale.BeginAnimation(ScaleTransform.ScaleXProperty, Nothing)
                ModernTooltipService.popupScale.BeginAnimation(ScaleTransform.ScaleYProperty, Nothing)
            End If
            ModernTooltipService.RestoreBorrowedContent()
        End Sub

        Private Sub RestoreBorrowedContent()
            If ModernTooltipService.borrowedToolTip Is Nothing Then
                Return
            End If
            ModernTooltipService.borrowedToolTip.Content = ModernTooltipService.borrowedContent
            ModernTooltipService.borrowedToolTip = Nothing
            ModernTooltipService.borrowedContent = Nothing
        End Sub

        Private Sub UpdatePosition(owner As FrameworkElement, point As Point)
            ModernTooltipService.popup.PlacementTarget = owner
            ModernTooltipService.popup.HorizontalOffset = Math.Round(point.X + 15.0)
            ModernTooltipService.popup.VerticalOffset = Math.Round(point.Y + 25.0)
        End Sub

        <NullableContext(2)>
        Private Sub Animate(opacity As Double, scale As Double, completed As EventHandler)
            Dim duration As Duration = New Duration(TimeSpan.FromMilliseconds(70.0))
            Dim doubleAnimation As DoubleAnimation = New DoubleAnimation(opacity, duration) With { .EasingFunction = New QuadraticEase() With { .EasingMode = EasingMode.EaseOut } }
            If completed IsNot Nothing Then
                AddHandler doubleAnimation.Completed, completed
            End If
            Dim doubleAnimation2 As DoubleAnimation = New DoubleAnimation(scale, duration) With { .EasingFunction = New QuadraticEase() With { .EasingMode = EasingMode.EaseOut } }
            ModernTooltipService.popupCard.BeginAnimation(UIElement.OpacityProperty, doubleAnimation, HandoffBehavior.SnapshotAndReplace)
            ModernTooltipService.popupScale.BeginAnimation(ScaleTransform.ScaleXProperty, doubleAnimation2, HandoffBehavior.SnapshotAndReplace)
            ModernTooltipService.popupScale.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation2, HandoffBehavior.SnapshotAndReplace)
        End Sub

        <NullableContext(2)>
        Private Sub HookComboBoxDropDown(comboBox As ComboBox)
            If comboBox Is Nothing OrElse CBool(comboBox.GetValue(ModernTooltipService.IsComboBoxDropDownHookedProperty)) Then
                Return
            End If
            comboBox.SetValue(ModernTooltipService.IsComboBoxDropDownHookedProperty, True)
            Dim <>9__53_ As EventHandler = ModernTooltipService.<>c.<>9__53_0
            Dim eventHandler As EventHandler = <>9__53_
            If <>9__53_ Is Nothing Then
                Dim eventHandler2 As EventHandler = Sub(_ As Object, _ As EventArgs)
                    If ModernTooltipService.currentOwner IsNot Nothing Then
                        ModernTooltipService.Close(True)
                    End If
                End Sub
                eventHandler = eventHandler2
                ModernTooltipService.<>c.<>9__53_0 = eventHandler2
            End If
            AddHandler comboBox.DropDownOpened, eventHandler
        End Sub

        <NullableContext(2)>
        <CompilerGenerated()>
        Friend Function <FindCurrentTooltipOwner>g__FindOwnerInAncestors|34_0(start As DependencyObject) As FrameworkElement
            Dim dependencyObject As DependencyObject = start
            While dependencyObject IsNot Nothing
                Dim frameworkElement As FrameworkElement = TryCast(dependencyObject, FrameworkElement)
                Dim obj As Object
                Dim toolTip As ToolTip
                If frameworkElement IsNot Nothing AndAlso ModernTooltipService.CanShowModernTooltip(frameworkElement) AndAlso ModernTooltipService.TryGetToolTip(frameworkElement, obj, toolTip) Then
                    Return frameworkElement
                End If
                dependencyObject = ModernTooltipService.GetParent(dependencyObject)
            End While
            Return Nothing
        End Function

        Public UseModernTooltipProperty As DependencyProperty = DependencyProperty.RegisterAttached("UseModernTooltip", GetType(Boolean), GetType(ModernTooltipService), New PropertyMetadata(True))

        Public FollowMouseProperty As DependencyProperty = DependencyProperty.RegisterAttached("FollowMouse", GetType(Boolean), GetType(ModernTooltipService), New PropertyMetadata(True))

        Private IsComboBoxDropDownHookedProperty As DependencyProperty = DependencyProperty.RegisterAttached("IsComboBoxDropDownHooked", GetType(Boolean), GetType(ModernTooltipService), New PropertyMetadata(False))

        Private Const closedScale As Double = 0.97

        Private Const shadowRadius As Integer = 18

        Private Const contentMaxWidth As Integer = 676

        Private contentMargin As Thickness = New Thickness(12.0, 11.0, 12.0, 8.0)

        Private backgroundBrush As Brush = New SolidColorBrush(Colors.White)

        Private borderBrush As Brush = New SolidColorBrush(Color.FromRgb(214, 214, 214))

        Private foregroundBrush As Brush = New SolidColorBrush(Color.FromRgb(82, 82, 82))

        Private toolTipPropertyDescriptor As DependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.ToolTipProperty, GetType(FrameworkElement))

        Private openTimer As DispatcherTimer = New DispatcherTimer()

        Private initialized As Boolean

        Private animationToken As Integer

        Private lastMousePoint As Point

        <Nullable(2)>
        Private popup As Popup

        <Nullable(2)>
        Private popupCard As Border

        <Nullable(2)>
        Private popupScale As ScaleTransform

        <Nullable(2)>
        Private currentOwner As FrameworkElement

        <Nullable(2)>
        Private observedToolTipOwner As FrameworkElement

        <Nullable(2)>
        Private borrowedToolTip As ToolTip

        <Nullable(2)>
        Private borrowedContent As Object

        <CompilerGenerated()>
        Private NotInheritable Class <>O
            <Nullable(0)>
            Public Shared <0>__OnToolTipChanged As EventHandler
        End Class
    End Module
End Namespace
