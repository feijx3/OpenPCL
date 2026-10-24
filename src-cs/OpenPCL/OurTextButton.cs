using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class OurTextButton : Label
    {
        // Note: this type is marked as 'beforefieldinit'.
        static OurTextButton()
        {
        }

        public event OurTextButton.ClickEventHandler Click
        {
            [CompilerGenerated]
            add
            {
                OurTextButton.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurTextButton.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurTextButton.ClickEventHandler clickEventHandler3 = (OurTextButton.ClickEventHandler)Delegate.Combine(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurTextButton.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurTextButton.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurTextButton.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurTextButton.ClickEventHandler clickEventHandler3 = (OurTextButton.ClickEventHandler)Delegate.Remove(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurTextButton.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
        }

        public OurTextButton()
        {
            base.PreviewMouseLeftButtonDown += this.OurTextButton_MouseLeftButtonDown;
            base.MouseLeave += this._Lambda___R7_2;
            base.PreviewMouseLeftButtonUp += this.OurTextButton_MouseLeftButtonUp;
            base.MouseEnter += this._Lambda___R7_3;
            base.MouseLeave += this._Lambda___R7_4;
            base.IsEnabledChanged += this._Lambda___R7_5;
            base.MouseLeftButtonDown += this._Lambda___R7_6;
            base.MouseLeftButtonUp += this._Lambda___R7_7;
            this.Uuid = ModBase.GetUuid();
            this.IsMouseDown = false;
            base.SetResourceReference(Control.ForegroundProperty, "ColorBrush1");
            base.Background = ModSecret.ColorSemiTransparent;
        }

        public string Text
        {
            get
            {
                return Conversions.ToString(base.GetValue(OurTextButton.TextProperty));
            }
            set
            {
                base.SetValue(OurTextButton.TextProperty, value);
            }
        }

        private void OurTextButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsMouseDown = true;
            e.Handled = true;
        }

        private void OurTextButton_MouseLeave()
        {
            this.IsMouseDown = false;
        }

        private void OurTextButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseDown)
            {
                this.IsMouseDown = false;
                Logger.Info(string.Format("按下文本按钮：{0}", this.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurTextButton.vb");
                OurTextButton.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(this, null);
                }
                this.RaiseCustomEvent();
                e.Handled = true;
            }
        }

        private void RefreshColor()
        {
            string text;
            int num;
            if (this.IsMouseDown)
            {
                text = "ColorBrush4";
                num = 30;
            }
            else if (base.IsMouseOver)
            {
                text = "ColorBrush3";
                num = 100;
            }
            else
            {
                text = "ColorBrush1";
                num = 200;
            }
            if (Operators.CompareString(this.ColorName, text, false) != 0)
            {
                this.ColorName = text;
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                {
                    ModAnimation.AniStart(ModAnimation.AaColor(this, Control.ForegroundProperty, text, num, 0, null, false), "OurTextButton Color " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStop("OurTextButton Color " + Conversions.ToString(this.Uuid));
                base.SetResourceReference(Control.ForegroundProperty, text);
            }
        }

        [CompilerGenerated]
        private void _Lambda___R7_2(object a0, MouseEventArgs a1)
        {
            this.OurTextButton_MouseLeave();
        }

        [CompilerGenerated]
        private void _Lambda___R7_3(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R7_4(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R7_5(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R7_6(object a0, MouseButtonEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R7_7(object a0, MouseButtonEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private OurTextButton.ClickEventHandler ClickEvent;

        public int Uuid;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(OurTextButton), new PropertyMetadata("", new PropertyChangedCallback(OurTextButton._Closure___._I._Lambda___R0_1)));

        public bool IsMouseDown;

        private const int AnimationTimeIn = 100;

        private const int AnimationTimeOut = 200;

        private string ColorName;

        public delegate void ClickEventHandler(object sender, EventArgs e);

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            public _Closure___()
            {
            }

            static _Closure___()
            {
            }

            internal void _Lambda___R0_1(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
            {
                ((OurTextButton._Closure___._I0_0 == null) ? (OurTextButton._Closure___._I0_0 = new VB_AnonymousDelegate_8<OurTextButton, DependencyPropertyChangedEventArgs>(OurTextButton._Closure___._I._Lambda___0_0)) : OurTextButton._Closure___._I0_0)((OurTextButton)a0, a1);
            }

            internal void _Lambda___0_0(OurTextButton sender, DependencyPropertyChangedEventArgs e)
            {
                OurTextButton._Closure___0_0 CS_locals1 = new OurTextButton._Closure___0_0();
                CS_locals1._VB_Local_sender = sender;
                CS_locals1._VB_Local_e = e;
                if (!Operators.ConditionalCompareObjectEqual(CS_locals1._VB_Local_e.OldValue, CS_locals1._VB_Local_e.NewValue, false))
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaOpacity(CS_locals1._VB_Local_sender, -CS_locals1._VB_Local_sender.Opacity, 50, 0, null, false),
                        ModAnimation.AaCode(new ThreadStart(CS_locals1._Lambda___1), 0, true),
                        ModAnimation.AaOpacity(CS_locals1._VB_Local_sender, 1.0, 170, 0, null, false)
                    }, "OurTextButton Text " + Conversions.ToString(CS_locals1._VB_Local_sender.Uuid), false);
                }
            }

            public static readonly OurTextButton._Closure___ _I = new OurTextButton._Closure___();

            public static VB_AnonymousDelegate_8<OurTextButton, DependencyPropertyChangedEventArgs> _I0_0;
        }

        [CompilerGenerated]
        private sealed class _Closure___0_0
        {
            public _Closure___0_0()
            {
            }

            internal void _Lambda___1()
            {
                this._VB_Local_sender.Content = RuntimeHelpers.GetObjectValue(this._VB_Local_e.NewValue);
            }

            public OurTextButton _VB_Local_sender;

            public DependencyPropertyChangedEventArgs _VB_Local_e;
        }
    }
}
