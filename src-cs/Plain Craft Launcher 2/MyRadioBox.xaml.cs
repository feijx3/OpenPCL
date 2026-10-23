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
using System.Windows.Shapes;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [ContentProperty("Inlines")]
    [DesignerGenerated]
    public partial class MyRadioBox : Grid, IMyRadio, ISettingControl
    {
        public MyRadioBox()
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
            base.IsEnabledChanged += delegate(object a0, DependencyPropertyChangedEventArgs a1)
            {
                this.Radiobox_IsEnabledChanged();
            };
            base.MouseEnter += delegate(object a0, MouseEventArgs a1)
            {
                this.Radiobox_MouseEnterAnimation();
            };
            base.MouseLeave += delegate(object a0, MouseEventArgs a1)
            {
                this.Radiobox_MouseLeaveAnimation();
            };
            this.Check += delegate(object a0, ModBase.RouteEventArgs a1)
            {
                this.SaveSetting();
            };
            this.Uuid = ModBase.GetUuid();
            this.MouseDowned = false;
            this.AllowMouseDown = true;
            this.InitializeComponent();
        }

        public event MyRadioBox.PreviewCheckEventHandler PreviewCheck;

        public event MyRadioBox.PreviewChangeEventHandler PreviewChange;

        public event IMyRadio.CheckEventHandler Check;

        public event IMyRadio.ChangedEventHandler Changed;

        public bool Checked
        {
            get
            {
                return Conversions.ToBoolean(base.GetValue(MyRadioBox.CheckedProperty));
            }
            set
            {
                this.SetChecked(value, false);
            }
        }

        public void SetChecked(bool value, bool user)
        {
            checked
            {
                try
                {
                    if (value && user)
                    {
                        ModBase.RouteEventArgs e = new ModBase.RouteEventArgs(user);
                        MyRadioBox.PreviewCheckEventHandler previewCheckEvent = this.PreviewCheckEvent;
                        if (previewCheckEvent != null)
                        {
                            previewCheckEvent(this, e);
                        }
                        if (e.Handled)
                        {
                            this.Radiobox_MouseLeave();
                            return;
                        }
                    }
                    bool flag = false;
                    if (base.IsLoaded && value != this.Checked)
                    {
                        MyRadioBox.PreviewChangeEventHandler previewChangeEvent = this.PreviewChangeEvent;
                        if (previewChangeEvent != null)
                        {
                            previewChangeEvent(this, new ModBase.RouteEventArgs(user));
                        }
                    }
                    if (value != this.Checked)
                    {
                        base.SetValue(MyRadioBox.CheckedProperty, value);
                        flag = true;
                    }
                    if (base.Parent != null)
                    {
                        List<MyRadioBox> list = new List<MyRadioBox>();
                        int num = 0;
                        try
                        {
                            foreach (object obj in ((IEnumerable)NewLateBinding.LateGet(base.Parent, null, "Children", new object[0], null, null, null)))
                            {
                                object objectValue = RuntimeHelpers.GetObjectValue(obj);
                                if (objectValue is MyRadioBox)
                                {
                                    list.Add((MyRadioBox)objectValue);
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
                        if (num > 1)
                        {
                            if (this.Checked)
                            {
                                try
                                {
                                    foreach (MyRadioBox myRadioBox in list)
                                    {
                                        if (myRadioBox.Checked && !myRadioBox.Equals(this))
                                        {
                                            myRadioBox.Checked = false;
                                        }
                                    }
                                    goto IL_01C0;
                                }
                                finally
                                {
                                    List<MyRadioBox>.Enumerator enumerator2;
                                    ((IDisposable)enumerator2).Dispose();
                                }
                            }
                            bool flag2 = false;
                            try
                            {
                                foreach (MyRadioBox myRadioBox2 in list)
                                {
                                    if (myRadioBox2.Checked)
                                    {
                                        if (flag2)
                                        {
                                            myRadioBox2.Checked = false;
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
                                List<MyRadioBox>.Enumerator enumerator3;
                                ((IDisposable)enumerator3).Dispose();
                            }
                        }
                        IL_01C0:
                        if (flag)
                        {
                            if (this.Checked)
                            {
                                IMyRadio.CheckEventHandler checkEvent = this.CheckEvent;
                                if (checkEvent != null)
                                {
                                    checkEvent(this, new ModBase.RouteEventArgs(user));
                                }
                            }
                            IMyRadio.ChangedEventHandler changedEvent = this.ChangedEvent;
                            if (changedEvent != null)
                            {
                                changedEvent(this, new ModBase.RouteEventArgs(user));
                            }
                            this.RaiseCustomEvent();
                            this.SyncUI();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "单选框勾选改变错误", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyRadioBox.xaml.vb");
                }
            }
        }

        private void SyncUI()
        {
            if (ModAnimation.AniControlEnabled == 0 && base.IsLoaded)
            {
                if (this.Checked)
                {
                    if (this.ShapeDot.Opacity < 0.01)
                    {
                        this.ShapeDot.Opacity = 1.0;
                    }
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaScale(this.ShapeBorder, 10.0 - this.ShapeBorder.Width, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false, true),
                        ModAnimation.AaScale(this.ShapeBorder, 8.0, 300, 90, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false, true)
                    }, "MyRadioBox Border " + Conversions.ToString(this.Uuid), false);
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaScale(this.ShapeDot, 9.0 - this.ShapeDot.Width, 390, 0, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false, true),
                        ModAnimation.AaOpacity(this.ShapeDot, 1.0 - this.ShapeDot.Opacity, 75, 90, null, false)
                    }, "MyRadioBox Dot " + Conversions.ToString(this.Uuid), false);
                    ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.ShapeBorder, Shape.StrokeProperty, base.IsMouseOver ? "ColorBrush3" : (base.IsEnabled ? "ColorBrush2" : "ColorBrushGray5"), 150, 0, null, false) }, "MyRadioBox BorderColor " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScale(this.ShapeBorder, 18.0 - this.ShapeBorder.Width, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false, true) }, "MyRadioBox Border " + Conversions.ToString(this.Uuid), false);
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaScale(this.ShapeDot, -this.ShapeDot.Width, 150, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false, true),
                    ModAnimation.AaOpacity(this.ShapeDot, -this.ShapeDot.Opacity, 75, 30, null, false)
                }, "MyRadioBox Dot " + Conversions.ToString(this.Uuid), false);
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.ShapeBorder, Shape.StrokeProperty, base.IsMouseOver ? "ColorBrush3" : (base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5"), 150, 0, null, false) }, "MyRadioBox BorderColor " + Conversions.ToString(this.Uuid), false);
                return;
            }
            else
            {
                ModAnimation.AniStop("MyRadioBox Border " + Conversions.ToString(this.Uuid));
                ModAnimation.AniStop("MyRadioBox Dot " + Conversions.ToString(this.Uuid));
                ModAnimation.AniStop("MyRadioBox BorderColor " + Conversions.ToString(this.Uuid));
                if (this.Checked)
                {
                    this.ShapeDot.Width = 9.0;
                    this.ShapeDot.Height = 9.0;
                    this.ShapeDot.Opacity = 1.0;
                    this.ShapeDot.Margin = new Thickness(5.5, 0.0, 0.0, 0.0);
                    this.ShapeBorder.SetResourceReference(Shape.StrokeProperty, base.IsEnabled ? "ColorBrush2" : "ColorBrushGray5");
                    return;
                }
                this.ShapeDot.Width = 0.0;
                this.ShapeDot.Height = 0.0;
                this.ShapeDot.Opacity = 0.0;
                this.ShapeDot.Margin = new Thickness(10.0, 0.0, 0.0, 0.0);
                this.ShapeBorder.SetResourceReference(Shape.StrokeProperty, base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5");
                return;
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
                return Conversions.ToString(base.GetValue(MyRadioBox.TextProperty));
            }
            set
            {
                base.SetValue(MyRadioBox.TextProperty, value);
            }
        }

        private void Radiobox_MouseUp()
        {
            if (this.MouseDowned)
            {
                Logger.Info(string.Format("按下单选框：{0}", this.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyRadioBox.xaml.vb");
                this.SetChecked(true, true);
                this.MouseDowned = false;
                ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Shape.FillProperty, "ColorBrushHalfWhite", 100, 0, null, false), "MyRadioBox Background " + Conversions.ToString(this.Uuid), false);
            }
        }

        private void Radiobox_MouseDown()
        {
            this.MouseDowned = true;
            base.Focus();
            ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Shape.FillProperty, "ColorBrushBg1", 100, 0, null, false), "MyRadioBox Background " + Conversions.ToString(this.Uuid), false);
            if (!this.Checked)
            {
                ModAnimation.AniStart(ModAnimation.AaScale(this.ShapeBorder, 16.5 - this.ShapeBorder.Width, 1000, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false, true), "MyRadioBox Border " + Conversions.ToString(this.Uuid), false);
            }
        }

        private void Radiobox_MouseLeave()
        {
            if (this.MouseDowned)
            {
                this.MouseDowned = false;
                ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Shape.FillProperty, "ColorBrushHalfWhite", 100, 0, null, false), "MyRadioBox Background " + Conversions.ToString(this.Uuid), false);
                if (!this.Checked)
                {
                    ModAnimation.AniStart(ModAnimation.AaScale(this.ShapeBorder, 18.0 - this.ShapeBorder.Width, 400, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false, true), "MyRadioBox Border " + Conversions.ToString(this.Uuid), false);
                }
            }
        }

        private void Radiobox_IsEnabledChanged()
        {
            if (!base.IsLoaded || ModAnimation.AniControlEnabled != 0)
            {
                ModAnimation.AniStop("MyRadioBox BorderColor " + Conversions.ToString(this.Uuid));
                ModAnimation.AniStop("MyRadioBox TextColor " + Conversions.ToString(this.Uuid));
                this.LabText.SetResourceReference(TextBlock.ForegroundProperty, base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5");
                this.ShapeBorder.SetResourceReference(Shape.StrokeProperty, base.IsEnabled ? (this.Checked ? "ColorBrush2" : "ColorBrush1") : "ColorBrushGray5");
                return;
            }
            if (base.IsEnabled)
            {
                this.Radiobox_MouseLeaveAnimation();
                return;
            }
            ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Shape.StrokeProperty, ModSecret.ColorGray5 - this.ShapeBorder.Stroke, 200, 0, null, false), "MyRadioBox BorderColor " + Conversions.ToString(this.Uuid), false);
            ModAnimation.AniStart(ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, ModSecret.ColorGray5 - this.LabText.Foreground, 200, 0, null, false), "MyRadioBox TextColor " + Conversions.ToString(this.Uuid), false);
        }

        private void Radiobox_MouseEnterAnimation()
        {
            ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Shape.StrokeProperty, "ColorBrush3", 100, 0, null, false), "MyRadioBox BorderColor " + Conversions.ToString(this.Uuid), false);
            ModAnimation.AniStart(ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 100, 0, null, false), "MyRadioBox TextColor " + Conversions.ToString(this.Uuid), false);
        }

        private void Radiobox_MouseLeaveAnimation()
        {
            if (base.IsEnabled)
            {
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                {
                    ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Shape.StrokeProperty, base.IsEnabled ? (this.Checked ? "ColorBrush2" : "ColorBrush1") : "ColorBrushGray5", 200, 0, null, false), "MyRadioBox BorderColor " + Conversions.ToString(this.Uuid), false);
                    ModAnimation.AniStart(ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5", 200, 0, null, false), "MyRadioBox TextColor " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStop("MyRadioBox BorderColor " + Conversions.ToString(this.Uuid));
                ModAnimation.AniStop("MyRadioBox TextColor " + Conversions.ToString(this.Uuid));
                this.ShapeBorder.SetResourceReference(Shape.StrokeProperty, base.IsEnabled ? (this.Checked ? "ColorBrush2" : "ColorBrush1") : "ColorBrushGray5");
                this.LabText.SetResourceReference(TextBlock.ForegroundProperty, base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5");
            }
        }

        void ISettingControl.RefreshSetting(string NewValue)
        {
            this.Checked = Operators.CompareString(NewValue, SettingService.GetValue(this), false) == 0;
        }

        string ISettingControl.GetCurrentSetting()
        {
            if (!this.Checked)
            {
                return null;
            }
            return SettingService.GetValue(this);
        }

        private void SaveSetting()
        {
            SettingService.SaveSetting(this);
        }

        internal virtual MyRadioBox PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabText
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Ellipse ShapeBorder
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Ellipse ShapeDot
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public int Uuid;

        public static readonly DependencyProperty CheckedProperty = DependencyProperty.Register("Checked", typeof(bool), typeof(MyRadioBox), new PropertyMetadata(false, delegate(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
        {
            ((MyRadioBox._Closure$__.$I0-0 == null) ? (MyRadioBox._Closure$__.$I0-0 = delegate(MyRadioBox d, DependencyPropertyChangedEventArgs e)
            {
                if (!d.IsLoaded)
                {
                    d.SyncUI();
                }
            }) : MyRadioBox._Closure$__.$I0-0)((MyRadioBox)a0, a1);
        }));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MyRadioBox), new PropertyMetadata(delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender != null)
            {
                ((MyRadioBox)sender).LabText.Text = Conversions.ToString(e.NewValue);
            }
        }));

        private bool MouseDowned;

        private bool AllowMouseDown;

        private const int AnimationTimeOfMouseIn = 100;

        private const int AnimationTimeOfMouseOut = 200;

        private const int AnimationTimeOfCheck = 150;

        public delegate void PreviewCheckEventHandler(object sender, ModBase.RouteEventArgs e);

        public delegate void PreviewChangeEventHandler(object sender, ModBase.RouteEventArgs e);
    }
}
