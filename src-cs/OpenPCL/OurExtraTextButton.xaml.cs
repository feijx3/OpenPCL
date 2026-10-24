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
using System.Windows.Media;
using System.Windows.Shapes;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [ContentProperty("Inlines")]
    [DesignerGenerated]
    public partial class OurExtraTextButton : Grid
    {
        public OurExtraTextButton()
        {
            base.Loaded += this._Lambda___R1_1;
            base.IsEnabledChanged += this._Lambda___R1_2;
            this.Uuid = ModBase.GetUuid();
            this._Logo = "";
            this._LogoScale = 1.0;
            this._Show = false;
            this.IsLeftMouseHeld = false;
            this.InitializeComponent();
            this.PanClick_WithEvents = this.PanClick;
        }

        public event OurExtraTextButton.ClickEventHandler Click
        {
            [CompilerGenerated]
            add
            {
                OurExtraTextButton.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurExtraTextButton.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurExtraTextButton.ClickEventHandler clickEventHandler3 = (OurExtraTextButton.ClickEventHandler)Delegate.Combine(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurExtraTextButton.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurExtraTextButton.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurExtraTextButton.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurExtraTextButton.ClickEventHandler clickEventHandler3 = (OurExtraTextButton.ClickEventHandler)Delegate.Remove(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurExtraTextButton.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
        }

        public string Logo
        {
            get
            {
                return this._Logo;
            }
            set
            {
                if (Operators.CompareString(value, this._Logo, false) != 0)
                {
                    this._Logo = value;
                    this.Path.Data = (Geometry)new GeometryConverter().ConvertFromString(value);
                }
            }
        }

        public double LogoScale
        {
            get
            {
                return this._LogoScale;
            }
            set
            {
                this._LogoScale = value;
                if (this.Path != null)
                {
                    this.Path.RenderTransform = new ScaleTransform
                    {
                        ScaleX = this.LogoScale,
                        ScaleY = this.LogoScale
                    };
                }
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
                return Conversions.ToString(base.GetValue(OurExtraTextButton.TextProperty));
            }
            set
            {
                base.SetValue(OurExtraTextButton.TextProperty, value);
            }
        }

        public bool Show
        {
            get
            {
                return this._Show;
            }
            set
            {
                OurExtraTextButton._Closure___25_0 CS_locals1 = new OurExtraTextButton._Closure___25_0();
                CS_locals1._VB_Me = this;
                CS_locals1._VB_Local_value = value;
                if (this._Show != CS_locals1._VB_Local_value)
                {
                    this._Show = CS_locals1._VB_Local_value;
                    ModBase.RunInUi(new Action(CS_locals1._Lambda___0), false);
                }
            }
        }

        private void Button_LeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsLeftMouseHeld)
            {
                Logger.Info(string.Format("按下附加图标按钮：{0}", this.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurExtraTextButton.xaml.vb");
                OurExtraTextButton.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
                e.Handled = true;
                this.RaiseCustomEvent();
                this.Button_LeftMouseUp();
            }
        }

        private void Button_LeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsLeftMouseHeld)
            {
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaScaleTransform(this.PanScale, 0.85 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 800, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false),
                    ModAnimation.AaScaleTransform(this.PanScale, -0.05, 60, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false)
                }, "OurExtraTextButton Scale " + Conversions.ToString(this.Uuid), false);
            }
            this.IsLeftMouseHeld = true;
            base.Focus();
        }

        private void Button_LeftMouseUp()
        {
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanScale, 1.0 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 300, 0, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false) }, "OurExtraTextButton Scale " + Conversions.ToString(this.Uuid), false);
            this.IsLeftMouseHeld = false;
            this.RefreshColor();
        }

        private void Button_RightMouseUp()
        {
            if (!this.IsLeftMouseHeld)
            {
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanScale, 1.0 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 300, 0, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false) }, "OurExtraTextButton Scale " + Conversions.ToString(this.Uuid), false);
            }
            this.RefreshColor();
        }

        private void Button_MouseLeave()
        {
            this.IsLeftMouseHeld = false;
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanScale, 1.0 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 500, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false) }, "OurExtraTextButton Scale " + Conversions.ToString(this.Uuid), false);
            this.RefreshColor();
        }

        public void RefreshColor()
        {
            try
            {
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                {
                    if (!base.IsEnabled)
                    {
                        ModAnimation.AniStart(ModAnimation.AaColor(this.PanColor, Panel.BackgroundProperty, "ColorBrushGray4", 120, 0, null, false), "OurExtraTextButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else if (base.IsMouseOver)
                    {
                        ModAnimation.AniStart(ModAnimation.AaColor(this.PanColor, Panel.BackgroundProperty, "ColorBrush4", 120, 0, null, false), "OurExtraTextButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else
                    {
                        ModAnimation.AniStart(ModAnimation.AaColor(this.PanColor, Panel.BackgroundProperty, "ColorBrush3", 150, 0, null, false), "OurExtraTextButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                }
                else
                {
                    ModAnimation.AniStop("OurExtraTextButton Color " + Conversions.ToString(this.Uuid));
                    if (!base.IsEnabled)
                    {
                        this.PanColor.SetResourceReference(Panel.BackgroundProperty, "ColorBrushGray4");
                    }
                    else if (base.IsMouseOver)
                    {
                        this.PanColor.SetResourceReference(Panel.BackgroundProperty, "ColorBrush4");
                    }
                    else
                    {
                        this.PanColor.SetResourceReference(Panel.BackgroundProperty, "ColorBrush3");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "刷新附加图标按钮颜色出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurExtraTextButton.xaml.vb");
            }
        }


        internal virtual Border PanClick_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._PanClick;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this.Button_LeftMouseUp);
                MouseButtonEventHandler mouseButtonEventHandler2 = new MouseButtonEventHandler(this.Button_LeftMouseDown);
                MouseButtonEventHandler mouseButtonEventHandler3 = new MouseButtonEventHandler(this._Lambda___R41_3);
                MouseButtonEventHandler mouseButtonEventHandler4 = new MouseButtonEventHandler(this._Lambda___R41_4);
                MouseEventHandler mouseEventHandler = new MouseEventHandler(this._Lambda___R41_5);
                MouseEventHandler mouseEventHandler2 = new MouseEventHandler(this._Lambda___R41_6);
                MouseEventHandler mouseEventHandler3 = new MouseEventHandler(this._Lambda___R41_7);
                Border border = this._PanClick;
                if (border != null)
                {
                    border.MouseLeftButtonUp -= mouseButtonEventHandler;
                    border.MouseLeftButtonDown -= mouseButtonEventHandler2;
                    border.MouseLeftButtonUp -= mouseButtonEventHandler3;
                    border.MouseRightButtonUp -= mouseButtonEventHandler4;
                    border.MouseLeave -= mouseEventHandler;
                    border.MouseEnter -= mouseEventHandler2;
                    border.MouseLeave -= mouseEventHandler3;
                }
                this._PanClick = value;
                border = this._PanClick;
                if (border != null)
                {
                    border.MouseLeftButtonUp += mouseButtonEventHandler;
                    border.MouseLeftButtonDown += mouseButtonEventHandler2;
                    border.MouseLeftButtonUp += mouseButtonEventHandler3;
                    border.MouseRightButtonUp += mouseButtonEventHandler4;
                    border.MouseLeave += mouseEventHandler;
                    border.MouseEnter += mouseEventHandler2;
                    border.MouseLeave += mouseEventHandler3;
                }
            }
        }





        [CompilerGenerated]
        private void _Lambda___R1_1(object a0, RoutedEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R1_2(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R41_3(object a0, MouseButtonEventArgs a1)
        {
            this.Button_LeftMouseUp();
        }

        [CompilerGenerated]
        private void _Lambda___R41_4(object a0, MouseButtonEventArgs a1)
        {
            this.Button_RightMouseUp();
        }

        [CompilerGenerated]
        private void _Lambda___R41_5(object a0, MouseEventArgs a1)
        {
            this.Button_MouseLeave();
        }

        [CompilerGenerated]
        private void _Lambda___R41_6(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R41_7(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private OurExtraTextButton.ClickEventHandler ClickEvent;

        public int Uuid;

        private string _Logo;

        private double _LogoScale;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(OurExtraTextButton), new PropertyMetadata(new PropertyChangedCallback(OurExtraTextButton._Closure___._I._Lambda___0_0)));

        private bool _Show;

        private bool IsLeftMouseHeld;

        private const int AnimationColorIn = 120;

        private const int AnimationColorOut = 150;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurExtraTextButton _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanClick")]
        private Border _PanClick;

        [CompilerGenerated]
        [AccessedThroughProperty("PanScale")]
        private Grid _PanScale;

        [CompilerGenerated]
        [AccessedThroughProperty("PanColor")]
        private Border _PanColor;

        [CompilerGenerated]
        [AccessedThroughProperty("Path")]
        private Path _Path;

        [CompilerGenerated]
        [AccessedThroughProperty("LabText")]
        private TextBlock _LabText;

        public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal void _Lambda___0_0(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                if (sender != null)
                {
                    ((OurExtraTextButton)sender).LabText.Text = Conversions.ToString(e.NewValue);
                }
            }

            public static readonly OurExtraTextButton._Closure___ _I = new OurExtraTextButton._Closure___();
        }

        [CompilerGenerated]
        private sealed class _Closure___25_0
        {
            internal void _Lambda___0()
            {
                if (this._VB_Local_value)
                {
                    this._VB_Me.Opacity = 0.0;
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaOpacity(this._VB_Me, 1.0 - this._VB_Me.Opacity, 80, 50, null, false),
                        ModAnimation.AaScaleTransform(this._VB_Me, 0.15 - ((ScaleTransform)this._VB_Me.RenderTransform).ScaleX, 400, 50, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false),
                        ModAnimation.AaScaleTransform(this._VB_Me, 0.85, 160, 50, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false)
                    }, "OurExtraTextButton MainScale " + Conversions.ToString(this._VB_Me.Uuid), false);
                }
                else
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaOpacity(this._VB_Me, -this._VB_Me.Opacity, 50, 50, null, false),
                        ModAnimation.AaScaleTransform(this._VB_Me, -((ScaleTransform)this._VB_Me.RenderTransform).ScaleX, 100, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), false)
                    }, "OurExtraTextButton MainScale " + Conversions.ToString(this._VB_Me.Uuid), false);
                }
                this._VB_Me.IsHitTestVisible = this._VB_Local_value;
            }

            public bool _VB_Local_value;

            public OurExtraTextButton _VB_Me;
        }
    }
}
