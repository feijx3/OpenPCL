using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MeloongCore;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class MyComboBox : ComboBox, ISettingControl
    {
        public MyComboBox()
        {
            base.PreviewMouseLeftButtonDown += this.MyComboBox_PreviewMouseLeftButtonDown;
            base.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.MyComboBox_PreviewMouseLeftButtonUp);
            base.MouseLeave += new MouseEventHandler(this.MyComboBox_PreviewMouseLeftButtonUp);
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
            base.PreviewMouseLeftButtonDown += delegate(object a0, MouseButtonEventArgs a1)
            {
                this.RefreshColor();
            };
            base.PreviewMouseLeftButtonUp += delegate(object a0, MouseButtonEventArgs a1)
            {
                this.RefreshColor();
            };
            base.GotKeyboardFocus += delegate(object a0, KeyboardFocusChangedEventArgs a1)
            {
                this.RefreshColor();
            };
            base.DropDownOpened += this.MyComboBox_DropDownOpened;
            base.DropDownClosed += this.MyComboBox_DropDownClosed;
            this.TextChanged += this.MyComboBox_TextChanged;
            base.SelectionChanged += this.MyComboBox_SelectionChanged;
            base.SelectionChanged += delegate(object a0, SelectionChangedEventArgs a1)
            {
                this.SaveSetting();
            };
            this.Uuid = ModBase.GetUuid();
            this.HintText = "";
            this._Text = Conversions.ToString(base.SelectedItem);
            this.IsMouseDown = false;
            this.IsTextChanging = false;
        }

        public event MyComboBox.TextChangedEventHandler TextChanged;

        public string HintText { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (base.IsEditable)
            {
                try
                {
                    this.TextBox = (MyTextBox)base.Template.FindName("PART_EditableTextBox", this);
                    this.TextBox.AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler(delegate(object a0, RoutedEventArgs a1)
                    {
                        this.RefreshColor();
                    }));
                    this.TextBox.ChangedEventList.Add(delegate(object sender, RoutedEventArgs e)
                    {
                        MyComboBox.TextChangedEventHandler textChangedEvent2 = this.TextChangedEvent;
                        if (textChangedEvent2 != null)
                        {
                            textChangedEvent2(RuntimeHelpers.GetObjectValue(sender), (TextChangedEventArgs)e);
                        }
                    });
                    this.TextBox.Tag = RuntimeHelpers.GetObjectValue(base.Tag);
                    if (Operators.CompareString(this.Text, "", false) == 0)
                    {
                        this.TextBox.Text = this._Text;
                    }
                    else
                    {
                        MyComboBox.TextChangedEventHandler textChangedEvent = this.TextChangedEvent;
                        if (textChangedEvent != null)
                        {
                            textChangedEvent(this, null);
                        }
                    }
                    if (this.HintText.Length > 0)
                    {
                        this.TextBox.HintText = this.HintText;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, string.Format("初始化可编辑文本框失败（{0}）", base.Name ?? ""), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyComboBox.vb");
                }
            }
        }

        public new string Text
        {
            get
            {
                string text;
                if (base.IsEditable)
                {
                    if (this.TextBox == null)
                    {
                        text = this._Text ?? "";
                    }
                    else
                    {
                        text = this.TextBox.Text ?? "";
                    }
                }
                else
                {
                    text = (base.SelectedItem ?? "").ToString();
                }
                return text;
            }
            set
            {
                if (!base.IsEditable)
                {
                    throw new NotSupportedException("该 ComboBox 不支持修改文本。");
                }
                if (Information.IsNothing(this.TextBox))
                {
                    this._Text = value;
                    return;
                }
                this.TextBox.Text = value;
            }
        }

        private void MyComboBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsMouseDown = true;
        }

        private void MyComboBox_PreviewMouseLeftButtonUp(object sender, EventArgs e)
        {
            this.IsMouseDown = false;
        }

        public void RefreshColor()
        {
            string text;
            string text2;
            int num;
            if (base.IsEnabled)
            {
                if (Conversions.ToBoolean(this.IsMouseDown || base.IsDropDownOpen || (base.IsEditable && Conversions.ToBoolean(NewLateBinding.LateGet(base.Template.FindName("PART_EditableTextBox", this), null, "IsFocused", new object[0], null, null, null)))))
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
                text = "ColorBrushGray5";
                text2 = "ColorBrushGray6";
                num = 200;
            }
            if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
            {
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaColor(this, Control.ForegroundProperty, text, num, 0, null, false),
                    ModAnimation.AaColor(this, Control.BackgroundProperty, text2, num, 0, null, false)
                }, "MyComboBox Color " + Conversions.ToString(this.Uuid), false);
                return;
            }
            ModAnimation.AniStop("MyComboBox Color " + Conversions.ToString(this.Uuid));
            base.SetResourceReference(Control.ForegroundProperty, text);
            base.SetResourceReference(Control.BackgroundProperty, text2);
        }

        private void MyComboBox_DropDownOpened(object sender, EventArgs e)
        {
            this.RealWidth = base.Width;
            base.Width = base.ActualWidth;
            try
            {
                ((Grid)base.Template.FindName("PanPopup", this)).Opacity = ModMain.FrmMain.Opacity;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "设置下拉框透明度失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyComboBox.vb");
            }
        }

        private void MyComboBox_DropDownClosed(object sender, EventArgs e)
        {
            base.Width = this.RealWidth;
        }

        private void MyComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsTextChanging && base.IsEditable && base.SelectedItem != null && Operators.CompareString(this.Text, base.SelectedItem.ToString(), false) != 0)
            {
                string text = this.Text;
                int selectionStart = this.TextBox.SelectionStart;
                this.IsTextChanging = true;
                base.SelectedItem = null;
                this.Text = text;
                this.TextBox.SelectionStart = selectionStart;
                this.IsTextChanging = false;
            }
        }

        public ContentPresenter ContentPresenter
        {
            get
            {
                return (ContentPresenter)base.Template.FindName("PART_Content", this);
            }
        }

        private void MyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
            {
                this.RaiseCustomEvent();
            }
        }

        void ISettingControl.RefreshSetting(string NewValue)
        {
            DependencyObject dependencyObject = base.Items.OfType<DependencyObject>().FirstOrDefault<DependencyObject>((DependencyObject i) => ModBase.Val(RuntimeHelpers.GetObjectValue(SettingService.GetValue(i) ?? (-1))) == Conversions.ToDouble(NewValue));
            if (dependencyObject != null)
            {
                base.SelectedItem = dependencyObject;
                return;
            }
            if ((double)base.Items.Count > Conversions.ToDouble(NewValue))
            {
                base.SelectedIndex = Conversions.ToInteger(NewValue);
                return;
            }
            Logger.Warn(string.Format("尝试显示设置 {0}，但没有找到设置值 {1} 的对应项", SettingService.GetKey(this), NewValue), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyComboBox.vb");
            base.SelectedIndex = -1;
            base.SelectedItem = null;
        }

        string ISettingControl.GetCurrentSetting()
        {
            string text;
            if (base.SelectedItem == null)
            {
                text = null;
            }
            else if (base.SelectedItem is DependencyObject)
            {
                text = Conversions.ToString(SettingService.GetValue((DependencyObject)base.SelectedItem) ?? base.SelectedIndex);
            }
            else
            {
                text = Conversions.ToString(base.SelectedIndex);
            }
            return text;
        }

        private void SaveSetting()
        {
            if (base.SelectedItem != null)
            {
                SettingService.SaveSetting(this);
            }
        }

        public int Uuid;

        private MyTextBox TextBox;

        private string _Text;

        private bool IsMouseDown;

        private double RealWidth;

        private bool IsTextChanging;

        public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);
    }
}
