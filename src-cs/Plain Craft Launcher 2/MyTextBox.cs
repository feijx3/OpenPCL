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
    public class MyTextBox : TextBox, ISettingControl
    {
        public MyTextBox()
        {
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.Validate();
            };
            base.TextChanged += delegate(object a0, TextChangedEventArgs a1)
            {
                this.MyTextBox_TextChanged((MyTextBox)a0, a1);
            };
            base.IsEnabledChanged += delegate(object a0, DependencyPropertyChangedEventArgs a1)
            {
                this.RefreshColor();
            };
            base.MouseEnter += delegate(object a0, MouseEventArgs a1)
            {
                this.RefreshColor();
            };
            base.MouseLeave += delegate(object a0, MouseEventArgs a1)
            {
                this.RefreshColor();
            };
            base.GotFocus += delegate(object a0, RoutedEventArgs a1)
            {
                this.RefreshColor();
            };
            base.LostFocus += delegate(object a0, RoutedEventArgs a1)
            {
                this.RefreshColor();
            };
            base.IsEnabledChanged += delegate(object a0, DependencyPropertyChangedEventArgs a1)
            {
                this.RefreshTextColor();
            };
            base.KeyUp += this.MyTextBox_KeyUp;
            this.ValidatedTextChanged += delegate(object a0, RoutedEventArgs a1)
            {
                this.SaveSetting();
            };
            this.HasBackground = true;
            this.ShowValidateResult = true;
            this._labWrong = null;
            this._labHint = null;
            this.Uuid = ModBase.GetUuid();
            this._ValidateRules = new Collection<Validate>();
            this.ShownValidateResult = MyTextBox.ValidateState.NotInited;
            this.IsTextChanged = false;
        }

        public bool HasBackground { get; set; }

        public bool ShowValidateResult { get; set; }

        public CornerRadius CornerRadius
        {
            get
            {
                object value = base.GetValue(MyTextBox.CornerRadiusProperty);
                if (value == null)
                {
                    return default(CornerRadius);
                }
                return (CornerRadius)value;
            }
            set
            {
                base.SetValue(MyTextBox.CornerRadiusProperty, value);
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

        public static event MyTextBox.ValidateChangedEventHandler ValidateChanged;

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
                List<RoutedEventHandler>.Enumerator enumerator;
                ((IDisposable)enumerator).Dispose();
            }
        }

        public string ValidateResult
        {
            get
            {
                return Conversions.ToString(base.GetValue(MyTextBox.ValidateResultProperty));
            }
            set
            {
                base.SetValue(MyTextBox.ValidateResultProperty, value);
            }
        }

        public bool IsValidated
        {
            get
            {
                return Conversions.ToBoolean(base.GetValue(MyTextBox.IsValidatedProperty));
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
            if (this.ShownValidateResult != (this.IsValidated ? MyTextBox.ValidateState.Success : MyTextBox.ValidateState.FailedAndShowDetail))
            {
                if (base.IsLoaded && this.labWrong != null)
                {
                    this.ChangeValidateResult(this.IsValidated, true);
                }
                else
                {
                    ModBase.RunInNewThread(delegate
                    {
                        Thread.Sleep(30);
                        ModBase.RunInUi(delegate
                        {
                            this.ChangeValidateResult(this.IsValidated, false);
                        }, false);
                    }, "DelayedValidate Change", ThreadPriority.Normal);
                }
            }
            if (this.ShowValidateResult && !this.IsValidated)
            {
                if (base.IsLoaded && this.labWrong != null)
                {
                    this.labWrong.Text = this.ValidateResult;
                    return;
                }
                ModBase.RunInNewThread(delegate
                {
                    MyTextBox._Closure$__46-0 CS$<>8__locals1 = new MyTextBox._Closure$__46-0(CS$<>8__locals1);
                    CS$<>8__locals1.$VB$Me = this;
                    CS$<>8__locals1.$VB$Local_IsFinished = false;
                    while (!CS$<>8__locals1.$VB$Local_IsFinished)
                    {
                        Thread.Sleep(20);
                        ModBase.RunInUiWait(delegate
                        {
                            if (CS$<>8__locals1.$VB$Me.labWrong != null)
                            {
                                CS$<>8__locals1.$VB$Me.labWrong.Text = CS$<>8__locals1.$VB$Me.ValidateResult;
                                CS$<>8__locals1.$VB$Local_IsFinished = true;
                            }
                            if (!CS$<>8__locals1.$VB$Me.IsLoaded)
                            {
                                CS$<>8__locals1.$VB$Local_IsFinished = true;
                            }
                        });
                    }
                }, "DelayedValidate Text", ThreadPriority.Normal);
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
                    this.ShownValidateResult = (IsSuccessful ? MyTextBox.ValidateState.Success : MyTextBox.ValidateState.FailedButTextNotChanged);
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaOpacity(this.labWrong, -this.labWrong.Opacity, 150, 0, null, false),
                        ModAnimation.AaHeight(this.labWrong, -this.labWrong.Height, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                        ModAnimation.AaCode(delegate
                        {
                            this.labWrong.Visibility = Visibility.Collapsed;
                        }, 0, true)
                    }, "MyTextBox Validate " + Conversions.ToString(this.Uuid), false);
                }
                else if (this.ShowValidateResult)
                {
                    this.ShownValidateResult = MyTextBox.ValidateState.FailedAndShowDetail;
                    this.labWrong.Visibility = Visibility.Visible;
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaOpacity(this.labWrong, 1.0 - this.labWrong.Opacity, 150, 0, null, false),
                        ModAnimation.AaHeight(this.labWrong, 21.0 - this.labWrong.Height, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false)
                    }, "MyTextBox Validate " + Conversions.ToString(this.Uuid), false);
                }
                else
                {
                    this.ShownValidateResult = MyTextBox.ValidateState.FailedAndHideDetail;
                }
            }
            else
            {
                this.ShownValidateResult = MyTextBox.ValidateState.NotLoaded;
            }
            this.RefreshColor();
            MyTextBox.ValidateChangedEventHandler validateChangedEvent = MyTextBox.ValidateChangedEvent;
            if (validateChangedEvent != null)
            {
                validateChangedEvent(this, new EventArgs());
            }
        }

        public string HintText
        {
            get
            {
                return Conversions.ToString(base.GetValue(MyTextBox.HintTextProperty));
            }
            set
            {
                base.SetValue(MyTextBox.HintTextProperty, value);
            }
        }

        private void MyTextBox_TextChanged(MyTextBox sender, TextChangedEventArgs e)
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
                Logger.Error(ex, "进行输入验证时出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyTextBox.vb");
            }
        }

        private void RefreshColor()
        {
            try
            {
                if (base.TemplatedParent == null || !(base.TemplatedParent is MyComboBox))
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
                        }, "MyTextBox Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else
                    {
                        ModAnimation.AniStop("MyTextBox Color " + Conversions.ToString(this.Uuid));
                        base.SetResourceReference(Control.BorderBrushProperty, text);
                        base.SetResourceReference(Control.BackgroundProperty, text2);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "文本框颜色改变出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyTextBox.vb");
            }
        }

        private void RefreshTextColor()
        {
            ModBase.MyColor myColor = (base.IsEnabled ? ModSecret.ColorGray1 : ModSecret.ColorGray4);
            if ((double)((SolidColorBrush)base.Foreground).Color.R != myColor.R)
            {
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0 && Operators.CompareString(base.Text, "", false) != 0)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this, Control.ForegroundProperty, base.IsEnabled ? "ColorBrushGray1" : "ColorBrushGray4", 200, 0, null, false) }, "MyTextBox TextColor " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStop("MyTextBox TextColor " + Conversions.ToString(this.Uuid));
                base.Foreground = myColor;
            }
        }

        private void MyTextBox_KeyUp(object sender, KeyEventArgs e)
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

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MyTextBox), new PropertyMetadata(new CornerRadius(3.0)));

        private TextBlock _labWrong;

        private TextBlock _labHint;

        public int Uuid;

        private List<RoutedEventHandler> _ChangedEventList;

        public static readonly DependencyProperty ValidateResultProperty = DependencyProperty.Register("ValidateResult", typeof(string), typeof(MyTextBox), new PropertyMetadata("", delegate(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
        {
            ((MyTextBox._Closure$__.$I0-0 == null) ? (MyTextBox._Closure$__.$I0-0 = delegate(MyTextBox d, DependencyPropertyChangedEventArgs e)
            {
                d.SetValue(MyTextBox.IsValidatedPropertyKey, string.IsNullOrEmpty(Conversions.ToString(e.NewValue)));
            }) : MyTextBox._Closure$__.$I0-0)((MyTextBox)a0, a1);
        }));

        private static readonly DependencyPropertyKey IsValidatedPropertyKey = DependencyProperty.RegisterReadOnly("IsValidated", typeof(bool), typeof(MyTextBox), new PropertyMetadata(true));

        public static readonly DependencyProperty IsValidatedProperty = MyTextBox.IsValidatedPropertyKey.DependencyProperty;

        private Collection<Validate> _ValidateRules;

        private MyTextBox.ValidateState ShownValidateResult;

        private bool IsTextChanged;

        public static readonly DependencyProperty HintTextProperty = DependencyProperty.Register("HintText", typeof(string), typeof(MyTextBox), new PropertyMetadata("", delegate(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
        {
            ((MyTextBox._Closure$__.$I0-1 == null) ? (MyTextBox._Closure$__.$I0-1 = delegate(MyTextBox t, DependencyPropertyChangedEventArgs e)
            {
                if (t.labHint != null)
                {
                    t.labHint.Text = ((Operators.CompareString(t.Text, "", false) == 0) ? t.HintText : "");
                }
            }) : MyTextBox._Closure$__.$I0-1)((MyTextBox)a0, a1);
        }));

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
    }
}
