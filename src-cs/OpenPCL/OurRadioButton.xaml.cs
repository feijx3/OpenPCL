using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
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
    public partial class OurRadioButton : Border
    {
        public OurRadioButton()
        {
            base.MouseLeftButtonUp += this._Lambda___R1_1;
            base.MouseLeftButtonDown += this._Lambda___R1_2;
            base.MouseLeave += this._Lambda___R1_3;
            base.MouseEnter += new MouseEventHandler(this.RefreshColor);
            base.MouseLeave += new MouseEventHandler(this.RefreshColor);
            base.Loaded += new RoutedEventHandler(this.RefreshColor);
            this.Uuid = ModBase.GetUuid();
            this._LogoScale = 1.0;
            this._Checked = false;
            this._ColorType = OurRadioButton.ColorState.White;
            this.IsMouseDown = false;
            this.InitializeComponent();
        }

        public event OurRadioButton.CheckEventHandler Check
        {
            [CompilerGenerated]
            add
            {
                OurRadioButton.CheckEventHandler checkEventHandler = this.CheckEvent;
                OurRadioButton.CheckEventHandler checkEventHandler2;
                do
                {
                    checkEventHandler2 = checkEventHandler;
                    OurRadioButton.CheckEventHandler checkEventHandler3 = (OurRadioButton.CheckEventHandler)Delegate.Combine(checkEventHandler2, value);
                    checkEventHandler = Interlocked.CompareExchange<OurRadioButton.CheckEventHandler>(ref this.CheckEvent, checkEventHandler3, checkEventHandler2);
                }
                while (checkEventHandler != checkEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurRadioButton.CheckEventHandler checkEventHandler = this.CheckEvent;
                OurRadioButton.CheckEventHandler checkEventHandler2;
                do
                {
                    checkEventHandler2 = checkEventHandler;
                    OurRadioButton.CheckEventHandler checkEventHandler3 = (OurRadioButton.CheckEventHandler)Delegate.Remove(checkEventHandler2, value);
                    checkEventHandler = Interlocked.CompareExchange<OurRadioButton.CheckEventHandler>(ref this.CheckEvent, checkEventHandler3, checkEventHandler2);
                }
                while (checkEventHandler != checkEventHandler2);
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

        public bool Checked
        {
            get
            {
                return this._Checked;
            }
            set
            {
                this.SetChecked(value, false, true);
            }
        }

        public void SetChecked(bool value, bool raiseByMouse, bool anime)
        {
            checked
            {
                try
                {
                    bool flag = false;
                    if (this._Checked != value)
                    {
                        this._Checked = value;
                        flag = true;
                    }
                    if (base.Parent != null)
                    {
                        List<OurRadioButton> list = new List<OurRadioButton>();
                        int num = 0;
                        try
                        {
                            foreach (object obj in ((IEnumerable)NewLateBinding.LateGet(base.Parent, null, "Children", new object[0], null, null, null)))
                            {
                                object objectValue = RuntimeHelpers.GetObjectValue(obj);
                                if (objectValue is OurRadioButton)
                                {
                                    list.Add((OurRadioButton)objectValue);
                                    if (Conversions.ToBoolean(NewLateBinding.LateGet(objectValue, null, "Checked", new object[0], null, null, null)))
                                    {
                                        num++;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator enumerator = default;
                            if (enumerator is IDisposable)
                            {
                                (enumerator as IDisposable).Dispose();
                            }
                        }
                        int num2 = num;
                        if (num2 == 0)
                        {
                            list[0].Checked = true;
                        }
                        else if (num2 > 1)
                        {
                            if (this.Checked)
                            {
                                try
                                {
                                    foreach (OurRadioButton ourRadioButton in list)
                                    {
                                        if (ourRadioButton.Checked && !ourRadioButton.Equals(this))
                                        {
                                            ourRadioButton.Checked = false;
                                        }
                                    }
                                    goto IL_016B;
                                }
                                finally
                                {
                                    List<OurRadioButton>.Enumerator enumerator2 = default;
                                    ((IDisposable)enumerator2).Dispose();
                                }
                            }
                            bool flag2 = false;
                            try
                            {
                                foreach (OurRadioButton ourRadioButton2 in list)
                                {
                                    if (ourRadioButton2.Checked)
                                    {
                                        if (flag2)
                                        {
                                            ourRadioButton2.Checked = false;
                                        }
                                        else
                                        {
                                            flag2 = true;
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                List<OurRadioButton>.Enumerator enumerator3 = default;
                                ((IDisposable)enumerator3).Dispose();
                            }
                        }
                        IL_016B:
                        if (flag)
                        {
                            this.RefreshColor(null, anime);
                            if (this.Checked)
                            {
                                OurRadioButton.CheckEventHandler checkEvent = this.CheckEvent;
                                if (checkEvent != null)
                                {
                                    checkEvent(this, raiseByMouse);
                                }
                            }
                            this.RaiseCustomEvent();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "单选按钮勾选改变错误", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurRadioButton.xaml.vb");
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
                return Conversions.ToString(base.GetValue(OurRadioButton.TextProperty));
            }
            set
            {
                base.SetValue(OurRadioButton.TextProperty, value);
            }
        }

        public OurRadioButton.ColorState ColorType
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

        public event OurRadioButton.PreviewClickEventHandler PreviewClick
        {
            [CompilerGenerated]
            add
            {
                OurRadioButton.PreviewClickEventHandler previewClickEventHandler = this.PreviewClickEvent;
                OurRadioButton.PreviewClickEventHandler previewClickEventHandler2;
                do
                {
                    previewClickEventHandler2 = previewClickEventHandler;
                    OurRadioButton.PreviewClickEventHandler previewClickEventHandler3 = (OurRadioButton.PreviewClickEventHandler)Delegate.Combine(previewClickEventHandler2, value);
                    previewClickEventHandler = Interlocked.CompareExchange<OurRadioButton.PreviewClickEventHandler>(ref this.PreviewClickEvent, previewClickEventHandler3, previewClickEventHandler2);
                }
                while (previewClickEventHandler != previewClickEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurRadioButton.PreviewClickEventHandler previewClickEventHandler = this.PreviewClickEvent;
                OurRadioButton.PreviewClickEventHandler previewClickEventHandler2;
                do
                {
                    previewClickEventHandler2 = previewClickEventHandler;
                    OurRadioButton.PreviewClickEventHandler previewClickEventHandler3 = (OurRadioButton.PreviewClickEventHandler)Delegate.Remove(previewClickEventHandler2, value);
                    previewClickEventHandler = Interlocked.CompareExchange<OurRadioButton.PreviewClickEventHandler>(ref this.PreviewClickEvent, previewClickEventHandler3, previewClickEventHandler2);
                }
                while (previewClickEventHandler != previewClickEventHandler2);
            }
        }

        private void Radiobox_MouseUp()
        {
            if (!this.Checked && this.IsMouseDown)
            {
                Logger.Info(string.Format("按下单选按钮：{0}", this.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurRadioButton.xaml.vb");
                this.IsMouseDown = false;
                ModBase.RouteEventArgs e = new ModBase.RouteEventArgs(true);
                OurRadioButton.PreviewClickEventHandler previewClickEvent = this.PreviewClickEvent;
                if (previewClickEvent != null)
                {
                    previewClickEvent(this, e);
                }
                if (!e.Handled)
                {
                    this.SetChecked(true, true, true);
                }
            }
        }

        private void Radiobox_MouseDown()
        {
            if (!this.Checked)
            {
                this.IsMouseDown = true;
                this.RefreshColor(null, null);
            }
        }

        private void Radiobox_MouseLeave()
        {
            this.IsMouseDown = false;
        }

        private void RefreshColor(object obj = null, object e = null)
        {
            try
            {
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0 && !false.Equals(RuntimeHelpers.GetObjectValue(e)))
                {
                    OurRadioButton.ColorState colorType = this.ColorType;
                    if (colorType != OurRadioButton.ColorState.White)
                    {
                        if (colorType == OurRadioButton.ColorState.Highlight)
                        {
                            if (this.Checked)
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[]
                                {
                                    ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, new ModBase.OurColor(255.0, 255.0, 255.0) - this.ShapeLogo.Fill, 120, 0, null, false),
                                    ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, new ModBase.OurColor(255.0, 255.0, 255.0) - this.LabText.Foreground, 120, 0, null, false)
                                }, "OurRadioButton Checked " + Conversions.ToString(this.Uuid), false);
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, "ColorBrush3", 120, 0, null, false), "OurRadioButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else if (this.IsMouseDown)
                            {
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, "ColorBrush6", 90, 0, null, false), "OurRadioButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else if (base.IsMouseOver)
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[]
                                {
                                    ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrush3", 90, 0, null, false),
                                    ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 90, 0, null, false)
                                }, "OurRadioButton Checked " + Conversions.ToString(this.Uuid), false);
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, "ColorBrush7", 90, 0, null, false), "OurRadioButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[]
                                {
                                    ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrush3", 150, 0, null, false),
                                    ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 150, 0, null, false)
                                }, "OurRadioButton Checked " + Conversions.ToString(this.Uuid), false);
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - base.Background, 150, 0, null, false), "OurRadioButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                        }
                    }
                    else if (this.Checked)
                    {
                        ModAnimation.AniStart(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrush3", 120, 0, null, false),
                            ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 120, 0, null, false)
                        }, "OurRadioButton Checked " + Conversions.ToString(this.Uuid), false);
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, new ModBase.OurColor(255.0, 255.0, 255.0) - base.Background, 120, 0, null, false), "OurRadioButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else if (this.IsMouseDown)
                    {
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, new ModBase.OurColor(120.0, ModSecret.Color8) - base.Background, 60, 0, null, false), "OurRadioButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else if (base.IsMouseOver)
                    {
                        ModAnimation.AniStart(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, new ModBase.OurColor(255.0, 255.0, 255.0) - this.ShapeLogo.Fill, 90, 0, null, false),
                            ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, new ModBase.OurColor(255.0, 255.0, 255.0) - this.LabText.Foreground, 90, 0, null, false)
                        }, "OurRadioButton Checked " + Conversions.ToString(this.Uuid), false);
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, new ModBase.OurColor(50.0, ModSecret.Color8) - base.Background, 90, 0, null, false), "OurRadioButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else
                    {
                        ModAnimation.AniStart(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, new ModBase.OurColor(255.0, 255.0, 255.0) - this.ShapeLogo.Fill, 150, 0, null, false),
                            ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, new ModBase.OurColor(255.0, 255.0, 255.0) - this.LabText.Foreground, 150, 0, null, false)
                        }, "OurRadioButton Checked " + Conversions.ToString(this.Uuid), false);
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - base.Background, 150, 0, null, false), "OurRadioButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                }
                else
                {
                    ModAnimation.AniStop("OurRadioButton Checked " + Conversions.ToString(this.Uuid));
                    ModAnimation.AniStop("OurRadioButton Color " + Conversions.ToString(this.Uuid));
                    OurRadioButton.ColorState colorType2 = this.ColorType;
                    if (colorType2 != OurRadioButton.ColorState.White)
                    {
                        if (colorType2 == OurRadioButton.ColorState.Highlight)
                        {
                            if (this.Checked)
                            {
                                base.SetResourceReference(Border.BackgroundProperty, "ColorBrush3");
                                this.ShapeLogo.Fill = new ModBase.OurColor(255.0, 255.0, 255.0);
                                this.LabText.Foreground = new ModBase.OurColor(255.0, 255.0, 255.0);
                            }
                            else
                            {
                                base.Background = ModSecret.ColorSemiTransparent;
                                this.ShapeLogo.SetResourceReference(Shape.FillProperty, "ColorBrush3");
                                this.LabText.SetResourceReference(TextBlock.ForegroundProperty, "ColorBrush3");
                            }
                        }
                    }
                    else if (this.Checked)
                    {
                        base.Background = new ModBase.OurColor(255.0, 255.0, 255.0);
                        this.ShapeLogo.SetResourceReference(Shape.FillProperty, "ColorBrush3");
                        this.LabText.SetResourceReference(TextBlock.ForegroundProperty, "ColorBrush3");
                    }
                    else
                    {
                        base.Background = ModSecret.ColorSemiTransparent;
                        this.ShapeLogo.Fill = new ModBase.OurColor(255.0, 255.0, 255.0);
                        this.LabText.Foreground = new ModBase.OurColor(255.0, 255.0, 255.0);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "刷新单选按钮颜色出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurRadioButton.xaml.vb");
            }
        }




        [CompilerGenerated]
        private void _Lambda___R1_1(object a0, MouseButtonEventArgs a1)
        {
            this.Radiobox_MouseUp();
        }

        [CompilerGenerated]
        private void _Lambda___R1_2(object a0, MouseButtonEventArgs a1)
        {
            this.Radiobox_MouseDown();
        }

        [CompilerGenerated]
        private void _Lambda___R1_3(object a0, MouseEventArgs a1)
        {
            this.Radiobox_MouseLeave();
        }

        public int Uuid;

        [CompilerGenerated]
        private OurRadioButton.CheckEventHandler CheckEvent;

        private double _LogoScale;

        private bool _Checked;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(OurRadioButton), new PropertyMetadata(new PropertyChangedCallback(OurRadioButton._Closure___._I._Lambda___0_0)));

        private OurRadioButton.ColorState _ColorType;

        [CompilerGenerated]
        private OurRadioButton.PreviewClickEventHandler PreviewClickEvent;

        private bool IsMouseDown;

        private const int AnimationTimeOfMouseIn = 90;

        private const int AnimationTimeOfMouseOut = 150;

        private const int AnimationTimeOfCheck = 120;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurRadioButton _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("ShapeLogo")]
        private Path _ShapeLogo;

        [CompilerGenerated]
        [AccessedThroughProperty("LabText")]
        private TextBlock _LabText;

        public delegate void CheckEventHandler(object sender, bool raiseByMouse);

        public enum ColorState
        {
            White,
            Highlight
        }

        public delegate void PreviewClickEventHandler(object sender, ModBase.RouteEventArgs e);

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal void _Lambda___0_0(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                if (sender != null)
                {
                    ((OurRadioButton)sender).LabText.Text = Conversions.ToString(e.NewValue);
                }
            }

            public static readonly OurRadioButton._Closure___ _I = new OurRadioButton._Closure___();
        }
    }
}
