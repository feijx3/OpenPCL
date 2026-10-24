using System;
using System.CodeDom.Compiler;
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
    public partial class OurExtraButton : Grid
    {
        public OurExtraButton()
        {
            base.Loaded += this._Lambda___R0_1;
            base.IsEnabledChanged += this._Lambda___R0_2;
            this._Progress = 0.0;
            this.Uuid = ModBase.GetUuid();
            this._Logo = "";
            this._LogoScale = 1.0;
            this._Show = false;
            this.ShowCheck = null;
            this._CanRightClick = false;
            this.IsLeftMouseHeld = false;
            this.IsRightMouseHeld = false;
            this.InitializeComponent();
            this.PanClick_WithEvents = this.PanClick;
        }

        public event OurExtraButton.ClickEventHandler Click
        {
            [CompilerGenerated]
            add
            {
                OurExtraButton.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurExtraButton.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurExtraButton.ClickEventHandler clickEventHandler3 = (OurExtraButton.ClickEventHandler)Delegate.Combine(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurExtraButton.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurExtraButton.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurExtraButton.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurExtraButton.ClickEventHandler clickEventHandler3 = (OurExtraButton.ClickEventHandler)Delegate.Remove(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurExtraButton.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
        }

        public event OurExtraButton.RightClickEventHandler RightClick
        {
            [CompilerGenerated]
            add
            {
                OurExtraButton.RightClickEventHandler rightClickEventHandler = this.RightClickEvent;
                OurExtraButton.RightClickEventHandler rightClickEventHandler2;
                do
                {
                    rightClickEventHandler2 = rightClickEventHandler;
                    OurExtraButton.RightClickEventHandler rightClickEventHandler3 = (OurExtraButton.RightClickEventHandler)Delegate.Combine(rightClickEventHandler2, value);
                    rightClickEventHandler = Interlocked.CompareExchange<OurExtraButton.RightClickEventHandler>(ref this.RightClickEvent, rightClickEventHandler3, rightClickEventHandler2);
                }
                while (rightClickEventHandler != rightClickEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurExtraButton.RightClickEventHandler rightClickEventHandler = this.RightClickEvent;
                OurExtraButton.RightClickEventHandler rightClickEventHandler2;
                do
                {
                    rightClickEventHandler2 = rightClickEventHandler;
                    OurExtraButton.RightClickEventHandler rightClickEventHandler3 = (OurExtraButton.RightClickEventHandler)Delegate.Remove(rightClickEventHandler2, value);
                    rightClickEventHandler = Interlocked.CompareExchange<OurExtraButton.RightClickEventHandler>(ref this.RightClickEvent, rightClickEventHandler3, rightClickEventHandler2);
                }
                while (rightClickEventHandler != rightClickEventHandler2);
            }
        }

        public double Progress
        {
            get
            {
                return this._Progress;
            }
            set
            {
                if (this._Progress != value)
                {
                    this._Progress = value;
                    if (value < 0.0001)
                    {
                        this.PanProgress.Visibility = Visibility.Collapsed;
                        return;
                    }
                    this.PanProgress.Visibility = Visibility.Visible;
                    this.RectProgress.Rect = new Rect(0.0, 40.0 * (1.0 - value), 40.0, 40.0 * value);
                }
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

        public bool Show
        {
            get
            {
                return this._Show;
            }
            set
            {
                OurExtraButton._Closure___27_0 CS_locals1 = new OurExtraButton._Closure___27_0();
                CS_locals1._VB_Me = this;
                CS_locals1._VB_Local_value = value;
                if (this._Show != CS_locals1._VB_Local_value)
                {
                    this._Show = CS_locals1._VB_Local_value;
                    ModBase.RunInUi(new Action(CS_locals1._Lambda___0), false);
                }
            }
        }

        public void ShowRefresh()
        {
            if (this.ShowCheck != null)
            {
                this.Show = this.ShowCheck();
            }
        }

        private void Button_LeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsLeftMouseHeld)
            {
                Logger.Info(string.Format("按下附加按钮{0}", RuntimeHelpers.GetObjectValue(Operators.ConditionalCompareObjectEqual(base.ToolTip, "", false) ? "" : Operators.ConcatenateObject("：", base.ToolTip))), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurExtraButton.xaml.vb");
                OurExtraButton.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
                e.Handled = true;
                this.Button_LeftMouseUp();
            }
        }

        private void Button_RightMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsRightMouseHeld)
            {
                Logger.Info(string.Format("右键按下附加按钮{0}", RuntimeHelpers.GetObjectValue(Operators.ConditionalCompareObjectEqual(base.ToolTip, "", false) ? "" : Operators.ConcatenateObject("：", base.ToolTip))), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurExtraButton.xaml.vb");
                OurExtraButton.RightClickEventHandler rightClickEvent = this.RightClickEvent;
                if (rightClickEvent != null)
                {
                    rightClickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
                e.Handled = true;
                this.Button_RightMouseUp();
            }
        }

        public bool CanRightClick
        {
            get
            {
                return this._CanRightClick;
            }
            set
            {
                this._CanRightClick = value;
            }
        }

        private void Button_LeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsLeftMouseHeld && !this.IsRightMouseHeld)
            {
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaScaleTransform(this.PanScale, 0.85 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 800, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false),
                    ModAnimation.AaScaleTransform(this.PanScale, -0.05, 60, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false)
                }, "OurExtraButton Scale " + Conversions.ToString(this.Uuid), false);
            }
            this.IsLeftMouseHeld = true;
            base.Focus();
        }

        private void Button_RightMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.CanRightClick)
            {
                if (!this.IsLeftMouseHeld && !this.IsRightMouseHeld)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaScaleTransform(this.PanScale, 0.85 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 800, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false),
                        ModAnimation.AaScaleTransform(this.PanScale, -0.05, 60, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false)
                    }, "OurExtraButton Scale " + Conversions.ToString(this.Uuid), false);
                }
                this.IsRightMouseHeld = true;
                base.Focus();
            }
        }

        private void Button_LeftMouseUp()
        {
            if (!this.IsRightMouseHeld)
            {
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanScale, 1.0 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 300, 0, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false) }, "OurExtraButton Scale " + Conversions.ToString(this.Uuid), false);
            }
            if (this.IsLeftMouseHeld)
            {
                this.RaiseCustomEvent();
            }
            this.IsLeftMouseHeld = false;
            this.RefreshColor();
        }

        private void Button_RightMouseUp()
        {
            if (this.CanRightClick)
            {
                if (!this.IsLeftMouseHeld)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanScale, 1.0 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 300, 0, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false) }, "OurExtraButton Scale " + Conversions.ToString(this.Uuid), false);
                }
                this.IsRightMouseHeld = false;
                this.RefreshColor();
            }
        }

        private void Button_MouseLeave()
        {
            this.IsLeftMouseHeld = false;
            this.IsRightMouseHeld = false;
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanScale, 1.0 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 500, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false) }, "OurExtraButton Scale " + Conversions.ToString(this.Uuid), false);
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
                        ModAnimation.AniStart(ModAnimation.AaColor(this.PanColor, Panel.BackgroundProperty, "ColorBrushGray4", 120, 0, null, false), "OurExtraButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else if (base.IsMouseOver)
                    {
                        ModAnimation.AniStart(ModAnimation.AaColor(this.PanColor, Panel.BackgroundProperty, "ColorBrush4", 120, 0, null, false), "OurExtraButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else
                    {
                        ModAnimation.AniStart(ModAnimation.AaColor(this.PanColor, Panel.BackgroundProperty, "ColorBrush3", 150, 0, null, false), "OurExtraButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                }
                else
                {
                    ModAnimation.AniStop("OurExtraButton Color " + Conversions.ToString(this.Uuid));
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
                Logger.Warn(ex, "刷新图标按钮颜色出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurExtraButton.xaml.vb");
            }
        }

        public void Ribble()
        {
            ModBase.RunInUi(new Action(this._Lambda___47_0), false);
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
                MouseButtonEventHandler mouseButtonEventHandler2 = new MouseButtonEventHandler(this.Button_RightMouseUp);
                MouseButtonEventHandler mouseButtonEventHandler3 = new MouseButtonEventHandler(this.Button_LeftMouseDown);
                MouseButtonEventHandler mouseButtonEventHandler4 = new MouseButtonEventHandler(this.Button_RightMouseDown);
                MouseButtonEventHandler mouseButtonEventHandler5 = new MouseButtonEventHandler(this._Lambda___R54_3);
                MouseButtonEventHandler mouseButtonEventHandler6 = new MouseButtonEventHandler(this._Lambda___R54_4);
                MouseEventHandler mouseEventHandler = new MouseEventHandler(this._Lambda___R54_5);
                MouseEventHandler mouseEventHandler2 = new MouseEventHandler(this._Lambda___R54_6);
                MouseEventHandler mouseEventHandler3 = new MouseEventHandler(this._Lambda___R54_7);
                Border border = this._PanClick;
                if (border != null)
                {
                    border.MouseLeftButtonUp -= mouseButtonEventHandler;
                    border.MouseRightButtonUp -= mouseButtonEventHandler2;
                    border.MouseLeftButtonDown -= mouseButtonEventHandler3;
                    border.MouseRightButtonDown -= mouseButtonEventHandler4;
                    border.MouseLeftButtonUp -= mouseButtonEventHandler5;
                    border.MouseRightButtonUp -= mouseButtonEventHandler6;
                    border.MouseLeave -= mouseEventHandler;
                    border.MouseEnter -= mouseEventHandler2;
                    border.MouseLeave -= mouseEventHandler3;
                }
                this._PanClick = value;
                border = this._PanClick;
                if (border != null)
                {
                    border.MouseLeftButtonUp += mouseButtonEventHandler;
                    border.MouseRightButtonUp += mouseButtonEventHandler2;
                    border.MouseLeftButtonDown += mouseButtonEventHandler3;
                    border.MouseRightButtonDown += mouseButtonEventHandler4;
                    border.MouseLeftButtonUp += mouseButtonEventHandler5;
                    border.MouseRightButtonUp += mouseButtonEventHandler6;
                    border.MouseLeave += mouseEventHandler;
                    border.MouseEnter += mouseEventHandler2;
                    border.MouseLeave += mouseEventHandler3;
                }
            }
        }






        [CompilerGenerated]
        private void _Lambda___R0_1(object a0, RoutedEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_2(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___27_1()
        {
            base.Visibility = Visibility.Collapsed;
        }

        [CompilerGenerated]
        private void _Lambda___47_0()
        {
            OurExtraButton._Closure___47_0 CS_locals1 = new OurExtraButton._Closure___47_0();
            CS_locals1._VB_Me = this;
            CS_locals1._VB_Local_Shape = new Border
            {
                CornerRadius = new CornerRadius(1000.0),
                BorderThickness = new Thickness(0.001),
                Opacity = 0.5,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new ScaleTransform()
            };
            CS_locals1._VB_Local_Shape.SetResourceReference(Border.BackgroundProperty, "ColorBrush5");
            this.PanScale.Children.Insert(0, CS_locals1._VB_Local_Shape);
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaScaleTransform(CS_locals1._VB_Local_Shape, 13.0, 1000, 0, new ModAnimation.AniEaseInoutFluent(ModAnimation.AniEasePower.Strong, 0.3), false),
                ModAnimation.AaOpacity(CS_locals1._VB_Local_Shape, -CS_locals1._VB_Local_Shape.Opacity, 1000, 0, null, false),
                ModAnimation.AaCode(new ThreadStart(CS_locals1._Lambda___1), 0, true)
            }, "ExtraButton Ribble " + Conversions.ToString(ModBase.GetUuid()), false);
        }

        [CompilerGenerated]
        private void _Lambda___R54_3(object a0, MouseButtonEventArgs a1)
        {
            this.Button_LeftMouseUp();
        }

        [CompilerGenerated]
        private void _Lambda___R54_4(object a0, MouseButtonEventArgs a1)
        {
            this.Button_RightMouseUp();
        }

        [CompilerGenerated]
        private void _Lambda___R54_5(object a0, MouseEventArgs a1)
        {
            this.Button_MouseLeave();
        }

        [CompilerGenerated]
        private void _Lambda___R54_6(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R54_7(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private OurExtraButton.ClickEventHandler ClickEvent;

        [CompilerGenerated]
        private OurExtraButton.RightClickEventHandler RightClickEvent;

        private double _Progress;

        public int Uuid;

        private string _Logo;

        private double _LogoScale;

        private bool _Show;

        public OurExtraButton.ShowCheckDelegate ShowCheck;

        private bool _CanRightClick;

        private bool IsLeftMouseHeld;

        private bool IsRightMouseHeld;

        private const int AnimationColorIn = 120;

        private const int AnimationColorOut = 150;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurExtraButton _PanBack;

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
        [AccessedThroughProperty("PanProgress")]
        private Border _PanProgress;

        [CompilerGenerated]
        [AccessedThroughProperty("RectProgress")]
        private RectangleGeometry _RectProgress;

        [CompilerGenerated]
        [AccessedThroughProperty("Path")]
        private Path _Path;

        public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);

        public delegate void RightClickEventHandler(object sender, MouseButtonEventArgs e);

        public delegate bool ShowCheckDelegate();

        [CompilerGenerated]
        private sealed class _Closure___27_0
        {
            internal void _Lambda___0()
            {
                if (this._VB_Local_value)
                {
                    this._VB_Me.Visibility = Visibility.Visible;
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaScaleTransform(this._VB_Me, 0.3 - ((ScaleTransform)this._VB_Me.RenderTransform).ScaleX, 500, 60, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false),
                        ModAnimation.AaScaleTransform(this._VB_Me, 0.7, 500, 60, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false),
                        ModAnimation.AaHeight(this._VB_Me, 50.0 - this._VB_Me.Height, 200, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false)
                    }, "OurExtraButton MainScale " + Conversions.ToString(this._VB_Me.Uuid), false);
                }
                else
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaScaleTransform(this._VB_Me, -((ScaleTransform)this._VB_Me.RenderTransform).ScaleX, 100, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), false),
                        ModAnimation.AaHeight(this._VB_Me, -this._VB_Me.Height, 400, 100, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                        ModAnimation.AaCode(new ThreadStart(this._VB_Me._Lambda___27_1), 0, true)
                    }, "OurExtraButton MainScale " + Conversions.ToString(this._VB_Me.Uuid), false);
                }
                this._VB_Me.IsHitTestVisible = this._VB_Local_value;
            }

            public bool _VB_Local_value;

            public OurExtraButton _VB_Me;
        }

        [CompilerGenerated]
        private sealed class _Closure___47_0
        {
            internal void _Lambda___1()
            {
                this._VB_Me.PanScale.Children.Remove(this._VB_Local_Shape);
            }

            public Border _VB_Local_Shape;

            public OurExtraButton _VB_Me;
        }
    }
}
