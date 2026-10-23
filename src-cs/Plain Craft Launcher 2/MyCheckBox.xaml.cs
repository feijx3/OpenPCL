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
using System.Windows.Media;
using System.Windows.Shapes;
using MeloongCore;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [ContentProperty("Inlines")]
    [DesignerGenerated]
    public partial class MyCheckBox : Grid, ISettingControl
    {
        public MyCheckBox()
        {
            base.MouseLeftButtonUp += delegate(object a0, MouseButtonEventArgs a1)
            {
                this.Checkbox_MouseUp();
            };
            base.MouseLeftButtonDown += delegate(object a0, MouseButtonEventArgs a1)
            {
                this.Checkbox_MouseDown();
            };
            base.MouseLeave += delegate(object a0, MouseEventArgs a1)
            {
                this.Checkbox_MouseLeave();
            };
            base.IsEnabledChanged += delegate(object a0, DependencyPropertyChangedEventArgs a1)
            {
                this.Checkbox_IsEnabledChanged();
            };
            base.MouseEnter += delegate(object a0, MouseEventArgs a1)
            {
                this.Checkbox_MouseEnterAnimation();
            };
            base.MouseLeave += delegate(object a0, MouseEventArgs a1)
            {
                this.Checkbox_MouseLeaveAnimation();
            };
            this.Change += delegate(object a0, bool a1)
            {
                this.SaveSetting();
            };
            this.Uuid = ModBase.GetUuid();
            this.MouseDowned = false;
            this.AllowMouseDown = true;
            this.InitializeComponent();
        }

        public event MyCheckBox.ChangeEventHandler Change;

        public event MyCheckBox.PreviewChangeEventHandler PreviewChange;

        public bool Checked
        {
            get
            {
                return Conversions.ToBoolean(base.GetValue(MyCheckBox.CheckedProperty));
            }
            set
            {
                this.SetChecked(value, false);
            }
        }

        public void SetChecked(bool value, bool user)
        {
            try
            {
                if (value != this.Checked)
                {
                    if (value && user)
                    {
                        ModBase.RouteEventArgs e = new ModBase.RouteEventArgs(user);
                        MyCheckBox.PreviewChangeEventHandler previewChangeEvent = this.PreviewChangeEvent;
                        if (previewChangeEvent != null)
                        {
                            previewChangeEvent(this, e);
                        }
                        if (e.Handled)
                        {
                            this.MouseDowned = true;
                            this.Checkbox_MouseLeave();
                            this.MouseDowned = false;
                            return;
                        }
                    }
                    base.SetValue(MyCheckBox.CheckedProperty, value);
                    if (base.IsLoaded)
                    {
                        MyCheckBox.ChangeEventHandler changeEvent = this.ChangeEvent;
                        if (changeEvent != null)
                        {
                            changeEvent(this, user);
                        }
                    }
                    this.SyncUI();
                    this.RaiseCustomEvent();
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "设置 Checked 失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyCheckBox.xaml.vb");
            }
        }

        private void SyncUI()
        {
            if (ModAnimation.AniControlEnabled == 0 && base.IsLoaded)
            {
                this.AllowMouseDown = false;
                if (this.Checked)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaScale(this.ShapeBorder, 12.0 - this.ShapeBorder.Width, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false, true),
                        ModAnimation.AaScaleTransform(this.ShapeCheck, 1.0 - ((ScaleTransform)this.ShapeCheck.RenderTransform).ScaleX, 300, 105, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false),
                        ModAnimation.AaScale(this.ShapeBorder, 6.0, 300, 105, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false, true)
                    }, "MyCheckBox Scale " + Conversions.ToString(this.Uuid), false);
                    ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.ShapeBorder, Border.BorderBrushProperty, base.IsEnabled ? (base.IsMouseOver ? "ColorBrush3" : "ColorBrush2") : "ColorBrushGray5", 150, 0, null, false) }, "MyCheckBox BorderColor " + Conversions.ToString(this.Uuid), false);
                    ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaCode(delegate
                    {
                        this.AllowMouseDown = true;
                    }, 300, false) }, "MyCheckBox AllowMouseDown " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaScale(this.ShapeBorder, 12.0 - this.ShapeBorder.Width, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false, true),
                    ModAnimation.AaScaleTransform(this.ShapeCheck, -((ScaleTransform)this.ShapeCheck.RenderTransform).ScaleX, 135, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), false),
                    ModAnimation.AaScale(this.ShapeBorder, 6.0, 300, 105, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false, true)
                }, "MyCheckBox Scale " + Conversions.ToString(this.Uuid), false);
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.ShapeBorder, Border.BorderBrushProperty, base.IsEnabled ? (base.IsMouseOver ? "ColorBrush3" : "ColorBrush1") : "ColorBrushGray5", 150, 0, null, false) }, "MyCheckBox BorderColor " + Conversions.ToString(this.Uuid), false);
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaCode(delegate
                {
                    this.AllowMouseDown = true;
                }, 300, false) }, "MyCheckBox AllowMouseDown " + Conversions.ToString(this.Uuid), false);
                return;
            }
            else
            {
                ModAnimation.AniStop("MyCheckBox Scale " + Conversions.ToString(this.Uuid));
                ModAnimation.AniStop("MyCheckBox BorderColor " + Conversions.ToString(this.Uuid));
                ModAnimation.AniStop("MyCheckBox AllowMouseDown " + Conversions.ToString(this.Uuid));
                if (this.Checked)
                {
                    ((ScaleTransform)this.ShapeCheck.RenderTransform).ScaleX = 1.0;
                    ((ScaleTransform)this.ShapeCheck.RenderTransform).ScaleY = 1.0;
                    this.ShapeBorder.SetResourceReference(Border.BorderBrushProperty, base.IsEnabled ? "ColorBrush2" : "ColorBrushGray5");
                    return;
                }
                ((ScaleTransform)this.ShapeCheck.RenderTransform).ScaleX = 0.0;
                ((ScaleTransform)this.ShapeCheck.RenderTransform).ScaleY = 0.0;
                this.ShapeBorder.SetResourceReference(Border.BorderBrushProperty, base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5");
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
                return Conversions.ToString(base.GetValue(MyCheckBox.TextProperty));
            }
            set
            {
                base.SetValue(MyCheckBox.TextProperty, value);
            }
        }

        private void Checkbox_MouseUp()
        {
            if (this.MouseDowned)
            {
                Logger.Info(string.Format("按下复选框（{0}）：{1}", !this.Checked, this.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyCheckBox.xaml.vb");
                this.MouseDowned = false;
                this.SetChecked(!this.Checked, true);
                ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Border.BackgroundProperty, "ColorBrushHalfWhite", 100, 0, null, false), "MyCheckBox Background " + Conversions.ToString(this.Uuid), false);
            }
        }

        private void Checkbox_MouseDown()
        {
            if (this.AllowMouseDown)
            {
                this.MouseDowned = true;
                base.Focus();
                ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Border.BackgroundProperty, "ColorBrushBg1", 100, 0, null, false), "MyCheckBox Background " + Conversions.ToString(this.Uuid), false);
                if (this.Checked)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaScale(this.ShapeBorder, 16.5 - this.ShapeBorder.Width, 1000, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false, true),
                        ModAnimation.AaScaleTransform(this.ShapeCheck, 0.9 - ((ScaleTransform)this.ShapeCheck.RenderTransform).ScaleX, 1000, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false)
                    }, "MyCheckBox Scale " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStart(ModAnimation.AaScale(this.ShapeBorder, 16.5 - this.ShapeBorder.Width, 1000, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false, true), "MyCheckBox Scale " + Conversions.ToString(this.Uuid), false);
            }
        }

        private void Checkbox_MouseLeave()
        {
            if (this.MouseDowned)
            {
                this.MouseDowned = false;
                ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Border.BackgroundProperty, "ColorBrushHalfWhite", 100, 0, null, false), "MyCheckBox Background " + Conversions.ToString(this.Uuid), false);
                if (this.Checked)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaScale(this.ShapeBorder, 18.0 - this.ShapeBorder.Width, 400, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false, true),
                        ModAnimation.AaScaleTransform(this.ShapeCheck, 1.0 - ((ScaleTransform)this.ShapeCheck.RenderTransform).ScaleX, 500, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false)
                    }, "MyCheckBox Scale " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStart(ModAnimation.AaScale(this.ShapeBorder, 18.0 - this.ShapeBorder.Width, 400, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false, true), "MyCheckBox Scale " + Conversions.ToString(this.Uuid), false);
            }
        }

        private void Checkbox_IsEnabledChanged()
        {
            if (!base.IsLoaded || ModAnimation.AniControlEnabled != 0)
            {
                ModAnimation.AniStop("MyCheckBox TextColor " + Conversions.ToString(this.Uuid));
                ModAnimation.AniStop("MyCheckBox BorderColor " + Conversions.ToString(this.Uuid));
                this.LabText.SetResourceReference(TextBlock.ForegroundProperty, base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5");
                this.ShapeBorder.SetResourceReference(Border.BorderBrushProperty, base.IsEnabled ? (this.Checked ? "ColorBrush2" : "ColorBrush1") : "ColorBrushGray5");
                return;
            }
            if (base.IsEnabled)
            {
                this.Checkbox_MouseLeaveAnimation();
                return;
            }
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.ShapeBorder, Border.BorderBrushProperty, ModSecret.ColorGray5 - this.ShapeBorder.BorderBrush, 200, 0, null, false) }, "MyCheckBox BorderColor " + Conversions.ToString(this.Uuid), false);
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, ModSecret.ColorGray5 - this.LabText.Foreground, 200, 0, null, false) }, "MyCheckBox TextColor " + Conversions.ToString(this.Uuid), false);
        }

        private void Checkbox_MouseEnterAnimation()
        {
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 100, 0, null, false) }, "MyCheckBox TextColor " + Conversions.ToString(this.Uuid), false);
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.ShapeBorder, Border.BorderBrushProperty, "ColorBrush3", 100, 0, null, false) }, "MyCheckBox BorderColor " + Conversions.ToString(this.Uuid), false);
        }

        private void Checkbox_MouseLeaveAnimation()
        {
            if (base.IsEnabled)
            {
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5", 200, 0, null, false) }, "MyCheckBox TextColor " + Conversions.ToString(this.Uuid), false);
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.ShapeBorder, Border.BorderBrushProperty, base.IsEnabled ? (this.Checked ? "ColorBrush2" : "ColorBrush1") : "ColorBrushGray5", 200, 0, null, false) }, "MyCheckBox BorderColor " + Conversions.ToString(this.Uuid), false);
            }
        }

        void ISettingControl.RefreshSetting(string NewValue)
        {
            this.Checked = bool.Parse(NewValue);
        }

        string ISettingControl.GetCurrentSetting()
        {
            return this.Checked.ToString();
        }

        private void SaveSetting()
        {
            SettingService.SaveSetting(this);
        }

        internal virtual MyCheckBox PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public virtual TextBlock LabText
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Border ShapeBorder
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Path ShapeCheck
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public int Uuid;

        public static readonly DependencyProperty CheckedProperty = DependencyProperty.Register("Checked", typeof(bool), typeof(MyCheckBox), new PropertyMetadata(false, delegate(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
        {
            ((MyCheckBox._Closure$__.$I0-0 == null) ? (MyCheckBox._Closure$__.$I0-0 = delegate(MyCheckBox d, DependencyPropertyChangedEventArgs e)
            {
                if (!d.IsLoaded)
                {
                    d.SyncUI();
                }
            }) : MyCheckBox._Closure$__.$I0-0)((MyCheckBox)a0, a1);
        }));

        private const int AnimationTimeOfCheck = 150;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MyCheckBox), new PropertyMetadata(delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!Information.IsNothing(sender))
            {
                ((MyCheckBox)sender).LabText.Text = Conversions.ToString(e.NewValue);
            }
        }));

        private bool MouseDowned;

        private bool AllowMouseDown;

        private const int AnimationTimeOfMouseIn = 100;

        private const int AnimationTimeOfMouseOut = 200;

        public delegate void ChangeEventHandler(object sender, bool user);

        public delegate void PreviewChangeEventHandler(object sender, ModBase.RouteEventArgs e);
    }
}
