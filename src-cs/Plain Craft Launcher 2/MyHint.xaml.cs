using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
    public partial class MyHint : Border
    {
        public MyHint()
        {
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.UpdateUI();
            };
            base.Loaded += this.MyHint_Loaded;
            base.MouseLeftButtonUp += this.MyHint_MouseUp;
            base.MouseLeftButtonDown += this.MyHint_MouseDown;
            base.MouseLeave += delegate(object a0, MouseEventArgs a1)
            {
                this.MyHint_MouseLeave();
            };
            this.Uuid = ModBase.GetUuid();
            this._ColorType = MyHint.Themes.Red;
            this.RelativeSetup = "";
            this.IsMouseDown = false;
            this.InitializeComponent();
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

        public MyHint.Themes Theme
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
            case MyHint.Themes.Blue:
                base.Background = new ModBase.MyColor("#d9ecff");
                base.BorderBrush = new ModBase.MyColor("#1172D4");
                this.LabText.Foreground = new ModBase.MyColor("#0F64B8");
                this.BtnClose.Foreground = new ModBase.MyColor("#0F64B8");
                return;
            case MyHint.Themes.Red:
                base.Background = new ModBase.MyColor("#ffdddf");
                base.BorderBrush = new ModBase.MyColor("#d82929");
                this.LabText.Foreground = new ModBase.MyColor("#bf0b0b");
                this.BtnClose.Foreground = new ModBase.MyColor("#bf0b0b");
                return;
            case MyHint.Themes.Yellow:
                base.Background = new ModBase.MyColor("#ffebd7");
                base.BorderBrush = new ModBase.MyColor("#f57a00");
                this.LabText.Foreground = new ModBase.MyColor("#d86c00");
                this.BtnClose.Foreground = new ModBase.MyColor("#d86c00");
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
                return this.Theme == MyHint.Themes.Red;
            }
            set
            {
                this.Theme = (value ? MyHint.Themes.Red : MyHint.Themes.Blue);
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
                return Conversions.ToString(base.GetValue(MyHint.TextProperty));
            }
            set
            {
                base.SetValue(MyHint.TextProperty, value);
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

        public string RelativeSetup { get; set; }

        private void MyHint_Loaded(object sender, RoutedEventArgs e)
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

        public event MyHint.ClickEventHandler Click;

        private void MyHint_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseDown)
            {
                this.IsMouseDown = false;
                Logger.Info(string.Format("按下提示条{0}", string.IsNullOrEmpty(base.Name) ? "" : ("：" + base.Name)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyHint.xaml.vb");
                MyHint.ClickEventHandler clickEvent = this.ClickEvent;
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

        private void MyHint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.IsMouseDown = true;
        }

        private void MyHint_MouseLeave()
        {
            this.IsMouseDown = false;
        }

        internal virtual MyHint PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabText
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnClose
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
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.BtnClose_Click);
                MyIconButton myIconButton = this._BtnClose;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnClose = value;
                myIconButton = this._BtnClose;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        public int Uuid;

        private MyHint.Themes _ColorType;

        public static readonly DependencyProperty IsWarnProperty = DependencyProperty.Register("IsWarn", typeof(bool), typeof(MyHint), new PropertyMetadata(true, delegate(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
        {
            ((MyHint._Closure$__.$I0-0 == null) ? (MyHint._Closure$__.$I0-0 = delegate(MyHint d, DependencyPropertyChangedEventArgs e)
            {
                d.Theme = (Conversions.ToBoolean(e.NewValue) ? MyHint.Themes.Red : MyHint.Themes.Blue);
            }) : MyHint._Closure$__.$I0-0)((MyHint)a0, a1);
        }));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MyHint), new PropertyMetadata("", delegate(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
        {
            ((MyHint._Closure$__.$I0-1 == null) ? (MyHint._Closure$__.$I0-1 = delegate(MyHint d, DependencyPropertyChangedEventArgs e)
            {
                d.LabText.Text = Conversions.ToString(e.NewValue);
            }) : MyHint._Closure$__.$I0-1)((MyHint)a0, a1);
        }));

        private bool IsMouseDown;

        public enum Themes
        {
            Blue,
            Red,
            Yellow
        }

        public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);
    }
}
