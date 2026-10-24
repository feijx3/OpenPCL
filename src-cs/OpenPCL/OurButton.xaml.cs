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
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [ContentProperty("Inlines")]
    [DesignerGenerated]
    public partial class OurButton : Border
    {
        public OurButton()
        {
            base.MouseEnter += new MouseEventHandler(this.RefreshColor);
            base.MouseLeave += new MouseEventHandler(this.RefreshColor);
            base.Loaded += new RoutedEventHandler(this.RefreshColor);
            base.IsEnabledChanged += this._Lambda___R1_2;
            base.MouseLeftButtonUp += this.Button_MouseUp;
            base.MouseLeftButtonDown += this.Button_MouseDown;
            base.MouseEnter += this._Lambda___R1_3;
            base.MouseLeftButtonUp += this._Lambda___R1_4;
            base.MouseLeave += this._Lambda___R1_5;
            this.Uuid = ModBase.GetUuid();
            this._ColorType = OurButton.ColorState.Normal;
            this.IsMouseDown = false;
            this.InitializeComponent();
        }

        public event OurButton.ClickEventHandler Click
        {
            [CompilerGenerated]
            add
            {
                OurButton.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurButton.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurButton.ClickEventHandler clickEventHandler3 = (OurButton.ClickEventHandler)Delegate.Combine(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurButton.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurButton.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurButton.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurButton.ClickEventHandler clickEventHandler3 = (OurButton.ClickEventHandler)Delegate.Remove(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurButton.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
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
                return Conversions.ToString(base.GetValue(OurButton.TextProperty));
            }
            set
            {
                base.SetValue(OurButton.TextProperty, value);
            }
        }

        public Thickness TextPadding
        {
            get
            {
                return this.LabText.Padding;
            }
            set
            {
                this.LabText.Padding = value;
            }
        }

        public OurButton.ColorState ColorType
        {
            get
            {
                return this._ColorType;
            }
            set
            {
                this._ColorType = value;
                this.RefreshColor(null, null);
            }
        }

        public new Thickness Padding
        {
            get
            {
                return this.PanFore.Padding;
            }
            set
            {
                this.PanFore.Padding = value;
            }
        }

        public Transform RealRenderTransform
        {
            get
            {
                return this.PanFore.RenderTransform;
            }
            set
            {
                this.PanFore.RenderTransform = value;
            }
        }

        private void RefreshColor(object obj = null, object e = null)
        {
            try
            {
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                {
                    if (base.IsEnabled)
                    {
                        switch (this.ColorType)
                        {
                        case OurButton.ColorState.Normal:
                            if (base.IsMouseOver)
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.PanFore, Border.BorderBrushProperty, "ColorBrush3", 100, 0, null, false) }, "OurButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.PanFore, Border.BorderBrushProperty, "ColorBrush1", 200, 0, null, false) }, "OurButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            break;
                        case OurButton.ColorState.Highlight:
                            if (base.IsMouseOver)
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.PanFore, Border.BorderBrushProperty, "ColorBrush3", 100, 0, null, false) }, "OurButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.PanFore, Border.BorderBrushProperty, "ColorBrush2", 200, 0, null, false) }, "OurButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            break;
                        case OurButton.ColorState.Red:
                            if (base.IsMouseOver)
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.PanFore, Border.BorderBrushProperty, "ColorBrushRedLight", 100, 0, null, false) }, "OurButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.PanFore, Border.BorderBrushProperty, "ColorBrushRedDark", 200, 0, null, false) }, "OurButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            break;
                        }
                    }
                    else
                    {
                        ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.PanFore, Border.BorderBrushProperty, ModSecret.ColorGray4 - this.PanFore.BorderBrush, 200, 0, null, false) }, "OurButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                }
                else
                {
                    ModAnimation.AniStop("OurButton Color " + Conversions.ToString(this.Uuid));
                    if (base.IsEnabled)
                    {
                        switch (this.ColorType)
                        {
                        case OurButton.ColorState.Normal:
                            if (base.IsMouseOver)
                            {
                                this.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrush3");
                            }
                            else
                            {
                                this.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrush1");
                            }
                            break;
                        case OurButton.ColorState.Highlight:
                            if (base.IsMouseOver)
                            {
                                this.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrush3");
                            }
                            else
                            {
                                this.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrush2");
                            }
                            break;
                        case OurButton.ColorState.Red:
                            if (base.IsMouseOver)
                            {
                                this.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrushRedLight");
                            }
                            else
                            {
                                this.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrushRedDark");
                            }
                            break;
                        }
                    }
                    else
                    {
                        this.PanFore.BorderBrush = ModSecret.ColorGray4;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "刷新按钮颜色出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurButton.xaml.vb");
            }
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseDown)
            {
                Logger.Info(string.Format("按下按钮：{0}", this.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurButton.xaml.vb");
                OurButton.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
                this.RaiseCustomEvent();
            }
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.IsMouseDown = true;
            base.Focus();
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaScaleTransform(this.PanFore, 0.955 - ((ScaleTransform)this.PanFore.RenderTransform).ScaleX, 80, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.ExtraStrong), false),
                ModAnimation.AaScaleTransform(this.PanFore, -0.01, 700, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false)
            }, "OurButton Scale " + Conversions.ToString(this.Uuid), false);
        }

        private void Button_MouseEnter()
        {
            ModAnimation.AniStart(ModAnimation.AaColor(this.PanFore, Border.BackgroundProperty, (this._ColorType == OurButton.ColorState.Red) ? "ColorBrushRedBack" : "ColorBrush7", 100, 0, null, false), "OurButton Background " + Conversions.ToString(this.Uuid), false);
        }

        private void Button_MouseUp()
        {
            if (this.IsMouseDown)
            {
                this.IsMouseDown = false;
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanFore, 1.0 - ((ScaleTransform)this.PanFore.RenderTransform).ScaleX, 300, 10, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false) }, "OurButton Scale " + Conversions.ToString(this.Uuid), false);
            }
        }

        private void Button_MouseLeave()
        {
            ModAnimation.AniStart(ModAnimation.AaColor(this.PanFore, Border.BackgroundProperty, "ColorBrushHalfWhite", 200, 0, null, false), "OurButton Background " + Conversions.ToString(this.Uuid), false);
            if (this.IsMouseDown)
            {
                this.IsMouseDown = false;
                ModAnimation.AniStart(ModAnimation.AaScaleTransform(this.PanFore, 1.0 - ((ScaleTransform)this.PanFore.RenderTransform).ScaleX, 800, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false), "OurButton Scale " + Conversions.ToString(this.Uuid), false);
            }
        }




        [CompilerGenerated]
        private void _Lambda___R1_2(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.RefreshColor(RuntimeHelpers.GetObjectValue(a0), a1);
        }

        [CompilerGenerated]
        private void _Lambda___R1_3(object a0, MouseEventArgs a1)
        {
            this.Button_MouseEnter();
        }

        [CompilerGenerated]
        private void _Lambda___R1_4(object a0, MouseButtonEventArgs a1)
        {
            this.Button_MouseUp();
        }

        [CompilerGenerated]
        private void _Lambda___R1_5(object a0, MouseEventArgs a1)
        {
            this.Button_MouseLeave();
        }

        [CompilerGenerated]
        private OurButton.ClickEventHandler ClickEvent;

        public int Uuid;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(OurButton), new PropertyMetadata(new PropertyChangedCallback(OurButton._Closure___._I._Lambda___0_0)));

        private OurButton.ColorState _ColorType;

        public new static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(OurButton), new PropertyMetadata(new PropertyChangedCallback(OurButton._Closure___._I._Lambda___R0_1)));

        private const int AnimationColorIn = 100;

        private const int AnimationColorOut = 200;

        private bool IsMouseDown;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurButton _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanFore")]
        private Border _PanFore;

        [CompilerGenerated]
        [AccessedThroughProperty("LabText")]
        private TextBlock _LabText;

        public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);

        public enum ColorState
        {
            Normal,
            Highlight,
            Red
        }

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal void _Lambda___0_0(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                if (sender != null)
                {
                    ((OurButton)sender).LabText.Text = Conversions.ToString(e.NewValue);
                }
            }

            internal void _Lambda___R0_1(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
            {
                ((OurButton._Closure___._I0_1 == null) ? (OurButton._Closure___._I0_1 = new VB_AnonymousDelegate_8<OurButton, DependencyPropertyChangedEventArgs>(OurButton._Closure___._I._Lambda___0_1)) : OurButton._Closure___._I0_1)((OurButton)a0, a1);
            }

            internal void _Lambda___0_1(OurButton sender, DependencyPropertyChangedEventArgs e)
            {
                if (sender != null)
                {
                    Border panFore = sender.PanFore;
                    object newValue = e.NewValue;
                    panFore.Padding = ((newValue != null) ? ((Thickness)newValue) : default(Thickness));
                }
            }

            public static readonly OurButton._Closure___ _I = new OurButton._Closure___();

            public static VB_AnonymousDelegate_8<OurButton, DependencyPropertyChangedEventArgs> _I0_1;
        }
    }
}
