using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using MeloongCore;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class OurIconButton : Border
    {
        public OurIconButton()
        {
            base.MouseLeftButtonUp += this.Button_MouseUp;
            base.MouseLeftButtonDown += this.Button_MouseDown;
            base.MouseLeftButtonUp += this._Lambda___R0_1;
            base.MouseLeave += this._Lambda___R0_2;
            base.MouseEnter += this._Lambda___R0_3;
            base.MouseLeave += this._Lambda___R0_4;
            base.Loaded += this._Lambda___R0_5;
            base.IsEnabledChanged += this._Lambda___R0_6;
            this.Uuid = ModBase.GetUuid();
            this._LogoScale = 1.0;
            this.Theme = OurIconButton.Themes.Color;
            this._Foreground = new SolidColorBrush(Color.FromRgb(128, 128, 128));
            this.IsMouseDown = false;
            this.CanClick = true;
            this.InitializeComponent();
        }

        public event OurIconButton.ClickEventHandler Click
        {
            [CompilerGenerated]
            add
            {
                OurIconButton.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurIconButton.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurIconButton.ClickEventHandler clickEventHandler3 = (OurIconButton.ClickEventHandler)Delegate.Combine(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurIconButton.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurIconButton.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurIconButton.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurIconButton.ClickEventHandler clickEventHandler3 = (OurIconButton.ClickEventHandler)Delegate.Remove(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurIconButton.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
        }

        public string Logo
        {
            get
            {
                return this.Path.Data.ToString();
            }
            set
            {
                this.Path.Data = (Geometry)new GeometryConverter().ConvertFromString(value);
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
                if (!Information.IsNothing(this.Path))
                {
                    this.Path.RenderTransform = new ScaleTransform
                    {
                        ScaleX = this.LogoScale,
                        ScaleY = this.LogoScale
                    };
                }
            }
        }

        public OurIconButton.Themes Theme
        {
            [CompilerGenerated]
            get
            {
                return this._Theme;
            }
            [CompilerGenerated]
            set
            {
                this._Theme = value;
            }
        }

        public SolidColorBrush Foreground
        {
            get
            {
                return this._Foreground;
            }
            set
            {
                this._Foreground = value;
                checked
                {
                    ModAnimation.AniControlEnabled++;
                    this.RefreshAnim();
                    ModAnimation.AniControlEnabled--;
                }
            }
        }

        public bool CanClick
        {
            [CompilerGenerated]
            get
            {
                return this._CanClick;
            }
            [CompilerGenerated]
            set
            {
                this._CanClick = value;
            }
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CanClick && this.IsMouseDown)
            {
                Logger.Info(string.Format("按下图标按钮{0}", string.IsNullOrEmpty(base.Name) ? "" : ("：" + base.Name)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurIconButton.xaml.vb");
                OurIconButton.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
                e.Handled = true;
                this.Button_MouseUp();
                this.RaiseCustomEvent();
            }
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.CanClick)
            {
                this.IsMouseDown = true;
                base.Focus();
                ModAnimation.AniStart(ModAnimation.AaScaleTransform(this.PanBack, 0.8 - ((ScaleTransform)this.PanBack.RenderTransform).ScaleX, 400, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false), "OurIconButton Scale " + Conversions.ToString(this.Uuid), false);
            }
        }

        private void Button_MouseUp()
        {
            if (this.CanClick)
            {
                if (this.IsMouseDown)
                {
                    this.IsMouseDown = false;
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaScaleTransform(this.PanBack, 1.05 - ((ScaleTransform)this.PanBack.RenderTransform).ScaleX, 250, 0, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false),
                        ModAnimation.AaScaleTransform(this.PanBack, -0.05, 250, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false)
                    }, "OurIconButton Scale " + Conversions.ToString(this.Uuid), false);
                }
                this.RefreshAnim();
            }
        }

        private void Button_MouseLeave()
        {
            if (this.CanClick)
            {
                this.IsMouseDown = false;
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanBack, 1.0 - ((ScaleTransform)this.PanBack.RenderTransform).ScaleX, 250, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false) }, "OurIconButton Scale " + Conversions.ToString(this.Uuid), false);
                this.RefreshAnim();
            }
        }

        public void RefreshAnim()
        {
            try
            {
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                {
                    if (this.PanBack.Background == null)
                    {
                        this.PanBack.Background = new ModBase.OurColor(0.0, 255.0, 255.0, 255.0);
                    }
                    if (this.Path.Fill == null)
                    {
                        switch (this.Theme)
                        {
                        case OurIconButton.Themes.Black:
                            this.Path.Fill = new ModBase.OurColor(160.0, 0.0, 0.0, 0.0);
                            break;
                        case OurIconButton.Themes.Red:
                            this.Path.Fill = new ModBase.OurColor(160.0, 255.0, 76.0, 76.0);
                            break;
                        case OurIconButton.Themes.Custom:
                            this.Path.Fill = new ModBase.OurColor(160.0, this.Foreground);
                            break;
                        }
                    }
                    List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
                    if (!base.IsEnabled)
                    {
                        list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, "ColorBrushGray5", 120, 0, null, false));
                    }
                    else if (base.IsMouseOver)
                    {
                        switch (this.Theme)
                        {
                        case OurIconButton.Themes.Color:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, "ColorBrush2", 120, 0, null, false));
                            break;
                        case OurIconButton.Themes.White:
                            list.Add(ModAnimation.AaColor(this.PanBack, Border.BackgroundProperty, new ModBase.OurColor(50.0, 255.0, 255.0, 255.0) - this.PanBack.Background, 120, 0, null, false));
                            break;
                        case OurIconButton.Themes.Black:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, new ModBase.OurColor(230.0, 0.0, 0.0, 0.0) - this.Path.Fill, 120, 0, null, false));
                            break;
                        case OurIconButton.Themes.Red:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, new ModBase.OurColor(255.0, 76.0, 76.0) - this.Path.Fill, 120, 0, null, false));
                            break;
                        case OurIconButton.Themes.Custom:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, new ModBase.OurColor(255.0, this.Foreground) - this.Path.Fill, 120, 0, null, false));
                            break;
                        }
                    }
                    else
                    {
                        switch (this.Theme)
                        {
                        case OurIconButton.Themes.Color:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, "ColorBrush4", 150, 0, null, false));
                            this.PanBack.Background = new ModBase.OurColor(0.0, 255.0, 255.0, 255.0);
                            break;
                        case OurIconButton.Themes.White:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, "ColorBrush8", 150, 0, null, false));
                            list.Add(ModAnimation.AaColor(this.PanBack, Border.BackgroundProperty, new ModBase.OurColor(0.0, 255.0, 255.0, 255.0) - this.PanBack.Background, 150, 0, null, false));
                            break;
                        case OurIconButton.Themes.Black:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, new ModBase.OurColor(160.0, 0.0, 0.0, 0.0) - this.Path.Fill, 150, 0, null, false));
                            this.PanBack.Background = new ModBase.OurColor(0.0, 255.0, 255.0, 255.0);
                            break;
                        case OurIconButton.Themes.Red:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, new ModBase.OurColor(160.0, 255.0, 76.0, 76.0) - this.Path.Fill, 150, 0, null, false));
                            this.PanBack.Background = new ModBase.OurColor(0.0, 255.0, 255.0, 255.0);
                            break;
                        case OurIconButton.Themes.Custom:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, new ModBase.OurColor(160.0, this.Foreground) - this.Path.Fill, 150, 0, null, false));
                            this.PanBack.Background = new ModBase.OurColor(0.0, 255.0, 255.0, 255.0);
                            break;
                        }
                    }
                    ModAnimation.AniStart(list, "OurIconButton Color " + Conversions.ToString(this.Uuid), false);
                }
                else
                {
                    ModAnimation.AniStop("OurIconButton Color " + Conversions.ToString(this.Uuid));
                    if (base.IsEnabled)
                    {
                        switch (this.Theme)
                        {
                        case OurIconButton.Themes.Color:
                            this.Path.SetResourceReference(Shape.FillProperty, "ColorBrush5");
                            break;
                        case OurIconButton.Themes.White:
                            this.Path.SetResourceReference(Shape.FillProperty, "ColorBrush8");
                            break;
                        case OurIconButton.Themes.Black:
                            this.Path.Fill = new ModBase.OurColor(160.0, 0.0, 0.0, 0.0);
                            break;
                        case OurIconButton.Themes.Red:
                            this.Path.Fill = new ModBase.OurColor(160.0, 255.0, 76.0, 76.0);
                            break;
                        case OurIconButton.Themes.Custom:
                            this.Path.Fill = new ModBase.OurColor(160.0, this.Foreground);
                            break;
                        }
                    }
                    else
                    {
                        this.Path.SetResourceReference(Shape.FillProperty, "ColorBrushGray5");
                    }
                    this.PanBack.Background = new ModBase.OurColor(0.0, 255.0, 255.0, 255.0);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "刷新图标按钮动画状态出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurIconButton.xaml.vb");
            }
        }



        [CompilerGenerated]
        private void _Lambda___R0_1(object a0, MouseButtonEventArgs a1)
        {
            this.Button_MouseUp();
        }

        [CompilerGenerated]
        private void _Lambda___R0_2(object a0, MouseEventArgs a1)
        {
            this.Button_MouseLeave();
        }

        [CompilerGenerated]
        private void _Lambda___R0_3(object a0, MouseEventArgs a1)
        {
            this.RefreshAnim();
        }

        [CompilerGenerated]
        private void _Lambda___R0_4(object a0, MouseEventArgs a1)
        {
            this.RefreshAnim();
        }

        [CompilerGenerated]
        private void _Lambda___R0_5(object a0, RoutedEventArgs a1)
        {
            this.RefreshAnim();
        }

        [CompilerGenerated]
        private void _Lambda___R0_6(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.RefreshAnim();
        }

        [CompilerGenerated]
        private OurIconButton.ClickEventHandler ClickEvent;

        public int Uuid;

        private double _LogoScale;

        [CompilerGenerated]
        private OurIconButton.Themes _Theme;

        private SolidColorBrush _Foreground;

        private bool IsMouseDown;

        [CompilerGenerated]
        private bool _CanClick;

        private const int AnimationColorIn = 120;

        private const int AnimationColorOut = 150;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private Border _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("Path")]
        private Path _Path;

        public delegate void ClickEventHandler(object sender, EventArgs e);

        public enum Themes
        {
            Color,
            White,
            Black,
            Red,
            Custom
        }
    }
}
