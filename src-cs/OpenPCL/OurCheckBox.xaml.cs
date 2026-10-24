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
    public partial class OurCheckBox : Grid, ISettingControl
    {
        public OurCheckBox()
        {
            base.MouseLeftButtonUp += this._Lambda___R1_2;
            base.MouseLeftButtonDown += this._Lambda___R1_3;
            base.MouseLeave += this._Lambda___R1_4;
            base.IsEnabledChanged += this._Lambda___R1_5;
            base.MouseEnter += this._Lambda___R1_6;
            base.MouseLeave += this._Lambda___R1_7;
            this.Change += this._Lambda___R1_8;
            this.Uuid = ModBase.GetUuid();
            this.MouseDowned = false;
            this.AllowMouseDown = true;
            this.InitializeComponent();
        }

        public event OurCheckBox.ChangeEventHandler Change
        {
            [CompilerGenerated]
            add
            {
                OurCheckBox.ChangeEventHandler changeEventHandler = this.ChangeEvent;
                OurCheckBox.ChangeEventHandler changeEventHandler2;
                do
                {
                    changeEventHandler2 = changeEventHandler;
                    OurCheckBox.ChangeEventHandler changeEventHandler3 = (OurCheckBox.ChangeEventHandler)Delegate.Combine(changeEventHandler2, value);
                    changeEventHandler = Interlocked.CompareExchange<OurCheckBox.ChangeEventHandler>(ref this.ChangeEvent, changeEventHandler3, changeEventHandler2);
                }
                while (changeEventHandler != changeEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurCheckBox.ChangeEventHandler changeEventHandler = this.ChangeEvent;
                OurCheckBox.ChangeEventHandler changeEventHandler2;
                do
                {
                    changeEventHandler2 = changeEventHandler;
                    OurCheckBox.ChangeEventHandler changeEventHandler3 = (OurCheckBox.ChangeEventHandler)Delegate.Remove(changeEventHandler2, value);
                    changeEventHandler = Interlocked.CompareExchange<OurCheckBox.ChangeEventHandler>(ref this.ChangeEvent, changeEventHandler3, changeEventHandler2);
                }
                while (changeEventHandler != changeEventHandler2);
            }
        }

        public event OurCheckBox.PreviewChangeEventHandler PreviewChange
        {
            [CompilerGenerated]
            add
            {
                OurCheckBox.PreviewChangeEventHandler previewChangeEventHandler = this.PreviewChangeEvent;
                OurCheckBox.PreviewChangeEventHandler previewChangeEventHandler2;
                do
                {
                    previewChangeEventHandler2 = previewChangeEventHandler;
                    OurCheckBox.PreviewChangeEventHandler previewChangeEventHandler3 = (OurCheckBox.PreviewChangeEventHandler)Delegate.Combine(previewChangeEventHandler2, value);
                    previewChangeEventHandler = Interlocked.CompareExchange<OurCheckBox.PreviewChangeEventHandler>(ref this.PreviewChangeEvent, previewChangeEventHandler3, previewChangeEventHandler2);
                }
                while (previewChangeEventHandler != previewChangeEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurCheckBox.PreviewChangeEventHandler previewChangeEventHandler = this.PreviewChangeEvent;
                OurCheckBox.PreviewChangeEventHandler previewChangeEventHandler2;
                do
                {
                    previewChangeEventHandler2 = previewChangeEventHandler;
                    OurCheckBox.PreviewChangeEventHandler previewChangeEventHandler3 = (OurCheckBox.PreviewChangeEventHandler)Delegate.Remove(previewChangeEventHandler2, value);
                    previewChangeEventHandler = Interlocked.CompareExchange<OurCheckBox.PreviewChangeEventHandler>(ref this.PreviewChangeEvent, previewChangeEventHandler3, previewChangeEventHandler2);
                }
                while (previewChangeEventHandler != previewChangeEventHandler2);
            }
        }

        public bool Checked
        {
            get
            {
                return Conversions.ToBoolean(base.GetValue(OurCheckBox.CheckedProperty));
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
                        OurCheckBox.PreviewChangeEventHandler previewChangeEvent = this.PreviewChangeEvent;
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
                    base.SetValue(OurCheckBox.CheckedProperty, value);
                    if (base.IsLoaded)
                    {
                        OurCheckBox.ChangeEventHandler changeEvent = this.ChangeEvent;
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
                Logger.Warn(ex, "设置 Checked 失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurCheckBox.xaml.vb");
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
                    }, "OurCheckBox Scale " + Conversions.ToString(this.Uuid), false);
                    ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.ShapeBorder, Border.BorderBrushProperty, base.IsEnabled ? (base.IsMouseOver ? "ColorBrush3" : "ColorBrush2") : "ColorBrushGray5", 150, 0, null, false) }, "OurCheckBox BorderColor " + Conversions.ToString(this.Uuid), false);
                    ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaCode(new ThreadStart(this._Lambda___19_0), 300, false) }, "OurCheckBox AllowMouseDown " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaScale(this.ShapeBorder, 12.0 - this.ShapeBorder.Width, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false, true),
                    ModAnimation.AaScaleTransform(this.ShapeCheck, -((ScaleTransform)this.ShapeCheck.RenderTransform).ScaleX, 135, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), false),
                    ModAnimation.AaScale(this.ShapeBorder, 6.0, 300, 105, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false, true)
                }, "OurCheckBox Scale " + Conversions.ToString(this.Uuid), false);
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.ShapeBorder, Border.BorderBrushProperty, base.IsEnabled ? (base.IsMouseOver ? "ColorBrush3" : "ColorBrush1") : "ColorBrushGray5", 150, 0, null, false) }, "OurCheckBox BorderColor " + Conversions.ToString(this.Uuid), false);
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaCode(new ThreadStart(this._Lambda___19_1), 300, false) }, "OurCheckBox AllowMouseDown " + Conversions.ToString(this.Uuid), false);
                return;
            }
            else
            {
                ModAnimation.AniStop("OurCheckBox Scale " + Conversions.ToString(this.Uuid));
                ModAnimation.AniStop("OurCheckBox BorderColor " + Conversions.ToString(this.Uuid));
                ModAnimation.AniStop("OurCheckBox AllowMouseDown " + Conversions.ToString(this.Uuid));
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
                return Conversions.ToString(base.GetValue(OurCheckBox.TextProperty));
            }
            set
            {
                base.SetValue(OurCheckBox.TextProperty, value);
            }
        }

        private void Checkbox_MouseUp()
        {
            if (this.MouseDowned)
            {
                Logger.Info(string.Format("按下复选框（{0}）：{1}", !this.Checked, this.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurCheckBox.xaml.vb");
                this.MouseDowned = false;
                this.SetChecked(!this.Checked, true);
                ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Border.BackgroundProperty, "ColorBrushHalfWhite", 100, 0, null, false), "OurCheckBox Background " + Conversions.ToString(this.Uuid), false);
            }
        }

        private void Checkbox_MouseDown()
        {
            if (this.AllowMouseDown)
            {
                this.MouseDowned = true;
                base.Focus();
                ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Border.BackgroundProperty, "ColorBrushBg1", 100, 0, null, false), "OurCheckBox Background " + Conversions.ToString(this.Uuid), false);
                if (this.Checked)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaScale(this.ShapeBorder, 16.5 - this.ShapeBorder.Width, 1000, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false, true),
                        ModAnimation.AaScaleTransform(this.ShapeCheck, 0.9 - ((ScaleTransform)this.ShapeCheck.RenderTransform).ScaleX, 1000, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false)
                    }, "OurCheckBox Scale " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStart(ModAnimation.AaScale(this.ShapeBorder, 16.5 - this.ShapeBorder.Width, 1000, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false, true), "OurCheckBox Scale " + Conversions.ToString(this.Uuid), false);
            }
        }

        private void Checkbox_MouseLeave()
        {
            if (this.MouseDowned)
            {
                this.MouseDowned = false;
                ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Border.BackgroundProperty, "ColorBrushHalfWhite", 100, 0, null, false), "OurCheckBox Background " + Conversions.ToString(this.Uuid), false);
                if (this.Checked)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaScale(this.ShapeBorder, 18.0 - this.ShapeBorder.Width, 400, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false, true),
                        ModAnimation.AaScaleTransform(this.ShapeCheck, 1.0 - ((ScaleTransform)this.ShapeCheck.RenderTransform).ScaleX, 500, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false)
                    }, "OurCheckBox Scale " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStart(ModAnimation.AaScale(this.ShapeBorder, 18.0 - this.ShapeBorder.Width, 400, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false, true), "OurCheckBox Scale " + Conversions.ToString(this.Uuid), false);
            }
        }

        private void Checkbox_IsEnabledChanged()
        {
            if (!base.IsLoaded || ModAnimation.AniControlEnabled != 0)
            {
                ModAnimation.AniStop("OurCheckBox TextColor " + Conversions.ToString(this.Uuid));
                ModAnimation.AniStop("OurCheckBox BorderColor " + Conversions.ToString(this.Uuid));
                this.LabText.SetResourceReference(TextBlock.ForegroundProperty, base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5");
                this.ShapeBorder.SetResourceReference(Border.BorderBrushProperty, base.IsEnabled ? (this.Checked ? "ColorBrush2" : "ColorBrush1") : "ColorBrushGray5");
                return;
            }
            if (base.IsEnabled)
            {
                this.Checkbox_MouseLeaveAnimation();
                return;
            }
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.ShapeBorder, Border.BorderBrushProperty, ModSecret.ColorGray5 - this.ShapeBorder.BorderBrush, 200, 0, null, false) }, "OurCheckBox BorderColor " + Conversions.ToString(this.Uuid), false);
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, ModSecret.ColorGray5 - this.LabText.Foreground, 200, 0, null, false) }, "OurCheckBox TextColor " + Conversions.ToString(this.Uuid), false);
        }

        private void Checkbox_MouseEnterAnimation()
        {
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 100, 0, null, false) }, "OurCheckBox TextColor " + Conversions.ToString(this.Uuid), false);
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.ShapeBorder, Border.BorderBrushProperty, "ColorBrush3", 100, 0, null, false) }, "OurCheckBox BorderColor " + Conversions.ToString(this.Uuid), false);
        }

        private void Checkbox_MouseLeaveAnimation()
        {
            if (base.IsEnabled)
            {
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5", 200, 0, null, false) }, "OurCheckBox TextColor " + Conversions.ToString(this.Uuid), false);
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.ShapeBorder, Border.BorderBrushProperty, base.IsEnabled ? (this.Checked ? "ColorBrush2" : "ColorBrush1") : "ColorBrushGray5", 200, 0, null, false) }, "OurCheckBox BorderColor " + Conversions.ToString(this.Uuid), false);
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





        [CompilerGenerated]
        private void _Lambda___R1_2(object a0, MouseButtonEventArgs a1)
        {
            this.Checkbox_MouseUp();
        }

        [CompilerGenerated]
        private void _Lambda___R1_3(object a0, MouseButtonEventArgs a1)
        {
            this.Checkbox_MouseDown();
        }

        [CompilerGenerated]
        private void _Lambda___R1_4(object a0, MouseEventArgs a1)
        {
            this.Checkbox_MouseLeave();
        }

        [CompilerGenerated]
        private void _Lambda___R1_5(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.Checkbox_IsEnabledChanged();
        }

        [CompilerGenerated]
        private void _Lambda___R1_6(object a0, MouseEventArgs a1)
        {
            this.Checkbox_MouseEnterAnimation();
        }

        [CompilerGenerated]
        private void _Lambda___R1_7(object a0, MouseEventArgs a1)
        {
            this.Checkbox_MouseLeaveAnimation();
        }

        [CompilerGenerated]
        private void _Lambda___R1_8(object a0, bool a1)
        {
            this.SaveSetting();
        }

        [CompilerGenerated]
        private void _Lambda___19_0()
        {
            this.AllowMouseDown = true;
        }

        [CompilerGenerated]
        private void _Lambda___19_1()
        {
            this.AllowMouseDown = true;
        }

        public int Uuid;

        [CompilerGenerated]
        private OurCheckBox.ChangeEventHandler ChangeEvent;

        [CompilerGenerated]
        private OurCheckBox.PreviewChangeEventHandler PreviewChangeEvent;

        public static readonly DependencyProperty CheckedProperty = DependencyProperty.Register("Checked", typeof(bool), typeof(OurCheckBox), new PropertyMetadata(false, new PropertyChangedCallback(OurCheckBox._Closure___._I._Lambda___R0_1)));

        private const int AnimationTimeOfCheck = 150;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(OurCheckBox), new PropertyMetadata(new PropertyChangedCallback(OurCheckBox._Closure___._I._Lambda___0_1)));

        private bool MouseDowned;

        private bool AllowMouseDown;

        private const int AnimationTimeOfMouseIn = 100;

        private const int AnimationTimeOfMouseOut = 200;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurCheckBox _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("LabText")]
        private TextBlock _LabText;

        [CompilerGenerated]
        [AccessedThroughProperty("ShapeBorder")]
        private Border _ShapeBorder;

        [CompilerGenerated]
        [AccessedThroughProperty("ShapeCheck")]
        private Path _ShapeCheck;

        public delegate void ChangeEventHandler(object sender, bool user);

        public delegate void PreviewChangeEventHandler(object sender, ModBase.RouteEventArgs e);

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal void _Lambda___R0_1(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
            {
                ((OurCheckBox._Closure___._I0_0 == null) ? (OurCheckBox._Closure___._I0_0 = new VB_AnonymousDelegate_6<OurCheckBox, DependencyPropertyChangedEventArgs>(OurCheckBox._Closure___._I._Lambda___0_0)) : OurCheckBox._Closure___._I0_0)((OurCheckBox)a0, a1);
            }

            internal void _Lambda___0_0(OurCheckBox d, DependencyPropertyChangedEventArgs e)
            {
                if (!d.IsLoaded)
                {
                    d.SyncUI();
                }
            }

            internal void _Lambda___0_1(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                if (!Information.IsNothing(sender))
                {
                    ((OurCheckBox)sender).LabText.Text = Conversions.ToString(e.NewValue);
                }
            }

            public static readonly OurCheckBox._Closure___ _I = new OurCheckBox._Closure___();

            public static VB_AnonymousDelegate_6<OurCheckBox, DependencyPropertyChangedEventArgs> _I0_0;
        }
    }
}
