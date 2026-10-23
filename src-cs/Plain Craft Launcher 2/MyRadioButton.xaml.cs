using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
    public partial class MyRadioButton : Border
    {
        public MyRadioButton()
        {
            base.MouseLeftButtonUp += delegate(object a0, MouseButtonEventArgs a1)
            {
                this.Radiobox_MouseUp();
            };
            base.MouseLeftButtonDown += delegate(object a0, MouseButtonEventArgs a1)
            {
                this.Radiobox_MouseDown();
            };
            base.MouseLeave += delegate(object a0, MouseEventArgs a1)
            {
                this.Radiobox_MouseLeave();
            };
            base.MouseEnter += new MouseEventHandler(this.RefreshColor);
            base.MouseLeave += new MouseEventHandler(this.RefreshColor);
            base.Loaded += new RoutedEventHandler(this.RefreshColor);
            this.Uuid = ModBase.GetUuid();
            this._LogoScale = 1.0;
            this._Checked = false;
            this._ColorType = MyRadioButton.ColorState.White;
            this.IsMouseDown = false;
            this.InitializeComponent();
        }

        public event MyRadioButton.CheckEventHandler Check;

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
                        List<MyRadioButton> list = new List<MyRadioButton>();
                        int num = 0;
                        try
                        {
                            foreach (object obj in ((IEnumerable)NewLateBinding.LateGet(base.Parent, null, "Children", new object[0], null, null, null)))
                            {
                                object objectValue = RuntimeHelpers.GetObjectValue(obj);
                                if (objectValue is MyRadioButton)
                                {
                                    list.Add((MyRadioButton)objectValue);
                                    if (Conversions.ToBoolean(NewLateBinding.LateGet(objectValue, null, "Checked", new object[0], null, null, null)))
                                    {
                                        num++;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator enumerator;
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
                                    foreach (MyRadioButton myRadioButton in list)
                                    {
                                        if (myRadioButton.Checked && !myRadioButton.Equals(this))
                                        {
                                            myRadioButton.Checked = false;
                                        }
                                    }
                                    goto IL_016B;
                                }
                                finally
                                {
                                    List<MyRadioButton>.Enumerator enumerator2;
                                    ((IDisposable)enumerator2).Dispose();
                                }
                            }
                            bool flag2 = false;
                            try
                            {
                                foreach (MyRadioButton myRadioButton2 in list)
                                {
                                    if (myRadioButton2.Checked)
                                    {
                                        if (flag2)
                                        {
                                            myRadioButton2.Checked = false;
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
                                List<MyRadioButton>.Enumerator enumerator3;
                                ((IDisposable)enumerator3).Dispose();
                            }
                        }
                        IL_016B:
                        if (flag)
                        {
                            this.RefreshColor(null, anime);
                            if (this.Checked)
                            {
                                MyRadioButton.CheckEventHandler checkEvent = this.CheckEvent;
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
                    Logger.Error(ex, "单选按钮勾选改变错误", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyRadioButton.xaml.vb");
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
                return Conversions.ToString(base.GetValue(MyRadioButton.TextProperty));
            }
            set
            {
                base.SetValue(MyRadioButton.TextProperty, value);
            }
        }

        public MyRadioButton.ColorState ColorType
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

        public event MyRadioButton.PreviewClickEventHandler PreviewClick;

        private void Radiobox_MouseUp()
        {
            if (!this.Checked && this.IsMouseDown)
            {
                Logger.Info(string.Format("按下单选按钮：{0}", this.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyRadioButton.xaml.vb");
                this.IsMouseDown = false;
                ModBase.RouteEventArgs e = new ModBase.RouteEventArgs(true);
                MyRadioButton.PreviewClickEventHandler previewClickEvent = this.PreviewClickEvent;
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
                    MyRadioButton.ColorState colorType = this.ColorType;
                    if (colorType != MyRadioButton.ColorState.White)
                    {
                        if (colorType == MyRadioButton.ColorState.Highlight)
                        {
                            if (this.Checked)
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[]
                                {
                                    ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, new ModBase.MyColor(255.0, 255.0, 255.0) - this.ShapeLogo.Fill, 120, 0, null, false),
                                    ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, new ModBase.MyColor(255.0, 255.0, 255.0) - this.LabText.Foreground, 120, 0, null, false)
                                }, "MyRadioButton Checked " + Conversions.ToString(this.Uuid), false);
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, "ColorBrush3", 120, 0, null, false), "MyRadioButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else if (this.IsMouseDown)
                            {
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, "ColorBrush6", 90, 0, null, false), "MyRadioButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else if (base.IsMouseOver)
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[]
                                {
                                    ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrush3", 90, 0, null, false),
                                    ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 90, 0, null, false)
                                }, "MyRadioButton Checked " + Conversions.ToString(this.Uuid), false);
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, "ColorBrush7", 90, 0, null, false), "MyRadioButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[]
                                {
                                    ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrush3", 150, 0, null, false),
                                    ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 150, 0, null, false)
                                }, "MyRadioButton Checked " + Conversions.ToString(this.Uuid), false);
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - base.Background, 150, 0, null, false), "MyRadioButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                        }
                    }
                    else if (this.Checked)
                    {
                        ModAnimation.AniStart(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrush3", 120, 0, null, false),
                            ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 120, 0, null, false)
                        }, "MyRadioButton Checked " + Conversions.ToString(this.Uuid), false);
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, new ModBase.MyColor(255.0, 255.0, 255.0) - base.Background, 120, 0, null, false), "MyRadioButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else if (this.IsMouseDown)
                    {
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, new ModBase.MyColor(120.0, ModSecret.Color8) - base.Background, 60, 0, null, false), "MyRadioButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else if (base.IsMouseOver)
                    {
                        ModAnimation.AniStart(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, new ModBase.MyColor(255.0, 255.0, 255.0) - this.ShapeLogo.Fill, 90, 0, null, false),
                            ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, new ModBase.MyColor(255.0, 255.0, 255.0) - this.LabText.Foreground, 90, 0, null, false)
                        }, "MyRadioButton Checked " + Conversions.ToString(this.Uuid), false);
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, new ModBase.MyColor(50.0, ModSecret.Color8) - base.Background, 90, 0, null, false), "MyRadioButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else
                    {
                        ModAnimation.AniStart(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, new ModBase.MyColor(255.0, 255.0, 255.0) - this.ShapeLogo.Fill, 150, 0, null, false),
                            ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, new ModBase.MyColor(255.0, 255.0, 255.0) - this.LabText.Foreground, 150, 0, null, false)
                        }, "MyRadioButton Checked " + Conversions.ToString(this.Uuid), false);
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - base.Background, 150, 0, null, false), "MyRadioButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                }
                else
                {
                    ModAnimation.AniStop("MyRadioButton Checked " + Conversions.ToString(this.Uuid));
                    ModAnimation.AniStop("MyRadioButton Color " + Conversions.ToString(this.Uuid));
                    MyRadioButton.ColorState colorType2 = this.ColorType;
                    if (colorType2 != MyRadioButton.ColorState.White)
                    {
                        if (colorType2 == MyRadioButton.ColorState.Highlight)
                        {
                            if (this.Checked)
                            {
                                base.SetResourceReference(Border.BackgroundProperty, "ColorBrush3");
                                this.ShapeLogo.Fill = new ModBase.MyColor(255.0, 255.0, 255.0);
                                this.LabText.Foreground = new ModBase.MyColor(255.0, 255.0, 255.0);
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
                        base.Background = new ModBase.MyColor(255.0, 255.0, 255.0);
                        this.ShapeLogo.SetResourceReference(Shape.FillProperty, "ColorBrush3");
                        this.LabText.SetResourceReference(TextBlock.ForegroundProperty, "ColorBrush3");
                    }
                    else
                    {
                        base.Background = ModSecret.ColorSemiTransparent;
                        this.ShapeLogo.Fill = new ModBase.MyColor(255.0, 255.0, 255.0);
                        this.LabText.Foreground = new ModBase.MyColor(255.0, 255.0, 255.0);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "刷新单选按钮颜色出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyRadioButton.xaml.vb");
            }
        }

        internal virtual MyRadioButton PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Path ShapeLogo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabText
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public int Uuid;

        private double _LogoScale;

        private bool _Checked;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MyRadioButton), new PropertyMetadata(delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender != null)
            {
                ((MyRadioButton)sender).LabText.Text = Conversions.ToString(e.NewValue);
            }
        }));

        private MyRadioButton.ColorState _ColorType;

        private bool IsMouseDown;

        private const int AnimationTimeOfMouseIn = 90;

        private const int AnimationTimeOfMouseOut = 150;

        private const int AnimationTimeOfCheck = 120;

        public delegate void CheckEventHandler(object sender, bool raiseByMouse);

        public enum ColorState
        {
            White,
            Highlight
        }

        public delegate void PreviewClickEventHandler(object sender, ModBase.RouteEventArgs e);
    }
}
