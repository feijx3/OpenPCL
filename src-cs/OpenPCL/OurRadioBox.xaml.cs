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
using System.Windows.Shapes;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [ContentProperty("Inlines")]
    [DesignerGenerated]
    public partial class OurRadioBox : Grid, IOurRadio, ISettingControl
    {
        public OurRadioBox()
        {
            base.MouseLeftButtonUp += this._Lambda___R1_2;
            base.MouseLeftButtonDown += this._Lambda___R1_3;
            base.MouseLeave += this._Lambda___R1_4;
            base.IsEnabledChanged += this._Lambda___R1_5;
            base.MouseEnter += this._Lambda___R1_6;
            base.MouseLeave += this._Lambda___R1_7;
            this.Check += this._Lambda___R1_8;
            this.Uuid = ModBase.GetUuid();
            this.MouseDowned = false;
            this.AllowMouseDown = true;
            this.InitializeComponent();
        }

        public event OurRadioBox.PreviewCheckEventHandler PreviewCheck
        {
            [CompilerGenerated]
            add
            {
                OurRadioBox.PreviewCheckEventHandler previewCheckEventHandler = this.PreviewCheckEvent;
                OurRadioBox.PreviewCheckEventHandler previewCheckEventHandler2;
                do
                {
                    previewCheckEventHandler2 = previewCheckEventHandler;
                    OurRadioBox.PreviewCheckEventHandler previewCheckEventHandler3 = (OurRadioBox.PreviewCheckEventHandler)Delegate.Combine(previewCheckEventHandler2, value);
                    previewCheckEventHandler = Interlocked.CompareExchange<OurRadioBox.PreviewCheckEventHandler>(ref this.PreviewCheckEvent, previewCheckEventHandler3, previewCheckEventHandler2);
                }
                while (previewCheckEventHandler != previewCheckEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurRadioBox.PreviewCheckEventHandler previewCheckEventHandler = this.PreviewCheckEvent;
                OurRadioBox.PreviewCheckEventHandler previewCheckEventHandler2;
                do
                {
                    previewCheckEventHandler2 = previewCheckEventHandler;
                    OurRadioBox.PreviewCheckEventHandler previewCheckEventHandler3 = (OurRadioBox.PreviewCheckEventHandler)Delegate.Remove(previewCheckEventHandler2, value);
                    previewCheckEventHandler = Interlocked.CompareExchange<OurRadioBox.PreviewCheckEventHandler>(ref this.PreviewCheckEvent, previewCheckEventHandler3, previewCheckEventHandler2);
                }
                while (previewCheckEventHandler != previewCheckEventHandler2);
            }
        }

        public event OurRadioBox.PreviewChangeEventHandler PreviewChange
        {
            [CompilerGenerated]
            add
            {
                OurRadioBox.PreviewChangeEventHandler previewChangeEventHandler = this.PreviewChangeEvent;
                OurRadioBox.PreviewChangeEventHandler previewChangeEventHandler2;
                do
                {
                    previewChangeEventHandler2 = previewChangeEventHandler;
                    OurRadioBox.PreviewChangeEventHandler previewChangeEventHandler3 = (OurRadioBox.PreviewChangeEventHandler)Delegate.Combine(previewChangeEventHandler2, value);
                    previewChangeEventHandler = Interlocked.CompareExchange<OurRadioBox.PreviewChangeEventHandler>(ref this.PreviewChangeEvent, previewChangeEventHandler3, previewChangeEventHandler2);
                }
                while (previewChangeEventHandler != previewChangeEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurRadioBox.PreviewChangeEventHandler previewChangeEventHandler = this.PreviewChangeEvent;
                OurRadioBox.PreviewChangeEventHandler previewChangeEventHandler2;
                do
                {
                    previewChangeEventHandler2 = previewChangeEventHandler;
                    OurRadioBox.PreviewChangeEventHandler previewChangeEventHandler3 = (OurRadioBox.PreviewChangeEventHandler)Delegate.Remove(previewChangeEventHandler2, value);
                    previewChangeEventHandler = Interlocked.CompareExchange<OurRadioBox.PreviewChangeEventHandler>(ref this.PreviewChangeEvent, previewChangeEventHandler3, previewChangeEventHandler2);
                }
                while (previewChangeEventHandler != previewChangeEventHandler2);
            }
        }

        public event IOurRadio.CheckEventHandler Check
        {
            [CompilerGenerated]
            add
            {
                IOurRadio.CheckEventHandler checkEventHandler = this.CheckEvent;
                IOurRadio.CheckEventHandler checkEventHandler2;
                do
                {
                    checkEventHandler2 = checkEventHandler;
                    IOurRadio.CheckEventHandler checkEventHandler3 = (IOurRadio.CheckEventHandler)Delegate.Combine(checkEventHandler2, value);
                    checkEventHandler = Interlocked.CompareExchange<IOurRadio.CheckEventHandler>(ref this.CheckEvent, checkEventHandler3, checkEventHandler2);
                }
                while (checkEventHandler != checkEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                IOurRadio.CheckEventHandler checkEventHandler = this.CheckEvent;
                IOurRadio.CheckEventHandler checkEventHandler2;
                do
                {
                    checkEventHandler2 = checkEventHandler;
                    IOurRadio.CheckEventHandler checkEventHandler3 = (IOurRadio.CheckEventHandler)Delegate.Remove(checkEventHandler2, value);
                    checkEventHandler = Interlocked.CompareExchange<IOurRadio.CheckEventHandler>(ref this.CheckEvent, checkEventHandler3, checkEventHandler2);
                }
                while (checkEventHandler != checkEventHandler2);
            }
        }

        public event IOurRadio.ChangedEventHandler Changed
        {
            [CompilerGenerated]
            add
            {
                IOurRadio.ChangedEventHandler changedEventHandler = this.ChangedEvent;
                IOurRadio.ChangedEventHandler changedEventHandler2;
                do
                {
                    changedEventHandler2 = changedEventHandler;
                    IOurRadio.ChangedEventHandler changedEventHandler3 = (IOurRadio.ChangedEventHandler)Delegate.Combine(changedEventHandler2, value);
                    changedEventHandler = Interlocked.CompareExchange<IOurRadio.ChangedEventHandler>(ref this.ChangedEvent, changedEventHandler3, changedEventHandler2);
                }
                while (changedEventHandler != changedEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                IOurRadio.ChangedEventHandler changedEventHandler = this.ChangedEvent;
                IOurRadio.ChangedEventHandler changedEventHandler2;
                do
                {
                    changedEventHandler2 = changedEventHandler;
                    IOurRadio.ChangedEventHandler changedEventHandler3 = (IOurRadio.ChangedEventHandler)Delegate.Remove(changedEventHandler2, value);
                    changedEventHandler = Interlocked.CompareExchange<IOurRadio.ChangedEventHandler>(ref this.ChangedEvent, changedEventHandler3, changedEventHandler2);
                }
                while (changedEventHandler != changedEventHandler2);
            }
        }

        public bool Checked
        {
            get
            {
                return Conversions.ToBoolean(base.GetValue(OurRadioBox.CheckedProperty));
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
                        OurRadioBox.PreviewCheckEventHandler previewCheckEvent = this.PreviewCheckEvent;
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
                        OurRadioBox.PreviewChangeEventHandler previewChangeEvent = this.PreviewChangeEvent;
                        if (previewChangeEvent != null)
                        {
                            previewChangeEvent(this, new ModBase.RouteEventArgs(user));
                        }
                    }
                    if (value != this.Checked)
                    {
                        base.SetValue(OurRadioBox.CheckedProperty, value);
                        flag = true;
                    }
                    if (base.Parent != null)
                    {
                        List<OurRadioBox> list = new List<OurRadioBox>();
                        int num = 0;
                        try
                        {
                            foreach (object obj in ((IEnumerable)NewLateBinding.LateGet(base.Parent, null, "Children", new object[0], null, null, null)))
                            {
                                object objectValue = RuntimeHelpers.GetObjectValue(obj);
                                if (objectValue is OurRadioBox)
                                {
                                    list.Add((OurRadioBox)objectValue);
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
                        if (num > 1)
                        {
                            if (this.Checked)
                            {
                                try
                                {
                                    foreach (OurRadioBox ourRadioBox in list)
                                    {
                                        if (ourRadioBox.Checked && !ourRadioBox.Equals(this))
                                        {
                                            ourRadioBox.Checked = false;
                                        }
                                    }
                                    goto IL_01C0;
                                }
                                finally
                                {
                                    List<OurRadioBox>.Enumerator enumerator2 = default;
                                    ((IDisposable)enumerator2).Dispose();
                                }
                            }
                            bool flag2 = false;
                            try
                            {
                                foreach (OurRadioBox ourRadioBox2 in list)
                                {
                                    if (ourRadioBox2.Checked)
                                    {
                                        if (flag2)
                                        {
                                            ourRadioBox2.Checked = false;
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
                                List<OurRadioBox>.Enumerator enumerator3 = default;
                                ((IDisposable)enumerator3).Dispose();
                            }
                        }
                        IL_01C0:
                        if (flag)
                        {
                            if (this.Checked)
                            {
                                IOurRadio.CheckEventHandler checkEvent = this.CheckEvent;
                                if (checkEvent != null)
                                {
                                    checkEvent(this, new ModBase.RouteEventArgs(user));
                                }
                            }
                            IOurRadio.ChangedEventHandler changedEvent = this.ChangedEvent;
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
                    Logger.Error(ex, "单选框勾选改变错误", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurRadioBox.xaml.vb");
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
                    }, "OurRadioBox Border " + Conversions.ToString(this.Uuid), false);
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaScale(this.ShapeDot, 9.0 - this.ShapeDot.Width, 390, 0, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false, true),
                        ModAnimation.AaOpacity(this.ShapeDot, 1.0 - this.ShapeDot.Opacity, 75, 90, null, false)
                    }, "OurRadioBox Dot " + Conversions.ToString(this.Uuid), false);
                    ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.ShapeBorder, Shape.StrokeProperty, base.IsMouseOver ? "ColorBrush3" : (base.IsEnabled ? "ColorBrush2" : "ColorBrushGray5"), 150, 0, null, false) }, "OurRadioBox BorderColor " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScale(this.ShapeBorder, 18.0 - this.ShapeBorder.Width, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false, true) }, "OurRadioBox Border " + Conversions.ToString(this.Uuid), false);
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaScale(this.ShapeDot, -this.ShapeDot.Width, 150, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false, true),
                    ModAnimation.AaOpacity(this.ShapeDot, -this.ShapeDot.Opacity, 75, 30, null, false)
                }, "OurRadioBox Dot " + Conversions.ToString(this.Uuid), false);
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.ShapeBorder, Shape.StrokeProperty, base.IsMouseOver ? "ColorBrush3" : (base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5"), 150, 0, null, false) }, "OurRadioBox BorderColor " + Conversions.ToString(this.Uuid), false);
                return;
            }
            else
            {
                ModAnimation.AniStop("OurRadioBox Border " + Conversions.ToString(this.Uuid));
                ModAnimation.AniStop("OurRadioBox Dot " + Conversions.ToString(this.Uuid));
                ModAnimation.AniStop("OurRadioBox BorderColor " + Conversions.ToString(this.Uuid));
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
                return Conversions.ToString(base.GetValue(OurRadioBox.TextProperty));
            }
            set
            {
                base.SetValue(OurRadioBox.TextProperty, value);
            }
        }

        private void Radiobox_MouseUp()
        {
            if (this.MouseDowned)
            {
                Logger.Info(string.Format("按下单选框：{0}", this.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurRadioBox.xaml.vb");
                this.SetChecked(true, true);
                this.MouseDowned = false;
                ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Shape.FillProperty, "ColorBrushHalfWhite", 100, 0, null, false), "OurRadioBox Background " + Conversions.ToString(this.Uuid), false);
            }
        }

        private void Radiobox_MouseDown()
        {
            this.MouseDowned = true;
            base.Focus();
            ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Shape.FillProperty, "ColorBrushBg1", 100, 0, null, false), "OurRadioBox Background " + Conversions.ToString(this.Uuid), false);
            if (!this.Checked)
            {
                ModAnimation.AniStart(ModAnimation.AaScale(this.ShapeBorder, 16.5 - this.ShapeBorder.Width, 1000, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false, true), "OurRadioBox Border " + Conversions.ToString(this.Uuid), false);
            }
        }

        private void Radiobox_MouseLeave()
        {
            if (this.MouseDowned)
            {
                this.MouseDowned = false;
                ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Shape.FillProperty, "ColorBrushHalfWhite", 100, 0, null, false), "OurRadioBox Background " + Conversions.ToString(this.Uuid), false);
                if (!this.Checked)
                {
                    ModAnimation.AniStart(ModAnimation.AaScale(this.ShapeBorder, 18.0 - this.ShapeBorder.Width, 400, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false, true), "OurRadioBox Border " + Conversions.ToString(this.Uuid), false);
                }
            }
        }

        private void Radiobox_IsEnabledChanged()
        {
            if (!base.IsLoaded || ModAnimation.AniControlEnabled != 0)
            {
                ModAnimation.AniStop("OurRadioBox BorderColor " + Conversions.ToString(this.Uuid));
                ModAnimation.AniStop("OurRadioBox TextColor " + Conversions.ToString(this.Uuid));
                this.LabText.SetResourceReference(TextBlock.ForegroundProperty, base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5");
                this.ShapeBorder.SetResourceReference(Shape.StrokeProperty, base.IsEnabled ? (this.Checked ? "ColorBrush2" : "ColorBrush1") : "ColorBrushGray5");
                return;
            }
            if (base.IsEnabled)
            {
                this.Radiobox_MouseLeaveAnimation();
                return;
            }
            ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Shape.StrokeProperty, ModSecret.ColorGray5 - this.ShapeBorder.Stroke, 200, 0, null, false), "OurRadioBox BorderColor " + Conversions.ToString(this.Uuid), false);
            ModAnimation.AniStart(ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, ModSecret.ColorGray5 - this.LabText.Foreground, 200, 0, null, false), "OurRadioBox TextColor " + Conversions.ToString(this.Uuid), false);
        }

        private void Radiobox_MouseEnterAnimation()
        {
            ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Shape.StrokeProperty, "ColorBrush3", 100, 0, null, false), "OurRadioBox BorderColor " + Conversions.ToString(this.Uuid), false);
            ModAnimation.AniStart(ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 100, 0, null, false), "OurRadioBox TextColor " + Conversions.ToString(this.Uuid), false);
        }

        private void Radiobox_MouseLeaveAnimation()
        {
            if (base.IsEnabled)
            {
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                {
                    ModAnimation.AniStart(ModAnimation.AaColor(this.ShapeBorder, Shape.StrokeProperty, base.IsEnabled ? (this.Checked ? "ColorBrush2" : "ColorBrush1") : "ColorBrushGray5", 200, 0, null, false), "OurRadioBox BorderColor " + Conversions.ToString(this.Uuid), false);
                    ModAnimation.AniStart(ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5", 200, 0, null, false), "OurRadioBox TextColor " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStop("OurRadioBox BorderColor " + Conversions.ToString(this.Uuid));
                ModAnimation.AniStop("OurRadioBox TextColor " + Conversions.ToString(this.Uuid));
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





        [CompilerGenerated]
        private void _Lambda___R1_2(object a0, MouseButtonEventArgs a1)
        {
            this.Radiobox_MouseUp();
        }

        [CompilerGenerated]
        private void _Lambda___R1_3(object a0, MouseButtonEventArgs a1)
        {
            this.Radiobox_MouseDown();
        }

        [CompilerGenerated]
        private void _Lambda___R1_4(object a0, MouseEventArgs a1)
        {
            this.Radiobox_MouseLeave();
        }

        [CompilerGenerated]
        private void _Lambda___R1_5(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.Radiobox_IsEnabledChanged();
        }

        [CompilerGenerated]
        private void _Lambda___R1_6(object a0, MouseEventArgs a1)
        {
            this.Radiobox_MouseEnterAnimation();
        }

        [CompilerGenerated]
        private void _Lambda___R1_7(object a0, MouseEventArgs a1)
        {
            this.Radiobox_MouseLeaveAnimation();
        }

        [CompilerGenerated]
        private void _Lambda___R1_8(object a0, ModBase.RouteEventArgs a1)
        {
            this.SaveSetting();
        }

        public int Uuid;

        [CompilerGenerated]
        private OurRadioBox.PreviewCheckEventHandler PreviewCheckEvent;

        [CompilerGenerated]
        private OurRadioBox.PreviewChangeEventHandler PreviewChangeEvent;

        [CompilerGenerated]
        private IOurRadio.CheckEventHandler CheckEvent;

        [CompilerGenerated]
        private IOurRadio.ChangedEventHandler ChangedEvent;

        public static readonly DependencyProperty CheckedProperty = DependencyProperty.Register("Checked", typeof(bool), typeof(OurRadioBox), new PropertyMetadata(false, new PropertyChangedCallback(OurRadioBox._Closure___._I._Lambda___R0_1)));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(OurRadioBox), new PropertyMetadata(new PropertyChangedCallback(OurRadioBox._Closure___._I._Lambda___0_1)));

        private bool MouseDowned;

        private bool AllowMouseDown;

        private const int AnimationTimeOfMouseIn = 100;

        private const int AnimationTimeOfMouseOut = 200;

        private const int AnimationTimeOfCheck = 150;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurRadioBox _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("LabText")]
        private TextBlock _LabText;

        [CompilerGenerated]
        [AccessedThroughProperty("ShapeBorder")]
        private Ellipse _ShapeBorder;

        [CompilerGenerated]
        [AccessedThroughProperty("ShapeDot")]
        private Ellipse _ShapeDot;

        public delegate void PreviewCheckEventHandler(object sender, ModBase.RouteEventArgs e);

        public delegate void PreviewChangeEventHandler(object sender, ModBase.RouteEventArgs e);

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal void _Lambda___R0_1(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
            {
                ((OurRadioBox._Closure___._I0_0 == null) ? (OurRadioBox._Closure___._I0_0 = new VB_AnonymousDelegate_6<OurRadioBox, DependencyPropertyChangedEventArgs>(OurRadioBox._Closure___._I._Lambda___0_0)) : OurRadioBox._Closure___._I0_0)((OurRadioBox)a0, a1);
            }

            internal void _Lambda___0_0(OurRadioBox d, DependencyPropertyChangedEventArgs e)
            {
                if (!d.IsLoaded)
                {
                    d.SyncUI();
                }
            }

            internal void _Lambda___0_1(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                if (sender != null)
                {
                    ((OurRadioBox)sender).LabText.Text = Conversions.ToString(e.NewValue);
                }
            }

            public static readonly OurRadioBox._Closure___ _I = new OurRadioBox._Closure___();

            public static VB_AnonymousDelegate_6<OurRadioBox, DependencyPropertyChangedEventArgs> _I0_0;
        }
    }
}
