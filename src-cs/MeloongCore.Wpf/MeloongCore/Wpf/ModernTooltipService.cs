using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace MeloongCore.Wpf
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class ModernTooltipService
    {
        public static void SetUseModernTooltip(DependencyObject element, bool value)
        {
            element.SetValue(ModernTooltipService.UseModernTooltipProperty, value);
        }

        public static bool GetUseModernTooltip(DependencyObject element)
        {
            return (bool)element.GetValue(ModernTooltipService.UseModernTooltipProperty);
        }

        public static void SetFollowMouse(DependencyObject element, bool value)
        {
            element.SetValue(ModernTooltipService.FollowMouseProperty, value);
        }

        public static bool GetFollowMouse(DependencyObject element)
        {
            return (bool)element.GetValue(ModernTooltipService.FollowMouseProperty);
        }

        internal static void Init()
        {
            if (ModernTooltipService.initialized)
            {
                return;
            }
            ModernTooltipService.initialized = true;
            EventManager.RegisterClassHandler(typeof(FrameworkElement), UIElement.MouseEnterEvent, new MouseEventHandler(ModernTooltipService.OnMouseEnter), true);
            EventManager.RegisterClassHandler(typeof(FrameworkElement), UIElement.MouseMoveEvent, new MouseEventHandler(ModernTooltipService.OnMouseMove), true);
            EventManager.RegisterClassHandler(typeof(FrameworkElement), UIElement.MouseLeaveEvent, new MouseEventHandler(ModernTooltipService.OnMouseLeave), true);
            EventManager.RegisterClassHandler(typeof(FrameworkElement), UIElement.PreviewMouseUpEvent, new MouseButtonEventHandler(ModernTooltipService.OnPreviewMouseUp), true);
            EventManager.RegisterClassHandler(typeof(FrameworkElement), ToolTipService.ToolTipOpeningEvent, new ToolTipEventHandler(ModernTooltipService.OnToolTipOpening), true);
            EventManager.RegisterClassHandler(typeof(FrameworkElement), FrameworkElement.UnloadedEvent, new RoutedEventHandler(delegate(object sender, RoutedEventArgs _)
            {
                FrameworkElement frameworkElement = sender as FrameworkElement;
                if (frameworkElement != null && frameworkElement == ModernTooltipService.currentOwner)
                {
                    ModernTooltipService.Close(false);
                }
            }), true);
            EventManager.RegisterClassHandler(typeof(ComboBox), FrameworkElement.LoadedEvent, new RoutedEventHandler(delegate(object sender, RoutedEventArgs _)
            {
                ModernTooltipService.HookComboBoxDropDown(sender as ComboBox);
            }), true);
            EventManager.RegisterClassHandler(typeof(ComboBox), UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(delegate(object sender, MouseButtonEventArgs _)
            {
                ModernTooltipService.HookComboBoxDropDown(sender as ComboBox);
            }), true);
        }

        static ModernTooltipService()
        {
            ModernTooltipService.backgroundBrush.Freeze();
            ModernTooltipService.borderBrush.Freeze();
            ModernTooltipService.foregroundBrush.Freeze();
            ModernTooltipService.openTimer.Tick += delegate(object _, EventArgs _)
            {
                ModernTooltipService.openTimer.Stop();
                if (ModernTooltipService.currentOwner != null)
                {
                    ModernTooltipService.Show(ModernTooltipService.currentOwner, ModernTooltipService.lastMousePoint);
                }
            };
        }

        private static void OnMouseEnter(object sender, MouseEventArgs e)
        {
            FrameworkElement reference = sender as FrameworkElement;
            if (reference == null)
            {
                return;
            }
            reference.Dispatcher.BeginInvoke(new Action(delegate
            {
                ModernTooltipService.RefreshCurrentOwner(reference);
            }), DispatcherPriority.Input, Array.Empty<object>());
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null)
            {
                return;
            }
            Popup popup;
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                FrameworkElement frameworkElement2 = ModernTooltipService.currentOwner;
                if (frameworkElement2 != null)
                {
                    ModernTooltipService.lastMousePoint = Mouse.GetPosition(frameworkElement2);
                    if (ModernTooltipService.GetFollowMouse(frameworkElement2))
                    {
                        popup = ModernTooltipService.popup;
                        if (popup != null && popup.IsOpen)
                        {
                            ModernTooltipService.UpdatePosition(frameworkElement2, ModernTooltipService.lastMousePoint);
                            return;
                        }
                    }
                }
                else
                {
                    popup = ModernTooltipService.popup;
                    if (popup != null && popup.IsOpen)
                    {
                        FrameworkElement frameworkElement3 = popup.PlacementTarget as FrameworkElement;
                        if (frameworkElement3 != null && ModernTooltipService.GetFollowMouse(frameworkElement3))
                        {
                            ModernTooltipService.lastMousePoint = Mouse.GetPosition(frameworkElement3);
                            ModernTooltipService.UpdatePosition(frameworkElement3, ModernTooltipService.lastMousePoint);
                        }
                    }
                }
                return;
            }
            ModernTooltipService.RefreshCurrentOwner(frameworkElement);
            FrameworkElement frameworkElement4 = ModernTooltipService.currentOwner;
            if (frameworkElement4 != null)
            {
                ModernTooltipService.lastMousePoint = Mouse.GetPosition(frameworkElement4);
                if (ModernTooltipService.GetFollowMouse(frameworkElement4))
                {
                    popup = ModernTooltipService.popup;
                    if (popup != null && popup.IsOpen)
                    {
                        ModernTooltipService.UpdatePosition(frameworkElement4, ModernTooltipService.lastMousePoint);
                    }
                }
                return;
            }
            popup = ModernTooltipService.popup;
            if (popup != null && popup.IsOpen)
            {
                FrameworkElement frameworkElement5 = popup.PlacementTarget as FrameworkElement;
                if (frameworkElement5 != null && ModernTooltipService.GetFollowMouse(frameworkElement5))
                {
                    ModernTooltipService.lastMousePoint = Mouse.GetPosition(frameworkElement5);
                    ModernTooltipService.UpdatePosition(frameworkElement5, ModernTooltipService.lastMousePoint);
                }
            }
        }

        private static void OnMouseLeave(object sender, MouseEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null || frameworkElement != ModernTooltipService.currentOwner)
            {
                return;
            }
            if (!frameworkElement.IsEnabled && ToolTipService.GetShowOnDisabled(frameworkElement) && ModernTooltipService.IsPointInside(frameworkElement, Mouse.GetPosition(frameworkElement)))
            {
                return;
            }
            FrameworkElement frameworkElement2 = ModernTooltipService.FindCurrentTooltipOwner(frameworkElement);
            if (frameworkElement2 != null && frameworkElement2 != frameworkElement)
            {
                ModernTooltipService.BeginShow(frameworkElement2, Mouse.GetPosition(frameworkElement2));
                return;
            }
            ModernTooltipService.Close(true);
        }

        private static void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement reference = sender as FrameworkElement;
            if (reference == null)
            {
                return;
            }
            reference.Dispatcher.BeginInvoke(new Action(delegate
            {
                ModernTooltipService.RefreshCurrentOwner(reference);
            }), DispatcherPriority.Input, Array.Empty<object>());
        }

        private static void OnToolTipOpening(object sender, ToolTipEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            object obj;
            ToolTip toolTip;
            if (frameworkElement == null || !ModernTooltipService.CanShowModernTooltip(frameworkElement) || !ModernTooltipService.TryGetToolTip(frameworkElement, out obj, out toolTip))
            {
                return;
            }
            e.Handled = true;
            if (ModernTooltipService.ShouldSuppressTooltip(frameworkElement))
            {
                ModernTooltipService.Close(false);
                return;
            }
            if (frameworkElement.IsEnabled)
            {
                return;
            }
            if (ModernTooltipService.currentOwner != frameworkElement)
            {
                ModernTooltipService.HideImmediately();
            }
            ModernTooltipService.currentOwner = frameworkElement;
            ModernTooltipService.openTimer.Stop();
            ModernTooltipService.lastMousePoint = Mouse.GetPosition(frameworkElement);
            ModernTooltipService.Show(frameworkElement, ModernTooltipService.lastMousePoint);
        }

        private static void RefreshCurrentOwner(FrameworkElement reference)
        {
            FrameworkElement frameworkElement = ModernTooltipService.FindCurrentTooltipOwner(reference);
            FrameworkElement frameworkElement2 = ModernTooltipService.currentOwner;
            if (frameworkElement2 != null && !frameworkElement2.IsEnabled && ToolTipService.GetShowOnDisabled(frameworkElement2) && ModernTooltipService.IsPointInside(frameworkElement2, Mouse.GetPosition(frameworkElement2)))
            {
                return;
            }
            if (ModernTooltipService.ShouldSuppressTooltip(frameworkElement))
            {
                if (ModernTooltipService.currentOwner != null)
                {
                    DependencyObject dependencyObject = Mouse.Captured as DependencyObject;
                    if (dependencyObject != null && ModernTooltipService.IsSameTreeBranch(dependencyObject, ModernTooltipService.currentOwner) && ModernTooltipService.IsPointInside(ModernTooltipService.currentOwner, Mouse.GetPosition(ModernTooltipService.currentOwner)))
                    {
                        return;
                    }
                }
                ModernTooltipService.Close(false);
                return;
            }
            if (frameworkElement != null)
            {
                ModernTooltipService.BeginShow(frameworkElement, Mouse.GetPosition(frameworkElement));
                return;
            }
            if (ModernTooltipService.currentOwner != null)
            {
                ModernTooltipService.Close(true);
            }
        }

        [return: Nullable(2)]
        private static FrameworkElement FindCurrentTooltipOwner(FrameworkElement reference)
        {
            FrameworkElement frameworkElement = ModernTooltipService.<FindCurrentTooltipOwner>g__FindOwnerInAncestors|34_0(Mouse.DirectlyOver as DependencyObject);
            if (frameworkElement != null)
            {
                return frameworkElement;
            }
            PresentationSource presentationSource = PresentationSource.FromVisual(reference);
            UIElement uielement = ((presentationSource != null) ? presentationSource.RootVisual : null) as UIElement;
            if (uielement != null)
            {
                DependencyObject dependencyObject = uielement.InputHitTest(Mouse.GetPosition(uielement)) as DependencyObject;
                frameworkElement = ModernTooltipService.<FindCurrentTooltipOwner>g__FindOwnerInAncestors|34_0(dependencyObject);
                if (frameworkElement != null)
                {
                    return frameworkElement;
                }
                if (dependencyObject != null && !ModernTooltipService.IsSameTreeBranch(dependencyObject, reference))
                {
                    return null;
                }
            }
            DependencyObject dependencyObject2 = reference.InputHitTest(Mouse.GetPosition(reference)) as DependencyObject;
            if (dependencyObject2 == null)
            {
                return null;
            }
            return ModernTooltipService.<FindCurrentTooltipOwner>g__FindOwnerInAncestors|34_0(dependencyObject2);
        }

        private static bool CanShowModernTooltip(FrameworkElement owner)
        {
            return ModernTooltipService.GetUseModernTooltip(owner) && ToolTipService.GetIsEnabled(owner) && (owner.IsEnabled || ToolTipService.GetShowOnDisabled(owner));
        }

        [NullableContext(2)]
        private static bool TryGetToolTip([Nullable(1)] FrameworkElement owner, out object content, out ToolTip sourceToolTip)
        {
            object toolTip = owner.ToolTip;
            sourceToolTip = toolTip as ToolTip;
            content = ((sourceToolTip == null) ? toolTip : (sourceToolTip.Content ?? ((sourceToolTip == ModernTooltipService.borrowedToolTip) ? ModernTooltipService.borrowedContent : null)));
            if (content != null)
            {
                string text = content as string;
                return text == null || text.Length > 0;
            }
            return false;
        }

        [NullableContext(2)]
        private static bool ShouldSuppressTooltip(FrameworkElement owner)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed || Mouse.Captured == null)
            {
                return false;
            }
            DependencyObject dependencyObject = Mouse.Captured as DependencyObject;
            return dependencyObject == null || owner == null || !ModernTooltipService.IsSameTreeBranch(dependencyObject, owner);
        }

        private static bool IsPointInside(FrameworkElement owner, Point point)
        {
            return point.X >= 0.0 && point.Y >= 0.0 && point.X <= owner.ActualWidth && point.Y <= owner.ActualHeight;
        }

        private static bool IsSameTreeBranch(DependencyObject first, DependencyObject second)
        {
            if (first == second)
            {
                return true;
            }
            for (DependencyObject dependencyObject = ModernTooltipService.GetParent(second); dependencyObject != null; dependencyObject = ModernTooltipService.GetParent(dependencyObject))
            {
                if (dependencyObject == first)
                {
                    return true;
                }
            }
            for (DependencyObject dependencyObject2 = ModernTooltipService.GetParent(first); dependencyObject2 != null; dependencyObject2 = ModernTooltipService.GetParent(dependencyObject2))
            {
                if (dependencyObject2 == second)
                {
                    return true;
                }
            }
            return false;
        }

        [return: Nullable(2)]
        private static DependencyObject GetParent(DependencyObject current)
        {
            bool flag = current is Visual || current is Visual3D;
            if (flag)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(current);
                if (parent != null)
                {
                    return parent;
                }
            }
            return LogicalTreeHelper.GetParent(current);
        }

        private static void BeginShow(FrameworkElement owner, Point point)
        {
            object obj;
            ToolTip toolTip;
            if (!ModernTooltipService.CanShowModernTooltip(owner) || !ModernTooltipService.TryGetToolTip(owner, out obj, out toolTip))
            {
                return;
            }
            if (ModernTooltipService.ShouldSuppressTooltip(owner))
            {
                ModernTooltipService.Close(false);
                return;
            }
            ComboBox comboBox = owner as ComboBox;
            if (comboBox != null)
            {
                ModernTooltipService.HookComboBoxDropDown(comboBox);
            }
            if (ModernTooltipService.currentOwner == owner)
            {
                ModernTooltipService.lastMousePoint = point;
                Popup popup = ModernTooltipService.popup;
                if ((popup == null || !popup.IsOpen) && !ModernTooltipService.openTimer.IsEnabled)
                {
                    ModernTooltipService.StartOpenTimer(owner);
                }
                return;
            }
            ModernTooltipService.HideImmediately();
            ModernTooltipService.currentOwner = owner;
            ModernTooltipService.lastMousePoint = point;
            ModernTooltipService.StartOpenTimer(owner);
        }

        private static void StartOpenTimer(FrameworkElement owner)
        {
            ModernTooltipService.openTimer.Stop();
            int num = Math.Max(0, ToolTipService.GetInitialShowDelay(owner));
            if (num == 0)
            {
                ModernTooltipService.Show(owner, ModernTooltipService.lastMousePoint);
                return;
            }
            ModernTooltipService.openTimer.Interval = TimeSpan.FromMilliseconds((double)num);
            ModernTooltipService.openTimer.Start();
        }

        private static void Show(FrameworkElement owner, Point point)
        {
            object obj;
            ToolTip toolTip;
            if (owner != ModernTooltipService.currentOwner || !ModernTooltipService.CanShowModernTooltip(owner) || !ModernTooltipService.TryGetToolTip(owner, out obj, out toolTip))
            {
                return;
            }
            if (owner.IsEnabled && owner != ModernTooltipService.FindCurrentTooltipOwner(owner))
            {
                ModernTooltipService.Close(false);
                return;
            }
            if (ModernTooltipService.popup == null)
            {
                ModernTooltipService.popupScale = new ScaleTransform(0.97, 0.97);
                ModernTooltipService.popupCard = new Border
                {
                    Background = ModernTooltipService.backgroundBrush,
                    BorderBrush = ModernTooltipService.borderBrush,
                    BorderThickness = new Thickness(1.0),
                    CornerRadius = new CornerRadius(8.0),
                    MaxWidth = 700.0,
                    SnapsToDevicePixels = true,
                    UseLayoutRounding = true,
                    RenderTransform = ModernTooltipService.popupScale,
                    RenderTransformOrigin = new Point(0.0, 0.0),
                    Effect = new DropShadowEffect
                    {
                        Opacity = 0.15,
                        BlurRadius = 18.0,
                        ShadowDepth = 0.0,
                        Color = Colors.Black
                    }
                };
                Grid grid = new Grid
                {
                    Margin = new Thickness(19.0),
                    SnapsToDevicePixels = true,
                    UseLayoutRounding = true
                };
                grid.Children.Add(ModernTooltipService.popupCard);
                ModernTooltipService.popup = new Popup
                {
                    AllowsTransparency = true,
                    IsHitTestVisible = false,
                    StaysOpen = true,
                    PopupAnimation = PopupAnimation.None,
                    Placement = PlacementMode.Relative,
                    Child = grid
                };
            }
            ModernTooltipService.popup.PlacementTarget = owner;
            ModernTooltipService.UpdatePosition(owner, point);
            ModernTooltipService.ObserveToolTip(owner);
            ModernTooltipService.UpdateContent(owner, obj, toolTip);
            ModernTooltipService.animationToken++;
            ModernTooltipService.popupCard.Opacity = 0.0;
            ModernTooltipService.popupScale.ScaleX = 0.97;
            ModernTooltipService.popupScale.ScaleY = 0.97;
            ModernTooltipService.popup.IsOpen = true;
            ModernTooltipService.Animate(1.0, 1.0, null);
        }

        private static void UpdateContent(FrameworkElement owner, object content, [Nullable(2)] ToolTip sourceToolTip)
        {
            ModernTooltipService.popupCard.DataContext = ((sourceToolTip != null) ? sourceToolTip.DataContext : null) ?? owner.DataContext;
            ModernTooltipService.popupCard.FlowDirection = owner.FlowDirection;
            ModernTooltipService.popupCard.Child = null;
            ModernTooltipService.RestoreBorrowedContent();
            if (sourceToolTip != null)
            {
                DependencyObject dependencyObject = content as DependencyObject;
                if (dependencyObject != null && LogicalTreeHelper.GetParent(dependencyObject) == sourceToolTip)
                {
                    ModernTooltipService.borrowedToolTip = sourceToolTip;
                    ModernTooltipService.borrowedContent = content;
                    sourceToolTip.Content = null;
                }
            }
            bool flag = ((sourceToolTip != null) ? sourceToolTip.ContentTemplate : null) != null || ((sourceToolTip != null) ? sourceToolTip.ContentTemplateSelector : null) != null;
            Decorator decorator = ModernTooltipService.popupCard;
            string text = content as string;
            UIElement uielement;
            if (text == null || flag)
            {
                ContentPresenter contentPresenter = new ContentPresenter();
                contentPresenter.Content = content;
                contentPresenter.ContentTemplate = ((sourceToolTip != null) ? sourceToolTip.ContentTemplate : null);
                contentPresenter.ContentTemplateSelector = ((sourceToolTip != null) ? sourceToolTip.ContentTemplateSelector : null);
                contentPresenter.ContentStringFormat = ((sourceToolTip != null) ? sourceToolTip.ContentStringFormat : null);
                contentPresenter.Margin = ModernTooltipService.contentMargin;
                uielement = contentPresenter;
                contentPresenter.MaxWidth = 676.0;
            }
            else
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = text;
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Foreground = ModernTooltipService.foregroundBrush;
                textBlock.Margin = ModernTooltipService.contentMargin;
                textBlock.FontSize = 12.5;
                textBlock.LineHeight = 17.0;
                uielement = textBlock;
                textBlock.MaxWidth = 676.0;
            }
            decorator.Child = uielement;
        }

        private static void ObserveToolTip(FrameworkElement owner)
        {
            if (ModernTooltipService.observedToolTipOwner == owner)
            {
                return;
            }
            ModernTooltipService.StopObservingToolTip();
            ModernTooltipService.observedToolTipOwner = owner;
            PropertyDescriptor propertyDescriptor = ModernTooltipService.toolTipPropertyDescriptor;
            EventHandler eventHandler;
            if ((eventHandler = ModernTooltipService.<>O.<0>__OnToolTipChanged) == null)
            {
                eventHandler = (ModernTooltipService.<>O.<0>__OnToolTipChanged = new EventHandler(ModernTooltipService.OnToolTipChanged));
            }
            propertyDescriptor.AddValueChanged(owner, eventHandler);
        }

        private static void StopObservingToolTip()
        {
            if (ModernTooltipService.observedToolTipOwner == null)
            {
                return;
            }
            PropertyDescriptor propertyDescriptor = ModernTooltipService.toolTipPropertyDescriptor;
            object obj = ModernTooltipService.observedToolTipOwner;
            EventHandler eventHandler;
            if ((eventHandler = ModernTooltipService.<>O.<0>__OnToolTipChanged) == null)
            {
                eventHandler = (ModernTooltipService.<>O.<0>__OnToolTipChanged = new EventHandler(ModernTooltipService.OnToolTipChanged));
            }
            propertyDescriptor.RemoveValueChanged(obj, eventHandler);
            ModernTooltipService.observedToolTipOwner = null;
        }

        private static void OnToolTipChanged([Nullable(2)] object sender, EventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            if (frameworkElement != null && frameworkElement == ModernTooltipService.currentOwner)
            {
                Popup popup = ModernTooltipService.popup;
                if (popup != null && popup.IsOpen)
                {
                    object obj;
                    ToolTip toolTip;
                    if (!ModernTooltipService.CanShowModernTooltip(frameworkElement) || !ModernTooltipService.TryGetToolTip(frameworkElement, out obj, out toolTip))
                    {
                        ModernTooltipService.Close(true);
                        return;
                    }
                    ModernTooltipService.UpdateContent(frameworkElement, obj, toolTip);
                    return;
                }
            }
        }

        private static void Close(bool animated)
        {
            ModernTooltipService.openTimer.Stop();
            ModernTooltipService.currentOwner = null;
            ModernTooltipService.StopObservingToolTip();
            Popup popup = ModernTooltipService.popup;
            if (popup == null || !popup.IsOpen || ModernTooltipService.popupCard == null || !animated)
            {
                ModernTooltipService.HideImmediately();
                return;
            }
            int token = ++ModernTooltipService.animationToken;
            ModernTooltipService.Animate(0.0, 0.97, delegate(object _, EventArgs _)
            {
                if (token != ModernTooltipService.animationToken)
                {
                    return;
                }
                ModernTooltipService.HideImmediately();
            });
        }

        private static void HideImmediately()
        {
            ModernTooltipService.openTimer.Stop();
            ModernTooltipService.animationToken++;
            ModernTooltipService.StopObservingToolTip();
            if (ModernTooltipService.popup != null)
            {
                ModernTooltipService.popup.IsOpen = false;
            }
            if (ModernTooltipService.popupCard != null)
            {
                ModernTooltipService.popupCard.BeginAnimation(UIElement.OpacityProperty, null);
                ModernTooltipService.popupCard.Child = null;
            }
            if (ModernTooltipService.popupScale != null)
            {
                ModernTooltipService.popupScale.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                ModernTooltipService.popupScale.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            }
            ModernTooltipService.RestoreBorrowedContent();
        }

        private static void RestoreBorrowedContent()
        {
            if (ModernTooltipService.borrowedToolTip == null)
            {
                return;
            }
            ModernTooltipService.borrowedToolTip.Content = ModernTooltipService.borrowedContent;
            ModernTooltipService.borrowedToolTip = null;
            ModernTooltipService.borrowedContent = null;
        }

        private static void UpdatePosition(FrameworkElement owner, Point point)
        {
            ModernTooltipService.popup.PlacementTarget = owner;
            ModernTooltipService.popup.HorizontalOffset = Math.Round(point.X + 15.0);
            ModernTooltipService.popup.VerticalOffset = Math.Round(point.Y + 25.0);
        }

        [NullableContext(2)]
        private static void Animate(double opacity, double scale, EventHandler completed)
        {
            Duration duration = new Duration(TimeSpan.FromMilliseconds(70.0));
            DoubleAnimation doubleAnimation = new DoubleAnimation(opacity, duration)
            {
                EasingFunction = new QuadraticEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            if (completed != null)
            {
                doubleAnimation.Completed += completed;
            }
            DoubleAnimation doubleAnimation2 = new DoubleAnimation(scale, duration)
            {
                EasingFunction = new QuadraticEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            ModernTooltipService.popupCard.BeginAnimation(UIElement.OpacityProperty, doubleAnimation, HandoffBehavior.SnapshotAndReplace);
            ModernTooltipService.popupScale.BeginAnimation(ScaleTransform.ScaleXProperty, doubleAnimation2, HandoffBehavior.SnapshotAndReplace);
            ModernTooltipService.popupScale.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation2, HandoffBehavior.SnapshotAndReplace);
        }

        [NullableContext(2)]
        private static void HookComboBoxDropDown(ComboBox comboBox)
        {
            if (comboBox == null || (bool)comboBox.GetValue(ModernTooltipService.IsComboBoxDropDownHookedProperty))
            {
                return;
            }
            comboBox.SetValue(ModernTooltipService.IsComboBoxDropDownHookedProperty, true);
            comboBox.DropDownOpened += delegate(object _, EventArgs _)
            {
                if (ModernTooltipService.currentOwner != null)
                {
                    ModernTooltipService.Close(true);
                }
            };
        }

        [NullableContext(2)]
        [CompilerGenerated]
        internal static FrameworkElement <FindCurrentTooltipOwner>g__FindOwnerInAncestors|34_0(DependencyObject start)
        {
            for (DependencyObject dependencyObject = start; dependencyObject != null; dependencyObject = ModernTooltipService.GetParent(dependencyObject))
            {
                FrameworkElement frameworkElement = dependencyObject as FrameworkElement;
                object obj;
                ToolTip toolTip;
                if (frameworkElement != null && ModernTooltipService.CanShowModernTooltip(frameworkElement) && ModernTooltipService.TryGetToolTip(frameworkElement, out obj, out toolTip))
                {
                    return frameworkElement;
                }
            }
            return null;
        }

        public static readonly DependencyProperty UseModernTooltipProperty = DependencyProperty.RegisterAttached("UseModernTooltip", typeof(bool), typeof(ModernTooltipService), new PropertyMetadata(true));

        public static readonly DependencyProperty FollowMouseProperty = DependencyProperty.RegisterAttached("FollowMouse", typeof(bool), typeof(ModernTooltipService), new PropertyMetadata(true));

        private static readonly DependencyProperty IsComboBoxDropDownHookedProperty = DependencyProperty.RegisterAttached("IsComboBoxDropDownHooked", typeof(bool), typeof(ModernTooltipService), new PropertyMetadata(false));

        private const double closedScale = 0.97;

        private const int shadowRadius = 18;

        private const int contentMaxWidth = 676;

        private static readonly Thickness contentMargin = new Thickness(12.0, 11.0, 12.0, 8.0);

        private static readonly Brush backgroundBrush = new SolidColorBrush(Colors.White);

        private static readonly Brush borderBrush = new SolidColorBrush(Color.FromRgb(214, 214, 214));

        private static readonly Brush foregroundBrush = new SolidColorBrush(Color.FromRgb(82, 82, 82));

        private static readonly DependencyPropertyDescriptor toolTipPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.ToolTipProperty, typeof(FrameworkElement));

        private static readonly DispatcherTimer openTimer = new DispatcherTimer();

        private static bool initialized;

        private static int animationToken;

        private static Point lastMousePoint;

        [Nullable(2)]
        private static Popup popup;

        [Nullable(2)]
        private static Border popupCard;

        [Nullable(2)]
        private static ScaleTransform popupScale;

        [Nullable(2)]
        private static FrameworkElement currentOwner;

        [Nullable(2)]
        private static FrameworkElement observedToolTipOwner;

        [Nullable(2)]
        private static ToolTip borrowedToolTip;

        [Nullable(2)]
        private static object borrowedContent;

        [CompilerGenerated]
        private static class <>O
        {
            [Nullable(0)]
            public static EventHandler <0>__OnToolTipChanged;
        }
    }
}
