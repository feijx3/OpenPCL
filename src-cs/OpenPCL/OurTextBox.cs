using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MeloongCore;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class OurTextBox : TextBox, ISettingControl
    {
        // Note: this type is marked as 'beforefieldinit'.
        static OurTextBox()
        {
        }

        public OurTextBox()
        {
            base.Loaded += this._Lambda___R1_3;
            base.TextChanged += this._Lambda___R1_4;
            base.IsEnabledChanged += this._Lambda___R1_5;
            base.MouseEnter += this._Lambda___R1_6;
            base.MouseLeave += this._Lambda___R1_7;
            base.GotFocus += this._Lambda___R1_8;
            base.LostFocus += this._Lambda___R1_9;
            base.IsEnabledChanged += this._Lambda___R1_10;
            base.KeyUp += this.OurTextBox_KeyUp;
            this.ValidatedTextChanged += this._Lambda___R1_11;
            this.HasBackground = true;
            this.ShowValidateResult = true;
            this._labWrong = null;
            this._labHint = null;
            this.Uuid = ModBase.GetUuid();
            this._ValidateRules = new Collection<Validate>();
            this.ShownValidateResult = OurTextBox.ValidateState.NotInited;
            this.IsTextChanged = false;
        }

        public bool HasBackground
        {
            [CompilerGenerated]
            get
            {
                return this._HasBackground;
            }
            [CompilerGenerated]
            set
            {
                this._HasBackground = value;
            }
        }

        public bool ShowValidateResult
        {
            [CompilerGenerated]
            get
            {
                return this._ShowValidateResult;
            }
            [CompilerGenerated]
            set
            {
                this._ShowValidateResult = value;
            }
        }

        public CornerRadius CornerRadius
        {
            get
            {
                object value = base.GetValue(OurTextBox.CornerRadiusProperty);
                if (value == null)
                {
                    return default(CornerRadius);
                }
                return (CornerRadius)value;
            }
            set
            {
                base.SetValue(OurTextBox.CornerRadiusProperty, value);
            }
        }

        private TextBlock labWrong
        {
            get
            {
                TextBlock textBlock;
                if (base.Template == null)
                {
                    textBlock = null;
                }
                else
                {
                    if (this._labWrong == null)
                    {
                        this._labWrong = (TextBlock)base.Template.FindName("labWrong", this);
                    }
                    textBlock = this._labWrong;
                }
                return textBlock;
            }
        }

        private TextBlock labHint
        {
            get
            {
                TextBlock textBlock;
                if (base.Template == null)
                {
                    textBlock = null;
                }
                else
                {
                    if (this._labHint == null)
                    {
                        this._labHint = (TextBlock)base.Template.FindName("labHint", this);
                    }
                    textBlock = this._labHint;
                }
                return textBlock;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Operators.CompareString(this.HintText, "", false) != 0 && Operators.CompareString(this.labHint.Text, "", false) == 0)
            {
                this.labHint.Text = ((Operators.CompareString(base.Text, "", false) == 0) ? this.HintText : "");
            }
        }

        public static event OurTextBox.ValidateChangedEventHandler ValidateChanged
        {
            [CompilerGenerated]
            add
            {
                OurTextBox.ValidateChangedEventHandler validateChangedEventHandler = OurTextBox.ValidateChangedEvent;
                OurTextBox.ValidateChangedEventHandler validateChangedEventHandler2;
                do
                {
                    validateChangedEventHandler2 = validateChangedEventHandler;
                    OurTextBox.ValidateChangedEventHandler validateChangedEventHandler3 = (OurTextBox.ValidateChangedEventHandler)Delegate.Combine(validateChangedEventHandler2, value);
                    validateChangedEventHandler = Interlocked.CompareExchange<OurTextBox.ValidateChangedEventHandler>(ref OurTextBox.ValidateChangedEvent, validateChangedEventHandler3, validateChangedEventHandler2);
                }
                while (validateChangedEventHandler != validateChangedEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurTextBox.ValidateChangedEventHandler validateChangedEventHandler = OurTextBox.ValidateChangedEvent;
                OurTextBox.ValidateChangedEventHandler validateChangedEventHandler2;
                do
                {
                    validateChangedEventHandler2 = validateChangedEventHandler;
                    OurTextBox.ValidateChangedEventHandler validateChangedEventHandler3 = (OurTextBox.ValidateChangedEventHandler)Delegate.Remove(validateChangedEventHandler2, value);
                    validateChangedEventHandler = Interlocked.CompareExchange<OurTextBox.ValidateChangedEventHandler>(ref OurTextBox.ValidateChangedEvent, validateChangedEventHandler3, validateChangedEventHandler2);
                }
                while (validateChangedEventHandler != validateChangedEventHandler2);
            }
        }

        public List<RoutedEventHandler> ChangedEventList
        {
            get
            {
                if (this._ChangedEventList == null)
                {
                    this._ChangedEventList = new List<RoutedEventHandler>();
                }
                return this._ChangedEventList;
            }
        }

        public event RoutedEventHandler ValidatedTextChanged
        {
            add
            {
                this.ChangedEventList.Add(value);
            }
            remove
            {
                this.ChangedEventList.Remove(value);
            }
        }

        // Note: this method is marked as 'fire'.
        private void raise_ValidatedTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                foreach (RoutedEventHandler routedEventHandler in this.ChangedEventList)
                {
                    if (!Information.IsNothing(routedEventHandler))
                    {
                        routedEventHandler(RuntimeHelpers.GetObjectValue(sender), e);
                    }
                }
            }
            finally
            {
                List<RoutedEventHandler>.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
        }

        public string ValidateResult
        {
            get
            {
                return Conversions.ToString(base.GetValue(OurTextBox.ValidateResultProperty));
            }
            set
            {
                base.SetValue(OurTextBox.ValidateResultProperty, value);
            }
        }

        public bool IsValidated
        {
            get
            {
                return Conversions.ToBoolean(base.GetValue(OurTextBox.IsValidatedProperty));
            }
        }

        public Collection<Validate> ValidateRules
        {
            get
            {
                return this._ValidateRules;
            }
            set
            {
                this._ValidateRules = value;
                this.Validate();
            }
        }

        public void Validate()
        {
            this.ValidateResult = ModValidate.Validate(base.Text, this.ValidateRules);
            if (this.ShownValidateResult != (this.IsValidated ? OurTextBox.ValidateState.Success : OurTextBox.ValidateState.FailedAndShowDetail))
            {
                if (base.IsLoaded && this.labWrong != null)
                {
                    this.ChangeValidateResult(this.IsValidated, true);
                }
                else
                {
                    ModBase.RunInNewThread(new Action(this._Lambda___46_0), "DelayedValidate Change", ThreadPriority.Normal);
                }
            }
            if (this.ShowValidateResult && !this.IsValidated)
            {
                if (base.IsLoaded && this.labWrong != null)
                {
                    this.labWrong.Text = this.ValidateResult;
                    return;
                }
                ModBase.RunInNewThread(new Action(this._Lambda___46_2), "DelayedValidate Text", ThreadPriority.Normal);
            }
        }

        public void ForceShowAsSuccess()
        {
            this.IsTextChanged = false;
            this.ChangeValidateResult(this.IsValidated, true);
        }

        private void ChangeValidateResult(bool IsSuccessful, bool IsLoaded)
        {
            if (IsLoaded && ModAnimation.AniControlEnabled == 0 && this.labWrong != null)
            {
                if (IsSuccessful || !this.IsTextChanged)
                {
                    this.ShownValidateResult = (IsSuccessful ? OurTextBox.ValidateState.Success : OurTextBox.ValidateState.FailedButTextNotChanged);
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaOpacity(this.labWrong, -this.labWrong.Opacity, 150, 0, null, false),
                        ModAnimation.AaHeight(this.labWrong, -this.labWrong.Height, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                        ModAnimation.AaCode(new ThreadStart(this._Lambda___49_0), 0, true)
                    }, "OurTextBox Validate " + Conversions.ToString(this.Uuid), false);
                }
                else if (this.ShowValidateResult)
                {
                    this.ShownValidateResult = OurTextBox.ValidateState.FailedAndShowDetail;
                    this.labWrong.Visibility = Visibility.Visible;
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaOpacity(this.labWrong, 1.0 - this.labWrong.Opacity, 150, 0, null, false),
                        ModAnimation.AaHeight(this.labWrong, 21.0 - this.labWrong.Height, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false)
                    }, "OurTextBox Validate " + Conversions.ToString(this.Uuid), false);
                }
                else
                {
                    this.ShownValidateResult = OurTextBox.ValidateState.FailedAndHideDetail;
                }
            }
            else
            {
                this.ShownValidateResult = OurTextBox.ValidateState.NotLoaded;
            }
            this.RefreshColor();
            OurTextBox.ValidateChangedEventHandler validateChangedEvent = OurTextBox.ValidateChangedEvent;
            if (validateChangedEvent != null)
            {
                validateChangedEvent(this, new EventArgs());
            }
        }

        public string HintText
        {
            get
            {
                return Conversions.ToString(base.GetValue(OurTextBox.HintTextProperty));
            }
            set
            {
                base.SetValue(OurTextBox.HintTextProperty, value);
            }
        }

        private void OurTextBox_TextChanged(OurTextBox sender, TextChangedEventArgs e)
        {
            try
            {
                if (this.labHint != null)
                {
                    this.labHint.Text = ((Operators.CompareString(base.Text, "", false) == 0) ? this.HintText : "");
                }
                this.IsTextChanged = base.IsLoaded;
                this.Validate();
                if (this.IsValidated)
                {
                    this.raise_ValidatedTextChanged(sender, e);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "进行输入验证时出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurTextBox.vb");
            }
        }

        private void RefreshColor()
        {
            try
            {
                if (base.TemplatedParent == null || !(base.TemplatedParent is OurComboBox))
                {
                    string text;
                    string text2;
                    int num;
                    if (base.IsEnabled)
                    {
                        if (this.IsValidated || !this.IsTextChanged)
                        {
                            if (base.IsFocused)
                            {
                                text = "ColorBrush3";
                                text2 = "ColorBrush7";
                                num = 10;
                            }
                            else if (base.IsMouseOver)
                            {
                                text = "ColorBrush4";
                                text2 = "ColorBrush7";
                                num = 100;
                            }
                            else
                            {
                                text = "ColorBrushBg0";
                                text2 = "ColorBrushHalfWhite";
                                num = 100;
                            }
                        }
                        else
                        {
                            text = "ColorBrushRedLight";
                            text2 = "ColorBrushRedBack";
                            num = 200;
                        }
                    }
                    else
                    {
                        text = "ColorBrushGray5";
                        text2 = "ColorBrushGray6";
                        num = 200;
                    }
                    if (!this.HasBackground)
                    {
                        text2 = "ColorBrushTransparent";
                    }
                    if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                    {
                        ModAnimation.AniStart(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaColor(this, Control.BorderBrushProperty, text, num, 0, null, false),
                            ModAnimation.AaColor(this, Control.BackgroundProperty, text2, num, 0, null, false)
                        }, "OurTextBox Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else
                    {
                        ModAnimation.AniStop("OurTextBox Color " + Conversions.ToString(this.Uuid));
                        base.SetResourceReference(Control.BorderBrushProperty, text);
                        base.SetResourceReference(Control.BackgroundProperty, text2);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "文本框颜色改变出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurTextBox.vb");
            }
        }

        private void RefreshTextColor()
        {
            ModBase.OurColor ourColor = (base.IsEnabled ? ModSecret.ColorGray1 : ModSecret.ColorGray4);
            if ((double)((SolidColorBrush)base.Foreground).Color.R != ourColor.R)
            {
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0 && Operators.CompareString(base.Text, "", false) != 0)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this, Control.ForegroundProperty, base.IsEnabled ? "ColorBrushGray1" : "ColorBrushGray4", 200, 0, null, false) }, "OurTextBox TextColor " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStop("OurTextBox TextColor " + Conversions.ToString(this.Uuid));
                base.Foreground = ourColor;
            }
        }

        private void OurTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.RaiseCustomEvent();
            }
        }

        void ISettingControl.RefreshSetting(string NewValue)
        {
            base.Text = NewValue;
        }

        string ISettingControl.GetCurrentSetting()
        {
            return base.Text;
        }

        private void SaveSetting()
        {
            SettingService.SaveSetting(this);
        }

        [CompilerGenerated]
        private void _Lambda___R1_3(object a0, RoutedEventArgs a1)
        {
            this.Validate();
        }

        [CompilerGenerated]
        private void _Lambda___R1_4(object a0, TextChangedEventArgs a1)
        {
            this.OurTextBox_TextChanged((OurTextBox)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R1_5(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R1_6(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R1_7(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R1_8(object a0, RoutedEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R1_9(object a0, RoutedEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R1_10(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.RefreshTextColor();
        }

        [CompilerGenerated]
        private void _Lambda___R1_11(object a0, RoutedEventArgs a1)
        {
            this.SaveSetting();
        }

        [CompilerGenerated]
        private void _Lambda___46_0()
        {
            Thread.Sleep(30);
            ModBase.RunInUi(new Action(this._Lambda___46_1), false);
        }

        [CompilerGenerated]
        private void _Lambda___46_1()
        {
            this.ChangeValidateResult(this.IsValidated, false);
        }

        [CompilerGenerated]
        private void _Lambda___46_2()
        {
            OurTextBox._Closure___46_0 CS_locals1 = new OurTextBox._Closure___46_0(null);
            CS_locals1._VB_Me = this;
            CS_locals1._VB_Local_IsFinished = false;
            while (!CS_locals1._VB_Local_IsFinished)
            {
                Thread.Sleep(20);
                ModBase.RunInUiWait(new Action(CS_locals1._Lambda___3));
            }
        }

        [CompilerGenerated]
        private void _Lambda___49_0()
        {
            this.labWrong.Visibility = Visibility.Collapsed;
        }

        [CompilerGenerated]
        private bool _HasBackground;

        [CompilerGenerated]
        private bool _ShowValidateResult;

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(OurTextBox), new PropertyMetadata(new CornerRadius(3.0)));

        private TextBlock _labWrong;

        private TextBlock _labHint;

        public int Uuid;

        [CompilerGenerated]
        private static OurTextBox.ValidateChangedEventHandler ValidateChangedEvent;

        private List<RoutedEventHandler> _ChangedEventList;

        public static readonly DependencyProperty ValidateResultProperty = DependencyProperty.Register("ValidateResult", typeof(string), typeof(OurTextBox), new PropertyMetadata("", new PropertyChangedCallback(OurTextBox._Closure___._I._Lambda___R0_1)));

        private static readonly DependencyPropertyKey IsValidatedPropertyKey = DependencyProperty.RegisterReadOnly("IsValidated", typeof(bool), typeof(OurTextBox), new PropertyMetadata(true));

        public static readonly DependencyProperty IsValidatedProperty = OurTextBox.IsValidatedPropertyKey.DependencyProperty;

        private Collection<Validate> _ValidateRules;

        private OurTextBox.ValidateState ShownValidateResult;

        private bool IsTextChanged;

        public static readonly DependencyProperty HintTextProperty = DependencyProperty.Register("HintText", typeof(string), typeof(OurTextBox), new PropertyMetadata("", new PropertyChangedCallback(OurTextBox._Closure___._I._Lambda___R0_2)));

        public delegate void ValidateChangedEventHandler(object sender, EventArgs e);

        private enum ValidateState
        {
            NotInited,
            Success,
            FailedButTextNotChanged,
            FailedAndShowDetail,
            FailedAndHideDetail,
            NotLoaded
        }

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            public _Closure___()
            {
            }

            static _Closure___()
            {
            }

            internal void _Lambda___R0_1(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
            {
                ((OurTextBox._Closure___._I0_0 == null) ? (OurTextBox._Closure___._I0_0 = new VB_AnonymousDelegate_6<OurTextBox, DependencyPropertyChangedEventArgs>(OurTextBox._Closure___._I._Lambda___0_0)) : OurTextBox._Closure___._I0_0)((OurTextBox)a0, a1);
            }

            internal void _Lambda___0_0(OurTextBox d, DependencyPropertyChangedEventArgs e)
            {
                d.SetValue(OurTextBox.IsValidatedPropertyKey, string.IsNullOrEmpty(Conversions.ToString(e.NewValue)));
            }

            internal void _Lambda___R0_2(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
            {
                ((OurTextBox._Closure___._I0_1 == null) ? (OurTextBox._Closure___._I0_1 = new VB_AnonymousDelegate_7<OurTextBox, DependencyPropertyChangedEventArgs>(OurTextBox._Closure___._I._Lambda___0_1)) : OurTextBox._Closure___._I0_1)((OurTextBox)a0, a1);
            }

            internal void _Lambda___0_1(OurTextBox t, DependencyPropertyChangedEventArgs e)
            {
                if (t.labHint != null)
                {
                    t.labHint.Text = ((Operators.CompareString(t.Text, "", false) == 0) ? t.HintText : "");
                }
            }

            public static readonly OurTextBox._Closure___ _I = new OurTextBox._Closure___();

            public static VB_AnonymousDelegate_6<OurTextBox, DependencyPropertyChangedEventArgs> _I0_0;

            public static VB_AnonymousDelegate_7<OurTextBox, DependencyPropertyChangedEventArgs> _I0_1;
        }

        [CompilerGenerated]
        private sealed class _Closure___46_0
        {
            public _Closure___46_0(OurTextBox._Closure___46_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_IsFinished = arg0._VB_Local_IsFinished;
                }
            }

            internal void _Lambda___3()
            {
                if (this._VB_Me.labWrong != null)
                {
                    this._VB_Me.labWrong.Text = this._VB_Me.ValidateResult;
                    this._VB_Local_IsFinished = true;
                }
                if (!this._VB_Me.IsLoaded)
                {
                    this._VB_Local_IsFinished = true;
                }
            }

            public bool _VB_Local_IsFinished;

            public OurTextBox _VB_Me;
        }
    }
}
