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
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [ContentProperty("Inlines")]
    [DesignerGenerated]
    public partial class OurIconTextButton : Border
    {
        public OurIconTextButton()
        {
            base.MouseLeftButtonUp += this._Lambda___R1_1;
            base.MouseLeftButtonDown += this._Lambda___R1_2;
            base.MouseLeave += this._Lambda___R1_3;
            base.MouseEnter += new MouseEventHandler(this.RefreshColor);
            base.Loaded += new RoutedEventHandler(this.RefreshColor);
            base.IsEnabledChanged += this._Lambda___R1_4;
            this.Uuid = ModBase.GetUuid();
            this._LogoScale = 1.0;
            this.IsMouseDown = false;
            this.InitializeComponent();
        }

        public event OurIconTextButton.CheckEventHandler Check
        {
            [CompilerGenerated]
            add
            {
                OurIconTextButton.CheckEventHandler checkEventHandler = this.CheckEvent;
                OurIconTextButton.CheckEventHandler checkEventHandler2;
                do
                {
                    checkEventHandler2 = checkEventHandler;
                    OurIconTextButton.CheckEventHandler checkEventHandler3 = (OurIconTextButton.CheckEventHandler)Delegate.Combine(checkEventHandler2, value);
                    checkEventHandler = Interlocked.CompareExchange<OurIconTextButton.CheckEventHandler>(ref this.CheckEvent, checkEventHandler3, checkEventHandler2);
                }
                while (checkEventHandler != checkEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurIconTextButton.CheckEventHandler checkEventHandler = this.CheckEvent;
                OurIconTextButton.CheckEventHandler checkEventHandler2;
                do
                {
                    checkEventHandler2 = checkEventHandler;
                    OurIconTextButton.CheckEventHandler checkEventHandler3 = (OurIconTextButton.CheckEventHandler)Delegate.Remove(checkEventHandler2, value);
                    checkEventHandler = Interlocked.CompareExchange<OurIconTextButton.CheckEventHandler>(ref this.CheckEvent, checkEventHandler3, checkEventHandler2);
                }
                while (checkEventHandler != checkEventHandler2);
            }
        }

        public event OurIconTextButton.ChangeEventHandler Change
        {
            [CompilerGenerated]
            add
            {
                OurIconTextButton.ChangeEventHandler changeEventHandler = this.ChangeEvent;
                OurIconTextButton.ChangeEventHandler changeEventHandler2;
                do
                {
                    changeEventHandler2 = changeEventHandler;
                    OurIconTextButton.ChangeEventHandler changeEventHandler3 = (OurIconTextButton.ChangeEventHandler)Delegate.Combine(changeEventHandler2, value);
                    changeEventHandler = Interlocked.CompareExchange<OurIconTextButton.ChangeEventHandler>(ref this.ChangeEvent, changeEventHandler3, changeEventHandler2);
                }
                while (changeEventHandler != changeEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurIconTextButton.ChangeEventHandler changeEventHandler = this.ChangeEvent;
                OurIconTextButton.ChangeEventHandler changeEventHandler2;
                do
                {
                    changeEventHandler2 = changeEventHandler;
                    OurIconTextButton.ChangeEventHandler changeEventHandler3 = (OurIconTextButton.ChangeEventHandler)Delegate.Remove(changeEventHandler2, value);
                    changeEventHandler = Interlocked.CompareExchange<OurIconTextButton.ChangeEventHandler>(ref this.ChangeEvent, changeEventHandler3, changeEventHandler2);
                }
                while (changeEventHandler != changeEventHandler2);
            }
        }

        public string Logo
        {
            get
            {
                return this.ShapeLogo.Data.ToString();
            }
            set
            {
                this.ShapeLogo.Data = (Geometry)new GeometryConverter().ConvertFromString(value);
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
                if (!Information.IsNothing(this.ShapeLogo))
                {
                    this.ShapeLogo.RenderTransform = new ScaleTransform
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
                return Conversions.ToString(base.GetValue(OurIconTextButton.TextProperty));
            }
            set
            {
                base.SetValue(OurIconTextButton.TextProperty, value);
            }
        }

        public OurIconTextButton.ColorState ColorType
        {
            get
            {
                return (OurIconTextButton.ColorState)Conversions.ToInteger(base.GetValue(OurIconTextButton.ColorTypeProperty));
            }
            set
            {
                if (this.ColorType != value)
                {
                    base.SetValue(OurIconTextButton.ColorTypeProperty, value);
                    this.RefreshColor(null, null);
                }
            }
        }

        public event OurIconTextButton.ClickEventHandler Click
        {
            [CompilerGenerated]
            add
            {
                OurIconTextButton.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurIconTextButton.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurIconTextButton.ClickEventHandler clickEventHandler3 = (OurIconTextButton.ClickEventHandler)Delegate.Combine(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurIconTextButton.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurIconTextButton.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurIconTextButton.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurIconTextButton.ClickEventHandler clickEventHandler3 = (OurIconTextButton.ClickEventHandler)Delegate.Remove(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurIconTextButton.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
        }

        private void OurIconTextButton_MouseUp()
        {
            if (this.IsMouseDown)
            {
                Logger.Info(string.Format("按下带图标按钮：{0}", this.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurIconTextButton.xaml.vb");
                this.IsMouseDown = false;
                OurIconTextButton.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(this, new ModBase.RouteEventArgs(true));
                }
                this.RaiseCustomEvent();
                this.RefreshColor(null, null);
            }
        }

        private void OurIconTextButton_MouseDown()
        {
            this.IsMouseDown = true;
            this.RefreshColor(null, null);
        }

        private void OurIconTextButton_MouseLeave()
        {
            this.IsMouseDown = false;
            this.RefreshColor(null, null);
        }

        private void RefreshColor(object obj = null, object e = null)
        {
            try
            {
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0 && !false.Equals(RuntimeHelpers.GetObjectValue(e)))
                {
                    OurIconTextButton.ColorState colorType = this.ColorType;
                    if (colorType != OurIconTextButton.ColorState.Black)
                    {
                        if (colorType == OurIconTextButton.ColorState.Highlight)
                        {
                            if (this.IsMouseDown)
                            {
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, "ColorBrush6", 70, 0, null, false), "OurIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else if (base.IsMouseOver)
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[]
                                {
                                    ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrush3", 100, 0, null, false),
                                    ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 100, 0, null, false)
                                }, "OurIconTextButton Checked " + Conversions.ToString(this.Uuid), false);
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, "ColorBrushBg1", 100, 0, null, false), "OurIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else if (base.IsEnabled)
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[]
                                {
                                    ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrush3", 150, 0, null, false),
                                    ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 150, 0, null, false)
                                }, "OurIconTextButton Checked " + Conversions.ToString(this.Uuid), false);
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - base.Background, 150, 0, null, false), "OurIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[]
                                {
                                    ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrushGray5", 100, 0, null, false),
                                    ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrushGray5", 100, 0, null, false)
                                }, "OurIconTextButton Checked " + Conversions.ToString(this.Uuid), false);
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - base.Background, 150, 0, null, false), "OurIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                        }
                    }
                    else if (this.IsMouseDown)
                    {
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, "ColorBrush6", 70, 0, null, false), "OurIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else if (base.IsMouseOver)
                    {
                        ModAnimation.AniStart(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrush3", 100, 0, null, false),
                            ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 100, 0, null, false)
                        }, "OurIconTextButton Checked " + Conversions.ToString(this.Uuid), false);
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, "ColorBrushBg1", 100, 0, null, false), "OurIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else if (base.IsEnabled)
                    {
                        ModAnimation.AniStart(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrush1", 150, 0, null, false),
                            ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush1", 150, 0, null, false)
                        }, "OurIconTextButton Checked " + Conversions.ToString(this.Uuid), false);
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - base.Background, 150, 0, null, false), "OurIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else
                    {
                        ModAnimation.AniStart(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrushGray5", 100, 0, null, false),
                            ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrushGray5", 100, 0, null, false)
                        }, "OurIconTextButton Checked " + Conversions.ToString(this.Uuid), false);
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - base.Background, 150, 0, null, false), "OurIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                }
                else
                {
                    ModAnimation.AniStop("OurIconTextButton Checked " + Conversions.ToString(this.Uuid));
                    ModAnimation.AniStop("OurIconTextButton Color " + Conversions.ToString(this.Uuid));
                    OurIconTextButton.ColorState colorType2 = this.ColorType;
                    if (colorType2 != OurIconTextButton.ColorState.Black)
                    {
                        if (colorType2 == OurIconTextButton.ColorState.Highlight)
                        {
                            base.Background = ModSecret.ColorSemiTransparent;
                            this.ShapeLogo.SetResourceReference(Shape.FillProperty, base.IsEnabled ? "ColorBrush3" : "ColorBrushGray5");
                            this.LabText.SetResourceReference(TextBlock.ForegroundProperty, base.IsEnabled ? "ColorBrush3" : "ColorBrushGray5");
                        }
                    }
                    else
                    {
                        base.Background = ModSecret.ColorSemiTransparent;
                        this.ShapeLogo.SetResourceReference(Shape.FillProperty, base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5");
                        this.LabText.SetResourceReference(TextBlock.ForegroundProperty, base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "刷新带图标按钮颜色出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurIconTextButton.xaml.vb");
            }
        }




        [CompilerGenerated]
        private void _Lambda___R1_1(object a0, MouseButtonEventArgs a1)
        {
            this.OurIconTextButton_MouseUp();
        }

        [CompilerGenerated]
        private void _Lambda___R1_2(object a0, MouseButtonEventArgs a1)
        {
            this.OurIconTextButton_MouseDown();
        }

        [CompilerGenerated]
        private void _Lambda___R1_3(object a0, MouseEventArgs a1)
        {
            this.OurIconTextButton_MouseLeave();
        }

        [CompilerGenerated]
        private void _Lambda___R1_4(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.RefreshColor(RuntimeHelpers.GetObjectValue(a0), a1);
        }

        public int Uuid;

        [CompilerGenerated]
        private OurIconTextButton.CheckEventHandler CheckEvent;

        [CompilerGenerated]
        private OurIconTextButton.ChangeEventHandler ChangeEvent;

        private double _LogoScale;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(OurIconTextButton), new PropertyMetadata(new PropertyChangedCallback(OurIconTextButton._Closure___._I._Lambda___0_0)));

        public static readonly DependencyProperty ColorTypeProperty = DependencyProperty.Register("ColorType", typeof(OurIconTextButton.ColorState), typeof(OurIconTextButton), new PropertyMetadata(OurIconTextButton.ColorState.Black));

        [CompilerGenerated]
        private OurIconTextButton.ClickEventHandler ClickEvent;

        private bool IsMouseDown;

        private const int AnimationTimeOfMouseIn = 100;

        private const int AnimationTimeOfMouseOut = 150;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurIconTextButton _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("ShapeLogo")]
        private Path _ShapeLogo;

        [CompilerGenerated]
        [AccessedThroughProperty("LabText")]
        private TextBlock _LabText;

        public delegate void CheckEventHandler(object sender, bool raiseByMouse);

        public delegate void ChangeEventHandler(object sender, bool raiseByMouse);

        public enum ColorState
        {
            Black,
            Highlight
        }

        public delegate void ClickEventHandler(object sender, ModBase.RouteEventArgs e);

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal void _Lambda___0_0(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                if (!Information.IsNothing(sender))
                {
                    ((OurIconTextButton)sender).LabText.Text = Conversions.ToString(e.NewValue);
                }
            }

            public static readonly OurIconTextButton._Closure___ _I = new OurIconTextButton._Closure___();
        }
    }
}
