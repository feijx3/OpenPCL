using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [ContentProperty("Inlines")]
    [DesignerGenerated]
    public partial class OurHint : Border
    {
        public OurHint()
        {
            base.Loaded += this._Lambda___R1_3;
            base.Loaded += this.OurHint_Loaded;
            base.MouseLeftButtonUp += this.OurHint_MouseUp;
            base.MouseLeftButtonDown += this.OurHint_MouseDown;
            base.MouseLeave += this._Lambda___R1_4;
            this.Uuid = ModBase.GetUuid();
            this._ColorType = OurHint.Themes.Red;
            this.RelativeSetup = "";
            this.IsMouseDown = false;
            this.InitializeComponent();
            this.BtnClose_WithEvents = this.BtnClose;
        }

        public bool HasBorder
        {
            get
            {
                return base.BorderThickness.Top > 0.0;
            }
            set
            {
                if (value)
                {
                    base.BorderThickness = new Thickness(3.0, ModBase.GetWPFSize(1.0), ModBase.GetWPFSize(1.0), ModBase.GetWPFSize(1.0));
                    return;
                }
                base.BorderThickness = new Thickness(3.0, 0.0, 0.0, 0.0);
            }
        }

        public OurHint.Themes Theme
        {
            get
            {
                return this._ColorType;
            }
            set
            {
                this._ColorType = value;
                this.UpdateUI();
            }
        }

        private void UpdateUI()
        {
            switch (this.Theme)
            {
            case OurHint.Themes.Blue:
                base.Background = new ModBase.OurColor("#d9ecff");
                base.BorderBrush = new ModBase.OurColor("#1172D4");
                this.LabText.Foreground = new ModBase.OurColor("#0F64B8");
                this.BtnClose.Foreground = new ModBase.OurColor("#0F64B8");
                return;
            case OurHint.Themes.Red:
                base.Background = new ModBase.OurColor("#ffdddf");
                base.BorderBrush = new ModBase.OurColor("#d82929");
                this.LabText.Foreground = new ModBase.OurColor("#bf0b0b");
                this.BtnClose.Foreground = new ModBase.OurColor("#bf0b0b");
                return;
            case OurHint.Themes.Yellow:
                base.Background = new ModBase.OurColor("#ffebd7");
                base.BorderBrush = new ModBase.OurColor("#f57a00");
                this.LabText.Foreground = new ModBase.OurColor("#d86c00");
                this.BtnClose.Foreground = new ModBase.OurColor("#d86c00");
                return;
            default:
                return;
            }
        }

        [Obsolete("IsWarn 已过时。请换用 Theme 属性。")]
        public bool IsWarn
        {
            get
            {
                return this.Theme == OurHint.Themes.Red;
            }
            set
            {
                this.Theme = (value ? OurHint.Themes.Red : OurHint.Themes.Blue);
            }
        }

        public InlineCollection Inlines
        {
            get
            {
                return this.LabText.Inlines;
            }
        }

        public string Text
        {
            get
            {
                return Conversions.ToString(base.GetValue(OurHint.TextProperty));
            }
            set
            {
                base.SetValue(OurHint.TextProperty, value);
            }
        }

        public bool CanClose
        {
            get
            {
                return this.BtnClose.Visibility == Visibility.Visible;
            }
            set
            {
                this.BtnClose.Visibility = (value ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public string RelativeSetup
        {
            [CompilerGenerated]
            get
            {
                return this._RelativeSetup;
            }
            [CompilerGenerated]
            set
            {
                this._RelativeSetup = value;
            }
        }

        private void OurHint_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.CanClose && Settings.Get<bool>(this.RelativeSetup, null))
            {
                base.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Settings.SetSafe(this.RelativeSetup, true, null);
            ModAnimation.AniDispose(this, false, null);
        }

        public event OurHint.ClickEventHandler Click
        {
            [CompilerGenerated]
            add
            {
                OurHint.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurHint.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurHint.ClickEventHandler clickEventHandler3 = (OurHint.ClickEventHandler)Delegate.Combine(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurHint.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurHint.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurHint.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurHint.ClickEventHandler clickEventHandler3 = (OurHint.ClickEventHandler)Delegate.Remove(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurHint.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
        }

        private void OurHint_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseDown)
            {
                this.IsMouseDown = false;
                Logger.Info(string.Format("按下提示条{0}", string.IsNullOrEmpty(base.Name) ? "" : ("：" + base.Name)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurHint.xaml.vb");
                OurHint.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
                if (!e.Handled)
                {
                    e.Handled = true;
                    this.RaiseCustomEvent();
                }
            }
        }

        private void OurHint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.IsMouseDown = true;
        }

        private void OurHint_MouseLeave()
        {
            this.IsMouseDown = false;
        }



        internal virtual OurIconButton BtnClose_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnClose;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this.BtnClose_Click);
                OurIconButton ourIconButton = this._BtnClose;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                this._BtnClose = value;
                ourIconButton = this._BtnClose;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R1_3(object a0, RoutedEventArgs a1)
        {
            this.UpdateUI();
        }

        [CompilerGenerated]
        private void _Lambda___R1_4(object a0, MouseEventArgs a1)
        {
            this.OurHint_MouseLeave();
        }

        public int Uuid;

        private OurHint.Themes _ColorType;

        public static readonly DependencyProperty IsWarnProperty = DependencyProperty.Register("IsWarn", typeof(bool), typeof(OurHint), new PropertyMetadata(true, new PropertyChangedCallback(OurHint._Closure___._I._Lambda___R0_1)));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(OurHint), new PropertyMetadata("", new PropertyChangedCallback(OurHint._Closure___._I._Lambda___R0_2)));

        [CompilerGenerated]
        private string _RelativeSetup;

        [CompilerGenerated]
        private OurHint.ClickEventHandler ClickEvent;

        private bool IsMouseDown;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurHint _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("LabText")]
        private TextBlock _LabText;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnClose")]
        private OurIconButton _BtnClose;

        public enum Themes
        {
            Blue,
            Red,
            Yellow
        }

        public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal void _Lambda___R0_1(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
            {
                ((OurHint._Closure___._I0_0 == null) ? (OurHint._Closure___._I0_0 = new VB_AnonymousDelegate_6<OurHint, DependencyPropertyChangedEventArgs>(OurHint._Closure___._I._Lambda___0_0)) : OurHint._Closure___._I0_0)((OurHint)a0, a1);
            }

            internal void _Lambda___0_0(OurHint d, DependencyPropertyChangedEventArgs e)
            {
                d.Theme = (Conversions.ToBoolean(e.NewValue) ? OurHint.Themes.Red : OurHint.Themes.Blue);
            }

            internal void _Lambda___R0_2(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
            {
                ((OurHint._Closure___._I0_1 == null) ? (OurHint._Closure___._I0_1 = new VB_AnonymousDelegate_6<OurHint, DependencyPropertyChangedEventArgs>(OurHint._Closure___._I._Lambda___0_1)) : OurHint._Closure___._I0_1)((OurHint)a0, a1);
            }

            internal void _Lambda___0_1(OurHint d, DependencyPropertyChangedEventArgs e)
            {
                d.LabText.Text = Conversions.ToString(e.NewValue);
            }

            public static readonly OurHint._Closure___ _I = new OurHint._Closure___();

            public static VB_AnonymousDelegate_6<OurHint, DependencyPropertyChangedEventArgs> _I0_0;

            public static VB_AnonymousDelegate_6<OurHint, DependencyPropertyChangedEventArgs> _I0_1;
        }
    }
}
