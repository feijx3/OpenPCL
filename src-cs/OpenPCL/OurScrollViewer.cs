using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class OurScrollViewer : ScrollViewer
    {
        public OurScrollViewer()
        {
            base.PreviewMouseWheel += this.OurScrollViewer_PreviewMouseWheel;
            base.ScrollChanged += this.OurScrollViewer_ScrollChanged;
            base.IsVisibleChanged += this.OurScrollViewer_IsVisibleChanged;
            base.Loaded += this._Lambda___R0_1;
            base.PreviewGotKeyboardFocus += this.OurScrollViewer_PreviewGotKeyboardFocus;
            this.DeltaMult = 1.0;
        }

        public double DeltaMult
        {
            [CompilerGenerated]
            get
            {
                return this._DeltaMult;
            }
            [CompilerGenerated]
            set
            {
                this._DeltaMult = value;
            }
        }

        private void OurScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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
            ModAnimation.AniStart(ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___R7_2), Delta * this.DeltaMult, 300, 0, new ModAnimation.AniEaseOutFluent((ModAnimation.AniEasePower)6), false), "", false);
        }

        private void OurScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.RealOffset = base.VerticalOffset;
            if (ModMain.FrmMain != null && (e.VerticalChange != 0.0 || e.ViewportHeightChange != 0.0))
            {
                ModMain.FrmMain.BtnExtraBack.ShowRefresh();
            }
        }

        private void OurScrollViewer_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ModMain.FrmMain.BtnExtraBack.ShowRefresh();
        }

        private void Load()
        {
            this.ScrollBar = (OurScrollBar)base.GetTemplateChild("PART_VerticalScrollBar");
        }

        private void OurScrollViewer_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus != null && e.NewFocus is OurSlider)
            {
                e.Handled = true;
            }
        }

        [CompilerGenerated]
        private void _Lambda___R0_1(object a0, RoutedEventArgs a1)
        {
            this.Load();
        }

        [CompilerGenerated]
        private void _Lambda___R7_2(object a0)
        {
            this._Lambda___7_0(Conversions.ToDouble(a0));
        }

        [CompilerGenerated]
        private void _Lambda___7_0(double AnimDelta)
        {
            this.RealOffset = (this.RealOffset + AnimDelta).Clamp(0.0, base.ExtentHeight - base.ActualHeight);
            base.ScrollToVerticalOffset(this.RealOffset);
        }

        [CompilerGenerated]
        private double _DeltaMult;

        private double RealOffset;

        public OurScrollBar ScrollBar;
    }
}
