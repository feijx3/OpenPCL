using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    public partial class OurSlider : Border, ISettingControl
    {
        public OurSlider()
        {
            base.SizeChanged += this.RefreshWidth;
            base.MouseLeftButtonDown += this.DragStart;
            base.IsEnabledChanged += this._Lambda___R0_1;
            base.MouseEnter += this._Lambda___R0_2;
            base.MouseLeave += this._Lambda___R0_3;
            base.MouseEnter += this._Lambda___R0_4;
            base.KeyDown += this.OurSlider_KeyDown;
            this.Change += this._Lambda___R0_5;
            this.Uuid = ModBase.GetUuid();
            this._MaxValue = 100;
            this.ChangeByKey = false;
            this._Value = 0;
            this.ValueByKey = 1U;
            this.InitializeComponent();
        }

        public event OurSlider.ChangeEventHandler Change
        {
            [CompilerGenerated]
            add
            {
                OurSlider.ChangeEventHandler changeEventHandler = this.ChangeEvent;
                OurSlider.ChangeEventHandler changeEventHandler2;
                do
                {
                    changeEventHandler2 = changeEventHandler;
                    OurSlider.ChangeEventHandler changeEventHandler3 = (OurSlider.ChangeEventHandler)Delegate.Combine(changeEventHandler2, value);
                    changeEventHandler = Interlocked.CompareExchange<OurSlider.ChangeEventHandler>(ref this.ChangeEvent, changeEventHandler3, changeEventHandler2);
                }
                while (changeEventHandler != changeEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurSlider.ChangeEventHandler changeEventHandler = this.ChangeEvent;
                OurSlider.ChangeEventHandler changeEventHandler2;
                do
                {
                    changeEventHandler2 = changeEventHandler;
                    OurSlider.ChangeEventHandler changeEventHandler3 = (OurSlider.ChangeEventHandler)Delegate.Remove(changeEventHandler2, value);
                    changeEventHandler = Interlocked.CompareExchange<OurSlider.ChangeEventHandler>(ref this.ChangeEvent, changeEventHandler3, changeEventHandler2);
                }
                while (changeEventHandler != changeEventHandler2);
            }
        }

        public event OurSlider.PreviewChangeEventHandler PreviewChange
        {
            [CompilerGenerated]
            add
            {
                OurSlider.PreviewChangeEventHandler previewChangeEventHandler = this.PreviewChangeEvent;
                OurSlider.PreviewChangeEventHandler previewChangeEventHandler2;
                do
                {
                    previewChangeEventHandler2 = previewChangeEventHandler;
                    OurSlider.PreviewChangeEventHandler previewChangeEventHandler3 = (OurSlider.PreviewChangeEventHandler)Delegate.Combine(previewChangeEventHandler2, value);
                    previewChangeEventHandler = Interlocked.CompareExchange<OurSlider.PreviewChangeEventHandler>(ref this.PreviewChangeEvent, previewChangeEventHandler3, previewChangeEventHandler2);
                }
                while (previewChangeEventHandler != previewChangeEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurSlider.PreviewChangeEventHandler previewChangeEventHandler = this.PreviewChangeEvent;
                OurSlider.PreviewChangeEventHandler previewChangeEventHandler2;
                do
                {
                    previewChangeEventHandler2 = previewChangeEventHandler;
                    OurSlider.PreviewChangeEventHandler previewChangeEventHandler3 = (OurSlider.PreviewChangeEventHandler)Delegate.Remove(previewChangeEventHandler2, value);
                    previewChangeEventHandler = Interlocked.CompareExchange<OurSlider.PreviewChangeEventHandler>(ref this.PreviewChangeEvent, previewChangeEventHandler3, previewChangeEventHandler2);
                }
                while (previewChangeEventHandler != previewChangeEventHandler2);
            }
        }

        public int MaxValue
        {
            get
            {
                return this._MaxValue;
            }
            set
            {
                if (value != this._MaxValue)
                {
                    this._MaxValue = value;
                    this.RefreshWidth(null, null);
                }
            }
        }

        public int Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                try
                {
                    value = value.Clamp(0, this.MaxValue);
                    if (this._Value != value)
                    {
                        int value2 = this._Value;
                        this._Value = value;
                        if (ModAnimation.AniControlEnabled == 0)
                        {
                            ModBase.RouteEventArgs e = new ModBase.RouteEventArgs(false);
                            OurSlider.PreviewChangeEventHandler previewChangeEvent = this.PreviewChangeEvent;
                            if (previewChangeEvent != null)
                            {
                                previewChangeEvent(this, e);
                            }
                            if (e.Handled)
                            {
                                this._Value = value2;
                                this.DragStop();
                                return;
                            }
                        }
                        if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                        {
                            if (base.ActualWidth < this.ShapeDot.Width)
                            {
                                return;
                            }
                            double num = (double)this._Value / (double)this.MaxValue * (base.ActualWidth - this.ShapeDot.Width);
                            double num2 = Math.Abs(this.LineFore.Width / (base.ActualWidth - this.ShapeDot.Width) - (double)this._Value / (double)this.MaxValue);
                            double num3 = (1.0 - Math.Pow(1.0 - num2, 3.0)) * 300.0 + (double)(this.ChangeByKey ? 100 : 0);
                            ModAnimation.AniStart(new ModAnimation.AniData[]
                            {
                                ModAnimation.AaWidth(this.LineFore, Math.Max(0.0, num + ((num < 0.5) ? 0.0 : 0.5)) - this.LineFore.Width, checked((int)Math.Round(num3)), 0, (ModAnimation.AniEase)((num3 > 50.0) ? new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle) : new ModAnimation.AniEaseLinear()), false),
                                ModAnimation.AaWidth(this.LineBack, Math.Max(0.0, base.ActualWidth - this.ShapeDot.Width - num + ((base.ActualWidth - this.ShapeDot.Width - num < 0.5) ? 0.0 : 0.5)) - this.LineBack.Width, checked((int)Math.Round(num3)), 0, (ModAnimation.AniEase)((num3 > 50.0) ? new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle) : new ModAnimation.AniEaseLinear()), false),
                                ModAnimation.AaX(this.ShapeDot, num - this.ShapeDot.Margin.Left, checked((int)Math.Round(num3)), 0, (ModAnimation.AniEase)((num3 > 50.0) ? new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle) : new ModAnimation.AniEaseLinear()), false)
                            }, "OurSlider Progress " + Conversions.ToString(this.Uuid), false);
                        }
                        else
                        {
                            this.RefreshWidth(null, null);
                        }
                        if (ModAnimation.AniControlEnabled == 0)
                        {
                            OurSlider.ChangeEventHandler changeEvent = this.ChangeEvent;
                            if (changeEvent != null)
                            {
                                changeEvent(this, false);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "滑动条进度改变出错", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurSlider.xaml.vb");
                }
            }
        }

        private void RefreshWidth(object sender, SizeChangedEventArgs e)
        {
            if (!Information.IsNothing(e))
            {
                this.PanMain.Width = e.NewSize.Width;
            }
            ModAnimation.AniStop("OurSlider Progress " + Conversions.ToString(this.Uuid));
            double num = (double)this._Value / (double)this.MaxValue * (base.ActualWidth - this.ShapeDot.Width);
            this.LineFore.Width = Math.Max(0.0, num + ((num < 0.5) ? 0.0 : 0.5));
            this.LineBack.Width = Math.Max(0.0, base.ActualWidth - this.ShapeDot.Width - num + ((base.ActualWidth - this.ShapeDot.Width - num < 0.5) ? 0.0 : 0.5));
            this.ShapeDot.Margin = new Thickness(num, this.ShapeDot.Margin.Top, this.ShapeDot.Margin.Right, this.ShapeDot.Margin.Bottom);
        }

        private void DragStart(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ModMain.DragControl = this;
            this.RefreshColor();
            ModMain.FrmMain.DragDoing();
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.ShapeDot, 1.3 - ((ScaleTransform)this.ShapeDot.RenderTransform).ScaleX, 40, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false) }, "OurSlider Scale " + Conversions.ToString(this.Uuid), false);
            this.RefreshPopup();
            ModAnimation.AniStop("OurSlider KeyPopup " + Conversions.ToString(this.Uuid));
        }

        public void DragDoing()
        {
            checked
            {
                int num = (int)Math.Round(unchecked(((Mouse.GetPosition(this.PanMain).X - this.ShapeDot.Width / 2.0) / (base.ActualWidth - this.ShapeDot.Width)).Clamp(0.0, 1.0) * (double)this.MaxValue));
                if (num != this.Value)
                {
                    this.Value = num;
                }
                this.RefreshPopup();
            }
        }

        public void DragStop()
        {
            this.RefreshColor();
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.ShapeDot, 1.0 - ((ScaleTransform)this.ShapeDot.RenderTransform).ScaleX, 200, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false) }, "OurSlider Scale " + Conversions.ToString(this.Uuid), false);
            this.Popup.IsOpen = false;
        }

        public void RefreshPopup()
        {
            if (this.GetHintText != null)
            {
                this.Popup.IsOpen = true;
                this.TextHint.Text = Conversions.ToString(this.GetHintText.DynamicInvoke(new object[] { this.Value }));
                Typeface typeface = new Typeface(this.TextHint.FontFamily, this.TextHint.FontStyle, this.TextHint.FontWeight, this.TextHint.FontStretch);
                FormattedText formattedText = new FormattedText(this.TextHint.Text, Thread.CurrentThread.CurrentCulture, this.TextHint.FlowDirection, typeface, this.TextHint.FontSize, this.TextHint.Foreground, (double)ModBase.DPI);
                this.TextHint.Width = formattedText.Width;
            }
        }

        private void RefreshColor()
        {
            try
            {
                string text;
                string text2;
                int num;
                if (base.IsEnabled)
                {
                    if (!Information.IsNothing(RuntimeHelpers.GetObjectValue(ModMain.DragControl)) && ModMain.DragControl.Equals(this))
                    {
                        text = "ColorBrush3";
                        text2 = "ColorBrush3";
                        num = 40;
                    }
                    else if (base.IsMouseOver)
                    {
                        text = "ColorBrush3";
                        text2 = "ColorBrush3";
                        num = 40;
                    }
                    else
                    {
                        text = "ColorBrushBg0";
                        text2 = "ColorBrushBg0";
                        num = 100;
                    }
                }
                else
                {
                    text = "ColorBrushGray5";
                    text2 = "ColorBrushGray5";
                    num = 200;
                }
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaColor(this, Border.BorderBrushProperty, text, num, 0, null, false),
                        ModAnimation.AaColor(this.ShapeDot, Shape.FillProperty, text2, num, 0, null, false)
                    }, "OurSlider Color " + Conversions.ToString(this.Uuid), false);
                }
                else
                {
                    ModAnimation.AniStop("OurSlider Color " + Conversions.ToString(this.Uuid));
                    base.SetResourceReference(Border.BorderBrushProperty, text);
                    this.ShapeDot.SetResourceReference(Shape.FillProperty, text2);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "滑动条颜色改变出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurSlider.xaml.vb");
            }
        }

        public uint ValueByKey
        {
            [CompilerGenerated]
            get
            {
                return this._ValueByKey;
            }
            [CompilerGenerated]
            set
            {
                this._ValueByKey = value;
            }
        }

        private void OurSlider_MouseEnter()
        {
            base.Focus();
        }

        private void OurSlider_KeyDown(object sender, KeyEventArgs e)
        {
            checked
            {
                if (!object.ReferenceEquals(this, RuntimeHelpers.GetObjectValue(ModMain.DragControl)))
                {
                    if (e.Key == Key.Left)
                    {
                        this.ChangeByKey = true;
                        this.Value = (int)(unchecked((long)this.Value) - (long)(unchecked((ulong)this.ValueByKey)));
                        this.ChangeByKey = false;
                        e.Handled = true;
                    }
                    else
                    {
                        if (e.Key != Key.Right)
                        {
                            return;
                        }
                        this.ChangeByKey = true;
                        this.Value = (int)(unchecked((long)this.Value) + (long)(unchecked((ulong)this.ValueByKey)));
                        this.ChangeByKey = false;
                        e.Handled = true;
                    }
                    if (this.GetHintText != null)
                    {
                        this.RefreshPopup();
                        ModAnimation.AniStop("OurSlider KeyPopup " + Conversions.ToString(this.Uuid));
                        ModAnimation.AniStart(ModAnimation.AaCode(new ThreadStart(this._Lambda___33_0), (int)Math.Round(unchecked(700.0 * ModAnimation.AniSpeed)), false), "OurSlider KeyPopup " + Conversions.ToString(this.Uuid), false);
                    }
                }
            }
        }

        void ISettingControl.RefreshSetting(string NewValue)
        {
            this.Value = checked((int)Math.Round(ModBase.Val(NewValue)));
        }

        string ISettingControl.GetCurrentSetting()
        {
            return Conversions.ToString(this.Value);
        }

        private void SaveSetting()
        {
            SettingService.SaveSetting(this);
        }








        [CompilerGenerated]
        private void _Lambda___R0_1(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_2(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_3(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_4(object a0, MouseEventArgs a1)
        {
            this.OurSlider_MouseEnter();
        }

        [CompilerGenerated]
        private void _Lambda___R0_5(object a0, bool a1)
        {
            this.SaveSetting();
        }

        [CompilerGenerated]
        private void _Lambda___33_0()
        {
            this.Popup.IsOpen = false;
        }

        public int Uuid;

        [CompilerGenerated]
        private OurSlider.ChangeEventHandler ChangeEvent;

        [CompilerGenerated]
        private OurSlider.PreviewChangeEventHandler PreviewChangeEvent;

        private int _MaxValue;

        private bool ChangeByKey;

        private int _Value;

        public Delegate GetHintText;

        [CompilerGenerated]
        private uint _ValueByKey;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurSlider _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanMain")]
        private Grid _PanMain;

        [CompilerGenerated]
        [AccessedThroughProperty("LineBack")]
        private Line _LineBack;

        [CompilerGenerated]
        [AccessedThroughProperty("LineFore")]
        private Line _LineFore;

        [CompilerGenerated]
        [AccessedThroughProperty("ShapeDot")]
        private Ellipse _ShapeDot;

        [CompilerGenerated]
        [AccessedThroughProperty("Popup")]
        private Popup _Popup;

        [CompilerGenerated]
        [AccessedThroughProperty("TextHint")]
        private TextBlock _TextHint;

        public delegate void ChangeEventHandler(object sender, bool user);

        public delegate void PreviewChangeEventHandler(object sender, ModBase.RouteEventArgs e);
    }
}
