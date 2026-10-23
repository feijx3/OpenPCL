using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class MyScrollViewer : ScrollViewer
    {
        public MyScrollViewer()
        {
            base.PreviewMouseWheel += this.MyScrollViewer_PreviewMouseWheel;
            base.ScrollChanged += this.MyScrollViewer_ScrollChanged;
            base.IsVisibleChanged += this.MyScrollViewer_IsVisibleChanged;
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.Load();
            };
            base.PreviewGotKeyboardFocus += this.MyScrollViewer_PreviewGotKeyboardFocus;
            this.DeltaMult = 1.0;
        }

        public double DeltaMult { get; set; }

        private void MyScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta != 0 && base.ActualHeight != 0.0 && base.ScrollableHeight != 0.0)
            {
                Type type = e.Source.GetType();
                if (NewLateBinding.LateGet(base.Content, null, "TemplatedParent", new object[0], null, null, null) == null)
                {
                    if ((typeof(ComboBox).IsAssignableFrom(type) && ((ComboBox)e.Source).IsDropDownOpen) || (typeof(TextBox).IsAssignableFrom(type) && ((TextBox)e.Source).AcceptsReturn) || typeof(ComboBoxItem).IsAssignableFrom(type))
                    {
                        return;
                    }
                    if (e.Source is Visual)
                    {
                        PresentationSource presentationSource = PresentationSource.FromVisual((Visual)e.Source);
                        string text;
                        if (presentationSource == null)
                        {
                            text = null;
                        }
                        else
                        {
                            Visual rootVisual = presentationSource.RootVisual;
                            text = ((rootVisual != null) ? rootVisual.GetType().FullName : null);
                        }
                        if (Operators.CompareString(text, "System.Windows.Controls.Primitives.PopupRoot", false) == 0)
                        {
                            return;
                        }
                    }
                }
                e.Handled = true;
                this.PerformVerticalOffsetDelta((double)(checked(0 - e.Delta)));
            }
        }

        public void PerformVerticalOffsetDelta(double Delta)
        {
            ModAnimation.AniStart(ModAnimation.AaDouble(delegate(object a0)
            {
                this._Lambda$__7-0(Conversions.ToDouble(a0));
            }, Delta * this.DeltaMult, 300, 0, new ModAnimation.AniEaseOutFluent((ModAnimation.AniEasePower)6), false), "", false);
        }

        private void MyScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.RealOffset = base.VerticalOffset;
            if (ModMain.FrmMain != null && (e.VerticalChange != 0.0 || e.ViewportHeightChange != 0.0))
            {
                ModMain.FrmMain.BtnExtraBack.ShowRefresh();
            }
        }

        private void MyScrollViewer_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ModMain.FrmMain.BtnExtraBack.ShowRefresh();
        }

        private void Load()
        {
            this.ScrollBar = (MyScrollBar)base.GetTemplateChild("PART_VerticalScrollBar");
        }

        private void MyScrollViewer_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus != null && e.NewFocus is MySlider)
            {
                e.Handled = true;
            }
        }

        private double RealOffset;

        public MyScrollBar ScrollBar;
    }
}
